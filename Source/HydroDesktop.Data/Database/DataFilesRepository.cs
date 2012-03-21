using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class DataFilesRepository : BaseRepository<DataFile>, IDataFilesRepository
    {
        public DataFilesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public DataFilesRepository(IHydroDbOperations db) : base(db)
        {
        }

        public override string TableName
        {
            get { return "DataFiles"; }
        }
    }
}