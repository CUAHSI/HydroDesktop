using System.Data;

namespace DataImport.Excel
{
    public class ExcelImportSettings : IDataTableImportSettings
    {
        public string PathToFile {get;set;}
        public string SheetName { get; set; }
        public DataSet DataSet { get; set; }
        public DataTable Preview { get; set; }
        public DataTable Data { get; set; }
    }
}