using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for <see cref="SeriesProvenance"/> Repository
    /// </summary>
    public interface ISeriesProvenanceRepository : IRepository<SeriesProvenance>
    {
        void AddNew(SeriesProvenance entity);
    }
}