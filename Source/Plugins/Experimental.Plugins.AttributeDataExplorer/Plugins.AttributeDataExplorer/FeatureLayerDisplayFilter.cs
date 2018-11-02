using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using System;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public sealed class FeatureLayerDisplayFilter : GridViewMapBase
	{
		private object _CurrentDataSource;

		public FeatureLayerDisplayFilter(IMap map, GridView gridView) : base(map, gridView)
		{
			this._GridView.ColumnFilterChanged += new EventHandler(this.GridView_ColumnFilterChanged);
			this._GridView.GridControl.DataSourceChanged += new EventHandler(this.GridControl_DataSourceChanged);
		}

		private void GridControl_DataSourceChanged(object sender, EventArgs e)
		{
			this._CurrentDataSource = this._GridView.GridControl.DataSource;
		}

		private void GridView_ColumnFilterChanged(object sender, EventArgs e)
		{
			if (this._CurrentDataSource != this._GridView.GridControl.DataSource)
			{
				this._GridView.ClearColumnsFilter();
				return;
			}
			if (MainForm.IsLayoutRestoring)
			{
				return;
			}
			this.ToggleFeatureVisibility();
		}

		internal void ShowAllFeatures()
		{
			IMapFeatureLayer[] featureLayers = this._Map.GetFeatureLayers();
			for (int i = 0; i < (int)featureLayers.Length; i++)
			{
				IMapFeatureLayer mapFeatureLayer = featureLayers[i];
				if (mapFeatureLayer.DataSet.AttributesPopulated)
				{
					for (int j = 0; j < (int)mapFeatureLayer.DrawnStates.Length; j++)
					{
						mapFeatureLayer.DrawnStates[j].Visible = true;
					}
				}
			}
			this._Map.MapFrame.ResetBuffer();
		}

		private void ToggleFeatureVisibility()
		{
			IFeatureLayer activeFeatureLayer = this._LayerManager.ActiveFeatureLayer;
			if (activeFeatureLayer == null)
			{
				return;
			}
			if (activeFeatureLayer.DataSet.AttributesPopulated)
			{
				for (int i = 0; i < (int)activeFeatureLayer.DrawnStates.Length; i++)
				{
					int rowHandle = this._GridView.GetRowHandle(i);
					activeFeatureLayer.DrawnStates[i].Visible = rowHandle != -2147483648;
				}
			}
			this._Map.MapFrame.ResetBuffer();
		}
	}
}