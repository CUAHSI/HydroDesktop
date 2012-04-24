using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for Units Repository
    /// </summary>
    public interface IUnitsRepository : IRepository<Unit>
    {
        Unit GetByName(string name);
        bool Exists(string name);
        void AddUnit(Unit unit);
    }
}