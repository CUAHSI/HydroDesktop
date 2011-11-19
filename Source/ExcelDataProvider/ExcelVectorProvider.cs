using System.Data;
using System.Diagnostics;
using System.IO;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using Excel;
using System;
using ExcelExtension.Properties;

namespace ExcelExtension
{
    /// <summary>
    /// Excel Vector Provider. It let converts data from MS Excel files into FeatureSet.
    /// Points coordinates msut be in Latitude/Longitude columns.
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
                        object value = reader.GetValue(j);
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
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

        public string DialogReadFilter
        {
            get
            {
                return "Excel files (*.xlsx,*.xls)|*.xlsx;*.xls"; 
            }
        }

        public string DialogWriteFilter
        {
            get { return string.Empty; }
        }

        public string Name
        {
            get { return "Excel data provider"; }
        }

        public IProgressHandler ProgressHandler
        {
            get;set;
        }

        public string Description
        {
            get { return "MS Excel data"; }
        }

        public IFeatureSet CreateNew(string fileName, FeatureType featureType, bool inRam, IProgressHandler progressHandler)
        {
            throw new NotSupportedException();
        }

        FeatureType IVectorProvider.GetFeatureType(string fileName)
        {
            return FeatureType.MultiPoint;
        }
    }

}

