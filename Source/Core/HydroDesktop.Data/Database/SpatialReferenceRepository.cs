using System;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.DAL;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for <see cref="SpatialReference"/>
    /// </summary>
    class SpatialReferenceRepository : BaseRepository<SpatialReference>, ISpatialReferenceRepository
    {
        #region Constructors

        public SpatialReferenceRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public SpatialReferenceRepository(IHydroDbOperations db) : base(db)
        {
        }

        #endregion

        #region Overrides of BaseRepository<SpatialReference>

        protected override SpatialReference DataRowToEntity(DataRow row)
        {
            var result = new SpatialReference
                             {
                                 Id = Convert.ToInt64(row["SpatialReferenceID"]),
                                 Notes = Convert.ToString(row["Notes"]),
                                 SRSID = Convert.ToInt32(row["SRSID"]),
                                 SRSName = Convert.ToString(row["SRSName"]),
                             };
            return result;
        }

        protected override string TableName
        {
            get { return "SpatialReferences"; }
        }

        protected override string PrimaryKeyName
        {
            get { return "SpatialReferenceID"; }
        }

        #endregion
    }
}