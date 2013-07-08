using System;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.DAL;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class SourcesRepository : BaseRepository<Source>, ISourcesRepository
    {
        #region Fields

        private readonly IISOMetadataRepository _iisoMetadataRepository;

        #endregion

        #region Constructors

        public SourcesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
            _iisoMetadataRepository = new ISOMetadataRepository(dbType, connectionString);
        }

        public SourcesRepository(IHydroDbOperations db) : base(db)
        {
            _iisoMetadataRepository = new ISOMetadataRepository(db);
        }

        #endregion

        protected override Source DataRowToEntity(System.Data.DataRow row)
        {
            var result = new Source
                             {
                                 Id = Convert.ToInt64(row["SourceId"]),
                                 Address = Convert.ToString(row["Address"]),
                                 Citation = Convert.ToString(row["Citation"]),
                                 City = Convert.ToString(row["City"]),
                                 ContactName = Convert.ToString(row["ContactName"]),
                                 Description = Convert.ToString(row["SourceDescription"]),
                                 Email = Convert.ToString(row["Email"]),
                                 Link = Convert.ToString(row["SourceLink"]),
                                 State = Convert.ToString(row["State"]),
                                 Organization = Convert.ToString(row["Organization"]),
                                 Phone = Convert.ToString(row["Phone"]),
                                 ZipCode = Convert.ToInt32(row["ZipCode"]),
                                 ISOMetadata = _iisoMetadataRepository.GetByKey(row["MetadataID"]),
                             };

            return result;
        }

        protected override string TableName
        {
            get { return "Sources"; }
        }

        protected override string PrimaryKeyName
        {
            get { return "SourceID"; }
        }
    }
}
