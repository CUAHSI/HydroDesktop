using System;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for <see cref="OffsetType"/>
    /// </summary>
    class OffsetTypesRepository : BaseRepository<OffsetType>, IOffsetTypesRepository
    {
        private readonly IUnitsRepository _unitsRepository = RepositoryFactory.Instance.Get<IUnitsRepository>();

        #region Constructors

        public OffsetTypesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public OffsetTypesRepository(IHydroDbOperations db) : base(db)
        {
        }

        #endregion

        public override string TableName
        {
            get { return "OffsetTypes"; }
        }

        protected override OffsetType DataRowToEntity(DataRow row)
        {
            var entity = new OffsetType
                             {
                                 Id = Convert.ToInt64(row["OffsetTypeID"]),
                                 Description = Convert.ToString(row["OffsetDescription"]),
                                 Unit = _unitsRepository.GetByKey(Convert.ToInt64(row["OffsetUnitsID"]))
                             };
            return entity;
        }
    }
}