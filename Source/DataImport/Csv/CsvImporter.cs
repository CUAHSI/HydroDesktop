using System;
using System.IO;

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

        public void Import(string pathToFile)
        {
            throw new NotImplementedException();
        }
    }
}