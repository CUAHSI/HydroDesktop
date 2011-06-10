using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DotSpatial.Data;
using DotSpatial.Topology;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices;

namespace HydroDesktop.Search
{
	/// <summary>
	/// Helper class - converts the search results list to a GIS Feature set that can be
    /// shown in map
	/// </summary>
	public class SearchHelper
	{
        /// <summary>
        /// Converts a list of SeriesMetadata objects to a data table
        /// </summary>
        /// <param name="seriesList">the list of SeriesMetadata objects (from metadata cache 
        /// or from web service)</param>
        /// <returns>the data table</returns>
        public static DataTable SeriesListToDataTable(IList<SeriesDataCart> seriesList)
        {
            DataTable tab = new DataTable();
            tab.Columns.Add(new DataColumn("DataSource", typeof(string)));
            tab.Columns.Add(new DataColumn("SiteName", typeof(string)));
            tab.Columns.Add(new DataColumn("VarName", typeof(string)));
            tab.Columns.Add(new DataColumn("SiteCode", typeof(string)));
            tab.Columns.Add(new DataColumn("VarCode", typeof(string)));
            tab.Columns.Add(new DataColumn("ValueCount", typeof(int)));
            tab.Columns.Add(new DataColumn("StartDate", typeof(string)));
            tab.Columns.Add(new DataColumn("EndDate", typeof(string)));
            tab.Columns.Add(new DataColumn("ServiceURL", typeof(string)));
            tab.Columns.Add(new DataColumn("ServiceCode", typeof(string)));
            tab.Columns.Add(new DataColumn("DataType", typeof(string)));
            tab.Columns.Add(new DataColumn("ValueType", typeof(string)));
            tab.Columns.Add(new DataColumn("TimeUnits", typeof(string)));
            tab.Columns.Add(new DataColumn("TimeSupport", typeof(double)));
            tab.Columns.Add(new DataColumn("Latitude", typeof(double)));
            tab.Columns.Add(new DataColumn("Longitude", typeof(double)));
           
            foreach (SeriesDataCart series in seriesList)
            {
                DataRow row = tab.NewRow();
                PopulateDataRow(series, row);
                tab.Rows.Add(row);
            }

            return tab;
        }

        private static void PopulateDataRow(SeriesDataCart series, DataRow row)
        {
            row["DataSource"] = series.ServCode;
            row["SiteName"] = series.SiteName;
            row["VarName"] = series.VariableName;
            row["SiteCode"] = series.SiteCode;
            row["VarCode"] = series.VariableCode;
            row["ValueCount"] = series.ValueCount;
            row["StartDate"] = series.BeginDate.ToString("yyyy-MM-dd");
            row["EndDate"] = series.EndDate.ToString("yyyy-MM-dd");
            row["ServiceURL"] = series.ServURL;
            row["ServiceCode"] = series.ServCode;
            row["DataType"] = series.DataType;
            row["ValueType"] = series.ValueType;
            row["TimeUnits"] = series.TimeUnit;
            row["TimeSupport"] = series.TimeSupport;
            row["Latitude"] = series.Latitude;
            row["Longitude"] = series.Longitude;
        }
        
        /// <summary>
        /// Divides the search bounding box into several 'tiles' to prevent
        /// </summary>
        /// <param name="bigBoundingBox">the original bounding box</param>
        /// <param name="tileWidth">The tile width in decimal degrees</param>
        /// <param name="tileHeight">The tile height (south-north) in decimal degrees</param>
        /// <returns></returns>
        public static List<Box> CreateTiles(Box bigBoundingBox, double tileWidth, double tileHeight)
        {
            List<Box> tiles = new List<Box>();
            double fullWidth = Math.Abs(bigBoundingBox.xmax - bigBoundingBox.xmin);
            double fullHeight = Math.Abs(bigBoundingBox.ymax - bigBoundingBox.ymin);

            if (fullWidth < tileWidth || fullHeight < tileHeight)
            {
                tiles.Add(bigBoundingBox);
                return tiles;
            }

            double xll = bigBoundingBox.xmin; //x-coordinate of the tile's lower left corner
            double yll = bigBoundingBox.ymin; //y-coordinate of the tile's lower left corner
            int numColumns = (int)(Math.Ceiling(fullWidth / tileWidth));
            int numRows = (int)(Math.Ceiling(fullHeight / tileHeight));
            double lastTileWidth = fullWidth - ((numColumns - 1) * tileWidth);
            double lastTileHeight = fullHeight - ((numRows - 1) * tileHeight);
            int r = 0;
            int c = 0;

            for (r = 0; r < numRows; r++)
            {
                xll = bigBoundingBox.xmin;

                if (r == numRows - 1)
                {
                    tileHeight = lastTileHeight;
                }

                for (c = 0; c < numColumns; c++)
                {
                    Box newTile = new Box(0,0,0,0);
                    if (c == (numColumns - 1))
                    {
                        newTile = new Box(xll, xll + lastTileWidth, yll, yll + tileHeight);
                    }
                    else
                    {
                        newTile = new Box(xll, xll + tileWidth, yll, yll + tileHeight);
                    }
                    tiles.Add(newTile);
                    xll = xll + tileWidth;
                }
                yll = yll + tileHeight;
            }
            return tiles;
        }
        
        /// <summary>
		/// Creates a new point feature set from the list of data series
		/// info items. The coordinates of points are longitude and
		/// latitude in decimal degrees.
		/// </summary>
		/// <returns></returns>
		public static FeatureSet ToFeatureSet ( IList<SeriesDataCart> seriesList )
		{
			//display sites on the map
            FeatureSet fs = CreateEmptyFeatureSet();

			//add the series list to the feature set
			AddToFeatureSet ( seriesList, fs );
			return fs;
		}

		/// <summary>
		/// Creates a new point feature set from the list of data series
		/// info items. The coordinates of points are longitude and
		/// latitude in decimal degrees.
		/// <param name="seriesList">The input list of data series </param>
		/// <param name="polygons">A list of polygons. Only locations that are
		/// within the polygons will be included in the resulting feature set</param>
		/// </summary>
		/// <returns></returns>
		public static FeatureSet ToFeatureSet ( IList<SeriesDataCart> seriesList, IList<IFeature> polygons )
		{
            FeatureSet fs = CreateEmptyFeatureSet();
			AddToFeatureSet ( seriesList, fs, polygons );
			return fs;
		}

        /// <summary>
        /// Clips the list of series by the polygon
        /// </summary>
        /// <param name="polygon">the polygon shape</param>
        /// <returns>a new list of series metadata that is only within the polygon</returns>
        public static IList<SeriesDataCart> ClipByPolygon(IList<SeriesDataCart> fullSeriesList, IFeature polygon)
        {
            List<SeriesDataCart> newList = new List<SeriesDataCart>();
            
            foreach (SeriesDataCart series in fullSeriesList)
            {
                double lat = series.Latitude;
                double lon = series.Longitude;
                Coordinate coord = new Coordinate(lon, lat);
                if (polygon.Intersects(coord))
                {
                    newList.Add(series);
                }
            }
            return newList;
        }

        /// <summary>
        /// Adds the necessary attribute columns to the featureSet's attribute table
        /// </summary>
        /// <param name="fs"></param>
        private static FeatureSet CreateEmptyFeatureSet()
        {
            FeatureSet fs = new FeatureSet(FeatureType.Point);
            
            DataTable tab = fs.DataTable;
            tab.Columns.Add(new DataColumn("DataSource", typeof(string)));           
            tab.Columns.Add(new DataColumn("SiteName", typeof(string)));
            tab.Columns.Add(new DataColumn("VarName", typeof(string)));
            tab.Columns.Add(new DataColumn("SiteCode", typeof(string)));
            tab.Columns.Add(new DataColumn("VarCode", typeof(string)));
            tab.Columns.Add(new DataColumn("ValueCount", typeof(int)));
            tab.Columns.Add(new DataColumn("StartDate", typeof(string)));
            tab.Columns.Add(new DataColumn("EndDate", typeof(string)));
            tab.Columns.Add(new DataColumn("ServiceURL", typeof(string)));
            tab.Columns.Add(new DataColumn("ServiceCode", typeof(string)));
            tab.Columns.Add(new DataColumn("DataType", typeof(string)));
            tab.Columns.Add(new DataColumn("ValueType", typeof(string)));           
            tab.Columns.Add(new DataColumn("TimeUnits", typeof(string)));
            tab.Columns.Add(new DataColumn("TimeSupport", typeof(double)));
            tab.Columns.Add(new DataColumn("Latitude", typeof(double)));
            tab.Columns.Add(new DataColumn("Longitude", typeof(double)));

            return fs;
        }

		/// <summary>
		/// Adds sites from the list of data series
		/// to an existing feature set
		/// </summary>
		/// <param name="fs"></param>
		public static void AddToFeatureSet ( IList<SeriesDataCart> seriesList, IFeatureSet fs )
		{
			foreach ( SeriesDataCart series in seriesList )
			{
				double lat = series.Latitude;
				double lon = series.Longitude;
				Coordinate coord = new Coordinate ( lon, lat );

                Feature f = new Feature(FeatureType.Point, new Coordinate[] { coord });
                fs.Features.Add(f);

                DataRow row = f.DataRow;
                PopulateDataRow(series, row);
			}
		}

		/// <summary>
		/// Adds sites from the list of data series which are inside the polygons 
		/// to an existing feature set
		/// <param name="seriesList"></param>
		/// <param name="fs"></param>
		/// <param name="polygons"></param>
		/// </summary>
		public static void AddToFeatureSet ( IList<SeriesDataCart> seriesList, IFeatureSet fs, IList<IFeature> polygons )
		{
			if ( polygons.Count == 0 )
			{
				AddToFeatureSet ( seriesList, fs );
				return;
			}

			foreach ( SeriesDataCart series in seriesList )
			{
				double lat = series.Latitude;
				double lon = series.Longitude;
				Coordinate coord = new Coordinate ( lon, lat );

				if ( TestPointInPolygons ( coord, polygons ) == true )
				{

					Feature f = new Feature ( FeatureType.Point, new Coordinate[] { coord } );
					fs.Features.Add ( f );

					DataRow row = f.DataRow;
                    PopulateDataRow(series, row);
				}
			}
		}

		private static bool TestPointInPolygons ( Coordinate coord, IList<IFeature> polygons )
		{
			Point pt = new Point ( coord );
			foreach ( IFeature poly in polygons )
			{
				if ( poly.Intersects ( pt ) )
				{
					return true;
				}
			}
			return false;
		}
	}
}
