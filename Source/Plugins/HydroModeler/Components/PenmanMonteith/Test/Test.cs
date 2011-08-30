using Evptrnsprtion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System;

using Oatc.OpenMI.Sdk.Backbone;


namespace TestProject
{
    
    [TestClass()]
    public class eTTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod()]
        public void InitializeTest()
        {
            Evptrnsprtion.eT component = new eT();

            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../Source/config.xml");
            component.Initialize(args);

        }
        [TestMethod()]
        public void PerformTimeStepTest()
        {
            Evptrnsprtion.eT component = new eT();

            //Initialize Component
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../Source/config.xml");
            component.Initialize(args);

            //Define Inputs
            double[] NetRadiation = { 4, 5, 6, 7 };
            component.SetValues("NSR", "NetRadiation", new ScalarSet(NetRadiation));



            //Call perform timestep in the component class
            component.PerformTimeStep();

            //retrieve values from the compenent class
            ScalarSet OutputValues = (ScalarSet)component.GetValues("PET", "pet");
            ScalarSet OutputValues3 = (ScalarSet)component.GetValues("StandardizedET", "ETsz");
            double[] PET = OutputValues.data;
            double[] ETsz = OutputValues3.data;
            
        }

    }
}
