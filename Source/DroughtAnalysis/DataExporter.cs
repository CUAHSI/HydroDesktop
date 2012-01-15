using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Database;
using System.Data;
using System.IO;
using System.Globalization;
using System.Collections;
using HydroDesktop.ImportExport;

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
            var dbm = RepositoryFactory.Instance.CreateRepositoryManager(HydroDesktop.Interfaces.DatabaseTypes.SQLite, HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString);
      
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

            //change the column names
            valuesTable.Columns[0].ColumnName = "Datum";
            for (int i = 0; i < station.DataSeriesList.Count; i++)
            {
                string vName = station.DataSeriesList[i].Variable.Name.ToLower();
                if (vName.Contains("temp"))
                    valuesTable.Columns[i + 1].ColumnName = "Teploty";
                else if (vName.Contains("precip"))
                    valuesTable.Columns[i + 1].ColumnName = "Srazky";
            }

            string separator = "\t";
            DelimitedTextWriter.DataTableToDelimitedFile(valuesTable, outFileName,
            new DelimitedFormatOptions { Append = false, Delimiter = separator, IncludeHeaders = true, UseShortDateFormat = true, UseInvariantCulture = true });
            
            //todo: Use backgroundWorker for writing large files
        }

        
    }
}
