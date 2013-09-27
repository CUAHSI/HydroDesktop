using System.Collections.Generic;
using System.IO;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.UnitConversions
{
    class DefaultDatabaseUnitsSource : IUnitsSource
    {
        #region Implementation of IUnitsSource

        public IEnumerable<Unit> GetUnits()
        {
            var path = Path.GetTempFileName();
            try
            {
                SQLiteHelper.CreateSQLiteDatabase(path);
                var connectionString = SQLiteHelper.GetSQLiteConnectionString(path);
                var unitsRepo = RepositoryFactory.Instance.Get<IUnitsRepository>(DatabaseTypes.SQLite, connectionString);
                return unitsRepo.GetAll();
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        #endregion
    }
}