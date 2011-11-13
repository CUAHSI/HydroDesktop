using DotSpatial.Symbology;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface of data export plugin
    /// </summary>
    public interface IDataExportPlugin
    {
        /// <summary>
        /// Export layer
        /// </summary>
        /// <param name="layer">Layer to export.</param>
        void Export(IFeatureLayer layer);
    }
}