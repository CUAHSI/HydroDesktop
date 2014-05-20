using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using System;
using System.Collections.Generic;
using System.IO;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public class PersistLayoutInMemory : GridViewMapBase
	{
		private IFeatureLayer _LastFeatureLayer;

		private Dictionary<IFeatureLayer, MemoryStream> _PersistedSettings = new Dictionary<IFeatureLayer, MemoryStream>();

		public PersistLayoutInMemory(IMap map, GridView gridView) : base(map, gridView)
		{
			this._LayerManager.ActiveLayerChanged += new EventHandler(this.LayerManager_ActiveLayerChanged);
			this._GridView.GridControl.TextChanged += new EventHandler(this.GridControl_TextChanged);
		}

		private void GridControl_TextChanged(object sender, EventArgs e)
		{
			this.PersistLayout();
		}

		private void LayerManager_ActiveLayerChanged(object sender, EventArgs e)
		{
			IFeatureLayer activeFeatureLayer = this._LayerManager.ActiveFeatureLayer;
			this._LastFeatureLayer = activeFeatureLayer;
			if (activeFeatureLayer == null)
			{
				return;
			}
			MainForm.IsLayoutRestoring = true;
			if (this._PersistedSettings.ContainsKey(activeFeatureLayer))
			{
				MemoryStream item = this._PersistedSettings[activeFeatureLayer];
				this._GridView.RestoreLayoutFromStream(item);
				item.Position = (long)0;
			}
			MainForm.IsLayoutRestoring = false;
		}

		private void PersistLayout()
		{
			IFeatureLayer featureLayer = this._LastFeatureLayer;
			if (featureLayer == null)
			{
				return;
			}
			MemoryStream memoryStream = new MemoryStream();
			this._GridView.SaveLayoutToStream(memoryStream);
			memoryStream.Position = (long)0;
			this._PersistedSettings[featureLayer] = memoryStream;
		}
	}
}