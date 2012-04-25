using DotSpatial.Symbology;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Provides methods for data aggregation
    /// </summary>
    public interface IDataAggregationPlugin
    {
        /// <summary>
        /// Attach layer to data aggregation plug-in
        /// </summary>
        /// <param name="layer">Layer to attach</param>
        void AttachLayerToPlugin(ILayer layer);
    }
}