using System;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for Methods
    /// </summary>
    class MethodsRepository : BaseRepository<Method>, IMethodsRepository
    {
        #region Constructors
        
        public MethodsRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public MethodsRepository(IHydroDbOperations db)
            : base(db)
        {
        }

        #endregion

        #region Public methods

        public int InsertMethod(string methodDescription, string methodLink)
        {
            var methodID = DbOperations.GetNextID(TableName, "MethodID");
            DbOperations.ExecuteNonQuery(
                string.Format(
                    "INSERT INTO Methods(MethodID, MethodDescription, MethodLink) VALUES ({0}, '{1}', '{2}')", methodID,
                    methodDescription, methodLink));

            return methodID;
        }
       
        public void UpdateMethod(int methodID, string methodDescription, string methodLink)
        {
            DbOperations.ExecuteNonQuery(
                string.Format("UPDATE Methods SET MethodDescription='{0}', MethodLink='{1}' Where MethodID = {2}",
                              methodDescription, methodLink, methodID)
                );
        }
      
        public int? GetMethodID(string methodDescription)
        {
            var res = DbOperations.ExecuteSingleOutput(string.Format("select MethodID from Methods where MethodDescription='{0}'", methodDescription));
            if (res == null || res == DBNull.Value)
                return null;
            if (res.ToString() == String.Empty)
                return null;
            return Convert.ToInt32(res);
        }

        #endregion

        protected override Method DataRowToEntity(System.Data.DataRow row)
        {
            return new Method
            {
                Id = Convert.ToInt32(row["MethodID"]),
                Code = Convert.ToInt32(row["MethodID"]),
                Description = Convert.ToString(row["MethodDescription"]),
                Link = Convert.ToString(row["MethodLink"]),
            };
        }

        protected override string TableName
        {
            get { return "Methods"; }
        }

        protected override string PrimaryKeyName
        {
            get { return "MethodID"; }
        }
    }
}