using System;
using System.Diagnostics.Contracts;
using System.Linq;
using DotSpatial.Controls;

namespace Hydrodesktop.Common
{
    /// <summary>
    /// Tools for 'Data Sites Layer'
    /// </summary>
    public static class DataSitesLayerTools
    {
        /// <summary>
        /// Get Data Sites Layer for given map
        /// </summary>
        /// <param name="map">Map</param>
        /// <param name="createIfNotExists">Create data sites layer, if it not exists.</param>
        /// <returns>Data Sites Layer.</returns>
        public static IMapGroup GetDataSitesLayer(this IMap map, bool createIfNotExists = false)
        {
            if (map == null) throw new ArgumentNullException("map");
            Contract.EndContractBlock();

            var layerName = LayerConstants.SearchGroupName;
            var layer = FindGroupLayerByName(map, layerName);
            if (layer == null && createIfNotExists)
            {
                layer = new MapGroup(map, layerName);
            }
            return layer;
        }

        private static IMapGroup FindGroupLayerByName(IMap map, string layerName)
        {
            return map.Layers
                .OfType<IMapGroup>()
                .FirstOrDefault(group => group.LegendText.ToLower() == layerName.ToLower());
        }
    }
}