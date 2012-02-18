using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for Units Repository
    /// </summary>
    public interface IUnitsRepository
    {
        /// <summary>
        /// Get <see cref="Unit"/> by ID
        /// </summary>
        /// <param name="id">UnitID</param>
        /// <returns>Instance of <see cref="Unit"/> or null, if entity not found.</returns>
        Unit GetByID(long id);

        /// <summary>
        /// Get all units.
        /// </summary>
        /// <returns>All units.</returns>
        Unit[] GetAll();
    }
}