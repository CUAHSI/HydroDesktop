using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HydroDesktop.Common;
using Search3.Keywords;
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
        /// <exception cref="SearchSettingsValidationException">Throws if settings to search has any problems.</exception>
        public void Run(SearchSettings settings)
        {
            if (settings == null) 
                throw new ArgumentNullException("settings");
            if (IsBusy)
            {
                ShowUI();
                throw new InvalidOperationException("The previous search command is still active.");
            }
            CheckSettingsForErrors(settings);

            _searcherUI = new SearchProgressForm(this);
            ShowUI();
            LogMessage("Search started.");
            InternalStartSearching(settings);
        }

        /// <summary>
        /// Cancel searching.
        /// </summary>
        public void Cancel()
        {
            if (_cancellationTokenSource == null ||
                !_cancellationTokenSource.Token.CanBeCanceled) return;

            LogMessage("Cancelling...");
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Shows that searhing is active.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _searchTask != null && !_searchTask.IsCompleted;
            }
        }

        public bool IsUIVisible
        {
            get
            {
                if (_searcherUI == null) return false;
                return _searcherUI.Visible;
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
            _searchTask = Task.Factory.StartNew<SearchResult>(DoSearch, settings, _cancellationTokenSource.Token);
            _searchTask.ContinueWith(OnFinishedTask, new CancellationTokenSource().Token, TaskContinuationOptions.None,
                                     TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LogAggregateException(Exception ex)
        {
            var aggr = ex as AggregateException;
            if (aggr != null)
            {
                LogMessage("AggregateException Error: " + aggr.Message);
                foreach (var innerException in aggr.InnerExceptions)
                {
                    LogAggregateException(innerException);
                }
            }
            else
            {
                LogMessage("Error:", ex);       
            }
        }

        private void OnFinishedTask(Task<SearchResult> task)
        {
            if (task == null) return;
            SearchResult result = null;
            CompletedReasones reason;
            if (task.IsFaulted)
            {
                reason = CompletedReasones.Faulted;
                if (task.Exception != null)
                {
                    foreach (var error in task.Exception.InnerExceptions)
                    {
                        LogAggregateException(error);
                    }
                }
                else
                {
                    LogMessage("Unknow error.");
                }
            }
            else if (task.IsCanceled)
            {
                reason = CompletedReasones.Cancelled;
            }
            else
            {
                try
                {
                    result = task.Result;
                    reason = CompletedReasones.NormalCompleted;
                }
                catch (AggregateException aex)
                {
                    reason = CompletedReasones.Faulted;
                    foreach (var error in aex.InnerExceptions)
                    {
                        LogMessage("Error:", error);
                    }
                }
            }

            var completeArgs = new CompletedEventArgs(result, reason, new ProgressHandler(this));
            RaiseCompleted(completeArgs);
            _searcherUI.DoSearchFinished(completeArgs);
        }

        private SearchResult DoSearch(object state)
        {
            var settings = (SearchSettings) state;
            var progressHandler = new ProgressHandler(this);

            SeriesSearcher searcher;
            double tileWidth, tileHeight;
            if (settings.CatalogSettings.TypeOfCatalog == TypeOfCatalog.HisCentral)
            {
                searcher = new HISCentralSearcher(settings.CatalogSettings.HISCentralUrl);
                tileWidth = 1.0;
                tileHeight = 1.0;
            }
            else
            {
                searcher = new MetadataCacheSearcher();
                tileWidth = 2.0;
                tileHeight = 2.0;
            }

            SearchResult result;
            
            var webServices = settings.WebServicesSettings.TotalCount == settings.WebServicesSettings.CheckedCount &&
                              settings.WebServicesSettings.TotalCount > 1
                                  ? new WebServiceNode[] {}
                                  : settings.WebServicesSettings.WebServices.Where(item => item.Checked).ToArray();

            var keywords = settings.KeywordsSettings.SelectedKeywords.ToList();

            if (settings.CatalogSettings.TypeOfCatalog == TypeOfCatalog.HisCentral)
            {
                //todo: do we need to do this?
                var ontologyXml = HdSearchOntologyHelper.ReadOntologyXmlFile();
                HdSearchOntologyHelper.RefineKeywordList(keywords, ontologyXml);
            }
            else
            {
                //in the special case of metadata cache - hydrosphere keyword
                if (keywords.Contains("Hydrosphere"))
                {
                    keywords.Clear();
                }
            }

            if (settings.AreaSettings.AreaRectangle != null)
            {
                var box = Area.AreaHelper.ReprojectBoxToWGS84(settings.AreaSettings.AreaRectangle,
                                                              settings.AreaSettings.RectangleProjection);

                result = searcher.GetSeriesCatalogInRectangle(box, keywords.ToArray(), tileWidth, tileHeight,
                                                              settings.DateSettings.StartDate,
                                                              settings.DateSettings.EndDate,
                                                              webServices, progressHandler);
            }
            else
            {
                var polygons = Area.AreaHelper.ReprojectPolygonsToWGS84(settings.AreaSettings.Polygons);

                result = searcher.GetSeriesCatalogInPolygon(polygons, keywords.ToArray(), tileWidth, tileHeight,
                                                            settings.DateSettings.StartDate,
                                                            settings.DateSettings.EndDate,
                                                            webServices, progressHandler);
            }
            return result;
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

        private static void CheckSettingsForErrors(SearchSettings settings)
        {
            var selectedKeywords = settings.KeywordsSettings.SelectedKeywords.ToList();
            if (selectedKeywords.Count == 0)
                throw new SearchSettingsValidationException("Please provide at least one Keyword for search.");

            var webServicesCount = settings.WebServicesSettings.CheckedCount;
            if (webServicesCount == 0)
                throw new SearchSettingsValidationException("Please provide at least one Web Service for search.");

            if (!settings.AreaSettings.HasAnyArea)
                throw new SearchSettingsValidationException("Please provide at least one Target Area for search.");
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

            public void ReportMessage(string message)
            {
                _parent.LogMessage(message);
            }

            public CancellationToken CancellationToken
            {
                get { return _parent._cancellationTokenSource.Token; }
            }
        }
    }
}
