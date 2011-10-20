using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Search3.Searching.Exceptions;
using Search3.Settings;

namespace Search3.Searching
{
    /// <summary>
    /// Data series searcher.
    /// </summary>
    public class Searcher
    {
        #region Fields

        private SearchProgressForm _searcherUI;
        private Task<SearchResult> _searchTask;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _monitorTask;

        #endregion

        #region Events

        /// <summary>
        /// Raised when progress changed.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;
        /// <summary>
        /// Raised when Searcher sends any message.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnMessage;
        /// <summary>
        /// Raised whane search is completed.
        /// </summary>
        public event EventHandler<CompletedEventArgs> Completed;

        #endregion

        #region Public methods

        /// <summary>
        /// Run search.
        /// </summary>
        /// <param name="settings">Settings to start search.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="settings"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Throws if previous search command is still active.</exception>
        /// <exception cref="SearchSettingsException">Throws if settings to search has any problems.</exception>
        public void Run(SearchSettings settings)
        {
            if (settings == null) 
                throw new ArgumentNullException("settings");
            if (IsBusy)
                throw new InvalidOperationException("The previous search command is still active.");
            CheckSettingsForErrors(settings);

            _searcherUI = new SearchProgressForm(this);
            ShowUI();
            InternalStartSearching(settings);
        }

        /// <summary>
        /// Cancel searching.
        /// </summary>
        public void Cancel()
        {
            if (_cancellationTokenSource == null ||
                !_cancellationTokenSource.Token.CanBeCanceled) return;

            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Shows that searhing is active.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _searchTask != null && _monitorTask != null &&
                       (!_searchTask.IsCompleted || !_monitorTask.IsCompleted);
            }
        }

        /// <summary>
        /// Show GUI for searching.
        /// </summary>
        public void ShowUI()
        {
            if (_searcherUI == null) return;
            _searcherUI.Show();
        }


        /// <summary>
        /// Hide GUI for searching.
        /// </summary>
        public void HideUI()
        {
            if (_searcherUI == null) return;
            _searcherUI.Hide();
        }

        #endregion

        #region Private methods

        private void InternalStartSearching(SearchSettings settings)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var _cancellationToken = _cancellationTokenSource.Token;
            _searchTask = Task.Factory.StartNew<SearchResult>(DoSearch, settings, _cancellationToken);
            _monitorTask = Task.Factory.StartNew(DoMonitorTask);
        }

        private void DoMonitorTask()
        {
            while (true)
            {
                if (_searchTask == null) break;

                if (!_searchTask.IsCompleted)
                {
                    Thread.Sleep(500);
                    continue;
                }


                SearchResult result = null;
                if (_searchTask.IsFaulted)
                {
                    if (_searchTask.Exception != null)
                    {
                        foreach (var error in _searchTask.Exception.InnerExceptions)
                        {
                            LogMessage("Error", error);
                        }
                    }
                    else
                    {
                        LogMessage("Unknow error");
                    }
                }
                else if (_searchTask.IsCanceled)
                {
                    LogMessage("Cancelled");
                }else
                {
                    try
                    {
                        result = _searchTask.Result;
                    }
                    catch (AggregateException aex)
                    {
                        foreach (var error in aex.InnerExceptions)
                        {
                            LogMessage("Error", error);
                        }
                    }
                }

                RaiseCompleted(new CompletedEventArgs(result));
                break;
            }
        }

        private SearchResult DoSearch(object state)
        {
            LogMessage("Search started.");

            var settings = (SearchSettings) state;
            var parameters = Search2Helper.GetSearchParameters(settings);
            var searcher = new BackgroundSearchWithFailover();
            var e = new DoWorkEventArgs(parameters);
            var progressHandler = new ProgressHandler(this);
            if (parameters.SearchMethod == TypeOfCatalog.HisCentral)
            {
                searcher.HISCentralSearchWithFailover(e, HydroDesktop.Configuration.Settings.Instance.HISCentralURLList, progressHandler);
            }
            else
            {
                searcher.RunMetadataCacheSearch(e, progressHandler);
            }

            LogMessage("Search finished successfully.");

            return (SearchResult)e.Result;
        }

        private void RaiseCompleted(CompletedEventArgs eventArgs)
        {
            var handler = Completed;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        private void LogMessage(string message, Exception exception = null)
        {
            var handler = OnMessage;
            if (handler != null)
            {
                handler(this, new LogMessageEventArgs(message, exception));
            }
        }

        private void Progress(int progressPercentage, string message)
        {
            var progressHandler = ProgressChanged;
            if (progressHandler != null)
            {
                progressHandler(this, new ProgressChangedEventArgs(progressPercentage, message));
            }
        }

        private void CheckSettingsForErrors(SearchSettings settings)
        {
            var selectedKeywords = settings.KeywordsSettings.SelectedKeywords.ToList();
            if (selectedKeywords.Count == 0)
                throw new NoSelectedKeywordsException();

            var webServicesCount = settings.WebServicesSettings.CheckedCount;
            if (webServicesCount == 0)
                throw new NoWebServicesException();

            if (!settings.AreaSettings.HasAnyArea)
                throw new NoAreaToSearchException();
        }

        #endregion

        private class ProgressHandler : IProgressHandler
        {
            private readonly Searcher _parent;

            public ProgressHandler(Searcher parent)
            {
                if (parent == null) throw new ArgumentNullException("parent");
                _parent = parent;
            }

            public void ReportProgress(int persentage, object state)
            {
                _parent.Progress(persentage, state.ToString());
            }

            public void CheckForCancel()
            {
                _parent._cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }
    }
}
