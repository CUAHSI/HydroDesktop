using System;
using System.ComponentModel;

namespace HydroDesktop.Search.Download
{
    /// <summary>
    /// Information about download progress
    /// </summary>
    class DownloadProgressInfo : INotifyPropertyChanged
    {
        #region Properties

        private int _downloadedAndSaved;
        /// <summary>
        /// Number of downloaded and saved series
        /// </summary>
        public int DownloadedAndSaved
        {
            get { return _downloadedAndSaved; }
            set
            {
                _downloadedAndSaved = value;
                NotifyPropertyChanged("DownloadedAndSaved");

                // update depended properties
                RemainingSeries = TotalSeries - (DownloadedAndSaved + WithError);
            }
        }

        private int _downloaded;
        /// <summary>
        /// Number of downloaded series
        /// </summary>
        public int Downloaded
        {
            get { return _downloaded; }
            set
            {
                _downloaded = value;
                NotifyPropertyChanged("Downloaded");
            }
        }

        private int _withError;
        /// <summary>
        /// Number of series with errors
        /// </summary>
        public int WithError
        {
            get { return _withError; }
            set
            {
                _withError = value;
                NotifyPropertyChanged("WithError");

                // update depended properties
                RemainingSeries = TotalSeries - (DownloadedAndSaved + WithError);
            }
        }


        private int _totalSeries;
        public int TotalSeries
        {
            get { return _totalSeries; }
            set
            {
                _totalSeries = value;
                NotifyPropertyChanged("TotalSeries");

                // update depended properties
                EstimatedTimeForDownload = new TimeSpan(0, 0, _totalSeries * 15);
                EstimatedTimeForSave = new TimeSpan(0, 0, _totalSeries * 5);
                RemainingSeries = TotalSeries;
                WithError = 0;
                DownloadedAndSaved = 0;
                Downloaded = 0;
            }
        }


        private int _remainingSeries;
        public int RemainingSeries
        {
            get { return _remainingSeries; }
            private set
            {
                _remainingSeries = value;
                NotifyPropertyChanged("RemainingSeries");
            }
        }

        private TimeSpan _estimatedTime;
        public TimeSpan EstimatedTime
        {
            get { return _estimatedTime; }
            private set
            {
                _estimatedTime = value;
                NotifyPropertyChanged("EstimatedTime");
            }
        }

        private TimeSpan _estimatedTimeForDownload;
        public TimeSpan EstimatedTimeForDownload
        {
            get { return _estimatedTimeForDownload; }
            set
            {
                _estimatedTimeForDownload = value;
                NotifyPropertyChanged("EstimatedTimeForDownload");

                EstimatedTime = EstimatedTimeForDownload.Add(EstimatedTimeForSave);
            }
        }

        private TimeSpan _estimatedTimeForSave;
        public TimeSpan EstimatedTimeForSave
        {
            get { return _estimatedTimeForSave; }
            set
            {
                _estimatedTimeForSave = value;
                NotifyPropertyChanged("EstimatedTimeForSave");

                EstimatedTime = EstimatedTimeForDownload.Add(EstimatedTimeForSave);
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private methods

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
