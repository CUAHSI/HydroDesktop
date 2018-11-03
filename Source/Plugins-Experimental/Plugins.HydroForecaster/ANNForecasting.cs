using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Spatial.Tools;
using System.Spatial.Tools.Param;

namespace HydroForecaster
{
    class ANNForecasting:ITool
    {
        private Parameter[] _inputParameters;
        private Parameter[] _outputParameters;
        private string _workingPath;

        #region ITool Members

        /// <summary>
        /// Gets or Sets the input paramater array
        /// </summary>
        Parameter[] ITool.InputParameters
        {
            get { return (_inputParameters); }
        }

        /// <summary>
        /// Gets or Sets the output paramater array
        /// </summary>
        Parameter[] ITool.OutputParameters
        {
            get { return (_outputParameters); }
        }


        public string Author
        {
            get { return "Neural Network Model"; }
        }

        public string Category
        {
            get { return "Forecasting"; }
        }

        public string Description
        {
            get { return "Neural Network Model"; }
        }

        bool ITool.Execute(ICancelProgressHandler cancelProgressHandler)
        {
            string InputFile = _inputParameters[1].Value.ToString();

            string ResultDestination = _outputParameters[0].Value.ToString();

            string Networkpath = _inputParameters[0].Value.ToString();

            if (Execute(Networkpath,InputFile, ResultDestination, cancelProgressHandler))
            {
              return true;
            }
            else
            {
                            
            }
            {
                  return false ;
            }
          
        }

        public bool Execute(string NetworkWeight, string Inputdata,string Result, ICancelProgressHandler cancelProgressHandler)
        {
            NeuralNetworkTesting nn = new NeuralNetworkTesting();

            nn.loadNetwork(NetworkWeight);

            nn.runTest(Inputdata, Result);

            for (int i=0;i<nn.numLayers;i++)
            {
                cancelProgressHandler.Progress("", 100, nn.layersNeurons[i].ToString() + nn.layersTypes[i].ToString());
            if (cancelProgressHandler.Cancel)
                return false;
            }
           
            return true;            
        }

        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        public string HelpText
        {
            get { return "This tool uses Feed Forward Neural Network structure and Error back propagation algorithm. Users need to give input trainned neurons and their weights.Based on the neurons weight and input normalized data this tool give the forecasting output."; }
        }

        public string HelpUrl
        {
            get { return "Neural Network Model"; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return (null); }
        }

        void ITool.Initialize()
        {
            _inputParameters = new Parameter[3];

            _inputParameters[0] = new FileParam("Select the Neurons and their weights for the network");

            _inputParameters[1] = new FileParam("Select the input data file");

            _inputParameters[2] = new DateTimeParam("Please select the forecasting final date");
            
            _outputParameters = new Parameter[1];
            _outputParameters[0] = new FileParam("Forecasting Values");
        }


        public string Name
        {
            get { return "Neural Network Model"; }
        }


        void ITool.ParameterChanged(Parameter sender)
        {
            return;
        }

        public string ToolTip
        {
            get { return "Neural Network Model"; }
        }

        public string UniqueName
        {
            get { return "Neural Network Model"; }
        }

        public Version Version
        {
            get { return (new Version(1, 0, 0, 0)); }
        }

        string ITool.WorkingPath
        {
            set { _workingPath = value; }
        }

        #endregion
    }
}
