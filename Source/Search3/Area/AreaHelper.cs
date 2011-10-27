using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Controls;

namespace Search3.Area
{
    static class AreaHelper
    {
        #region Fields

        private const string BASE_MAP_DATA_ROOT = "Base Map Data";

        #endregion

        #region Public methods

        public static IEnumerable<IMapPolygonLayer> GetAllPolygonLayers(IMap map)
        {
            if (map == null) throw new ArgumentNullException("map");

            return map.GetLayers().Where(layer => layer.LegendText == BASE_MAP_DATA_ROOT &&
                                                  layer is MapGroup)
                .SelectMany(layer => ((MapGroup) layer).GetLayers().OfType<IMapPolygonLayer>());
        }

        public static IEnumerable<IMapPolygonLayer> GetAllSelectedPolygonLayers(IMap map)
        {
            if (map == null) throw new ArgumentNullException("map");

            return GetAllPolygonLayers(map)
                  .Where(subLayer => subLayer.IsVisible && 
                         subLayer.IsSelected);
        }


        public static void SelectFirstVisiblePolygonLayer(IMap map)
        {
            if (map == null) throw new ArgumentNullException("map");

            var hasSelected = GetAllPolygonLayers(map).Where(subLayer => subLayer.IsVisible)
                                                      .Any(item => item.IsSelected);
            if (hasSelected)
                return;

            foreach (var layer in GetAllPolygonLayers(map).Where(subLayer => subLayer.IsVisible))
            {
                layer.IsSelected = true;
                map.Legend.RefreshNodes();
                break;
            }
        }

        #endregion
    }
}
