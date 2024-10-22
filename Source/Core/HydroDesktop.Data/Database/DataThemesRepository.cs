using System;
using System.ComponentModel;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for DataThemes
    /// </summary>
    class DataThemesRepository : BaseRepository<Theme>, IDataThemesRepository
    {
        #region Constants

        private const string OTHER_DATA_SERIES = "Other Data Series";

        #endregion

        #region Constructors

        public DataThemesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public DataThemesRepository(IHydroDbOperations db)
            : base(db)
        {
        }

        #endregion

        #region Public methods

        public DataTable GetThemesForAllSeries()
        {
            var dtThemes = DbOperations.LoadTable(TableName, "SELECT ThemeID, ThemeName from DataThemeDescriptions");
            if (Int32.Parse(DbOperations.ExecuteSingleOutput("Select count(*) from DataSeries " +
                                                             "Where SeriesID not in (Select SeriesID from DataThemes)").
                                ToString()) > 0)
            {
                dtThemes.Rows.Add(DBNull.Value, OTHER_DATA_SERIES);
            }
            return dtThemes;
        }

        public DataTable GetThemesTableForThemeManager(long? themeID)
        {
            var sql =
                "SELECT src.Organization as 'DataSource', ds.SeriesID, " +
                "s.SiteName as 'SiteName', s.Latitude as 'Latitude', s.Longitude as 'Longitude', s.SiteCode as 'SiteCode', " +
                "v.VariableName as 'VarName', v.DataType as 'DataType', v.SampleMedium as 'SampleMed', " +
                "v.VariableCode as 'VarCode', u.UnitsName as 'Units', " +
                "v.VariableCode as 'ServiceCode', " +
                "m.MethodDescription as 'Method', qc.Definition as 'QCLevel', " +
                "ds.BeginDateTime as 'StartDate', ds.EndDateTime as 'EndDate', ds.ValueCount as 'ValueCount', " +
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

        public int? GetID(string themeName)
        {
            const string sql = "SELECT ThemeID from DataThemeDescriptions WHERE ThemeName =?";
            var objThemeId = DbOperations.ExecuteSingleOutput(sql, themeName);
            if (objThemeId == null || objThemeId == DBNull.Value) return null;
            return Convert.ToInt32(objThemeId);
        }

        /// <summary>
        /// Delete a theme - a background worker and progress bar is used
        /// </summary>
        /// <param name="themeID">The themeID (this needs to be a valid ID)</param>
        /// <param name="worker">The background worker component</param>
        /// <returns>True - on success, otherwise false.</returns>
        public bool DeleteTheme(long themeID, BackgroundWorker worker = null)
        {
            var sqlTheme = "SELECT SeriesID FROM DataThemes where ThemeID = " + themeID;
            var sqlDeleteTheme = "DELETE FROM DataThemeDescriptions WHERE ThemeID = " + themeID;
            var sqlDeleteTheme2 = "DELETE FROM DataThemes WHERE ThemeID = " + themeID;

            var tblSeries = DbOperations.LoadTable("tblSeries", sqlTheme);
            var seriesRepository = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
            for (var i = 0; i < tblSeries.Rows.Count; i++)
            {
                // Check cancellation
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }

                var seriesRow = tblSeries.Rows[i];
                var seriesID = Convert.ToInt32(seriesRow["SeriesID"]);
                seriesRepository.DeleteSeries(seriesID, themeID);

                // Progress report
                if (worker != null && worker.WorkerReportsProgress)
                {
                    var percent = (int)(((i + 1) / (float)tblSeries.Rows.Count) * 100);
                    var userState = "Deleting series " + (i + 1) + " of " + tblSeries.Rows.Count + "...";
                    worker.ReportProgress(percent, userState);
                }
            }

            // Delete the actual theme
            DbOperations.ExecuteNonQuery(sqlDeleteTheme2);
            DbOperations.ExecuteNonQuery(sqlDeleteTheme);

            return true;
        }

        public void InsertNewTheme(long seriesID, long newSeriesID)
        {
            var SQLstring = "SELECT ThemeID FROM DataThemes WHERE SeriesID = " + seriesID;
            var ThemeID = Convert.ToInt64(DbOperations.ExecuteSingleOutput(SQLstring));

            SQLstring = "INSERT INTO DataThemes(ThemeID, SeriesID) VALUES (";
            SQLstring += ThemeID + "," + newSeriesID + ")";

            DbOperations.ExecuteNonQuery(SQLstring);
        }

        #endregion

        protected override Theme DataRowToEntity(DataRow row)
        {
            var newTheme = new Theme
            {
                Name = Convert.ToString(row["ThemeName"]),
                Description = Convert.ToString(row["ThemeDescription"]),
                Id = Convert.ToInt64(row["ThemeID"]),
                DateCreated = row["DateCreated"] != DBNull.Value? Convert.ToDateTime(row["DateCreated"]) : DateTime.MinValue,
            };
            return newTheme;
        }

        protected override string TableName
        {
            get { return "DataThemeDescriptions"; }
        }
    }
}