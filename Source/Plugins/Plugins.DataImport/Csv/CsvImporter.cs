using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using HydroDesktop.Plugins.DataImport.CommonPages;
using HydroDesktop.Plugins.DataImport.CommonPages.Complete;
using HydroDesktop.Plugins.DataImport.CommonPages.Progress;
using HydroDesktop.Plugins.DataImport.CommonPages.SelectLayer;
using HydroDesktop.Plugins.DataImport.DataTableImport;
using HydroDesktop.ImportExport;
using Wizard.UI;

namespace HydroDesktop.Plugins.DataImport.Csv
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

        public void UpdatePreview(IWizardImporterSettings settings)
        {
            var preview = CsvFileParser.ParseFileToDataTable(settings.PathToFile, true, null, null, 10);
            settings.Preview = preview;
        }

        public void UpdateData(IWizardImporterSettings settings)
        {
            var data = CsvFileParser.ParseFileToDataTable(settings.PathToFile, true);
            settings.Data = data;
        }

        public ICollection<WizardPage> GetWizardPages(WizardContext context)
        {
            return new Collection<WizardPage>
                       {
                           new FormatOptionsPage(context),
                           new FieldPropertiesPage(context),
                           new DataLayerPage(context),
                           new ProgressPage(context),
                           new CompletePage(),
                       };
        }
    }
}