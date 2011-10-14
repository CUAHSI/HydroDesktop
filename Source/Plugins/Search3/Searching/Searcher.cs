using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Search3.Searching.Exceptions;
using Search3.Settings;

namespace Search3.Searching
{
    public class Searcher
    {
        #region Fields

        private SearchProgressForm _searcherUI;
        private Task _searchTask;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler<LogMessageEventArgs> OnMessage;

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
            InternalStartSearching();
        }

        public void Cancel()
        {
            if (_cancellationTokenSource == null ||
                !_cancellationTokenSource.Token.CanBeCanceled) return;

            _cancellationTokenSource.Cancel();
        }

        public bool IsBusy
        {
            get
            {
                return _searchTask != null && !_searchTask.IsCompleted;
            }
        }

        public void ShowUI()
        {
            if (_searcherUI == null) return;
            _searcherUI.Show();
        }

        #endregion

        #region Private methods

        private void InternalStartSearching()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var _cancellationToken = _cancellationTokenSource.Token;
            _cancellationToken.Register(() => LogMessage("Cancelled"));
            _searchTask = Task.Factory.StartNew(DoSearch, _cancellationToken);
            Task.Factory.StartNew(() =>
                                      {
                                          while (true)
                                          {
                                              if (_searchTask == null)
                                              {
                                                  break;
                                              }
                                              if (_searchTask.IsFaulted)
                                              {
                                                  LogMessage("Error", _searchTask.Exception);
                                                  break;
                                              }
                                              if (_searchTask.IsCompleted)
                                              {
                                                  break;
                                              }
                                              Thread.Sleep(500);
                                          }
                                      });
        }

        private void DoSearch()
        {
            LogMessage("Search started.");
            //todo: DO SEARCH
            for(int i = 0; i<100; i++)
            {
                // Check for cancel
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                // Do some work
                Thread.Sleep(100);


                Progress((int)(i / (100.0 - 1) * 100.0), ((int)(i / (100.0 - 1) * 100.0)).ToString());
            }
            LogMessage("Search finished.");
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
    }

    public class LogMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public LogMessageEventArgs(string message, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }
    }
}
