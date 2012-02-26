using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class SamplesRepository : BaseRepository<Sample>, ISamplesRepository
    {
        public SamplesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public SamplesRepository(IHydroDbOperations db) : base(db)
        {
        }

        public override string TableName
        {
            get { return "Samples"; }
        }
    }
}