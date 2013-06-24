using System.Collections.Generic;
using HydroDesktop.Common;
using HydroDesktop.Interfaces.ObjectModel;

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
        /// <returns>Imported series.</returns>
        IList<Series> Import(IImporterSettings settings);

        /// <summary>
        /// Progress handler
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
    }
}