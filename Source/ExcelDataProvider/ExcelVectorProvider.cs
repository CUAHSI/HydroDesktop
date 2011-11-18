using System.Data;
using System.IO;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using System.Linq;
using Excel;
using System;
using ExcelExtension.Properties;

namespace ExcelExtension
{
    public class ExcelVectorProvider : IVectorProvider
    {
        internal static string[] Extensions = { ".xls", ".xlsx" };

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
                var table = ds.Tables[0];
                int latColumnIndex = table.Columns.IndexOf("Latitude");
                int lngColumnIndex = table.Columns.IndexOf("Longitude");
                if (latColumnIndex == -1 || lngColumnIndex == -1)
                {
                    throw new Exception("Latitude or Longitude column not found");
                }

                for (int i = 0; i < reader.FieldCount; i++)
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
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object value = reader[i];
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        feature.DataRow[i] = value;
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
                return "MS Excel files|*.xlsx;*.xls"; 
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
            return null;
        }

        FeatureType IVectorProvider.GetFeatureType(string fileName)
        {
            return FeatureType.MultiPoint;
        }
    }

}

