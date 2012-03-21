using N_SolarRadiation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using SMW;
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard;

namespace TestProject
{
    
    [TestClass()]
    public class N_SRTest
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
            N_SolarRadiation.N_SR component = new N_SR();

            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../source/config.xml");
            args.Add("wtmpFolder", "../../../data/wtmp");
            component.Initialize(args);
        }

        [TestMethod()]
        public void PerformTimeStepTest()
        {
            N_SolarRadiation.N_SR component = new N_SR();

            //Initialize Component
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../source/config.xml");
            args.Add("wtmpFolder", "../../../data/wtmp");
            component.Initialize(args);

            //Call perform timestep in the component class
            component.PerformTimeStep();

            //retrieve values from the compenent class
            ScalarSet _NSR = (ScalarSet)component.GetValues("NSR", "NetRad");
            double[] NSR = _NSR.data;
        }
    }
}
