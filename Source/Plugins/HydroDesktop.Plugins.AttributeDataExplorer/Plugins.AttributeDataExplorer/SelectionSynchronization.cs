using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public class SelectionSynchronization : GridViewMapBase
	{
		public bool IsUserMakingSelection
		{
			get;
			set;
		}

		public SelectionSynchronization(IMap map, GridView gridView) : base(map, gridView)
		{
			this._GridView.SelectionChanged += new SelectionChangedEventHandler(this.GridView_SelectionChanged);
			this.WireUpMapEvents();
			this._LayerManager.ActiveLayerChanged += new EventHandler(this.LayerManager_ActiveLayerChanged);
		}

		private int FindRowHandleByRowObject(GridView view, object row)
		{
			if (row != null)
			{
				for (int i = 0; i < view.DataRowCount; i++)
				{
					if (row.Equals(view.GetRow(i)))
					{
						return i;
					}
				}
			}
			return -2147483648;
		}

		private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add && e.ControllerRow == 0)
			{
				return;
			}
			if (MainForm.IsLayoutRestoring)
			{
				return;
			}
			this.IsUserMakingSelection = true;
			IFeatureLayer activeFeatureLayer = this._LayerManager.ActiveFeatureLayer;
			if (activeFeatureLayer == null)
			{
				return;
			}
			activeFeatureLayer.Selection.SuspendChanges();
			activeFeatureLayer.UnSelectAll();
			if (this._GridView.SelectedRowsCount > 0 && activeFeatureLayer.DataSet.AttributesPopulated)
			{
				int[] selectedRows = this._GridView.GetSelectedRows();
				for (int i = 0; i < (int)selectedRows.Length; i++)
				{
					int num = selectedRows[i];
					activeFeatureLayer.Select(this._GridView.GetDataSourceRowIndex(num));
				}
			}
			activeFeatureLayer.Selection.ResumeChanges();
			this.IsUserMakingSelection = false;
		}

		private void LayerManager_ActiveLayerChanged(object sender, EventArgs e)
		{
			this.ShowSelectionFromCurrentLayer(this._Map);
		}

		private void MapFrame_SelectionChanged(object sender, EventArgs e)
		{
			this.ShowSelectionFromCurrentLayer(this._Map);
		}

		public void ShowSelectionFromCurrentLayer(IMap map)
		{
			if (this.IsUserMakingSelection)
			{
				return;
			}
			if (this._GridView.RowCount == 0)
			{
				return;
			}
			IFeatureLayer activeFeatureLayer = this._LayerManager.ActiveFeatureLayer;
			if (activeFeatureLayer == null)
			{
				return;
			}
			List<IFeature> featureList = activeFeatureLayer.Selection.ToFeatureList();
			this._GridView.BeginSelection();
			this._GridView.ClearSelection();
			try
			{
				foreach (IFeature feature in featureList)
				{
					int num = feature.DataRow.Table.Rows.IndexOf(feature.DataRow);
					this._GridView.SelectRow(this._GridView.GetRowHandle(num));
				}
			}
			finally
			{
				this._GridView.SelectionChanged -= new SelectionChangedEventHandler(this.GridView_SelectionChanged);
				this._GridView.EndSelection();
				this._GridView.SelectionChanged += new SelectionChangedEventHandler(this.GridView_SelectionChanged);
			}
		}

        public void WireUpMapEvents()
        {
            _Map.MapFrame.SelectionChanged += MapFrame_SelectionChanged;
        }
	}
}