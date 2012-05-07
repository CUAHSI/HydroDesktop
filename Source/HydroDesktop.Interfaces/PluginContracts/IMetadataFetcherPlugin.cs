namespace HydroDesktop.Interfaces.PluginContracts
{
    /// <summary>
    /// Interface of MetadataFetcher plug-in
    /// </summary>
    public interface IMetadataFetcherPlugin
    {
        /// <summary>
        /// Add services to the list of services that can be harvested in metadata catalog.
        /// </summary>
        void AddServices();
    }
}
