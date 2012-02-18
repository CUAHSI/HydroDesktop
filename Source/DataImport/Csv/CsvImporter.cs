using System;
using System.Collections.Generic;
using System.IO;
using DataImport.DataTableImport;
using Wizard.UI;

namespace DataImport.Csv
{
    class CsvImporter : IWizardImporter
    {
        public string Filter
        {
            get { return "CSV File (*.csv)|*.csv"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            return string.Equals(Path.GetExtension(pathToFile), ".csv", StringComparison.InvariantCultureIgnoreCase);
        }

        public IWizardImporterSettings GetDefaultSettings()
        {
            return new CsvImportSettings();
        }
        
        public IImporter GetImporter()
        {
            return new DataTableImporterImpl();
        }

        public void SetPreview(IWizardImporterSettings settings)
        {
            throw new NotImplementedException();
        }

        public void SetData(IWizardImporterSettings settings)
        {
            throw new NotImplementedException();
        }

        public ICollection<WizardPage> GetWizardPages(WizardContext context)
        {
            throw new NotImplementedException();
        }
    }
}