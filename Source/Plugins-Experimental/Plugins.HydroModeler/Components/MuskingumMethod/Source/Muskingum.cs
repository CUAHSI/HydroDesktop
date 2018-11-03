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
//----- Additional Notes -----
//  This model compontent is currently designed for elementset.shp's that contain only 1 outlet.  The main
//computations occur within the Muskingum Routing Web Service.  This script mainly manages sending input
//values to the web service, retriving results, and outputing results.  The Muskingum Routing web service 
//is written in python and utilizes an XML-RPC type web service.
#endregion

#define DEBUG
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
//--- Web Service Assemblies ----
using CookComputing.XmlRpc;
//--- SharpMap Assemblies ----
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;
//--- Debugging Assemblies ----
using System.Diagnostics;
//--- SQL ----
using System.Data.SqlClient;

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
        MuskingumPY model = XmlRpcProxyGen.Create<MuskingumPY>();
        private Dictionary<string, double> routed_outflow = new Dictionary<string, double>();
        ArrayList vals = new ArrayList();
        ArrayList Datetime = new ArrayList();
        System.IO.StreamWriter sw;
        string _outDir = null;
        Dictionary<int, ArrayList> reachOrder = new Dictionary<int, ArrayList>();

        double[] transformedInflow;

        //System.IO.StreamWriter sw2;
        
        /// <summary>
        /// This method performs specified actions upon the closure of the model
        /// </summary>
        public override void Finish()
        { 
            sw.Close();
            //sw2.Close();
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
                    _outDir = (string)properties["OutDir"];


            }

            //lookup model's configuration file to determine interface properties
            SetVariablesFromConfigFile(config);
            SetValuesTableFields();

            //input exchange item attributes
            int num_inputs = this.GetInputExchangeItemCount();
            InputExchangeItem input = this.GetInputExchangeItem(num_inputs -1);
            input_elementset = input.ElementSet.ID;
            input_quantity = input.Quantity.ID;
            //output exchange item attributes
            int num_outputs = this.GetOutputExchangeItemCount();
            OutputExchangeItem output = this.GetOutputExchangeItem(num_outputs-1);
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

            //this uses the free SharpMap API for reading a shapefile
            VectorLayer myLayer = new VectorLayer("elements_layer"); 
            myLayer.DataSource = new ShapeFile(shapefilePath);
            myLayer.DataSource.Open();

            //initialize array to hold the transformed inflow values
            int size = myLayer.DataSource.GetFeatureCount();
            transformedInflow = new double[size];


            //--- BUILD XML STRING TO INITIALIZE THE WEB SERVICE ---

            string reaches = "<elementset>";
            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); ++i)
            {
                FeatureDataRow feat = myLayer.DataSource.GetFeature(i);

                //TODO: eliminate this, by spatially referencing the elements
                //Get the correct ArcID to ensure that the elementset will align correctly

                //This routine, ensures that the reaches are added to the XML stream such that the the elements
                // align with the subwatersheds.shp.  In order to do this I had to assign ArcID's that match the
                // corresponding Subwatershed.shp elements.
                bool IsCorrect = false;

                uint j = 0;
                while (!IsCorrect)
                {
                    feat = myLayer.DataSource.GetFeature(j);
                    if (feat.ItemArray[0].ToString() == (i + 1).ToString())
                        IsCorrect = true;
                    else
                        j++;
                }

                //Add the FROM attribute of each reach, in the order that  
                // ---> they are established within the xml string
                ArrayList attributes = new ArrayList();
                attributes.Add(feat.ItemArray[feat.Table.Columns.IndexOf("TO_COMID")].ToString());
                attributes.Add(feat.ItemArray[feat.Table.Columns.IndexOf("FROM_COMID")].ToString());

                //Add i to the reachOrder array. This will index the order in which values are retrieved
                // --> from the upstream component.
                reachOrder.Add(Convert.ToInt32(i), attributes);

                //Add shapefile attributes into the xml string
                reaches += "<element>";
                reaches += "<From>" + feat.ItemArray[feat.Table.Columns.IndexOf("FROM_COMID")].ToString() + "</From>";
                reaches += "<To>" + feat.ItemArray[feat.Table.Columns.IndexOf("TO_COMID")].ToString() + "</To>";
                reaches += "<K>" + feat.ItemArray[feat.Table.Columns.IndexOf("K")].ToString() + "</K>"; //TravelTime in hours
                reaches += "<X>" + feat.ItemArray[feat.Table.Columns.IndexOf("X")].ToString() + "</X>";
                reaches += "</element>";
            }
            reaches += "<TimeHorizon>";
            reaches += "<TimeStepInSeconds>" + timestep + "</TimeStepInSeconds>";
            reaches += "<StartDateTime>" + start + "</StartDateTime>";
            reaches += "<EndDateTime>" + end + "</EndDateTime>";
            reaches += "</TimeHorizon></elementset>";


            //--- INITIALIZE THE WEB SERVICE ---

            bool init = model.initialize(reaches);
            
            if (init == false)
                throw new Exception("The Muskingum Web Service Failed to Initialize");


            //Create a streamwriter to save results from Perform Time Step
            if (_outDir != null)
            {
                try
                { sw = new System.IO.StreamWriter(_outDir + "/MuskingumRouting_output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception("The Muskingum (Python) Component was unable to create the desired output file. " +
                                         "This is possibly due to an invalid \'OutDir\' field supplied in " +
                                         "the *.omi file", e);
                }
            }
            else
            {
                try { sw = new System.IO.StreamWriter("../MuskingumRouting_output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception(" The Muskingum (Python) component failed in writing it output file to path " +
                     System.IO.Directory.GetCurrentDirectory() + ". This may be due to " +
                     "lack of user permissions.", e);
                }
            }



        }

        public override bool PerformTimeStep()
        {
            //--- GET INPUT VALUES ---

            //Get input streamflow 
            ScalarSet streamflow = (ScalarSet)this.GetValues(input_quantity, input_elementset);

            //transform streamflow to double[]
            double[] inflow_vals = streamflow.data;


            //--- TRANSFORM INPUT VALS TO BE APPLIED TO THEIR DOWNSTREAM CATCHMENT ---

            //initialize array (to hold transformed values) to zero
            int index=-999;
            for (int j = 0; j <= transformedInflow.Length - 1; j++)
                transformedInflow[j] = 0.0;

            //object to hold any input values that are applied directly to the outlet (i.e. no routing)
            double outlet = 0.0;

            for (int j = 0; j <= inflow_vals.Length - 1; j++)
            {
                //determine the "TO_COMID" for the reach corresponding to the first input element
                string toID = Convert.ToString(reachOrder[j][0]);

                //determine the index of the reach corresponding to this "TO_COMID"

                //This checks the case where their is no downstream element (reach outlet is dennoted as TO_COMID = -1)
                if (toID.ToUpper() == "-1")
                {
                    //save outlet value
                    outlet += inflow_vals[j];
                }
                else
                {
                    //search for a "FROM_COMID" that matches this "TO_COMID"
                    foreach (KeyValuePair<int, ArrayList> kvp in reachOrder)
                    {
                        if (Convert.ToString(kvp.Value[1]) == toID)
                        {
                            index = kvp.Key;
                            break;
                        }
                    }
                    //Transform this value, by adding it to the index of the "TO_COMID"
                    if(index != -999)
                        transformedInflow[index] += inflow_vals[j];
                }  
            }

            ////TODO: Remove
            //foreach (Double inflow in transformedInflow)
            //    sw2.Write(inflow.ToString() + ",");
            //sw2.Write("\n");

            //Build inflow xml stream to send to the Web Service
            string inflow_stream = "<inflow>";
            foreach (Double inflow in transformedInflow)
                inflow_stream += "<reach>" + inflow.ToString() + "</reach>";
            inflow_stream += "</inflow>";


            //--- PERFORM COMPUTATION BY CALLING THE WEBSERVICE ---

            //Send xml stream to web service, and Perform Computation
            string outflow_stream = model.performTimeStep(inflow_stream);

            //Parse outflow_stream into double[]
            double[] outflow = FromXML(outflow_stream);

            //add values that are applied directly to the outlet
            if (outflow.Length == 1)
                outflow[0] += outlet;
            else
                throw new Exception("Error on line 253 of Muskingum Routing, resulting from multiple outlets");

            //Save calculated values so they can be retrieved by a downstream component
            this.SetValues(output_quantity, output_elementset, new ScalarSet(outflow));


            //--- PRINT RESULTS ---

            TimeStamp tt = (TimeStamp)this.GetCurrentTime();
            DateTime TT = CalendarConverter.ModifiedJulian2Gregorian(tt.ModifiedJulianDay);
            sw.Write(TT.ToLongTimeString() + ",");
            foreach (double Out in outflow)
            {
                sw.Write(Out.ToString() + ",");
                vals.Add(Out);
                //get current time and convert into UTC time
                TimeStamp t = (TimeStamp)this.GetCurrentTime();
                DateTime T = CalendarConverter.ModifiedJulian2Gregorian(t.ModifiedJulianDay);
                Datetime.Add(T);
            }
            sw.Write("\n");

            //Advance time
            this.AdvanceTime();
            return true;
        }

        #region Auxilary Methods

        /// <summary>
        /// Extracts the output values from the Web Service's XML stream
        /// </summary>
        /// <param name="xml_stream">the output stream from the Web Service's PerformTimeStep method</param>
        /// <returns>an array containing the values calculated by the Web Service</returns>
        public double[] FromXML(string xml_stream)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(new StringReader(xml_stream));
            XmlElement elements = xmldoc.DocumentElement;
            XmlNodeList children = elements.ChildNodes;
            double[] vals = new double[children.Count];
            int i = 0;
            foreach (XmlNode child in children)
                vals[i] = Convert.ToDouble(child.InnerText); i++;

            return vals;
        }
 
        #endregion


    }



    /// <summary>
    /// This routine initializes the webservice and defines the three methods within it.
    /// </summary>
    [XmlRpcUrl("http://localhost:8000/RPC2")]
    public interface MuskingumPY : IXmlRpcProxy
    {
        [XmlRpcMethod("finalize")]
        bool finalize();

        //Returns true if it has completed initialization
        [XmlRpcMethod("initialize")]
        bool initialize(string XmlStream_reaches);

        //TODO: return the outflow at all reach outlets in the system
        //Returns the outflow at the reach outlet
        [XmlRpcMethod("performTimeStep")]
        string performTimeStep(string inflow);

        
    }
}
