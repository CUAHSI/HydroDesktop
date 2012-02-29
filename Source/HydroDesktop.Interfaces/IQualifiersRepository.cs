using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for <see cref="Qualifier"/> Repository
    /// </summary>
    public interface IQualifiersRepository : IRepository<Qualifier>
    {
        Qualifier FindByCode(string qualifierCode);
        Qualifier FindByCodeOrCreate(string qualifierCode);
        void AddQualifier(Qualifier entity);
        void Update(Qualifier entity);
    }
}