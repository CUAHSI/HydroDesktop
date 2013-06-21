namespace HydroDesktop.Common.Logging
{
    interface IExtraLog : ILog
    {
        /// <summary>
        /// Number of frames to skip when constructing stack-frame of calling method.
        /// </summary>
        int SkipFrames { get; set; }
    }
}