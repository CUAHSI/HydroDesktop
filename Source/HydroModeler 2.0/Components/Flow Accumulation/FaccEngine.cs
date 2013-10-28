using System;
using System.Collections;
using System.Collections.Generic;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessor;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.ModelWrapper;
using OpenMI.Standard2;
using System.Collections.ObjectModel;
using System.IO;

namespace FlowAccumulation
{
    public class FaccEngine : LinkableGetSetEngine
    {
               private List<IInput> _inputs = new List<IInput>();
        private List<IOutput> _outputs = new List<IOutput>();
        private EngineInputItem _InputItem;
        EngineOutputItem _OutputItem;
        private readonly Time _simulationEnd;
        private readonly Time _simulationStart;
        private Time _currentTime;
        private readonly double _timeStepLengthInSeconds; 
        private readonly double _timeStepLengthInDays;
        private string _Inpath;
        private string _Outpath;
        private string _outpath;

        private Geoprocessor GP;
        private int numProcessed;
        IAoInitialize license;

        public FaccEngine()
        {
            //model ID
            this.Id = "Flow Accumulation";
            //Component GUI Caption
            this.Caption = "Flow Accumulation";
            //model description
            this.Description = "Calculates the Flow Accumulation for each pixel on a surface raster";

            _simulationStart = new Time(new DateTime(2005, 1, 1, 0, 0, 0));
            _simulationEnd = new Time(new DateTime(2005, 1, 2, 0, 0, 0));
            _currentTime = new Time(_simulationStart);
            _timeStepLengthInSeconds = 3600 * 24; //one day
            _timeStepLengthInDays = _timeStepLengthInSeconds / (24.0 * 3600.0);
            numProcessed = 0;
            license = new AoInitializeClass();
        }
        public override void Finish()
        {
            license.Shutdown();

        }

        public override void Initialize(IArgument[] arguments)
        {
            //set component to run in loop mode
            this.CascadingUpdateCallsDisabled = true;
        
            Status = LinkableComponentStatus.Initializing;

            //read arguments
            foreach (IArgument entry in arguments)
            {
                if (entry.Id == "FlowDirection")
                {
                    _Inpath = Path.GetFullPath(entry.Value.ToString());
                }
                else if (entry.Id == "OutputFile")
                {
                    _outpath = Path.GetFullPath(entry.Value.ToString());
                }
            }

            // -- Time settings for input and output exchange items --
            ITime timeHorizon = new Time(StartTime, EndTime);


            //Create input element set
            Element e = new Element("Flow Direction");
            e.Id = "Flow Direction";
            ElementSet eSet = new ElementSet("Flow Direction", "Flow Direction", ElementType.IdBased);
            eSet.AddElement(e);
            Quantity quantity = new Quantity(new Unit("Raster", 1.0, 0.0, "Raster"), "Flow Direction", "Flow Direction");
            //add input item
            _InputItem = new EngineEInputItem("Flow Direction", quantity, eSet, this);
            //_InputItem.StoreValuesInExchangeItem = true;
            _InputItem.SetTimeHorizon(timeHorizon);
            this.EngineInputItems.Add(_InputItem);
            _InputItem.SetSingleTime(StartTime);
            

            //add input exchange item to input item list
            _inputs.Add(_InputItem);


            //create output element set
            e = new Element("Flow Accumulation");
            e.Id = "Flow Accumulation";
            eSet = new ElementSet("Flow Accumulation", "Flow Accumulation", ElementType.IdBased);
            eSet.AddElement(e);
            quantity = new Quantity(new Unit("Raster", 1.0, 0.0, "Raster"), "Flow Accumulation", "Flow Accumulation");
            //add output item
            _OutputItem = new EngineEOutputItem("Flow Accumulation", quantity, eSet, this);
            _OutputItem.SetSingleTime(StartTime);
            
            //_OutputItem.StoreValuesInExchangeItem = true;
            _OutputItem.SetTimeHorizon(timeHorizon);
            this.EngineOutputItems.Add(_OutputItem);
            
            //add output exchange item to output item list
            _outputs.Add(_OutputItem);




            //initialize geoprocessing objects
            GP = new Geoprocessor();
            GP.OverwriteOutput = true;

            //checkout spatial analyst license
            esriLicenseStatus LicenseStatus = esriLicenseStatus.esriLicenseUnavailable;
            LicenseStatus = license.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcInfo);
            LicenseStatus = license.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);

            
            Status = LinkableComponentStatus.Initialized;
        }



        public override void Prepare()
        {
        }

        public override IValueSet GetEngineValues(ExchangeItem exchangeItem)
        {
            //--- give output items ---

            IList values = new List<string>();
            if (exchangeItem is EngineInputItem)
            {
                // Input item, provide current input values
                if (exchangeItem == _InputItem)
                {
                    values.Add(_Inpath);

                    //determine outpath
                    string[] inpath = _Inpath.Split('/');
                    int l = inpath[inpath.Length].Length;
                    _Outpath = _Inpath.Remove(inpath.Length - l);
                    _Outpath += exchangeItem.Id + "_FDR";
                }
                else
                {
                    throw new ArgumentException("Unknown Exchange Item Id: \"" + exchangeItem.Id + "\"", "exchangeItem");
                }
            }
            else if (exchangeItem is EngineOutputItem)
            {
                // Output item, provide computed values
                if (exchangeItem == _OutputItem)
                {
                    //if (_Outpath == null)
                    //{
                        //if no values exist, then run perform time step to calculate them
                        ICollection<EngineOutputItem> outputs = new Collection<EngineOutputItem>();
                        outputs.Add(_outputExchangeItems[0]);
                        this.PerformTimestep(outputs);

                        if (_Outpath != null) { values.Add(_Outpath); }
                        else
                        {
                            //if output path is still null, try updating inputs
                            _inputExchangeItems[0].Update();
                            //this.ProcessActiveInputItems();
                            this.PerformTimestep(outputs);
                        }

                        if (_Outpath == null) { values.Add(null); }
                        else { values.Add(_Outpath); }
                    //}
                    //else
                    //{
                    //    values.Add(_Outpath);
                    //}
                }
                else
                {
                    throw new ArgumentException("Unknown Exchange Item Id: \"" + exchangeItem.Id + "\"", "exchangeItem");
                }
            }
            else
            {
                throw new Exception("Should be EngineInputItem or EngineOutputItem");
            
            }
            //clear output
            this._Outpath = null;
            return new ValueSet(new List<IList> { values });

        }

        public override void SetEngineValues(EngineInputItem inputItem, IValueSet values)
        {
            //--- set input values ---
            if (inputItem == _InputItem)
            {
                if (values == null)
                    _Inpath = null;
                else
                    _Inpath = (string)values.GetValue(0,0);

            }
            else
            {
                throw new ArgumentException("Unknown Input Item Id: \"" + inputItem.Id + "\"", "inputItem");
            }
        }

        public override ITime CurrentTime
        {
            get { return _currentTime; }
        }

        protected override ITime EndTime
        {
            get { return _simulationEnd; }
        }

        public override ITime GetCurrentTime(bool asStamp)
        {
            double timeStepLengthInDays = _timeStepLengthInSeconds / (60 * 60 * 24);

            if (asStamp)
            {
                return new Time(_currentTime);
            }

            return (new Time(_currentTime.StampAsModifiedJulianDay - timeStepLengthInDays, timeStepLengthInDays));
        }

        public override ITime GetInputTime(bool asStamp)
        {
            Time targetTime;
            if (asStamp)
                //targetTime = new Time(_currentTime.StampAsModifiedJulianDay + _timeStepLengthInDays);
                targetTime = _currentTime;
            else
                targetTime = new Time(_currentTime.StampAsModifiedJulianDay, _timeStepLengthInDays);
            return targetTime;
        }

        protected override void OnPrepare()
        {
        }

        protected override string[] OnValidate()
        {
            return new string[0];
        }

        protected override void PerformTimestep(ICollection<EngineOutputItem> requiredOutputItems)
        {
            //--- perform calculation ---
            //Status = LinkableComponentStatus.Updating;

            if (_Inpath != null)
            {
                ESRI.ArcGIS.SpatialAnalystTools.FlowAccumulation acc = new ESRI.ArcGIS.SpatialAnalystTools.FlowAccumulation();
                acc.in_flow_direction_raster = _Inpath;
                acc.data_type = "FLOAT";

                _Outpath = _outpath;


                if (_Outpath == null)
                {
                    //determine outpath
                    string[] inpath = _Inpath.Split('\\');
                    int l = inpath[inpath.Length - 1].Length;
                    _Outpath = _Inpath.Remove(_Inpath.Length - l);

                    string name = _OutputItem.ElementSet.Caption;
                    name = name.Replace(" ", "");
                    if (name.Length >= 9)
                        name = name.Remove(9);

                    _Outpath += name + "_acc";
                }


                acc.out_accumulation_raster = _Outpath;

                GP.Execute(acc, null);
                if (GP.MaxSeverity == 2)
                {
                    object sev = 2;
                    throw new Exception("Unable to perform raster flow acculumation operationESRI ERROR: " + GP.GetMessages(ref sev));
                }

               

                numProcessed++;
                _currentTime.AddSeconds(_timeStepLengthInSeconds);

                this._outputExchangeItems[0].SetSingleTime(this.GetCurrentTime(true));
                this._inputExchangeItems[0].SetSingleTime(this.GetCurrentTime(true));

                //Status = LinkableComponentStatus.Updated;

                //clear inputs
                this._Inpath = null;

                //if (numProcessed >= requiredOutputItems.Count)
                //{
                //    //_currentTime = _simulationEnd;
                //    Status = LinkableComponentStatus.Done;
                //}
                
            }

            

        }

        protected override ITime StartTime
        {
            get { return _simulationStart; }
        }
    }
}
