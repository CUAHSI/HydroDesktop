using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMW;
using System.Collections;
using Oatc.OpenMI.Sdk.Backbone;
using System.IO;

////////////////////////////////////////////////
//
//  Original Author:    Jiri Kadlec, Idaho State University
//  Created On:         January 31st 2012
//  Version:            1.0.0
//
//  Component Name:     Sample Component
//  Inputs:             None, shapefile optional
//  Purpose:            Provide a simple template for modelers wishing to implement a linkable components
//                      that has one input and one output. This model can read input from DbReader and write
//                      input to DbWriter
//  Methodology:        Component classifies snow cover into 3 categories: sporadic snow, contiguous snow (> 10 cm) 
//                      and deep snow. This is meant only to show how a modeler 
//                      can carry out a computation using the SMW
//
//  Addt'l Resources:   
//  Modification History: I modified this component by using original code from SampleComponent1.
//
////////////////////////////////////////////////

namespace SampleComponent2
{
    public class SampleClass2:SMW.Wrapper
    {

        #region Global Variables

        //define global variables
        double _timestep;
        string _input_elementset;
        string _input_quantity;
        string _output_elementset;
        string _output_quantity;

        List<double> _station1cache ;
        List<double> _station2cache;

        #endregion

        public SampleClass2()
        {
            _station1cache = new List<double>();
            _station2cache = new List<double>();
        }

        /// <summary>
        /// This method is used to perform actions after model simulation has ended
        /// </summary>
        public override void Finish()
        {
            //Write output data
            StreamWriter sw = new StreamWriter("./SampleOutput2.txt");
            sw.WriteLine("--- Sample Output --- ");
            for (int i = 0; i <= (_station1cache.Count / 2) - 1; i++)
            {
                sw.WriteLine(string.Format("Timestep:{0} {1} {2}", i, Math.Round(_station1cache[i]), Math.Round(_station2cache[i])));
            }

            sw.Close();
        }

        /// <summary>
        /// This method is used to perform actions prior to model simulation
        /// </summary>
        /// <param name="properties"></param>
        public override void Initialize(System.Collections.Hashtable properties)
        {
            //Get config file path defined in sample.omi
            string configFile = (string)properties["ConfigFile"];

            //set OpenMI internal variables
            this.SetVariablesFromConfigFile(configFile);
            
            // initialize a data structure to hold results
            this.SetValuesTableFields();

            //save the time step (defined within the config.xml)
            _timestep = this.GetTimeStep();

            //save input exchange item info 
            InputExchangeItem input = this.GetInputExchangeItem(0);
            _input_elementset = input.ElementSet.ID;
            _input_quantity = input.Quantity.ID;

            //save output exchange item info 
            OutputExchangeItem output = this.GetOutputExchangeItem(0);
            _output_elementset = output.ElementSet.ID;
            _output_quantity = output.Quantity.ID;

            //Note: this can region can be omitted if a shapefile is specified in config.xml
            //#region Manually Define Element Set

            ////Get the input and output element sets from the SMW
            //ElementSet out_elem = (ElementSet)this.Outputs[0].ElementSet;
            //ElementSet in_elem  = (ElementSet)this.Inputs[0].ElementSet;

            ////Set some ElementSet properties
            //out_elem.ElementType    = OpenMI.Standard.ElementType.XYPoint;
            //in_elem.ElementType     = OpenMI.Standard.ElementType.XYPoint;

            ////Create elements e1, e2 and e3
            //Element e1  = new Element();
            //Vertex v1   = new Vertex(1, 1, 0);
            //e1.AddVertex(v1);

            //Element e2  = new Element();
            //Vertex v2   = new Vertex(1, 5, 0);
            //e2.AddVertex(v2);

            //Element e3  = new Element();
            //Vertex v3   = new Vertex(5, 5, 0);
            //e2.AddVertex(v3);

            ////add these elements to the input and output element sets
            //out_elem.AddElement(e1);
            //out_elem.AddElement(e2);
            //out_elem.AddElement(e3);
            //in_elem.AddElement(e1);
            //in_elem.AddElement(e2);
            //in_elem.AddElement(e3);

            

            //#endregion


            ////Do some Calculations prior to simulation (for example - training ??)
            //for (int i = 0; i < in_elem.Elements.Length; i++)
            //    _seeds.Add(_rndNum.Next());
        }

        public override bool PerformTimeStep()
        {
            //Get input values from another component
            //for each shape we read one value
            ScalarSet invals    = (ScalarSet)this.GetValues(_input_quantity, _input_elementset);
            double[] indata     = invals.data;

            //create something to hold output
            double[] outdata    = new double[indata.Length];

            //do some computation... categorize snow
            for (int i = 0; i < indata.Length; i++)
            {
                if (indata[i] <= 0)
                    outdata[i] = 0;
                else if (indata[i] <= 10)
                    outdata[i] = 1;
                else
                    outdata[i] = 2;
            }

            //add vals to cache. This way we can for example calculate with vals of previous vals..
            _station1cache.Add(outdata[0]);
            _station2cache.Add(outdata[1]);

            //set output values for another component
            this.SetValues(_output_quantity, _output_elementset, new ScalarSet(outdata));

            //advance time by one time step
            this.AdvanceTime();

            return true;
        }
    }
}
