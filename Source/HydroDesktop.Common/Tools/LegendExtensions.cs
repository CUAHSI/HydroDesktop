using System;
using System.Collections.Generic;
using DotSpatial.Controls;
using DotSpatial.Symbology;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Contains legend extensions
    /// </summary>
    public static class LegendExtensions
    {
        /// <summary>
        /// Process all legend items of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="legend">Legend</param>
        /// <param name="action">Action</param>
        public static void ForEachRecursively<T>(this ILegend legend, Action<T> action) where T : class
        {
            ForEachRecursively(legend.RootNodes, action);
        }

        private static void ForEachRecursively<T>(IEnumerable<ILegendItem> nodes, Action<T> action) where T : class
        {
            if (nodes == null) return;
            foreach (var legendItem in nodes)
            {
                var tt = legendItem as T;
                if (tt != null)
                {
                    action(tt);
                }
                ForEachRecursively(legendItem.LegendItems, action);
            }
        }
    }
}