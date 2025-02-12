using System;
using System.Collections.Generic;
using System.Data;
using HydroDesktop.Common;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for DataSeries Repository
    /// </summary>
    public interface IDataSeriesRepository : IRepository<Series>
    {
        /// <summary>
        /// Get Variable.NoDataValue for given series
        /// </summary>
        /// <param name="seriesID">Series ID</param>
        /// <returns>NoDataValue</returns>
        double GetNoDataValueForSeriesVariable(long seriesID);

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

        Tuple<DateTime, DateTime> GetDatesRange(long seriesID);

        /// <summary>
        /// Deletes a series given it's ID. The series is only deleted when it belongs to one theme.
        /// </summary>
        /// <param name="seriesID">The database ID of the series</param>
        /// <param name="themeId">Theme Id</param>
        /// <returns>true if series was deleted, false otherwise</returns>
        void DeleteSeries(long seriesID, long themeId);

        /// <summary>
        /// Check that exists series with given Site and Variable.
        /// </summary>
        /// <param name="site">Site</param>
        /// <param name="variable">Variable</param>
        /// <returns>True - exists, otherwise - false.</returns>
        bool ExistsSeries(Site site, Variable variable);

        /// <summary>
        /// Get BeginDateTime and EndDateTime for given seriesID
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>BeginDateTime and EndDateTime</returns>
        Tuple<DateTime, DateTime> GetDateTimes(long seriesID);

        /// <summary>
        /// Get UnitsName, SiteName, VariableName for first series with given seriesID
        /// </summary>
        /// <param name="seriesID">SeriesID</param>
        /// <returns>Table with UnitsName, SiteName, VariableName</returns>
        DataTable GetUnitSiteVarForFirstSeries(long seriesID);

        void UpdateDataSeriesFromDataValues(long seriesID);

        string GetQualityControlLevelCode(long seriesID);
        long GetQualityControlLevelID(long seriesID);

        IList<long> GetDataValuesIDs(long seriesID);

        int InsertNewSeries(long sourceSeriesID, long variableID, long methodID, long qualityControlLevelID);

        void DeriveInsertAggregateDataValues(DataTable dt,
                                             long newSeriesID,
                                             DateTime currentdate, DateTime lastdate, DeriveAggregateMode mode,
                                             DeriveComputeMode computeMode,
                                             double nodatavalue, IProgressHandler progressHandler);

        void DeriveInsertDataValues(double A, double B, double C, double D, double E, double F,
                                    DataTable dt,
                                    long newSeriesID, long sourceSeriesID, bool isAlgebraic,
                                    IProgressHandler progressHandler);
    }

    public enum DeriveAggregateMode
    {
        Daily,
        Monthly,
        Quarterly,
    }

    public enum DeriveComputeMode
    {
        Maximum,
        Minimum,
        Average,
        Sum
    }
}