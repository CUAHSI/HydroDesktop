using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for <see cref="Site"/> repository
    /// </summary>
    public interface ISitesRepository
    {
        /// <summary>
        /// Get all sites.
        /// </summary>
        /// <returns>All sites.</returns>
        Site[] GetAll();

        /// <summary>
        /// Check that site already exists in the database
        /// </summary>
        /// <param name="site">Site to check.</param>
        /// <returns>True - if site exists, False - otherwise.</returns>
        bool Exists(Site site);
    }
}
