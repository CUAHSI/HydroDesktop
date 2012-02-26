using System;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for Variables
    /// </summary>
    class VariablesRepository : BaseRepository, IVariablesRepository
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

        public Variable[] GetAll()
        {
            var table = AsDataTable();
            var result = new Variable[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var variable = DataRowToVariable(row);
                result[i] = variable;
            }

            return result;
        }

        public Variable GetByID(long id)
        {
            var dt = DbOperations.LoadTable(TableName, "Select * FROM Variables where VariableID=" + id);
            if (dt == null || dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            var res = DataRowToVariable(row);
            return res;
        }
        
        public void AddVariable(Variable variable)
        {
            variable.Id = DbOperations.GetNextID(TableName, "VariableID");
            var query =
                string.Format(
                    @"INSERT INTO {0}(VariableID, VariableCode, VariableName, Speciation, SampleMedium, ValueType, IsRegular, IsCategorical, TimeSupport, DataType, GeneralCategory, NoDataValue, TimeUnitsID, VariableUnitsID)
                                       VALUES ({1}, '{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8}, {9}, '{10}', '{11}', {12}, {13}, {14})",
                    TableName,
                    variable.Id, variable.Code, variable.Name, variable.Speciation, variable.SampleMedium,
                    variable.ValueType, Convert.ToInt32(variable.IsRegular), Convert.ToInt32(variable.IsCategorical), variable.TimeSupport,
                    variable.DataType, variable.GeneralCategory, variable.NoDataValue,
                    variable.TimeUnit != null? variable.TimeUnit.Id : 0,
                    variable.VariableUnit != null? variable.VariableUnit.Id : 0);
            DbOperations.ExecuteNonQuery(query);
        }
        
        public void Update(Variable variable)
        {
            var query =
                string.Format(
                    @"UPDATE {0} SET VariableCode='{1}', VariableName='{2}', Speciation='{3}', SampleMedium='{4}', ValueType='{5}', IsRegular={6}, IsCategorical={7}, TimeSupport={8}, DataType='{9}', GeneralCategory='{10}', NoDataValue={11}, TimeUnitsID={12}, VariableUnitsID={13}
                    WHERE VariableID={14}",
                    TableName,
                    variable.Code, variable.Name, variable.Speciation, variable.SampleMedium,
                    variable.ValueType, Convert.ToInt32(variable.IsRegular), Convert.ToInt32(variable.IsCategorical), variable.TimeSupport,
                    variable.DataType, variable.GeneralCategory, variable.NoDataValue, 
                    variable.TimeUnit.Id,
                    variable.VariableUnit.Id, variable.Id);
            DbOperations.ExecuteNonQuery(query);
        }

        public bool Exists(Variable entity)
        {
            if (entity == null) return false;

            const string query = "select count(*) from {0} where VariableID = {1} and VariableCode = '{2}'";
            var result = DbOperations.ExecuteSingleOutput(string.Format(query, TableName, entity.Id, entity.Code));
            return Convert.ToInt32(result) > 0;
        }

        #endregion

        private Variable DataRowToVariable(DataRow row)
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
                              TimeUnit = unitsRepo.GetByID(Convert.ToInt64(row["TimeUnitsID"])),
                              VariableUnit = unitsRepo.GetByID(Convert.ToInt64(row["VariableUnitsID"])),
                          };
            return res;
        }

        public override string TableName
        {
            get { return "Variables"; }
        }
    }
}