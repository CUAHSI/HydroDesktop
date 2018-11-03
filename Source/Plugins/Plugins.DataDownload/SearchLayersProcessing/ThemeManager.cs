﻿using System;
using System.Data;
using System.Globalization;
using System.IO;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Plugins.DataDownload.SearchLayersProcessing
{
    /// <summary>
    /// The ThemeManager is responsible for reading themes
    /// from the database and converting them to feature sets which 
    /// can be displayed in the map. It also keeps track of added or
    /// deleted themes and notifies other HydroDesktop plug-ins to refresh
    /// their view when a theme is created, modified or deleted
    /// </summary>
    public class ThemeManager
    {
        #region Variables
        
        private readonly ProjectionInfo _wgs84Projection;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ThemeManager
        /// </summary>
        public ThemeManager()
        {
            _wgs84Projection = ProjectionInfo.FromEsriString(Properties.Resources.Wgs84ProjectionString);
        }
        
        #endregion

        /// <summary>
        /// Given a theme, create a feature set.
        /// The theme already needs to have been saved in
        /// the database and the themeID needs to be a
        /// valid ID
        /// </summary>
        /// <param name="themeID"></param>
        /// <returns></returns>
        private IFeatureSet GetFeatureSet(int themeID)
        {
            DataTable themeTable = LoadThemeAsTable(themeID);
            return TableToFeatureSet(themeTable);
        }

        /// <summary>
        /// Given a theme name, create a feature set.
        /// The theme already needs to be present in the 
        /// database.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns>The theme in the 'WGS84' projection</returns>
        /// <exception cref="ArgumentException">Throws when such theme not found in the database</exception>
        public IFeatureSet GetFeatureSet(string themeName)
        {
            var themeId = RepositoryFactory.Instance.Get<IDataThemesRepository>().GetID(themeName);
            if (themeId == null)
            {
                throw new ArgumentException("Theme not found in the database.");
            }

            return GetFeatureSet(themeId.Value);
        }

       
        private DataTable LoadThemeAsTable(int themeID)
        {
            var table = RepositoryFactory.Instance.Get<IDataThemesRepository>().GetThemesTableForThemeManager(themeID);

            //to get the 'ServiceCode'
            foreach (DataRow row in table.Rows)
            {
                string sCode = Convert.ToString(row["SiteCode"]);
                if (sCode.StartsWith("NWIS"))
                {
                    sCode = Convert.ToString(row["VarCode"]);
                    row["DataSource"] = "USGS";
                }
                else if (sCode.StartsWith("EPA"))
                {
                    row["DataSource"] = "EPA";
                }
                else if (sCode.StartsWith("NCDC"))
                {
                    row["DataSource"] = "National Climatic Data Center";
                }

                row["ServiceCode"] = sCode.Contains(":")
                                         ? sCode.Substring(0, sCode.IndexOf(":", StringComparison.Ordinal))
                                         : sCode;
            }

            return table;
        }
      
        private string ConvertTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// given a data table, create an in-memory point feature set.
        /// The feature set must have the 'Latitude' and 'Longitude' numeric
        /// columns
        /// </summary>
        /// <param name="themeTable">The table of distinct series</param>
        /// <param name="projection">The projection of the theme feature set</param>
        /// <returns>A point FeatureSet in the WGS-84 coordinate system
        /// All columns of the data table will be converted to attribute fields</returns>
        private IFeatureSet TableToFeatureSet(DataTable themeTable, ProjectionInfo projection = null)
        {        
            //index of the Latitude column
            int latColIndex = -1;

            //index of the Longitude column
            int lonColIndex = -1;

            //get the latitude and longitude column indices
            for (int col = 0; col < themeTable.Columns.Count; col++)
            {
                string colName = themeTable.Columns[col].ColumnName.ToLower();

                if (colName == "latitude") latColIndex = col;

                if (colName == "longitude") lonColIndex = col;
            }

            //check if the latitude column exists
            if (latColIndex == -1) throw new ArgumentException("The table doesn't have a column 'Latitude'");
            //check if the longitude column exists
            if (lonColIndex == -1) throw new ArgumentException("The table doesn't have a column 'Longitude'");

            //generate attribute table schema
            var fs = new FeatureSet(FeatureType.Point);
            var attributeTable = fs.DataTable;
            foreach (DataColumn column in themeTable.Columns)
            {
                attributeTable.Columns.Add(column.DataType == typeof (DateTime)
                                               ? new Field(column.ColumnName, 'C', 16, 0)
                                               : new DataColumn(column.ColumnName, column.DataType));
            }

            //generate features
            foreach (DataRow row in themeTable.Rows)
            {
                var lat = Convert.ToDouble(row[latColIndex]);
                var lon = Convert.ToDouble(row[lonColIndex]);
                var coord = new Coordinate(lon, lat);
                var newFeature = new Feature(FeatureType.Point, new[] { coord });
                fs.Features.Add(newFeature);

                var featureRow = newFeature.DataRow;
                for (int c = 0; c < attributeTable.Columns.Count; c++)
                {
                    featureRow[c] = themeTable.Columns[c].DataType == typeof (DateTime)
                                        ? ConvertTime((DateTime) row[c])
                                        : row[c];
                }
            }

            //to save the feature set to a file with unique name
            var uniqueID = DateTime.Now.ToString("yyyyMMdd_hhmmss", CultureInfo.InvariantCulture);
            var rnd = new Random();
            uniqueID += rnd.Next(100).ToString("000");
            var filename = Path.Combine(Settings.Instance.CurrentProjectDirectory, "theme_" + uniqueID + ".shp");
            fs.Filename = filename;
            fs.Projection = _wgs84Projection;
            fs.Save();
            fs.Dispose();

            var fs2 = FeatureSet.OpenFile(filename);

            //to re-project the feature set
            if (projection != null)
            {
                fs2.Reproject(projection);
            }

            return fs2;
        }
    }
}
