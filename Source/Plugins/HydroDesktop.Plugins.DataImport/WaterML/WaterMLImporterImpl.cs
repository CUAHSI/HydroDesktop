using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Common;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterML;

namespace HydroDesktop.Plugins.DataImport.WaterML
{
    class WaterMLImporterImpl : IImporter
    {
        public IList<Series> Import(IImporterSettings settings)
        {
            var wmlSettings = (WaterMLImportSettings) settings;
            
            string fileName = wmlSettings.PathToFile;
            var themeName = wmlSettings.ThemeName;

            string shortFileName = Path.GetFileNameWithoutExtension(fileName);
            string siteName = shortFileName;
            string variableName = shortFileName;

            int separatorIndex = shortFileName.IndexOf("_", StringComparison.Ordinal);
            if (separatorIndex > 0 && separatorIndex < shortFileName.Length - 1)
            {
                siteName = shortFileName.Substring(0, shortFileName.IndexOf("_"));
                variableName = shortFileName.Substring(shortFileName.IndexOf("_"));
            }


            IList<Series> seriesList = null;
            var parser = ParserFactory.GetParser(fileName);
            using (var fileStrem = File.OpenRead(fileName))
            {
                try
                {
                    seriesList = parser.ParseGetValues(fileStrem);  
  
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error parsing xml file: " + ex.Message);
                }
                
            }
            
            if (seriesList != null)
            {
                if (ValidateSeriesList(seriesList))
                {
                    var db = RepositoryFactory.Instance.Get<IRepositoryManager>();
                    var theme = new Theme(themeName);
                    foreach (var series in seriesList)
                    {
                        //check if the series has values
                        if (series.ValueCount == 0) continue;

                        if (String.IsNullOrEmpty(series.Site.Name))
                        {
                            series.Site.Name = siteName;
                        }
                        if (String.IsNullOrEmpty(series.Variable.Name))
                        {
                            series.Variable.Name = variableName;
                        }
                        db.SaveSeries(series, theme, OverwriteOptions.Copy);
                    }
                }
            }

            return seriesList ?? (new List<Series>(0));
        }

        public IProgressHandler ProgressHandler
        {
            get;set;
        }

        private static bool ValidateSeriesList(ICollection<Series> seriesList)
        {
            if (seriesList.Count == 0)
            {
                MessageBox.Show("There was an error parsing the WaterML document. " +
                    "Please check if the document is in correct WaterML 1.0 format.");
                return false;
            }

            int numValues = seriesList.Sum(curSeries => curSeries.ValueCount);
            if (numValues == 0)
            {
                MessageBox.Show("The series read from the xml file doesn't contain any data values." +
                    " The series will not be saved to the database.");
                return false;
            }
            return true;
        }
    }
}