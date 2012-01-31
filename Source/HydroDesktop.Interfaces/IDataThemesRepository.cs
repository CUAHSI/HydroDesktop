using System.Data;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for DataThemes Repository
    /// </summary>
    public interface IDataThemesRepository
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
    }
}