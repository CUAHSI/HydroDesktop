using HydroDesktop.Common;

namespace DataImport
{
    /// <summary>
    /// Interface for importer
    /// </summary>
    public interface IImporter
    {
        /// <summary>
        /// Import data using settings
        /// </summary>
        /// <param name="settings">Setting for import</param>
        void Import(IImporterSettings settings);

        /// <summary>
        /// Progress handler
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
    }
}