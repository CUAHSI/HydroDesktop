using System.Collections.Generic;
using DataImport.CommonPages;


namespace DataImport
{
    public interface IColumnDataImportSettings : IDataTableImportSettings
    {
        IList<ColumnData> ColumnDatas { get; set; }
        string DateTimeColumn { get; set; }
    }
}