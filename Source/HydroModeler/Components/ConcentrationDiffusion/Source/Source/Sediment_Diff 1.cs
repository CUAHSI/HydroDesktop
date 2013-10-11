using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMW;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;
using System.IO;
using OpenMI.Standard;
using System.Collections;

namespace Sediment_Diff
{
    public class Sediment_Diff : SMW.Wrapper
    {
        Dictionary<string, string> _InputExchangeItems = new Dictionary<string, string>();
        Dictionary<DateTime, double[,]> outputValues = new Dictionary<DateTime, double[,]>();
        string output_elementset; string output_quantity;
        double[,] cs; int rows_W; int cols_W; int rows_S; int cols_S; double _h; double _dt;
        double[,] cw; double u; double D;
        double[] diff_prev; double[,] adv_prev;
        double[] initial; List<double[,]> outv = new List<double[,]>();
        //bool IsFirstTimeStep = true;
        string outputPath = System.IO.Directory.GetCurrentDirectory() + "/output";
        int nt = 5;// number of earliest outputs need to be saved 

        public override void Finish()
        {
            //other output dropping the initial time step to match the saved output with that of water component
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }
            //System.IO.Directory.CreateDirectory("sediment2output");
            StreamWriter swaa = new StreamWriter(outputPath + "/sedimentDiffusion.csv");
            swaa.WriteLine("This is some info about the model....");
            DateTime start = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().Start).ModifiedJulianDay);
            DateTime end = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().End).ModifiedJulianDay);
            swaa.WriteLine("StartDate: , " + String.Format("{0:d/M/yyyy HH:mm:ss}", start));
            swaa.WriteLine("EndDate: , " + String.Format("{0:d/M/yyyy HH:mm:ss}", end));
            swaa.WriteLine("Time [hh:mm:ss]");
            swaa.WriteLine();
            foreach (KeyValuePair<DateTime, double[,]> kvp in outputValues)
            {

                string time = String.Format("{0:HH:mm:ss}", kvp.Key);
                swaa.WriteLine("\n" + time + ",");
                // StreamWriter sw = new StreamWriter("sedimentoutput/" + time + ".csv");
                for (int i = 0; i < kvp.Value.GetLength(0); i++)
                {
                    for (int j = 0; j < kvp.Value.GetLength(1); j++)
                    {
                        swaa.Write(kvp.Value[i, j].ToString() + ",");
                    }
                    swaa.Write("\n");
                }

            }

            swaa.Close();
        }
        public override void Initialize(System.Collections.Hashtable properties)
        {
            //string configFile = null; string output_elementset; string output_quantity;
            string configFile = null; string inputfile = null;

                //set initial conditions through *.omi file. (optional)
            if (properties.ContainsKey("ConfigFile"))
                configFile = (string)properties["ConfigFile"];

            if (properties.ContainsKey("Inputs"))
                inputfile = (string)properties["Inputs"];


            if (properties.ContainsKey("OutputDir"))
                outputPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), (string)properties["OutputDir"]);
           

            //lookup model's configuration file to determine interface properties
            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            int num_inputs = this.GetInputExchangeItemCount();
            if (inputfile != null)
            {
                //read input Ux,Uy,and concentration matrix
                StreamReader sr = new StreamReader(inputfile);
                string line = sr.ReadLine();
                u = Convert.ToDouble(line.Split(',')[1]);

                line = sr.ReadLine();
                D = Convert.ToDouble(line.Split(',')[1]);

                line = sr.ReadLine();
                rows_S = Convert.ToInt32(line.Split(',')[1]);

                line = sr.ReadLine();
                cols_S = Convert.ToInt32(line.Split(',')[1]);

                line = sr.ReadLine();
                rows_W = Convert.ToInt32(line.Split(',')[1]);

                line = sr.ReadLine();
                cols_W = Convert.ToInt32(line.Split(',')[1]);

                cs = new double[rows_S, cols_S]; cw = new double[rows_W, cols_W];

                //get elementset from smw
                ElementSet ein = (ElementSet)this.Inputs[0].ElementSet;
                ElementSet eout = (ElementSet)this.Outputs[0].ElementSet;

                //change element type from id to point
                ein.ElementType = ElementType.XYPoint;
                eout.ElementType = ElementType.XYPoint;

                //initialize cw to intial values (importating the data from the txt file(.models\sediment\inputs.csv)
                line = sr.ReadLine();
                int i = 0;
                while (line != null)
                {
                    string[] values = line.Split(',');
                    for (int j = 0; j <= values.Length - 1; j++)
                    {
                        cs[i, j] = Convert.ToDouble(values[j]);

                        //Build Element Set from first row only!
                        if (i == 0)
                        {
                            //create new element
                            Element e = new Element();
                            int x, y;
                            x = j;
                            y = -i;
                            Vertex v = new Vertex(x, y, 0);
                            e.AddVertex(v);

                            //add element to elementset
                            ein.AddElement(e);
                            eout.AddElement(e);
                        }
                    }
                    i++;
                    line = sr.ReadLine();
                }
            }
            else
            {

            }


            //output exchange item attributes
            int num_outputs = this.GetOutputExchangeItemCount();
            OutputExchangeItem output = this.GetOutputExchangeItem(num_outputs - 1);
            output_elementset = output.ElementSet.ID;
            output_quantity = output.Quantity.ID;

            //defining the equation constants
            _dt = this.GetTimeStep();
            _h = u * _dt / 0.85533;    // l / (rows - 2);

            //setup the intial value of diff_prev= upper row of cs since cs contain 9 rows
            diff_prev = new double[cs.GetUpperBound(1) + 1];
            for (int i = 0; i <= cs.GetLength(1) - 1; i++)
            {
                diff_prev[i] = cs[cs.GetUpperBound(0), i];
            }
            //this sets the initial cond.

            this.SetValues("Concentration", "sed", new ScalarSet(diff_prev));

            // intiating am matrix that contain the intial values to bypass the open mi setting
            initial = new double[cs.GetUpperBound(1) + 1];
            for (int i = 0; i <= cs.GetLength(1) - 1; i++)
            {
                initial[i] = cs[cs.GetUpperBound(0), i];
            }

            //calling the intial value of adv_prev=cw from water component
            adv_prev = new double[1, cw.GetUpperBound(1) + 1];
            for (int j = 0; j <= cw.GetLength(1) - 1; j++)
                adv_prev[0, j] = cw[cw.GetUpperBound(0), j];
        }
        public override bool PerformTimeStep()
        {
            //Defie current time step and the row of water component
            TimeStamp SedTime = (TimeStamp)this.GetCurrentTime();
            ScalarSet aa = (ScalarSet)this.GetValues("Concentration", "water");
            //set adv_prev to the imported values
            for (int i = 0; i <= cw.GetLength(1) - 1; i++)
            {
                adv_prev[0, i] = aa.data[i];
            }
            // specifies if the values should be saved.
            bool SaveValues = true;
            TimeStamp t = (TimeStamp)this.GetCurrentTime();
            DateTime time = CalendarConverter.ModifiedJulian2Gregorian(t.ModifiedJulianDay);

            // calculate concentration in sediment using diffusion mechanism from water
            cs = Calculate_concentration();

            //setup the value of diff_prev= upper row of cs since cs contain 9 rows
            for (int i = 0; i <= cs.GetLength(1) - 1; i++)
                diff_prev[i] = cs[cs.GetUpperBound(0), i];


            //save results for output during Finish()
            double[,] z = new double[rows_S, cols_S];
            if (SaveValues)
            {
                for (int i = 0; i <= cs.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j <= cs.GetLength(1) - 1; j++)
                    {
                        z[i, j] = cs[i, j];
                    }
                }

                //save results for output during Finish()
                outputValues.Add(time, z);
                outv.Add(z);
            }
            //setting boundary conditions so that they are available to water component.
            double[] outvals = new double[cs.GetLength(1)];
            for (int i = 0; i <= cs.GetLength(1) - 1; i++)
            {
                outvals[i] = cs[8, i];
            }
            this.SetValues("Concentration", "sed", new ScalarSet(outvals));
            this.AdvanceTime();

            return true;
        }

        public double[,] Calculate_concentration()
        {

            int r = cs.GetLength(0) - 1;
            int m = cs.GetLength(1) - 1;

            //calculate the upper row of sediment component linked with water component
            int i = r;

            for (int j = 1; j <= m - 1; j++)
            {
                cs[i, j] = diff_prev[j] + (D * _dt / Math.Pow(_h, 2.0)) * (cs[i - 1, j] + diff_prev[j + 1] + adv_prev[0, j] + diff_prev[j - 1] - 4 * diff_prev[j]);
            }

            //calculate the rest of rows of sediment component 
            for (i = r - 1; i >= 1; i--)
            {
                for (int j = 1; j <= m - 1; j++)
                {
                    cs[i, j] = cs[i, j] + (D * _dt / Math.Pow(_h, 2.0)) * (cs[i + 1, j] + cs[i, j + 1] + cs[i - 1, j] + cs[i, j - 1] - 4 * cs[i, j]);
                }
            }

            //create left and right boundary(cols)
            for (i = 0; i <= r; i++)
            {
                cs[i, 0] = cs[i, 1];
                cs[i, m] = cs[i, m - 1];
            }

            //create lower boundary(row)
            for (int j = 0; j <= m; j++)
            {
                cs[0, j] = cs[1, j];

            }

            return cs;
        }
        // storing previous values for higher order polynomial interpolation
        //public override OpenMI.Standard.ITimeStamp GetEarliestNeededTime()
        //{
        //    //get the current time
        //    TimeStamp time = (TimeStamp)this.GetCurrentTime();
        //    DateTime curr_time = CalendarConverter.ModifiedJulian2Gregorian(time.ModifiedJulianDay);

        //    //create a pseudo next time
        //    DateTime nextTime = curr_time.AddSeconds(this.GetTimeStep() * (nt - 1));

        //    //get the timestep as a timespan
        //    System.TimeSpan tspan = nextTime - curr_time;

        //    //calculate earliest needed time
        //    DateTime earliest = curr_time.Subtract(tspan);

        //    //return the desired time step
        //    if (CalendarConverter.Gregorian2ModifiedJulian(earliest) < this.GetTimeHorizon().Start.ModifiedJulianDay)
        //        return this.GetTimeHorizon().Start;
        //    else
        //        return new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(earliest));

        //}
    }
}
