using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PT_Evapotranspiration;
using SMW;
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard;

namespace TestClass
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void TestInitialize()
        {
            PT_Evapotranspiration.PT_PET component = new PT_PET();

            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../Source/PT_PETconfig.xml");
            args.Add("DataFolder", "../../../data");
            args.Add("OutDir", "../../../");
            component.Initialize(args);
  
        }

        [Test]
        public void TestPerformTimeStep()
        {
            PT_Evapotranspiration.PT_PET component = new PT_PET();

            //Initialize Component
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "../../../Source/PT_PETconfig.xml");
            args.Add("DataFolder", "../../../data");
            args.Add("OutDir", "../../../");
            component.Initialize(args);

            //Define Inputs
            double[] NetRadiation = { 4, 17.28, 6, 7 };
            component.SetValues("NSR", "NetRadiation", new ScalarSet(NetRadiation));
            double[] temp = { 4.3, 25, 6.5, 7.6 };
            component.SetValues("Temp", "T", new ScalarSet(temp));
            double[] elevation = { 14, 0, 16, 17 };
            component.SetValues("Elevation", "Z", new ScalarSet(elevation));

          
            //Call perform timestep in the component class
            component.PerformTimeStep();

            //retrieve values from the compenent class
            ScalarSet OutputValues = (ScalarSet)component.GetValues("PET", "pet");
            ScalarSet OutputValues2 = (ScalarSet)component.GetValues("StandardizedET", "ETsz");
            double[] PET = OutputValues.data;
            double[] ETsz = OutputValues2.data;

            //Call finish
            component.Finish();
            
        }
    }
}
