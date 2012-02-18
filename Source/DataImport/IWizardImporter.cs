using System.Collections.Generic;
using Wizard.UI;

namespace DataImport
{
    /// <summary>
    /// Common interface for wizard importer
    /// </summary>
    public interface IWizardImporter
    {
        /// <summary>
        /// Filter in open file dialog
        /// </summary>
        string Filter { get; }

        /// <summary>
        /// Get that file can be imported by current data importer.
        /// </summary>
        /// <param name="pathToFile">Path to file with data.</param>
        /// <returns></returns>
        bool CanImportFromFile(string pathToFile);

        /// <summary>
        /// Get default settings for current importer
        /// </summary>
        /// <returns>Default settings</returns>
        IWizardImporterSettings GetDefaultSettings();

        /// <summary>
        /// Get importer to import
        /// </summary>
        /// <returns>Importer</returns>
        IImporter GetImporter();

        void SetPreview(IWizardImporterSettings settings);

        void SetData(IWizardImporterSettings settings);
      

        /// <summary>
        /// Get wizard pages
        /// </summary>
        /// <param name="context">Wizards data context</param>
        /// <returns>Collection of wizard pages</returns>
        ICollection<WizardPage> GetWizardPages(WizardContext context);
    }
}