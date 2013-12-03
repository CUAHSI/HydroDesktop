using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HydroForecaster
{
    class NeuralNetwork
    {
        // Data
        // number of layers
     public   int numLayers;

        // number of neurons in particular layers
       public  int[] layersNeurons;

        // types of neurons in particular layers
      public   int[] layersTypes;

        // Neural network layers
      public   NeuralLayer[] layers;

        // dimensionality of the input vector
        int dimInput;

        // Methods
        // Constructor
        public NeuralNetwork()
        {
            this.numLayers = 0;
            this.layersNeurons = null;
            this.layersTypes = null;
            this.dimInput = 0;
            this.layers = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_numLayers">Number of layers in the neural network</param>
        /// <param name="_layersNeurons">Neurons in each layer</param>
        /// <param name="_layersTypes">Layers type</param>
        /// <param name="_dimInput">input vectors/pattern for the neuralnetwork</param>
        public NeuralNetwork(int _numLayers, int[] _layersNeurons, int[] _layersTypes, int _dimInput)
        {
            this.numLayers = _numLayers;
            this.dimInput = _dimInput;
            this.layers = new NeuralLayer[this.numLayers];
            this.layersTypes = new int[this.numLayers];
            this.layersNeurons = new int[this.numLayers];

            for (int i = 0; i < this.numLayers; i++)
            {
                this.layersNeurons[i] = _layersNeurons[i];
                this.layersTypes[i] = _layersTypes[i];

                if (i == 0)
                {
                    this.layers[i] = new NeuralLayer(this.dimInput, this.layersTypes[i], this.layersNeurons[i]);
                }
                else
                {
                    this.layers[i] = new NeuralLayer(this.layersNeurons[i - 1], this.layersTypes[i], this.layersNeurons[i]);
                }
            }
        }


        /// <summary>
        /// Initialization of the network
        /// </summary>
        /// <param name="_numLayers">number of layers</param>
        /// <param name="_layersNeurons">number of neurons in layers</param>
        /// <param name="_layersTypes">layer type</param>
        /// <param name="_dimInput">input weight</param>
        public void init(int _numLayers, int[] _layersNeurons, int[] _layersTypes, int _dimInput)
        {
            this.numLayers = _numLayers;
            this.dimInput = _dimInput;

            this.layers = new NeuralLayer[this.numLayers];
            this.layersTypes = new int[this.numLayers];
            this.layersNeurons = new int[this.numLayers];

            for (int i = 0; i < this.numLayers; i++)
            {
                this.layersNeurons[i] = _layersNeurons[i];
                this.layersTypes[i] = _layersTypes[i];

                if (i == 0)
                {
                    this.layers[i] = new NeuralLayer(this.dimInput, this.layersTypes[i], this.layersNeurons[i]);
                    this.layers[i].init();
                }
                else
                {
                    this.layers[i] = new NeuralLayer(this.layersNeurons[i - 1], this.layersTypes[i], this.layersNeurons[i]);
                    this.layers[i].init();
                }
            }
        }

        // Computes the output of the ANN for the given input vector
        public float[] evaluate(float[] input)
        {
            float[] _out = input;
            float[] _in = input;

            for (int i = 0; i < this.numLayers; i++)
            {

                _out = this.layers[i].evaluate(_in);
                _in = _out;
            }

            return _out;
        }

        // Reads the NN source text file and sets up a network according to it
        public void loadNetwork(string fileName)
        {

            System.IO.TextReader file = new System.IO.StreamReader(fileName);

            string line;
            string token;
            int indexNext;

             // read the number of layers
            line = file.ReadLine();
            int _numLayers = int.Parse((line.ToString()));

            line = file.ReadLine();

            // read the dimension of the input vector
            indexNext = line.IndexOf(' ');
            token = line.Substring(0, indexNext + 1);
            line = line.Substring(indexNext + 1);

            int _dimInput = int.Parse((token.ToString()));

            // read the number of neurons in each layer
            int[] _layersNeurons = new int[_numLayers];


            for (int i = 0; i < (_numLayers - 1); i++)
            {
                indexNext = line.IndexOf(' ');
                _layersNeurons[i] = int.Parse((line.Substring(0, indexNext + 1).ToString()));
                line = line.Substring(indexNext + 1);
            }

            _layersNeurons[_numLayers - 1] = int.Parse((line.ToString()));

            line = file.ReadLine(); ;


            // read the types of neurons in each layer
            int[] _layersTypes = new int[_numLayers];
            for (int i = 0; i < (_numLayers - 1); i++)
            {
                indexNext = line.IndexOf(' ');
                _layersTypes[i] = int.Parse((line.Substring(0, indexNext).ToString()));
                line = line.Substring(indexNext + 1);
            }

            _layersTypes[_numLayers - 1] = int.Parse((line.ToString()));

            line = file.ReadLine();

            // Initialize the network
            this.init(_numLayers, _layersNeurons, _layersTypes, _dimInput);

            float[] _weightSet = null;
            float _bias = 0;

            for (int l = 0; l < this.numLayers; l++)
            {

                int neuronCount = 0;

                while (neuronCount < this.layersNeurons[l])
                {

                    if (l == 0)
                    {
                        _weightSet = new float[this.dimInput + 1];
                    }
                    else
                    {
                        _weightSet = new float[this.layersNeurons[l - 1]];
                    }

                    int weightCount = 0;
                    bool next = false;
                    while (!next)
                    {
                        indexNext = line.IndexOf(' ');
                        if (indexNext > 0)
                        {
                            _weightSet[weightCount] = float.Parse((line.Substring(0, indexNext + 1).ToString()));
                            line = line.Substring(indexNext + 1);
                        }
                        else
                        {
                            _bias = float.Parse((line.ToString()));

                            next = true;
                        }

                        weightCount++;
                    }

                    this.layers[l].neurons[neuronCount].init(_weightSet, _bias);

                    line = file.ReadLine();
                    neuronCount++;
                }
            }

            file.Close();
            //Console.WriteLine("Neural Network from: " + fileName + " sucessfully loaded.");

           // this.print();

        }       

        public void runTest(string testFile,string outPutFile)
        {
            System.IO.TextReader fileIn = new System.IO.StreamReader(testFile);
            System.IO.TextWriter fileOut = new System.IO.StreamWriter(outPutFile);
            string line;
            int indexNext;

            /// Open the file

            float[] input;
            float[] output;
            int numInput;

            //I just take the columns Teva
            line = fileIn.ReadLine();

            line = fileIn.ReadLine();
            
            //just teva add
           // line = line.IndexOf(' ').ToString();

            numInput = int.Parse((line.ToString()));
                      
            for (int x = 0; x < numInput; x++)
            {
                line = fileIn.ReadLine();
                input = new float[this.dimInput];
                
                for (int i = 0; i < (this.dimInput - 1); i++)
                {
                    indexNext = line.IndexOf(' ');
                    //indexNext = line.IndexOf(',');
                    input[i] = float.Parse((line.Substring(0, indexNext + 1).ToString()));
                    line = line.Substring(indexNext + 1);
                }

                input[this.dimInput - 1] = float.Parse((line.ToString()));

                output = this.evaluate(input);

                for (int i = 0; i < this.layersNeurons[this.numLayers - 1]; i++)
                {
                    fileOut.Write(output[i] + " ");
                }

                fileOut.WriteLine();
            }

            fileOut.Close();
            fileIn.Close();
        }

        //public void print()
        //{
        //    StreamWriter sr = null;

        //    sr=new StreamWriter("C:\\pf\\Test.txt");

        //    StringBuilder builder = new StringBuilder();


        //    builder.Append("*********************************************************");

        //    builder.Append("Number of layers: " + this.numLayers);

        //    builder.Append("Number of neurons in each layer: ");

        //    for (int i = 0; i < this.numLayers; i++)
        //    {
        //        builder.Append(this.layersNeurons[i] + " ");
        //    }


        //    builder.Append("Types of neurons in each layer: ");
        //    for (int i = 0; i < this.numLayers; i++)
        //    {
        //        builder.Append(this.layersTypes[i] + " ");
        //    }
            
        //    for (int l = 0; l < this.numLayers; l++)
        //    {
        //        builder.Append("Layer " + l + ":");
        //        this.layers[l].print(l);
        //    }

        //    sr.WriteLine(builder.ToString());

        //    sr.Close();

        //}
    };
}
