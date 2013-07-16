using System.Collections.Generic;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for <see cref="Site"/> repository
    /// </summary>
    public interface ISitesRepository : IRepository<Site>
    {
        /// <summary>
        /// Check that site already exists in the database
        /// </summary>
        /// <param name="site">Site to check.</param>
        /// <returns>True - if site exists, False - otherwise.</returns>
        bool Exists(Site site);
       

        /// <summary>
        /// Gets the site objects that have both variables.
        /// </summary>
        /// <param name="variable1">the first variable</param>
        /// <param name="variable2">the second variable</param>
        /// <returns></returns>
        IList<Site> GetSitesWithBothVariables(Variable variable1, Variable variable2);
    }
}
