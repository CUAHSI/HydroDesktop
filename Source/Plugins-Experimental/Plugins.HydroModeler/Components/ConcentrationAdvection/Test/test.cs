using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
//using AdveicationDiffuision;
using System.Collections;
using SMW;

namespace Test
{
    [TestFixture]
    public class testclass
    {
        [Test]
        public void testInitialize()
        {
            Water_adv.Water_adv model = new Water_adv.Water_adv();

            //create new hashtable
            Hashtable arguments = new Hashtable();
            arguments.Add("ConfigFile", "../../config.xml");
            arguments.Add("Concentration", "../../../Water_adv/inputs.csv");
            arguments.Add("Ux", "../../../Water_adv/inputs.csv");
            arguments.Add("Uy", "../../../Water_adv/inputs.csv");
            arguments.Add("L", "../../../Water_adv/inputs.csv");
            arguments.Add("d", "../../../Water_adv/inputs.csv");
            model.Initialize(arguments);

        }
        [Test]
        public void PTS_Changing_C_only()
        {
            Water_adv.Water_adv model = new Water_adv.Water_adv();

            //create new hashtable
            Hashtable arguments = new Hashtable();
            arguments.Add("ConfigFile", "../../../Water_adv/configuration.xml");
            arguments.Add("Concentration", "../../../Water_adv/inputs.csv");
            arguments.Add("Ux", "../../../Water_adv/inputs.csv");
            arguments.Add("Uy", "../../../Water_adv/inputs.csv");
            arguments.Add("L", "../../../Water_adv/inputs.csv");
            arguments.Add("d", "../../../Water_adv/inputs.csv");
            model.Initialize(arguments);




            
            {
                //set values
                // model.SetValues("C", "Test C", new double[1] { .5 });

                //run ts
                model.PerformTimeStep();

            }

            model.Finish();

        }
    }
}
