using System.Data;

namespace DataImport
{
    public interface IFileImportSettings
    {
        string PathToFile { get; set; }
    }

    public interface IDataTableImportSettings : IFileImportSettings
    {
        DataTable Preview { get; set; }
        DataTable Data { get; set; }
    }
}