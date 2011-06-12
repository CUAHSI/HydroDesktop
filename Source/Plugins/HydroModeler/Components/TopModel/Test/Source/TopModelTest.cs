using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using SMW;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using SharpMap;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using System.Collections;
//using TopModel;
using System.Diagnostics;
using Oatc.OpenMI.Sdk.Wrapper;
using NUnit.Framework;
using System.IO;

namespace TopModelTest.UnitTest
{
    [TestFixture]
    public class Test
    {
        TopModel.TopModel engine = new TopModel.TopModel();
        [Test]
        public void Initialize()
        {
            Console.WriteLine("\n----Begining Intialize----");
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../Data/configTest.xml");
            args.Add("TI", "../../Data/TI_raster.txt");
            args.Add("m","180");
            args.Add("Tmax", "250000");
            args.Add("R", "9.66");
          
            engine.Initialize(args);
            Console.WriteLine("\n ----- Initialize finished Sucessfully -----");

        }
        [Test]
        //In case the PET inputs are from txt file. need to uncomment the Pet reading input in TopModel.cs
        public void PTS_readingfrominputfile()
        {
            Console.WriteLine("\n ----- Beginning Perform-Time-Step -----");
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../data/configTest.xml");
            args.Add("TI", "../../Data/TI_raster.txt");
            args.Add("m", "180");
            args.Add("Tmax", "250000");
            args.Add("Interception", "3");
            args.Add("R", "9.66");

            engine.Initialize(args);

            //passing the daily precip and ET (mm/hr)
            double[] p = new double[5] {10.5,0,0,0,5.613};
            double [] pet= new double[5] {1.355,1.665,1.665,1.815,1.665};


            //Known Runoff results
            double [] Known_pe=new double[5] {12.02,9.35,8.80,8.30,9.84};
            Queue<double> Known_pe_Answers = new Queue<double>(Known_pe);
            Console.WriteLine("Day  \t\t\t Runoff ");

            for(int j = 0; j <= p.Length - 1; j++)
            {
                double[] P = new double[1] { p[j] };
                IValueSet Precip = new ScalarSet(P);
                engine.SetValues("Precip", "TopModel", Precip);

                double[] Pet = new double[1] { pet[j] };
                IValueSet pett = new ScalarSet(Pet);
                engine.SetValues("PET", "TopModel", pett);

                engine.PerformTimeStep();
                double[] pe = ((ScalarSet)engine.GetValues("Runoff", "TopModel")).data;


                Console.WriteLine("{0:D}\t\t\t {1:F}", j, pe[j], Known_pe[j]); 
            }

            Console.WriteLine("\n ----- Perform-Time-Step finished Sucessfully -----");

        }

        public void PTS_inputexchangeitem()
        {
            Console.WriteLine("\n ----- Beginning Perform-Time-Step -----");
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../data/configTest.xml");
            args.Add("TI", "../../Data/TI.csv");
            args.Add("Weather", "../../Data/Weather.csv");

            engine.Initialize(args);

            //passing the daily precip and ET (mm/hr)
            double[] p = new double[5] { 10.5, 0, 0, 0, 5.613 };
            double[] pet = new double[5] { 1.355, 1.665, 1.665, 1.815, 1.665 };
            double R = 9.66;


            //Known Runoff results
            double[] Known_runoff = new double[5] { 12.02, 9.35, 8.80, 8.30, 9.84 };
            Queue<double> Known_runoff_Answers = new Queue<double>(Known_runoff);
            Console.WriteLine("Day  \t\t\t Runoff ");

            for (int j = 0; j <= p.Length - 1; j++)
            {
                double[] P = new double[1] { p[j] };
                IValueSet Precip = new ScalarSet(P);
                engine.SetValues("PPT", "TopModel Testing", Precip);

                double[] Pet = new double[1] { pet[j] };
                IValueSet pett = new ScalarSet(Pet);
                engine.SetValues("PET", "TopModel", pett);

                engine.PerformTimeStep();
                double[] pe = ((ScalarSet)engine.GetValues("Runoff", "TopModel")).data;


                Console.WriteLine("{0:D}\t\t\t {1:F}", j, pe[j], Known_runoff[j]);

                //Check to see if the computed values equal the known ones
                Assert.IsTrue(Math.Round(pe[j], 2) == Known_runoff_Answers.Dequeue());
            }

            Console.WriteLine("\n ----- Perform-Time-Step finished Sucessfully -----");

        }
        [Test]
        public void Finish()
        {
            engine.Finish();
        }

        /// <summary>
        /// Tests the read_topo_input method.
        /// </summary>
        [Test]
        public void read_Topo()
        {
            TopModel.TopModel model = new TopModel.TopModel();
            double[] t;
            double[] f;
            model.read_topo_input("../../../Data/TI_raster.txt", out t, out f);

            List<double> tarray = new List<double>(t);
            Assert.IsTrue(tarray.Contains(-3.9816));
            Assert.IsTrue(tarray.Contains(3.4375));
            Assert.IsTrue(tarray.Contains(4.0013));

            List<double> farray = new List<double>(f);
            Assert.IsTrue(farray.Contains(0.0001684069));
            Assert.IsTrue(farray.Contains(0.0003368137));
        }
        
    }
}
