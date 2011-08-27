//--system Assemblies--
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Xml;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
//--openMI Assemblies--


//--Project Assemblies--
using SMW;
//--sharpMaps Assemblies--
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;
using Oatc.OpenMI.Sdk.Backbone;
//--QuickGraph Assemblies--
//using QuickGraph;

namespace TopModel
{
    public class TopModel : SMW.Wrapper
    {
        # region Global Variables
        //---GLOBAL VARIABLES  ----

        //---- model inputs
        double R;//subsurface Recharge rate [L/T]
        double c; //recession parameter (m)
        double Tmax; //Average effective transmissivity of the soil when the profile is just saturated
        double interception;//intial interciption of the watershed

        double[] TI;//topographic index
        double[] freq;//topographic index frequency

        double lamda_average;//average lamda
        double PPT_daily;
        double ET_daily;
        double q_overland;
        double q_subsurface;
        double q_infiltration;
        double S_average; //average saturation deficit

        double _watershedArea = 0; //area of the watershed. used to convert runoff into streamflow

        Dictionary<DateTime, double> Precip = new Dictionary<DateTime, double>();
        Dictionary<DateTime, double> ET = new Dictionary<DateTime, double>();
        Dictionary<DateTime, double> outputValues = new Dictionary<DateTime, double>();

        string[] _input_elementset;
        string[] _output_elementset;
        string[] _output_quantity;
        string[] _input_quantity;

        ArrayList _DateTimes = new ArrayList();
        ArrayList q_outputs = new ArrayList();
        ArrayList q_infltration_outputs = new ArrayList();

        string outputPath = System.IO.Directory.GetCurrentDirectory() + "/output";

        #endregion

        public TopModel()
        {
            _input_elementset = null;
            _input_quantity = null;
            _output_elementset = null;
            _output_quantity = null;

        }

        public override void Finish()
        {
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            //System.IO.Directory.CreateDirectory("wateroutput");
            StreamWriter swa = new StreamWriter(outputPath + "/Runoff.csv");
            swa.WriteLine("Daily Runoff....");
            DateTime start = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().Start).ModifiedJulianDay);
            DateTime end = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().End).ModifiedJulianDay);
            swa.WriteLine("StartDate: , " + String.Format("{0:MM/dd/yyyy hh:mm:ss}", start));
            swa.WriteLine("EndDate: , " + String.Format("{0:MM/dd/yyyy hh:mm:ss}", end));
            swa.WriteLine();
            swa.WriteLine("Time [0:MM/dd/yyyy hh:mm:ss], Runoff");


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
            string configFile = (string)properties["ConfigFile"];

            //read topographic input file
            string topo_input = (string)properties["TI"];

            //read model input parameters
            c = Convert.ToDouble(properties["m"]);
            Tmax = Convert.ToDouble(properties["Tmax"]);
            R = Convert.ToDouble(properties["R"]);
            interception = Convert.ToDouble(properties["Interception"]);
            _watershedArea = Convert.ToDouble(properties["WatershedArea_SquareMeters"]);

            //set OpenMI internal variables
            this.SetVariablesFromConfigFile(configFile);

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

            //read topographic indices from input file
            read_topo_input(topo_input, out TI, out freq);

            //---- calculate saturation deficit
            //calculate lamda average for the watershed
            double[] TI_freq = new double[TI.GetLength(0)];

            for (int i = 0; i <= TI.GetLength(0) - 1; i++)
            {
                TI_freq[i] = TI[i] * freq[i];
            }

            lamda_average = TI_freq.Sum() / freq.Sum();

            //catchement average saturation deficit(S_bar)
            double S_bar = -c * ((Math.Log(R / Tmax)) + lamda_average);
            S_average = S_bar;

        }
        public override bool PerformTimeStep()
        {
            //reading the appropriate value from PPT & PET dictionary 
            ScalarSet input_pet = (ScalarSet)this.GetValues(_input_quantity[1], _input_elementset[1]);  //PET
            ScalarSet input_precip = (ScalarSet)this.GetValues(_input_quantity[0], _input_elementset[0]);  //Rainfall

            for (int i = 0; i < input_pet.Count; i++)
            {
                ET_daily = input_pet.data[i];
            }
            for (int h = 0; h < input_precip.Count; h++)
            {
                PPT_daily = input_precip.data[h];
            }

            //declaring the flow matrices here since they are related with the size of input matrices
            double[] S_d = new double[TI.GetLength(0)];
            double[] over_flow = new double[TI.GetLength(0)]; //Infiltration excess
            double[] reduced_ET = new double[TI.GetLength(0)];//Reduced ET due to dryness

            //calculate the saturation deficit for each TIpoint 
            double[] S = new double[TI.GetLength(0)];
            for (int j = 0; j <= TI.GetLength(0) - 1; j++)
            {
                S[j] = S_average + c * (lamda_average - TI[j]);
            }

            //remove the interception effect from PPT matrix, and update the saturation deficit matrix, calculating q_infiltration
            PPT_daily = Math.Max(0, (PPT_daily - interception)); // 
            for (int m = 0; m <= TI.GetLength(0) - 1; m++)
            {
                S[m] = S[m] - PPT_daily + ET_daily;
            }
            q_infiltration = PPT_daily - ET_daily;
            double[] MM = new double[TI.GetLength(0)];
            if ((PPT_daily - ET_daily) > 0)
            {
                //create a list for S values<0 
                for (int m = 0; m <= TI.GetLength(0) - 1; m++)
                {
                    if (S[m] < 0) { over_flow[m] = -S[m]; S[m] = 0; }
                    else { over_flow[m] = 0; }
                    MM[m] = freq[m] * over_flow[m];
                }
            }
            else
            {
                double[] NN = new double[TI.GetLength(0)];
                for (int m = 0; m <= TI.GetLength(0) - 1; m++)
                {
                    if (S[m] > 5000) { reduced_ET[m] = -5000 + S[m]; S[m] = 5000; } //KK.Add(S[m]);
                    else { reduced_ET[m] = 0; }
                    NN[m] = freq[m] * reduced_ET[m];
                }

                q_infiltration = q_infiltration + ((NN.Sum()) / (freq.Sum()));
            }
            q_subsurface = Tmax * (Math.Exp(-lamda_average)) * (Math.Exp(-S_average / c));
            q_overland = (MM.Sum()) / (freq.Sum());

            //calculate the new average deficit using cachement mass balance
            S_average = S_average + q_subsurface + q_overland - q_infiltration;

            //calculating runoff q
            double q = q_overland + q_subsurface;

            //Storing values of DateTimes and surface runoff values
            TimeStamp time = (TimeStamp)this.GetCurrentTime();
            DateTime curr_time = CalendarConverter.ModifiedJulian2Gregorian(time.ModifiedJulianDay);
            _DateTimes.Add(curr_time);
            q_outputs.Add(q);
            q_infltration_outputs.Add(q_infiltration);
            outputValues.Add(curr_time, q);

            //save runoff
            double[] runoff = new double[1];
            runoff[0] = q;// *_watershedArea;

            //-- calculate streamflow using watershed area
            double[] streamflow = new double[1];
            streamflow[0] = q * (_watershedArea) / 86400;



            //set the basin outflow as runoff output
            string q1 = this.GetOutputExchangeItem(0).Quantity.ID;
            string e1 = this.GetOutputExchangeItem(0).ElementSet.ID;
            this.SetValues(q1, e1, new ScalarSet(runoff));

            string q2 = this.GetOutputExchangeItem(1).Quantity.ID;
            string e2 = this.GetOutputExchangeItem(1).ElementSet.ID;
            this.SetValues(q2, e2, new ScalarSet(streamflow));
            

            this.AdvanceTime();
            return true;
        }

        /// <summary>
        /// Reads an input raster ascii file containing topographic index to produce topographic index and topographic frequency arrays
        /// </summary>
        /// <param name="topographicIndex">ASCII raster file containing topographic index values</param>
        /// <param name="ti">output topographic index array</param>
        /// <param name="freq">output topographic frequency array</param>
        public void read_topo_input(string topographicIndex, out double[] ti, out double[] freq)
        {
            //---- begin reading the values stored in the topo file
            StreamReader sr = new StreamReader(topographicIndex);

            //-- read header info
            string line = null;
            for (int i=0; i<=4; i++)
                line = sr.ReadLine();

            //-- save the cellsize
            double cellsize = Convert.ToDouble(line.Split(' ')[line.Split(' ').Length - 1]);
            line = sr.ReadLine();

            //-- save the nodata value
            string nodata = line.Split(' ')[line.Split(' ').Length-1];
            line = sr.ReadLine();

            //-- store all values != nodata in a list
            List<double> topoList = new List<double>();
            int lineNum = 0;
            while (!String.IsNullOrEmpty(line))
            {
                lineNum += 1;
                string[] vals = line.TrimEnd(' ').Split(' ');
                for (int i = 0; i <= vals.Length - 1; i++)
                    if (vals[i] != nodata)
                        topoList.Add(Convert.ToDouble(vals[i])); _watershedArea += cellsize;
                        
                line = sr.ReadLine();
            }

            //---- calculate frequency of each topographic index
            //-- consolidate topo list into unique values 
            Dictionary<double, double> d = new Dictionary<double, double>();
            foreach (double t in topoList)
                if (d.ContainsKey(t))
                    d[t] += 1.0;
                else
                    d.Add(t, 1.0);

            //-- calculate topo frequency, then return both topographic index and topo frequency arrays
            double total = (double)topoList.Count;
            ti = new double[d.Count];
            freq = new double[d.Count];
            int index = 0;
            foreach (KeyValuePair<double, double> pair in d)
            {
                ti[index] = Math.Round(pair.Key,4);
                freq[index] = Math.Round(d[pair.Key] / total, 10);
                index ++;
            }
        }
    }
    
}

  