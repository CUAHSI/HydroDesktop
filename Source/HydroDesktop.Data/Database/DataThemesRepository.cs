using System;
using System.Data;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for DataThemes
    /// </summary>
    class DataThemesRepository : BaseRepository, IDataThemesRepository
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
      
        public int? GetID(string themeName)
        {
            const string sql = "SELECT ThemeID from DataThemeDescriptions WHERE ThemeName =?";
            var objThemeId = DbOperations.ExecuteSingleOutput(sql, new[] { themeName });
            if (objThemeId == null || objThemeId == DBNull.Value) return null;
            return Convert.ToInt32(objThemeId);
        }

        #endregion

        public override string TableName
        {
            get { return "DataThemes"; }
        }
    }
}