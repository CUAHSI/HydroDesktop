using System.Data;
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

        /// <summary>
        /// Export DataTable
        /// </summary>
        /// <param name="dataTable">DataTable to export</param>
        void Export(DataTable dataTable);
    }
}