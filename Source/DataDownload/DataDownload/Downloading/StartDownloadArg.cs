using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DotSpatial.Symbology;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.DataDownload.Downloading
{
    /// <summary>
    /// Class with information needed to call DownloadManager.Start()
    /// </summary>
    public class StartDownloadArg
    {
        /// <summary>
        /// Constructor of DownloadArg
        /// </summary>
        /// <param name="downloadList">Collection of DownloadInfo</param>
        /// <param name="dataTheme">Data theme</param>
        /// <exception cref="ArgumentNullException">downloadList, dataTheme must be not null.</exception>
        public StartDownloadArg(IList<OneSeriesDownloadInfo> downloadList, Theme dataTheme)
        {
            if (downloadList == null) throw new ArgumentNullException("downloadList");
            if (dataTheme == null) throw new ArgumentNullException("dataTheme");

            ItemsToDownload = new ReadOnlyCollection<OneSeriesDownloadInfo>(downloadList);
            DataTheme = dataTheme;
        }

        /// <summary>
        /// Constructor of DownloadArg
        /// </summary>
        /// <param name="downloadList">Collection of DownloadInfo</param>
        /// <param name="dataThemeName">Data theme name</param>
        /// <exception cref="ArgumentNullException">downloadList, dataThemeName must be not null.</exception>
        public StartDownloadArg(IList<OneSeriesDownloadInfo> downloadList, string dataThemeName): 
            this(downloadList, new Theme(dataThemeName))
        {
        }

        /// <summary>
        /// Collection of all items to be downloaded.
        /// </summary>
        public ReadOnlyCollection<OneSeriesDownloadInfo> ItemsToDownload { get; private set; }
        /// <summary>
        /// Data theme.
        /// </summary>
        public Theme DataTheme { get; private set; }

        /// <summary>
        /// Gets or sets source feature layer
        /// </summary>
        public IFeatureLayer FeatureLayer { get; set; }
    }
}
