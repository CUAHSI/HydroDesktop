using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
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
        /// Deletes a theme and all its series as long as the series don't belong to any other theme.
        /// </summary>
        /// <param name="themeID">The Theme ID</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool DeleteTheme(int themeID)
        {
            string sqlTheme = "SELECT SeriesID FROM DataThemes where ThemeID = " + themeID;
            DataTable tblSeries = DbOperations.LoadTable("tblSeries", sqlTheme);

            foreach (DataRow seriesRow in tblSeries.Rows)
            {
                int seriesID = Convert.ToInt32(seriesRow["SeriesID"]);

                var seriesRepository = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
                seriesRepository.DeleteSeries(seriesID);
            }

            //delete the actual theme
            string sqlDeleteTheme = "DELETE FROM DataThemeDescriptions WHERE ThemeID = " + themeID;
            try
            {
                DbOperations.ExecuteNonQuery(sqlDeleteTheme);
            }
            catch { }

            //re-check the number of series in the theme

            return true;
        }

        /// <summary>
        /// Delete a theme - a background worker and progress bar is used
        /// </summary>
        /// <param name="themeID">The themeID (this needs to be a valid ID)</param>
        /// <param name="worker">The background worker component</param>
        /// <param name="e">The arguments for background worker</param>
        /// <returns></returns>
        public bool DeleteTheme(int themeID, BackgroundWorker worker, DoWorkEventArgs e)
        {
            string sqlTheme = "SELECT SeriesID FROM DataThemes where ThemeID = " + themeID;
            DataTable tblSeries = DbOperations.LoadTable("tblSeries", sqlTheme);

            int numSeries = tblSeries.Rows.Count;
            int count = 0;

            if (numSeries == 0)
            {
                return false;
            }

            foreach (DataRow seriesRow in tblSeries.Rows)
            {
                if (worker != null)
                {
                    //check cancellation
                    if (e != null && worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return false;
                    }
                }

                int seriesID = Convert.ToInt32(seriesRow["SeriesID"]);

                var seriesRepository = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
                seriesRepository.DeleteSeries(seriesID);

                //progress report
                count++;

                if (worker != null && worker.WorkerReportsProgress)
                {
                    var percent = (int)((count / (float)numSeries) * 100);
                    var userState = "Deleting series " + count + " of " + numSeries + "...";
                    worker.ReportProgress(percent, userState);
                }
            }

            //delete the actual theme

            string sqlDeleteTheme = "DELETE FROM DataThemeDescriptions WHERE ThemeID = " + themeID;
            try
            {
                DbOperations.ExecuteNonQuery(sqlDeleteTheme);
                e.Result = "Theme deleted successfully";
            }
            catch { }

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

        public void AddTheme(string themeName, List<long> series)
        {
            var str = "Insert into DataThemeDescriptions(ThemeName) Values(@p0)" + LastRowIDSelect;
            var strId = DbOperations.ExecuteSingleOutput(str, new object[]
                                                                 {
                                                                   themeName
                                                                 });
            var themeId = Convert.ToInt64(strId);
            var parameters = new object[series.Count + 1];
            parameters[0] = themeId;

            var query = new StringBuilder();
            for (int i = 0; i < series.Count; i++)
            {
                parameters[i + 1] = series[i];
                query.AppendFormat("Insert into DataThemes(ThemeID, SeriesID) Values(@p0, @p{0});", i + 1);
                query.AppendLine();
            }

            DbOperations.ExecuteNonQuery(query.ToString(), parameters);
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