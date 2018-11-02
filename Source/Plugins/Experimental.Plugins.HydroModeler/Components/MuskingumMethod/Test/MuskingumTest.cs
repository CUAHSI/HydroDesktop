#define DEBUG
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using edu.SC.Models.Routing;
using SMW;
using Oatc.OpenMI.Sdk.Wrapper;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using System.Diagnostics;

namespace Muskingum.Test
{
    /// <summary>
    /// This class was developed to test the functionality of the Muskingum Routing component, under varying
    /// circumstances that result from various stream networks.
    /// </summary>

    [TestFixture]
    public class Test
    {
        Process p = new Process();

        /// <summary>
        /// This method starts the web service running locally.
        /// </summary>
        [TestFixtureSetUp]
        public void StartWebService()
        {
            Debug.Write("\n Test Fixture Setup... ");

            //Get the current directory
            string currDir = System.IO.Directory.GetCurrentDirectory();

            //move to directory containing the web service
            System.IO.Directory.SetCurrentDirectory("../../../Source");

            //start the python web service by calling this .bat file
            p.StartInfo.FileName = "start_python_service.bat";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            
            //wait 5 seconds for the web service to start up before moving on
            System.Threading.Thread.Sleep(5000);

            //reset the current directory
            System.IO.Directory.SetCurrentDirectory(currDir);
            Debug.WriteLine("done. \n");
        }

        /// <summary>
        /// This method terminates the web service.
        /// </summary>
        [TestFixtureTearDown]
        public void EndWebService()
        {
            Debug.Write("Test Fixture Teardown... ");

            //End the web service process
            Process[] webservice = Process.GetProcessesByName("cmd");
            webservice[0].Kill();

            //End the python process started by the webservice
            Process[] python = Process.GetProcessesByName("python");
            python[0].Kill();

            Debug.WriteLine("done. \n");
        }

        /// <summary>
        /// This method tests that the Initialize method of the muskingum component works properly.
        /// </summary>
        [Test]
        public void Initialize()
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'Initialize' Test");
            Debug.WriteLine("---------------------------------------------------");

            //create instance of the Muskingum class
            edu.SC.Models.Routing.Muskingum Routing = new edu.SC.Models.Routing.Muskingum();

            //define input arguments
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "ConfigTest_2reaches.xml");

            //Call initialize within the muskingum component.  If everything executes successfully, this
            //--> test method should pass here.
            Routing.Initialize(args);

            Debug.WriteLine("Initialize has completed successfully");

            //Teardown the component
            Routing.Finish();
        }

        /// <summary>
        /// This method tests the Perform Time Step function of the Muskingum Routing component.  It is
        /// designed to emulate the computation performed in Example 9.3.2 of the 2005 edition of the book 
        /// "Water Resources Engineering" by Larry Mays. It utilizes an input file containing two reaches.
        /// </summary>
        [Test]
        public void PTS_2ReachNetwork()
        {

            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Testing the Perform Time Step Method for 2 Reaches");
            Debug.WriteLine("---------------------------------------------------");

            //initialize the component
            edu.SC.Models.Routing.Muskingum Routing = new edu.SC.Models.Routing.Muskingum();
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "ConfigTest_2reaches.xml");
            Routing.Initialize(args);


            //Begin Perform Time Step Procedures

            //define the input hydrograph.
            double[,] p = new double[16, 2] {{0.0,0},{800.0,0},{2000.0,0},{4200.0,0},{5200.0,0},
                                             {4400.0,0},{3200.0,0},{2500.0,0},{2000.0,0},{1500.0,0},
                                             {1000.0,0},{700.0,0},{400.0,0},{0.0,0},{0.0,0},{0.0,0}};

            //define known muskingum routed vals, from example 9.3.2
            double[] vals = new double[16] { 0, 272, 1178, 2701, 4455, 4886, 4020, 3009, 2359, 1851, 1350, 918, 610, 276, 16, 1 };
            Queue<double> KnownAnswers = new Queue<double>(vals);

            //loop over all hydrograph values
            for (int j = 0; j <= p.GetLength(0) - 1; j++)
            {
                Console.WriteLine("TimeStep " + Convert.ToString(j + 1));
                double[] Array = new double[p.GetLength(1)];
                for (int k = 0; k <= p.GetLength(1) - 1; k++)
                {
                    //create array to hold the input hydrograph values for this timestep
                    Array[k] = p[j, k];
                }

                //Save input values to the Simple Model Wrappers DataTable using "SetValues"
                IValueSet Precip = new ScalarSet(Array);
                Routing.SetValues("Excess Rainfall", "Smith Branch", Precip);

                //Call Perform Time Step within the Muskingum routing component.
                Routing.PerformTimeStep();

                //Retrieve the calculated outflow from the Simple Model Wrappers DataTable, by calling GetValues()
                ScalarSet Outflow = (ScalarSet)Routing.GetValues("Streamflow", "Smith Branch");

                //Write the output results to the screen
                for (int i = 0; i <= Outflow.Count - 1; i++)
                {
                    Console.WriteLine("Outlet: " + i.ToString() + "\t Outflow [cfs]: " + Outflow.data[i].ToString() + "\n");
                }

                //Check to see if the computed values equal the known ones
                Assert.IsTrue(Math.Round(Outflow.data[0], 0) == KnownAnswers.Dequeue());
            }

            //Teardown the component
            Routing.Finish();
        }


        /// <summary>
        /// This method tests the web services PerformTimeStep method, for an input file containing three
        /// reaches.  The resultant values are checked with thoughs produced by the HEC-HMS model, for the 
        /// same stream network.
        /// </summary>
        [Test]
        public void PTS_3ReachNetwork()
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Testing the Perform Time Step Method for 3 Reaches");
            Debug.WriteLine("---------------------------------------------------");

            //initialize the component
            edu.SC.Models.Routing.Muskingum Routing = new edu.SC.Models.Routing.Muskingum();
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "ConfigTest_3reaches.xml");
            Routing.Initialize(args);


            //Begin Perform Time Step Procedures

            //define the input hydrograph.
            double[,] p = new double[13, 3] {   {0,0,0},
                                                {0,0.235733,0},
                                                {0,0.711618,0},
                                                {0.438576,1.377104,0.174021},
                                                {3.081694,2.348495,4.341684},
                                                {8.084392,3.419044,13.786119},
                                                {13.492319,4.193174,21.125838},
                                                {15.109068,4.572254,20.66142},
                                                {13.640105,4.594348,15.781394},
                                                {10.458048,4.369926,9.388822},
                                                {6.77479,3.962268,5.85001},
                                                {4.549214,3.452857,3.737803},
                                                {3.124193,2.795561,2.306243} };
                        


            //define known muskingum routed vals, from example 9.3.2
            double[] vals = new double[13] { 0.0,
                                            -0.0,
                                            0.0,
                                            0.3,
                                            4.8,
                                            15.3,
                                            25.2,
                                            28.6,
                                            27.3,
                                            23.1,
                                            20.2,
                                            17.2,
                                            14.3};

            Queue<double> KnownAnswers = new Queue<double>(vals);

            //loop over all hydrograph values
            for (int j = 0; j <= p.GetLength(0) - 1; j++)
            {
                Console.WriteLine("TimeStep " + Convert.ToString(j + 1));
                double[] Array = new double[p.GetLength(1)];
                for (int k = 0; k <= p.GetLength(1) - 1; k++)
                {
                    //create array to hold the input hydrograph values for this timestep
                    Array[k] = p[j, k];
                }

                //Save input values to the Simple Model Wrappers DataTable using "SetValues"
                IValueSet Precip = new ScalarSet(Array);
                Routing.SetValues("Excess Rainfall", "Smith Branch", Precip);

                //Call Perform Time Step within the Muskingum routing component.
                Routing.PerformTimeStep();

                //Retrieve the calculated outflow from the Simple Model Wrappers DataTable, by calling GetValues()
                ScalarSet Outflow = (ScalarSet)Routing.GetValues("Streamflow", "Smith Branch");

                //Write the output results to the screen
                for (int i = 0; i <= Outflow.Count - 1; i++)
                {
                    Console.WriteLine("Outlet: " + i.ToString() + "\t Outflow [cfs]: " + Outflow.data[i].ToString() + "\n");
                }

                //Check to see if the computed values equal the known ones
                Assert.IsTrue(Math.Round(Outflow.data[0], 1) == KnownAnswers.Dequeue());
            }

            Routing.Finish();
        }
    }
}
