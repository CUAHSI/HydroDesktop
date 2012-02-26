using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for DataThemes Repository
    /// </summary>
    public interface IDataThemesRepository : IRepository
    {
        /// <summary>
        /// Get themes for all series.
        /// If series do not belongs any theme, than "Other Data Series" theme added to result DataTable
        /// </summary>
        /// <returns>DataTable with themes</returns>
        DataTable GetThemesForAllSeries();

        /// <summary>
        /// Get Theme ID by Theme Name
        /// </summary>
        /// <param name="themeName">Theme name</param>
        /// <returns>Theme ID or null, if themeID not found.</returns>
        int? GetID(string themeName);

        /// <summary>
        /// Deletes a theme and all its series as long as the series don't belong to any other theme.
        /// </summary>
        /// <param name="themeID">The Theme ID</param>
        /// <returns>true if successful, false otherwise</returns>
        bool DeleteTheme(int themeID);


        /// <summary>
        /// Delete a theme - a background worker and progress bar is used
        /// </summary>
        /// <param name="themeID">The themeID (this needs to be a valid ID)</param>
        /// <param name="worker">The background worker component</param>
        /// <param name="e">The arguments for background worker</param>
        /// <returns>true if the theme was successfully deleted</returns>
        bool DeleteTheme(int themeID, BackgroundWorker worker, DoWorkEventArgs e);

        /// <summary>
        /// Gets all themes from the database ordered by the theme name
        /// </summary>
        /// <returns>The list of all themes</returns>
        IList<Theme> GetAllThemes();
    }
}