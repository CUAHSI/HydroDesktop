﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Topology;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.Search.Searching
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
            row["StartDate"] = series.BeginDate.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            row["EndDate"] = series.EndDate.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            row["ServiceURL"] = series.ServURL;
            row["ServiceCode"] = series.ServCode;
            row["DataType"] = series.DataType;
            row["ValueType"] = series.ValueType;
            row["SampleMed"] = series.SampleMedium;
            row["TimeUnits"] = series.TimeUnit;
            row["TimeSupport"] = series.TimeSupport;
            row["Latitude"] = series.Latitude;
            row["Longitude"] = series.Longitude;
            row["IsRegular"] = series.IsRegular;
            row["Units"] = series.VariableUnits;
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
            tab.Columns.Add(new DataColumn("SampleMed", typeof(string)));
            tab.Columns.Add(new DataColumn("TimeUnits", typeof(string)));
            tab.Columns.Add(new DataColumn("TimeSupport", typeof(double)));
            tab.Columns.Add(new DataColumn("Latitude", typeof(double)));
            tab.Columns.Add(new DataColumn("Longitude", typeof(double)));
            tab.Columns.Add(new DataColumn("IsRegular", typeof(bool)));
            tab.Columns.Add(new DataColumn("Units", typeof(string)));
          
            return fs;
        }

        
        /// <summary>
        /// Divides the search bounding box into several 'tiles' to prevent
        /// </summary>
        /// <param name="bigBoundingBox">the original bounding box</param>
        /// <param name="tileWidth">The tile width in decimal degrees</param>
        /// <param name="tileHeight">The tile height (south-north) in decimal degrees</param>
        /// <returns></returns>
        public static List<Extent> CreateTiles(Extent bigBoundingBox, double tileWidth, double tileHeight)
        {
            var tiles = new List<Extent>();
            double fullWidth = Math.Abs(bigBoundingBox.MaxX - bigBoundingBox.MinX);
            double fullHeight = Math.Abs(bigBoundingBox.MaxY - bigBoundingBox.MinY);

            if (fullWidth < tileWidth || fullHeight < tileHeight)
            {
                tiles.Add(bigBoundingBox);
                return tiles;
            }

            double yll = bigBoundingBox.MinY; //y-coordinate of the tile's lower left corner
            var numColumns = (int)(Math.Ceiling(fullWidth / tileWidth));
            var numRows = (int)(Math.Ceiling(fullHeight / tileHeight));
            var lastTileWidth = fullWidth - ((numColumns - 1) * tileWidth);
            var lastTileHeight = fullHeight - ((numRows - 1) * tileHeight);
            int r;

            for (r = 0; r < numRows; r++)
            {
                double xll = bigBoundingBox.MinX; //x-coordinate of the tile's lower left corner

                if (r == numRows - 1)
                {
                    tileHeight = lastTileHeight;
                }

                int c;
                for (c = 0; c < numColumns; c++)
                {
                    var newTile = c == (numColumns - 1) ? new Extent(xll, yll, xll + lastTileWidth, yll + tileHeight) :
                                                          new Extent(xll, yll, xll + tileWidth, yll + tileHeight);
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

	        var resultCollection = new List<SearchResultItem>();
            foreach(var dataCart in seriesList)
            {
                IFeatureSet featureSet;
                var searchItem = resultCollection.FirstOrDefault(item => item.ServiceCode == dataCart.ServCode);
                if (searchItem == null)
                {
                    featureSet =  CreateEmptyFeatureSet();
                    searchItem = new SearchResultItem(dataCart.ServCode, featureSet);
                    resultCollection.Add(searchItem);
                }
                featureSet = searchItem.FeatureSet;
                AddToFeatureSet(dataCart, featureSet);
            }

            return new SearchResult(resultCollection);
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
        /// Updates  BeginDate/EndDate/ValueCount in the SeriesDataCart to the user-specified range
        /// </summary>
        /// <param name="series">Series to update</param>
        /// <param name="startDate">User-specified startDate</param>
        /// <param name="endDate">User-specified endDate</param>
        public static void UpdateDataCartToDateInterval(SeriesDataCart series, DateTime startDate, DateTime endDate)
        {
            // Update BeginDate/EndDate/ValueCount to the user-specified range
            var seriesStartDate = series.BeginDate < startDate ? startDate : series.BeginDate;
            // Fix http://hydrodesktop.codeplex.com/workitem/8468
            // HIS Central sometimes doesn't contains actual end dates for datasets,
            // so always set end date of series to user-specified endDate.
            var seriesEndDate = endDate; //var seriesEndDate = series.EndDate > endDate ? endDate : series.EndDate;

            var serverDateRange = series.EndDate.Subtract(series.BeginDate);
            var userDateRange = seriesEndDate.Subtract(seriesStartDate);

            var userFromServerPercentage = serverDateRange.TotalDays > 0
                                               ? userDateRange.TotalDays / serverDateRange.TotalDays
                                               : 1.0;
            if (userFromServerPercentage > 1.0)
                userFromServerPercentage = 1.0;
            var esimatedValueCount = (int)(series.ValueCount * userFromServerPercentage);

            series.ValueCount = esimatedValueCount;
            series.BeginDate = seriesStartDate;
            series.EndDate = seriesEndDate;
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
	}
}
