using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class SourcesRepository : BaseRepository<Source>, ISourcesRepository
    {
        #region Constructors

        public SourcesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public SourcesRepository(IHydroDbOperations db) : base(db)
        {
            
        }

        #endregion

        public override string TableName
        {
            get { return "Sources"; }
        }
    }
}
