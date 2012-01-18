using System.Collections.Generic;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for DataSeries Repository
    /// </summary>
    public interface IDataSeriesRepository
    {
        /// <summary>
        /// Get VariableID for given SeriesID
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>VariableID</returns>
        int GetVariableID(int seriesID);

        /// <summary>
        /// Returns a detailed data table for all series in the database. The
        /// table includes the IDs of site, variable, source, method and QCLevel.
        /// </summary>
        /// <returns>The DataTable with series metadata information</returns>
        DataTable GetDetailedSeriesTable();

        /// <summary>
        /// Returns a detailed table for all series that match the attributes
        /// </summary>
        /// <param name="listOfSeriesID">The list of series IDs</param>
        /// <returns>the data table</returns>
        DataTable GetSeriesTable(IEnumerable<int> listOfSeriesID);


        /// <summary>
        /// Returns a detailed data table for all series in the database with custom filter. The
        /// table includes the IDs of site, variable, source, method and QCLevel.
        /// </summary>
        /// <returns>The DataTable with series metadata information</returns>
        DataTable GetSeriesTable(string seriesDataFilter);

        /// <summary>
        /// Returns all series
        /// </summary>
        /// <returns>Series collection</returns>
        IList<Series> GetAllSeries();

        /// <summary>
        /// Returns the data table of detailed properties for one series
        /// </summary>
        /// <param name="seriesID">The id of the series</param>
        /// <returns>The detailed table. This table only has one data row.</returns>
        DataTable GetSeriesTable(int seriesID);

        /// <summary>
        /// Returns the data table of SeriesID and NoDataValue
        /// </summary>
        /// <param name="themeIDs">The ids of the themes</param>
        /// <returns>Data table.</returns>
        DataTable GetSeriesIDsWithNoDataValueTable(IEnumerable<int?> themeIDs);

        /// <summary>
        /// Given a Series ID, finds corresponding series in the database
        /// </summary>
        /// <param name="seriesID">the series ID</param>
        /// <returns>The Series object</returns>
        /// <remarks>This method only returns the series metadata. It doesn't return the full list of data
        /// values to save memory space.</remarks>
        Series GetSeriesByID(int seriesID);


        /// <summary>
        /// Gets the list of all series that are available at a site
        /// </summary>
        /// <param name="mySite">The site</param>
        /// <returns>The list of the series from the db</returns>
        /// <remarks>The data values aren't loaded unless requested</remarks>
        IList<Series> GetAllSeriesForSite(Site mySite);

        /// <summary>
        /// Gets list of all series that are associated with the site
        /// </summary>
        IList<Series> GetSeriesBySite(Site site);

        /// <summary>
        /// Deletes a series given it's ID. The series is only deleted when it belongs to one theme.
        /// </summary>
        /// <param name="seriesID">The database ID of the series</param>
        /// <returns>true if series was deleted, false otherwise</returns>
        bool DeleteSeries(int seriesID);

    }
}