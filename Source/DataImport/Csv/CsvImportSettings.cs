using System.Collections.Generic;
using System.Data;

namespace DataImport.Csv
{
    /// <summary>
    /// Setting for csv
    /// </summary>
    public class CsvImportSettings : IWizardImporterSettings
    {
        public string PathToFile{get;set;}
        public DataTable Preview { get; set; }
        public DataTable Data { get; set; }
        public IList<ColumnInfo> ColumnDatas { get; set; }
        public string DateTimeColumn { get; set; }
    }
}