using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMW;
using Oatc.OpenMI.Sdk.Backbone;
using System.Collections;

namespace DiffusiveWave.Source
{
    public class Wrapper: SMW.Wrapper
    {
       

        public  double[,] _soy;
        public  double[,] _sox;
        public  double[,] _elevation;
        public  double hw;
        public  double nx { get; set; }
        public  double ny { get; set; }
        public  double _cellsize;
        private double[] _h;
        private double[] h1;
        private Discretization _discretization;
        private Engine              _engine;
        private string  _inStageElementSet,     _inStageQuantity,
                        _inExcessElementSet,    _inExcessQuantity,
                        _outExcessElementSet,   _outExcessQuantity;
        public double _dt;

        public Wrapper()
        {
            hw = 0.007;
            nx = 0.1;
            ny = 0.1;
            //create an instance of the Engine.  Set wier height hw.
            _engine = new Engine(hw);
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string config = null;
            string elevation = null;
            string fdr = null;

            //read input arguments
            foreach (DictionaryEntry arg in properties)
            {
                switch (arg.Key.ToString())
                {
                    case "ConfigFile":
                        config = arg.Value.ToString();
                        break;
                    case "SurfaceElevation":
                        elevation = arg.Value.ToString();
                        break;
                    case "FlowDirection":
                        fdr = arg.Value.ToString();
                        break;
                }
            }

            //make sure that Surface Elevation and Config path have been specified
            if (config == null || elevation == null || fdr == null)
                throw new Exception("Exception Occured in Diffusive Wave Wrapper: User Must Supply config path, elevation, and fdr in *.omi"); 

            //setup model properties
            this.SetValuesTableFields();
            this.SetVariablesFromConfigFile(config);

            //get model information
            InputExchangeItem input = this.GetInputExchangeItem(0);
            _inExcessElementSet = input.ElementSet.ID;
            _inExcessQuantity = input.Quantity.ID;

            input = this.GetInputExchangeItem(1);
            _inStageElementSet = input.ElementSet.ID;
            _inStageQuantity = input.Quantity.ID;

            OutputExchangeItem output = this.GetOutputExchangeItem(0);
            _outExcessElementSet = output.ElementSet.ID;
            _outExcessQuantity = output.Quantity.ID;

            _dt = this.GetTimeStep();
            
            //Define element set
            ElementSet eset;
            _engine.BuildElementSet(elevation, fdr, this.GetModelID(), this.GetModelDescription(), out eset, out _sox, out _soy);

            //save cell size
            this._cellsize = _engine._cellsize;

            //save elevation
            this._elevation = _engine.elevations;

            //TODO:  Add more discretization schemes to the class, and allow the user to choose which one
            //       to use by passing an argument in the *.omi file.
            //Initialize the Discretization class

            //Try using Differend Discretizations
            //_discretization = new ForwardDifferencing(_engine._cellsize, this.GetTimeStep(), _engine._elementCount);
            _discretization = new Euler(_engine._cellsize,this.GetTimeStep(), _engine._elementCount, _engine.elevations);
            _discretization.Datum = _engine.datum;

            //set Sox and Soy
            _discretization.Sox = _sox;
            _discretization.Soy = _soy;

            //set Nx and Ny
            _discretization.Nx = nx;
            _discretization.Ny = ny;
            
            ////set Sox and Soy
            //_discretization._sox = _sox;
            //_discretization._soy = _soy;

            ////set Nx and Ny
            //_discretization.nx = nx;
            //_discretization.ny = ny;

            //STARTHERE: !!!!!  Read land cover data and set Nx Ny
            //TODO:  Read land cover data and set Nx Ny
            //_discretization._nx = _nx;
            //_discretization._ny = _ny;

            //create array to hold head
            _h = new double[_engine._elementCount];
        }

        public override bool PerformTimeStep()
        {
            //get inputs
            ScalarSet inStage = (ScalarSet)this.GetValues(_inStageQuantity, _inStageElementSet);
            double[] stage = inStage.data;
            ScalarSet inExcess = (ScalarSet)this.GetValues(_inExcessQuantity,_inExcessElementSet);
            double[] excess = inExcess.data;
            
            
            //transform stage into flow
            double[] flow = _engine.Stage2Flow(stage, _h,this.GetTimeStep());

            //initialize excess, if no values were found
            if (excess.Length != _h.Length)
                excess = new double[_h.Length];

            //set the source term equal to the flow into/out-of the floodplain
            double[] b = flow;

            //--- Calculate Heads ---

            //set the head equal to the head from the previous timestep
            _discretization.Head = _h;

            //create stiffness and source matrices
            _discretization.CreateStiffness(stage, excess, _discretization.Head, b, 0);

            //perform SOR to get first approximation of head
            h1 = _engine.SuccessiveOverRelaxation(_discretization.A, _discretization.q);

            //if any head values are negative, set them to zero
            for (int i = 0; i <= h1.Length - 1; i++)
                if (h1[i] < 0)
                    h1[i] = 0;

            double[,] AA = _discretization.A;
            double[] qq = _discretization.q; 

            //reset A and q to zero
            Array.Clear(_discretization.A, 0, _discretization.A.Length);
            Array.Clear(_discretization.q, 0, _discretization.q.Length);

            //create stiffness and source matrices again using the initial stage and excess, but with the heads calculated in the previous iteration
            _discretization.CreateStiffness(stage, excess, h1, b, 1);

            //perform SOR to get first approximation of head
            h1 = _engine.SuccessiveOverRelaxation(_discretization.A, _discretization.q);

            //if any head values are negative, set them to zero
            for (int i = 0; i <= h1.Length - 1; i++)
                if (h1[i] < 0)
                    h1[i] = 0;

            //save these head values for the next time step
            _h = h1;

            //this.SetValues(_outExcessQuantity, _outExcessElementSet, new ScalarSet(_discretization.q));
            this.SetValues(_outExcessQuantity, _outExcessElementSet, new ScalarSet(_h));

            //reset A and q to zero
            Array.Clear(_discretization.A, 0, _discretization.A.Length);
            Array.Clear(_discretization.q, 0, _discretization.q.Length);

            return true;
        }

        public override void Finish()
        {
        }
    }

    public class Wrapper2 : SMW.Wrapper
    {


        //public double[,] _soy;
        //public double[,] _sox;
        public int rows, cols;
        //public double[,] _elevation;
        public double hw;
        public double nx { get; set; }
        public double ny { get; set; }
        public double _cellsize;
        private double[] _h;
        private double[] h1;
        private Engine2 _engine;
        private string _inStageElementSet, _inStageQuantity,
                        _inExcessElementSet, _inExcessQuantity,
                        _outExcessElementSet, _outExcessQuantity;
        public double _dt;

        public Wrapper2()
        {
            hw = 0.007;
            nx = 0.1;
            ny = 0.1;
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string config = null;
            string elevation = null;
            string fdr = null;

            //read input arguments
            foreach (DictionaryEntry arg in properties)
            {
                switch (arg.Key.ToString())
                {
                    case "ConfigFile":
                        config = arg.Value.ToString();
                        break;
                    case "SurfaceElevation":
                        elevation = arg.Value.ToString();
                        break;
                    case "FlowDirection":
                        fdr = arg.Value.ToString();
                        break;
                }
            }

            //make sure that Surface Elevation and Config path have been specified
            if (config == null || elevation == null || fdr == null)
                throw new Exception("Exception Occured in Diffusive Wave Wrapper: User Must Supply config path, elevation, and fdr in *.omi");

            //setup model properties
            this.SetValuesTableFields();
            this.SetVariablesFromConfigFile(config);

            //get model information
            InputExchangeItem input = this.GetInputExchangeItem(0);
            _inExcessElementSet = input.ElementSet.ID;
            _inExcessQuantity = input.Quantity.ID;

            input = this.GetInputExchangeItem(1);
            _inStageElementSet = input.ElementSet.ID;
            _inStageQuantity = input.Quantity.ID;

            OutputExchangeItem output = this.GetOutputExchangeItem(0);
            _outExcessElementSet = output.ElementSet.ID;
            _outExcessQuantity = output.Quantity.ID;

            _dt = this.GetTimeStep();

            //create instance of engine
            _engine = new Engine2(hw, _dt);

            //Define element set
            ElementSet eset;
            _engine.BuildElementSet(elevation, this.GetModelID(), this.GetModelDescription(), out eset);

            //save cell size
            this._cellsize = _engine._cellsize;

            //save elevation
            //this._elevation = _engine._elevations;
            rows = _engine.rows;
            cols = _engine.cols;
            
            //set ny and nx
            _engine.ny = this.ny;
            _engine.nx = this.nx;

            //STARTHERE: !!!!!  Read land cover data and set Nx Ny
            //TODO:  Read land cover data and set Nx Ny
            //_discretization._nx = _nx;
            //_discretization._ny = _ny;

            //create array to hold head
            _h = new double[_engine._elementCount];
        }

        public override bool PerformTimeStep()
        {
            //get inputs
            ScalarSet inStage = (ScalarSet)this.GetValues(_inStageQuantity, _inStageElementSet);
            double[] stage = inStage.data;
            ScalarSet inExcess = (ScalarSet)this.GetValues(_inExcessQuantity, _inExcessElementSet);
            double[] excess = inExcess.data;


            //transform stage into flow
            double[] flow = _engine.Stage2Flow(stage, _h, this.GetTimeStep());

            //initialize excess, if no values were found
            if (excess.Length != _h.Length)
                excess = new double[_h.Length];

            //set the source term equal to the flow into/out-of the floodplain
            double[] b = flow;

            //--- Calculate Heads ---

            //set the head equal to the head from the previous timestep
            _engine.head = _h;

            //create stiffness and source matrices
            _engine.CreateStiffness(stage, excess, _engine.head, b, 0);

            //perform SOR to get first approximation of head
            h1 = _engine.SuccessiveOverRelaxation();

            //if any head values are negative, set them to zero
            for (int i = 0; i <= h1.Length - 1; i++)
                if (h1[i] < 0)
                    h1[i] = 0;

            double[,] AA = _engine.A;
            double[] qq = _engine.q;

            //reset A and q to zero
            Array.Clear(_engine.A, 0, _engine.A.Length);
            Array.Clear(_engine.q, 0, _engine.q.Length);

            //create stiffness and source matrices again using the initial stage and excess, but with the heads calculated in the previous iteration
            _engine.CreateStiffness(stage, excess, h1, b, 1);

            //perform SOR to get first approximation of head
            h1 = _engine.SuccessiveOverRelaxation();

            //if any head values are negative, set them to zero
            for (int i = 0; i <= h1.Length - 1; i++)
                if (h1[i] < 0)
                    h1[i] = 0;

            //save these head values for the next time step
            _h = h1;

            //this.SetValues(_outExcessQuantity, _outExcessElementSet, new ScalarSet(_discretization.q));
            this.SetValues(_outExcessQuantity, _outExcessElementSet, new ScalarSet(_h));

            //reset A and q to zero
            Array.Clear(_engine.A, 0, _engine.A.Length);
            Array.Clear(_engine.q, 0, _engine.q.Length);

            return true;
        }

        public override void Finish()
        {
        }
    }
}
