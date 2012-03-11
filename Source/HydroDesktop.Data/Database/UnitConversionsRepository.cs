using System;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for <see cref="UnitConversion"/>
    /// </summary>
    class UnitConversionsRepository : BaseRepository<UnitConversion>, IUnitConversionsRepository
    {
        #region Fields

        private readonly IUnitsRepository _unitsRepository = RepositoryFactory.Instance.Get<IUnitsRepository>();

        #endregion

        #region Constructors

        public UnitConversionsRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public UnitConversionsRepository(IHydroDbOperations db) : base(db)
        {
        }

        #endregion

        #region Overrides of BaseRepository<UnitConversion>

        public override string TableName
        {
            get { return "UnitConversions"; }
        }

        public override string PrimaryKeyName
        {
            get { return "ConversionID"; }
        }

        protected override UnitConversion DataRowToEntity(System.Data.DataRow row)
        {
            var entity = new UnitConversion
                             {
                                 Id = Convert.ToInt64(row["ConversionID"]),
                                 FromUnit = _unitsRepository.GetByKey(Convert.ToInt64(row["FromUnitsID"])),
                                 ToUnit = _unitsRepository.GetByKey(Convert.ToInt64(row["ToUnitsID"])),
                                 ConversionFactor = Convert.ToDouble(row["ConversionFactor"]),
                                 Offset = Convert.ToDouble(row["Offset"]),
                             };
            return entity;
        }

        #endregion
    }
}