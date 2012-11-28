using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HydroDesktop.Common;

namespace HydroDesktop.DataDownload.Downloading
{
    /// <summary>
    /// Information about download progress
    /// </summary>
    class ManagerInformation : ObservableObject<ManagerInformation>
    {
        private readonly DownloadManager _parent;

        #region Fields
       
        private ICollection<int> _indecesToDownload;

        private const int INITIAL_TIME_TO_SAVE = 1;

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

            StartArgs = startDownloadArg;
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
                var allIndeces = new List<int>(StartArgs.ItemsToDownload.Count);
                for (int i = 0; i < allIndeces.Capacity; i++)
                    allIndeces.Add(i);
                indecesToDownload = allIndeces;
            }

            _indecesToDownload = indecesToDownload;

            // change status
            foreach (var ind in _indecesToDownload)
                StartArgs.ItemsToDownload[ind].Status = DownloadInfoStatus.Pending;
            
            IndecesToDownload = new ReadOnlyCollection<int>(new List<int>(_indecesToDownload));
            TotalSeries = _indecesToDownload.Count;
        }

        #endregion

        #region Properties

        internal ReadOnlyCollection<int> IndecesToDownload { get; private set; }
        public StartDownloadArg StartArgs { get; private set; }

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
                NotifyPropertyChanged(() => DownloadedAndSaved);

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
                NotifyPropertyChanged(() => Downloaded);
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
                NotifyPropertyChanged(() => WithError);

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
                NotifyPropertyChanged(() => TotalSeries);

                // update depended properties
                RemainingSeries = TotalSeries;
                WithError = 0;
                DownloadedAndSaved = 0;
                Downloaded = 0;
                TimeTakenForDownloading = new TimeSpan();
            }
        }


        private volatile int _remainingSeries;
        public int RemainingSeries
        {
            get { return _remainingSeries; }
            private set
            {
                _remainingSeries = value;
                NotifyPropertyChanged(() => RemainingSeries);

                // update depended properties
                RefreshEstimatedTimeForDownload();
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
                NotifyPropertyChanged(() => EstimatedTime);
            }
        }

        private TimeSpan _estimatedTimeForDownload;
        public TimeSpan EstimatedTimeForDownload
        {
            get { return _estimatedTimeForDownload; }
            private set
            {
                _estimatedTimeForDownload = value;
                NotifyPropertyChanged(() => EstimatedTimeForDownload);

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
                NotifyPropertyChanged(() => EstimatedTimeForSave);

                EstimatedTime = EstimatedTimeForDownload.Add(EstimatedTimeForSave);
            }
        }

        private TimeSpan TimeTakenForDownloading { get; set; }
        public void AddTimeTaken(TimeSpan timeSpan)
        {
            TimeTakenForDownloading = TimeTakenForDownloading.Add(timeSpan);
        }
     
        public void RefreshEstimatedTimeForDownload()
        {
            double avgTimeToSeries = 0;
            if (TotalSeries != RemainingSeries)
                avgTimeToSeries = TimeTakenForDownloading.TotalSeconds / (TotalSeries - RemainingSeries);
            
            double remaingTime = 0;
            foreach (var item in StartArgs.ItemsToDownload)
            {
                if (item.Status == DownloadInfoStatus.Pending && avgTimeToSeries > 0)
                    item.EstimatedTimeToDownload = avgTimeToSeries;

                if (item.Status == DownloadInfoStatus.Downloading || item.Status == DownloadInfoStatus.Pending) 
                    remaingTime += item.EstimatedTimeToDownload;
            }

            EstimatedTimeForDownload = new TimeSpan(0, 0, (int)remaingTime);
        }

        public double GetTotalProgress()
        {
            return StartArgs.ItemsToDownload.Sum(item => item.Progress)/StartArgs.ItemsToDownload.Count;
        }

        #endregion
    }
}
