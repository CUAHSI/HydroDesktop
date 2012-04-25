using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using System.Xml;
using System.IO;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;


namespace UnitTest
{
    [TestFixture]
    public class Test
    {


        string omiPath = null;
        string config = null;
        DiffusiveWave.Source.Wrapper wrapper;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Debug.Write("\n Test Fixture Setup... ");

            omiPath = System.IO.Directory.GetCurrentDirectory() + "../../../Data/2dDiffusiveWave.omi";
            //config = System.IO.Directory.GetCurrentDirectory() + "../../../Data/2dDiffusiveWaveConfig.xml";

            omiPath = Path.GetFullPath(omiPath);
            //config = Path.GetFullPath(config);

            wrapper = new DiffusiveWave.Source.Wrapper();

            Debug.WriteLine("done. \n");

        }



        [Test]
        public void Initialize()
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'Initialize' Test");
            Debug.WriteLine("---------------------------------------------------");

            //create input argument hashtable
            System.Collections.Hashtable args = BuildArgs(omiPath);

            //Initialize Model
            wrapper.Initialize(args);

            Debug.WriteLine("Initialize has completed successfully");

            //Teardown the component
            wrapper.Finish();

            

        }

        /// <summary>
        /// Tests how the model hanels stage input.  This is what can be expected from river overflow
        /// </summary>
        [Test]
        [TestCase("X and Y Slope", 7,  7,   0.1,  0.1,   .1,   .1,  1,  0.007, 1, new double[7] { .03, .03, .03, .03, .03, .03, .03 })]
        //            ID         ROWS COLS  SOX  SOY    NX    NY  CELL  WEIR   DT-hrs             INFLOW
        public void StageInput(string id,  int r,  int c, double sx, double sy, double nx, double ny, double cell, double hw, double dt, double[] InStage)
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'Stage Input' Test, ID: " + id);
            Debug.WriteLine("---------------------------------------------------");

            //create input argument hashtable
            System.Collections.Hashtable args = BuildArgs(omiPath);

           //set the number of rows and columns (for this test)
            
            wrapper.nx = nx;
            wrapper.ny = ny;
            wrapper.hw = hw;
            double[] stage = InStage;
            //Initialize Model
            wrapper.Initialize(args);

            //set stage values
            IValueSet Stage = new ScalarSet(stage);
            wrapper.SetValues("Stage", "SmithBranch", Stage);

            //run timestep
            wrapper.PerformTimeStep();

            ScalarSet excess = (ScalarSet)wrapper.GetValues("Excess Rainfall", "Smith Branch");

            //get number of rows and cols, assuming a squar matrix
            int elem = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(excess.Count)));

            StreamWriter sw = new StreamWriter(id + ".csv", false);

            //Write the output results to the screen
            Debug.WriteLine("Excess Rainfall Results");
            Queue<double> s = new Queue<double>(stage);
            double sum = 0;
            double riverVolume = 0;
            for (int i = 0; i <= excess.Count - 1; i++)
            {

                if (i % (stage.Length) == 0)
                {
                    double ss = s.Dequeue();
                    if (ss != 0)
                        riverVolume += (ss - wrapper.hw);
                    Debug.Write("\n" + excess.data[i].ToString() + "\t\t");
                    sw.Write("\n" + excess.data[i].ToString() + ",");
                }
                else
                {
                    Debug.Write(excess.data[i].ToString() + "\t\t");
                    sw.Write(excess.data[i].ToString() + ",");
                }

                sum += excess.data[i];
                
            }
            
            Debug.Write("\nTotal head on floodplain = " + sum.ToString() +". \n");
            Debug.WriteLine("\nStage Input Test has completed successfully");
            sw.Close();
            //Teardown the component
            wrapper.Finish();

            
        }

        /// <summary>
        /// Tests how the model handels just excess rainfall input.  This is what is expected for overland flow
        /// </summary>
        //[TestCase("Excess_Complex", "../../Data/elev5meter_test.txt", 0.05, 0.05, 0.01, new double[15]{.01,0,0,0,0,0,0,0,0,0,0,0,0,0,0})]
        [TestCase("Excess_Complex", "../../Data/elev5meter.txt", 0.05, 0.05, 0.01, new double[15] { 0.01, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        public void ExcessInput(string id,  string elevTxt, double nx, double ny, double hw, double[] InExcess)
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'Excess Input' Test, ID: " + id);
            Debug.WriteLine("---------------------------------------------------");


            //get xllcorner and yll corner for output file
            StreamReader sr = new StreamReader(elevTxt);
            string line = sr.ReadLine(); line = sr.ReadLine(); line = sr.ReadLine();
            string xllcorner = line; line = sr.ReadLine();
            string yllcorner = line; line = sr.ReadLine(); line = sr.ReadLine();
            string NoData = line;
            sr.Close();

            //create input argument hashtable
            System.Collections.Hashtable args = BuildArgs(omiPath);

            //set elevation file in omi
            args["SurfaceElevation"] = elevTxt;
            wrapper = new DiffusiveWave.Source.Wrapper();
            wrapper.nx = nx;
            wrapper.ny = ny;
            wrapper.hw = hw;
            //Initialize Model
            wrapper.Initialize(args);

            int num_rows = wrapper._sox.GetLength(0);
            int num_cols = wrapper._sox.GetLength(1);
            double inexcess = 0.001;
            StreamWriter results_output = new StreamWriter("flood_results.csv",false);
            results_output.WriteLine(wrapper._elevation.GetLength(0).ToString() + "," + wrapper._elevation.GetLength(1).ToString());
            for (int time = 1; time <= 3642; time++)
            {
                //inexcess = 0.01 / 3652;
                //double supplied_excess = InExcess[time-1] * num_cols * num_rows;

                //create stage array with values of zero
                double[] stage = new double[wrapper._sox.GetLength(0) * wrapper._sox.GetLength(1)];

                //create excess array
                double[] excess = new double[wrapper._sox.GetLength(0) * wrapper._sox.GetLength(1)];
                for (int i = 0; i <= excess.Length - 1; i++)
                    excess[i] = inexcess;
                    //excess[i] = InExcess[time-1];

                //set excess and stage values
                IValueSet Excess = new ScalarSet(excess);
                wrapper.SetValues("Excess Rainfall", "SmithBranch", Excess);
                IValueSet Stage = new ScalarSet(stage);
                wrapper.SetValues("Stage", "SmithBranch", Stage);

                //run timestep
                wrapper.PerformTimeStep();

                //get result values
                ScalarSet result = (ScalarSet)wrapper.GetValues("Excess Rainfall", "Smith Branch");


                //create output file and write some header data
                //StreamWriter sw = new StreamWriter(id + "_" + time * wrapper.GetTimeStep() + ".txt", false);
                //sw.WriteLine("ncols         " + wrapper._sox.GetLength(1));
                //sw.WriteLine("nrows         " + wrapper._sox.GetLength(0));
                //sw.WriteLine(xllcorner);
                //sw.WriteLine(yllcorner);
                //sw.WriteLine("cellsize      " + wrapper._cellsize);
                //sw.WriteLine(NoData);


                //get number of rows and cols, assuming a squar matrix
                int elem = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(result.Count)));

                //Write the output results to the screen
                Debug.WriteLine("Excess Rainfall Results: current time= " + (time*wrapper._dt).ToString() + "seconds");
                double calculated_excess = 0;

                results_output.WriteLine((time * wrapper._dt).ToString());

                for (int row = 0; row <= num_rows - 1; row++)
                {
                    for (int col = 0; col <= num_cols - 1; col++)
                    {
                        calculated_excess += result.data[row * num_cols + col];

                        string value = null;
                        if (result.data[row * num_cols + col] == 0)
                            value = "-9999";
                        else
                            value = result.data[row * num_cols + col].ToString("F7");


                        //Debug.Write(value + " ");
                        //sw.Write(value + " ");
                        results_output.Write(value + ",");
                    }

                    //Debug.Write("\n");
                    results_output.Write("\n");
                    //if (row != num_rows - 1)
                    //    sw.Write("\n");

                }
                //double initialExcess = InExcess[time-1] * result.Count;
                //Debug.Write("Total excess on floodplain = " + calculated_excess.ToString() + ". Initial excess = " + supplied_excess.ToString());
                //Debug.WriteLine("\n Stage Input Test has completed successfully");
                //Debug.WriteLine("----------");
                //sw.Close();
                inexcess = 0;

            }

            results_output.Close();

            //Teardown the component
            wrapper.Finish();

        }
        
        [TestCase("Discretization Test", "../../Data/elev5meter_test.txt", 0.05, 0.05, 0.01, new double[5] { .01, 0, 0, 0, 0 })]
        public void Discretization(string id,  string elevTxt, double nx, double ny, double hw, double[] InExcess)
        {
            StreamReader sr = new StreamReader(elevTxt);
            string line = sr.ReadLine();

            int cols = Convert.ToInt32(line.Split(' ')[line.Split(' ').Length - 1]); line = sr.ReadLine();
            int rows = Convert.ToInt32(line.Split(' ')[line.Split(' ').Length - 1]); line = sr.ReadLine(); line = sr.ReadLine(); line = sr.ReadLine();
            int cellsize = Convert.ToInt32(line.Split(' ')[line.Split(' ').Length - 1]); line = sr.ReadLine(); line = sr.ReadLine();
            double[,] elevations = new double[rows, cols];
            int i = 0;
            while (!String.IsNullOrEmpty(line))
            {
                string[] elev = line.Split(' ');
                for (int j = 0; j <= elev.Length - 2; j++)
                    if(!String.IsNullOrEmpty(elev[j]))
                        elevations[i, j] = Convert.ToDouble(elev[j]);

                line = sr.ReadLine();
                i++;
            }
            
            sr.Close();

            DiffusiveWave.Source.Discretization euler = new DiffusiveWave.Source.Euler(cellsize, 0.083, rows * cols, elevations);
            //create a new instance of the euler class
            //DiffusiveWave.Source.Euler euler = new DiffusiveWave.Source.Euler(cellsize, 0.083, rows * cols, elevations);
            
            //create excess
            double[] excess = new double[rows*cols];
            for(int e=0; e<=rows*cols-1; e++)
            {
                excess[e] = 0.01;
            }
            euler.Sox = new double[rows, cols];
            euler.Soy = new double[rows, cols];
            euler.Head = new double[rows* cols];
            //run CreateStiffness
            euler.CreateStiffness(new double[rows], excess, new double[rows * cols], new double[rows * cols], 0);
            
            StreamWriter sw = new StreamWriter("./Stiffness.txt");
            double[,] A = euler.A;
            for (int r = 0; r <= A.GetLength(0) - 1; r++)
            {
                for (int c = 0; c <= A.GetLength(1) - 1; c++)
                {
                    sw.Write(A[r, c].ToString("F7") + " ");
                }
                sw.Write("\n");
            }
            sw.Close();

            sw = new StreamWriter("./Source.txt");
            double[] q = euler.q;
            for (int r = 0; r <= q.GetLength(0) - 1; r++)
            {
                sw.Write(q[r].ToString("F7") + " ");
                if((r+1) %(cols) == 0)
                    sw.Write("\n");
            }
            sw.Close();

        }

        [Test]
        public void TestSuccessiveOverRelaxation()
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'SOR' Test");
            Debug.WriteLine("---------------------------------------------------");

            //DiffusiveWave.Source.Engine engine = new DiffusiveWave.Source.Engine(0.02);
            DiffusiveWave.Source.Engine2 engine = new DiffusiveWave.Source.Engine2(0.02, 0.083);
            //create A matrix
            double[,] A = new double[4,4]{
                                        {10,-1,2,0},
                                        {-1,11,-1,3},
                                        {2,-1,10,-1},
                                        {0,3,-1,8}
                                        };

            //create B matrix
            double[] b = new double[4] { 6,25,-11,15 };

            //define known results
            double[] x_known = new double[4]{1,2,-1,1};

            engine.A = A;
            engine.q = b;

            //call SOR
            double[] x_calc = engine.SuccessiveOverRelaxation();


            for (int i = 0; i <= x_known.GetLength(0)-1; i++)
            {
                Assert.IsTrue(Math.Round(x_calc[i], 3) == x_known[i],"Known value = "+x_known[i].ToString()+
                                        ", calculated value = " + Math.Round(x_calc[i], 3).ToString());
            }
            Debug.WriteLine("SOR Input Test has completed successfully");
        }

        [Test]
        public void Finish()
        {

           
        }

        [Test]
        public void BuildElementSet()
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'BuildElementSet' Test");
            Debug.WriteLine("---------------------------------------------------");

            
            string elevationTXT = "../../Data/elevation.txt";
            DiffusiveWave.Source.Engine engine = new DiffusiveWave.Source.Engine(0.02);
            ElementSet e;
            double[,] sox, soy;
            
            engine.BuildElementSet(elevationTXT, "none", "test element set", "test", out e, out sox, out soy);
            double ElementCount = sox.GetLength(0) * sox.GetLength(1);
            Assert.IsTrue(e.ElementCount == ElementCount, "Element count equals " + e.ElementCount + ", not "+ElementCount.ToString());
            Assert.IsTrue(e.ElementType == ElementType.XYPoint, "Element type is " + e.ElementType + ", not XYPoint");


            Debug.WriteLine("BuildElementSet has completed successfully");

            //Teardown the component
            wrapper.Finish();


        }


        [TestCase("complexTerrain_compact", "../../Data/elevation_compact.txt", 0.05, 0.05, 0.007, .011)]
        public void ComplexTerrain(string id, string elevTXT, double nx, double ny, double hw, double InStage)
        {
            //create input argument hashtable
            System.Collections.Hashtable args = BuildArgs(omiPath);
            //set elevation file in omi
            args["SurfaceElevation"] = elevTXT;

            //get xllcorner and yll corner for output file
            StreamReader sr = new StreamReader(elevTXT);
            string line = sr.ReadLine(); line = sr.ReadLine(); line = sr.ReadLine();
            string xllcorner = line; line = sr.ReadLine();
            string yllcorner = line;line = sr.ReadLine();line = sr.ReadLine();
            string NoData = line;
            sr.Close();

            wrapper.nx = nx;
            wrapper.ny = ny;
            wrapper.hw = hw;
            //Initialize Model
            wrapper.Initialize(args);

            //create stage input array
            double[] stage = new double[wrapper._sox.GetLength(0) * wrapper._sox.GetLength(1)];
            for (int i = 0; i <= wrapper._sox.GetLength(0)-1; i++)
                stage[i * wrapper._sox.GetLength(0)] = InStage;

            for (int time = 1; time <= 2; time++)
            {

                //set stage values
                IValueSet Stage = new ScalarSet(stage);
                wrapper.SetValues("Stage", "SmithBranch", Stage);

                //run timestep
                wrapper.PerformTimeStep();

                ScalarSet excess = (ScalarSet)wrapper.GetValues("Excess Rainfall", "Smith Branch");

                //get number of rows and cols, assuming a squar matrix
                int elem = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(excess.Count)));

                //create output file and write some header data
                StreamWriter sw = new StreamWriter(id + "_" + time*wrapper.GetTimeStep() + ".txt", false);
                sw.WriteLine("ncols         " + wrapper._sox.GetLength(1));
                sw.WriteLine("nrows         " + wrapper._sox.GetLength(0));
                sw.WriteLine(xllcorner);
                sw.WriteLine(yllcorner);
                sw.WriteLine("cellsize      " + wrapper._cellsize);
                sw.Write(NoData);

                //Write the output results to the screen
                Debug.WriteLine("Excess Rainfall Results");
                Queue<double> s = new Queue<double>(stage);
                double sum = 0;
                int c = 1 ;
                for (int i = 0; i <= excess.Count - 1; i++)
                {
                    string value = excess.data[i].ToString("F7");

                    if (excess.data[i] == 0)
                        value = "-9999";

                    Debug.Write(value + "\t\t");
                    sw.Write(value + " ");

                    if (c == wrapper._sox.GetLength(1))
                    {
                        //write a new line
                        Debug.Write("\n");
                        sw.Write("\n");
                        //reset c
                        c = 1;
                    }
                    else
                    {
                        c++;
                    }

                    sum += excess.data[i];

                }

                Debug.Write("\nTotal head on floodplain = " + sum.ToString() + ". \n");
                Debug.WriteLine("\nStage Input Test has completed successfully");
                sw.Close();

            }
            //Teardown the component
            wrapper = null;

        }



        //[TestCase("complexTerrain_compact", "../../Data/elevation_compact.txt", 0.05, 0.05, 0.007, .011)]
        [TestCase("complexTerrain_medium", "../../Data/elevation_med.txt", 0.05, 0.05, 0.03, .035)]
        public void ComplexTerrain_RefactoredCode(string id, string elevTXT, double nx, double ny, double hw, double InStage)
        {
            //create input argument hashtable
            System.Collections.Hashtable args = BuildArgs(omiPath);
            //set elevation file in omi
            args["SurfaceElevation"] = elevTXT;

            //get xllcorner and yll corner for output file
            StreamReader sr = new StreamReader(elevTXT);
            string line = sr.ReadLine(); line = sr.ReadLine(); line = sr.ReadLine();
            string xllcorner = line; line = sr.ReadLine();
            string yllcorner = line; line = sr.ReadLine(); line = sr.ReadLine();
            string NoData = line;
            sr.Close();
            DiffusiveWave.Source.Wrapper2 wrapper = new DiffusiveWave.Source.Wrapper2(); 
            wrapper.nx = nx;
            wrapper.ny = ny;
            wrapper.hw = hw;
            //Initialize Model
            wrapper.Initialize(args);
            
            //create stage input array
            double[] stage = new double[wrapper.rows * wrapper.cols];
            for (int i = 0; i <= wrapper.rows - 1; i++)
                stage[i * wrapper.cols] = InStage;
            int print_num = 10;
            int print_count = 1;
            bool print = true;
            for (int time = 1; time <= 300; time++)
            {

                //set stage values
                IValueSet Stage = new ScalarSet(stage);
                wrapper.SetValues("Stage", "SmithBranch", Stage);

                //run timestep
                wrapper.PerformTimeStep();

                ScalarSet excess = (ScalarSet)wrapper.GetValues("Excess Rainfall", "Smith Branch");

                //get number of rows and cols, assuming a square matrix
                int elem = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(excess.Count)));
                if (print_count == print_num)
                    print = true;

                if (print)
                {
                    //create output file and write some header data
                    StreamWriter sw = new StreamWriter("../../Data/model_output/" + id + "_" + (time * wrapper.GetTimeStep()).ToString("F2") + ".txt", false);
                    sw.WriteLine("ncols         " + wrapper.cols);
                    sw.WriteLine("nrows         " + wrapper.rows);
                    sw.WriteLine(xllcorner);
                    sw.WriteLine(yllcorner);
                    sw.WriteLine("cellsize      " + wrapper._cellsize);
                    sw.WriteLine(NoData);

                    //Write the output results to the screen
                    //Debug.WriteLine("Excess Rainfall Results");
                    Queue<double> s = new Queue<double>(stage);
                    double sum = 0;
                    //int k=0;
                    int c = 1;
                    for (int i = 0; i <= excess.Count - 1; i++)
                    {
                        //string value = (excess.data[i] + wrapper._elevation[k,i - k*wrapper._elevation.GetLength(1)]).ToString("F7");
                        string value = excess.data[i].ToString("F7");

                        if (excess.data[i] == 0)
                            value = "-9999";

                        //Debug.Write(value + "\t\t");
                        sw.Write(value + " ");

                        if (c == wrapper.cols)
                        {
                            //write a new line
                            //Debug.Write("\n");
                            sw.Write("\n");
                            //reset c
                            c = 1;
                        }
                        else
                        {
                            c++;
                        }

                        sum += excess.data[i];

                    }                
                    sw.Close();
                    if(time != 1)
                        print_count = 1;
                    print = false;
                }
                Debug.WriteLine("Time Step " + (time).ToString() + " completed...");
                print_count++;
            }
            //Teardown the component
            wrapper = null;
            Debug.WriteLine("\n--------------------------------------------");
            Debug.WriteLine("Stage Input Test has completed successfully");
        }

        //TODO:
        //Test that inputs can be applied to specific elements in the calculation grid!

        private System.Collections.Hashtable BuildArgs(string omi)
        {
            System.Collections.Hashtable args = new System.Collections.Hashtable();

            //read arguments from omi
            XmlDocument doc = new XmlDocument();
            doc.Load(omi);

            //get root
            XmlElement root = doc.DocumentElement;

            //get arguments
            foreach (XmlNode arg in root.ChildNodes[0].ChildNodes)
            {

                //get argument name
                string key = null;
                string[] element = arg.OuterXml.Split(' ');
                foreach (string e in element)
                {

                    if (e.Split('=')[0] == "Key")
                    {
                        key = (e.Split('=')[1].Remove(0, 1)).Remove(e.Split('=')[1].Length - 2, 1);
                        //string value
                    }
                    else if (e.Split('=')[0] == "Value")
                    {
                        string value = "../../Data" + (e.Split('=')[1].Remove(0, 1)).Remove(e.Split('=')[1].Length - 2, 1);
                        args.Add(key, value);
                        break;
                    }


                }
            }
            return args;
        }

    }
}
