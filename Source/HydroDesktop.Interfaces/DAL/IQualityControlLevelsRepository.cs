using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for <see cref="QualityControlLevel"/> Repository
    /// </summary>
    public interface IQualityControlLevelsRepository : IRepository<QualityControlLevel>
    {
        void AddNew(QualityControlLevel entity);
        void Update(QualityControlLevel entity);
    }
}