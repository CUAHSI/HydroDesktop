// Purpose: Evaluate Standardized Evapotranspiration and Potantial Evapotranspiration in mm per day.
// Author: Mehmet Ercan (mehmetbercan@gmail.com)
// Advisor: Jonathan L. Goodall (goodall@sc.edu)
// History: Created (07-29-2010)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SMW;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;

namespace PT_Evapotranspiration
{
    public class PT_PET : SMW.Wrapper
    {
        public string input_quantity;
        public string input_elementset;
        public string input_quantity2;
        public string input_elementset2;
        public string input_quantity3;
        public string input_elementset3;        
        
        public string output_quantity;
        public string output_elementset;       
        public string output_quantity2;        
        public string output_elementset2;

        double[] C_set = new double[2]; //define the coefficients from set file
        Dictionary<ITime, List<double[]>> output = new Dictionary<ITime, List<double[]>>();
        string _outDir = null;

        public override void Finish()
        {
            //intialize streamwriter to write output data. 
            System.IO.StreamWriter sw;
            if (_outDir != null)
            {
                try
                { sw = new System.IO.StreamWriter(_outDir + "/output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception("The ET Component was unable to create the desired output file. " +
                                         "This is possibly due to an invalid \'OutDir\' field supplied in " +
                                         "the *.omi file", e);
                }
            }
            else
            {
                try { sw = new System.IO.StreamWriter("../output.csv"); }
                catch (SystemException e)
                {
                    throw new Exception(" The ET component failed in writing it output file to path " +
                     System.IO.Directory.GetCurrentDirectory() + ". This may be due to " +
                     "lack of user permissions.", e);
                }
            }

            int i = 1;
            foreach (KeyValuePair<ITime, List<double[]>> val in output)
            {
                if (i == 2)
                {    //Write Station IDs 
                    sw.Write("StationID:");
                    for (int st = 0; st < (val.Value[0]).Count(); st++)
                    {
                        sw.Write("," + st);
                    }
                    //Write Latitudes  
                    sw.WriteLine();
                    sw.Write("Latitude:");
                    for (int st = 0; st < (val.Value[0]).Count(); st++)
                    {
                        sw.Write(",NA"); //+ val.Value[st][1]);
                    }
                    //Write Longitudes  
                    sw.WriteLine();
                    sw.Write("Longitude:");
                    for (int st = 0; st < (val.Value[0]).Count(); st++)
                    {
                        sw.Write(",NA");// + val.Value[st][2]);
                    }
                    //Write Elevation 
                    sw.WriteLine();
                    sw.Write("Elevation:");
                    for (int st = 0; st < (val.Value[0]).Count(); st++)
                    {
                        sw.Write("," + val.Value[2][st]);
                    }
                    //Write first line of date and Standartized ET  
                    sw.WriteLine();
                    sw.Write("Date:");
                    for (int st = 0; st < (val.Value[0]).Count(); st++)
                    {
                        sw.Write(",ETsz(mm/d)");
                    }
                }
                if (i != 1 && i != 2)
                {
                    //Write daily standartized ETs
                    string time = String.Format("{0:MM-dd-yyyy}", CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)val.Key).ModifiedJulianDay));
                    sw.WriteLine();
                    sw.Write(time);
                    for (int st = 0; st < (val.Value[0]).Count(); st++)
                    {
                        sw.Write("," + val.Value[1][st]);
                    }
                }
                i++;
            }
            sw.Close();
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string configFile = null;
            string data_dir = null;

            //Get Config file directory from .omi file
            if (properties.ContainsKey("ConfigFile"))
                configFile = (string)properties["ConfigFile"];
            //Get wtmp files directory from .omi file
            if (properties.ContainsKey("DataFolder"))
                data_dir = (string)properties["DataFolder"];
            if (properties.ContainsKey("OutDir"))
                _outDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), (string)properties["OutDir"]);


            //Set variables
            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            //get input exchange item attributes
            input_elementset = this.GetInputExchangeItem(0).ElementSet.ID;
            input_quantity = this.GetInputExchangeItem(0).Quantity.ID;
            input_elementset2 = this.GetInputExchangeItem(1).ElementSet.ID;
            input_quantity2 = this.GetInputExchangeItem(1).Quantity.ID;
            input_elementset3 = this.GetInputExchangeItem(2).ElementSet.ID;
            input_quantity3 = this.GetInputExchangeItem(2).Quantity.ID;

            //get output exchange item attributes
            output_elementset = this.GetOutputExchangeItem(0).ElementSet.ID;
            output_quantity = this.GetOutputExchangeItem(0).Quantity.ID;
            output_elementset2 = this.GetOutputExchangeItem(1).ElementSet.ID;
            output_quantity2 = this.GetOutputExchangeItem(1).Quantity.ID;         

            //Read .set file to get coefficients
            StreamReader data1 = new StreamReader(data_dir + "/PT-PET.set");
            data1.ReadLine(); //Read first info line...
            data1.ReadLine(); //Read Second info line...
            C_set[0] = Convert.ToDouble(data1.ReadLine().Split(',')[1]); //Read Crop Coefficient
            C_set[1] = Convert.ToDouble(data1.ReadLine().Split(',')[1]); //Read Alpha coefficient

            //////////////shape file/////////////////////////////////////////
            ////get shapefile path
            //string shapefilePath = this.GetShapefilePath();
            ////this uses the free SharpMap API for reading a shapefile
            //VectorLayer myLayer = new VectorLayer("elements_layer");
            //myLayer.DataSource = new ShapeFile(shapefilePath);
            //myLayer.DataSource.Open();
            ////initialize array to hold the transformed inflow values
            //int size = myLayer.DataSource.GetFeatureCount();
            ////--- BUILD NETWORK ---
            //// loop through all features in feature class
            //for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); ++i)
            //{
            //    FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
            //    string STid = feat.ItemArray[feat.Table.Columns.IndexOf("StationID")].ToString();
            //    string Elev = feat.ItemArray[feat.Table.Columns.IndexOf("Elevation_")].ToString();
            //    string lat = feat.ItemArray[feat.Table.Columns.IndexOf("Latitude")].ToString();
            //    string lon = feat.ItemArray[feat.Table.Columns.IndexOf("Longitude")].ToString();

            //}
            //////////////shape file/////////////////////////////////////////

        }

        public override bool PerformTimeStep()
        {
            //Retrieve values from another component
            ScalarSet _NetRadiation = (ScalarSet)this.GetValues(input_quantity, input_elementset);
            double[] Net_Radiation = _NetRadiation.data;
            ScalarSet _temp = (ScalarSet)this.GetValues(input_quantity2, input_elementset2);
            double[] temp = _temp.data;
            ScalarSet _elevation = (ScalarSet)this.GetValues(input_quantity3, input_elementset3);
            double[] elevation = _elevation.data;

            /////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////PT POTANTIAL EVAPOTRANSPIRATION CALCULATION/////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////

            int st = temp.Length;
            double[] ETsz = new double[st];
            double[] PET = new double[st];

            //Looping the stations
            for (int i = 0; i < st; i++)
            {
                //Defining variables
                double T, es, D, z, P, Gama, cp, Lamda, alpha, Kc, NR, Er; 
                Kc = C_set[0];
                alpha = C_set[1];
                T = temp[i]; // Temperature (C)
                NR = Net_Radiation[i]; //Net Radiation (MJ/m^2/d)
                z = elevation[i];//z:elevation(m)

                //Calculation of Saturation vapor pressure-temperature gradient
                es = 6.107799961 + 4.436518521 * Math.Pow(10, -1) * T + 1.428945805 * Math.Pow(10, -2) * Math.Pow(T, 2) + 2.650648471 * Math.Pow(10, -4) * Math.Pow(T, 3) + 3.031240396 * Math.Pow(10, -6) * Math.Pow(T, 4) + 2.034080948 * Math.Pow(10, -8) * Math.Pow(T, 5) + 6.136820929 * Math.Pow(10, -11) * Math.Pow(T, 6); //(in mb) Vapor pressure is determined from the dew point temperature using the polynomial provided by Lowe (1977).
                es = 0.1 * es; //mb to Kpa convertion.
                D = (4098 * es) / (Math.Pow((237.3 + T), 2)); //D:Saturation vapor pressure-temperature gradient(kPa/C)(Shuttleworth,1993)

                //Calculation of Psychrometric constant
                P = 101.3 * Math.Pow(((293 - 0.0065 * z) / 293), 5.256); //P:Atmospheric pressure (kPa)
                cp = (1.013) / 1000; // Specific heat of moist air (MJ/kg/C)
                Lamda = (2501 - 2.361 * T) / 1000; //LatentHeatofVaporization (Mj/kg)
                Gama = (cp * P) / (0.622 * Lamda); //Psychrometric constant(kPa/C)

                Er = NR / (Lamda * 1); //Evaporation by energy balance method (mm/d) (water density 1 Mg/m^3 and in daily step soil heat flux(G) is assumed to be zero)

                ETsz[i] = alpha * Er * (D / (D + Gama)); //Standartized Evapotranspiration (mm/day)
                PET[i] = ETsz[i] * Kc; //Potantial Evapotranspiration (mm/day)

            }

            //set values
            this.SetValues(output_quantity, output_elementset, new ScalarSet(PET));
            this.SetValues(output_quantity2, output_elementset2, new ScalarSet(ETsz));

            //prepare output file
            List<double[]> PetEtsz = new List<double[]>();
            PetEtsz.Add(PET);
            PetEtsz.Add(ETsz);
            PetEtsz.Add(elevation);
            output.Add(this.GetCurrentTime(), PetEtsz); ;

            //set advance model trough time 
            this.AdvanceTime();

            return true;
        }
    }
}






