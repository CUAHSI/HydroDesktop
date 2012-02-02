using System;
using HydroDesktop.Common;

namespace HydroDesktop.DataDownload.DataAggregation
{
    /// <summary>
    /// Contains settings for aggregation
    /// </summary>
    internal class AggregationSettings : ObservableObject<AggregationSettings>
    {
        private AggregationMode _aggregationMode;
        public AggregationMode AggregationMode
        {
            get { return _aggregationMode; }
            set
            {
                _aggregationMode = value;
                NotifyPropertyChanged(x => AggregationMode);
            }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                NotifyPropertyChanged(x => StartTime);
            }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                NotifyPropertyChanged(x => EndTime);
            }
        }

        private string _variableCode;
        public string VariableCode
        {
            get { return _variableCode; }
            set
            {
                _variableCode = value;
                NotifyPropertyChanged(x => VariableCode);
            }
        }

        private bool _createNewLayer;
        public bool CreateNewLayer
        {
            get { return _createNewLayer; }
            set
            {
                _createNewLayer = value;
                NotifyPropertyChanged(x => CreateNewLayer);
            }
        }
    }
}