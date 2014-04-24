using DevExpress.XtraGrid.Views.Grid;
using DotSpatial.Controls;
using System;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public class GridViewMapBase
	{
		protected readonly LayerManager _LayerManager;

		protected readonly GridView _GridView;

		protected readonly IMap _Map;

		public GridViewMapBase(IMap map, GridView gridView)
		{
			this._GridView = gridView;
			this._Map = map;
			this._LayerManager = this._Map.GetLayerManager();
		}
	}
}