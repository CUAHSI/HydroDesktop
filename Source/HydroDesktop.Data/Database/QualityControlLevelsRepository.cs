using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class QualityControlLevelsRepository : BaseRepository<QualityControlLevel>, IQualityControlLevelsRepository
    {
        #region Constructors

        public QualityControlLevelsRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public QualityControlLevelsRepository(IHydroDbOperations db) : base(db)
        {
        }

        #endregion

        public override string TableName
        {
            get { return "QualityControlLevels"; }
        }
    }
}