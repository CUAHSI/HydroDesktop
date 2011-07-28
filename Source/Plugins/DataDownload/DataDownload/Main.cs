using System;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Symbology;

namespace HydroDesktop.DataDownload
{
    public class Main : Extension, IMapPlugin
    {
        #region Fields

        const string TableTabKey = "kHome";
        private IMapPluginArgs _mapArgs;

        #endregion

        public IMapPluginArgs MapArgs
        {
            get { return _mapArgs; }
            private set { _mapArgs = value; }
        }

        public void Initialize(IMapPluginArgs args)
        {
            if (args == null) throw new ArgumentNullException("args");
            MapArgs = args;

            // Initialize menu
            var btnDownload = new SimpleActionItem("Download", DoDownload)
                                  {RootKey = TableTabKey, GroupCaption = "Search"};
            args.AppManager.HeaderControl.Add(btnDownload);

            // Subscribe to events
            MapArgs.Map.LayerAdded += Map_LayerAdded;
            args.AppManager.SerializationManager.Deserializing += SerializationManager_Deserializing;
            //----

            Global.PluginEntryPoint = this;
        }
       
        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            MapArgs.Map.LayerAdded -= Map_LayerAdded;
            MapArgs.AppManager.HeaderControl.RemoveItems();
            Global.PluginEntryPoint = null;

            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }


        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            CheckLayers();
        }

        protected override void OnActivate()
        {
            CheckLayers();
            base.OnActivate();
        }

        private void CheckLayers()
        {
            foreach (var layer in MapArgs.Map.MapFrame.GetAllLayers().Where(SearchResultsLayerHelper.IsSearchLayer))
                SearchResultsLayerHelper.AddCustomFeaturesToSearchLayer((IFeatureLayer)layer, (Map)MapArgs.Map);
        }

        void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            if (SearchResultsLayerHelper.IsSearchLayer(e.Layer))
                SearchResultsLayerHelper.AddCustomFeaturesToSearchLayer((IFeatureLayer)e.Layer, (Map)MapArgs.Map);

            if (e.Layer is IGroup)
            {
                var group = (IGroup) e.Layer;
                group.LayerAdded += Map_LayerAdded; // TODO: unsubscribe from event when layer removed
            }
        }

        private void DoDownload(object sender, EventArgs args)
        {
            //Todo: implement download button
        }
    }
}
