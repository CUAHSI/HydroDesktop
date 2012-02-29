using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using HydroDesktop.Interfaces;
using System.Globalization;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for DataValues
    /// </summary>
    class DataValuesRepository : BaseRepository<DataValue>, IDataValuesRepository
    {
        #region Constructors

        public DataValuesRepository(DatabaseTypes dbType, string connectionString)
            : base(dbType, connectionString)
        {
        }

        public DataValuesRepository(IHydroDbOperations db)
            : base(db)
        {
        }

        #endregion

        #region Public methods

        public double? AggregateValues(long seriesID, string aggregateFunction, DateTime minDate, DateTime maxDate)
        {
            var query =
                string.Format(
                    "select {0}(DataValue) from {1} main " +
                    "LEFT JOIN DataSeries ds ON ds.SeriesID = main.SeriesID " +
                    "LEFT JOIN Variables v ON v.VariableID = ds.VariableID " +
                    "WHERE DateTimeUTC >= '{2}' and DateTimeUTC <= '{3}' and main.SeriesID = {4} and DataValue <> v.NoDataValue ",
                    aggregateFunction, TableName, 
                    minDate.ToString("yyyy-MM-dd HH:mm:ss"), maxDate.ToString("yyyy-MM-dd HH:mm:ss"), 
                    seriesID);
            var value =  DbOperations.ExecuteSingleOutput(query);
            if (value != DBNull.Value)
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }
        }

        public double CalculatePercAvailable(long seriesID, DateTime minDate, DateTime maxDate)
        {
            var query =
              string.Format(
                  "select count(DataValue) from {0} main " +
                  "LEFT JOIN DataSeries ds ON ds.SeriesID = main.SeriesID " +
                  "LEFT JOIN Variables v ON v.VariableID = ds.VariableID " +
                  "WHERE DateTimeUTC >= '{1}' and DateTimeUTC <= '{2}' and main.SeriesID = {3} and DataValue <> v.NoDataValue ",
                  TableName,
                  minDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), maxDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                  seriesID);
            var hasValues = Convert.ToDouble(DbOperations.ExecuteSingleOutput(query), CultureInfo.InvariantCulture);

            query =
              string.Format(
                  "select count(DataValue) from {0} main " +
                  "WHERE DateTimeUTC >= '{1}' and DateTimeUTC <= '{2}' and main.SeriesID = {3} ",
                  TableName,
                  minDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), maxDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                  seriesID);
            var totalValues = Convert.ToDouble(DbOperations.ExecuteSingleOutput(query), CultureInfo.InvariantCulture);

            var value = hasValues*100.0/totalValues;
            return value;
        }

        public DataTable GetAll(long seriesID)
        {
            var query = "SELECT * FROM DataValues WHERE SeriesID = " + seriesID;
            var result = DbOperations.LoadTable(TableName, query);
            return result;
        }

        public DataTable GetAllOrderByLocalDateTime(long seriesID)
        {
            var query = "SELECT * FROM DataValues WHERE SeriesID = " + seriesID +  " ORDER BY LocalDateTime";
            var result = DbOperations.LoadTable(TableName, query);
            return result;
        }

        public IList<double> GetValues(long seriesID)
        {
            var list = DbOperations.Read("SELECT DataValue FROM DataValues WHERE SeriesID = " + seriesID,
                                                 r => r.GetDouble(0));
            return list;
        }

        public DataTable GetTableForExportFromTimeSeriesPlot(long seriesID)
        {
            var query =
                "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.DataValue, dv.LocalDateTime, U.UnitsName " +
                "FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U " +
                "WHERE v.VariableID = ds.VariableID AND s.SiteID = ds.SiteID AND dv.SeriesID = ds.SeriesID AND U.UnitsID = v.VariableUnitsID AND ds.SeriesID = " +
                seriesID;
            return DbOperations.LoadTable(TableName, query);
        }

        public DataTable GetTableForExport(long seriesID, double? noDataValue = null, string dateColumn = null, DateTime? firstDate = null, DateTime? lastDate = null)
        {
            var sql =
                "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.LocalDateTime, dv.DataValue, U1.UnitsName As VarUnits, v.DataType, s.SiteID, s.SiteCode, v.VariableID, v.VariableCode, " +
                "S.Organization, S.SourceDescription, S.SourceLink, v.ValueType, v.TimeSupport, U2.UnitsName As TimeUnits, v.IsRegular, v.NoDataValue, " +
                "dv.UTCOffset, dv.DateTimeUTC, s.Latitude, s.Longitude, dv.ValueAccuracy, dv.CensorCode, m.MethodDescription, q.QualityControlLevelCode, v.SampleMedium, v.GeneralCategory " +
                "FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U1, Units U2, Methods m, QualityControlLevels q, Sources S " +
                "WHERE v.VariableID = ds.VariableID " +
                "AND s.SiteID = ds.SiteID " +
                "AND m.MethodID = ds.MethodID " +
                "AND q.QualityControlLevelID = ds.QualityControlLevelID " +
                "AND S.SourceID = ds.SourceID " +
                "AND dv.SeriesID = ds.SeriesID " +
                "AND U1.UnitsID = v.VariableUnitsID " +
                "AND U2.UnitsID = v.TimeUnitsID " +
                "AND ds.SeriesID = " + seriesID;
            if (noDataValue.HasValue)
            {
                sql += " AND dv.DataValue != " + noDataValue;
            }

            var cmd = DbOperations.CreateCommand(sql);

            // Append date range filter
            if (!string.IsNullOrEmpty(dateColumn) && 
                firstDate.HasValue && lastDate.HasValue)
            {
                cmd.CommandText += string.Format(" AND ({0} >=  @p1 and {0} <=  @p2)", dateColumn);
                var startDateParameter = DbOperations.AddParameter(cmd, "@p1", DbType.DateTime);
                var endDateParemater = DbOperations.AddParameter(cmd, "@p2", DbType.DateTime);

                startDateParameter.Value = firstDate.Value;
                endDateParemater.Value = lastDate.Value;
            }

            var tbl = DbOperations.LoadTable("values", cmd);

            return tbl;
        }

        public DataTable GetTableForGraphView(long seriesID, double nodatavalue, DateTime startDate, DateTime endDate)
        {
            var strStartDate = startDate.ToString("yyyy-MM-dd HH:mm:ss");
            var strEndDate = endDate.ToString("yyyy-MM-dd HH:mm:ss");
            var strNoDataValue = nodatavalue.ToString(CultureInfo.InvariantCulture);

            var query =
                "SELECT DataValue, LocalDateTime, CensorCode, strftime('%m', LocalDateTime) as DateMonth, strftime('%Y', LocalDateTime) as DateYear FROM DataValues WHERE (SeriesID = " +
                +seriesID + ") AND (DataValue <> " + strNoDataValue + ") AND (LocalDateTime between '" + strStartDate +
                "' AND '" + strEndDate + "')  ORDER BY LocalDateTime";
            var table = DbOperations.LoadTable("DataValues", query);
            return table;
        }

        public long GetCountForAllFieldsInSequence(IList<int> seriesIDs)
        {
            var whereClause = GetWhereClauseForIds(seriesIDs);
            var countQuery = "select count(*) from DataValues WHERE " + whereClause;
            var res = DbOperations.ExecuteSingleOutput(countQuery);
            return Convert.ToInt64(res);
        }

        public long GetCountForJustValuesInParallel(IList<int> seriesIDs)
        {
            var whereClause = GetWhereClauseForIds(seriesIDs);
            var countQuery =
                string.Format("select count(*) from (select distinct LocalDateTime from DataValues where {0}) A",
                              whereClause);
            var res = DbOperations.ExecuteSingleOutput(countQuery);
            return Convert.ToInt64(res);
        }

        public DataTable GetTableForAllFieldsInSequence(IList<int> seriesIDs, int valuesPerPage, int currentPage)
        {
            var whereClause = GetWhereClauseForIds(seriesIDs);
            var dataQuery =
                "SELECT ValueID, SeriesID, DataValue, LocalDateTime, UTCOffset, CensorCode FROM DataValues WHERE " +
                whereClause;
            var table = DbOperations.LoadTable(string.Format("{0} limit {1} offset {2}", dataQuery, valuesPerPage, currentPage * valuesPerPage));
            return table;
        }

        public DataTable GetTableForJustValuesInParallel(IList<int> seriesIDs, int valuesPerPage, int currentPage)
        {
            /*
             Example of builded query:            
            
             select
                 A.LocalDateTime as DateTime, 
                 (select  DV1.DataValue from DataValues DV1 where DV1.LocalDateTime = A.LocalDateTime and DV1.seriesId = 1 limit 1) as D1,
                 (select  DV2.DataValue from DataValues DV2 where DV2.LocalDateTime = A.LocalDateTime and DV2.seriesId = 2 limit 1) as D2
             from
                 (select distinct LocalDateTime from DataValues where seriesId in (1,2)) A
             order by LocalDateTime
            
             */


            var whereClause = GetWhereClauseForIds(seriesIDs);
            var dataQueryBuilder = new StringBuilder();
            dataQueryBuilder.Append("select A.LocalDateTime as DateTime");
            foreach (var id in seriesIDs)
            {
                dataQueryBuilder.AppendFormat(
                    ", (select DV{0}.DataValue from DataValues DV{0} where DV{0}.LocalDateTime = A.LocalDateTime and DV{0}.seriesId = {0} limit 1) as D{0}",
                    id);
            }
            dataQueryBuilder.AppendFormat(" from (select distinct LocalDateTime  from DataValues where {0}) A",
                                          whereClause);
            dataQueryBuilder.Append(" order by LocalDateTime");

            var dataQuery = dataQueryBuilder.ToString();

            var table = DbOperations.LoadTable(string.Format("{0} limit {1} offset {2}", dataQuery, valuesPerPage, currentPage * valuesPerPage));
            return table;
        }

        #endregion

        private static string GetWhereClauseForIds(ICollection<int> seriesIDs)
        {
            string whereClause;
            if (seriesIDs.Count == 0)
            {
                whereClause = "1 = 0";
            }
            else
            {
                var sb = new StringBuilder("SeriesID in (");
                foreach (var id in seriesIDs)
                    sb.AppendFormat(" {0},", id);
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                whereClause = sb.ToString();
            }
            return whereClause;
        }

        public override string TableName
        {
            get { return "DataValues"; }
        }

        public override string PrimaryKeyName
        {
            get { return "ValueID"; }
        }
    }
}