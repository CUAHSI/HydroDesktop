namespace HydroDesktop.DataDownload.Downloading
{
    /// <summary>
    /// Statuses of DownloadInfo
    /// </summary>
    public enum DownloadInfoStatus
    {
        /// <summary>
        /// Pending (awaitng to downloading)
        /// </summary>
        Pending,
        /// <summary>
        /// Downloading
        /// </summary>
        Downloading,
        /// <summary>
        /// Downloaded
        /// </summary>
        Downloaded,
        /// <summary>
        /// Some error occured during downloading or saving
        /// </summary>
        Error,
        /// <summary>
        /// Downloaded and saved without errors/warnings.
        /// </summary>
        Ok,
        /// <summary>
        /// Downloaded and saved with warnings.
        /// </summary>
        OkWithWarnings
    }
}