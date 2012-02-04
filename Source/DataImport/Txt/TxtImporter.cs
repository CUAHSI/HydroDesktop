using System;
using System.IO;

namespace DataImport.Txt
{
    class TxtImporter : IDataImporter
    {
        public string Filter
        {
            get { return "Text File (*.txt)|*.txt"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            return string.Equals(Path.GetExtension(pathToFile), ".txt", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Import(string pathToFile)
        {
            throw new NotImplementedException();
        }
    }
}