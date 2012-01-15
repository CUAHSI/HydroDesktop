using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;
using System.Data;
using System.ComponentModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// This is a general interface for accessing the data repository
    /// </summary>
    public interface IRepositoryManager
    {
        /// <summary>
        /// Gets the database operations tools
        /// </summary>
        IHydroDbOperations DbOperations { get; }
        
        /// <summary>
        /// Deletes a theme and all its series as long as the series don't belong to any other theme.
        /// </summary>
        /// <param name="themeID">The Theme ID</param>
        /// <returns>true if successful, false otherwise</returns>
        bool DeleteTheme(int themeID);


        /// <summary>
        /// Delete a theme - a background worker and progress bar is used
        /// </summary>
        /// <param name="themeID">The themeID (this needs to be a valid ID)</param>
        /// <param name="worker">The background worker component</param>
        /// <param name="e">The arguments for background worker</param>
        /// <returns>true if the theme was successfully deleted</returns>
        bool DeleteTheme(int themeID, BackgroundWorker worker, DoWorkEventArgs e);

        
        /// <summary>
        /// Deletes a series given it's ID. The series is only deleted when it belongs to one theme.
        /// </summary>
        /// <param name="seriesID">The database ID of the series</param>
        /// <returns>true if series was deleted, false otherwise</returns>
        bool DeleteSeries(int seriesID);

 
        /// <summary>
        /// Gets a list of all available series in the form of a data table
        /// </summary>
        /// <returns></returns>
        DataTable GetSeriesListTable();
 

        /// <summary>
        /// Returns a detailed data table for all series in the database
        /// </summary>
        /// <returns>The DataTable with series metadata information</returns>
        DataTable GetSeriesTable();
        
        /// <summary>
        /// Returns a detailed data table for all series in the database. The
        /// table includes the IDs of site, variable, source, method and QCLevel.
        /// </summary>
        /// <returns>The DataTable with series metadata information</returns>
        DataTable GetSeriesTable2();
        

        /// <summary>
        /// Returns a detailed table for all series that match the attributes
        /// </summary>
        /// <param name="seriesIDs">The list of series IDs</param>
        /// <returns>the data table</returns>
        DataTable GetSeriesTable(int[] seriesIDs);
        
        /// <summary>
        /// Returns a detailed table for all series that match the attributes
        /// </summary>
        /// <param name="listOfSeriesID">The list of series IDs</param>
        /// <returns>the data table</returns>
        DataTable GetSeriesTable(IEnumerable<int> listOfSeriesID);
        

        /// <summary>
        /// Returns the data table of detailed properties for one series
        /// </summary>
        /// <param name="seriesID">The id of the series</param>
        /// <returns>The detailed table. This table only has one data row.</returns>
        DataTable GetSeriesTable(int seriesID);

        /// <summary>
        /// Gets the DataSeries object associated with the ID.
        /// If not found, returns null
        /// </summary>
        /// <param name="seriesID">The SeriesID</param>
        /// <returns>The Series object(without its dataValues populated)</returns>
        Series GetSeriesByID(int seriesID);


        /// <summary>
        /// Adds an existing series to an existing theme
        /// </summary>
        /// <param name="series">The existing Series object</param>
        /// <param name="theme">The existing Theme object</param>
        void AddSeriesToTheme(Series series, Theme theme);


        /// <summary>
        /// Simplified version of SaveSeries (for HydroForecaster)
        /// </summary>
        /// <param name="siteID">site ID</param>
        /// <param name="variableID">variable ID</param>
        /// <param name="methodDescription">Method description</param>
        /// <param name="themeName">theme name</param>
        /// <param name="dataValues">The table with data values. First column must be DateTime and second column must be Double.</param>
        /// <returns>number of saved data values</returns>
        int SaveSeries(int siteID, int variableID, string methodDescription, string themeName, DataTable dataValues);


        /// <summary>
        /// Saves a data series to the database. The series will be associated with the 
        /// specified theme. Depending on the OverwriteOptions, this will check if the series
        /// already exists in the database and overwrite data values in the database if required. 
        /// </summary>
        /// <param name="seriesToSave">The data series to be saved. This should contain
        /// information about site, variable, method, source and quality control level.</param>
        /// <param name="theme">The theme where this series should belong to</param>
        /// <param name="overwrite">The overwrite options. Set this to 'Copy' if 
        /// a new series should be created in the database. For options other than 'Copy',
        /// some of the existing data values in the database may be overwritten.</param>
        /// <returns>The number of saved data values</returns>
        int SaveSeries(Series seriesToSave, Theme theme, OverwriteOptions overwrite);

        
        /// <summary>
        /// Saves a data series to the database. The series will be associated with the 
        /// specified theme. This method does not check whether there are any existing series with 
        /// the same properties in the database. It will always create a new 'copy' of the series
        /// </summary>
        /// <param name="series">The time series</param>
        /// <param name="theme">The associated theme</param>
        /// <returns>Number of DataValue saved</returns>
        int SaveSeriesAsCopy(Series series, Theme theme);
        


        /// <summary>
        /// Gets all themes from the database ordered by the theme name
        /// </summary>
        /// <returns>The list of all themes</returns>
        IList<Theme> GetAllThemes();

        IList<Site> GetSitesWithBothVariables(Variable variable1, Variable variable2);

    }

    public interface IDataSeriesRepository
    {
    }
}
