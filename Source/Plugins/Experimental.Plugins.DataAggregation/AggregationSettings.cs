using System;
using HydroDesktop.Common;

namespace HydroDesktop.Plugins.DataAggregation
{
    /// <summary>
    /// Contains settings for aggregation
    /// </summary>
    internal class AggregationSettings : ObservableObject<AggregationSettings>
    {
        #region Fields

        private AggregationMode _aggregationMode;
        private DateTime _startTime;
        private DateTime _endTime;
        private string _variableCode;
        private bool _createNewLayer;
        private bool _createCategories;
        private byte _decimalPlaces;

        #endregion

        #region Properties

        public AggregationMode AggregationMode
        {
            get { return _aggregationMode; }
            set
            {
                _aggregationMode = value;
                NotifyPropertyChanged(() => AggregationMode);
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                NotifyPropertyChanged(() => StartTime);
            }
        }
        
        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                NotifyPropertyChanged(() => EndTime);
            }
        }
        
        public string VariableCode
        {
            get { return _variableCode; }
            set
            {
                _variableCode = value;
                NotifyPropertyChanged(() => VariableCode);
            }
        }
        
        public bool CreateNewLayer
        {
            get { return _createNewLayer; }
            set
            {
                _createNewLayer = value;
                NotifyPropertyChanged(() => CreateNewLayer);
            }
        }

        public bool CreateCategories
        {
            get { return _createCategories; }
            set
            {
                _createCategories = value;
                NotifyPropertyChanged(() => CreateCategories);
            }
        }

        public byte DecimalPlaces
        {
            get { return _decimalPlaces; }
            set
            {
                _decimalPlaces = value;
                NotifyPropertyChanged(() => DecimalPlaces);
            }
        }

        #endregion
    }
}