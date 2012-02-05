using System;
using System.Collections.Generic;
using System.Data;
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

        IDataImportSettings GetDefaultSettings();
        void Import(IDataImportSettings settings);
        ICollection<Func<DataImportContext, WizardPage>> GePageCreators();
        DataTable GetPreview(IDataImportSettings settings);
    }

    public class DataImportContext
    {
        public IDataImporter Importer { get; set; }
        public IDataImportSettings Settings { get; set; }
    }

    public interface IDataImportSettings
    {
        string PathToFile { get; set; }
    }
}