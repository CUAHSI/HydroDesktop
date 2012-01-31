using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using HydroDesktop.Database;
using DotSpatial.Projections;
using HydroDesktop.Configuration;
using System.IO;

namespace HydroDesktop.DataDownload.SearchLayersProcessing
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
        private DbOperations _db;
        private ProjectionInfo _wgs84Projection;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new ThemeManager, using the database connection string
        /// </summary>
        /// <param name="dbConnectionString">the SQLite database connection string</param>
        public ThemeManager(string dbConnectionString)
        {
            _db = new DbOperations(dbConnectionString, Interfaces.DatabaseTypes.SQLite);
            _wgs84Projection = ProjectionInfo.FromEsriString(Properties.Resources.Wgs84ProjectionString);
        }
        /// <summary>
        /// Creates a new ThemeManager, using the dbOperations object
        /// </summary>
        public ThemeManager(DbOperations dbTools)
        {
            _db = dbTools;
            string esri = Properties.Resources.Wgs84ProjectionString;
            _wgs84Projection = ProjectionInfo.FromEsriString(Properties.Resources.Wgs84ProjectionString);

        }
        #endregion

        /// <summary>
        /// Synchronize all theme layers in the map with the themes in the current database
        /// </summary>
        /// <param name="mainMap">The main map</param>
        public void RefreshAllThemes(Map mainMap)
        {
            ReportProgress(mainMap, 0, "Loading Themes...");

            //(1) Find theme names from database and from map
            List<string> themeNamesInDb = GetThemeNamesFromDb();
            List<string> themeNamesInMap = GetThemeNamesFromMap(mainMap);

            //(2) Find which themes to remove from map and which
            //    themes to check
            List<string> themesToRemove = new List<string>();
            List<string> themesToCheck = new List<string>();
            foreach (string themeInMap in themeNamesInMap)
            {
                if (themeNamesInDb.Contains(themeInMap))
                {
                    themesToCheck.Add(themeInMap);
                }
                else
                {
                    themesToRemove.Add(themeInMap);
                }
            }

            //(3) Find which new themes to add to the map from the database
            List<string> themesToAdd = new List<string>();
            foreach (string themeInDb in themeNamesInDb)
            {
                if (!themeNamesInMap.Contains(themeInDb))
                {
                    themesToAdd.Add(themeInDb);
                }
            }
            int numThemes = themesToRemove.Count + themesToAdd.Count + themesToCheck.Count;
            int counter = 0;

            //(4) Removing themes from map
            foreach (string themeToRemove in themesToRemove)
            {
                int percent = (counter * 100) / numThemes;
                string message = String.Format("Loading Theme {0}, {1}% complete.", themeToRemove, percent);
                ReportProgress(mainMap, percent, message);
                counter++;
                
                RemoveThemeFromMap(mainMap, themeToRemove);
            }
            //(5) Check if the theme in the map has the same series as the
            //    theme in the database. If not, reload theme from database.
            foreach (string themeToCheck in themesToCheck)
            {
                int percent = (counter * 100) / numThemes;
                string message = String.Format("Loading Theme {0}, {1}% complete.", themeToCheck, percent);
                ReportProgress(mainMap, percent, message);
                counter++;
                
                SynchronizeTheme(mainMap, themeToCheck);
            }
            //(6) Add any new themes from database to map
            foreach (string themeToAdd in themesToAdd)
            {
                int percent = (counter * 100) / numThemes;
                string message = String.Format("Loading Theme {0}, {1}% complete.", themeToAdd, percent);
                ReportProgress(mainMap, percent, message);
                counter++;
                
                IFeatureSet fs = GetFeatureSet(themeToAdd, mainMap.Projection);
                AddThemeToMap(fs, themeToAdd, mainMap);
            }

            ////First remove all existing theme layers
            //IMapGroup themeGroup = FindThemeGroup(mainMap);
            //if (themeGroup != null)
            //{
            //    themeGroup.Layers.Clear();
            //}
            //RepositoryManagerSQL repoManager = new RepositoryManagerSQL(_db);
            //IList<Theme> themes = repoManager.GetAllThemes();
            //int numThemes = themes.Count;
            //int counter = 0;
            //foreach (Theme theme in themes)
            //{
            //    int percent = (counter * 100) / numThemes;
            //    string message = String.Format("Loading Theme {0}, {1}% complete.", theme.Name, percent);
            //    ReportProgress(mainMap, percent, message);
            //    counter++;

            //    IFeatureSet fs = GetFeatureSet(theme.Name, mainMap.Projection);
            //    AddThemeToMap(fs, theme.Name, mainMap);
            //}
            mainMap.MapFrame.ResetBuffer();
            ReportProgress(mainMap, 0, String.Empty);
        }

        /// <summary>
        /// Synchronizes the theme in the map with the theme stored in the database.
        /// If the series in the database have changed, refresh the map theme by
        /// reloading it from the database.
        /// </summary>
        /// <param name="map">The main map</param>
        /// <param name="themeName">The name of the theme to synchronize</param>
        public void SynchronizeTheme(Map map, string themeName)
        {
            IMapPointLayer layer = FindThemeLayer(map, themeName);
            if (layer == null) return;

            DataTable themeTable = layer.DataSet.DataTable;
            int[] dbThemeStatistics = GetThemeStatistics(themeName);
            if (dbThemeStatistics == null) return;
            int numSeries = dbThemeStatistics[0];
            int numValues = dbThemeStatistics[1];

            //get numValues in feature table
            int numSeriesInMap = themeTable.Rows.Count;
            int numValuesInMap = 0;
            foreach (DataRow row in themeTable.Rows)
            {
                numValuesInMap += Convert.ToInt32(row["ValueCount"]);
            }
            //Synchronize the theme if required 
            //TODO: reuse existing symbolizer
            //TODO: reuse label settings
            if (numSeriesInMap != numSeries || numValuesInMap != numValues)
            {
                IFeatureSet fs = GetFeatureSet(themeName, map.Projection);
                AddThemeToMap(fs, themeName, map);
            }
        }
        /// <summary>
        /// Finds the layer corresponding to the theme name
        /// </summary>
        /// <param name="map">the main map</param>
        /// <param name="themeName">the theme name</param>
        /// <returns>The theme name point layer, or NULL if the layer is not found</returns>
        private IMapPointLayer FindThemeLayer(Map map, string themeName)
        {
            IMapPointLayer themeLayer = null;
            foreach (ILayer layer in map.GetAllLayers())
            {
                if (layer.LegendText == themeName)
                {
                    themeLayer = layer as IMapPointLayer;
                    break;
                }
            }
            return themeLayer;
        }

        //Get the theme statistics (number of series and number of data values)
        private int[] GetThemeStatistics(string themeName)
        {
            string sql =
            "SELECT desc.ThemeID, desc.ThemeName, COUNT(themes.SeriesID) as 'NumSeries', SUM(series.ValueCount) as 'NumValues'" +
            "FROM DataThemeDescriptions desc " +
            "LEFT JOIN DataThemes themes ON desc.ThemeID = themes.ThemeID " +
            "LEFT JOIN DataSeries series ON themes.SeriesID = series.SeriesID " +
            "WHERE desc.ThemeName = '" + themeName + "'";
            DataTable resultTable = _db.LoadTable(sql);
            if (resultTable.Rows.Count == 1)
            {
                int[] stats = new int[2];
                stats[0] = Convert.ToInt32(resultTable.Rows[0][2]);
                stats[1] = Convert.ToInt32(resultTable.Rows[0][3]);
                return stats;
            }
            else
            {
                return null;
            }     
        }

        private void ReportProgress(Map map, int percent, string message)
        {
            map.ProgressHandler.Progress(message, percent, message);
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
            string groupLegendText = "Themes";
            foreach (LegendItem item in mainMap.Layers)
            {
                if (item is IMapGroup && item.LegendText.ToLower() == groupLegendText.ToLower())
                {
                    return (IMapGroup)item;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a list of all theme names from the DB
        /// </summary>
        public List<string> GetThemeNamesFromDb()
        {
            List<string> themeNameList = new List<string>();
            string query = "SELECT ThemeName FROM DataThemeDescriptions";
            DataTable resultTable = _db.LoadTable(query);
            foreach (DataRow row in resultTable.Rows)
            {
                themeNameList.Add(row[0].ToString());
            }
            return themeNameList;
        }

        /// <summary>
        /// Gets a list of all theme names from the Map
        /// </summary>
        private List<string> GetThemeNamesFromMap(Map mainMap)
        {
            //TODO: Identify the theme layer by using 'Name' property
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
        /// Given a theme, create a feature set.
        /// The theme needs to have the SeriesList
        /// already populated.
        /// </summary>
        /// <param name="theme">The theme object</param>
        /// <returns>The feature set representing the theme</returns>
        public IFeatureSet GetFeatureSet(Theme theme)
        {
            if (theme.Id > 0 || theme.SeriesList.Count > 0)
            {
                IFeatureSet fs = initializeFeatureSet();
                foreach (Series series in theme.SeriesList)
                {
                    AddSeriesToFeatureSet(series, fs);
                }
                return fs;
            }
            return null;
        }

        /// <summary>
        /// Given a theme, create a feature set.
        /// The theme needs to have the SeriesList
        /// already populated.
        /// </summary>
        /// <param name="theme">The theme object</param>
        /// <returns>The feature set representing the theme</returns>
        public IFeatureSet GetFeatureSet(Theme theme, ProjectionInfo projection)
        {
            if (theme.Id > 0)
            {
                return GetFeatureSet(Convert.ToInt32(theme.Id), projection);
            }
            else if (theme.Name != String.Empty)
            {
                return GetFeatureSet(theme.Name, projection);
            }
            else
            {
                throw new ArgumentException("Please specify the theme ID or theme name");
            }
        }

        /// <summary>
        /// Given a theme, create a feature set.
        /// The theme already needs to have been saved in
        /// the database and the themeID needs to be a
        /// valid ID
        /// </summary>
        /// <param name="themeID"></param>
        /// <returns></returns>
        public IFeatureSet GetFeatureSet(int themeID)
        {
            DataTable themeTable = LoadThemeAsTable(themeID);
            return TableToFeatureSet(themeTable);
        }

        /// <summary>
        /// Given a theme, create a feature set.
        /// The theme already needs to have been saved in
        /// the database and the themeID needs to be a
        /// valid ID
        /// </summary>
        /// <param name="themeID"></param>
        /// <returns>the feature set of sites in the theme in the user
        /// specified projection</returns>
        public IFeatureSet GetFeatureSet(int themeID, ProjectionInfo projection)
        {
            DataTable themeTable = LoadThemeAsTable(themeID);
            IFeatureSet unprojectedFs = TableToFeatureSet(themeTable);
            unprojectedFs.Projection = _wgs84Projection;
            unprojectedFs.Reproject(projection);
            return unprojectedFs;
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
            string sql = "SELECT ThemeID from DataThemeDescriptions WHERE ThemeName =?";
            object objThemeId = null;
            objThemeId = _db.ExecuteSingleOutput(sql, new string[] { themeName });

            if (objThemeId == null)
            {
                throw new ArgumentException("Theme not found in the database.");
            }
            else
            {
                int themeID = Convert.ToInt32(objThemeId);
                return GetFeatureSet(themeID);
            }
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
            string sql = "SELECT ThemeID from DataThemeDescriptions WHERE ThemeName =?";
            object objThemeId = null;
            objThemeId = _db.ExecuteSingleOutput(sql, new string[] { themeName });

            if (objThemeId == null)
            {
                throw new ArgumentException("Theme not found in the database.");
            }
            else
            {
                int themeID = Convert.ToInt32(objThemeId);
                return GetFeatureSet(themeID, projection);
            }
        }

        /// <summary>
        /// Loads the 
        /// </summary>
        /// <param name="themeID"></param>
        /// <returns></returns>
        private DataTable LoadThemeAsTable(int themeID)
        {
            //TODO: we need to compare the performance of the SQL query versus NHibernate object query
            //string sql =
            //    "SELECT MAX(src.Organization) as 'DataSource', ds.SeriesID, " +
            //    "MAX(s.SiteName) as 'SiteName', MAX(s.Latitude) as 'Latitude', MAX(s.Longitude) as 'Longitude', MAX(s.SiteCode) as 'SiteCode', " +
            //    "MAX(v.VariableName) as 'VariableName', MAX(v.DataType) as 'DataType', MAX(v.SampleMedium) as 'SampleMedium', MAX(v.VariableCode) as 'VariableCode', MAX(u.UnitsName) as 'Units', " +
            //    "MAX(v.VariableCode) as 'ServiceCode', " +
            //    "MAX(m.MethodDescription) as 'Method', MAX(qc.Definition) as 'QualityControl', " +
            //    "MAX(ds.BeginDateTime) as 'BeginDateTime', MAX(ds.EndDateTime) as 'EndDateTime', MAX(ds.ValueCount) as 'ValueCount', " +
            //    "AVG(dv.DataValue) as 'avg', MIN(dv.DataValue) as 'min', MAX(dv.DataValue) as 'max' " +
            //    "FROM DataThemes dt " +
            //    "INNER JOIN DataSeries ds on dt.SeriesID = ds.SeriesID " +
            //    "INNER JOIN Sites s on ds.SiteID = s.SiteID " +
            //    "INNER JOIN Variables v on ds.VariableID = v.VariableID " +
            //    "INNER JOIN Units u on u.UnitsID = v.VariableUnitsID " +
            //    "INNER JOIN Methods m on ds.MethodID = m.MethodID " +
            //    "INNER JOIN Sources src on ds.SourceID = src.SourceID " +
            //    "INNER JOIN QualityControlLevels qc on ds.QualityControlLevelID = qc.QualityControlLevelID " +
            //    "INNER JOIN DataValues dv on ds.SeriesID = dv.SeriesID " +
            //    "WHERE dt.ThemeID = " + themeID + " AND dv.DataValue > -99 " +
            //    "GROUP BY ds.SeriesID ";

            string sql =
                "SELECT src.Organization as 'DataSource', ds.SeriesID, " +
                "s.SiteName as 'SiteName', s.Latitude as 'Latitude', s.Longitude as 'Longitude', s.SiteCode as 'SiteCode', " +
                "v.VariableName as 'VariableName', v.DataType as 'DataType', v.SampleMedium as 'SampleMedium', " +
                "v.VariableCode as 'VariableCode', u.UnitsName as 'Units', " +
                "v.VariableCode as 'ServiceCode', " +
                "m.MethodDescription as 'Method', qc.Definition as 'QualityControl', " +
                "ds.BeginDateTime as 'BeginDateTime', ds.EndDateTime as 'EndDateTime', ds.ValueCount as 'ValueCount' " +
                "FROM DataThemes dt " +
                "LEFT JOIN DataSeries ds on dt.SeriesID = ds.SeriesID " +
                "LEFT JOIN Sites s on ds.SiteID = s.SiteID " +
                "LEFT JOIN Variables v on ds.VariableID = v.VariableID " +
                "LEFT JOIN Units u on u.UnitsID = v.VariableUnitsID " +
                "LEFT JOIN Methods m on ds.MethodID = m.MethodID " +
                "LEFT JOIN Sources src on ds.SourceID = src.SourceID " +
                "LEFT JOIN QualityControlLevels qc on ds.QualityControlLevelID = qc.QualityControlLevelID " +
                "WHERE dt.ThemeID = " + themeID;

            DataTable table = _db.LoadTable("ThemeTable", sql);

            //to get the 'ServiceCode'
            foreach (DataRow row in table.Rows)
            {
                string sCode = Convert.ToString(row["SiteCode"]);
                if (sCode.StartsWith("NWIS"))
                {
                    sCode = Convert.ToString(row["VariableCode"]);
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

        private IFeatureSet initializeFeatureSet()
        {
            IFeatureSet fs = new FeatureSet(FeatureType.Point);
            DataTable table = fs.DataTable;
            table.Columns.Add("DataSource", typeof(string));
            table.Columns.Add("SeriesID", typeof(int));
            table.Columns.Add("SiteName", typeof(string));
            table.Columns.Add("Latitude", typeof(double));
            table.Columns.Add("Longitude", typeof(double));
            table.Columns.Add("SiteCode", typeof(string));
            table.Columns.Add("VariableName", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("SampleMedium", typeof(string));
            table.Columns.Add("VariableCode", typeof(string));
            table.Columns.Add("Units", typeof(string));
            table.Columns.Add("Method", typeof(string));
            table.Columns.Add("QualityControl", typeof(string));
            table.Columns.Add("ServiceCode", typeof(string));
            Field fld = new Field("BeginDateTime", 'C', 16, 0);
            table.Columns.Add(fld);
            fld = new Field("EndDateTime", 'C', 16, 0);
            table.Columns.Add(fld);
            table.Columns.Add("ValueCount", typeof(int));
            return fs;
        }

        private void AddSeriesToFeatureSet(Series series, IFeatureSet fs)
        {
            try
            {
                double lat = series.Site.Latitude;
                double lon = series.Site.Longitude;
                DotSpatial.Topology.Point pt = new DotSpatial.Topology.Point(lon, lat);
                IFeature newFeature = fs.AddFeature(pt);

                DataRow featureRow = newFeature.DataRow;
                featureRow["DataSource"] = series.Source.Organization;
                featureRow["SeriesID"] = (series.Id > 0) ? series.Id : newFeature.Fid;
                featureRow["SiteName"] = series.Site.Name;
                featureRow["Latitude"] = series.Site.Latitude;
                featureRow["Longitude"] = series.Site.Longitude;
                featureRow["SiteCode"] = series.Site.Code;
                featureRow["VariableName"] = series.Variable.Name;
                featureRow["DataType"] = series.Variable.DataType;
                featureRow["SampleMedium"] = series.Variable.SampleMedium;
                featureRow["VariableCode"] = series.Variable.Code;
                featureRow["Units"] = series.Variable.VariableUnit.Name;
                featureRow["Method"] = series.Method.Description;
                featureRow["QualityControl"] = series.QualityControlLevel.Definition;
                featureRow["BeginDateTime"] = ConvertTime(series.BeginDateTime);
                featureRow["EndDateTime"] = ConvertTime(series.EndDateTime);
                featureRow["ValueCount"] = series.ValueCount;
                featureRow["ServiceCode"] = series.Variable.Code.Substring(0, series.Variable.Code.IndexOf(":"));
            }
            catch { }
        }

        private IMapGroup FindGroupByName(DotSpatial.Controls.Map map, string groupLegendText)
        {
            foreach (IMapGroup item in map.GetAllGroups())
            {
                if (item.LegendText == groupLegendText)
                {
                    return (IMapGroup)item;
                }
            }
            return null;
        }

        private MapPointLayer FindLayerByName(DotSpatial.Controls.Map mainMap, string layerLegendText)
        {
            foreach (ILayer item in mainMap.GetAllLayers())
            {
                if (item is MapPointLayer && item.LegendText == layerLegendText)
                    return item as MapPointLayer;
            }
            return null;
        }

        /// <summary>
        /// Adds the theme feature set to the map
        /// </summary>
        /// <param name="fs">The theme feature set (this can be an in-memory feature set)</param>
        /// <param name="themeName">The theme name (this will appear as legend text)</param>
        /// <param name="mainMap">The main map</param>
        /// <returns>Created map point layer</returns>
        public MapPointLayer AddThemeToMap(IFeatureSet fs, string themeName, DotSpatial.Controls.Map mainMap)
        {
            //the url to get the icons
            string hisCentralUrl = Settings.Instance.SelectedHISCentralURL;
            
            //Find or create the 'Themes' map group
            IMapGroup themeGroup = FindGroupByName(mainMap, "Themes");
            if (themeGroup == null)
            {
                themeGroup = new MapGroup();
                themeGroup.LegendText = "Themes";
                themeGroup.IsVisible = true;
                mainMap.Layers.Add(themeGroup);
                //themeGroup.MapFrame = mainMap.MapFrame;
            }

            //remove any existing theme with the same name
            MapPointLayer existing = FindLayerByName(mainMap, themeName);
            if (existing != null)
            {
                mainMap.Layers.Remove(existing);
                themeGroup.Layers.Remove(existing);
            }

            SymbologyCreator sym = new SymbologyCreator(hisCentralUrl);
            MapPointLayer lay = sym.CreateSearchResultLayer(fs);
            lay.LegendText = themeName;

            //get the theme ID
            int themeID = GetThemeID(themeName);
            lay.Name = themeID.ToString();

            themeGroup.Layers.Add(lay);
            lay.SetParentItem(themeGroup);
            lay.MapFrame = mainMap.MapFrame;
            //lay.MapFrame = themeGroup.ParentMapFrame;

            return lay;
        }

        /// <summary>
        /// Rename the theme in the map and in the database
        /// </summary>
        /// <param name="themeID">The theme ID</param>
        public void RenameTheme(int themeID, string newThemeName)
        {
            string sqlQuery = string.Format("UPDATE DataThemeDescriptions SET ThemeName = '{0}' WHERE ThemeID = {1}", newThemeName, themeID);
            _db.ExecuteNonQuery(sqlQuery);
        }

        /// <summary>
        /// Given the theme name, get the theme ID (returns 0 if nothing found)
        /// </summary>
        public int GetThemeID(string themeName)
        {
            string sqlQuery = string.Format("SELECT ThemeID FROM DataThemeDescriptions WHERE ThemeName = '{0}'", themeName);
            object result = _db.ExecuteSingleOutput(sqlQuery);
            return (result != null) ? Convert.ToInt32(result) : 0;
        }

        public string GetThemeName(int themeID)
        {
            string sqlQuery = string.Format("SELECT ThemeName FROM DataThemeDescriptions WHERE ThemeID = {0}", themeID);
            object result = _db.ExecuteSingleOutput(sqlQuery);
            return result.ToString();
        }

        private string ConvertTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd hh:mm");
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
