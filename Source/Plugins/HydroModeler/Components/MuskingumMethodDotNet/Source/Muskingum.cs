#region MetaData
//----- Component Description -----
//This component computes the routed streamflow through a channel network.

//----- Input Files -----
//  This component requires 2 input files, a config.xml file and an elementset.shp that is referenced within
//the configuration file.  All input parameters must be included within one shapefile.  Each row of the
//attribute table represents an individual reach and must contain the following columns.
//
//      GridID = The id of each reach.  Used to ensure that element sets align properly.  This value should
//               correspond with the ID's of the input element set.                 
//      FROM_NODE = The start point ID of the reach (same as GridID)
//      TO_NODE = The end point ID of the reach
//      K = the Proportionality Coefficient approximated as the time of travel of the flood wave through the reach
//      X = Wedge storage weighting factor, X = 0 for reservoir-type storage, X = 0.5 for 'full' wedge storage.  0.0 <= X <= 0.5
//
#endregion

//--- System Assemblies ----
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using System.Xml;
//--- OpenMI Assemblies ----
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;
//--- Project Assemblies ----
using SMW;                      
//--- SharpMap Assemblies ----
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;
//--- QuickGraph Assemblies ---
using QuickGraph;


namespace edu.SC.Models.Routing
{
    public class Muskingum : Wrapper
    {

        //--- GLOBAL VARIABLES ---
        public DataTable elementValues = new DataTable();
        public string input_quantity;
        public string output_quantity;
        public string input_elementset;
        public string output_elementset;
        double _ts;
        private Dictionary<string, double> routed_outflow = new Dictionary<string, double>();
        ArrayList vals = new ArrayList();
        ArrayList Datetime = new ArrayList();
        System.IO.StreamWriter sw;
        string _outDir = null; 

        Dictionary<int, ArrayList> reachOrder = new Dictionary<int, ArrayList>();

        Dictionary<DateTime, double[]> _output = new Dictionary<DateTime, double[]>();


        IEnumerable<Int64> _topoSort;

        double[] transformedInflow;

        AdjacencyGraph<Int64, TaggedEdge<Int64, Reach>> g = new AdjacencyGraph<Int64, TaggedEdge<Int64, Reach>>();
        

        /// <summary>
        /// This method performs specified actions upon the closure of the model
        /// </summary>
        public override void Finish()
        {

            //intialize streamwriter to write output data. 
            if (_outDir != null)
            {
                try
                { sw = new System.IO.StreamWriter(_outDir + "/MuskingumRouting_output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception("The Muskingum Component was unable to create the desired output file. " +
                                         "This is possibly due to an invalid \'OutDir\' field supplied in " +
                                         "the *.omi file", e);
                }
            }
            else
            {
                try { sw = new System.IO.StreamWriter("../MuskingumRouting_output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception(" The Muskingum component failed in writing it output file to path " +
                     System.IO.Directory.GetCurrentDirectory() + ". This may be due to " +
                     "lack of user permissions.", e);
                }
            }



            // current time
            TimeStamp time = (TimeStamp)this.GetCurrentTime();
            DateTime current = CalendarConverter.ModifiedJulian2Gregorian(time.ModifiedJulianDay);
            Double step_sec = this.GetTimeStep();
            //get the last time
            current = current.AddSeconds(-1*step_sec); 

            sw.Write("Date,");
            for (int i = 1; i <= _output[current].Length; i++)
                sw.Write("Element " + i.ToString() + ",");
            sw.Write("\n");


            foreach (KeyValuePair<DateTime, double[]> kvp in _output)
            {
                sw.Write(kvp.Key.ToShortDateString() +" "+ kvp.Key.ToLongTimeString() + ",");
                for (int j = 0; j < kvp.Value.Length; j++)
                    sw.Write(kvp.Value[j].ToString() + ",");
                sw.Write("\n");

            }
            sw.Close();

        }

        /// <summary>
        /// Used to Initialize the component.  Performs routines that must be completed prior to simulation start.
        /// </summary>
        /// <param name="properties">properties extracted from the components *.omi file</param>
        public override void Initialize(System.Collections.Hashtable properties)
        {
            //--- GET MODEL ATTRIBUTES FROM SMW ---

            string config = null;

            //set the config path and output directory from the *.omi file.
            foreach (DictionaryEntry p in properties)
            {
                if (p.Key.ToString() == "ConfigFile")
                    config = (string)properties["ConfigFile"];
                else if (p.Key.ToString() == "OutDir")
                    _outDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), (string)properties["OutDir"]);
            }

            //lookup model's configuration file to determine interface properties
            SetVariablesFromConfigFile(config);
            SetValuesTableFields();

            //input exchange item attributes
            int num_inputs = this.GetInputExchangeItemCount();
            InputExchangeItem input = this.GetInputExchangeItem(num_inputs - 1);
            input_elementset = input.ElementSet.ID;
            input_quantity = input.Quantity.ID;
            
            //output exchange item attributes
            int num_outputs = this.GetOutputExchangeItemCount();
            OutputExchangeItem output = this.GetOutputExchangeItem(num_outputs - 1);
            output_elementset = output.ElementSet.ID;
            output_quantity = output.Quantity.ID;
            
            //timehorizon attributes
            ITimeSpan time_horizon = this.GetTimeHorizon();
            string start = CalendarConverter.ModifiedJulian2Gregorian(time_horizon.Start.ModifiedJulianDay).ToString();
            string end = CalendarConverter.ModifiedJulian2Gregorian(time_horizon.End.ModifiedJulianDay).ToString();
            
            //get shapefile path
            string shapefilePath = this.GetShapefilePath();
            
            //get timestep
            string timestep = this.GetTimeStep().ToString();
            _ts = Convert.ToDouble(timestep);

            //this uses the free SharpMap API for reading a shapefile
            VectorLayer myLayer = new VectorLayer("elements_layer");
            myLayer.DataSource = new ShapeFile(shapefilePath);
            myLayer.DataSource.Open();

            //initialize array to hold the transformed inflow values
            int size = myLayer.DataSource.GetFeatureCount();
            transformedInflow = new double[size];

            //--- BUILD NETWORK ---
            // loop through all features in feature class
            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); ++i)
            {
                FeatureDataRow feat = myLayer.DataSource.GetFeature(i);


                //This routine, ensures that the reaches are added to the XML stream such that the the elements
                // align with the subwatersheds.shp.  In order to do this I had to assign ArcID's that match the
                // corresponding Subwatershed.shp elements.
                //bool IsCorrect = false;

                //uint j = 0;
                //while (!IsCorrect)
                //{
                //    feat = myLayer.DataSource.GetFeature(j);
                //    if (feat.ItemArray[feat.Table.Columns.IndexOf("GridID")].ToString() == (i + 1).ToString())
                //        IsCorrect = true;
                //    else
                //        j++;
                //}



                string to_comid = feat.ItemArray[feat.Table.Columns.IndexOf("TO_NODE")].ToString();
                string from_comid = feat.ItemArray[feat.Table.Columns.IndexOf("FROM_NODE")].ToString();
                string k = feat.ItemArray[feat.Table.Columns.IndexOf("K")].ToString();
                string x = feat.ItemArray[feat.Table.Columns.IndexOf("X")].ToString();
                string id = feat.ItemArray[feat.Table.Columns.IndexOf("GridID")].ToString();

                Int64 v1 = Convert.ToInt64(from_comid);
                Int64 v2 = Convert.ToInt64(to_comid);
                    
                
                g.AddVertex(v1);
                g.AddVertex(v2);

                var e1 = new TaggedEdge<Int64, Reach>(v1, v2, new Reach(Convert.ToInt64(id), Convert.ToDouble(x), Convert.ToDouble(k), Convert.ToDateTime(start), Convert.ToDouble(timestep)));
                g.AddEdge(e1);
            }

            //sort the graph topologically
            _topoSort = QuickGraph.Algorithms.AlgorithmExtensions.TopologicalSort<Int64, TaggedEdge<Int64, Reach>>(g);



        }

        public override bool PerformTimeStep()
        {
            //Get input streamflow 
            ScalarSet streamflow = (ScalarSet)this.GetValues(input_quantity, input_elementset);

            //transform streamflow to double[]
            double[] inflow_vals = streamflow.data;

            //initialize the flow at the outlet to zero
            double flow_at_outlet = 0.0;


            //get the current time
            TimeStamp time = (TimeStamp) this.GetCurrentTime();
            DateTime current = CalendarConverter.ModifiedJulian2Gregorian(time.ModifiedJulianDay);
            
            //get the time when calc's will be performed
            
            current = current.AddSeconds(_ts);
            
            //round to the nearest 5min interval
            current = new DateTime(((current.Ticks + 25000000) / 50000000) * 50000000);

            //get enumerator of source nodes for the topologically sorted graph
            IEnumerator topoEnum = _topoSort.GetEnumerator();

            

            #region Set Inflows

            //loop through all of the reaches in the order they are sorted
            while (topoEnum.MoveNext())
            {
                //loop through all of the edges in the graph
                foreach (TaggedEdge<Int64, Reach> edge in g.Edges)
                {
                    //check if the source node (unsorted graph) of the edge equals the current source node of the sorted graph
                    if (edge.Source == Convert.ToInt64(topoEnum.Current))
                    {
                        int sourceID = 0;

                        //loop through all of the edges of the graph again
                        foreach (TaggedEdge<Int64, Reach> downstreamEdge in g.Edges)
                        {
                            //find the edge in which the source node equals the current edge's target node
                            if (downstreamEdge.Source == edge.Target)
                            {
                                //get the index of the target (based on the unsorted graph)to determine the input value 
                                //  corresponding with the edge.Target edge.
                                int index = 0;
                                foreach (TaggedEdge<Int64, Reach> r in g.Edges)
                                {
                                    if (r.Source == edge.Source)
                                        break;
                                    index++;
                                }

                                //apply the inflow to the downstream edge
                                if (downstreamEdge.Tag.inflow.ContainsKey(current))
                                {
                                    //HACK:  This assumes that reach ID's are 1-based
                                    //downstreamEdge.Tag.inflow[current] += inflow_vals[edge.Tag.id - 1];

                                    downstreamEdge.Tag.inflow[current] += inflow_vals[index];
                                }
                                else
                                {
                                    //HACK:  This assumes that reach ID's are 1-based
                                    //downstreamEdge.Tag.inflow[current] = inflow_vals[edge.Tag.id - 1];

                                    downstreamEdge.Tag.inflow.Add(current,inflow_vals[index]);
                                }

                                //break the loop
                                break;
                            }
                            sourceID++;

                            //if no downstream edges are found, this edge must connect to the outlet
                            if (sourceID == inflow_vals.Length)
                            {
                                //get the index (based on the unsorted graph)to determine the correct input value
                                int index = 0;
                                foreach (TaggedEdge<Int64, Reach> r in g.Edges)
                                {
                                    if (r.Source == edge.Source)
                                        break;
                                    index++;
                                }

                                //flow_at_outlet += inflow_vals[edge.Tag.id-1];
                                flow_at_outlet += inflow_vals[index];

                                break;
                            }
                        }

                        //break the loop
                        break;
                    }

                    //index++;
                    
                }

            }
            #endregion

            #region Route Flows

            //reset the sorted graph
            topoEnum.Reset();

            while(topoEnum.MoveNext())
            {
                //---- ROUTE THE FLOW DOWN THIS REACH ----
                TaggedEdge<Int64, Reach> e = null;

                //find the edge that correspondes to this source node
                foreach (TaggedEdge<Int64, Reach> edge in g.Edges)
                {
                    //get the source node
                    if (edge.Source == Convert.ToInt64(topoEnum.Current))
                    {
                        e = edge;
                        break;
                    }
                }

                //no downstream edge found (outlet condition)
                if (e == null)
                    break;

                #region Muskingum Computation

                //get inflow from previous time step
                double In0 = e.Tag.inflow[current.AddSeconds(-1.0 * _ts)];

                //check that this inflow is not negative
                if(e.Tag.inflow.ContainsKey(current))
                {
                    if (e.Tag.inflow[current] < 0)
                        e.Tag.inflow[current] = 0;
                }
                else
                {
                    e.Tag.inflow[current] = 0.0;
                }

                ////////check that this inflow is not negative
                //////try
                //////{
                    
                //////    if (e.Tag.inflow[current] < 0)
                //////        e.Tag.inflow[current] = 0;
                //////}
                //////catch (KeyNotFoundException)
                //////{
                //////    e.Tag.inflow[current] = 0.0;
                //////}

                //get the inflow from the current timestep
                double In1 = e.Tag.inflow[current];

                //get outflow from previous time step
                double Out0 = e.Tag.outflow[current.AddSeconds(-1.0 * _ts)];

                //TODO: write to log if the calculated flow is unstable
                //calculate outflow for current time step
                double Out1 = e.Tag.C1 * In1 + e.Tag.C2 * In0 + e.Tag.C3 * Out0;

                //store calculated outflow in the Reach class
                e.Tag.outflow[current] = Out1;

                #endregion

                #region Add this flow to downstream reach

                Int64 targetNode = e.Target;

                TaggedEdge<Int64, Reach> outEdge = null;

                //get outflow edge
                foreach (TaggedEdge<Int64, Reach> edge in g.Edges)
                {
                    if (edge.Source == targetNode)
                    {
                        //set the out edge
                        outEdge = edge;

                        //break the loop
                        break;

                    }
                }

                //if outEdge == null then the current reach has no successors
                if (outEdge != null)
                {
                    //HACK: this is taken from the other Muskingum component
                    //outEdge.Tag.inflow[current] = In1;

                    //apply outflow to downstream reach
                    if(outEdge.Tag.inflow.ContainsKey(current))
                    {

                        outEdge.Tag.inflow[current] += Out1;
                    }
                    else
                    {
                        outEdge.Tag.inflow[current] = Out1;
                    }

                    ////////apply outflow to downstream reach
                    //////try
                    //////{

                    //////    outEdge.Tag.inflow[current] += Out1;
                    //////}
                    //////catch (SystemException)
                    //////{
                    //////    outEdge.Tag.inflow[current] = Out1;
                    //////}
                }
                //downstream must be the outlet
                else
                {
                    flow_at_outlet += Out1;
                }

                //Set the current inflow for this reach equal to the inflow from the previos timestep
                //save the previous inflow value
                e.Tag.inflow[current.AddSeconds(-1*_ts)] = In1;

            }
            #endregion


            #endregion

            #region Save the Outflows
            //Parse outflow_stream into double[] in the order of the input file
            topoEnum.Reset();

            double[] outflows = new double[g.EdgeCount];
            while (topoEnum.MoveNext())
            {

                //find corresponding edge index
                int index = 0;
                TaggedEdge<Int64, Reach> edge = null;
                foreach (TaggedEdge<Int64, Reach> e in g.Edges)
                {
                    if (e.Source == Convert.ToInt64(topoEnum.Current))
                    {
                        //set the edge
                        edge = e;
                        break;
                    }
                    index++;
                }

                if (index == outflows.Length)
                {
                    int index2 = 0;
                    TaggedEdge<Int64, Reach> edge2 = null;

                    //find the reach the goes to the outlet
                    foreach (TaggedEdge<Int64, Reach> e in g.Edges)
                    {
                        if (e.Target == Convert.ToInt64(topoEnum.Current))
                        {
                            //set the edge
                            edge2 = e;
                            break;
                        }
                        index2++;
                    }
                    //HACK
                    //outflows[index2] = flow_at_outlet;
                }
                else
                {
                    try
                    {
                        outflows[index] = edge.Tag.outflow[current];
                    }
                    catch (SystemException) 
                    {
                        throw new Exception("Stop");
                    }
                }

            }
            #endregion

            //HACK:  Negative values should be reported so that the modeler knows that the simulation is unstable
            //set any negative flow values equal to zero
            for(int o = 0; o<=outflows.Length-1;o++)
                if (outflows[o] < 0) { outflows[o] = 0; }

            //set the values
            this.SetValues(output_quantity, output_elementset, new ScalarSet(outflows));



            //--- save values to output them in the Finish() method ---

            //get current time and convert into UTC time
            TimeStamp currenttime = (TimeStamp)this.GetCurrentTime();
            DateTime modTime = CalendarConverter.ModifiedJulian2Gregorian(currenttime.ModifiedJulianDay);
            _output[modTime] = outflows; 

            //Advance time
            this.AdvanceTime();
            return true;
        }
    }

    class Reach
    {
        public Int64 id;
        public double x;
        public double k;
        public double C1;
        public double C2;
        public double C3;
        public Dictionary<DateTime, double> inflow;
        public Dictionary<DateTime, double> outflow;

        public Reach(Int64 ID, double X, double K, DateTime start, double timeStepSec)
        {
            this.x = X;
            this.k = K;
            this.C1 = (timeStepSec / 3600.0 - 2 * this.k * this.x) / (2 * this.k * (1 - this.x) + timeStepSec / 3600.0);
            this.C2 = (timeStepSec / 3600.0 + 2 * this.k * this.x) / (2 * this.k * (1 - this.x) + timeStepSec / 3600.0);
            this.C3 = (2 * this.k * (1 - this.x) - timeStepSec / 3600.0) / (2 * this.k * (1 - this.x) + timeStepSec / 3600.0);
            this.inflow = new Dictionary<DateTime, double>();
            this.outflow = new Dictionary<DateTime, double>();
            this.inflow[start] = 0.0;
            this.outflow[start] = 0.0;
            this.id = ID;
        }

    }
}
