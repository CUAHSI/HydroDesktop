using System;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for Variables
    /// </summary>
    class VariablesRepository : BaseRepository<Variable>, IVariablesRepository
    {
        #region Constructors

        public VariablesRepository(DatabaseTypes dbType, string connectionString)
            : base(dbType, connectionString)
        {
        }

        public VariablesRepository(IHydroDbOperations db)
            : base(db)
        {
        }

        #endregion

        #region Public methods
        
        public void AddVariable(Variable variable)
        {
            var query =
                string.Format(
                    @"INSERT INTO {0}(VariableCode, VariableName, Speciation, SampleMedium, ValueType, IsRegular, IsCategorical, TimeSupport, DataType, GeneralCategory, NoDataValue, TimeUnitsID, VariableUnitsID)
                                       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    TableName) + LastRowIDSelect;
            var res = DbOperations.ExecuteSingleOutput(query,
                                         new object[]
                                             {
                                                     variable.Code, variable.Name, variable.Speciation,
                                                     variable.SampleMedium,
                                                     variable.ValueType, Convert.ToInt32(variable.IsRegular),
                                                     Convert.ToInt32(variable.IsCategorical), variable.TimeSupport,
                                                     variable.DataType, variable.GeneralCategory, variable.NoDataValue,
                                                     variable.TimeUnit != null ? variable.TimeUnit.Id : 0,
                                                     variable.VariableUnit != null ? variable.VariableUnit.Id : 0
                                             });
            variable.Id = Convert.ToInt64(res);
        }
        
        public void Update(Variable variable)
        {
            var query =
                string.Format(
                    @"UPDATE {0} SET VariableCode=?, VariableName=?, Speciation=?, SampleMedium=?, ValueType=?, IsRegular=?, IsCategorical=?, TimeSupport=?, DataType=?, GeneralCategory=?, NoDataValue=?, TimeUnitsID=?, VariableUnitsID=?
                    WHERE VariableID=?",
                    TableName);
            DbOperations.ExecuteNonQuery(query, new object[]
                                                    {
                                                            variable.Code, variable.Name, variable.Speciation,
                                                            variable.SampleMedium,
                                                            variable.ValueType, Convert.ToInt32(variable.IsRegular),
                                                            Convert.ToInt32(variable.IsCategorical),
                                                            variable.TimeSupport,
                                                            variable.DataType, variable.GeneralCategory,
                                                            variable.NoDataValue,
                                                            variable.TimeUnit.Id,
                                                            variable.VariableUnit.Id, variable.Id
                                                    });
        }

        public bool Exists(Variable entity)
        {
            if (entity == null) return false;

            const string query = "select count(*) from {0} where VariableID = {1} and VariableCode = '{2}'";
            var result = DbOperations.ExecuteSingleOutput(string.Format(query, TableName, entity.Id, entity.Code));
            return Convert.ToInt32(result) > 0;
        }

        #endregion

        protected override Variable DataRowToEntity(DataRow row)
        {
            var unitsRepo = RepositoryFactory.Instance.Get<IUnitsRepository>();
            var res = new Variable
                          {
                              Id = Convert.ToInt64(row["VariableID"]),
                              Code = Convert.ToString(row["VariableCode"]),
                              Name = Convert.ToString(row["VariableName"]),
                              Speciation = Convert.ToString(row["Speciation"]),
                              SampleMedium = Convert.ToString(row["SampleMedium"]),
                              ValueType = Convert.ToString(row["ValueType"]),
                              IsRegular = Convert.ToBoolean(row["IsRegular"]),
                              IsCategorical = Convert.ToBoolean(row["IsCategorical"]),
                              TimeSupport = Convert.ToSingle(row["TimeSupport"]),
                              DataType = Convert.ToString(row["DataType"]),
                              GeneralCategory = Convert.ToString(row["GeneralCategory"]),
                              NoDataValue = Convert.ToDouble(row["NoDataValue"]),
                              TimeUnit = unitsRepo.GetByKey(row["TimeUnitsID"]),
                              VariableUnit = unitsRepo.GetByKey(row["VariableUnitsID"]),
                          };
            return res;
        }

        protected override string PrimaryKeyName
        {
            get
            {
                return "VariableID";
            }
        }

        protected override string TableName
        {
            get { return "Variables"; }
        }
    }
}