using System;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.DAL;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for <see cref="ISOMetadata"/>
    /// </summary>
    class ISOMetadataRepository : BaseRepository<ISOMetadata>, IISOMetadataRepository
    {
        public ISOMetadataRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public ISOMetadataRepository(IHydroDbOperations db) : base(db)
        {
        }

        #region Overrides of BaseRepository<ISOMetadata>

        protected override ISOMetadata DataRowToEntity(System.Data.DataRow row)
        {
            var result = new ISOMetadata
                             {
                                 Id = Convert.ToInt64(row["MetadataID"]),
                                 Abstract = Convert.ToString(row["Abstract"]),
                                 MetadataLink = Convert.ToString(row["MetadataLink"]),
                                 ProfileVersion = Convert.ToString(row["ProfileVersion"]),
                                 Title = Convert.ToString(row["Title"]),
                                 TopicCategory = Convert.ToString(row["TopicCategory"]),
                             };
            return result;
        }

        protected override string TableName
        {
            get { return "ISOMetadata"; }
        }

        protected override string PrimaryKeyName
        {
            get { return "MetadataID"; }
        }

        #endregion
    }
}