using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using HydroDesktop.Plugins.DataDownload.Downloading.Exceptions;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.Plugins.DataDownload.Downloading
{
    /// <summary>
    /// Download manager. It downloads and saves data series.
    /// </summary>
    public class DownloadManager
    {
        #region Fields

        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private DownloadManagerUI _downloadManagerUI;
        private static readonly object _syncObjForDownload = new object();
        private readonly List<OneSeriesDownloadInfo> _savedData = new List<OneSeriesDownloadInfo>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of <see cref="DownloadManager"/>
        /// </summary>
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

        /// <summary>
        /// Raised when download progress is changed
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;
        /// <summary>
        /// Raised when downloading is completed
        /// </summary>
        public event EventHandler<RunWorkerCompletedEventArgs> Completed;
        /// <summary>
        /// Raised when downloading is cancelled
        /// </summary>
        public event EventHandler Canceled;
        /// <summary>
        /// Raised when download manager log message 
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnMessage;

        #endregion

        #region Properties

        internal bool IsBusy
        {
            get { return _worker.IsBusy; }
        }

        /// <summary>
        /// Is GUI of download manager is visible?
        /// </summary>
        internal bool IsUIVisible
        {
            get { return _downloadManagerUI != null && _downloadManagerUI.Visible; }
        }
        
        /// <summary>
        /// Information about manager.
        /// </summary>
        internal ManagerInformation Information { get; private set; }

        /// <summary>
        /// Get collection of saved data
        /// </summary>
        /// <returns>Saved series</returns>
        public IEnumerable<OneSeriesDownloadInfo> GetSavedData()
        {
            return _savedData.AsReadOnly();
        }

        #endregion

        #region Public & Internal methods

        /// <summary>
        /// Start downloading with custom indeces of items to be downloaded
        /// </summary>
        /// <param name="indeces">Indeces of items to download</param>
        /// <exception cref="InvalidOperationException">If downloading in progress, you must waiting while it completed.
        /// Also it throws, when no previous downloads found.
        /// </exception>
        internal void SubStart(ICollection<int> indeces = null)
        {
            if (IsBusy)
                throw new InvalidOperationException("Re-downloading can not be started when downloading in progress.");
            if (Information == null)
                throw new InvalidOperationException("No previous downloads found.");
            
            Information.SetSeriesToDownload(indeces);

            var indecesCount = indeces == null
                                   ? Information.StartArgs.ItemsToDownload.Count
                                   : indeces.Count;

            DoLogInfo(string.Format("Re-download series ({0} of {1}) started...", indecesCount, Information.StartArgs.ItemsToDownload.Count));
            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// Start downloading
        /// </summary>
        /// <param name="startDownloadArg">Download args</param>
        /// <exception cref="ArgumentNullException">args must be not null.</exception>
        /// <exception cref="InvalidOperationException">Throws if previous download commnad is stiil active.</exception>
        internal void Start(StartDownloadArg startDownloadArg)
        {
            if (startDownloadArg == null)
                throw new ArgumentNullException("startDownloadArg");
            if (IsBusy)
                throw new InvalidOperationException("The previous download command is still active.");
            
            Information = new ManagerInformation(startDownloadArg, this);
            _downloadManagerUI = new DownloadManagerUI(this);
            ShowUI();

            DoLogInfo("Starting downloading...");
            _worker.RunWorkerAsync();
          
        }

        /// <summary>
        /// Cancel downloading.
        /// </summary>
        internal void Cancel()
        {
            DoLogInfo("Cancelling...");
            _worker.CancelAsync();

            var handler = Canceled;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Show UI of download manager.
        /// </summary>
        internal void ShowUI()
        {
            if (_downloadManagerUI == null) return;
            _downloadManagerUI.Show();
        }

        /// <summary>
        /// Hide UI of download manager.
        /// </summary>
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

        private void DoLogError(string message, Exception exception, OneSeriesDownloadInfo di)
        {
            if (di != null)
                message = string.Format("Error in {0}:" + Environment.NewLine + message, di.SeriesDescription);
            else
                message = "Error: " + message;

            DoLog(LogKind.Error, message, exception);
        }

        private void DoLogWarn(string message)
        {
            message = "Warning: " + message;
            DoLog(LogKind.Warn, message);
        }

        private void DoLog(LogKind logKind, string message, Exception exception = null)
        {
            var handler = OnMessage;
            if (handler != null)
            {
                handler(this, new LogMessageEventArgs(message, logKind, exception));
            }
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RaiseCompletedEvent(e);
        }

        private void RaiseCompletedEvent(RunWorkerCompletedEventArgs e)
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

            _savedData.Clear();
            _savedData.Capacity = Information.StartArgs.ItemsToDownload.Count;

            var downloadList = Information.StartArgs.ItemsToDownload;
            var indeces = Information.IndecesToDownload;

            // max count of downloading threads
            int maxThreadsToDownloadCount = 16;
            if (Information.StartArgs.DownloadOptions.UseSingleThread)
            {
                maxThreadsToDownloadCount = 1;
            }

            var commonInfo = new CommnonDoDownloadInfo(new Downloader(Information.StartArgs.DownloadOptions)); // common info, shared through downloading threads

            // Starting (if possible) maxThreadsToDownloadCount downloading threads 
            for (int i = 0, startedThreadsCount = 0; startedThreadsCount < maxThreadsToDownloadCount &&
                                                     i < indeces.Count; i++)
            {
                var thread = new Thread(DoDownload);
                var dda = new DoDownloadArg(downloadList[indeces[i]], commonInfo, thread);
                commonInfo.AddDonwloadingThread(thread);
                commonInfo.LastDownloadingIndex++;
                thread.Start(dda);
                startedThreadsCount++;
            }

            // Starting save thread
            var saveThread = new Thread(DoSave);
            saveThread.Start(commonInfo);

            // Blocking current thread until all the series will not saved, or downloading cancelled
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
            foreach (var ind in indeces)
            {
                var di = downloadList[ind];
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

            var di = dda.DownloadInfo;
            var objDownloader = dda.CommnonInfo.Downloader;
          
            var hasException = true;
            try
            {
                if (_worker.CancellationPending) return;

                di.Status = DownloadInfoStatus.Downloading;
                var progressHandler = new GetValueProgressHandler(this, di);
                di.FilesWithData = objDownloader.DownloadXmlDataValues(di, progressHandler);
                if (_worker.CancellationPending) return;
                di.Status = DownloadInfoStatus.Downloaded;
                lock (_syncObjForDownload)
                {
                    dda.CommnonInfo.Downloaded.Enqueue(di);
                    Information.Downloaded++;
                }

                hasException = false;

                var message = string.Format("Downloaded {0} ({1} of {2}).", di.SeriesDescription,
                                            Information.Downloaded,
                                            Information.TotalSeries);
                DoLogInfo(message);
            }
            catch (DownloadXmlException ex)
            {
                di.Status = DownloadInfoStatus.Error;
                di.ErrorMessage = ex.Message;
                DoLogError(ex.Message, ex, di);
                Information.WithError++;
            }
            finally
            {
                //Update estimated times
                Information.AddTimeTaken(di.DownloadTimeTaken);
                Information.RefreshEstimatedTimeForDownload(); 

                // common progress
                if (hasException)
                {
                    ProgressSeries();
                }

                // resume saving thread
                dda.CommnonInfo.SaveManualResetEvent.Set();

                // start new thread if need
                if (!_worker.CancellationPending &&
                    dda.CommnonInfo.LastDownloadingIndex < Information.TotalSeries)
                {
                    var thread = new Thread(DoDownload);
                    dda.CommnonInfo.AddDonwloadingThread(thread);
                    var ld = dda.CommnonInfo.LastDownloadingIndex;
                    dda.CommnonInfo.LastDownloadingIndex++;
                    thread.Start(new DoDownloadArg(Information.StartArgs.ItemsToDownload[Information.IndecesToDownload[ld]],
                                                   dda.CommnonInfo, thread));

                }

                dda.CommnonInfo.RemoveDonwloadingThread(dda.DownloadThread);
                if (dda.CommnonInfo.DownloadingThreadsCount == 0)
                {
                    Information.RefreshEstimatedTimeForDownload();
                }
            }
        }

        private void ProgressSeries()
        {
            _worker.ReportProgress((int)Information.GetTotalProgress(), "Downloading...");
        }

        private void DoSave(object state)
        {
            var commonInfo = state as CommnonDoDownloadInfo;
            if (commonInfo == null) throw new InvalidOperationException();

            var objDownloader = commonInfo.Downloader;
           
            while (Information.RemainingSeries > 0)
            {
                if (_worker.CancellationPending) break;

                if (commonInfo.Downloaded.Count == 0)
                {
                    commonInfo.SaveManualResetEvent.WaitOne();
                    continue;
                }

                // Common progress
                ProgressSeries();

                OneSeriesDownloadInfo dInfo;
                lock (_syncObjForDownload)
                {
                    dInfo = commonInfo.Downloaded.Dequeue();
                }
                Debug.Assert(dInfo != null);
                
                IList<Series> series;
                var startTime = DateTime.Now;

                // Parsing series from xml
                try
                {
                    series = objDownloader.DataSeriesFromXml(dInfo).ToList();
                }
                catch (DataSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    Information.WithError++;
                    DoLogError(ex.Message, ex, dInfo);
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }
                catch (NoSeriesFromXmlException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    Information.WithError++;
                    DoLogError(ex.Message, null, dInfo); // No stack trace
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }

                // Save series to database
                int numSavedValues;
                try
                {
                    numSavedValues = objDownloader.SaveDataSeries(series, Information.StartArgs.DataTheme, dInfo.OverwriteOption);
                    Information.DownloadedAndSaved++;

                    dInfo.ResultSeries = series;
                    _savedData.Add(dInfo);

                    if (numSavedValues == 0)
                    {
                        DoLogWarn(string.Format("Saved 0 values for {0}", dInfo.SeriesDescription));
                        dInfo.Status = DownloadInfoStatus.OkWithWarning;
                    }
                    else
                        dInfo.Status = DownloadInfoStatus.Ok;
                }
                catch (SaveDataSeriesException ex)
                {
                    dInfo.Status = DownloadInfoStatus.Error;
                    DoLogError(ex.Message, ex, dInfo);
                    Information.WithError++;
                    dInfo.ErrorMessage = ex.Message;
                    continue;
                }
                finally
                {
                    // Calculate estimated time
                    var endTime = DateTime.Now;
                    var diff = endTime.Subtract(startTime).TotalSeconds;
                    var interval = diff * Information.RemainingSeries + 1;
                    Information.EstimatedTimeForSave = interval < Int32.MaxValue
                                                             ? new TimeSpan(0, 0, (int)(interval))
                                                             : TimeSpan.MaxValue;
                }

                var message = string.Format("Saved {0}. Values: {1}.", dInfo.SeriesDescription, numSavedValues);
                DoLogInfo(message);
            }

            ProgressSeries();

            Information.EstimatedTimeForSave = new TimeSpan();
            commonInfo.SavingWaitingEvent.Set();
        }

        #endregion

        #region Nested types

        class DoDownloadArg
        {
            public OneSeriesDownloadInfo DownloadInfo { get; private set; }
            public CommnonDoDownloadInfo CommnonInfo { get; private set; }
            public Thread DownloadThread { get; private set; }

            public DoDownloadArg(OneSeriesDownloadInfo di, CommnonDoDownloadInfo commonInfo, Thread downloadThread)
            {
                DownloadInfo = di;
                CommnonInfo = commonInfo;
                DownloadThread = downloadThread;
            }
        }
        class CommnonDoDownloadInfo
        {
            private readonly ManualResetEvent _manualResetEvent;

            public CommnonDoDownloadInfo(Downloader downloader)
            {
                _manualResetEvent = new ManualResetEvent(false);
                Downloader = downloader;
            }

            public Downloader Downloader { get; private set; }

            private readonly Queue<OneSeriesDownloadInfo> _downloaded = new Queue<OneSeriesDownloadInfo>();

            public ManualResetEvent SaveManualResetEvent
            {
                get { return _manualResetEvent; }
            }

            public Queue<OneSeriesDownloadInfo> Downloaded
            {
                get { return _downloaded; }
            }

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

        private class GetValueProgressHandler : IGetValuesProgressHandler
        {
            private readonly DownloadManager _manager;
            private readonly OneSeriesDownloadInfo _di;
            private double _totalTime;

            public GetValueProgressHandler(DownloadManager manager, OneSeriesDownloadInfo di)
            {
                _manager = manager;
                _di = di;
            }

            public void Progress(int intervalNumber, int totalIntervalsCount, double timeTaken)
            {
                _totalTime += timeTaken;
                _di.EstimatedTimeToDownload = timeTaken*(totalIntervalsCount - (intervalNumber + 1)) + 1;
                _di.DownloadedChunksPercent = (intervalNumber + 1)*1.0/totalIntervalsCount*100.0;
                _manager.Information.RefreshEstimatedTimeForDownload();

                if (intervalNumber == totalIntervalsCount - 1)
                {
                    _di.DownloadTimeTaken = new TimeSpan(0, 0, (int) _totalTime);
                }

                _manager.ProgressSeries();
            }

            public bool CancellationPending
            {
                get { return _manager._worker.CancellationPending; }
            }
        }

        #endregion
    }

    /// <summary>
    /// Event arguments for <see cref="DownloadManager.OnMessage"/> event
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message {get;private set;}

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Log Kind
        /// </summary>
        public LogKind LogKind { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="LogMessageEventArgs"/>
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="logKind"></param>
        public LogMessageEventArgs(string message, LogKind logKind, Exception exception = null)
        {
            Message = message;
            Exception = exception;
            LogKind = logKind;
        }
    }


    public enum LogKind
    {
        Info,
        Error,
        Warn
    }
}
