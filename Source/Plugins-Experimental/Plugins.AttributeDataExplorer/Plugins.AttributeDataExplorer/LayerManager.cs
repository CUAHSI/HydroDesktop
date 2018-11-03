using DotSpatial.Controls;
using DotSpatial.Symbology;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public sealed class LayerManager
	{
		private ILayer _ActiveLayer;

		private ILayer _RemovedLayer;

		private readonly IMap _Map;

		public IFeatureLayer ActiveFeatureLayer
		{
			get
			{
				return this.ActiveLayer as IFeatureLayer;
			}
		}

		public ILayer ActiveLayer
		{
			get
			{
				return this._ActiveLayer;
			}
		}

		public EventHandler<LayerSelectedEventArgs> Layer_LayerSelected
		{
			get;
			set;
		}

		public LayerManager(IMap map)
		{
			this._Map = map;
			this.WireUpMapEvents();
			this.LayerSelected();
		}

		private void Layers_LayerRemoved(object sender, LayerEventArgs e)
		{
			if (e.Layer == this._ActiveLayer)
			{
				this._RemovedLayer = this._ActiveLayer;
				this._ActiveLayer = null;
				this.OnActiveLayerChanged(EventArgs.Empty);
			}
		}

		private void Layers_LayerSelected(object sender, LayerSelectedEventArgs e)
		{
			this.LayerSelected();
		}

		private void LayerSelected()
		{
			if (this._Map.Layers.SelectedLayer != this._ActiveLayer)
			{
				this._ActiveLayer = this._Map.Layers.SelectedLayer;
				this.OnActiveLayerChanged(EventArgs.Empty);
			}
		}

		private void MapFrame_LayerAdded(object sender, LayerEventArgs e)
		{
			if (this._Map.Layers.SelectedLayer == null && this._Map.Layers.Count == 1)
			{
				this._ActiveLayer = this._Map.Layers[0];
				this.OnActiveLayerChanged(EventArgs.Empty);
				return;
			}
			if (this._Map.Layers.SelectedLayer == null && this._RemovedLayer != null)
			{
				this._ActiveLayer = this._RemovedLayer;
				this._RemovedLayer = null;
				this.OnActiveLayerChanged(EventArgs.Empty);
			}
		}

		private void MapFrame_LayerSelected(object sender, LayerSelectedEventArgs e)
		{
			this.LayerSelected();
		}

		public void OnActiveLayerChanged(EventArgs ea)
		{
			if (this.ActiveLayerChanged != null)
			{
				this.ActiveLayerChanged(null, ea);
			}
		}

		public void WireUpMapEvents()
        {
            _Map.MapFrame.LayerSelected += MapFrame_LayerSelected;
            _Map.MapFrame.LayerAdded += MapFrame_LayerAdded;
            _Map.MapFrame.Layers.LayerSelected += Layers_LayerSelected;
            _Map.MapFrame.Layers.LayerRemoved += Layers_LayerRemoved;
        }

		public event EventHandler ActiveLayerChanged;
	}
}