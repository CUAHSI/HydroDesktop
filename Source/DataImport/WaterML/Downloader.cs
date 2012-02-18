using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.WebServices;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterOneFlow;
using HydroDesktop.Interfaces;
using HydroDesktop.Configuration;
using System.Globalization;
namespace ImportFromWaterML
{
    /// <summary>
    /// Methods to download observation data from WaterML web services and save them to 
    /// the ActualData database.
    /// </summary>
    public class Downloader
    {
        

        #region File I/O Methods
        /// <summary>
        /// Gets the temporary directory for xml files downloaded
        /// by HydroDesktop
        /// </summary>
        /// <returns>the directory path</returns>
        public string GetXmlTempDirectory()
        {
            //Check if we need to create a temporary folder for storing the xml file
            string tempDirectory = Path.Combine(Path.GetTempPath(), "HydroDesktop");
            if (Directory.Exists(tempDirectory) == false)
            {
                try
                {
                    Directory.CreateDirectory(tempDirectory);
                }
                catch
                {
                    tempDirectory = Path.GetTempPath();
                }
            }
            return tempDirectory;
        }
        #endregion

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
                WaterOneFlow10Parser parser = new WaterOneFlow10Parser();
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
        /// <param name="theme">The theme associated with this data series</param>
        /// <param name="overwrite">Determines how to handle duplicate data values.</param>
        /// <returns>True if series was successfully saved.</returns>
        public bool SaveDataSeries(Series series, string themeName, string siteName, string variableName)
        {
            //check if the series has values
            if (series.ValueCount == 0) return false;

            var db = RepositoryFactory.Instance.Get<IRepositoryManager>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);

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

        

        public DateTime ConvertDateTime(object timeObj)
        {
            if (timeObj == null) return DateTime.MinValue;

            string timeStr = timeObj.ToString();
            timeStr = timeStr.Replace("T", " ");
            return Convert.ToDateTime(timeStr, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
