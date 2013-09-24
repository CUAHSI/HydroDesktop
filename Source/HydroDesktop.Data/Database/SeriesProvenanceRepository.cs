using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for <see cref="SeriesProvenance"/>
    /// </summary>
    class SeriesProvenanceRepository : BaseRepository<SeriesProvenance>, ISeriesProvenanceRepository
    {
        public SeriesProvenanceRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public SeriesProvenanceRepository(IHydroDbOperations db) : base(db)
        {
        }

        public void AddNew(SeriesProvenance entity)
        {
            entity.Id = DbOperations.GetNextID("SeriesProvenance", "ProvenanceID");
        const string query = "INSERT INTO SeriesProvenance(ProvenanceID, ProvenanceDateTime, InputSeriesID, OutputSeriesID, MethodID, Comment) VALUES (?, ?,?,?,?,?)";
            DbOperations.ExecuteNonQuery(query, new object[]
                                                                  {
                                                                          entity.Id,
                                                                          entity.ProvenanceDateTime,
                                                                          entity.InputSeries.Id,
                                                                          entity.OutputSeries.Id,
                                                                          entity.Method.Id,
                                                                          entity.Comment
                                                                  });
        }

        protected override string TableName
        {
            get { return "SeriesProvenance"; }
        }
    }
}