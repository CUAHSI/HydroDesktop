using System;
using System.Data;
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
                    "select {0}(DataValue) from {1} main " +
                    "LEFT JOIN DataSeries ds ON ds.SeriesID = main.SeriesID " +
                    "LEFT JOIN Variables v ON v.VariableID = ds.VariableID " +
                    "WHERE DateTimeUTC >= '{2}' and DateTimeUTC <= '{3}' and main.SeriesID = {4} and DataValue <> v.NoDataValue ",
                    aggregateFunction, TableName, 
                    minDate.ToString("yyyy-MM-dd HH:mm:ss"), maxDate.ToString("yyyy-MM-dd HH:mm:ss"), 
                    seriesID);
            var value =  Convert.ToDouble(DbOperations.ExecuteSingleOutput(query));
            return value;
        }

        public double CalculatePercAvailable(long seriesID, DateTime minDate, DateTime maxDate)
        {
            var query =
              string.Format(
                  "select count(DataValue) from {0} main " +
                  "LEFT JOIN DataSeries ds ON ds.SeriesID = main.SeriesID " +
                  "LEFT JOIN Variables v ON v.VariableID = ds.VariableID " +
                  "WHERE DateTimeUTC >= '{1}' and DateTimeUTC <= '{2}' and main.SeriesID = {3} and DataValue <> v.NoDataValue ",
                  TableName,
                  minDate.ToString("yyyy-MM-dd HH:mm:ss"), maxDate.ToString("yyyy-MM-dd HH:mm:ss"),
                  seriesID);
            var hasValues = Convert.ToDouble(DbOperations.ExecuteSingleOutput(query));

            query =
              string.Format(
                  "select count(DataValue) from {0} main " +
                  "WHERE DateTimeUTC >= '{1}' and DateTimeUTC <= '{2}' and main.SeriesID = {3} ",
                  TableName,
                  minDate.ToString("yyyy-MM-dd HH:mm:ss"), maxDate.ToString("yyyy-MM-dd HH:mm:ss"),
                  seriesID);
            var totalValues = Convert.ToDouble(DbOperations.ExecuteSingleOutput(query));

            var value = hasValues*100.0/totalValues;
            return value;
        }

        public DataTable GetAll(long seriesID)
        {
            var query = "SELECT * FROM DataValues WHERE SeriesID = " + seriesID;
            var result = DbOperations.LoadTable(TableName, query);
            return result;
        }

        #endregion

        public override string TableName
        {
            get { return "DataValues"; }
        }
    }
}