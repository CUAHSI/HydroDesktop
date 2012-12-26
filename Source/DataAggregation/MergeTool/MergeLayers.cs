using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Symbology;
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
            return dataSitesGroup.Layers.OfType<IFeatureLayer>();
        }

        public static void Merge(MergeData data)
        {
            if (data == null) throw new ArgumentNullException("data");
        }
    }

    class MergeData
    {
        public string NewLayerName { get; set; }
        public List<IFeatureLayer> Layers { get; set; }
    }
}
