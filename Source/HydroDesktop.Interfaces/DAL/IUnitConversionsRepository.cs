using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for <see cref="UnitConversion"/> Repository
    /// </summary>
    public interface IUnitConversionsRepository : IRepository<UnitConversion>
    {
        /// <summary>
        /// Check that exists direct conversion from unitA to unitB
        /// </summary>
        /// <param name="unitA">UnitA (source)</param>
        /// <param name="unitB">UnitB (dest)</param>
        /// <returns>True - exists direct convrsion from unitA to unitB, otherwise - False.</returns>
        bool ExistsConversion(Unit unitA, Unit unitB);

        /// <summary>
        /// Get UnitConversion from unitA to unitB
        /// </summary>
        /// <param name="unitA">UnitA (source)</param>
        /// <param name="unitB">UnitB (dest)</param>
        /// <returns>UnitConversion from unitA to unitB, or null, if it not exists.</returns>
        UnitConversion GetConversion(Unit unitA, Unit unitB);
    }
}