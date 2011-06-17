using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using HydroDesktop.Configuration;
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

        internal DownloadArg CurrentDownloadArg { get; private set; }
        internal DownloadProgressInfo DownloadProgressInfo { get; private set; }

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
            DownloadProgressInfo = new DownloadProgressInfo {TotalSeries = CurrentDownloadArg.DownloadList.Count};
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
                handler(this, new LogMessageEventArgs(message, exception));
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
            {
                var mes = e.UserState.ToString();
                if (!string.IsNullOrEmpty(mes))
                    DoLogInfo(mes);
            }

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

            var downloadedFiles = new Dictionary<string, DownloadInfo>();

            var objDownloader = new Downloader();
            var numTotalSeries = downloadList.Count;
            var numDownloaded = 0;
            var downloadedCount = 0;

            #region Downloading
            
            foreach (var di in downloadList)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    goto finalLogging;
                }

                string fileName;
                var startTime = DateTime.Now;
                try
                {
                    di.Status = DownloadInfoStatus.Downloading;
                    fileName = objDownloader.DownloadXmlDataValues(di);
                    di.Status = DownloadInfoStatus.Downloaded;
                    DownloadProgressInfo.Downloaded++;
                }
                catch (DownloadXmlException ex)
                {
                    di.Status = DownloadInfoStatus.Error;
                    di.ErrorMessage = ex.Message;
                    DoLogError(ex.Message, ex);
                    DownloadProgressInfo.WithError++;
                    continue;
                }
                finally
                {
                    // Calculcate estimated time
                    var endTime = DateTime.Now;
                    var diff = endTime.Subtract(startTime).TotalSeconds;
                    di.DownloadTimeTaken = TimeSpan.FromSeconds(diff);
                    var interval = diff*
                                   (DownloadProgressInfo.TotalSeries -
                                    (DownloadProgressInfo.WithError + DownloadProgressInfo.Downloaded))/
                                   DownloadProgressInfo.Downloaded + 1;
                    DownloadProgressInfo.EstimatedTimeForDownload = interval < Int32.MaxValue
                                                             ? new TimeSpan(0, 0, (int) interval)
                                                             : TimeSpan.MaxValue;
                }

                Debug.Assert(File.Exists(fileName)); // we just downloaded this file

                //to ensure there are no duplicate file names:
                if (!downloadedFiles.ContainsKey(fileName))
                {
                    downloadedFiles.Add(fileName, di);
                    numDownloaded++;

                    downloadedCount++;
                    var message = "Downloading series " + downloadedCount + " of " + numTotalSeries;
                    var percentProgress = (numDownloaded*100)/numTotalSeries;
                    _worker.ReportProgress(percentProgress, message);
                }
            }

            DownloadProgressInfo.EstimatedTimeForDownload = new TimeSpan();

            #endregion

            //In the next step we'll save data values to database
            _worker.ReportProgress(0, "Saving values..");

            #region Saving

            var numTotalFiles = downloadedFiles.Count;
            var numCurrentFile = 0;
            foreach (var kp in downloadedFiles)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    goto finalLogging;
                }
                
                var dInfo = kp.Value;
                Series series;
                var startTime = DateTime.Now;

                // Parsing series from xml
                try
                {
                    series = objDownloader.DataSeriesFromXml(kp.Key, dInfo);
                }
                catch (DataSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DownloadProgressInfo.WithError++;
                    DoLogError(ex.Message, ex);
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }catch(NoSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DownloadProgressInfo.WithError++;
                    DoLogError(ex.Message); // No stack trace
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }
                catch(TooMuchSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DownloadProgressInfo.WithError++;
                    DoLogError(ex.Message); // No stack trace
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }
                Debug.Assert(series != null);
                
                // Save series to database
                int numSavedValues;
                try
                {
                    numSavedValues = objDownloader.SaveDataSeries(series, theme);
                    DownloadProgressInfo.DownloadedAndSaved++;
                    
                    if (numSavedValues == 0)
                    {
                        DoLogInfo(string.Format("Warning: {0} has no data values!", series));
                        dInfo.Status = DownloadInfoStatus.OkWithWarnings;
                    }else
                        dInfo.Status = DownloadInfoStatus.Ok;
                }
                catch (SaveDataSeriesException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DoLogError(ex.Message, ex);
                    DownloadProgressInfo.WithError++;
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }finally
                {
                    // Calculcate estimated time
                    var endTime = DateTime.Now;
                    var diff = endTime.Subtract(startTime).TotalSeconds;
                    var interval = diff * DownloadProgressInfo.RemainingSeries / DownloadProgressInfo.DownloadedAndSaved + 1;
                    DownloadProgressInfo.EstimatedTimeForSave = interval < Int32.MaxValue
                                                             ? new TimeSpan(0, 0, (int)(interval))
                                                             : TimeSpan.MaxValue;
                }

                var message = "Saving " + (numCurrentFile + 1) + " series out of " + downloadedFiles.Count;
                message += dInfo.SiteName + " - " + dInfo.VariableName +
                               " " + numSavedValues + " values saved." + "\n";

                var percentProgress = (numCurrentFile * 100) / numTotalFiles + 1;
                _worker.ReportProgress(percentProgress, message);
                numCurrentFile++;
            }

            DownloadProgressInfo.EstimatedTimeForSave = new TimeSpan();

            #endregion

finalLogging:

            #region Final logging

            var sb = new StringBuilder(Environment.NewLine);
            sb.AppendLine("===============" );
            sb.AppendLine("Total:");
            sb.AppendLine("ServiceURL SiteCode VariableCode StartDate EndDate DownloadTime Status ErrorMessage");
            foreach (var di in downloadList)
            {
                sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7}" + Environment.NewLine, di.Wsdl, di.FullSiteCode, di.FullVariableCode, di.StartDate,
                                di.EndDate, di.DownloadTimeTaken, di.Status, di.ErrorMessage);
            }
            sb.AppendLine("===============");
            DoLogInfo(sb.ToString());
               
            #endregion

            _worker.ReportProgress(100, "Download Complete.");
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
        public Exception Exception { get; private set; }

        public LogMessageEventArgs(string message, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }
    }
}
