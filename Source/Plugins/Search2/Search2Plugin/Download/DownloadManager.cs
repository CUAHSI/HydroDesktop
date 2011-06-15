using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Search.Download.Exceptions;

namespace HydroDesktop.Search.Download
{
    class DownloadManager
    {
        #region Fields

        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private DownloadManagerUI _downloadManagerUI;

        #endregion

        #region Constructors

        public DownloadManager()
        {
            _worker.DoWork += _worker_DoWork;
            _worker.ProgressChanged += _worker_ProgressChanged;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
        }

        #endregion

        #region Events

        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler<RunWorkerCompletedEventArgs> Completed;
        public event EventHandler Canceled;
        public event EventHandler<LogMessageEventArgs> OnMessage;

        #endregion

        #region Properties

        internal bool IsBusy
        {
            get { return _worker.IsBusy; }
        }

        internal log4net.ILog Log
        {
            get;
            set;
        }

        internal bool IsUIVisible
        {
            get { return _downloadManagerUI == null ? false : _downloadManagerUI.Visible; }
        }

        internal DownloadArg CurrentDownloadArg
        {
            get;
            private set;
        }

        #endregion

        #region Public & Internal methods

        /// <summary>
        /// Start downloading
        /// </summary>
        /// <param name="args">Download args</param>
        /// <exception cref="ArgumentNullException">args must be not null</exception>
        internal void Start(DownloadArg args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            CurrentDownloadArg = args;
            _downloadManagerUI = new DownloadManagerUI(this);
            ShowUI();
            _worker.RunWorkerAsync(new DownloadArgWrapper { DownloadArg = args, DownloadManagerUI = _downloadManagerUI });

            DoLogInfo("Starting downloading...");
        }

        internal void Cancel()
        {
            _worker.CancelAsync();

            var handler = Canceled;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            DoLogInfo("Cancelling...");
        }

        internal void ShowUI()
        {
            if (_downloadManagerUI == null) return;
            _downloadManagerUI.Show();
        }

        internal void HideUI()
        {
            if (_downloadManagerUI == null) return;
            _downloadManagerUI.Hide();
        }

        #endregion

        #region Private methods

        private void DoLogInfo(string message, Exception exception = null)
        {
            DoLog(LogKind.Info, message, exception);
        }

        private void DoLogError(string message, Exception exception = null)
        {
            DoLog(LogKind.Error, message, exception);
        }

        private void DoLog(LogKind logKind, string message, Exception exception = null)
        {
            if (Log != null)
            {
                switch (logKind)
                {
                    case LogKind.Error:
                        Log.Error(message, exception);
                        break;
                    case LogKind.Info:
                        Log.Info(message, exception);
                        break;
                }
            }

            var handler = OnMessage;
            if (handler != null)
            {
                handler(this, new LogMessageEventArgs(message));
            }
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                DoLogInfo("Cancelled.");

            var handler = Completed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                DoLogInfo(e.UserState.ToString());

            var handler = ProgressChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _worker.ReportProgress(0, "Connecting to server...");

            var arg = (DownloadArgWrapper)e.Argument;
            var downloadList = arg.DownloadArg.DownloadList;
            var theme = arg.DownloadArg.DataTheme;

            var downloadFiles = new Dictionary<string, DownloadInfo>();
            var seriesList = new List<Series>();

            var objDownloader = new Downloader
                                    {
                                        ConnectionString = Settings.Instance.DataRepositoryConnectionString
                                    };

            var numTotalSeries = downloadList.Count;
            var numDownloaded = 0;
            var downloadedCount = 1;

            foreach (var di in downloadList)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                string fileName;

                try
                {
                    di.Status = DownloadInfoStatus.Downloading;
                    fileName = objDownloader.DownloadXmlDataValues(di);
                    di.Status = DownloadInfoStatus.Downloaded;
                }
                catch (DownloadXmlException ex)
                {
                    di.Status = DownloadInfoStatus.Error;
                    DoLogError(ex.Message, ex);
                    continue;
                }

                Debug.Assert(File.Exists(fileName)); // we just downloaded this file

                //to ensure there are no duplicate file names:
                if (!downloadFiles.ContainsKey(fileName))
                {
                    downloadFiles.Add(fileName, di);
                    numDownloaded++;

                    var message = "Downloading series " + (downloadedCount++) + " of " + numTotalSeries;
                    var percentProgress = (numDownloaded*100)/numTotalSeries;
                    _worker.ReportProgress(percentProgress, message);
                }
            }

            //In the next step we'll save data values to database
            _worker.ReportProgress(0, "Saving values..");

            var numTotalFiles = downloadFiles.Count;
            var numCurrentFile = 0;
            foreach (var kp in downloadFiles)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var xmlFileName = kp.Key;
                var dInfo = kp.Value;
                IList<Series> seriesLst;

                try
                {
                    seriesLst = objDownloader.DataSeriesFromXml(xmlFileName, dInfo);
                }
                catch (DataSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DoLogError(ex.Message, ex);
                    continue;
                }

                Debug.Assert(seriesLst != null);

                var savedCount = 0;
                foreach (var series in seriesLst)
                {
                    var message = string.Empty;

                    int numSavedValues;
                    try
                    {
                        numSavedValues = objDownloader.SaveDataSeries(series, theme, OverwriteOptions.Copy);
                        savedCount++;
                    }
                    catch (SaveDataSeriesException ex)
                    {
                        DoLogError(ex.Message, ex);
                        numSavedValues = 0;
                    }

                    if (numSavedValues > 0)
                    {
                        seriesList.Add(series);
                        message = "Saving " + (numCurrentFile + 1) + " series out of " + downloadFiles.Count;
                        message += dInfo.SiteName + " - " + dInfo.VariableName +
                                   " " + numSavedValues + " values saved." + "\n";
                    }

                    var percentProgress = (numCurrentFile*100)/numTotalFiles + 1;
                    _worker.ReportProgress(percentProgress, message);
                }

                dInfo.Status = savedCount == seriesLst.Count
                                   ? DownloadInfoStatus.Ok
                                   : DownloadInfoStatus.Error;

                numCurrentFile += 1;
            }
           
            _worker.ReportProgress(0, "Saving Theme");
            _worker.ReportProgress(100, "Download Complete.");

            // Prepare results to send back.
            var results = new TimeSeriesDownloadResults();
            if (theme != null)
            {
                // TODO: What happens if theme is null? No name sent back?
                results.ThemeName = theme.Name;
            }

            e.Result = results;
        }

        #endregion

        #region Nested types

        class DownloadArgWrapper
        {
            public DownloadArg DownloadArg { get; set; }
            public DownloadManagerUI DownloadManagerUI { get; set; }
        }

        enum LogKind
        {
            Info,
            Error
        }

        #endregion
    }

    class LogMessageEventArgs : EventArgs
    {
        public string Message {get;private set;}

        public LogMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
