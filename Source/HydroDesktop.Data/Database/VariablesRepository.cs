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
        
        public DataTable GetAll()
        {
            var dt = DbOperations.LoadTable("Variables", "Select * FROM Variables");
            return dt;
        }

        public Variable GetByID(long id)
        {
            var dt = DbOperations.LoadTable("Variables", "Select * FROM Variables where VariableID=" + id);
            if (dt == null || dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            var unitsRepo = RepositoryFactory.Instance.Get<IUnitsRepository>(DbOperations);
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

        /// <summary>
        /// Create copy of variable and save it to database
        /// </summary>
        /// <param name="variableID"></param>
        /// <returns>ID of created variable</returns>
        public int CreateCopy(int variableID)
        {
            // var newID = DbOperations.GetNextID("Variables", "MethodID");
            throw new NotImplementedException();
        }

        #endregion
    }
}