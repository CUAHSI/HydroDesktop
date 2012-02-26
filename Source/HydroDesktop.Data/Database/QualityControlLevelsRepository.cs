using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    class QualityControlLevelsRepository : BaseRepository, IQualityControlLevelsRepository
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