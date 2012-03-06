using DotSpatial.Controls;
using DotSpatial.Symbology;
using HydroDesktop.Interfaces;

namespace DataAggregation
{
    /// <summary>
    /// Allow to aggregate data in features layers
    /// </summary>
    public class DataAggregationPlugin : Extension, IDataAggregationPlugin
    {
        #region Extension methods

        public override void Activate()
        {
            base.Activate();

            App.Map.LayerAdded += Map_LayerAdded;
            App.Map.Layers.LayerRemoved += Layers_LayerRemoved;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;

        }

        public override void Deactivate()
        {
            App.Map.LayerAdded -= Map_LayerAdded;
            App.Map.Layers.LayerRemoved -= Layers_LayerRemoved;
            App.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            foreach (var layer in App.Map.MapFrame.Layers)
                DettachLayerFromPlugin(layer);

            base.Deactivate();
        }

        #endregion

        #region Private methods

        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            foreach (var layer in App.Map.MapFrame.Layers)
                AttachLayerToPlugin(layer);
        }
       
        private void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            if (e.Layer == null) return; //occurs when moving layer

            AttachLayerToPlugin(e.Layer);
        }

        void Layers_LayerRemoved(object sender, LayerEventArgs e)
        {
            DettachLayerFromPlugin(e.Layer);
        }

        private void DettachLayerFromPlugin(ILayer layer)
        {
            Aggregator.RemoveContextMenu(layer);

            var group = layer as IGroup;
            if (group != null)
            {
                group.LayerAdded -= Map_LayerAdded;
                group.LayerRemoved -= Layers_LayerRemoved;

                foreach (var child in group.GetLayers())
                    DettachLayerFromPlugin(child);
            }
        }

        #endregion
       
        public void AttachLayerToPlugin(ILayer layer)
        {
            // Check for DataAggregation 
            if (Aggregator.CanAggregateLayer(layer))
            {
                Aggregator.UpdateContextMenu((IFeatureLayer)layer);
            }

            var group = layer as IGroup;
            if (group != null)
            {
                group.LayerAdded += Map_LayerAdded;
                group.LayerRemoved += Layers_LayerRemoved;

                foreach (var child in group.GetLayers())
                    AttachLayerToPlugin(child);
            }
        }
    }
}
