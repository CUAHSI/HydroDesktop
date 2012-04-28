using HydroDesktop.Interfaces.ObjectModel;
using System.Data;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// This is a general interface for accessing the data repository
    /// </summary>
    public interface IRepositoryManager
    {
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

    }
}
