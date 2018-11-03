using System.Data;

namespace HydroDesktop.Plugins.DataImport
{
    /// <summary>
    /// Wizard settings for data importer
    /// </summary>
    public interface IWizardImporterSettings : IImporterSettings
    {
        /// <summary>
        /// Data table with preview data
        /// </summary>
        DataTable Preview { get; set; }
    }
}