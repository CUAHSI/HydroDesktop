#region MetaData
//----- Component Description -----
//This component computes watershed runoff, by using the SCS UnitHydrograph Procedure.

//----- Input Files -----
//  This component requires 2 input files, a config.xml file and an elementset.shp that is referenced within
//the configuration file.  All input parameters must be included within one shapefile.  Each row of the
//attribute table represents a single sub-catchment and must contain the following columns.
//
//      Tc = Time of concentration of the watershed [min]
//      Area = Watershed area [mi^2]
//      Watershed = Watershed ID number

//----- Additional Notes -----
//This model performs computations on the current timestep, given input values. As a result, the output values
//are also applied to the current time step.  By convention, the Unit Hydrograph outflow is applied either to 
//the beginning or end of the timestep.  This model applies the result to the end of the timestep, i.e. if at
//time1 hourly precipitation is known, then hourly Pe is supplied as input at time1. The output of this component
// is runoff at time1 as opposed to time0 or time1/2.

#endregion

using System;
using System.Collections;
using System.Data;
using System.Xml;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SMW;



namespace edu.sc.Models.Routing
{

    public class SCSUnitHydrograph: Wrapper
    {
        
        
        //stores the summation hydrographs for each watershed
        DataTable SummationHydrograph = new DataTable();
        //stores the shapefile properties
        DataTable elementValues = new DataTable();
        //stores a series of datatables containing Sub Watershed Convolutions
        DataSet Convolution = new DataSet("Convolution");
        //stores the Start Time that ois defined in the config file
        DateTime _SimulationStartTime;

        string _inputElementSet;
        string _inputQuantity;
        string _outputElementSet;
        string _outputQuantity;

        double[] UH_Times;
        double[] UH_Ordinates;
        double[][] _uh;
        double[][] _pe;

        //Dictionary object to store outputs
        System.Collections.Generic.Dictionary<DateTime, ArrayList> output 
            = new System.Collections.Generic.Dictionary<DateTime, ArrayList>();
        
        //StreamWriter Object to store output
        System.IO.StreamWriter sw;
        string _outDir = null;

        /// <summary>
        /// This method is being used to write the calculated values to file, for use outside of the OpenMI environment.
        /// </summary>
        public override void Finish()
        {
            //intialize streamwriter to write output data. 
            if (_outDir != null)
            {
                try
                { sw = new System.IO.StreamWriter(_outDir + "/SCSUnitHydrograph_output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception("The UH Component was unable to create the desired output file. " +
                                         "This is possibly due to an invalid \'OutDir\' field supplied in " +
                                         "the *.omi file", e);
                }
            }
            else
            {
                try { sw = new System.IO.StreamWriter("../SCSUnitHydrograph_output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception(" The UH component failed in writing it output file to path " +
                     System.IO.Directory.GetCurrentDirectory() + ". This may be due to " +
                     "lack of user permissions.", e);
                }
            }


            //Write output data

            sw.Write("Date, Time, ");

            OpenMI.Standard.ITimeSpan ts = this.GetTimeHorizon();
            TimeStamp t = (TimeStamp)ts.Start;
            DateTime dt = CalendarConverter.ModifiedJulian2Gregorian((double)t.ModifiedJulianDay);

            for (int i = 0; i <= output[dt].Count - 1; i++)
            {
                sw.Write("element " + (i + 1).ToString() + ",");
            }
            sw.Write("\n");

            foreach (System.Collections.Generic.KeyValuePair<DateTime, ArrayList> kvp in output)
            {
                sw.Write(String.Format("{0:MM/dd/yyyy}",kvp.Key) + ", " + 
                         String.Format("{0:hh:mm tt}",kvp.Key) + "," );
                for (int i = 0; i <= kvp.Value.Count - 1; i++)
                {
                    sw.Write(kvp.Value[i].ToString() + ",");
                }
                sw.Write("\n");
            }

            sw.Close();
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string configFile = null;

            //set the config path and output directory from the *.omi file.
            foreach (DictionaryEntry p in properties)
            {
                if (p.Key.ToString() == "ConfigFile")
                    configFile = (string)properties["ConfigFile"];
                else if (p.Key.ToString() == "OutDir")
                    _outDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), (string)properties["OutDir"]);
            }

            // --- lookup model's configuration file to determine interface properties
            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            // Get shapefile path from Config file
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlElement root = doc.DocumentElement;
            XmlNode elementSet = root.SelectSingleNode("//InputExchangeItem//ElementSet");
            string shapefilePath = elementSet["ShapefilePath"].InnerText;

            //Get exchange item properties
            InputExchangeItem input = this.GetInputExchangeItem(0);
            OutputExchangeItem output = this.GetOutputExchangeItem(0);
            _inputElementSet = input.ElementSet.ID;
            _inputQuantity = input.Quantity.ID;
            _outputElementSet = output.ElementSet.ID;
            _outputQuantity = output.Quantity.ID;

            // --- setup elementValues DataTable to store element attributes
            elementValues.Columns.Add("ID");
            elementValues.Columns.Add("Tc");
            elementValues.Columns.Add("Area");

            //Get time horizon properties
            XmlNode TimeHorizon = root.SelectSingleNode("//TimeHorizon");
            _SimulationStartTime = Convert.ToDateTime(TimeHorizon["StartDateTime"].InnerText);
            double timestep_sec = Convert.ToDouble (TimeHorizon["TimeStepInSeconds"].InnerText);

            //Open elementset shapefile
            VectorLayer myLayer = new VectorLayer("elements_layer"); 
            myLayer.DataSource = new ShapeFile(shapefilePath);
            myLayer.DataSource.Open();


            #region Create Unit Hydrographs


            //Define the Unit Hydrograph dimensionless time ordinates
            UH_Times = new double[33] {0.0,0.1,0.2,0.3,0.4,
                                       0.5,0.6,0.7,0.8,0.9,
                                       1.0,1.1,1.2,1.3,1.4,
                                       1.5,1.6,1.7,1.8,1.9,
                                       2.0,2.2,2.4,2.6,2.8,
                                       3.0,3.2,3.4,3.6,3.8,
                                       4.0,4.5,5.0};
            //Define the Unit Hydrograph dimensionless flow ordinates
            UH_Ordinates = new double[33]{0.0,0.03,0.1,0.19,0.31,
                                            0.47,0.66,0.82,0.93,0.99,
                                            1.0,0.99,0.93,0.86,0.78,
                                            0.68,0.56,0.46,0.39,0.33,
                                            0.28,0.207,0.147,0.107,0.077,
                                            0.055,0.04,0.029,0.021,0.015,
                                            0.011,0.005,0.0};



            _uh = new double[myLayer.DataSource.GetFeatureCount()][];
            _pe = new double[_uh.Length][];

            //add feature attributes from shapefile to datatable
            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); ++i)
            {
                FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
                DataTable att = feat.Table;

                //Get features from the shapefile's datatable            
                object ID = feat.ItemArray[feat.Table.Columns.IndexOf("Watershed")];
                object TC = Convert.ToDouble(feat.ItemArray[feat.Table.Columns.IndexOf("Tc")]);
                object AREA = Convert.ToDouble(feat.ItemArray[feat.Table.Columns.IndexOf("Area")]);
              

                //---   BUILD N-MIN SYNTHETIC UNIT HYDROGRAPHS following the SCS procedure ----
                double tc_min = Convert.ToDouble(TC) ;
                double area = Convert.ToDouble(AREA);
                double timestep_min = this.GetTimeStep()/60;

                double L = 0.6 * tc_min;

                double Tp = timestep_min / 2 + L;

                //Calculte Qp for 1 inch of rainfall
                double Qp = (area * 484) / (Tp/60);

                ArrayList Q = new ArrayList();

                //Tp = CalendarConverter.Gregorian2ModifiedJulian(_SimulationStartTime.AddMinutes(Tp));

                double currTime = timestep_min;

                double timeRatio = currTime / Tp;
                do
                {
                    double UH_value = GetUH_Ordinate(timeRatio);

                    double scaledUH = UH_value * Qp;
                    Q.Add(scaledUH);

                    currTime += timestep_min;

                    timeRatio = currTime / Tp;
                } while (timeRatio <= 5 || Q.Count < 10);

                //initialize _uh and _pe
                _uh[i] = new double[Q.Count];
                _pe[i] = new double[Q.Count];

                for (int j = 0; j <= Q.Count - 1; j++)
                {
                    _uh[i][j] = Convert.ToDouble(Q[j]);
                    _pe[i][j] = 0.0;
                }
            }
            #endregion

        }

        /// <summary>
        /// Returns the unit hydrograph q/Qp ratio that corresponds to the given timeRatio.  This
        /// is determined by linearly interpolating between the known UH time ratios.
        /// </summary>
        /// <param name="timeRatio">this value is equivelent to T/Tp; where T = time and Tp = time to peak</param>
        /// <returns>this returns the UH dimensionless value equal to Q/Qp; where Q = flow Rate, and Qp = peak flow rate</returns>
        private double GetUH_Ordinate(double timeRatio)
        {
            bool IsBound = false;
            int i = 0;
            double t1, t2, q1, q2;
            double Q = 0;

            //Check that timeRatio is valid
            if (timeRatio < 0.0 || timeRatio > 5.0)
                return 0.0;

            //Find the correct Q/Qp ratio for the given timeRatio
            while (!IsBound)
            {
                t1 = UH_Times[i];
                t2 = UH_Times[i + 1];

                //Find lower bound
                if (t1 <= timeRatio)
                {
                    //Find upper bound
                    if (t2 >= timeRatio)
                    {
                        q1 = UH_Ordinates[i];
                        q2 = UH_Ordinates[i + 1];

                        //Perform Linear Interpolation
                        Q = (q2 - q1) / (t2 - t1) * (timeRatio - t1) + q1;

                        break;
                    }
                }
                i++;

                if (i == 32)
                {
                    Q = 0;
                    break;
                }
            }

            return Q;
        }

        public override bool PerformTimeStep()
        {
            //request the excess precipitation an infiltration component
            ScalarSet excess = (ScalarSet)this.GetValues(_inputQuantity, _inputElementSet);

            double[] Outflow = new double[excess.Count];
            
            //--- CALCULATE BURST EXCESS ----
            for (int i = 0; i <= excess.Count-1; i++)
            { 
                //Push the burst value into the Pe array
                Push(_pe[i],excess.data[i]);

                //Calculate the resulting Outflow at this timestep
                Outflow[i] = DotProduct(_uh[i], _pe[i]);
        
            }

            //store the Outflow, to write out in Finish()
            ArrayList outputVals = new ArrayList();
            for (int k = 0; k <= Outflow.Length - 1; k++)
            {
                outputVals.Add(Outflow[k]);
            }

            //add the output values and corresponding times to the output arraylist.  This will be used in Finish()
            DateTime Currenttime = CalendarConverter.ModifiedJulian2Gregorian(
                                    ((TimeStamp)this.GetCurrentTime()).ModifiedJulianDay);
            output.Add(Currenttime, outputVals);

            //Set the output values for the downstream component
            this.SetValues(_outputQuantity, _outputElementSet, new ScalarSet(Outflow));

            //advance time
            AdvanceTime();

            return true;
        }

        /// <summary>
        /// This method puts the input "burst excess" value at the top of an array, and shifts all others down.  Since the array has
        /// a fixed size, the last entry will be removed.
        /// </summary>
        /// <param name="arr">an array with a size equal to the length of the Unit Hydrograph</param>
        /// <param name="val">the input "burst excess" that will be placed at the top of the array</param>
        public void Push(double[] arr, double val)
        {
            int size = arr.Length;
            for (int i = 0; i <= size - 2; i++)
            {
                //move all values of the array down one indice
                arr[size - 1 - i] = arr[size - 2 - i];
            }
            //set the input value as the first element in the array
            arr[0] = (double)val;
        }

        /// <summary>
        /// This method performs the dot product between two arrays of the same size.  This method is used to calculate the convoluted
        /// runoff that results from the "burst excess"
        /// </summary>
        /// <param name="a">an array of size 'X' (the unit hydrograph array)</param>
        /// <param name="b">an array of size 'X' (the burst excess array) </param>
        /// <returns>the dot product of the two arrays</returns>
        public double DotProduct(double[] a, double[] b)
        {
            double c = 0.0;

            //check that the arrays are the same length
            if (a.Length == b.Length)
            {
                //perform the dot product
                for (int i = 0; i <= a.Length - 1; i++)
                    c += a[i] * b[i];
            }
            else
                throw new Exception("In order to perform the Dot Product, matrix dimesions must agree");

            return c;
        }
    }

}
