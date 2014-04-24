using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HydroForecaster
{
    class NeuralLayer
    {

        // Data
        // type of neurons in the layer
        int type;

        // number of neurons in this layer
        int neuronNumber;

        // dimensionality of the input vector
        int dim;

        // array of neurons in this layer
        public Neuron[] neurons;

        // Methods
        // Constructor
        public NeuralLayer()
        {
            this.dim = 0;
            this.type = 0;
            this.neuronNumber = 0;

            this.neurons = null;
        }

        public NeuralLayer(int _dim, int _type, int _neuronNumber)
        {
            this.dim = _dim;
            this.type = _type;
            this.neuronNumber = _neuronNumber;

            this.neurons = new Neuron[this.neuronNumber];
        }

        // Initialization of the network layer
        public void init()
        {
            for (int i = 0; i < this.neuronNumber; i++)
            {
                this.neurons[i] = new Neuron();
                this.neurons[i].init(this.dim, this.type);
            }
        }

        public void init(int _dim, int _type, int _neuronNumber)
        {
            this.dim = _dim;
            this.type = _type;
            this.neuronNumber = _neuronNumber;

            this.neurons = new Neuron[this.neuronNumber];

            for (int i = 0; i < this.neuronNumber; i++)
            {
                this.neurons[i].init(this.dim, this.type);
            }
        }

        // Computes the output of this neural layer
        public float[] evaluate(float[] input)
        {
            float[] output = new float[this.neuronNumber];

            for (int i = 0; i < this.neuronNumber; i++)
            {
                output[i] = this.neurons[i].evaluate(input);
            }

            return output;
        }

        //// Prints out the weight sets of nerons in the layer 
        //public void print(int layer)
        //{

        //    StreamWriter sr = null;

        //    sr = new StreamWriter("C:\\pf\\Neuron" + layer + ".txt");

        //    StringBuilder builder = new StringBuilder();


        //    for (int i = 0; i < this.neuronNumber; i++)
        //    {
        //        builder.Append(i + ": ");
        //        this.neurons[i].printWeightSet(i);
        //    }
        //    sr.WriteLine(builder.ToString());

        //    sr.Close();
        //}

    };
}
