using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Wizard.UI;

namespace DataImport.Csv
{
    class CsvImporter : IDataImporter
    {
        public string Filter
        {
            get { return "CSV File (*.csv)|*.csv"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            return string.Equals(Path.GetExtension(pathToFile), ".csv", StringComparison.InvariantCultureIgnoreCase);
        }

        public IDataImportSettings GetDefaultSettings()
        {
            return new CsvImportSettings();
        }

        public void Import(IDataImportSettings settings)
        {
            throw new NotImplementedException();
        }

        public ICollection<Func<DataImportContext, WizardPage>> GePageCreators()
        {
            throw new NotImplementedException();
        }

        public void SetPreview(IDataImportSettings settings)
        {
            throw new NotImplementedException();
        }
    }

    public class CsvImportSettings : IDataImportSettings
    {
        public string PathToFile{get;set;}
        public DataTable Preview { get; set; }
    }
}