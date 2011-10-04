using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using DotSpatial.Data;
using DotSpatial.Topology;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices;

namespace Search3
{
	/// <summary>
	/// Helper class - converts the search results list to a GIS Feature set that can be
    /// shown in map
	/// </summary>
	public static class SearchHelper
	{
        private static void PopulateDataRow(SeriesDataCart series, DataRow row)
        {
            row["DataSource"] = series.ServCode;
            row["SiteName"] = series.SiteName;
            row["VarName"] = series.VariableName;
            row["SiteCode"] = series.SiteCode;
            row["VarCode"] = series.VariableCode;
            row["Keyword"] = series.ConceptKeyword;
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
            var tiles = new List<Box>();
            double fullWidth = Math.Abs(bigBoundingBox.xmax - bigBoundingBox.xmin);
            double fullHeight = Math.Abs(bigBoundingBox.ymax - bigBoundingBox.ymin);

            if (fullWidth < tileWidth || fullHeight < tileHeight)
            {
                tiles.Add(bigBoundingBox);
                return tiles;
            }

            double yll = bigBoundingBox.ymin; //y-coordinate of the tile's lower left corner
            var numColumns = (int)(Math.Ceiling(fullWidth / tileWidth));
            var numRows = (int)(Math.Ceiling(fullHeight / tileHeight));
            var lastTileWidth = fullWidth - ((numColumns - 1) * tileWidth);
            var lastTileHeight = fullHeight - ((numRows - 1) * tileHeight);
            int r;

            for (r = 0; r < numRows; r++)
            {
                double xll = bigBoundingBox.xmin; //x-coordinate of the tile's lower left corner

                if (r == numRows - 1)
                {
                    tileHeight = lastTileHeight;
                }

                int c;
                for (c = 0; c < numColumns; c++)
                {
                    Box newTile = c == (numColumns - 1) ? new Box(xll, xll + lastTileWidth, yll, yll + tileHeight) : 
                                                          new Box(xll, xll + tileWidth, yll, yll + tileHeight);
                    tiles.Add(newTile);
                    xll = xll + tileWidth;
                }
                yll = yll + tileHeight;
            }
            return tiles;
        }

	    public static SearchResult ToFeatureSetsByDataSource(IEnumerable<SeriesDataCart> seriesList)
        {
            if (seriesList == null) throw new ArgumentNullException("seriesList");

            var result = new Dictionary<string, IFeatureSet>();
            foreach(var dataCart in seriesList)
            {
                IFeatureSet featureSet;
                if (!result.TryGetValue(dataCart.ServCode, out featureSet))
                {
                    featureSet =  CreateEmptyFeatureSet();
                    result.Add(dataCart.ServCode, featureSet);
                }

                AddToFeatureSet(dataCart, featureSet);
            }

            return new SearchResult(result);
        }

	    /// <summary>
	    /// Clips the list of series by the polygon
	    /// </summary>
	    /// <param name="fullSeriesList">List of series</param>
	    /// <param name="polygon">the polygon shape</param>
	    /// <returns>a new list of series metadata that is only within the polygon</returns>
	    public static IEnumerable<SeriesDataCart> ClipByPolygon(IEnumerable<SeriesDataCart> fullSeriesList, IFeature polygon)
        {
            var newList = new List<SeriesDataCart>();
            
            foreach (SeriesDataCart series in fullSeriesList)
            {
                double lat = series.Latitude;
                double lon = series.Longitude;
                var coord = new Coordinate(lon, lat);
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
	    private static FeatureSet CreateEmptyFeatureSet()
        {
            var fs = new FeatureSet(FeatureType.Point);
            
            var tab = fs.DataTable;
            tab.Columns.Add(new DataColumn("DataSource", typeof(string)));           
            tab.Columns.Add(new DataColumn("SiteName", typeof(string)));
            tab.Columns.Add(new DataColumn("VarName", typeof(string)));
            tab.Columns.Add(new DataColumn("SiteCode", typeof(string)));
            tab.Columns.Add(new DataColumn("VarCode", typeof(string)));
            tab.Columns.Add(new DataColumn("Keyword", typeof(string)));
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
		private static void AddToFeatureSet ( IEnumerable<SeriesDataCart> seriesList, IFeatureSet fs )
		{
		    if (seriesList == null) throw new ArgumentNullException("seriesList");

		    foreach (var series in seriesList )
		    {
		        AddToFeatureSet(series, fs);
			}
		}

	    /// <summary>
	    /// Adds series to an existing feature set
	    /// </summary>
	    /// <param name="series">Series</param>
	    /// <param name="fs">Feature set</param>
	    private static void AddToFeatureSet(SeriesDataCart series, IFeatureSet fs)
        {
            double lat = series.Latitude;
            double lon = series.Longitude;
            var coord = new Coordinate(lon, lat);

            var f = new Feature(FeatureType.Point, new[] {coord});
            fs.Features.Add(f);

            var row = f.DataRow;
            PopulateDataRow(series, row);
        }

	    /// <summary>
		/// Adds sites from the list of data series which are inside the polygons 
		/// to an existing feature set
		/// <param name="seriesList"></param>
		/// <param name="fs"></param>
		/// <param name="polygons"></param>
		/// </summary>
	    private static void AddToFeatureSet ( IEnumerable<SeriesDataCart> seriesList, IFeatureSet fs, IList<IFeature> polygons )
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
				var coord = new Coordinate ( lon, lat );

				if ( TestPointInPolygons ( coord, polygons ) )
				{

					var f = new Feature ( FeatureType.Point, new[] { coord } );
					fs.Features.Add ( f );

					DataRow row = f.DataRow;
                    PopulateDataRow(series, row);
				}
			}
		}

		private static bool TestPointInPolygons ( Coordinate coord, IEnumerable<IFeature> polygons )
		{
			var pt = new Point ( coord );
		    return polygons.Any(poly => poly.Intersects(pt));
		}
	}
}
