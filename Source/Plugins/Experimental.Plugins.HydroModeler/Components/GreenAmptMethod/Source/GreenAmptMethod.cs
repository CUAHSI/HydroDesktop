using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using SMW;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using SharpMap;
using Numerics;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using System.Collections;
//----------------------------------------------------------------------------------
//                          NOTES:
//This algorithm was developed from the derivation of the Green-Ampt method supplied
//within "Water-Resources Engineering" David A. Chin (2000) Prentice-Hall Inc. Upper
//Saddle River, New Jersey
//----------------------------------------------------------------------------------

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
// Input parameters included within elements.shp: 
//      1.) saturated hydraulic conductivity[[mm/h]         
//      2.) average suction head [mm]                       
//      3.) porosity                                         
//      4.) field capacity
//      5.) wilting point
//      6.) depression storage [mm]

//      Input exchange item needs to be in mm/hr

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------

namespace GreenAmpt
{
    public class GreenAmptMethod : SMW.Wrapper
    {
        public DataTable _elementValues = new DataTable();
        double _dt;
        double[] _F;
        double[] _cumStorage;
        double _T, _tp1, _tp2, _infil, _Excess;
        float _Ks, _n, _FieldCap, _Wilting, _Phi, _DepStor, _theta;
        bool[] PondingCalculated;
        ArrayList _DateTimes = new ArrayList();
        ArrayList _Vals = new ArrayList();
        string _outDir = null;

        public override void Finish()
        {
            SMW.ODM odmUtils = new ODM();
            //Utilities utils = new Utilities();
            string outputPath;

            if (_outDir != null)
                outputPath = _outDir + "/GreenAmpt_output.csv";
            else
                outputPath = "../GreenAmpt_output.csv";

            //------ Create ODM csv -----

            if (_Vals.Count > 0)
            {
                //Set Datetimes
                odmUtils.DateTimes = _DateTimes;
                //Set Values
                odmUtils.Values = _Vals;
                //Set Variable
                odmUtils.VariableName = "Streamflow";
                //Set Corresponding Variable Code
                odmUtils.VariableCode = "3";
                //Set Site Name
                string siteName = this.GetModelID().ToString();
                odmUtils.SiteName = siteName;
                //Select a Unique Site ID
                odmUtils.SiteCode = "40";
                //Set CSV Path
                odmUtils.CSVPath = System.Environment.CurrentDirectory;
                //Write CSV file
                odmUtils.CreateODMcsv();
                Console.WriteLine("ODM .csv has been written");
                //Write BAT file
                odmUtils.CreateBat();
                Console.WriteLine("ODM .bat has been written");
                //Load CSV into ODM by executing the BAT file
                odmUtils.LoadIntoODM();
                Console.WriteLine("Data has been loaded");
            }


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
            _elementValues.Columns.Add("Ks", typeof(float));
            _elementValues.Columns.Add("SuctionHead", typeof(float));
            _elementValues.Columns.Add("Porosity", typeof(float));
            _elementValues.Columns.Add("FieldCapacity", typeof(float));
            _elementValues.Columns.Add("WiltingPoint", typeof(float));
            _elementValues.Columns.Add("DepressionStorage", typeof(float));
            _elementValues.Columns.Add("F", typeof(float));      //Cumulative infiltration

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

            //initialize cumulative infiltration 
            _F = new double[myLayer.DataSource.GetFeatureCount()];
            //initialize cumulative storage
            //_TotStor = new double[myLayer.DataSource.GetFeatureCount()];
            _cumStorage = new double[myLayer.DataSource.GetFeatureCount()];
            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); i++)
            {
                SharpMap.Data.FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
                DataTable att = feat.Table;

                object Watershed = feat.ItemArray[feat.Table.Columns.IndexOf("Id")];
                object Ks = feat.ItemArray[feat.Table.Columns.IndexOf("Ks")];
                object SuctionHead = feat.ItemArray[feat.Table.Columns.IndexOf("Suction")];
                object Porosity = feat.ItemArray[feat.Table.Columns.IndexOf("Porosity")];
                object FieldCapacity = feat.ItemArray[feat.Table.Columns.IndexOf("FieldCapac")];
                object WiltingPoint = feat.ItemArray[feat.Table.Columns.IndexOf("WiltingPt")];
                object DepressionStorage = feat.ItemArray[feat.Table.Columns.IndexOf("DepStorage")];
                _elementValues.LoadDataRow(new object[] { Watershed, Ks, SuctionHead, Porosity, FieldCapacity, WiltingPoint, DepressionStorage }, true);
                _F[i] = 0.0;
                //_TotStor[i] = 0.0;
                _cumStorage[i] = 0.0;
            }
            // dt in hours
            _dt = this.GetTimeStep() / 3600;
            _F = new double[myLayer.DataSource.GetFeatureCount()];


            PondingCalculated = new bool[myLayer.DataSource.GetFeatureCount()];

        }

        public override bool PerformTimeStep()
        {
            //reading the input exchange items Quantatiy and ID 
            ScalarSet precip = (ScalarSet)this.GetValues(this.GetInputExchangeItem(0).Quantity.ID, this.GetInputExchangeItem(0).ElementSet.ID);
            double[] Fp = new double[precip.Count];
            //double[] F = new double[precip.Count];
            double[] Runoff = new double[precip.Count];
            double[] cumulative_infiltration = new double[precip.Count];

            for (int i = 0; i <= precip.Count - 1; i++)
            {

                //Get input values from elementValues datatable
                _Ks = Convert.ToSingle(_elementValues.Rows[i]["Ks"]);
                _n = Convert.ToSingle(_elementValues.Rows[i]["Porosity"]);
                _FieldCap = Convert.ToSingle(_elementValues.Rows[i]["FieldCapacity"]);
                _Wilting = Convert.ToSingle(_elementValues.Rows[i]["WiltingPoint"]);
                _Phi = Convert.ToSingle(_elementValues.Rows[i]["SuctionHead"]);
                _DepStor = Convert.ToSingle(_elementValues.Rows[i]["DepressionStorage"]);
                _theta = (float).5 * (_Wilting + _FieldCap);

                //Calculate infiltration capacity
                Fp[i] = _Ks + (_Ks * (_n - _theta) * _Phi) / _F[i];


                //Determine how much rainfall will infiltrate
                if (precip.data[i] <= _Ks)
                {
                    //All rainfall will be absorbed
                    _F[i] += precip.data[i] * _dt;
                    _Excess = 0.0;
                    //Console.WriteLine("Cumulative Infiltration: " + _F[i].ToString());
                }
                else
                {
                    if (PondingCalculated[i] == false)
                    {
                        //Calculate the time ponding occurs
                        calculatePonding(precip.data[i], _F[i]);
                        PondingCalculated[i] = true;
                    }


                    Numerics.Numerics solver = new Numerics.Numerics();
                    Dictionary<string, double> Results = new Dictionary<string, double>();

                    double x1 = 0;
                    double x2 = 50;

                    double fx1 = fval(x1);
                    double fx2 = fval(x2);
                    double e = 1;

                    while (e > 0.000001)
                    {
                        Results = solver.SecantMethod(fx1, x1, fx2, x2);
                        e = Results["error"];
                        x1 = Results["x1"];
                        x2 = Results["x2"];
                        fx1 = fval(x1);
                        fx2 = fval(x2);
                    }


                    _Excess = _F[i] + precip.data[i] * _dt - x1;
                    _F[i] = x1;
                }

                //Determine how much rainfall will become depression storage
                if ((_DepStor - _cumStorage[i]) > 0.0)
                {
                    if (_Excess >= (_DepStor - _cumStorage[i]))
                    {
                        _Excess -= _DepStor - _cumStorage[i];
                        _cumStorage[i] = _DepStor;
                    }
                    else
                    {
                        _cumStorage[i] += _Excess;
                        _Excess = 0.0;
                    }
                }

                Runoff[i] = _Excess;
                cumulative_infiltration[i] = _cumStorage[i];

                //Add values to DateTimes and Vals, for ODM writeout in Finish()
                _Vals.Add(_Excess);
                TimeStamp t = (TimeStamp)this.GetCurrentTime();
                DateTime T = CalendarConverter.ModifiedJulian2Gregorian(t.ModifiedJulianDay);
                _DateTimes.Add(T);


            }
            //Console.WriteLine("{0:F2} \t\t {1:F3} \t\t {2:F0} \t\t {3:F2} \t\t {4:F2} \t\t {5:F2} ", _F[0], _dt, precip.data[0], precip.data[0] * _dt, _cumStorage[0], _Excess);


            //set the excess rainfall values as runoff output
            string q1 = this.GetOutputExchangeItem(0).Quantity.ID;
            string e1 = this.GetOutputExchangeItem(0).ElementSet.ID;
            this.SetValues(q1, e1, new ScalarSet(Runoff));

            //set the cumulative infitration storage depth
            string q2 = this.GetOutputExchangeItem(1).Quantity.ID;
            string e2 = this.GetOutputExchangeItem(1).ElementSet.ID;
            this.SetValues(q2, e2, new ScalarSet(_F));

            _T += _dt;
            this.AdvanceTime();
            return true;
        }


        double fval(double x)
        {
            return x - (_n - _theta) * _Phi * Math.Log(1 + x / ((_n - _theta) * _Phi)) - _Ks * (_T + _dt - _tp1 + _tp2);
        }

        void calculatePonding(double i, double cumInfil)
        {
            //Potential Infiltration (f)
            double f = (_Ks * (_n - _theta) * _Phi) / (i - _Ks);
            //Equivelent Infiltration time (corresponding to the potential infiltration)
            double t_prime = (f - cumInfil) / i;
            //Time at which ponding occurs
            _tp1 = _T + t_prime;
            //Time it takes for the potential infiltration to infiltrate
            _tp2 = ((f - (_n - _theta) * _Phi * Math.Log(1 + f / ((_n - _theta) * _Phi))) / _Ks);
        }
    }
}
