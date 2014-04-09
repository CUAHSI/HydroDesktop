using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using edu.SC.Models.Infiltration;
using Oatc.OpenMI.Sdk.Backbone;
using SMW;
using OpenMI.Standard;

namespace CurveNumberTEST
{
    [TestFixture]
    public class SCSAbstractionMethodTest
    {

        [Test]
        public void Initialize()
        {
            SCSAbstractionMethod CN = new SCSAbstractionMethod();
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "./configTest.xml");
            args.Add("OutDir", "../../");
            CN.Initialize(args);

            CN.Finish();

        }

        [Test]
        public void PTS()
        {


            SCSAbstractionMethod CN = new SCSAbstractionMethod();
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("ConfigFile", "./configTest.xml");
            CN.Initialize(args);

            //Precipitation values from Precip Reader Component [based on 30 Min rainfall]
            double[,] p = new double[8, 1] {{0.00},
                                            {0.20},
                                            {0.70},
                                            {0.37},
                                            {1.04},
                                            {2.34},
                                            {0.64},
                                            {0.07}};


            for (int j = 0; j <= p.GetLength(0)-1 ; j++)
            {
                Console.WriteLine("TimeStep " + Convert.ToString(j+1));
                double[] precipArray = new double[p.GetLength(1)];
                for (int k = 0; k <= p.GetLength(1) - 1; k++)
                {
                    //create double array
                    precipArray[k] = p[j,k];
                    
                }
                IValueSet Precip = new ScalarSet(precipArray);
                CN.SetValues("Rainfall", "SmithBranch", Precip);
                CN.PerformTimeStep();
                ScalarSet Excess = (ScalarSet)CN.GetValues("Excess Rainfall", "Smith Branch");

                for (int i = 0; i <= Excess.Count - 1; i++)
                {
                    Console.WriteLine("Watershed: " + i.ToString() + "\t Excess Rainfall: " + Excess.data[i].ToString());
                }

            }

            CN.Finish();
        }
    }
}
