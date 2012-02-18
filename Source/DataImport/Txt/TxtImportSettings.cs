using System.Collections.Generic;
using System.Data;
using DataImport.CommonPages;

namespace DataImport.Txt
{
    public class TxtImportSettings : IColumnDataImportSettings
    {
        public string PathToFile{get; set;}
        public TxtFileType FileType { get; set; }
        public string Delimiter { get; set; }
        public DataTable Preview { get; set; }
        public IList<ColumnData> ColumnDatas { get; set; }
        public string DateTimeColumn { get; set; }
        public DataTable Data { get; set; }
    }
}