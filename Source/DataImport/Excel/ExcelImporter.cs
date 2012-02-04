using System;
using System.IO;

namespace DataImport.Excel
{
    class ExcelImporter : IDataImporter
    {
        public string Filter
        {
            get { return "Excel File (*.xls;*.xlsx)|*.xls;*.xlsx"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            var extension = Path.GetExtension(pathToFile);
            return string.Equals(extension, ".xlsx", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(extension, ".xls", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Import(string pathToFile)
        {
            throw new NotImplementedException();
        }
    }
}