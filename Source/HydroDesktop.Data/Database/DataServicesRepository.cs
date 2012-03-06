using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class DataServicesRepository: BaseRepository<DataServiceInfo>, IDataServicesRepository
    {
        public DataServicesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public DataServicesRepository(IHydroDbOperations db) : base(db)
        {
        }

        public override string TableName
        {
            get { return "DataServices"; }
        }
    }
}