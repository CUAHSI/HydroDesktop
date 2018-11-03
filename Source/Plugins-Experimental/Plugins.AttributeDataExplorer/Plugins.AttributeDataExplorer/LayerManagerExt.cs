using DotSpatial.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public static class LayerManagerExt
	{
		private static Dictionary<IMap, LayerManager> _LayerManagers;

		private static object syncLock;

		static LayerManagerExt()
		{
			LayerManagerExt._LayerManagers = new Dictionary<IMap, LayerManager>();
			LayerManagerExt.syncLock = new object();
		}

		public static void ClearCache()
		{
			LayerManagerExt._LayerManagers.Clear();
		}

		public static LayerManager GetLayerManager(this IMap map)
		{
			LayerManager item;
			lock (LayerManagerExt.syncLock)
			{
				if (!LayerManagerExt._LayerManagers.ContainsKey(map))
				{
					LayerManager layerManager = new LayerManager(map);
					LayerManagerExt._LayerManagers.Add(map, layerManager);
					item = layerManager;
				}
				else
				{
					item = LayerManagerExt._LayerManagers[map];
				}
			}
			return item;
		}
	}
}