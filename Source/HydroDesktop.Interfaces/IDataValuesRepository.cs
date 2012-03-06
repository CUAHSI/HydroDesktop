using System;
using System.Collections.Generic;
using System.Data;
using HydroDesktop.Common;
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
        /// Get all DataValues (ordered by LocalDateTime) for specified seriesID
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>DataTable with DataValues</returns>
        DataTable GetAllOrderByLocalDateTime(long seriesID);

        /// <summary>
        /// Get all values for specified seriesID
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>List of values.</returns>
        IList<double> GetValues(long seriesID);

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

        /// <summary>
        /// Get DataTable for export for time series plot
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>DataTable for export.</returns>
        DataTable GetTableForExportFromTimeSeriesPlot(long seriesID);

        /// <summary>
        /// Get DataTable with values for GraphView
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <param name="nodatavalue">NoDataValue</param>
        /// <param name="startDate">StartDate.</param>
        /// <param name="endDate">EndDate.</param>
        /// <returns>DataTable for GraphView.</returns>
        DataTable GetTableForGraphView(long seriesID, double nodatavalue, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get DataTable with values for EditView
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>DataTable for EditView.</returns>
        DataTable GetTableForEditView(long seriesID);

        double GetMaxValue(long seriesID);
        double GetMinValue(long seriesID);

        long GetCountForAllFieldsInSequence(IList<int> seriesIDs);
        long GetCountForJustValuesInParallel(IList<int> seriesIDs);

        DataTable GetTableForAllFieldsInSequence(IList<int> seriesIDs, int valuesPerPage, int currentPage);
        DataTable GetTableForJustValuesInParallel(IList<int> seriesIDs, int valuesPerPage, int currentPage);

        void DeleteById(long valueID);
        void UpdateValuesForEditView(DataTable table);
    }
}