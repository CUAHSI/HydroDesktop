using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.ModelWrapper;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.esriSystem;
using System.IO;
using System.Collections.ObjectModel;

namespace Spline
{
    public class SplineEngine : LinkableGetSetEngine
    {
        private EngineInputItem _InputItem;
        EngineOutputItem _OutputItem;
        private readonly Time _simulationEnd;
        private readonly Time _simulationStart;
        private Time _currentTime;
        private readonly double _timeStepLengthInSeconds;
        double _timeStepLengthInDays;
        private string _Inpath;
        private string _Outpath;
        private string _outpath;

        private Geoprocessor GP;
        Dictionary<string, bool> _outputFiles;
        private int numProcessed;
        IAoInitialize license;

        public SplineEngine()
        {
            //Model Info
            Id = "Spline Interpolation";
            Caption = "Spline Interpolation";
            Description = "Performs a spline interpolation to create a surface raster";

            _simulationStart = new Time(new DateTime(2005, 1, 1, 0, 0, 0));
            _simulationEnd = new Time(new DateTime(2005, 1, 2, 0, 0, 0));
            _currentTime = new Time(_simulationStart);
            _timeStepLengthInSeconds = 3600 * 24;
            _timeStepLengthInDays = _timeStepLengthInSeconds / (24.0 * 3600.0);
            _outputFiles = new Dictionary<string,bool>();
            numProcessed = 0;
            license = new AoInitializeClass();
        }

        public override IValueSet GetEngineValues(Oatc.OpenMI.Sdk.Backbone.ExchangeItem exchangeItem)
        {
            //--- give output items ---

            IList values = new List<string>();
            if (exchangeItem is EngineInputItem)
            {
                // Input item, provide current input values
                if (exchangeItem == _InputItem)
                {
                    values.Add(_Inpath);
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
                _Inpath = (string)values.GetValue(0, 0);

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

        public override void Finish()
        {
            license.Shutdown();
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

        public override void Initialize(IArgument[] arguments)
        {

            //set component to run in loop mode
            this.CascadingUpdateCallsDisabled = true;

            Status = LinkableComponentStatus.Initializing;

            //read arguments
            foreach (IArgument entry in arguments)
            {
                if (entry.Id == "ElevationPoints")
                {
                    _Inpath = Path.GetFullPath( entry.Value.ToString());
                }
                else if (entry.Id == "OutputFile")
                {
                    _outpath = Path.GetFullPath(entry.Value.ToString());
                }
            }


            // -- Time settings for input and output exchange items --
            ITime timeHorizon = new Time(StartTime, EndTime);


            //Create input element set
            Element e = new Element("Elevation Points");
            e.Id = "Elevation Points";
            ElementSet eSet = new ElementSet("Elevation Points", "Elevation Points", ElementType.IdBased);
            eSet.AddElement(e);
            Quantity quantity = new Quantity(new Unit("Point Shapefile", 1.0, 0.0, "Point Shapefile"), "Elevation", "Elevation");
            //add input item
            _InputItem = new EngineEInputItem("Elevation Points", quantity, eSet, this);
            //_InputItem.StoreValuesInExchangeItem = true;
            _InputItem.SetTimeHorizon(timeHorizon);
            _InputItem.SetSingleTime(StartTime);
            this.EngineInputItems.Add(_InputItem);
            

            //create output element set
            e = new Element("Spline Surface Interpolation");
            e.Id = "Spline Surface Interpolation";
            eSet = new ElementSet("Spline Surface Interpolation", "Spline Surface Interpolatione", ElementType.IdBased);
            eSet.AddElement(e);
            quantity = new Quantity(new Unit("Raster", 1.0, 0.0, "Raster"), "Spline Surface Interpolation", "Spline Surface Interpolation");
            //add output item
            _OutputItem = new EngineEOutputItem("Spline Surface Interpolation", quantity, eSet, this);
            _OutputItem.SetSingleTime(StartTime);
            
            //_OutputItem.StoreValuesInExchangeItem = true;
            _OutputItem.SetTimeHorizon(timeHorizon);
            this.EngineOutputItems.Add(_OutputItem);


            //initialize geoprocessing obkects
            GP = new Geoprocessor();
            GP.OverwriteOutput = true;

            //checkout spatial analyst license
            esriLicenseStatus LicenseStatus = esriLicenseStatus.esriLicenseUnavailable;
            LicenseStatus = license.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcInfo);
            LicenseStatus = license.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);



            Status = LinkableComponentStatus.Initialized;

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

            //this.Status = LinkableComponentStatus.Updating;

            if (_Inpath != null)
            {
                ESRI.ArcGIS.SpatialAnalystTools.Spline spline = new ESRI.ArcGIS.SpatialAnalystTools.Spline();
                spline.in_point_features = _Inpath;
                spline.z_field = "Z";
                spline.number_points = 12;
                spline.weight = 0.1;
                spline.spline_type = "REGULARIZED";
                spline.cell_size = 10;

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

                    _Outpath += name + "_spl";


                }
                spline.out_raster = _Outpath;

                //check to see if this file has already been created
                if (!_outputFiles.ContainsKey(_Outpath))
                {
                    _outputFiles.Add(_Outpath, true);

                    GP.Execute(spline, null);
                    if (GP.MaxSeverity == 2)
                    {
                        object sev = 2;
                        throw new Exception("Spline Interpolation Failed. Input file path = " + _Inpath + "  ESRI ERROR: " + GP.GetMessages(ref sev));
                    }
                }

                //clear inputs
                //this._Inpath = null;
            }


            numProcessed++;

            
            _currentTime.AddSeconds(_timeStepLengthInSeconds);
            this._outputExchangeItems[0].SetSingleTime(this.GetCurrentTime(true));
            this._inputExchangeItems[0].SetSingleTime(this.GetCurrentTime(true));

            //if (numProcessed >= requiredOutputItems.Count)
            //{
            //    _currentTime = _simulationEnd;
            //}
            //else
            //    this.Status = LinkableComponentStatus.Updated;

            
        }

        protected override ITime StartTime
        {
            get { return _simulationStart; }
        }



    }
}
