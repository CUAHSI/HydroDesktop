using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HydroDesktop.Interfaces;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using HydroDesktop.Database;
using DotSpatial.Projections;
using HydroDesktop.Configuration;
using System.IO;
using Hydrodesktop.Common;

namespace TableView
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

        private readonly ISearchPlugin _searchPlugin;
        private ProjectionInfo _wgs84Projection;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new ThemeManager
        /// </summary>
        public ThemeManager(ISearchPlugin searchPlugin)
        {
            _searchPlugin = searchPlugin;
            _wgs84Projection = ProjectionInfo.FromEsriString(Properties.Resources.Wgs84ProjectionString);
        }
        
        #endregion

        /// <summary>
        /// Synchronize all theme layers in the map with the themes in the current database
        /// </summary>
        /// <param name="mainMap">The main map</param>
        public void RefreshAllThemes(Map mainMap)
        {
            //(1) Find theme names from database and from map
            var themeNamesInDb = GetThemeNamesFromDb();
            var themeNamesInMap = GetThemeNamesFromMap(mainMap);

            //(2) Find which themes to remove from map and which
            //    themes to check
            List<string> themesToRemove = themeNamesInMap;

            //(3) Find which new themes to add to the map from the database
            List<string> themesToAdd = themeNamesInDb;
            
            //(4) Removing themes from map
            foreach (string themeToRemove in themesToRemove)
            {
                RemoveThemeFromMap(mainMap, themeToRemove);
            }

            //(6) Add any new themes from database to map
            var itemsToAdd = new List<Tuple<string, IFeatureSet>>(themesToAdd.Count);
            foreach (var themeToAdd in themesToAdd)
            {
                var fs = GetFeatureSet(themeToAdd, mainMap.Projection);
                itemsToAdd.Add(new Tuple<string, IFeatureSet>(themeToAdd, fs));
            }
            _searchPlugin.AddFeatures(itemsToAdd);
            
            mainMap.MapFrame.ResetBuffer();
         
        }

        private void RemoveThemeFromMap(Map mainMap, string themeName)
        {
            List<ILayer> layers = mainMap.GetAllLayers();
            IMapLayer themeLayer = null;
            foreach (ILayer layer in layers)
            {
                if (layer.LegendText == themeName)
                {
                    themeLayer = layer as IMapLayer;
                    break;
                }
            }
            if (themeLayer != null)
            {
                IMapGroup parentItem = themeLayer.GetParentItem() as IMapGroup;
                if (parentItem != null)
                {
                    parentItem.Remove(themeLayer);
                }
            }
        }

        /// <summary>
        /// Finds the map group with the name 'Themes'
        /// </summary>
        /// <param name="map">the map to search</param>
        /// <returns>The group named 'Themes'</returns>
        private IMapGroup FindThemeGroup(Map mainMap)
        {
            return mainMap.GetDataSitesLayer();
        }

        /// <summary>
        /// Gets a list of all theme names from the DB
        /// </summary>
        private List<string> GetThemeNamesFromDb()
        {
            var repo = RepositoryFactory.Instance.Get<IDataThemesRepository>();
            
            var resultTable = repo.GetThemesForAllSeries();
            var themeNameList = new List<string>(resultTable.Rows.Count);
            themeNameList.AddRange(from DataRow row in resultTable.Rows select Convert.ToString(row["ThemeName"]));
            return themeNameList;
        }

        /// <summary>
        /// Gets a list of all theme names from the Map
        /// </summary>
        private List<string> GetThemeNamesFromMap(Map mainMap)
        {
            List<string> themeNameList = new List<string>();
            IMapGroup themeGroup = FindThemeGroup(mainMap);
            if (themeGroup != null)
            {
                foreach (ILayer lay in themeGroup.Layers)
                {
                    MapPointLayer pl = lay as MapPointLayer;
                    if (pl != null)
                    {
                        themeNameList.Add(pl.LegendText);
                    }
                }
            }
            return themeNameList;
        }
     

        /// <summary>
        /// Given a theme name, create a feature set.
        /// The theme already needs to be present in the 
        /// database.
        /// </summary>
        /// <param name="themeName">the theme name (also appears in 'legend'</param>
        /// <param name="projection">The desired projection of the theme</param>
        /// <returns>the feature set with sites of the theme in the
        /// user specified projection</returns>
        public IFeatureSet GetFeatureSet(string themeName, ProjectionInfo projection)
        {
            var repo = RepositoryFactory.Instance.Get<IDataThemesRepository>();
            var themeID = repo.GetID(themeName);

            var themeTable = LoadThemeAsTable(themeID);
            var unprojectedFs = TableToFeatureSet(themeTable);
            unprojectedFs.Projection = _wgs84Projection;
            unprojectedFs.Reproject(projection);
            return unprojectedFs;
        }
       
        private DataTable LoadThemeAsTable(int? themeID)
        {
            var repo = RepositoryFactory.Instance.Get<IDataThemesRepository>();
            var table = repo.GetThemesTableForThemeManager(themeID);

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

                if (sCode.IndexOf(":") > -1)
                {
                    row["ServiceCode"] = sCode.Substring(0, sCode.IndexOf(":"));
                }
                else
                {
                    row["ServiceCode"] = sCode;
                }
            }

            return table;
        }

        private string ConvertTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// given a data table, create an in-memory point feature set.
        /// The feature set must have the 'Latitude' and 'Longitude' numeric
        /// columns
        /// </summary>
        /// <param name="themeTable">The table of distinct series</param>
        /// <returns>A point FeatureSet in the WGS-84 coordinate system
        /// All columns of the data table will be converted to atrribute fields</returns>
        private IFeatureSet TableToFeatureSet(DataTable themeTable)
        {
            return TableToFeatureSet(themeTable, null);
        }

        /// <summary>
        /// given a data table, create an in-memory point feature set.
        /// The feature set must have the 'Latitude' and 'Longitude' numeric
        /// columns
        /// </summary>
        /// <param name="themeTable">The table of distinct series</param>
        /// <param name="projection">The projection of the theme feature set</param>
        /// <returns>A point FeatureSet in the WGS-84 coordinate system
        /// All columns of the data table will be converted to atrribute fields</returns>
        private IFeatureSet TableToFeatureSet(DataTable themeTable, ProjectionInfo projection)
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
            FeatureSet fs = new FeatureSet(FeatureType.Point);
            DataTable attributeTable = fs.DataTable;
            foreach (DataColumn column in themeTable.Columns)
            {
                if (column.DataType == typeof(DateTime))
                {
                    attributeTable.Columns.Add(new Field(column.ColumnName, 'C', 16, 0));
                }
                else
                {
                    attributeTable.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
                }
            }

            //generate features
            foreach (DataRow row in themeTable.Rows)
            {
                double lat = Convert.ToDouble(row[latColIndex]);
                double lon = Convert.ToDouble(row[lonColIndex]);
                Coordinate coord = new Coordinate(lon, lat);
                Feature newFeature = new Feature(FeatureType.Point, new Coordinate[] { coord });
                fs.Features.Add(newFeature);

                DataRow featureRow = newFeature.DataRow;
                for (int c = 0; c < attributeTable.Columns.Count; c++)
                {
                    if (themeTable.Columns[c].DataType == typeof(DateTime))
                    {
                        featureRow[c] = ConvertTime((DateTime)row[c]);
                    }
                    else
                    {
                        featureRow[c] = row[c];
                    }
                }
            }

            //to save the feature set to a file with unique name
            string uniqueID = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            Random rnd = new Random();
            uniqueID += rnd.Next(100).ToString("000");
            string filename = Path.Combine(Settings.Instance.CurrentProjectDirectory, "theme_" + uniqueID + ".shp");
            fs.Filename = filename;
            fs.Projection = _wgs84Projection;
            fs.Save();
            fs.Dispose();
            fs = null;

            IFeatureSet fs2 = FeatureSet.OpenFile(filename);

            //to reproject the feature set
            if (projection != null)
            {
                fs2.Reproject(projection);
            }

            return fs2;
        }
    }
}
