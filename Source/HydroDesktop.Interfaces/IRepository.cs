using System.Data;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Common interface for all repositories
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Get all data from current repository as DataTable
        /// </summary>
        /// <returns>DataTable with all data.</returns>
        DataTable AsDataTable();
    }
}