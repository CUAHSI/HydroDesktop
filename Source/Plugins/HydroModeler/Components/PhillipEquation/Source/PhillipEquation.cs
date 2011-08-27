using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Xml;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;
using SMW;
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;

namespace PhillipEquation
{

    public class PhillipEquation : SMW.Wrapper
    {
        # region Global Variable
        //input parameters
        double Z; //Depth to water table[m]
        double S; //soil moisture storage
        double m_z;// deacy factor of conductivity with depth
        double p; //porosity deacy parameter
        double Pa_entry; //air pressure entery.
        double Pa_f;//air entery profile.
        double Ksat_surf; //saturated hydraulic conductivity at surface[m/day]
        double K_sat;//saturated hydraulic conductivity of soil profile[m/day]
        double P_surf; //porosity at surface
        double porosity; // porosity profile 
        double theta;
        double tp; //time to pond
        double Sp;//Sorptivity
        double duration;//time step in hrs
        List<double> PPT = new List<double>();
        string[] _output_elementset;
        string[] _output_quantity;
        bool isSoilSaturated = false;

        //output parameters
        Dictionary<DateTime, double> outputValues = new Dictionary<DateTime, double>();
        string[] _input_elementset;
        string[] _input_quantity;
        double infltration;
        double runoff;
        ArrayList q_infltration_outputs = new ArrayList();
        ArrayList Q_runoff_outputs = new ArrayList();
        ArrayList _DateTimes = new ArrayList();
        Dictionary<DateTime, double> infltration_outputValues = new Dictionary<DateTime, double>();
        Dictionary<DateTime, double> runoff_outputValues = new Dictionary<DateTime, double>();
        string outputPath = System.IO.Directory.GetCurrentDirectory() + "/output";
        # endregion

        public override void Finish()
        {
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            //System.IO.Directory.CreateDirectory("wateroutput");
            StreamWriter swa = new StreamWriter(outputPath + "/Infiltration.csv");
            swa.WriteLine("Daily infiltration....");
            DateTime start = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().Start).ModifiedJulianDay);
            DateTime end = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().End).ModifiedJulianDay);
            swa.WriteLine("StartDate: , " + String.Format("{0:MM/dd/yyyy hh:mm:ss}", start));
            swa.WriteLine("EndDate: , " + String.Format("{0:MM/dd/yyyy hh:mm:ss}", end));
            swa.WriteLine();
            swa.WriteLine("Time [0:MM/dd/yyyy hh:mm:ss], Infiltration");


            foreach (KeyValuePair<DateTime, double> kvp in outputValues)
            {

                string time = String.Format("{0:MM/dd/yyyy hh:mm:ss}", kvp.Key);
                swa.Write(time + ",");

                swa.Write(kvp.Value.ToString() + ",");

                swa.Write("\n");


            }
            swa.Close();
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            //Get config file path defined in sample.omi
            string Config = (string)properties["ConfigFile"];

            //read topographic input file
            string PPT_input = (string)properties["PPT"];

            //read model input parameters
            m_z = Convert.ToDouble(properties["m_z"]);
            Pa_entry = Convert.ToDouble(properties["Pa_entry"]);
            Z = Convert.ToDouble(properties["Z"]);
            S = Convert.ToDouble(properties["S"]);
           Ksat_surf = Convert.ToDouble(properties["Ksat_surf"]);
           P_surf = Convert.ToDouble(properties["P_surf"]);
           p = Convert.ToDouble(properties["p"]);

            //set OpenMI internal variables
            this.SetVariablesFromConfigFile(Config);

            // initialize a data structure to hold results
            this.SetValuesTableFields();

            //save input exchange item info 
            int num_inputs = this.GetInputExchangeItemCount();
            _input_elementset = new string[num_inputs];
            _input_quantity = new string[num_inputs];
            for (int i = 0; i < num_inputs; i++)
            {
                _input_elementset[i] = this.GetInputExchangeItem(i).ElementSet.ID;
                _input_quantity[i] = this.GetInputExchangeItem(i).Quantity.ID;
            }
            int num_outputs = this.GetOutputExchangeItemCount();

            _output_elementset = new string[num_outputs];
            _output_quantity = new string[num_outputs];
            for (int i = 0; i < num_outputs; i++)
            {
                _output_elementset[i] = this.GetOutputExchangeItem(i).ElementSet.ID;
                _output_quantity[i] = this.GetOutputExchangeItem(i).Quantity.ID;
            }
            if (PPT_input != null)
            {
                //read input Ux,Uy,and concentration matrix
                StreamReader sr = new StreamReader(PPT_input);
                string line = sr.ReadLine();
            
                while (line != null)
                {
                    string[] vals = line.Split(',');
                    for (int j = 0; j <= vals.Length - 1; j++)
                    {
                        
                        PPT.Add(Convert.ToDouble(vals[j]));
                    }
                    line = sr.ReadLine();
                }
            }
        }

        public override bool PerformTimeStep()
        {
            //Identifying the current time step & the start time and detrimine the current time step
            TimeStamp time = (TimeStamp)this.GetCurrentTime();
            DateTime curr_time = CalendarConverter.ModifiedJulian2Gregorian(time.ModifiedJulianDay);
            _DateTimes.Add(curr_time);
            TimeStamp times = (TimeStamp)this.GetTimeHorizon().Start;
            DateTime start_time = CalendarConverter.ModifiedJulian2Gregorian(times.ModifiedJulianDay);
            System.TimeSpan tspan = curr_time - start_time;
            duration = tspan.Days ;// Days conter
            int k = Convert.ToInt32(duration);


            //infiltration for unsaturated soil
            if (S < 1 && Ksat_surf > 0)
            {
                if (m_z > 0) { K_sat = m_z * Ksat_surf * (1 - Math.Exp(-Z / m_z) / Z); }

                else { K_sat = Ksat_surf; }

                if (p < 1000) { porosity = p * P_surf * (1 - Math.Exp(-Z / p)) / Z; }

                else { porosity = P_surf; }

                theta = S * porosity;

                //Estimate Sorptivity
                Pa_f = 0.76 * Pa_entry;

                Sp = Math.Sqrt(2) * K_sat * Pa_f;

                //calculate ponding time using Green & Ampt method
                if (PPT[k] > K_sat) { tp = K_sat * Pa_f * ((porosity - theta) / (PPT[k] * (PPT[k] - K_sat))); }

                else { tp = duration; }

                //calculate the infiltration 
                double t = duration - tp;
                if (duration <= tp) { infltration = PPT[k]; }
                else {infltration = Sp * Math.Pow(t,  0.5) + K_sat * 1.5 / 3.0 * t + tp * PPT[k];}
                infltration_outputValues.Add(curr_time,infltration);
            }

            else {infltration = 0;}
            if (isSoilSaturated)
            {
                double _dt = this.GetTimeStep() / 86400;
                Z = Z - (infltration /_dt);
                if (Z <= 0) { isSoilSaturated = true; Z = 0; } //need to add a moisture content function with time
            }

            //calculate the runoff using water balance
            runoff = PPT[k] - infltration;
            runoff_outputValues.Add(curr_time,runoff);
           
            //set the basin infiltration and runoff as outputs
            double[] infiltration_save = new double[1];
            infiltration_save[0] = infltration;// set the infiltration as an scalar output;
            string q1 = this.GetOutputExchangeItem(0).Quantity.ID;
            string e1 = this.GetOutputExchangeItem(0).ElementSet.ID;
            this.SetValues(q1, e1, new ScalarSet(infiltration_save));
            
            double[] runoff_save = new double[1];
            runoff_save[0] = runoff;
            string q2 = this.GetOutputExchangeItem(1).Quantity.ID;
            string e2 = this.GetOutputExchangeItem(1).ElementSet.ID;
            this.SetValues(q2, e2, new ScalarSet(runoff_save));

            this.AdvanceTime();
            return true;
        }
        # region to read input ruster 
        ///// <summary>
        ///// Reads an input raster ascii file containing topographic index to produce topographic index and topographic frequency arrays
        ///// </summary>
        ///// <param name="topographicIndex">ASCII raster file containing topographic index values</param>
        ///// <param name="ti">output topographic index array</param>
        ///// <param name="freq">output topographic frequency array</param>
        //public void read_topo_input(string topographicIndex, out double[] ti, out double[] freq)
        //{
        //    //---- begin reading the values stored in the topo file
        //    StreamReader sr = new StreamReader(topographicIndex);

        //    //-- read header info
        //    string line = null;
        //    for (int i = 0; i <= 4; i++)
        //        line = sr.ReadLine();

        //    //-- save the cellsize
        //    double cellsize = Convert.ToDouble(line.Split(' ')[line.Split(' ').Length - 1]);
        //    line = sr.ReadLine();

        //    //-- save the nodata value
        //    string nodata = line.Split(' ')[line.Split(' ').Length - 1];
        //    line = sr.ReadLine();

        //    //-- store all values != nodata in a list
        //    List<double> topoList = new List<double>();
        //    int lineNum = 0;
        //    while (!String.IsNullOrEmpty(line))
        //    {
        //        lineNum += 1;
        //        string[] vals = line.TrimEnd(' ').Split(' ');
        //        for (int i = 0; i <= vals.Length - 1; i++)
        //            if (vals[i] != nodata)
        //                topoList.Add(Convert.ToDouble(vals[i])); _watershedArea += cellsize;

        //        line = sr.ReadLine();
        //    }

        //}
        #endregion
    }
}