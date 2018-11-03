#region MetaData
//----- Component Description -----
//This component computes watershed infiltration, by using the SCS Curve Numer Abstraction Procedure.

//----- Input Files -----
//  This component requires 2 input files, a config.xml file and an elementset.shp that is referenced within
//the configuration file.  All input parameters must be included within one shapefile.  Each row of the
//attribute table represents a single sub-catchment and must contain the following columns.
//
//      Watershed = Watershed ID number
//      CN = Weighted Curve Number of watershed


//----- Additional Notes -----
//This model outputs the excess precipitation for each watershed element defined within the element set
//shapefile, resulting from a given rainfall event

#endregion
using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Layers;
using SharpMap.Data;
using System.Data;
using Oatc.OpenMI.Sdk.Backbone;
using SharpMap.Data.Providers;
using System.Xml;
using System.Diagnostics;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard;
using System.Collections;

namespace edu.SC.Models.Infiltration
{
    public class SCSAbstractionMethod : SMW.Wrapper
    {

        //stores model element properties
        public DataTable elementValues = new DataTable();
        System.Collections.ArrayList in_vals = new System.Collections.ArrayList();
        System.Collections.ArrayList out_vals = new System.Collections.ArrayList();
        System.Collections.ArrayList times = new System.Collections.ArrayList();
        public string input_quantity;
        public string output_quantity;
        public string input_elementset;
        public string output_elementset;
        private int _featureCount;
        double[] PeArray_last;
        System.IO.StreamWriter writer;
        string _outDir = null;

        public override void Finish()
        {

            //intialize streamwriter to write output data. 
            if (_outDir != null)
            {
                try
                { writer = new System.IO.StreamWriter(_outDir + "/SCSAbstraction_output.csv"); }
                catch (SystemException e) {
                    throw new Exception("The Cn Component was unable to create the desired output file. "+
                                         "This is possibly due to an invalid \'OutDir\' field supplied in "+
                                         "the *.omi file", e);}
            }
            else
            {
                try { writer = new System.IO.StreamWriter("../SCSAbstraction_output.csv"); }
                catch (SystemException e) {throw new Exception(" The Cn component failed in writing it output file to path "+
                                            System.IO.Directory.GetCurrentDirectory() + ". This may be due to "+
                                            "lack of user permissions.",e); }
            }

            //Write output data

            writer.WriteLine("Precip In");
            writer.Write("Date, Time, ");

            for (int i = 0; i <= _featureCount-1; i++)
            {
                writer.Write("element " + (i + 1).ToString() + ",");
            }
            writer.Write("\n");


            for (int i = 0; i <= times.Count - 1; i++)
            {
                writer.Write(times[i].ToString().Split(' ')[0] + 
                             "," + times[i].ToString().Split(' ')[1] + ",");

                for (int j = 0; j <= _featureCount - 1; j++)
                {
                    writer.Write(in_vals[_featureCount*i + j] +",");
                }
                writer.Write("\n");

                
            }

            writer.WriteLine("\n\n Excess Precip Out");
            writer.Write("Date, Time,");
            for (int i = 0; i <= _featureCount - 1; i++)
            {
                writer.Write("element " + (i + 1).ToString() + ",");
            }
            writer.Write("\n");

            for (int i = 0; i <= times.Count - 1; i++)
            {
                writer.Write(times[i].ToString().Split(' ')[0] +
                             "," + times[i].ToString().Split(' ')[1] + ",");

                for (int j = 0; j <= _featureCount - 1; j++)
                {
                    writer.Write(out_vals[_featureCount * i + j] + ",");
                }
                writer.Write("\n");
            }
            
            writer.Close();

        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string configFile = null;

            //set the config path and output directory from the *.omi file.
            foreach (DictionaryEntry p in properties)
            {
                if(p.Key.ToString() == "ConfigFile")
                    configFile = (string)properties["ConfigFile"];
                else if (p.Key.ToString() == "OutDir")
                    _outDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), (string)properties["OutDir"]);
            }

            

            //lookup model's configuration file to determine interface properties
            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            //setup elementValues DataTable to store element attributes
            elementValues.Columns.Add("Watershed");
            elementValues.Columns.Add("CurveNumber");
            elementValues.Columns.Add("cumInfil");
            elementValues.Columns.Add("CumPrecip",typeof(double));
            elementValues.Columns.Add("HyetoVal", typeof(double));


            //get input exchange item attributes
            int num_inputs = this.GetInputExchangeItemCount();
            InputExchangeItem input = this.GetInputExchangeItem(num_inputs - 1);
            input_elementset = input.ElementSet.ID;
            input_quantity = input.Quantity.ID;

            //get output exchange item attributes
            int num_outputs = this.GetOutputExchangeItemCount();
            OutputExchangeItem output = this.GetOutputExchangeItem(num_outputs - 1);
            output_elementset = output.ElementSet.ID;
            output_quantity = output.Quantity.ID;
            

            // Get shapefile path from Config file
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlElement root = doc.DocumentElement;
            XmlNode elementSet = root.SelectSingleNode("//InputExchangeItem//ElementSet");
            string shapefilePath = elementSet["ShapefilePath"].InnerText;


            //open the input shapefile
            VectorLayer myLayer = new VectorLayer("elements_layer"); //this uses the free SharpMap API for reading a shapefile
            myLayer.DataSource = new ShapeFile(shapefilePath);
            myLayer.DataSource.Open();

            //get the number of features in the shapefile
            _featureCount = myLayer.DataSource.GetFeatureCount();

            //add feature attributes from shapefile to datatable
            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); ++i)
            {
                FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
                DataTable att = feat.Table;
                
                object Watershed = feat.ItemArray[feat.Table.Columns.IndexOf("Watershed")];
                object CN = feat.ItemArray[feat.Table.Columns.IndexOf("CN")];
                elementValues.LoadDataRow(new object[] { Watershed, CN }, true);
            }

            //initialize PeArray_last to store the Pe from the previous timestep
            PeArray_last = new double[elementValues.Rows.Count];
            for (int k = 0; k <= PeArray_last.Length - 1; k++)
                PeArray_last[k] = 0.0;

            ////intialize streamwriter to write output data.  This is used in the Finish() method
            //writer = new System.IO.StreamWriter("../SCSAbstraction_output.csv");
        }

        public override bool PerformTimeStep()
        {

            // --- request precipitation on a 5min (300 sec) interval ----
            ScalarSet precip = (ScalarSet)this.GetValues(input_quantity, input_elementset);

            if (precip.Count != 0)
            {
                //load the precip values into Cum-Precip column of "elementValues"
                for (int k = 0; k <= precip.Count - 1; k++)
                {
                    in_vals.Add(precip.data[k]);

                    //calculate the cumulative precipitation
                    try
                    {
                        elementValues.Rows[k]["CumPrecip"] = Convert.ToDouble(elementValues.Rows[k]["CumPrecip"]) + Math.Round(precip.data[k],5);
                    }
                    catch (System.InvalidCastException)
                    { elementValues.Rows[k]["CumPrecip"] = precip.data[k]; }

                }

                double[] PeArray = new double[elementValues.Rows.Count];

                // --- loop through all elements
                for (int i = 0; i <= elementValues.Rows.Count - 1; i++)
                {
                    //get element attributes
                    object[] itemArray = elementValues.Rows[i].ItemArray;
                    double CN = Math.Round(Convert.ToDouble(itemArray[elementValues.Columns.IndexOf("CurveNumber")]),0);
                    double cumInfil = 0;

                    if (!String.IsNullOrEmpty(Convert.ToString(itemArray[elementValues.Columns.IndexOf("cumInfil")])))
                    {
                        cumInfil = Convert.ToDouble(itemArray[elementValues.Columns.IndexOf("cumInfil")]);
                    }


                    //get precip
                    double P = Convert.ToDouble(elementValues.Rows[i]["CumPrecip"]);
                    //set excess precip to zero (initially)
                    double Pe = 0.0;
                    //set continuing abstraction to zero (initially)
                    double Fa = 0;  

                    //calculate S based on element's CN attribute
                    double S = (1000 / CN) - 10;
                    //calculate initial abstraction
                    double Ia = Math.Round(0.2 * S,2);

                    // --- Calculate Pe and cumInfil
                    if (P >= Ia)
                    {
                        //Calculate Continuing abstractions
                        Fa = (S * (P - Ia)) / (P - Ia + S);
                        //Calculate Excess Precipitation
                        Pe = P - Ia - Fa;
                        //cumInfil = Fa;
                        cumInfil = Math.Round(Fa, 5);
                    }
                    else
                    {
                        //All rainfall will infiltrate
                        cumInfil = Math.Round(P, 5);
                    }

                    //store Pe
                    if (Pe >= 0)
                        PeArray[i] = Pe;
                    else
                        PeArray[i] = 0;

                    // --- update cummulative infiltration in elementValues DT
                    itemArray[elementValues.Columns.IndexOf("CumInfil")] = cumInfil;
                    elementValues.Rows[i].ItemArray = itemArray;
                }


                // --- Calculate the Excess Hyetograph ----

                DateTime timecheck = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetCurrentTime()).ModifiedJulianDay);

                double[] Hyeto = new double[PeArray.Length];
                for (int i = 0; i <= PeArray.Length - 1; i++)
                {
                    //calc hyeto value
                    Hyeto[i] = PeArray[i] - PeArray_last[i];

                    //if hyeto value is negative, then set it to zero
                    if (Hyeto[i] < 0.0)
                        Hyeto[i] = 0.0;

                    //store this value for the next timestep within PeArray_last
                    PeArray_last[i] = PeArray[i];

                    //set this value within the datatable
                    elementValues.Rows[i]["HyetoVal"] = Hyeto[i];

                    //store this value so that it can be written to file within the Finish() method
                    out_vals.Add(Hyeto[i]);
                }

                //Store the current time, for when writing output values within the Finish() method
                TimeStamp t = (TimeStamp)this.GetCurrentTime();
                DateTime T = CalendarConverter.ModifiedJulian2Gregorian(t.ModifiedJulianDay);
                times.Add(T);

                // --- set the excess precip values 
                this.SetValues(output_quantity, output_elementset, new ScalarSet(Hyeto));

            }
 
            //Advance to the next timestep 
            AdvanceTime();
            return true;
        }
    }
}
