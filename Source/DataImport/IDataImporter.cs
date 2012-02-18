using System;
using System.Collections.Generic;
using Wizard.UI;

namespace DataImport
{
    /// <summary>
    /// Common interface for Data Import class
    /// </summary>
    public interface IDataImporter
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

        IFileImportSettings GetDefaultSettings();
        void Import(IFileImportSettings settings);
        ICollection<Func<DataImportContext, WizardPage>> GePageCreators();
        void SetPreview(IFileImportSettings settings);
        void SetData(IFileImportSettings settings);
    }
}