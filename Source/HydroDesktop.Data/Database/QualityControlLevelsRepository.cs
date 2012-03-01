using System;
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

        public void AddNew(QualityControlLevel entity)
        {
            var query =
                    "INSERT INTO QualityControlLevels(QualityControlLevelCode, Definition, Explanation) VALUES (?, ?, ?)" +
                    LastRowIDSelect;
            var id = DbOperations.ExecuteSingleOutput(query, new object[]
                                                        {
                                                                entity.Code, entity.Definition, entity.Explanation
                                                        });
            entity.Id = Convert.ToInt64(id);
        }

        public void Update(QualityControlLevel entity)
        {
            const string query = "UPDATE QualityControlLevels SET QualityControlLevelCode=?, Definition=?, Explanation=? WHERE QualityControlLevelID = ?";
            DbOperations.ExecuteNonQuery(query, new object[]
                                                        {
                                                                entity.Code, entity.Definition, entity.Explanation,
                                                                entity.Id
                                                        });
        }

        protected override QualityControlLevel DataRowToEntity(System.Data.DataRow row)
        {
            var res = new QualityControlLevel
                          {
                              Id = Convert.ToInt64(row["QualityControlLevelID"]),
                              Code = Convert.ToString(row["QualityControlLevelCode"]),
                              Definition = Convert.ToString(row["Definition"]),
                              Explanation = Convert.ToString(row["Explanation"]),
                          };
            return res;
        }

        public override string PrimaryKeyName
        {
            get
            {
                return "QualityControlLevelID";
            }
        }

        public override string TableName
        {
            get { return "QualityControlLevels"; }
        }
    }
}