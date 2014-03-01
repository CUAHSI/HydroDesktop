using System;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    class QualifiersRepository : BaseRepository<Qualifier>, IQualifiersRepository
    {
        #region Contsructors

        public QualifiersRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public QualifiersRepository(IHydroDbOperations db) : base(db)
        {
        }

        #endregion

        public Qualifier FindByCodeOrCreate(string qualifierCode)
        {
            var entity = FindByCode(qualifierCode);
            if (entity == null)
            {
                entity = new Qualifier {Code = qualifierCode, Description = string.Empty};
                AddQualifier(entity);
            }

            return entity;
        }

        public Qualifier FindByCode(string qualifierCode)
        {
            var res = DbOperations.LoadTable(string.Format("select * from {0} where QualifierCode='{1}'", TableName,
                                                           qualifierCode));
            if (res.Rows.Count == 0)
                return null;
            return DataRowToEntity(res.Rows[0]);
        }

        public void Update(Qualifier entity)
        {
            var query = "UPDATE Qualifiers SET QualifierCode = ?, QualifierDescription = ? WHERE QualifierID = " +
                        entity.Id;
            DbOperations.ExecuteNonQuery(query, new object[] {entity.Code, entity.Description});
        }

        public void AddQualifier(Qualifier entity)
        {
            var query = "INSERT INTO Qualifiers(QualifierCode, QualifierDescription) VALUES (?, ?)"
                        + LastRowIDSelect;
            var id = DbOperations.ExecuteSingleOutput(query, new object[]
                                                                 {
                                                                     entity.Code,
                                                                     entity.Description
                                                                 });
            entity.Id = Convert.ToInt64(id);
        }

        protected override string TableName
        {
            get { return "Qualifiers"; }
        }

        protected override string PrimaryKeyName
        {
            get { return "QualifierID"; }
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