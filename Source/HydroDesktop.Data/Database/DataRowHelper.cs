using System.Data;

namespace HydroDesktop.Database
{
    static class DataRowHelper
    {
        public static object GetDataOrNull(this DataRow row, string columnName)
        {
            if (row.Table != null && !row.Table.Columns.Contains(columnName))
                return null;
            return row[columnName];
        }
    }
}