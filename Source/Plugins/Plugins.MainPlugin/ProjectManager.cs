﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Drawing;
using System.Timers;

namespace HydroDesktop.Plugins.MainPlugin
{
    public class ProjectManager
    {
        /// <summary>
        /// The main app manager
        /// </summary>
        public AppManager App { get; private set; }

        /// <summary>
        /// Creates a new instance of the project manager
        /// </summary>
        /// <param name="mainApp"></param>
        public ProjectManager(AppManager mainApp) {
            App = mainApp;
        }

        public static ProjectionInfo DefaultProjection { get { return KnownCoordinateSystems.Projected.World.WebMercator; } }

        //sets the map extent to continental U.S
        private void SetDefaultMapExtents() {
            App.Map.ViewExtents = DefaultMapExtents().ToExtent();
        }

        public static Envelope DefaultMapExtents() {
            Envelope _defaultMapExtent = new Envelope(-130, -60, 10, 55);


            double[] xy = new double[4];
            xy[0] = _defaultMapExtent.Minimum.X;
            xy[1] = _defaultMapExtent.Minimum.Y;
            xy[2] = _defaultMapExtent.Maximum.X;
            xy[3] = _defaultMapExtent.Maximum.Y;
            double[] z = new double[] { 0, 0 };
            ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
            Reproject.ReprojectPoints(xy, z, wgs84, DefaultProjection, 0, 2);

            return new Envelope(xy[0], xy[2], xy[1], xy[3]);
        }

        private static void CopyStream(Stream input, Stream output) {
            byte[] buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Check if the path is a valid SQLite database
        /// This function returns false, if the SQLite db
        /// file doesn't exist or if the file size is 0 Bytes
        /// </summary>
        public static bool DatabaseExists(string dbPath) {
            return SQLiteHelper.DatabaseExists(dbPath);
        }

        /// <summary>
        /// To get the SQLite database path given the SQLite connection string
        /// </summary>
        public static string GetSQLiteFileName(string sqliteConnString) {
            return SQLiteHelper.GetSQLiteFileName(sqliteConnString);
        }
        /// <summary>
        /// To get the full SQLite connection string given the SQLite database path
        /// </summary>
        public static string GetSQLiteConnectionString(string dbFileName) {
            return SQLiteHelper.GetSQLiteConnectionString(dbFileName);
        }

        /// <summary>
        /// Create the default .SQLITE database in the user-specified path
        /// </summary>
        /// <returns>true if database was created, false otherwise</returns>
        public static Boolean CreateNewDatabase(string dbPath) {
            //to create the default.sqlite database file using the SQLiteHelper method
            return SQLiteHelper.CreateSQLiteDatabase(dbPath);
        }

        /// <summary>
        /// Checks if the two paths are on the same drive.
        /// </summary>
        /// <param name="path1">the first path</param>
        /// <param name="path2">the second path</param>
        /// <returns>true if the two paths are on same drive</returns>
        private static Boolean IsSameDrive(string path1, string path2) {
            if (Path.IsPathRooted(path1) && Path.IsPathRooted(path2) && !path1.StartsWith("\\\\") && !path2.StartsWith("\\\\"))
            {
                if (Path.GetPathRoot(path1) == Path.GetPathRoot(path2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Opens a project and updates the maps
        /// </summary>
        public void OpenProject(string projectFileName) {
            App.ProgressHandler.Progress("Opening Project", 0, "Opening Project");
            App.SerializationManager.OpenProject(projectFileName);
            App.ProgressHandler.Progress("Project opened", 0, "");
        }

        public void ProjectToGeoLocation()
        { 
            try
            {
                string locationsRequest = "http://ip-api.com/xml";
                double[] xy = GetLocation(locationsRequest);
                if (xy[0] == 0.0 || xy[1] == 0.0)
                    return;
                xy = LatLonReproject(xy[1], xy[0]);
                App.Map.MapFrame.ViewExtents = (new Envelope(xy[0] - 420000, xy[0] + 420000, xy[1] - 420000, xy[1] + 420000)).ToExtent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
          
        }

     
        private static double[] GetLocation(string requestUrl)
        {
            double[] location = new double[2];
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
 
                using (var reader = XmlReader.Create(response.GetResponseStream()))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "lat")
                            {
                                reader.Read();
                                location[0] = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                            }
                            else if (reader.Name == "lon")
                            {
                                reader.Read();
                                location[1] = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
                return location;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private double[] LatLonReproject(double x, double y)
        {
            double[] xy = new double[2] { x, y };

            //Change y coordinate to be less than 90 degrees to prevent a bug.
            if (xy[1] >= 90) xy[1] = 89.9;
            if (xy[1] <= -90) xy[1] = -89.9;

            //Need to convert points to proper projection. Currently describe WGS84 points which may or may not be accurate.
            bool isWgs84;

            String wgs84String = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            String mapProjEsriString = App.Map.Projection.ToEsriString();
            isWgs84 = (mapProjEsriString.Equals(wgs84String));

            //If the projection is not WGS84, then convert points to properly describe desired location.
            if (!isWgs84)
            {
                double[] z = new double[1];
                ProjectionInfo wgs84Projection = ProjectionInfo.FromEsriString(wgs84String);
                ProjectionInfo currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);
                Reproject.ReprojectPoints(xy, z, wgs84Projection, currentMapProjection, 0, 1);
            }

            //Return array with 1 x and 1 y value.
            return xy;
        }

       



        private void DisableProgressReportingForLayers() {
            foreach (IMapLayer layer in App.Map.MapFrame.GetAllLayers())
            {
                layer.ProgressHandler = null;

                MapPolygonLayer polyLay = layer as MapPolygonLayer;
                if (polyLay != null)
                {
                    polyLay.ProgressReportingEnabled = false;
                }
            }
            App.Map.ProgressHandler = null;
        }

        public void OpeningProject() {
            if (App.SerializationManager.CurrentProjectFile == null) return;

            //todo: change the configuration settings paths
            string projectFile = App.SerializationManager.CurrentProjectFile;
            Settings.Instance.CurrentProjectFile = App.SerializationManager.CurrentProjectFile;

            //also need to set-up the DB
            string dbFileName = Path.ChangeExtension(projectFile, "sqlite");
            string cacheDbFileName = dbFileName.Replace(".sqlite", "_cache.sqlite");

            if (!ValidateDatabase(dbFileName, DatabaseType.DefaulDatabase))
            {
                SQLiteHelper.CreateSQLiteDatabase(dbFileName);
            }
            if (!ValidateDatabase(cacheDbFileName, DatabaseType.MetadataCacheDatabase))
            {
                SQLiteHelper.CreateMetadataCacheDb(cacheDbFileName);
            }
            Settings.Instance.DataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(dbFileName);
            Settings.Instance.MetadataCacheConnectionString = SQLiteHelper.GetSQLiteConnectionString(cacheDbFileName);
        }

        //checks if the db exists. Also checks the db schema
        private bool ValidateDatabase(string dbFileName, DatabaseType dbType) {
            //check if db exists
            if (SQLiteHelper.DatabaseExists(dbFileName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a new 'empty' project
        /// </summary>
        public void CreateEmptyProject() {
            App.SerializationManager.New();
        }

        //saves the current HydroDesktop project file to the user specified location
        public void SavingProject() {
            string projectFileName = App.SerializationManager.CurrentProjectFile;

            Settings.Instance.AddFileToRecentFiles(projectFileName);

            string newProjectDirectory = Path.GetDirectoryName(projectFileName);

            App.ProgressHandler.Progress("Saving Project " + projectFileName, 0, "");
            Application.DoEvents();

            //are we saving or are we doing 'save as' ?
            if (projectFileName != Settings.Instance.CurrentProjectFile)
            {

                //also create a copy of the .sqlite database
                string newDbPath = Path.ChangeExtension(projectFileName, ".sqlite");

                //current database path
                string currentDbPath = SQLiteHelper.GetSQLiteFileName(Settings.Instance.DataRepositoryConnectionString);
                //copy db to new path. If no db exists, create new db in the new location
                if (SQLiteHelper.DatabaseExists(currentDbPath))
                {
                    File.Copy(currentDbPath, newDbPath, true);
                }
                else
                {
                    CreateNewDatabase(newDbPath);
                }
                //create a copy of the metadata cache (_cache.sqlite) database
                string newCachePath = projectFileName.Replace(".dspx", "_cache.sqlite");

                //current database path
                string currentCachePath = SQLiteHelper.GetSQLiteFileName(Settings.Instance.MetadataCacheConnectionString);
                //copy db to new path. If no db exists, create new db in the new location
                if (SQLiteHelper.DatabaseExists(currentCachePath))
                {
                    File.Copy(currentCachePath, newCachePath, true);
                }
                else
                {
                    SQLiteHelper.CreateMetadataCacheDb(newCachePath);
                }

                //TODO: need to trigger a DatabaseChanged event (Settings.Instance.DatabaseChanged..)

                //update application level database configuration settings
                Settings.Instance.DataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(newDbPath);
                Settings.Instance.MetadataCacheConnectionString = SQLiteHelper.GetSQLiteConnectionString(newCachePath);
                Settings.Instance.CurrentProjectFile = App.SerializationManager.CurrentProjectFile;

                //Also save the files of all map layers 

                string projDir = App.SerializationManager.CurrentProjectDirectory;

                foreach (IMapLayer layer in App.Map.MapFrame.GetAllLayers())
                {
                    var fl = layer as IMapFeatureLayer;
                    if (fl != null)
                    {
                        if (!String.IsNullOrEmpty(fl.DataSet.Filename))
                        {
                            var fileName = Path.GetFileName(fl.DataSet.Filename);
                            if (fileName != null)
                            {
                                fl.DataSet.SaveAs(Path.Combine(projDir, fileName), true);
                            }
                        }
                    }

                    var rl = layer as IMapRasterLayer;
                    if (rl != null)
                    {
                        var fileName = Path.GetFileName(rl.DataSet.Filename);
                        if (fileName != null)
                        {
                            rl.DataSet.SaveAs(Path.Combine(projDir, fileName));
                            rl.DataSet.Filename = fileName; // Save relative path
                        }
                    }
                }
            }
            App.ProgressHandler.Progress(String.Empty, 0, String.Empty);
        }

    }
}
