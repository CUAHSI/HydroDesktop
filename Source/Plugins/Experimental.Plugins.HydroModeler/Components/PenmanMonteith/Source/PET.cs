// Purpose: Evaluate ASCE Standardized Evapotranspiration and Potantial Evapotranspiration in mm per day.
// Author: Mehmet Ercan (mehmetbercan@gmail.com)
// Advisor: Jonathan L. Goodall (goodall@sc.edu)
// History: Created (02-23-2010), put in OpenMI(03-11-2010)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;


using SMW;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;

namespace Evptrnsprtion
{
    public class eT : SMW.Wrapper
    {
        public DataTable _elementValues = new DataTable();

        List<InputExchangeItem> inputs = new List<InputExchangeItem>();
        List<OutputExchangeItem> outputs = new List<OutputExchangeItem>();


        ETworks In = new ETworks();
        Dictionary<ITime, List<double[]>> output = new Dictionary<ITime, List<double[]>>();

        //defining data storage list
        double[] Net_Radiation;
        double[] Temprtr;
        double[] TemprtrDew;
        double[] TemprtrMax;
        double[] TemprtrMin;
        double[] WindSpeed;

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
                if (i == 1)
                {    //Write Station IDs 
                    sw.Write("StationID:");
                    for (int st = 0; st < val.Value.Count; st++)
                    {
                        sw.Write("," + val.Value[st][0]);
                        double[] sdfj = val.Value[st];
                    }
                    //Write Latitudes  
                    sw.WriteLine();
                    sw.Write("Latitude:");
                    for (int st = 0; st < val.Value.Count; st++)
                    {
                        sw.Write("," + val.Value[st][1]);
                    }
                    //Write Longitudes  
                    sw.WriteLine();
                    sw.Write("Longitude:");
                    for (int st = 0; st < val.Value.Count; st++)
                    {
                        sw.Write("," + val.Value[st][2]);
                    }
                    //Write Elevation 
                    sw.WriteLine();
                    sw.Write("Elevation:");
                    for (int st = 0; st < val.Value.Count; st++)
                    {
                        sw.Write("," + val.Value[st][3]);
                    }
                    //Write first line of date and Standartized ET  
                    sw.WriteLine();
                    sw.Write("Date:");
                    for (int st = 0; st < val.Value.Count; st++)
                    {
                        sw.Write(",ETsz");
                    }
                }
                //Write daily standartized ETs
                string time = String.Format("{0:MM-dd-yyyy}", CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)val.Key).ModifiedJulianDay));
                sw.WriteLine();
                sw.Write(time);
                for (int st = 0; st < val.Value.Count; st++)
                {
                    sw.Write("," + val.Value[st][4]);
                }
                i++;
            }
            sw.Close();
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string configFile = null;

            //Get Config file directory from .omi file
            if (properties.ContainsKey("ConfigFile"))
                configFile = (string)properties["ConfigFile"];
            if (properties.ContainsKey("OutDir"))
                _outDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), (string)properties["OutDir"]);

            //Set variables
            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            //get input exchange item attributes
            int num_inputs = this.GetInputExchangeItemCount();
            for (int i = 0; i < num_inputs; i++)
            {
                inputs.Add(this.GetInputExchangeItem(i));
            }


            //get output exchange item attributes
            int num_outputs = this.GetOutputExchangeItemCount();
            for (int i = 0; i < num_outputs; i++)
            {
                outputs.Add(this.GetOutputExchangeItem(i));
            }


            // --- setup elementValues DataTable to store element attributes from elements.shp
            _elementValues.Columns.Add("Gauge_IDs", typeof(double));
            _elementValues.Columns.Add("Longitude", typeof(double));
            _elementValues.Columns.Add("Latitude", typeof(double));
            _elementValues.Columns.Add("kc1", typeof(double));
            _elementValues.Columns.Add("kc2", typeof(double));
            _elementValues.Columns.Add("kc3", typeof(double));
            _elementValues.Columns.Add("kc4", typeof(double));
            _elementValues.Columns.Add("kc5", typeof(double));
            _elementValues.Columns.Add("kc6", typeof(double));
            _elementValues.Columns.Add("kc7", typeof(double));
            _elementValues.Columns.Add("kc8", typeof(double));
            _elementValues.Columns.Add("kc9", typeof(double));
            _elementValues.Columns.Add("kc10", typeof(double));
            _elementValues.Columns.Add("kc11", typeof(double));
            _elementValues.Columns.Add("kc12", typeof(double));
            _elementValues.Columns.Add("RefVegType", typeof(double));
            _elementValues.Columns.Add("Elevation", typeof(double));

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

            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); i++)
            {
                SharpMap.Data.FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
                //DataTable att = feat.Table;

                object GaugeId = feat.ItemArray[feat.Table.Columns.IndexOf("GaugeIDs")];
                object Longitute = feat.ItemArray[feat.Table.Columns.IndexOf("X")];
                object Latitude = feat.ItemArray[feat.Table.Columns.IndexOf("Y")];
                object Kc1 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc1")];
                object Kc2 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc2")];
                object Kc3 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc3")];
                object Kc4 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc4")];
                object Kc5 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc5")];
                object Kc6 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc6")];
                object Kc7 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc7")];
                object Kc8 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc8")];
                object Kc9 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc9")];
                object Kc10 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc10")];
                object Kc11 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc11")];
                object Kc12 = feat.ItemArray[feat.Table.Columns.IndexOf("Kc12")];
                object RefVeg = feat.ItemArray[feat.Table.Columns.IndexOf("RefVeg")];
                object Elevation = feat.ItemArray[feat.Table.Columns.IndexOf("Elev")];
                _elementValues.LoadDataRow(new object[] { GaugeId, Longitute, Latitude, Kc1, Kc2, Kc3, Kc4, Kc5, Kc6, Kc7, Kc8, Kc9, Kc10, Kc11, Kc12, RefVeg , Elevation }, true);


            }

        }

        public override bool PerformTimeStep()
        {

            //Retrieve values from another component
            ScalarSet _NetRadiation = (ScalarSet)this.GetValues(inputs[0].Quantity.ID, inputs[0].ElementSet.ID);
            Net_Radiation = _NetRadiation.data;
            ScalarSet _Temprtr = (ScalarSet)this.GetValues(inputs[1].Quantity.ID, inputs[1].ElementSet.ID);
            Temprtr = _Temprtr.data;
            ScalarSet _TemprtrDew = (ScalarSet)this.GetValues(inputs[2].Quantity.ID, inputs[2].ElementSet.ID);
            TemprtrDew = _TemprtrDew.data;
            ScalarSet _TemprtrMax = (ScalarSet)this.GetValues(inputs[3].Quantity.ID, inputs[3].ElementSet.ID);
            TemprtrMax = _TemprtrMax.data;
            ScalarSet _TemprtrMin = (ScalarSet)this.GetValues(inputs[4].Quantity.ID, inputs[4].ElementSet.ID);
            TemprtrMin = _TemprtrMin.data;
            ScalarSet _WindSpeed = (ScalarSet)this.GetValues(inputs[5].Quantity.ID, inputs[5].ElementSet.ID);
            WindSpeed = _WindSpeed.data;



            /////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////EVAPOTRANSPIRATION CALCULATION/////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////

            //get current time
            DateTime currTime = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetCurrentTime()).ModifiedJulianDay);
            double current_time = Convert.ToDouble(Convert.ToString(currTime.Month) + Convert.ToString(currTime.Day) + Convert.ToString(currTime.Year));
            string month = Convert.ToString(currTime.Month);
            string Kcm = "Kc" + month;

            //Get data from saved attribute table
            int n = _elementValues.Rows.Count;
            double[] _gauge_ids = new double[n];
            double[] _lon = new double[n];
            double[] _lat = new double[n];
            double[] _kc = new double[n];
            double[] _refvegtyp = new double[n];
            double[] _elev = new double[n];
            for (int i = 0; i < n; i++)
            {
                _gauge_ids[i] = Convert.ToDouble(_elementValues.Rows[i]["Gauge_IDs"]);
                _lon[i] = Convert.ToDouble(_elementValues.Rows[i]["Longitude"]);
                _lat[i] = Convert.ToDouble(_elementValues.Rows[i]["Latitude"]);
                _kc[i] = Convert.ToDouble(_elementValues.Rows[i][Kcm]);
                _refvegtyp[i] = Convert.ToDouble(_elementValues.Rows[i]["RefVegType"]);
                _elev[i] = Convert.ToDouble(_elementValues.Rows[i]["Elevation"]);
            }
            
            //Setting values for the stations
            List<double[]> StIDET = new List<double[]>();
            for (int i = 0; i < n; i++)
            {
                //Setting crop coeficient
                In.kc = _kc[i];

                //Setting Conditional for vegetation referece (0:short, 1:tall)
                In.RVC = _refvegtyp[i];

                //setting Net Radiation(MJ/m^2/d)
                In.NetRadiation = Net_Radiation[i];

                //Setting watershed elevation(m)
                In.Elev = _elev[i];

                //Setting Temperature(C)
                In.Temp = Temprtr[i];

                //Setting Dew Point Temperature(C)
                In.Tdew = TemprtrDew[i];

                //Setting Maximum Temperature(C)
                In.TempMx = TemprtrMax[i];

                //Setting Minimum Temperature(C)
                In.TempMn = TemprtrMin[i];

                //setting wind speed (m/s)
                In.WindSpeed = WindSpeed[i];

                //Getting Calculated Evapotranspiration(mm/d)
                double[] PET1 = new double[7];
                PET1[0] = _gauge_ids[i]; //StationID
                PET1[1] = _lat[i]; //Latitude
                PET1[2] = _lon[i]; //Longitude
                PET1[3] = _elev[i]; //Elevation
                PET1[4] = In.StandardizationofReferenceET(); //result ETsz
                PET1[5] = In.PotantialET(); //result PET
                PET1[6] = current_time; //Date
                StIDET.Add(PET1);
            }

            

            //Get an array result for exchange with SMW
            double[] PET = new double[StIDET.Count()];//Potantial ET
            double[] ETsz = new double[StIDET.Count()];//Standartized Referance ET
            for (int i = 0; i < StIDET.Count(); i++)
            {
                PET[i] = StIDET[i][5];
                ETsz[i] = StIDET[i][4];
            }


            //set values
            this.SetValues(outputs[0].Quantity.ID, outputs[0].ElementSet.ID, new ScalarSet(PET));
            this.SetValues(outputs[1].Quantity.ID, outputs[1].ElementSet.ID, new ScalarSet(ETsz));

            output.Add(this.GetCurrentTime(), StIDET); ;

            //set advance model trough time 
            this.AdvanceTime();

            return true;
        }
    }


    class ETworks
    {
        //Setting Temperature value(C)
        double _temp;
        public double Temp { get { return _temp; } set { _temp = value; } }
        //Setting Max and Min Temperature value(C)
        double _tempmx;
        public double TempMx { get { return _tempmx; } set { _tempmx = value; } }
        double _tempmn;
        public double TempMn { get { return _tempmn; } set { _tempmn = value; } }
        //Setting Dew point temperature(C)
        double _Tdew;
        public double Tdew { get { return _Tdew; } set { _Tdew = value; } }
        //Setting wind speed(m/s)
        double _u;
        public double WindSpeed { get { return _u; } set { _u = value; } }
        //Setting vegetation coefficient, Kc
        double _kc;
        public double kc { get { return _kc; } set { _kc = value; } }
        //setting Net Radiattion(MJ/m^2/d)
        double _NetRadiation;
        public double NetRadiation { get { return _NetRadiation; } set { _NetRadiation = value; } }
        //setting elevation(m)
        double _Elev;
        public double Elev { get { return _Elev; } set { _Elev = value; } }
        //setting reference vegetation condition
        double _RVC;
        public double RVC { get { return _RVC; } set { _RVC = value; } }

        public double PotantialET()
        {
            //info from TOPNET: Kc:Vegetation crop coefficient thaat varies by month to represent growing seasons, is determined based upon land cover.
            double Kc = kc; //Kc:Vegetation crop coefficient//average crop coefficient should be determined over the watershed or the python codes can be used with ETsz to determine PET over the watershed.(http://grg.engr.sc.edu/mehmet/scripts.html)
            double ETsz = StandardizationofReferenceET();
            double PET = ETsz * Kc;
            return PET;
        }

        public double StandardizationofReferenceET()
        {
            double ETsz, Rn, G, T, u2, es, ea, D, Gama, Cn, Cd, Lamda, qw;//ETsc:Standardized reference crop ET (mm/d)
            //Rn:Net radiation((MJ/m^2/d)//G:Soil heat flux density at the soil surface(MJ/m^2/d). G = 0(dailyTimestep)(MJ/m^2/d)
            //T:Mean daily air temperature(Kelvin)(will be converted from C to K)//u2:Mean daily wind speed at 2m height(m/s)
            //es:Saturation vapor pressure (kPa)//ea:Mean actual vapor pressure(kPa)//D:Saturation vapor pressure-temperature gradient(kPa/C)
            //Gama:Psychrometric constant(kPa/C)//Lamda:Latent heat of Vaporization(MJ/kg)(2.45 is recommentded by ASCE)//qw:Water density(1000kg/m^3)
            //Cn:Numerator constant that changes with reference type and time units(K mm s^3/Mg/d)
            //Cd:Denominator constant that changes with reference type and time units(s/m)
            u2 = WindSpeed;
            double Tdw = Tdew;
            qw = 1; //(Mg/m^3)
            Lamda = LatentHeatofVaporization();//(Mj/kg)
            D = SaturationVaporPressureTemperatureGradient();//(kPa/C)
            Rn = NetRadiation;
            G = 0; //(Mj m^-2 h^-1)
            //Defining Cn and Cd values depending on short or tall vegetation reference
            if (RVC == 0) { Cn = 900; Cd = 0.34; }
            else { Cn = 1600; Cd = 0.38; } //(K mm s^3/Mg/d) and (s/m),respectively
            Gama = PsychrometricConstant();
            //ea = 0.6108 * Math.Exp((17.27 * Tdw) / (Tdw + 237.3)); ; //(kPa) Equation 2.14 (R.Burman and L.O.Pochop)look example at page 196(vapor pressure e)            
            ea = 6.107799961 + 4.436518521 * Math.Pow(10, -1) * Tdw + 1.428945805 * Math.Pow(10, -2) * Math.Pow(Tdw, 2) + 2.650648471 * Math.Pow(10, -4) * Math.Pow(Tdw, 3) + 3.031240396 * Math.Pow(10, -6) * Math.Pow(Tdw, 4) + 2.034080948 * Math.Pow(10, -8) * Math.Pow(Tdw, 5) + 6.136820929 * Math.Pow(10, -11) * Math.Pow(Tdw, 6); //(in mb) Vapor pressure is determined from the dew point temperature using the polynomial provided by Lowe (1977).
            ea = 0.1 * ea; //mb to Kpa convertion.
            T = (TempMx + TempMn) / 2;// Temperature(C)
            es = SaturationVaporPressure();
            ETsz = (((1 / (qw * Lamda)) * D * (Rn - G)) + (Cn * Gama * (u2 / (T + 273)) * (es - ea))) / (D + Gama * (1 + Cd * u2));
            return ETsz;

        }
        public double LatentHeatofVaporization()
        {
            double Lamda, T;
            T = Temp;// Temperature(C) 
            Lamda = (2501 - 2.361 * T) / 1000; //Lamda:Latent heat of Vaporization(Mj/kg)//ASCE recomments 2.45 MJ/kg for Lamda.
            return Lamda;
        }
        public double SaturationVaporPressureTemperatureGradient()
        {
            double D, es, T;
            T = (TempMx + TempMn) / 2;// Temperature(C)
            es = 6.107799961 + 4.436518521 * Math.Pow(10, -1) * T + 1.428945805 * Math.Pow(10, -2) * Math.Pow(T, 2) + 2.650648471 * Math.Pow(10, -4) * Math.Pow(T, 3) + 3.031240396 * Math.Pow(10, -6) * Math.Pow(T, 4) + 2.034080948 * Math.Pow(10, -8) * Math.Pow(T, 5) + 6.136820929 * Math.Pow(10, -11) * Math.Pow(T, 6); //(in mb) Vapor pressure is determined from the dew point temperature using the polynomial provided by Lowe (1977).
            es = 0.1 * es; //mb to Kpa convertion.
            D = (4098 * es) / (Math.Pow((237.3 + T), 2)); //D:Saturation vapor pressure-temperature gradient(kPa/C)(Shuttleworth,1993)
            return D;
        }

        public double PsychrometricConstant()
        {
            double z, P, Gama, cp;
            z = Elev;//z:elevation(m)
            P = 101.3 * Math.Pow(((293 - 0.0065 * z) / 293), 5.256); //P:Atmospheric pressure (kPa)
            cp = (1.013) / 1000; // Specific heat of moist air (MJ/kg/C)
            double Lamda = LatentHeatofVaporization(); //(Mj/kg)
            Gama = (cp * P) / (0.622 * Lamda); //Gama:Psychrometric constant(kPa/C)
            return Gama;
        }
        public double SaturationVaporPressure()
        {
            double es;//es is calculated for daily time steps as the average of the saturation vapor pressure at max and min air temperature
            double T = TempMx;
            double esMx;
            //esMx = 0.6108 * Math.Exp((17.27 * T) / (T + 237.3));//(kPa) Equation 2.14 (R.Burman and L.O.Pochop)
            esMx = 6.107799961 + 4.436518521 * Math.Pow(10, -1) * T + 1.428945805 * Math.Pow(10, -2) * Math.Pow(T, 2) + 2.650648471 * Math.Pow(10, -4) * Math.Pow(T, 3) + 3.031240396 * Math.Pow(10, -6) * Math.Pow(T, 4) + 2.034080948 * Math.Pow(10, -8) * Math.Pow(T, 5) + 6.136820929 * Math.Pow(10, -11) * Math.Pow(T, 6); //(in mb) Vapor pressure is determined from the dew point temperature using the polynomial provided by Lowe (1977).
            esMx = 0.1 * esMx; //mb to Kpa convertion.
            T = TempMn;
            double esMn;
            //esMn = 0.6108 * Math.Exp((17.27 * T) / (T + 237.3));//(kPa) Equation 2.14 (R.Burman and L.O.Pochop)
            esMn = 6.107799961 + 4.436518521 * Math.Pow(10, -1) * T + 1.428945805 * Math.Pow(10, -2) * Math.Pow(T, 2) + 2.650648471 * Math.Pow(10, -4) * Math.Pow(T, 3) + 3.031240396 * Math.Pow(10, -6) * Math.Pow(T, 4) + 2.034080948 * Math.Pow(10, -8) * Math.Pow(T, 5) + 6.136820929 * Math.Pow(10, -11) * Math.Pow(T, 6); //(in mb) Vapor pressure is determined from the dew point temperature using the polynomial provided by Lowe (1977).
            esMn = 0.1 * esMn; //mb to Kpa convertion.
            es = (esMx + esMn) / 2; // Saturation Vapor Pressure (kPa)
            return es;
        }
    }

}






