using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for DataSeries
    /// </summary>
    class DataSeriesRepository : BaseRepository<Series>, IDataSeriesRepository
    {
        #region Constructors
        
        /// <summary>
        /// Create new instance of <see cref="DataSeriesRepository"/>
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        public DataSeriesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="DataSeriesRepository"/>
        /// </summary>
        /// <param name="db">The DbOperations object for handling the database</param>
        public DataSeriesRepository(IHydroDbOperations db)
            : base(db)
        {
        }

        #endregion

        #region Public methods

        public double GetNoDataValueForSeriesVariable(long seriesID)
        {
            var query =
                "SELECT NoDataValue FROM DataSeries LEFT JOIN Variables ON DataSeries.VariableID = Variables.VariableID WHERE SeriesID = " +
                seriesID;
            var result = DbOperations.ExecuteSingleOutput(query);
            return Convert.ToDouble(result);
        }

        public int GetVariableID(int seriesID)
        {
            var result =
                DbOperations.ExecuteSingleOutput("SELECT VariableID FROM DataSeries WHERE SeriesID = " + seriesID);
            return Convert.ToInt32(result);
        }

        public DataTable GetDetailedSeriesTable()
        {
            string sql = DetailedSeriesSQLQuery();
            sql += " ORDER BY VariableName, SiteName";
            var table = DbOperations.LoadTable("SeriesListTable", sql);
            return table;
        }

        public DataTable GetSeriesTable(string seriesDataFilter)
        {
            string sql = DetailedSeriesSQLQuery() +
                " WHERE " + seriesDataFilter;

            var table = DbOperations.LoadTable("SeriesListTable", sql);
            return table;
        }

        public DataTable GetSeriesTable(IEnumerable<int> listOfSeriesID)
        {
            var seriesIDs = listOfSeriesID.ToArray();
            var sqlIn = new StringBuilder();
            for (int i = 0; i < seriesIDs.Length; i++)
            {
                sqlIn.Append(seriesIDs[i].ToString());
                if (i < seriesIDs.Length - 1)
                {
                    sqlIn.Append(",");
                }
            }
            string filter = "DataThemes.SeriesID in (" + sqlIn + ")";
            return GetSeriesTable(filter);
        }

        public DataTable GetSeriesTable(int seriesID)
        {
            return GetSeriesTable(string.Format("DataSeries.SeriesID={0}", seriesID));
        }
       
        public DataTable GetSeriesForThemeManager(int? themeID)
        {
            var sql =
                "SELECT src.Organization as 'DataSource', ds.SeriesID, " +
                "s.SiteName as 'SiteName', s.Latitude as 'Latitude', s.Longitude as 'Longitude', s.SiteCode as 'SiteCode', " +
                "v.VariableName as 'VariableName', v.VariableName as 'VarName', v.DataType as 'DataType', v.SampleMedium as 'SampleMedium', " +
                "v.VariableCode as 'VariableCode', u.UnitsName as 'Units', " +
                "v.VariableCode as 'ServiceCode', " + "v.VariableCode as 'VarCode', " +
                "m.MethodDescription as 'Method', qc.Definition as 'QualityControl', " +
                "ds.BeginDateTime as 'BeginDateTime', ds.EndDateTime as 'EndDateTime', ds.ValueCount as 'ValueCount', " +
                "ds.BeginDateTime as 'StartDate', ds.EndDateTime as 'EndDate', " +
                "null as 'ServiceURL' " +
                "FROM DataSeries ds " +
                "LEFT JOIN DataThemes dt on dt.SeriesID = ds.SeriesID " +
                "LEFT JOIN Sites s on ds.SiteID = s.SiteID " +
                "LEFT JOIN Variables v on ds.VariableID = v.VariableID " +
                "LEFT JOIN Units u on u.UnitsID = v.VariableUnitsID " +
                "LEFT JOIN Methods m on ds.MethodID = m.MethodID " +
                "LEFT JOIN Sources src on ds.SourceID = src.SourceID " +
                "LEFT JOIN QualityControlLevels qc on ds.QualityControlLevelID = qc.QualityControlLevelID " +
                (themeID.HasValue ? "WHERE dt.ThemeID = " + themeID : "WHERE dt.ThemeID is null");

            var table = DbOperations.LoadTable("ThemeTable", sql);
            return table;
        }
        
        public DataTable GetSeriesIDsWithNoDataValueTable(IEnumerable<int?> themeIDs)
        {
            var themeList = themeIDs.ToList();

            var hasNulls = themeList.Any(value => !value.HasValue);
            var notNullsFilter = new StringBuilder();
            const string separator = ", ";
            foreach (var themeID in themeList.Where(themeID => themeID.HasValue))
            {
                notNullsFilter.Append(themeID + separator);
            }
            if (notNullsFilter.Length > 0)
            {
                notNullsFilter.Remove(notNullsFilter.Length - separator.Length, separator.Length);
            }

            var query = "SELECT v.NoDataValue, ds.SeriesID " +
                        "FROM DataSeries ds INNER JOIN variables v ON ds.VariableID = v.VariableID " +
                        "LEFT JOIN DataThemes t ON ds.SeriesID = t.SeriesID " +
                        "WHERE t.ThemeID in (" + notNullsFilter + ")";
            if (hasNulls)
            {
                query += " or t.ThemeID is null";
            }

            var dtSeries = DbOperations.LoadTable("series", query);
            return dtSeries;
        }

        public IList<Series> GetAllSeriesForSite(Site mySite)
        {
            if (mySite.Id <= 0) throw new ArgumentException("The site must have a valid ID");
            Contract.EndContractBlock();

            string sqlQuery = DetailedSeriesSQLQuery();
            sqlQuery += string.Format(" WHERE DataSeries.SiteID = {0}", mySite.Id);
            DataTable tbl = DbOperations.LoadTable(sqlQuery);
            return SeriesListFromTable(tbl);
        }

        public IList<Series> GetAllSeries()
        {
            var seriesTable = GetDetailedSeriesTable();
            return SeriesListFromTable(seriesTable);
        }
        
        public IList<Series> GetSeriesBySite(Site site)
        {
            if (site.Id <= 0) throw new ArgumentException("site must have a valid ID");

            string filter = "DataSeries.SiteID = " + site.Id;
            DataTable seriesTable = GetSeriesTable(filter);
            return SeriesListFromTable(seriesTable);
        }

        public Series GetSeriesByID(long seriesID)
        {
            var seriesTable = DbOperations.LoadTable("seriesTable", "select * from DataSeries where seriesID=" + seriesID);
            if (seriesTable.Rows.Count == 0) return null;

            var series = new Series();
            DataRow seriesRow = seriesTable.Rows[0];
            series.BeginDateTime = Convert.ToDateTime(seriesRow["BeginDateTime"]);
            series.EndDateTime = Convert.ToDateTime(seriesRow["EndDateTime"]);
            series.BeginDateTimeUTC = Convert.ToDateTime(seriesRow["BeginDateTimeUTC"]);
            series.EndDateTimeUTC = Convert.ToDateTime(seriesRow["EndDatetimeUTC"]);
            series.Id = seriesID;
            series.IsCategorical = Convert.ToBoolean(seriesRow["IsCategorical"]);
            series.LastCheckedDateTime = Convert.ToDateTime(seriesRow["LastCheckedDateTime"]);
            series.UpdateDateTime = Convert.ToDateTime(seriesRow["UpdateDateTime"]);
            series.Subscribed = Convert.ToBoolean(seriesRow["Subscribed"]);
            series.ValueCount = Convert.ToInt32(seriesRow["ValueCount"]);

            int siteID = Convert.ToInt32(seriesRow["SiteID"]);

            string sqlSites = "SELECT SiteID, SiteCode, SiteName, Latitude, Longitude, Elevation_m, " +
                "VerticalDatum, LocalX, LocalY, State, County, Comments FROM Sites where SiteID = " + siteID;

            DataTable siteTable = DbOperations.LoadTable("siteTable", sqlSites);
            if (siteTable.Rows.Count == 0) return null;

            DataRow siteRow = siteTable.Rows[0];
            Site newSite = new Site();
            newSite.Id = Convert.ToInt32(siteRow[0]);
            newSite.Code = Convert.ToString(siteRow[1]);
            newSite.Name = Convert.ToString(siteRow[2]);
            newSite.Latitude = Convert.ToDouble(siteRow[3]);
            newSite.Longitude = Convert.ToDouble(siteRow[4]);
            newSite.Elevation_m = Convert.ToDouble(siteRow[5]);
            newSite.VerticalDatum = Convert.ToString(siteRow[6]);
            newSite.LocalX = Convert.ToDouble(siteRow["LocalX"]);
            newSite.LocalY = Convert.ToDouble(siteRow["LocalY"]);
            series.Site = newSite;

            int variableID = Convert.ToInt32(seriesRow["VariableID"]);

            series.Variable = RepositoryFactory.Instance.Get<IVariablesRepository>().GetByID(variableID);

            Method newMethod = new Method();
            newMethod.Id = Convert.ToInt32(seriesRow["MethodID"]);
            series.Method = newMethod;

            Source newSource = new Source();
            newSource.Id = Convert.ToInt32(seriesRow["SourceID"]);
            series.Source = newSource;

            QualityControlLevel newQC = new QualityControlLevel();
            newQC.Id = Convert.ToInt32(seriesRow["QualityControlLevelID"]);
            series.QualityControlLevel = newQC;


            return series;
        }

        public bool ExistsSeries(Site site, Variable variable)
        {
            var query = string.Format("select count(*) from DataSeries where SiteID={0} and VariableID={1}", site.Id,
                                      variable.Id);
            var res = DbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt64(res) > 0;
        }

        public bool DeleteSeries(int seriesID)
        {
            var _db = DbOperations;

            string sqlTheme =
                "SELECT ThemeID from DataThemes where SeriesID = " + seriesID;
            DataTable tblTheme = _db.LoadTable("tblTheme", sqlTheme);

            //if the series belongs to multiple themes, do not delete it.
            if (tblTheme.Rows.Count != 1) return false;

            //otherwise, delete the series
            var seriesToDel = GetSeriesByID(seriesID);

            //SQL Queries
            var sqlSite = "SELECT count(SiteID) from DataSeries where SiteID = " + seriesToDel.Site.Id;
            string sqlVariable = "SELECT count(VariableID) from DataSeries where VariableID = " + seriesToDel.Variable.Id;
            string sqlSource = "SELECT count(SourceID) from DataSeries where SourceID = " + seriesToDel.Source.Id;
            string sqlMethod = "SELECT count(MethodID) from DataSeries where MethodID = " + seriesToDel.Method.Id;
            string sqlQuality = "SELECT count(QualityControlLevelID) from DataSeries where QualityControlLevelID = " + seriesToDel.QualityControlLevel.Id;

            //SQL Delete Commands
            string sqlDeleteValues = "DELETE FROM DataValues WHERE SeriesID = " + seriesID;
            string sqlDeleteSeries = "DELETE FROM DataSeries WHERE SeriesID = " + seriesID;
            string sqlDeleteSeriesTheme = "DELETE FROM DataThemes WHERE SeriesID = " + seriesID;

            string sqlDeleteSite = "DELETE FROM Sites WHERE SiteID = " + seriesToDel.Site.Id;
            string sqlDeleteVariable = "DELETE FROM Variables WHERE VariableID = " + seriesToDel.Variable.Id;
            string sqlDeleteMethod = "DELETE FROM Methods WHERE MethodID = " + seriesToDel.Method.Id;
            string sqlDeleteSource = "DELETE FROM Sources WHERE SourceID = " + seriesToDel.Source.Id;
            string sqlDeleteQuality = "DELETE FROM QualityControlLevels WHERE QualityControlLevelID = " + seriesToDel.QualityControlLevel.Id;

            //Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    //delete the site
                    var sitesCount = Convert.ToInt32(_db.ExecuteSingleOutput(sqlSite));
                    if (sitesCount == 1)
                    {
                        using (DbCommand cmdDeleteSite = conn.CreateCommand())
                        {
                            cmdDeleteSite.CommandText = sqlDeleteSite;
                            cmdDeleteSite.ExecuteNonQuery();
                        }
                    }

                    //delete the variable
                    var variablesCount = Convert.ToInt32(_db.ExecuteSingleOutput(sqlVariable));
                    if (variablesCount == 1)
                    {
                        using (DbCommand cmdDeleteVariable = conn.CreateCommand())
                        {
                            cmdDeleteVariable.CommandText = sqlDeleteVariable;
                            cmdDeleteVariable.ExecuteNonQuery();
                        }
                    }

                    //delete the method
                    var methodsCount = Convert.ToInt32(_db.ExecuteSingleOutput(sqlMethod));
                    if (methodsCount == 1)
                    {
                        using (DbCommand cmdDeleteMethod = conn.CreateCommand())
                        {
                            cmdDeleteMethod.CommandText = sqlDeleteMethod;
                            cmdDeleteMethod.ExecuteNonQuery();
                        }
                    }

                    //delete the source
                    var sourcesCount = Convert.ToInt32(_db.ExecuteSingleOutput(sqlSource));
                    if (sourcesCount == 1)
                    {
                        using (DbCommand cmdDeleteSource = conn.CreateCommand())
                        {
                            cmdDeleteSource.CommandText = sqlDeleteSource;
                            cmdDeleteSource.ExecuteNonQuery();
                        }
                    }

                    //delete the quality control level
                    var qualitiesCount = Convert.ToInt32(_db.ExecuteSingleOutput(sqlQuality));
                    if (qualitiesCount == 1)
                    {
                        using (DbCommand cmdDeleteQuality = conn.CreateCommand())
                        {
                            cmdDeleteQuality.CommandText = sqlDeleteQuality;
                            cmdDeleteQuality.ExecuteNonQuery();
                        }
                    }

                    //delete the data values
                    using (DbCommand cmdDeleteValues = conn.CreateCommand())
                    {
                        cmdDeleteValues.CommandText = sqlDeleteValues;
                        cmdDeleteValues.ExecuteNonQuery();
                    }

                    //finally delete the series
                    using (DbCommand cmdDeleteSeries = conn.CreateCommand())
                    {
                        cmdDeleteSeries.CommandText = sqlDeleteSeries;
                        cmdDeleteSeries.ExecuteNonQuery();
                    }
                    using (DbCommand cmdDeleteSeriesTheme = conn.CreateCommand())
                    {
                        cmdDeleteSeriesTheme.CommandText = sqlDeleteSeriesTheme;
                        cmdDeleteSeriesTheme.ExecuteNonQuery();
                    }

                    //commit transaction
                    tran.Commit();
                }
            }

            //remove seriesID from 'Selection'
            string dele2 = "DELETE from Selection WHERE SeriesID=" + seriesID;
            try
            {
                _db.ExecuteNonQuery(dele2);
            }
            catch { }
            return true;
        }

        public Tuple<DateTime, DateTime> GetDateTimes(long seriesID)
        {
            var begin = Convert.ToDateTime(DbOperations.ExecuteSingleOutput("Select BeginDateTime FROM DataSeries WHERE SeriesID = " + seriesID));
            var end = Convert.ToDateTime(DbOperations.ExecuteSingleOutput("Select EndDateTime FROM DataSeries WHERE SeriesID = " + seriesID));
            return new Tuple<DateTime, DateTime>(begin, end);
        }

        public DataTable GetUnitSiteVarForFirstSeries(long seriesID)
        {
            var sqlQuery = string.Format("SELECT UnitsName, SiteName, VariableName FROM DataSeries " +
                                          "INNER JOIN Variables ON Variables.VariableID = DataSeries.VariableID " +
                                          "INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID " +
                                          "INNER JOIN Sites ON Sites.SiteID = DataSeries.SiteID WHERE SeriesID = {0} limit 1",
                                          seriesID);
            var seriesNameTable = DbOperations.LoadTable("table", sqlQuery);
            return seriesNameTable;
        }

        #endregion

        #region Private methods

        private string DetailedSeriesSQLQuery()
        {
            string sql = "SELECT DataSeries.SeriesID, " +
                "DataThemes.ThemeID, DataSeries.SiteID, DataSeries.VariableID, DataSeries.MethodID, DataSeries.SourceID, DataSeries.QualityControlLevelID, " +
                "SiteName, SiteCode, Latitude, Longitude, " +
                "VariableName, VariableCode, DataType, ValueType, Speciation, SampleMedium, " +
                "TimeSupport, GeneralCategory, NoDataValue, " +
                "units1.UnitsName as 'VariableUnitsName', units2.UnitsName as 'TimeUnitsName', " +
                "MethodDescription, " +
                "SourceDescription, Organization, Citation, " +
                "QualityControlLevelCode, Definition as 'QualityControlLevelDefinition', " +
                "BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, ThemeName " +
                "FROM DataSeries " +
                "LEFT JOIN DataThemes ON DataThemes.SeriesID = DataSeries.SeriesID " +
                "LEFT JOIN DataThemeDescriptions ON DataThemes.ThemeID = DataThemeDescriptions.ThemeID " +
                "LEFT JOIN Sites ON DataSeries.SiteID = Sites.SiteID " +
                "LEFT JOIN Variables ON DataSeries.VariableID = Variables.VariableID " +
                "LEFT JOIN Units units1 ON Variables.VariableUnitsID = units1.UnitsID " +
                "LEFT JOIN Units units2 ON Variables.TimeUnitsID = units2.UnitsID " +
                "LEFT JOIN Methods  ON DataSeries.MethodID = Methods.MethodID " +
                "LEFT JOIN Sources ON DataSeries.SourceID = Sources.SourceID " +
                "LEFT JOIN QualityControlLevels ON DataSeries.QualityControlLevelID = QualityControlLevels.QualityControlLevelID ";

            return sql;
        }

        private IList<Series> SeriesListFromTable(DataTable seriesTable)
        {
            return (from DataRow row in seriesTable.Rows select CreateSeriesFromSeriesRow(row)).ToList();
        }

        private Series CreateSeriesFromSeriesRow(DataRow row)
        {
            Site st = new Site();
            st.Id = (long)row["SiteID"];
            st.Latitude = (double)row["Latitude"];
            st.Longitude = (double)row["Longitude"];
            st.Name = (string)row["SiteName"];
            st.Code = (string)row["SiteCode"];

            Unit timeUnit = Unit.UnknownTimeUnit;
            timeUnit.Name = (string)row["TimeUnitsName"];

            Unit variableUnit = Unit.Unknown;
            variableUnit.Abbreviation = (string)row["VariableUnitsName"];

            Variable v = new Variable();
            v.Id = (long)row["VariableID"];
            v.Name = (string)row["VariableName"];
            v.Code = (string)row["VariableCode"];
            v.DataType = (string)row["DataType"];
            v.ValueType = (string)row["ValueType"];
            v.Speciation = row["Speciation"] == DBNull.Value ? null : (string)row["Speciation"];
            v.SampleMedium = row["SampleMedium"] == DBNull.Value ? null : (string)row["SampleMedium"];
            v.TimeSupport = row["TimeSupport"] == DBNull.Value ? 0 : (double)row["TimeSupport"];
            v.VariableUnit = variableUnit;
            v.TimeUnit = timeUnit;
            v.GeneralCategory = row["GeneralCategory"] == DBNull.Value ? null : (string)row["GeneralCategory"];
            v.NoDataValue = (double)row["NoDataValue"];

            Method m = new Method();
            m.Description = (string)row["MethodDescription"];

            Source src = Source.Unknown;
            src.Description = (string)row["SourceDescription"];
            src.Citation = (string)row["Citation"];
            src.Organization = (string)row["Organization"];

            QualityControlLevel qc = QualityControlLevel.Unknown;
            qc.Code = (string)row["QualityControlLevelCode"];
            qc.Definition = (string)row["QualityControlLevelDefinition"];

            Series ser = new Series(st, v, m, qc, src);
            ser.BeginDateTime = Convert.ToDateTime(row["BeginDateTime"]);
            ser.EndDateTime = Convert.ToDateTime(row["EndDateTime"]);
            ser.BeginDateTimeUTC = Convert.ToDateTime(row["BeginDateTimeUTC"]);
            ser.EndDateTimeUTC = Convert.ToDateTime(row["EndDateTimeUTC"]);
            ser.ValueCount = Convert.ToInt32(row["ValueCount"]);
            ser.Id = (long)row["SeriesID"];

            return ser;
        }

        #endregion

        public override string TableName
        {
            get { return "DataSeries"; }
        }
    }
}