using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMW;
using Oatc.OpenMI.Sdk.Wrapper;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using System.IO;

namespace Hargreaves
{
    public class Engine : SMW.Wrapper
    {
        public string[] input_quantity;
        public string output_quantity;
        public string[] input_elementset;
        public string output_elementset;
        Dictionary<DateTime, double[]> _output;
        string output_path = System.IO.Directory.GetCurrentDirectory() + "/output";

        public Engine()
        {
            _output = new Dictionary<DateTime,double[]>();
        }

        public override void Finish()
        {
            if (!System.IO.Directory.Exists(output_path))
            {
                System.IO.Directory.CreateDirectory(output_path);
            }

            StreamWriter sw = new StreamWriter(output_path + "./hargreaves_output.txt", false);

            //write header line
            sw.WriteLine("Simulation Time, PET[mm/day]");

            //write all values
            foreach (KeyValuePair<DateTime, double[]> kvp in _output)
            {
                sw.Write(String.Format("{0:MM/dd/yyyy: hh:mm tt}", kvp.Key));
                for (int i = 0; i <= kvp.Value.Length - 1; i++)
                    sw.Write("," + kvp.Value[i]);
                sw.Write("\n");
            }

            //close file
            sw.Close();
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            //---- get configuration data
            string config = null;

            if (properties.ContainsKey("ConfigFile"))
                config = properties["ConfigFile"].ToString();
            else
                throw new Exception("A configuration file must be supplied for the Hargreaves component!!!");

            if (properties.ContainsKey("Output"))
                output_path = properties["Output"].ToString();

            //---- set smw parameters
            this.SetVariablesFromConfigFile(config);
            this.SetValuesTableFields();

            //---- get exhange item attributes
            //-- input exchange items
            int num_inputs = this.GetInputExchangeItemCount();
            input_elementset = new string[num_inputs];
            input_quantity = new string[num_inputs];
            for(int i=0; i<= num_inputs-1; i++)
            {
                InputExchangeItem input = this.GetInputExchangeItem(i);
                input_elementset[i] = input.ElementSet.ID;
                input_quantity[i] = input.Quantity.ID;
            }

            //-- output exchange items
            int num_outputs = this.GetOutputExchangeItemCount();
            OutputExchangeItem output = this.GetOutputExchangeItem(num_outputs - 1);
            output_elementset = output.ElementSet.ID;
            output_quantity = output.Quantity.ID;


        }

        public override bool PerformTimeStep()
        {
            //---- get input data
            //-- temp
            double[] temp = ((ScalarSet)this.GetValues(input_quantity[0], input_elementset[0])).data;
            //-- max temp
            double[] maxtemp = ((ScalarSet)this.GetValues(input_quantity[1], input_elementset[1])).data;
            //-- min temp
            double[] mintemp = ((ScalarSet)this.GetValues(input_quantity[2], input_elementset[2])).data;

            //---- calculate pet for each element
            //-- get the number of elements (assuming that they're all the same)
            int elemcount = this.GetInputExchangeItem(0).ElementSet.ElementCount;
            double[] pet = new double[elemcount];
            for (int i = 0; i <= elemcount - 1; i++)
            {
                pet[i] = CalculatePET(temp[i], mintemp[i], maxtemp[i], i);
            }

            //---- save output values
            DateTime dt = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetCurrentTime()).ModifiedJulianDay);
            _output.Add(dt, pet);

            //---- set output values
            this.SetValues(output_quantity, output_elementset, new ScalarSet(pet));

            //---- advance to the next timestep
            this.AdvanceTime();

            return true;
        }

        /// <summary>
        /// Calculates the potential evapotranspiration using the Hargreaves-Samani method
        /// </summary>
        /// <param name="T">Averaged daily temperature</param>
        /// <param name="Tmin">Minimum daily temperature</param>
        /// <param name="Tmax">Maximum daily temperature</param>
        /// <param name="e">element index</param>
        /// <returns>PET in mm/day</returns>
        public double CalculatePET(double T, double Tmin, double Tmax, int eid)
        {

            //calc Ra from http://www.civil.uwaterloo.ca/watflood/Manual/02_03_2.htm

            //---- calculate the relative distance between the earth and sun
            //-- get julien day
            TimeStamp ts = (TimeStamp)this.GetCurrentTime();
            DateTime dt = CalendarConverter.ModifiedJulian2Gregorian(ts.ModifiedJulianDay);
            int j = dt.DayOfYear;
            double dr = 1 + 0.033 * Math.Cos((2 * Math.PI * j) / 365);

            //---- calculate the solar declination
            double d = 0.4093 * Math.Sin((2 * Math.PI * j) / 365 - 1.405);

            //---- calculate the sunset hour angle
            //-- get latitude in degrees
            ElementSet es = (ElementSet)this.GetInputExchangeItem(0).ElementSet;
            Element e = es.GetElement(eid);
            double p = e.GetVertex(0).y * Math.PI / 180;
            //-- calc ws
            double ws = Math.Acos(-1 * Math.Tan(p) * Math.Tan(d));

            //---- calculate the total incoming extra terrestrial solar radiation (tested against http://www.engr.scu.edu/~emaurer/tools/calc_solar_cgi.pl)
            double Ra = 15.392 * dr * (ws * Math.Sin(p) * Math.Sin(d) + Math.Cos(p) * Math.Cos(d) * Math.Sin(ws));

            //---- calculate PET (From Hargreaves and Samani 1985)
            //-- calculate latent heat of vaporization (from Water Resources Engineering, David A. Chin)
            double L = 2.501 - 0.002361 * T;
            double PET = (0.0023 * Ra * Math.Sqrt(Tmax - Tmin) * (T + 17.8)) / L;

            return PET;
            
        }
    }
}
