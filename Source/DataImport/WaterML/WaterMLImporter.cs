using System;
using System.IO;
using ImportFromWaterML;

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

        public void Import(string pathToFile)
        {
            var dlg = new ImportDialog(pathToFile);
            dlg.ShowDialog();
        }
    }
}