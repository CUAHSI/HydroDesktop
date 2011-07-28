using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HydroDesktop.DataDownload.Download
{
    /// <summary>
    /// Information about download progress
    /// </summary>
    class ManagerInformation : INotifyPropertyChanged
    {
        private readonly DownloadManager _parent;

        #region Fields
       
        private ICollection<int> _indecesToDownload;

        private const int INITIAL_TIME_TO_SAVE = 1;
        private const int INITIAL_TIME_TO_DOWNLOAD = 15;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of DownloadManagerProgress
        /// </summary>
        /// <param name="startDownloadArg">Information about start downloading call</param>
        /// <param name="parent">Source download manager</param>
        /// <exception cref="ArgumentNullException">startDownloadArg, parent must be not null.</exception>
        public ManagerInformation(StartDownloadArg startDownloadArg, DownloadManager parent)
        {
            if (startDownloadArg == null) throw new ArgumentNullException("startDownloadArg");
            if (parent == null) throw new ArgumentNullException("parent");

            _parent = parent;

            StartDownloadArg = startDownloadArg;
            SetSeriesToDownload(); // by default all series from initial list should be downloaded
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set series to download
        /// </summary>
        /// <param name="indecesToDownload">Series to download</param>
        /// <exception cref="InvalidOperationException">Changing of series to download can not be done when downloading in progress.</exception>
        public void SetSeriesToDownload(ICollection<int> indecesToDownload = null)
        {
            if (_parent.IsBusy)
            {
                throw new InvalidOperationException(
                    "Changing of series to download can not done when downloading in progress.");
            }

            if (indecesToDownload == null)
            {
                var allIndeces = new List<int>(StartDownloadArg.ItemsToDownload.Count);
                for (int i = 0; i < allIndeces.Capacity; i++)
                    allIndeces.Add(i);
                indecesToDownload = allIndeces;
            }

            _indecesToDownload = indecesToDownload;

            // change status
            foreach (var ind in _indecesToDownload)
                StartDownloadArg.ItemsToDownload[ind].Status = DownloadInfoStatus.Pending;
            
            IndecesToDownload = new ReadOnlyCollection<int>(new List<int>(_indecesToDownload));
            TotalSeries = _indecesToDownload.Count;
        }

        #endregion

        #region Properties

        internal ReadOnlyCollection<int> IndecesToDownload { get; private set; }
        public StartDownloadArg StartDownloadArg { get; private set; }

        private volatile int _downloadedAndSaved;
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

        private volatile int _withError;
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
            private set
            {
                _totalSeries = value;
                NotifyPropertyChanged("TotalSeries");

                // update depended properties
                RemainingSeries = TotalSeries;
                WithError = 0;
                DownloadedAndSaved = 0;
                Downloaded = 0;
            }
        }


        private volatile int _remainingSeries;
        public int RemainingSeries
        {
            get { return _remainingSeries; }
            private set
            {
                _remainingSeries = value;
                NotifyPropertyChanged("RemainingSeries");

                // update depended properties
                EstimatedTimeForDownload = new TimeSpan(0, 0, RemainingSeries * INITIAL_TIME_TO_DOWNLOAD);
                EstimatedTimeForSave = new TimeSpan(0, 0, RemainingSeries * INITIAL_TIME_TO_SAVE);
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
