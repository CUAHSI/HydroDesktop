using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using edu.sc.Models.Routing;
using System.Collections;
using SMW;
using Oatc.OpenMI.Sdk.Wrapper;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace TEST.Routing.NRCS_UnitHydrograph
{
    [TestFixture]
    public class SCSHydrographTest
    {

        /// <summary>
        /// This method test the initialize method of the component
        /// </summary>
        [Test]
        public void Initialize()
        {
            SCSUnitHydrograph NRCS = new SCSUnitHydrograph(); 
            Hashtable arguments = new Hashtable();
            arguments.Add("ConfigFile", "./configTest.xml");
            NRCS.Initialize(arguments);

        }

        /// <summary>
        /// This method tests the PerformTimeStep method of the component
        /// </summary>
        [Test]
        public void PTS()
        {
            //intialize the component
            SCSUnitHydrograph NRCS = new SCSUnitHydrograph();
            Hashtable arguments = new Hashtable();
            arguments.Add("ConfigFile", "./configTest.xml");
            NRCS.Initialize(arguments);

            //input incremental excess precipitation values
            double[,] pe = new double[20,1]
            {
                {0.0},{0.0},{0.0},{0.056015},{0.11552},
                {0.0},{0.0},{0.0},{0.0},{0.0},
                {0.0},{0.0},{0.0},{0.057091},{0.0},
                {0.0},{0.0},{0.0},{0.0},{0.0}
            };

            for (int j = 0; j <= pe.GetLength(0) -1 ; j++)
            {
                Console.WriteLine("TimeStep " + Convert.ToString(j + 1));
                double[] peArray = new double[pe.GetLength(1)];
                for (int k = 0; k < 1 ; k++)
                {
                    //create double array for the first element
                    peArray[k] = pe[j, k];

                }
                IValueSet Pe = new ScalarSet(peArray);
                //set the values defined above into the datatable
                NRCS.SetValues("Excess Rainfall", "Smith Branch", Pe);

                NRCS.PerformTimeStep();

                ScalarSet NRCS_Excess = (ScalarSet)NRCS.GetValues("Runoff", "Smith Branch");

                for (int i = 0; i <= NRCS_Excess.Count - 1; i++)
                {
                    Console.WriteLine("Watershed: " + i.ToString() + "\t Runoff: " + NRCS_Excess.data[i].ToString());
                }

            }
            NRCS.Finish();
        
        }

    }
}
