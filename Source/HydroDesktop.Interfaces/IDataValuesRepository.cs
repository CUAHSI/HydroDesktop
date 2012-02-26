using System;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for DataValues Repository
    /// </summary>
    public interface IDataValuesRepository : IRepository<DataValue>
    {
        /// <summary>
        /// Aggregate values for series
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <param name="aggregateFunction">SQL aggregate function: sum, min, max,...</param>
        /// <param name="minDate">Minimum date to aggregate series</param>
        /// <param name="maxDate">Maximum date to aggregate series</param>
        /// <returns>Aggregated value.</returns>
        double? AggregateValues(long seriesID, string aggregateFunction, DateTime minDate, DateTime maxDate);

        /// <summary>
        /// Calculates percent of avalible values (not equal NoDataValue) for given seriesID and period
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <param name="minDate">Minimum date of period</param>
        /// <param name="maxDate">Maximum date of period</param>
        /// <returns>Percent of avalible values</returns>
        double CalculatePercAvailable(long seriesID, DateTime minDate, DateTime maxDate);

        /// <summary>
        /// Get all DataValues for specified seriesID
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>DataTable with DataValues</returns>
        DataTable GetAll(long seriesID);

        /// <summary>
        /// Get DataTable for export
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <param name="noDataValue">NoDataValue filter</param>
        /// <param name="dateColumn">DateColumn filter</param>
        /// <param name="firstDate">First date from dateColumn filter</param>
        /// <param name="lastDate">Last date from dateColumn filter</param>
        /// <returns>DataTable for export</returns>
        DataTable GetTableForExport(long seriesID, double? noDataValue = null, string dateColumn = null,
                                    DateTime? firstDate = null, DateTime? lastDate = null);
    }
}