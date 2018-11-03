using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls.Header;
using DotSpatial.Controls;

namespace HydroDesktop.Plugins.Search
{
    /// <summary>
    /// Responsible for displaying search summary in the status bar
    /// </summary>
    public class SearchStatusDisplay
    {
        private bool _showSearchStatus = false;
        StatusPanel searchStatusPanel = null;
        private AppManager _app = null;

        public SearchStatusDisplay(AppManager app)
        {
            searchStatusPanel = new StatusPanel();
            searchStatusPanel.Width = 600;
            _app = app;
            //app.ProgressHandler.Add(searchStatusPanel);
        }
        
        /// <summary>
        /// True to display the search status, false otherwise
        /// </summary>
        public bool ShowSearchStatus
        {
            get
            {
                return _showSearchStatus;
            }
            set
            {
                bool oldStatus = _showSearchStatus;
                _showSearchStatus = value;

                if (oldStatus != _showSearchStatus)
                {
                    if (!_showSearchStatus)
                        _app.ProgressHandler.Remove(searchStatusPanel);
                    else
                        _app.ProgressHandler.Add(searchStatusPanel);
                }
            }
        }

        public string AreaStatus { get; set; }

        public string KeywordStatus { get; set; }

        public string DataSourceStatus { get; set; }

        public string TimeStatus { get; set; }

        /// <summary>
        /// Updates the status display
        /// </summary>
        public void UpdateStatus()
        {
            string keywordDisplay = KeywordStatus != null ? KeywordStatus : String.Empty;
            if (keywordDisplay.Length > 200)
            {
                keywordDisplay = keywordDisplay.Substring(0, 200) + "...";
            }
            string sourceDisplay = DataSourceStatus != null ? DataSourceStatus : String.Empty;
            if (sourceDisplay.Length > 200)
            {
                sourceDisplay = DataSourceStatus.Substring(0, 200) + "...";
            }
            
            searchStatusPanel.Caption = String.Format("Area: {0} | Keywords: {1} | Sources: {2}", AreaStatus, keywordDisplay, sourceDisplay);
        }
    }
}
