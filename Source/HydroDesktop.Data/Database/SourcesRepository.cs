using System.Data;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    class SourcesRepository : BaseRepository, ISourcesRepository
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
