using csvfileReader;
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
    public class WRTest
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

            csvfileReader.WR component = new WR();

            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "C:/research/code/HydroDesktopHG/Source/Plugins/HydroModeler/Components/wtmpReader/Source/config.xml");
            args.Add("DataFolder", "C:/research/code/HydroDesktopHG/Source/Plugins/HydroModeler/Components/wtmpReader/data");
            component.Initialize(args);
        }

        [TestMethod()]
        public void PerformTimeStepTest()
        {
            csvfileReader.WR component = new WR();

            //Initialize Component
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "C:/research/code/HydroDesktopHG/Source/Plugins/HydroModeler/Components/wtmpReader/Source/config.xml");
            args.Add("DataFolder", "C:/research/code/HydroDesktopHG/Source/Plugins/HydroModeler/Components/wtmpReader/data");
            component.Initialize(args);


            //Call perform timestep in the component class
            component.PerformTimeStep();

            //retrieve values from the compenent class
            ScalarSet OutputValues = (ScalarSet)component.GetValues("Temperature", "Temp");
            double[] T = OutputValues.data;

        }
    }
}
