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

        public IFileImportSettings GetDefaultSettings()
        {
            return new CsvImportSettings();
        }

        public void Import(IFileImportSettings settings)
        {
            throw new NotImplementedException();
        }

        public ICollection<Func<DataImportContext, WizardPage>> GePageCreators()
        {
            throw new NotImplementedException();
        }

        public void SetPreview(IFileImportSettings settings)
        {
            throw new NotImplementedException();
        }

        public void SetData(IFileImportSettings settings)
        {
            throw new NotImplementedException();
        }
    }

    public class CsvImportSettings : IFileImportSettings
    {
        public string PathToFile{get;set;}
        public DataTable Preview { get; set; }
        public DataTable Data { get; set; }
    }
}