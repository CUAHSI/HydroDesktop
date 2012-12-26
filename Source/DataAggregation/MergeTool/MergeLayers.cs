using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using HydroDesktop.Configuration;
using Hydrodesktop.Common;

namespace DataAggregation.MergeTool
{
    /// <summary>
    /// Contains Merge Layers logic
    /// </summary>
    class MergeLayers
    {
        public static IEnumerable<IFeatureLayer> GetDataSiteLayers(IMap map)
        {
            var dataSitesGroup = map.GetDataSitesLayer();
            if (dataSitesGroup == null) return Enumerable.Empty<IFeatureLayer>();
            return dataSitesGroup.Layers.OfType<IFeatureLayer>().Where(Aggregator.ContainsSeries);
        }

        public static IFeatureSet Merge(MergeData data)
        {
            if (data == null) throw new ArgumentNullException("data");

            IFeatureSet fs = new FeatureSet(FeatureType.Point);
            foreach (var layer in data.Layers)
            {
                // Add columns
                foreach (DataColumn column in layer.DataSet.DataTable.Columns)
                {
                    if (!fs.DataTable.Columns.Contains(column.ColumnName))
                    {
                        fs.DataTable.Columns.Add(column.ColumnName, column.DataType);
                    }
                }
                // Copy data
                foreach (var feature in layer.DataSet.Features)
                {
                    var f = fs.AddFeature(feature);
                    foreach (DataColumn column in layer.DataSet.DataTable.Columns)
                    {
                        f.DataRow[column.ColumnName] = feature.DataRow[column];
                    }
                }
            }
            var fileName = Path.Combine(Settings.Instance.CurrentProjectDirectory,
                                         string.Format("{0}.shp", data.NewLayerName));
            fs.Filename = fileName;
            return fs;
        }
    }

    class MergeData
    {
        public string NewLayerName { get; set; }
        public List<IFeatureLayer> Layers { get; set; }
        public IMap Map { get; set; }
    }
}
