// Purpose: Evaluate Net Solar Radiation in MJ/m^2/d from Tempreture, date and elevation data.
// Author: Mehmet Ercan (mehmetbercan@gmail.com)
// Advisor: Jonathan L. Goodall (goodall@sc.edu)
// History: Created (03-15-2010), put in OpenMI(06-07-2010), updated(05-06-2011)

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

namespace N_SolarRadiation
{
    public class N_SR : Wrapper
    {
        public DataTable _elementValues = new DataTable();

        public string input_quantity0;
        public string input_quantity1;
        public string input_quantity2;
        public string input_quantity3;
        public string input_quantity4;
        public string input_elementset0;
        public string input_elementset1;
        public string input_elementset2;
        public string input_elementset3;
        public string input_elementset4;

        public string output_quantity;
        public string output_elementset;

        //defining data storage list
        List<List<double[]>> Allwtmp = new List<List<double[]>>();
        double[] TemprtrDew;
        double[] TemprtrMax;
        double[] TemprtrMin;
        double[] TemprtrMinAd;
        double[] MnthADaTemp;


        public override void Finish()
        {
        }


        public override void Initialize(System.Collections.Hashtable properties)
        {
            string configFile = null;
            //Get Config file directory from .omi file
            if (properties.ContainsKey("ConfigFile"))
                configFile = (string)properties["ConfigFile"];

            //Set variables
            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            //get input exchange item attributes
            int num_inputs = this.GetInputExchangeItemCount();
            InputExchangeItem input0 = this.GetInputExchangeItem(num_inputs - 5);
            input_elementset0 = input0.ElementSet.ID;
            input_quantity0 = input0.Quantity.ID;
            InputExchangeItem input1 = this.GetInputExchangeItem(num_inputs - 4);
            input_elementset1 = input1.ElementSet.ID;
            input_quantity1 = input1.Quantity.ID;
            InputExchangeItem input2 = this.GetInputExchangeItem(num_inputs - 3);
            input_elementset2 = input2.ElementSet.ID;
            input_quantity2 = input2.Quantity.ID;
            InputExchangeItem input3 = this.GetInputExchangeItem(num_inputs - 2);
            input_elementset3 = input3.ElementSet.ID;
            input_quantity3 = input3.Quantity.ID;
            InputExchangeItem input4 = this.GetInputExchangeItem(num_inputs - 1);
            input_elementset4 = input4.ElementSet.ID;
            input_quantity4 = input4.Quantity.ID;

            //get output exchange item attributes
            int num_outputs = this.GetOutputExchangeItemCount();
            OutputExchangeItem output = this.GetOutputExchangeItem(num_outputs - 1);
            output_elementset = output.ElementSet.ID;
            output_quantity = output.Quantity.ID;

            // --- setup elementValues DataTable to store element attributes from elements.shp
            _elementValues.Columns.Add("Gauge_IDs", typeof(double));
            _elementValues.Columns.Add("Longitude", typeof(double));
            _elementValues.Columns.Add("Latitude", typeof(double));
            _elementValues.Columns.Add("Alpha", typeof(double));
            _elementValues.Columns.Add("Atm_Coeff", typeof(double));
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
                object Alpha = feat.ItemArray[feat.Table.Columns.IndexOf("Alpha")];
                object Coeff_a = feat.ItemArray[feat.Table.Columns.IndexOf("Coeff_a")];
                object Elevation = feat.ItemArray[feat.Table.Columns.IndexOf("Elevation")];
                _elementValues.LoadDataRow(new object[] { GaugeId, Longitute, Latitude, Alpha, Coeff_a, Elevation }, true);


            }

        }
        public override bool PerformTimeStep()
        {
            //Retrieve values from another component
            ScalarSet _TemprtrDew = (ScalarSet)this.GetValues(input_quantity0, input_elementset0);
            TemprtrDew = _TemprtrDew.data;
            ScalarSet _TemprtrMax = (ScalarSet)this.GetValues(input_quantity1, input_elementset1);
            TemprtrMax = _TemprtrMax.data;
            ScalarSet _TemprtrMin = (ScalarSet)this.GetValues(input_quantity2, input_elementset2);
            TemprtrMin = _TemprtrMin.data;
            ScalarSet _TemprtrMinAd = (ScalarSet)this.GetValues(input_quantity3, input_elementset3);
            TemprtrMinAd = _TemprtrMinAd.data;
            ScalarSet _MnthADaTemp = (ScalarSet)this.GetValues(input_quantity4, input_elementset4);
            MnthADaTemp = _MnthADaTemp.data;
            /////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////Net Solar Radiation Calculation////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////

            //Get data from saved attribute table
            int n = _elementValues.Rows.Count;
            double[] _gauge_ids = new double[n];
            double[] _lon = new double[n];
            double[] _lat = new double[n];
            double[] _alpha = new double[n];
            double[] _coeffa = new double[n];
            double[] _elev = new double[n];
            for (int i = 0; i < n; i++)
            {
                _gauge_ids[i] = Convert.ToDouble(_elementValues.Rows[i]["Gauge_IDs"]);
                _lon[i] = Convert.ToDouble(_elementValues.Rows[i]["Longitude"]);
                _lat[i] = Convert.ToDouble(_elementValues.Rows[i]["Latitude"]);
                _alpha[i] = Convert.ToDouble(_elementValues.Rows[i]["Alpha"]);
                _coeffa[i] = Convert.ToDouble(_elementValues.Rows[i]["Atm_Coeff"]);
                _elev[i] = Convert.ToDouble(_elementValues.Rows[i]["Elevation"]);
            }

            //get current time
            DateTime currTime = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetCurrentTime()).ModifiedJulianDay);
            double current_timed = Convert.ToDouble(Convert.ToString(currTime.Month) + Convert.ToString(currTime.Day) + Convert.ToString(currTime.Year));
            string current_time = Convert.ToString(currTime).Split(' ')[0];

            //Caluculate Julian day
            int Dm = Convert.ToInt32(current_time.Split('/')[1]); //the day of the month
            int M = Convert.ToInt32(current_time.Split('/')[0]); //the month
            int Y = Convert.ToInt32(current_time.Split('/')[2]); //the year
            double my4 = Y - (Math.Floor(Y / 4.0) * 4);
            double JulianDay = Dm - 32 + Math.Floor(275.0 * M / 9.0) + 2 * Math.Floor(3.0 / (M + 1)) + Math.Floor((M / 100.0) - (my4 / 4.0) + 0.975); //Allen et al.,2005

            NSRworks In = new NSRworks();

            

            //Setting values for the stations
            List<double[]> StIDNSR = new List<double[]>();
            for (int i = 0; i < n; i++)
            {
                //Setting Maximum Temperature(C)
                In.TempMx = TemprtrMax[i];

                //Setting Minimum Temperature(C)
                In.TempMn = TemprtrMin[i];

                //Setting Minimum Temperature for next day(C)
                In.TempMnn = TemprtrMinAd[i];

                //Setting Dew Point Temperature(C)
                In.Tdew = TemprtrDew[i];

                //Monthly mean daily average temperature range (C)
                In.MMAvTempR = MnthADaTemp[i];

                //Setting surface albeto
                In.Alpha = _alpha[i];

                //Setting Temperature(C)
                In.Lat = _lat[i];

                //Setting watershed elevation(m)
                In.Elev = _elev[i];

                //Set Julian Day
                In.Julian = JulianDay;

                //Setting the coefficient a to evaluate atmospheric transmisivity
                In.ca = _coeffa[i];

                //Getting Calculated Net Solar Radiation
                double[] NSR1 = new double[6];
                NSR1[0] = _gauge_ids[i]; //StationID
                NSR1[1] = _lat[i]; //Latitude
                NSR1[2] = _lon[i]; //Longitude
                NSR1[3] = _elev[i]; //Elevation
                NSR1[4] = In.NetRadiation(); //Net Solar Radiation (MJ/m^2/d)
                NSR1[5] = current_timed; //Date
                StIDNSR.Add(NSR1);
            }


            //Get an array result for exchange with SMW
            double[] NSR = new double[StIDNSR.Count()];
            for (int ff = 0; ff < StIDNSR.Count(); ff++)
            {
                NSR[ff] = StIDNSR[ff][4];
            }

            //set values
            this.SetValues(output_quantity, output_elementset, new ScalarSet(NSR));

            //set advance model trough time 
            this.AdvanceTime();

            return true;
        }
    }
    class NSRworks
    {
        //Setting Max and Min Temperature value(degrees Celsius)
        double _tempmx;
        public double TempMx { get { return _tempmx; } set { _tempmx = value; } }
        double _tempmn;
        public double TempMn { get { return _tempmn; } set { _tempmn = value; } }
        //Next day's min Temprature value (degrees Celsius)
        double _tempmnn;
        public double TempMnn { get { return _tempmnn; } set { _tempmnn = value; } }
        //Setting Dew point temperature(degrees Celsius)
        double _Tdew;
        public double Tdew { get { return _Tdew; } set { _Tdew = value; } }
        //Setting monthly mean daily average temperature range(degrees Celsius)
        double _MMAvTempR;
        public double MMAvTempR { get { return _MMAvTempR; } set { _MMAvTempR = value; } }
        //Setting surface albeto
        double _alpha;
        public double Alpha { get { return _alpha; } set { _alpha = value; } }
        //setting the latitude(in degree)
        double _lat;
        public double Lat { get { return _lat; } set { _lat = value; } }
        //Setting Elevation(in meter)
        double _elev;
        public double Elev { get { return _elev; } set { _elev = value; } }
        //Setting Julian Days
        double _Julian;
        public double Julian { get { return _Julian; } set { _Julian = value; } }
        //Setting the coefficient a to evaluate atmospheric transmisivity
        double _ca;
        public double ca { get { return _ca; } set { _ca = value; } }

        public double NetRadiation()
        {
            //Net Longwave Radiation (Ln)
            double ae, be, ed, Ep, cf, Tf, DTa, a, b, c, DT, Ln, Theta, Tmax, Tmin, Tminn;
            //Coefficients: ae=0.34, be=-0.14, a=0.8(Dr.Tarboton), a = 0.75 (considering Bristowet al.,1984), c=2.4, Theta=2.0747*10^-7 (kjK^-4m^-2h^-1) //ed:Vapor Pressure(kPa)
            //Ep:Net emisivity//cf:cloudiness factor//Tf:Atmospheric transmissivity// DTa:Mountly mean diurnal temprature range
            //b:a parameter depends on DTa//Dt:diurnal temprature range//Ln:Net Longwave Radiation
            //Tmax and Tmin are dailly max and min tempratures
            Tmax = TempMx; Tmin = TempMn; //dailly max and min tempratures
            Tminn = TempMnn;  //Next day's min temprature
            DT = Math.Abs(Tmax - ((Tmin + Tminn) / 2));  //diurnal temprature range (celsius)(Bristow et al.,1984)
            DTa = MMAvTempR; // monthly mean average temperature range.(Monthly mean diurnal temprature range)
            double Tdw = Tdew;
            a = ca;//get the coefficient a for atmospheric transmisivity calculation.
            ae = 0.34; be = -0.14; c = 2.4; Theta = 4.901 * Math.Pow(10, -9); //(Mj * K^-4 * m^-2 * d^-1)(Allen et al.,2005)
            //ed = 0.6108 * Math.Exp((17.27 * Tdw) / (Tdw + 237.3)); //(kPa)(ed = ea)Equation 2.14 (R.Burman and L.O.Pochop)look example at page 196(vapor pressure e)
            ed = 6.107799961 + 4.436518521 * Math.Pow(10, -1) * Tdw + 1.428945805 * Math.Pow(10, -2) * Math.Pow(Tdw, 2) + 2.650648471 * Math.Pow(10, -4) * Math.Pow(Tdw, 3) + 3.031240396 * Math.Pow(10, -6) * Math.Pow(Tdw, 4) + 2.034080948 * Math.Pow(10, -8) * Math.Pow(Tdw, 5) + 6.136820929 * Math.Pow(10, -11) * Math.Pow(Tdw, 6); //(in mb) Vapor pressure is determined from the dew point temperature using the polynomial provided by Lowe (1977).
            ed = 0.1 * ed; //mb to Kpa convertion.
            Ep = ae + be * (Math.Pow(ed, 0.5));
            b = 0.036 * Math.Exp(-0.154 * DTa);
            Tf = a * (1 - Math.Exp(-b * (Math.Pow(DT, c))));
            //cf = Tf / 0.8; //(Tarboton used in his model)(This always does not provide the condition explained by Allen et al., 2005)
            double elev = Elev;
            double Rs_Rso = Tf / (0.75 + 2 * Math.Pow(10, -5) * elev); // got this formula by combining Bristow et al.,1984 and Allen et al,2005.
            if (Rs_Rso < 0.3) { Rs_Rso = 0.3; }
            if (Rs_Rso > 1.0) { Rs_Rso = 1.0; }
            cf = 1.35 * Rs_Rso - 0.35; //(shottleworth, 1993 and Allen et al.,2005)

            Ln = -cf * Ep * Theta * ((Math.Pow((Tmax + 273.16), 4) + Math.Pow((Tmin + 273.16), 4)) / 2); //Net longwave radiation(MJ/m^2/h)

            //Net Shortwave Radiation (Sn)
            double So, alpha, Sn;//Alpha:The surface albedo which is derived from surface vegetation
            //Sn:Net Shortwave Radiation
            //Getting So from table 2-5 (R.Burman and L.O.Pochop page:40) 
            //So:Extraterrestrial radiation at the top of the atmosphere calculated based on sun angles from the date

            double Gsc = 4.92; // Solar constant (MJ/m^2/h)(In ASCE appendix example, houly Gsc is used for daily calculation and this constant given in daily calculation in the paper)
            double latitude = (Math.PI / 180) * Lat; // Latitude in Radians 
            double Jd = Julian;
            double dr = 1 + 0.033 * Math.Cos((2 * Math.PI / 365) * Jd); //Inverse relative distance factor (unitless)
            double SlrDec = 0.409 * Math.Sin(((2 * Math.PI / 365) * Jd) - 1.39); //Solar declination in radians
            double Ws = Math.Acos(-Math.Tan(latitude) * Math.Tan(SlrDec)); // Sunset Hour Angles in Radians
            So = (24 / Math.PI) * Gsc * dr * (Ws * Math.Sin(latitude) * Math.Sin(SlrDec) + Math.Cos(latitude) * Math.Cos(SlrDec) * Math.Sin(Ws)); //Extraterrestrial radiation (MJ/m^2/h)(Allen et al.,2005)

            //Getting surface albeto(alpha) value can be got from table 2-2 (R.Burman and L.O.Pochop page:31)
            alpha = Alpha;// Suface albeto
            Sn = (1 - alpha) * Tf * So;

            //Net Radiation
            double Rn; //Rn:Net Radiation
            Rn = Sn + Ln;
            return Rn;
        }
    }
}
