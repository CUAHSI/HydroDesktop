namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Progress handler for calling the GetValues WaterOneFlow service request
    /// </summary>
    public interface IGetValuesProgressHandler
    {
        /// <summary>
        /// Report progress 
        /// </summary>
        /// <param name="intervalNumber">Number of downloaded interval</param>
        /// <param name="totalIntervalsCount">Total intervals count</param>
        /// <param name="timeTaken">Time taken to download current interval (in seconds)</param>
        void Progress(int intervalNumber, int totalIntervalsCount, double timeTaken);

        /// <summary>
        /// Shows that current operation should be cancelled
        /// </summary>
        bool CancellationPending { get;}
    }
}