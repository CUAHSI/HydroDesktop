using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;

namespace Water_adv
{
    public class Water_adv : SMW.Wrapper
    {
        Dictionary<string, string> _InputExchangeItems = new Dictionary<string, string>();
        Dictionary<DateTime, double[,]> outputValues = new Dictionary<DateTime, double[,]>();
        string output_elementset; string output_quantity;
        double[,] cw; double[,] cs; int rows_W; int cols_W; int rows_S; int cols_S; double _h; double _dt;
        double u; double D; //double[,] adv_prev = null;
        double[] diff_prev; double[,] adv_prev; double[,] initial;
        // bool IsFirstTimeStep = true; //checks to see if it is the first timestep
        string outputPath = System.IO.Directory.GetCurrentDirectory() + "/output";
        int nt = 1;// number of earliest outputs need to be saved

        public override void Finish()
        {
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            //System.IO.Directory.CreateDirectory("wateroutput");
            StreamWriter swa = new StreamWriter(outputPath + "/waterAdvection.csv");
            swa.WriteLine("This is some info about the model....");
            DateTime start = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().Start).ModifiedJulianDay);
            DateTime end = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetTimeHorizon().End).ModifiedJulianDay);
            swa.WriteLine("StartDate: , " + String.Format("{0:d/M/yyyy HH:mm:ss}", start));
            swa.WriteLine("EndDate: , " + String.Format("{0:d/M/yyyy HH:mm:ss}", end));
            swa.WriteLine();
            swa.WriteLine("Time [H:mm:ss], Concentration");
            foreach (KeyValuePair<DateTime, double[,]> kvp in outputValues)
            {

                string time = String.Format("{0:HH:mm:ss}", kvp.Key);
                swa.Write(time + ",");
                //StreamWriter sw = new StreamWriter("wateroutput/" + time + ".csv");
                for (int i = 0; i < kvp.Value.GetLength(0); i++)
                {
                    for (int j = 0; j < kvp.Value.GetLength(1); j++)
                    {
                        swa.Write(kvp.Value[i, j].ToString() + ",");
                    }
                    swa.Write("\n");
                }

            }
            swa.Close();
        }
        public override void Initialize(System.Collections.Hashtable properties)
        {
            string configFile = null; string U = null; string d = null; string C = null; string inputfile= null;

            //set initial conditions through *.omi file. (optional)
            if (properties.ContainsKey("ConfigFile"))
                configFile = (string)properties["ConfigFile"];

            if (properties.ContainsKey("Inputs"))
                inputfile = (string)properties["Inputs"];

            if(properties.ContainsKey("OutputDir")) 
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
                rows_W = Convert.ToInt32(line.Split(',')[1]);

                line = sr.ReadLine();
                cols_W = Convert.ToInt32(line.Split(',')[1]);

                line = sr.ReadLine();
                rows_S = Convert.ToInt32(line.Split(',')[1]);

                line = sr.ReadLine();
                cols_S = Convert.ToInt32(line.Split(',')[1]);

                cw = new double[rows_W, cols_W]; cs = new double[rows_S, cols_S];

                //get input and output element sets from the SMW
                ElementSet ein = (ElementSet)this.Inputs[0].ElementSet;
                ElementSet eout = (ElementSet)this.Outputs[0].ElementSet;

                //set some element set properties
                ein.ElementType = OpenMI.Standard.ElementType.XYPoint;
                eout.ElementType = OpenMI.Standard.ElementType.XYPoint;

                //initialize cw to intial values (importating the data from the txt file(.models\water\inputs.csv))
                line = sr.ReadLine();
                int i = 0;
                while (line != null)
                {
                    string[] values = line.Split(',');

                    for (int j = 0; j <= values.Length - 1; j++)
                    {
                        cw[i, j] = Convert.ToDouble(values[j]);

                        //create element
                        Element e = new Element();
                        Vertex v1 = new Vertex(j, i, 0);
                        e.AddVertex(v1);

                        //add element to elementset 
                        ein.AddElement(e);
                        eout.AddElement(e);
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
            _h = u * _dt / 0.85533;

            //setup the intial value of adv_prev=cw since cw contain 1 row only
            adv_prev = cw;

            //this sets the initial cond.
            double[] outvals = new double[cw.GetLength(1)];
            for (int i = 0; i <= cw.GetLength(1) - 1; i++)
            {
                outvals[i] = cw[cw.GetLength(0) - 1, i];
            }
            this.SetValues("Concentration", "water", new ScalarSet(outvals));

            //setup initial value to be used once to bypass the first step due to openmi setting
            initial = new double[rows_W, cols_W];
            for (int i = 0; i <= cw.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= cw.GetLength(1) - 1; j++)
                {
                    initial[i, j] = cw[i, j];
                }
            }

            //calling the intial value of diff_prev=upper row of cs from sediment component 
            diff_prev = new double[cs.GetUpperBound(1) + 1];
            for (int i = 0; i <= cs.GetLength(1) - 1; i++)
                diff_prev[i] = cs[cs.GetUpperBound(0), i];
        }
        public override bool PerformTimeStep()
        {
            //Define time step and get lowest row from the Sediment component grid(lowest row of matrix= actual top row)
            TimeStamp WaterTime = (TimeStamp)this.GetCurrentTime();
            ScalarSet ss = (ScalarSet)this.GetValues("Concentration", "sed");
            //set diff_prev = the imported values 
            for (int i = 0; i <= cs.GetLength(1) - 1; i++)
            {
                diff_prev[i] = ss.data[i];
            }

            //specifies if values should be saved.
            bool SaveValues = true;
            TimeStamp t = (TimeStamp)this.GetCurrentTime();
            DateTime time = CalendarConverter.ModifiedJulian2Gregorian(t.ModifiedJulianDay);

            // calculate concentration in water using advection mechanismin water and diffusion from sediment
            cw = Calculate_concentration();

            //store these c values in adv_prev, for next timestep
            adv_prev = cw;

            //save results for output during Finish()
            double[,] z = new double[rows_W, cols_W];

            if (SaveValues)
            {
                for (int i = 0; i <= cw.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j <= cw.GetLength(1) - 1; j++)
                    {
                        z[i, j] = adv_prev[i, j];
                    }
                }
                outputValues.Add(time, z);

            }

            double[] outvals = new double[cw.GetLength(1)];
            for (int i = 0; i <= cw.GetLength(1) - 1; i++)
            {
                outvals[i] = cw[cw.GetLength(0) - 1, i];
            }

            this.SetValues("Concentration", "water", new ScalarSet(outvals));
            this.AdvanceTime();
            return true;

        }

        public double[,] Calculate_concentration()
        {

            int r = cw.GetLength(0) - 1;
            int m = cw.GetLength(1) - 1;
            //calculate the  row of water component linked with sediment component
            int i = r;
            for (int j = 3; j <= m; j++)
            {
                cw[i, j] = adv_prev[i, j] - (_dt * u / _h) * (adv_prev[i, j] - adv_prev[i, j - 1]) - (D * _dt / Math.Pow(_h, 2.0) * (adv_prev[i, j] - diff_prev[j]));

            }
            //create left and right boundary(cols)
            for (i = 0; i <= r; i++)
            {
                cw[i, 0] = cw[i, 1];

            }
            return cw;
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



