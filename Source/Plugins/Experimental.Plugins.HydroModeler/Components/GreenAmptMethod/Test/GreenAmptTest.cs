using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace GreenAmpt.UnitTest
{

    [TestFixture]
    public class Test
    {
        GreenAmpt.GreenAmptMethod engine = new GreenAmpt.GreenAmptMethod();

        [Test]
        public void Initialize()
        {
            Console.WriteLine("\n ----- Beginning Initialize -----");

            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "./configTest.xml");
            engine.Initialize(args);

            Console.WriteLine("\n ----- Initialize finished Sucessfully -----");
        }

        [Test]
        public void PTS()
        {

            Console.WriteLine("\n ----- Beginning Perform-Time-Step -----");

            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "./configTest.xml");
            engine.Initialize(args);

            //Precipitation values in mm/hr, on 10 min intervals
            double[] p = new double[6] { 10, 20, 80, 100, 80, 10};


            Console.WriteLine("F \t\t\t dt \t\t i \t\t\t i*dt \t Cumulative_Storage \t Runoff");
            for (int j = 0; j <= p.Length-1; j++)
            {               
                double[] P = new double[1]{ p[j]};
                IValueSet Precip = new ScalarSet(P);
                engine.SetValues("Rainfall", "Test", Precip);
                engine.PerformTimeStep();
                ScalarSet Excess = (ScalarSet)engine.GetValues("Excess Rainfall", "Test");

            }

            Console.WriteLine("\n ----- Perform-Time-Step finished Sucessfully -----");

        }

        [Test]
        public void Finish()
        {
            engine.Finish();
        }

    }
}