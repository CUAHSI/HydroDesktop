using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterML;

namespace ImportFromWaterML
{
    /// <summary>
    /// Methods to download observation data from WaterML web services and save them to 
    /// the ActualData database.
    /// </summary>
    public class Downloader
    {
        #region Database Methods

        public bool ValidateSeriesList(IList<Series> seriesList)
        {      
            if (seriesList.Count == 0)
                {
                    MessageBox.Show("There was an error parsing the WaterML document. " +
                        "Please check if the document is in correct WaterML 1.0 format.");
                    return false;
                }

            int numValues = 0;
            foreach (Series curSeries in seriesList)
            {
                numValues += curSeries.ValueCount;
            }
            if (numValues == 0)
            {
                MessageBox.Show("The series read from the xml file doesn't contain any data values." +
                    " The series will not be saved to the database.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the xml file to a list of data series objects.
        /// In most cases the list will only contain one object.
        /// </summary>
        /// <param name="xmlFile">The name of the xml file</param>
        /// <returns>the data series list or null in case of error</returns>
        public IList<Series> DataSeriesFromXml(string xmlFile)
        {
            IList<Series> seriesList = null;
            try
            {
                // todo: Support of other WML verions - 1.1 and 2.0
                var parser = new WaterML10Parser();
                seriesList = parser.ParseGetValues(xmlFile);
                if (seriesList == null)
                {
                    MessageBox.Show("There was an error parsing the WaterML document. " +
                        "Please check if the document is in correct WaterML 1.0 format.");
                    return null;
                }
            }
            catch
            {
                //in case of error - return an empty list
            }
            return seriesList;
        }

        /// <summary>
        /// creates a new DataSeries from a xml file and saves it to database.
        /// This function uses the underlying ORM (NHibernate) framework to
        /// communicate with the database
        /// </summary>
        /// <param name="series">The data series to be saved</param>
        /// <param name="themeName">Theme name</param>
        /// <param name="siteName">Site name</param>
        /// <param name="variableName">Variable name </param>
        /// <returns>True if series was successfully saved.</returns>
        public bool SaveDataSeries(Series series, string themeName, string siteName, string variableName)
        {
            //check if the series has values
            if (series.ValueCount == 0) return false;

            var db = RepositoryFactory.Instance.Get<IRepositoryManager>();

            Theme theme = new Theme(themeName, "");

            if (String.IsNullOrEmpty(series.Site.Name))
            {
                series.Site.Name = siteName;
            }
            if (String.IsNullOrEmpty(series.Variable.Name))
            {
                series.Variable.Name = variableName;
            }

            db.SaveSeries(series, theme, OverwriteOptions.Copy);

            return true;
        }
        

        #endregion
    }
}
