using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data.Forms;
using DotSpatial.Symbology;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.SearchLayersProcessing;

namespace HydroDesktop.DataDownload
{
    public class Main : Extension, IMapPlugin
    {
        #region Fields

        const string TableTabKey = "kHome";
        private IMapPluginArgs _mapArgs;
        private readonly SearchLayersPostProcessor _searchLayersPostProcessor = new SearchLayersPostProcessor();

        #endregion

        /// <summary>
        /// Args with wich this plug-in was activated
        /// </summary>
        public IMapPluginArgs MapArgs
        {
            get { return _mapArgs; }
            private set { _mapArgs = value; }
        }

        /// <summary>
        /// Download manager
        /// </summary>
        public DownloadManager DownloadManager
        {
            get { return DownloadManager.Instance; }
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
            foreach (var layer in MapArgs.Map.MapFrame.GetAllLayers())
                _searchLayersPostProcessor.AddCustomFeaturesToSearchLayer(layer, (Map)MapArgs.Map);
        }

        void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            _searchLayersPostProcessor.AddCustomFeaturesToSearchLayer(e.Layer, (Map)MapArgs.Map);

            if (e.Layer is IGroup)
            {
                var group = (IGroup) e.Layer;
                group.LayerAdded += Map_LayerAdded; // TODO: unsubscribe from event when layer removed
            }
        }

        private void DoDownload(object sender, EventArgs args)
        {
             foreach (var layer in MapArgs.Map.MapFrame.GetAllLayers())
             {
                 if (!layer.Checked || !_searchLayersPostProcessor.IsSearchLayer(layer)) continue;

                 var featureLayer = (IFeatureLayer) layer;
                 if (featureLayer.Selection.Count == 0) continue;

                 //TODO: Need logic related with dataTheme
                 string dataThemeName;
                 using (var inputBox = new InputBox("Input name of theme"))
                 {
                     if (inputBox.ShowDialog() != DialogResult.OK) return;
                     dataThemeName = inputBox.Result;
                 }

                 var oneSeriesList = new List<OneSeriesDownloadInfo>(featureLayer.Selection.Count);
                 oneSeriesList.AddRange(featureLayer.Selection.ToFeatureList().Select(ClassConvertor.IFeatureToOneSeriesDownloadInfo));

                 var startArgs = new StartDownloadArg(oneSeriesList, new Interfaces.ObjectModel.Theme(dataThemeName));

                 var downloadManager = Global.PluginEntryPoint.DownloadManager;
                 if (downloadManager.IsBusy)
                 {
                     //todo: inform user about busy?
                     return;
                 }
                 downloadManager.Start(startArgs);

                 break; // todo: what we must do if several layers are selected?
             }
        }
    }
}
