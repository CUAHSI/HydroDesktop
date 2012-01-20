using System;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for DataValues
    /// </summary>
    class DataValuesRepository : BaseRepository, IDataValuesRepository
    {
        #region Constructors

        public DataValuesRepository(DatabaseTypes dbType, string connectionString)
            : base(dbType, connectionString)
        {
        }

        public DataValuesRepository(IHydroDbOperations db)
            : base(db)
        {
        }

        #endregion

        #region Public methods

        public double AggregateValues(long seriesID, string aggregateFunction, DateTime minDate, DateTime maxDate)
        {
            var query =
                string.Format(
                    "select {0}(DataValue) from {1} WHERE DateTimeUTC >= '{2}' and DateTimeUTC <= '{3}' and SeriesID = {4}",
                    aggregateFunction, TableName, 
                    minDate.ToString("yyyy-MM-dd HH:mm:ss"), maxDate.ToString("yyyy-MM-dd HH:mm:ss"), 
                    seriesID);
            return Convert.ToDouble(DbOperations.ExecuteSingleOutput(query));
        }

        #endregion

        public override string TableName
        {
            get { return "DataValues"; }
        }
    }
}