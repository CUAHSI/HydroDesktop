using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DataImport.CommonPages;
using Wizard.UI;

namespace DataImport.WaterML
{
    class WaterMLImporter : IWizardImporter
    {
        public string Filter
        {
            get { return "WaterML File (*.xml)|*.xml"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            return string.Equals(Path.GetExtension(pathToFile), ".xml", StringComparison.InvariantCultureIgnoreCase);
        }

        public IWizardImporterSettings GetDefaultSettings()
        {
            return new WaterMLImportSettings();
        }

        public IImporter GetImporter()
        {
            return new WaterMLImporterImpl();
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
            return new Collection<WizardPage>
                       {
                           new OptionsPage(context),
                           new ProgressPage(context),
                           new CompletePage(),
                       };
        }
    }
}