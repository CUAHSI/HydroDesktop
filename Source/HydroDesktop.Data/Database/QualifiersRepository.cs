using System;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class QualifiersRepository : BaseRepository<Qualifier>, IQualifiersRepository
    {
        public QualifiersRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public QualifiersRepository(IHydroDbOperations db) : base(db)
        {
        }

        public override string TableName
        {
            get { return "Qualifiers"; }
        }

        protected override Qualifier DataRowToEntity(DataRow row)
        {
            var entity = new Qualifier
                             {
                                 Id = Convert.ToInt64(row["QualifierID"]),
                                 Description = Convert.ToString("QualifierDescription"),
                                 Code = Convert.ToString("QualifierCode"),
                             };
            return entity;
        }
    }
}