namespace DataImport
{
    public class DataImportContext
    {
        public IDataImporter Importer { get; set; }
        public IFileImportSettings Settings { get; set; }
    }
}