using System.ComponentModel;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for DataThemes Repository
    /// </summary>
    public interface IDataThemesRepository : IRepository<Theme>
    {
        /// <summary>
        /// Get themes for all series.
        /// If series do not belongs any theme, than "Other Data Series" theme added to result DataTable
        /// </summary>
        /// <returns>DataTable with themes</returns>
        DataTable GetThemesForAllSeries();

        /// <summary>
        /// Get DataTable which contains some specific columns for ThemeManager
        /// </summary>
        /// <returns>DataTable with data.</returns>
        DataTable GetThemesTableForThemeManager(long? themeId);

        /// <summary>
        /// Get Theme ID by Theme Name
        /// </summary>
        /// <param name="themeName">Theme name</param>
        /// <returns>Theme ID or null, if themeID not found.</returns>
        int? GetID(string themeName);

        /// <summary>
        /// Delete a theme - a background worker and progress bar is used
        /// </summary>
        /// <param name="themeID">The themeID (this needs to be a valid ID)</param>
        /// <param name="worker">The background worker component</param>
        /// <returns>True if the theme was successfully deleted</returns>
        bool DeleteTheme(long themeID, BackgroundWorker worker = null);

        void InsertNewTheme(long seriesID, long newSeriesID);
    }
}