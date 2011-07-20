namespace HydroDesktop.Search.LayerInformation
{
    /// <summary>
    /// Provides methods to extract some service info
    /// </summary>
    interface IServiceInfoExtractor
    {
        /// <summary>
        /// Get service description URL by serviceUrl
        /// </summary>
        /// <param name="serviceUrl">ServiceUrl</param>
        /// <returns>ServiceDesciptionUrl</returns>
        string GetServiceDesciptionUrl(string serviceUrl);
    }
}