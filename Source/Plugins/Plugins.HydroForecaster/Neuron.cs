using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HydroForecaster
{
    class Neuron
    {

        // Data
        // dimension
        int dim;
        // weight set
        float[] weightSet;
        // bias of the neuron
        float bias;
        // determines the activation function
        // 1 - tansig, 2 - logisg, 3 - purelin
        int type;

        // Methods
        // Constructor
        public Neuron()
        {
            this.dim = 0;
            this.type = 0;
            this.bias = 0;
            this.weightSet = null;
        }

        public Neuron(int _dim, int _type)
        {
            this.dim = _dim;
            this.type = _type;
            this.bias = 0;
            this.weightSet = new float[_dim];
        }


        public Neuron(int _dim, int _type, float[] _weight, float _bias)
        {
            this.dim = _dim;
            this.type = _type;
            this.bias = _bias;

            this.weightSet = new float[_dim];

            for (int i = 0; i < this.dim; i++)
            {
                this.weightSet[i] = _weight[i];
            }
        }

        // initialization of neurons
        public void init(int _dim, int _type)
        {
            this.dim = _dim;
            this.type = _type;
            this.bias = 0;
            this.weightSet = new float[_dim];
        }

        public void init(int _dim, int _type, float[] _weight, float _bias)
        {
            this.dim = _dim;
            this.type = _type;
            this.bias = _bias;
            this.weightSet = new float[_dim];

            for (int i = 0; i < this.dim; i++)
            {
                this.weightSet[i] = _weight[i];
            }
        }

        public void init(float[] _weight, float _bias)
        {
            this.bias = _bias;
            this.weightSet = new float[this.dim];

            for (int i = 0; i < this.dim; i++)
            {
                this.weightSet[i] = _weight[i];
            }
        }

        // sets the weight vector
        public void setWeightSet(float[] _weight)
        {
            for (int i = 0; i < this.dim; i++)
            {
                this.weightSet[i] = _weight[i];
            }
        }

        // computes the output of the neuron
        public float evaluate(float[] input)
        {
            float net = this.bias;

            for (int i = 0; i < this.dim; i++)
            {
                net += input[i] * this.weightSet[i];
            }

            if (this.type == 1)
            {
                return tansig(net);
            }
            else if (this.type == 2)
            {
                return logsig(net);
            }
            else
            {
                return purelin(net);
            }
        }

        //// prints out the weight set of the  neuron
        //public void printWeightSet(int neuron)
        //{
        //    StreamWriter sr = null;

        //    sr = new StreamWriter("C:\\pf\\NeuronsWeight" + neuron + ".txt");

        //    StringBuilder builder = new StringBuilder();

        //    builder.Append("Weights: ");

        //    for (int i = 0; i < this.dim; i++)
        //    {
        //        builder.Append(this.weightSet[i] + " ");
        //    }
        //    builder.Append("Bias: " + this.bias);

        //    sr.WriteLine(builder.ToString());

        //    sr.Close();
        //}

        // computes the tansig function
        private static float tansig(float _in)
        {
            return (2.0f / (1.0f + (float)System.Math.Exp((-2.0f * _in))) - 1.0f);
        }

        // computes the logsig function
        private static float logsig(float _in)
        {
            return 1.0f / (1.0f + (float)System.Math.Exp(-_in));
        }

        // computes the purelin function
        private static float purelin(float _in)
        {
            return _in;
        }
    };
}
