using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Search.Download.Exceptions;

namespace HydroDesktop.Search.Download
{
    class DownloadManager
    {
        #region Fields

        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private DownloadManagerUI _downloadManagerUI;
        private static readonly object _syncObjForDownload = new object();

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
            _worker.RunWorkerAsync(new DownloadArgWrapper { DownloadArg = args});

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

            var objDownloader = new Downloader();
            var numTotalSeries = downloadList.Count;
            
            const int maxThreadsCount = 4;
            var waitReset = new ManualResetEvent(false);
            var commonInfo = new CommnonDoDownloadInfo(numTotalSeries, downloadList, waitReset, objDownloader);

            for(int i = 0; i<maxThreadsCount && i <downloadList.Count; i++)
            {
                var thread = new Thread(DoDownload);
                var dda = new DoDownloadArg(downloadList[i], commonInfo, thread);
                commonInfo.AddDonwloadingThread(thread);
                commonInfo.LastDownloadingIndex++;
                thread.Start(dda);
            }

            var saveThread = new Thread(DoSave);
            saveThread.Start(commonInfo);

            commonInfo.SavingWaitingEvent = new ManualResetEvent(false);
            commonInfo.SavingWaitingEvent.WaitOne();

            if (_worker.CancellationPending)
            {
                e.Cancel = true;
            }

            // waiting all downloading threads
            while (commonInfo.DownloadingThreadsCount > 0) { }

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

            _worker.ReportProgress(100, e.Cancel? "Download cancelled." : "Download Complete.");
        } 

        private void DoDownload(object state)
        {
            var dda = state as DoDownloadArg;
            if (dda == null) throw new InvalidOperationException();
            if (_worker.CancellationPending) goto finish;

            var di = dda.DownloadInfo;
            var objDownloader = dda.CommnonInfo.Downloader;
          
            var startTime = DateTime.Now;
            try
            {
                di.Status = DownloadInfoStatus.Downloading;
                di.FileName = objDownloader.DownloadXmlDataValues(di);
                di.Status = DownloadInfoStatus.Downloaded;
                DownloadProgressInfo.Downloaded++;
            }
            catch (DownloadXmlException ex)
            {
                di.Status = DownloadInfoStatus.Error;
                di.ErrorMessage = ex.Message;
                DoLogError(ex.Message, ex);
                DownloadProgressInfo.WithError++;
                goto finish;
            }
            finally
            {
                // Calculcate estimated time
                var endTime = DateTime.Now;
                var diff = endTime.Subtract(startTime).TotalSeconds;
                di.DownloadTimeTaken = TimeSpan.FromSeconds(diff);
                var interval = diff*
                               (DownloadProgressInfo.TotalSeries -
                                (DownloadProgressInfo.WithError + DownloadProgressInfo.Downloaded))/ + 1;
                DownloadProgressInfo.EstimatedTimeForDownload = interval < Int32.MaxValue
                                                                    ? new TimeSpan(0, 0, (int) interval)
                                                                    : TimeSpan.MaxValue;
            }

            Debug.Assert(File.Exists(di.FileName)); // we just downloaded this file

            lock (_syncObjForDownload)
            {
                dda.CommnonInfo.Downloaded.Enqueue(di);
                dda.CommnonInfo.DownloadedCount++;
            }
            var message = "Downloaded series " + dda.CommnonInfo.DownloadedCount + " of " + dda.CommnonInfo.NumTotalSeries;
            var percentProgress = (dda.CommnonInfo.DownloadedCount * 100) / dda.CommnonInfo.NumTotalSeries;
            _worker.ReportProgress(percentProgress, message);

finish:
            dda.CommnonInfo.SaveManualResetEvent.Set();

            // start new thread if need
            if (dda.CommnonInfo.LastDownloadingIndex < dda.CommnonInfo.NumTotalSeries)
            {
                var thread = new Thread(DoDownload);
                dda.CommnonInfo.AddDonwloadingThread(thread);
                var ld = dda.CommnonInfo.LastDownloadingIndex;
                dda.CommnonInfo.LastDownloadingIndex++;
                thread.Start(new DoDownloadArg(dda.CommnonInfo.DownloadList[ld],
                                               dda.CommnonInfo, thread));
                
            }

            dda.CommnonInfo.RemoveDonwloadingThread(dda.DownloadThread);
            if (dda.CommnonInfo.DownloadingThreadsCount == 0)
            {
                DownloadProgressInfo.EstimatedTimeForDownload = new TimeSpan();
            }
        }

        private void DoSave(object state)
        {
            var commonInfo = state as CommnonDoDownloadInfo;
            if (commonInfo == null) throw new InvalidOperationException();

            var objDownloader = commonInfo.Downloader;

            while (DownloadProgressInfo.RemainingSeries > 0)
            {
                if (_worker.CancellationPending) break;

                if (commonInfo.Downloaded.Count == 0)
                {
                    commonInfo.SaveManualResetEvent.WaitOne();
                    continue;
                }

                DownloadInfo dInfo;
                lock (_syncObjForDownload)
                {
                    dInfo = commonInfo.Downloaded.Dequeue();
                }
                Debug.Assert(dInfo != null);
                
                Series series;
                var startTime = DateTime.Now;

                // Parsing series from xml
                try
                {
                    series = objDownloader.DataSeriesFromXml(dInfo);
                }
                catch (DataSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DownloadProgressInfo.WithError++;
                    DoLogError(ex.Message, ex);
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }
                catch (NoSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DownloadProgressInfo.WithError++;
                    DoLogError(ex.Message); // No stack trace
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }
                catch (TooMuchSeriesFromXmlException ex)
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
                    numSavedValues = objDownloader.SaveDataSeries(series, CurrentDownloadArg.DataTheme);
                    DownloadProgressInfo.DownloadedAndSaved++;

                    if (numSavedValues == 0)
                    {
                        DoLogInfo(string.Format("Warning: {0} has no data values!", series));
                        dInfo.Status = DownloadInfoStatus.OkWithWarnings;
                    }
                    else
                        dInfo.Status = DownloadInfoStatus.Ok;
                }
                catch (SaveDataSeriesException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DoLogError(ex.Message, ex);
                    DownloadProgressInfo.WithError++;
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }
                finally
                {
                    // Calculcate estimated time
                    var endTime = DateTime.Now;
                    var diff = endTime.Subtract(startTime).TotalSeconds;
                    var interval = diff * DownloadProgressInfo.RemainingSeries + 1;
                    DownloadProgressInfo.EstimatedTimeForSave = interval < Int32.MaxValue
                                                             ? new TimeSpan(0, 0, (int)(interval))
                                                             : TimeSpan.MaxValue;
                }

                var message = dInfo.SiteName + " - " + dInfo.VariableName + " " + numSavedValues + " values saved.";
                DoLogInfo(message);
            }

            DownloadProgressInfo.EstimatedTimeForSave = new TimeSpan();
            commonInfo.SavingWaitingEvent.Set();
        }

        #endregion

        #region Nested types

        class DoDownloadArg
        {
            public DownloadInfo DownloadInfo { get; private set; }
            public CommnonDoDownloadInfo CommnonInfo { get; private set; }
            public Thread DownloadThread { get; private set; }

            public DoDownloadArg(DownloadInfo di, CommnonDoDownloadInfo commonInfo, Thread downloadThread)
            {
                DownloadInfo = di;
                CommnonInfo = commonInfo;
                DownloadThread = downloadThread;
            }
        }
        class CommnonDoDownloadInfo
        {
            private readonly ManualResetEvent _manualResetEvent;

            public CommnonDoDownloadInfo(int numTotalSeries, ReadOnlyCollection<DownloadInfo> downloadList, 
                ManualResetEvent manualResetEvent,  Downloader downloader)
            {
                _manualResetEvent = manualResetEvent;
                NumTotalSeries = numTotalSeries;
                DownloadList = downloadList;
                Downloader = downloader;
            }

            public Downloader Downloader { get; private set; }

            private readonly Queue<DownloadInfo> _downloaded = new Queue<DownloadInfo>();

            public ManualResetEvent SaveManualResetEvent
            {
                get { return _manualResetEvent; }
            }

            public Queue<DownloadInfo> Downloaded
            {
                get { return _downloaded; }
            }

            public int NumTotalSeries { get; private set; }
            public ReadOnlyCollection<DownloadInfo> DownloadList { get; private set; }
            public int DownloadedCount { get; set; }

            private volatile int _lastDownloadingIndex;
            public int LastDownloadingIndex
            {
                get { return _lastDownloadingIndex; }
                set
                {
                    _lastDownloadingIndex = value;
                }
            }

            private static readonly object _downloadingThreadsSyncObject = new object();
            private readonly List<Thread> _downloadingThreads = new List<Thread>();
            public void AddDonwloadingThread(Thread thread)
            {
                lock (_downloadingThreadsSyncObject)
                {
                    _downloadingThreads.Add(thread);
                }
            }
            public void RemoveDonwloadingThread(Thread thread)
            {
                lock (_downloadingThreadsSyncObject)
                {
                    _downloadingThreads.Remove(thread);
                }
            }
            public int DownloadingThreadsCount {get { return _downloadingThreads.Count; }}


            public ManualResetEvent SavingWaitingEvent { get; set; }
        }

        class DownloadArgWrapper
        {
            public DownloadArg DownloadArg { get; set; }
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
