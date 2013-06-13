using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;


using SMW;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using SharpMap;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using System.Collections;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------

// Input parameters included within elements.shp: 

//      1.) cloud height [km]                     
//      2.) surface pressure [kpa]                                         
//      3.) area [km^2] //Not in use till now

//      Input exchange item needs to be in celsius

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------

namespace PrecipitableWater
{
    public class PrecipMethod : SMW.Wrapper
    {
        public DataTable _elementValues = new DataTable();
        double _cloudh, _surfp, _area;
        ArrayList _DateTimes = new ArrayList();
        ArrayList _Vals = new ArrayList();
        string _outDir = null;
        private int _featureCount;
        public double[] Precip;

        public override void Finish()
        {
            string outputPath;

            if (_outDir != null)
                outputPath = _outDir + "/PrecipWater_output.csv";
            else
                outputPath = "../PrecipWater_output.csv";

            StreamWriter writer = new StreamWriter(outputPath);

            writer.Write("Precipitable Water (mm)" + "\n");
            writer.Write("Time, ");

            for (int i = 0; i < _featureCount; i++)
            {
                writer.Write("element " + (i + 1).ToString() + ",");
            }

            writer.Write("\n");

            for (int j = 0; j < _DateTimes.Count; j++)
            {
                writer.Write(_DateTimes[j] + "," + _Vals[j * _featureCount]);

                for (int k = 1; k < _featureCount; k++)
                {
                    writer.Write("," + _Vals[j * _featureCount + k]);
                }
                
                writer.Write("\n");                
            }

            writer.Close();

            #region Previous Create ODM csv code
            //------ Create ODM csv -----

            //if (_Vals.Count > 0)
            //{
            //    //Set Datetimes
            //    odmUtils.DateTimes = _DateTimes;
            //    //Set Values
            //    odmUtils.Values = _Vals;
            //    //Set Variable
            //    odmUtils.VariableName = "Precipitable Water";
            //    //Set Corresponding Variable Code
            //    odmUtils.VariableCode = "5"; // What's the VariableCode?
            //    //Set Site Name
            //    string siteName = this.GetModelID().ToString();
            //    odmUtils.SiteName = siteName;
            //    //Select a Unique Site ID
            //    odmUtils.SiteCode = "40";
            //    //Set CSV Path
            //    odmUtils.CSVPath = System.Environment.CurrentDirectory;
            //    //Write CSV file
            //    odmUtils.CreateODMcsv();
            //    Console.WriteLine("ODM .csv has been written");
            //    //Write BAT files
            //    odmUtils.CreateBat();
            //    Console.WriteLine("ODM .bat has been written");
            //    //Load CSV into ODM by executing the BAT file
            //    odmUtils.LoadIntoODM();
            //    Console.WriteLine("Data has been loaded");
            //}
            #endregion
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
                    _outDir = (string)properties["OutDir"];
            }

            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            // --- setup elementValues DataTable to store element attributes from elements.shp
            _elementValues.Columns.Add("Watershed", typeof(int));
            _elementValues.Columns.Add("Pressure", typeof(double));
            _elementValues.Columns.Add("CloudH", typeof(double));
            _elementValues.Columns.Add("Area", typeof(double));

            //Get shapefile path
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlElement root = doc.DocumentElement;
            XmlNode elementSet = root.SelectSingleNode("//InputExchangeItem//ElementSet");
            string shapefilePath = elementSet["ShapefilePath"].InnerText;

            //Get watershed properties from elements.shp
            SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("elements_layer");
            myLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(shapefilePath);
            myLayer.DataSource.Open();

            //initialize cumulative precipitable water
            _featureCount = myLayer.DataSource.GetFeatureCount();
            Precip = new double[_featureCount];

            for (uint i = 0; i < _featureCount; i++)
            {
                SharpMap.Data.FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
                DataTable att = feat.Table;

                object Watershed = feat.ItemArray[feat.Table.Columns.IndexOf("Id")];
                object Pressure = feat.ItemArray[feat.Table.Columns.IndexOf("Pressure_s")];
                object CloudH = feat.ItemArray[feat.Table.Columns.IndexOf("CloudHt")];
                object Area = feat.ItemArray[feat.Table.Columns.IndexOf("Area")];

                _elementValues.LoadDataRow(new object[] { Watershed, Pressure, CloudH, Area }, true);
                
                //Set the initial _precipwater for each watershed = 0
                Precip[i] = 0.0;
            }

        }

        public override bool PerformTimeStep()
        {
            //GetValues - Receiving Data
            ScalarSet temp_s = (ScalarSet)this.GetValues("T", "Temperature");
            double[] Temperature = temp_s.data;
            
            //loop through all elements
            for (int i = 0; i < _featureCount; i++)
            {
                //Get input values from elementValues datatable
                _surfp = Convert.ToSingle(_elementValues.Rows[i]["Pressure"]);
                _cloudh = Convert.ToSingle(_elementValues.Rows[i]["CloudH"]);
                _area = Convert.ToSingle(_elementValues.Rows[i]["Area"]);

                //Calculate the precipitable water
                double Dz = 0.3; // Unit: km
                double Alpha = 6.5; // Unit: K/km
                double g = 9.81; //Unit: m/s^2
                double Ra = 287; //Unit: J/kg-K

                //Define input parameters
                double T = 0;
                if (Temperature.Length != 0)
                {
                    T = (Temperature[0]) + 273.15;//in Kelvin
                }

                //Define calculation Parameters
                double qa1, qa2; //air density
                double e1, e2; //Saturation vapor pressure
                double qv1, qv2; //Specific humidity
                double Dmp = 0;

                double zinc = 0;

                while (zinc < _cloudh)
                {

                    qa1 = (_surfp / (Ra * T)) * 1000;
                    e1 = 611 * Math.Exp((17.27 * (T - 273.15)) / (237.3 + (T - 273.15)));
                    qv1 = 0.622 * (e1 / (_surfp * 1000 - 0.378 * e1));

                    _surfp = _surfp * Math.Pow(((T - (Alpha * Dz)) / T), (g / ((Alpha / 1000) * Ra)));
                    T = T - (Alpha * Dz);

                    qa2 = (_surfp / (Ra * T)) * 1000;
                    e2 = 611 * Math.Exp((17.27 * (T - 273.15)) / (237.3 + (T - 273.15)));
                    qv2 = 0.622 * (e2 / (_surfp * 1000 - 0.378 * e2));

                    Dmp = Dmp + ((qv1 + qv2) / 2) * ((qa1 + qa2) / 2) * Dz * 1000;

                    zinc = zinc + Dz;
                }

                Dmp = Dmp / 25.4;

                Precip[i] = Dmp;

                _Vals.Add(Dmp);
            }

            TimeStamp t = (TimeStamp)this.GetCurrentTime();
            DateTime Time = CalendarConverter.ModifiedJulian2Gregorian(t.ModifiedJulianDay);
            _DateTimes.Add(Time);

            //Set Values in array
            this.SetValues("P", "Precipitable Water", new ScalarSet(Precip));

            this.AdvanceTime();
            return true;
        }
    }

}
