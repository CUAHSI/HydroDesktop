using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Controls;

namespace Search3.Area
{
    static class AreaHelper
    {
        #region Public methods

        public static IEnumerable<IMapPolygonLayer> GetAllPolygonLayers(Map map)
        {
            if (map == null) throw new ArgumentNullException("map");

            return map.GetAllLayers().OfType<IMapPolygonLayer>();
        }

        public static IEnumerable<IMapPolygonLayer> GetAllSelectedPolygonLayers(Map map)
        {
            if (map == null) throw new ArgumentNullException("map");

            return GetAllPolygonLayers(map)
                  .Where(subLayer => subLayer.IsVisible && 
                         subLayer.IsSelected);
        }


        public static void SelectFirstVisiblePolygonLayer(Map map)
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
