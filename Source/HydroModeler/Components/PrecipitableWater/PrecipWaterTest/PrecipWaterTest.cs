using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using NUnit.Framework;
using SMW;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard;
using PrecipitableWater;

namespace PrecipWaterTest
{
    #region Test1
    //[TestFixture]
    //static class PrecipWaterTest
    //{
    //    /// <summary>
    //    /// The main entry point for the application.
    //    /// </summary>

    //    static PrecipitableWater.PrecipMethod engine = new PrecipitableWater.PrecipMethod();

    //    [Test]
    //    static void Main() 
    //    {
    //        //Initialize
    //        Console.WriteLine("\n ----- Beginning Initialize -----");
    //        System.Collections.Hashtable args = new System.Collections.Hashtable();
    //        args.Add("ConfigFile", "./configTest.xml");
    //        engine.Initialize(args);

    //        Console.WriteLine("\n ----- Initialize finished Sucessfully -----");


    //        Console.WriteLine("\n ----- Beginning Perform-Time-Step -----");


    //        //temperature values, celcius, on 60 min intervals
    //        double[] t = new double[6] { 8.17, 23.5, 24, 26, 27, 25 };

    //        //Use the engine
    //        Console.WriteLine("Precipitable Water for Different Surface Temperature");
    //        for (int j = 0; j <= t.Length - 1; j++)
    //        {
    //            double[] T = new double[1] { t[j] };
    //            IValueSet temp = new ScalarSet(T);
    //            engine.SetValues("T", "Temperature", temp);
    //            engine.PerformTimeStep();
    //            ScalarSet Excess = (ScalarSet)engine.GetValues("P", "Precipitable Water");

    //        }

    //        Console.WriteLine("\n ----- Perform-Time-Step finished Sucessfully -----");

    //        Console.ReadLine();

    //        //Finish
    //        engine.Finish();
    //    }
    //}

    #endregion

    [TestFixture]
    public class Test
    {
        PrecipitableWater.PrecipMethod engine = new PrecipitableWater.PrecipMethod();

        [Test]
        public void Initialize()
        {
            Console.WriteLine("\n ----- Beginning Initialize -----");
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../Source/config.xml");
            engine.Initialize(args);

            Console.WriteLine("\n ----- initialize finished sucessfully -----");
        }

        [Test]
        public void Process()
        {
            Console.WriteLine("\n ----- Beginning Perform-Time-Step -----");

            //temperature values, celcius, on 60 min intervals
            double[] t = new double[6] { 8.17, 33.0, 32.6, 32.2, 27.5, 29.8 };

            //Use the engine
            Console.WriteLine("Precipitable Water for Different Surface Temperature");

            for (int j = 0; j < t.Count(); j++)
            {
                double[] T = new double[1] { t[j] };
                IValueSet temp = new ScalarSet(T);
                engine.SetValues("T", "Temperature", temp);
                engine.PerformTimeStep();
                ScalarSet Excess = (ScalarSet)engine.GetValues("P", "Precipitable Water");
            }           
            
            Console.WriteLine("\n ----- Perform-Time-Step finished Sucessfully -----");
        }

        [Test]
        public void finish()
        {
            Console.ReadLine();

            engine.Finish();
        }
    }
}
