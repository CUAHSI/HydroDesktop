using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Search.Download
{
    public class DownloadArg
    {
        /// <summary>
        /// Constructor of DownloadArg
        /// </summary>
        /// <param name="downloadList">Collection of DownloadInfo</param>
        /// <param name="dataTheme">Data theme</param>
        /// <exception cref="ArgumentNullException">downloadList must be not null.</exception>
        public DownloadArg(IList<DownloadInfo> downloadList, Theme dataTheme)
        {
            if (downloadList == null)
                throw new ArgumentNullException("downloadList");

            DownloadList = new ReadOnlyCollection<DownloadInfo>(downloadList);
            DataTheme = dataTheme;
        }

        public ReadOnlyCollection<DownloadInfo> DownloadList { get; private set; }
        public Theme DataTheme { get; private set; }
    }
}
