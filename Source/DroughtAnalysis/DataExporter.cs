using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Database;
using System.Data;
using System.IO;
using System.Globalization;

namespace DroughtAnalysis
{
    internal class VariableExportInfo
    {
        public VariableExportInfo(Variable var, string colName)
        {
            Variable = var;
            ColumnName = colName;
        }
        
        public Variable Variable { get; set; }
        public string ColumnName { get; set; }
    }
    
    /// <summary>
    /// Responsible for exporting data from the selected station
    /// </summary>
    internal class DataExporter
    {
        private List<VariableExportInfo> _variableList = new List<VariableExportInfo>();
        
        public IList<VariableExportInfo> VariablesToExport
        {
            get { return _variableList; }
        }
        
        //exports data in time, [variable1], [variable2]
        public void ExportDataForStation(Site station, string outFileName)
        {
            if (station.DataSeriesList == null)
                throw new ArgumentException("a list of DataSeries must be associated with the station");
            
            //valid variable ID's
            List<long> variableIDs = new List<long>();
            foreach(VariableExportInfo ve in _variableList)
            {
                variableIDs.Add(ve.Variable.Id);
            }
            
            //get the series
            RepositoryManagerSQL dbm = new RepositoryManagerSQL(HydroDesktop.Interfaces.DatabaseTypes.SQLite, HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString);
      
            IList<Series> validSeriesList = station.DataSeriesList;

            //generate the sql string
            StringBuilder sqlSelect = new StringBuilder("SELECT dv0.LocalDateTime, dv0.DataValue, ");
            StringBuilder sqlJoin = new StringBuilder(" FROM DataValues dv0 ");
            StringBuilder sqlWhere = new StringBuilder(String.Format(" WHERE dv0.SeriesID = {0} ", validSeriesList[0].Id));
            for (int i = 1; i < validSeriesList.Count; i++)
            {
                sqlSelect.Append(string.Format("dv{0}.DataValue", i));
                if (i < validSeriesList.Count - 1) sqlSelect.Append(", ");

                sqlJoin.Append(string.Format(" INNER JOIN DataValues dv{0} ON dv0.LocalDateTime = dv{0}.LocalDateTime ", i));

                sqlWhere.Append(string.Format(" AND dv{0}.SeriesID = {1} ", i, validSeriesList[i].Id));
            }

            string sqlQueryValues = sqlSelect.ToString() + sqlJoin.ToString() + sqlWhere.ToString();
            DataTable valuesTable = dbm.DbOperations.LoadTable(sqlQueryValues);

            string sep = "\t";
            using (StreamWriter wri = new StreamWriter(outFileName, false))
            {
                //write header
                wri.WriteLine(string.Format("{0}{1}{2}{3}{4}", "datum", sep, "tep", sep, "sra"));

                foreach (DataRow row in valuesTable.Rows)
                {
                    wri.WriteLine(string.Format("{0}{1}{2}{1}{3}{4}", Convert.ToDateTime(row[0]).ToString("yyyy-MM-dd"), sep, 
                        Convert.ToString(row[1], CultureInfo.InvariantCulture), sep, Convert.ToString(row[2], CultureInfo.InvariantCulture)));
                }
            }

        }
    }
}
