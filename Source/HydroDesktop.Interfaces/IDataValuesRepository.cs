using System;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for DataValues Repository
    /// </summary>
    public interface IDataValuesRepository
    {
        /// <summary>
        /// Aggregate values for series
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <param name="aggregateFunction">SQL aggregate function: sum, min, max,...</param>
        /// <param name="minDate">Minimum date to aggregate series</param>
        /// <param name="maxDate">Maximum date to aggregate series</param>
        /// <returns>Aggregated value.</returns>
        double AggregateValues(long seriesID, string aggregateFunction, DateTime minDate, DateTime maxDate);
    }
}