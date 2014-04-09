using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hargreaves;
using NUnit.Framework;
using SMW;
using Oatc.OpenMI.Sdk.Wrapper;
using System.Diagnostics;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace Test
{
    [TestFixture]
    public class TestClass
    {
        Hargreaves.Engine hargreaves;

        [TestFixtureSetUp]
        public void Initialize()
        {
            //---- create instance of the hargreaves model
            hargreaves = new Hargreaves.Engine();

            //---- define input arguments
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../data/config.xml");

            //---- call the initialize method
            hargreaves.Initialize(args);

            Debug.WriteLine("Initialize has completed successfully");

        }

        [Test]
        public void PerformTimeStep()
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'PerformTimeStep' Test");
            Debug.WriteLine("---------------------------------------------------");

            //---- put data into IValueSets
            IValueSet temp = new ScalarSet(new double[1] { 19 });
            IValueSet mintemp = new ScalarSet(new double[1] { 17 });
            IValueSet maxtemp = new ScalarSet(new double[1] { 21 });

            //---- set values
            hargreaves.SetValues("Temp", "Climate Station 01",temp);
            hargreaves.SetValues("Min Temp", "Climate Station 01", mintemp);
            hargreaves.SetValues("Max Temp", "Climate Station 01", maxtemp);

            //---- call perform time step
            hargreaves.PerformTimeStep();

            //---- read calculated results
            double[] pet = ((ScalarSet)hargreaves.GetValues("PET", "Coweeta")).data;

            double chk = Math.Round(pet[0], 2);
            Assert.IsTrue(chk == 1.16, "The calculated value of " + chk.ToString() + " does not equal the known value of 1.16");
        }

        [Test]
        public void CalculatePET()
        {
            Debug.WriteLine("\n\n---------------------------------------------------");
            Debug.WriteLine("Running the 'CalculatePET' Test");
            Debug.WriteLine("---------------------------------------------------");

            double Pet = hargreaves.CalculatePET(19, 17, 21, 0);

            double chk = Math.Round(Pet,2);
            Assert.IsTrue(chk == 1.16, "The calculated value of " + chk.ToString() + " does not equal the known value of 1.16");
        }


    }
}
