using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using ImportFromWaterML;
using Wizard.UI;

namespace DataImport.WaterML
{
    class WaterMLImporter : IDataImporter
    {
        public string Filter
        {
            get { return "WaterML File (*.xml)|*.xml"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            return string.Equals(Path.GetExtension(pathToFile), ".xml", StringComparison.InvariantCultureIgnoreCase);
        }

        public IFileImportSettings GetDefaultSettings()
        {
            throw new NotImplementedException();
        }

        public void Import(IFileImportSettings settings)
        {
            var wmlSettings = (WaterMLImportSettings) settings;

            var objDownloader = new Downloader();
            string fileName = wmlSettings.PathToFile;
            var themeName = wmlSettings.ThemeName;

            string shortFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string siteName = shortFileName;
            string variableName = shortFileName;

            int separatorIndex = shortFileName.IndexOf("_");
            if (separatorIndex > 0 && separatorIndex < shortFileName.Length - 1)
            {
                siteName = shortFileName.Substring(0, shortFileName.IndexOf("_"));
                variableName = shortFileName.Substring(shortFileName.IndexOf("_"));
            }


            IList<Series> seriesList = objDownloader.DataSeriesFromXml(fileName);
            if (seriesList == null)
            {
                MessageBox.Show("error converting xml file");
                return;
            }
            if (objDownloader.ValidateSeriesList(seriesList))
            {

                foreach (Series series in seriesList)
                {
                    objDownloader.SaveDataSeries(series, themeName, siteName, variableName);
                }
            }

        }

        public ICollection<Func<DataImportContext, WizardPage>> GePageCreators()
        {
            return new Collection<Func<DataImportContext, WizardPage>>
                       {
                           c => new OptionsPage(c),
                           c => new ProgressPage(c),
                           c => new CompletePage(),
                       };
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
}