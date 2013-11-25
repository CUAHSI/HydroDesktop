using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using Excel;
using ExcelExtension.Properties;

namespace ExcelExtension
{
    /// <summary>
    /// Excel Vector Provider. It let converts data from MS Excel files into FeatureSet.
    /// Points coordinates must be in Latitude/Longitude columns.
    /// </summary>
    public class ExcelVectorProvider : IVectorProvider
    {
        /// <summary>
        /// Opens the specified file
        /// </summary>
        /// <param name="fileName">Path to file.</param>
        /// <returns>Feature set from file data.</returns>
        /// <exception cref="Exception">Throws if no sheets in the file, or 
        /// Latitude or Longitude column not found.</exception>
        public IDataSet Open(string fileName)
        {
            var fs = new FeatureSet
                         {
                             Name = Path.GetFileNameWithoutExtension(fileName),
                             Filename = fileName
                         };
            var extension = Path.GetExtension(fileName);
            IExcelDataReader reader = null;
            var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                switch (extension)
                {
                    case ".xls":
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        break;
                    case ".xlsx":
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        break;
                    default:
                        goto case ".xls";
                }

                reader.IsFirstRowAsColumnNames = true;
                var ds = reader.AsDataSet();
                if (ds.Tables.Count == 0)
                {
                    throw new Exception("There is no sheets in the file");
                }

                // Find first table (sheet) with Latitude/Longitude columns.
                DataTable table = null;
                int latColumnIndex = -1;
                int lngColumnIndex = -1;
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    table = ds.Tables[i];
                    latColumnIndex = table.Columns.IndexOf("Latitude");
                    lngColumnIndex = table.Columns.IndexOf("Longitude");
                    if (latColumnIndex >= 0 && lngColumnIndex >=0)
                    {
                        break;
                    }
                }
                if (latColumnIndex == -1 || lngColumnIndex == -1)
                {
                    throw new Exception("Latitude or Longitude column not found");
                }
                Debug.Assert(table != null);

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    string sFieldName = table.Columns[i].ColumnName;
                    Type type = table.Columns[i].DataType;

                    int uniqueNumber = 1;
                    string uniqueName = sFieldName;
                    while (fs.DataTable.Columns.Contains(uniqueName))
                    {
                        uniqueName = sFieldName + uniqueNumber;
                        uniqueNumber++;
                    }
                    fs.DataTable.Columns.Add(new DataColumn(uniqueName, type));
                }
                reader.Read();

                while (reader.Read())
                {
                    var lat = reader.GetDouble(latColumnIndex);
                    var lng = reader.GetDouble(lngColumnIndex);
                    IGeometry geometry = new Point(lng, lat);

                    IFeature feature = new Feature(geometry);
                    feature.DataRow = fs.DataTable.NewRow();
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        object value = reader.GetValue(j) ?? DBNull.Value;
                        feature.DataRow[j] = value;
                    }
                    fs.Features.Add(feature);

                }
            }finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                stream.Close();
            }

            fs.Projection = ProjectionInfo.FromEsriString(Resources.wgs_84_esri_string);

            return fs;
        }

        /// <summary>
        /// Gets a dialog read filter that lists each of the file type descriptions and file extensions, delimited
        ///             by the | symbol.  Each will appear in DotSpatial's open file dialog filter, preceded by the name provided
        ///             on this object.
        /// </summary>
        public string DialogReadFilter
        {
            get
            {
                return "Excel files (*.xlsx,*.xls)|*.xlsx;*.xls"; 
            }
        }

        /// <summary>
        /// Gets a dialog filter that lists each of the file type descriptions and extensions for a Save File Dialog.
        ///             Each will appear in DotSpatial's open file dialog filter, preceded by the name provided on this object.
        ///             In addition, the same extension mapping will be used in order to pair a string driver code to the
        ///             extension.
        /// </summary>
        public string DialogWriteFilter
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets a preferably short name that identifies this data provider.  Example might be GDAL.
        ///             This will be prepended to each of the DialogReadFilter members from this plugin.
        /// </summary>
        public string Name
        {
            get { return "Excel data provider"; }
        }

        /// <summary>
        /// Gets or sets the progress handler to use.
        /// </summary>
        public IProgressHandler ProgressHandler
        {
            get;set;
        }

        /// <summary>
        /// This provides a basic description of what your provider does.
        /// </summary>
        public string Description
        {
            get { return "MS Excel data"; }
        }

        /// <summary>
        /// This create new method implies that this provider has the priority for creating a new file.
        ///             An instance of the dataset should be created and then returned.  By this time, the fileName
        ///             will already be checked to see if it exists, and deleted if the user wants to overwrite it.
        /// </summary>
        /// <param name="fileName">The string fileName for the new instance</param><param name="featureType">Point, Line, Polygon etc.  Sometimes this will be specified, sometimes it will be "Unspecified"</param><param name="inRam">Boolean, true if the dataset should attempt to store data entirely in ram</param><param name="progressHandler">An IProgressHandler for status messages.</param>
        /// <returns>
        /// An IRaster
        /// </returns>
        public IFeatureSet CreateNew(string fileName, FeatureType featureType, bool inRam, IProgressHandler progressHandler)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This tests the specified file in order to determine what type of vector the file contains.
        ///             This returns unspecified if the file format is not supported by this provider.
        /// </summary>
        /// <param name="fileName">The string fileName to test</param>
        /// <returns>
        /// A FeatureType clarifying what sort of features are stored on the data type.
        /// </returns>
        FeatureType IVectorProvider.GetFeatureType(string fileName)
        {
            return FeatureType.MultiPoint;
        }
    }

}

