using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data.Forms;
using DotSpatial.Symbology;
using HydroDesktop.Configuration;
using HydroDesktop.Controls.Themes;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.SearchLayersProcessing;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;

namespace HydroDesktop.DataDownload
{
    public class Main : Extension, IMapPlugin
    {
        #region Fields

        private const string TableTabKey = "kHome";
        private readonly SearchLayerModifier _searchLayerModifier = new SearchLayerModifier();

        private static readonly DbOperations dbOperations =
            new DbOperations(Settings.Instance.DataRepositoryConnectionString,
                             DatabaseTypes.SQLite);

        private readonly ThemeManager _themeManager = new ThemeManager(dbOperations);

        #endregion

        #region Properties

        /// <summary>
        /// Args with wich this plug-in was activated
        /// </summary>
        private IMapPluginArgs MapArgs { get; set; }

        /// <summary>
        /// Download manager
        /// </summary>
        private DownloadManager DownloadManager
        {
            get { return DownloadManager.Instance; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts downloading
        /// </summary>
        /// <param name="startArgs">Args to start</param>
        /// <param name="layer">Layer, wich contains points to download</param>
        public void StartDownloading(StartDownloadArg startArgs, IFeatureLayer layer)
        {
            if (startArgs == null) throw new ArgumentNullException("startArgs");

            var downloadManager = DownloadManager;
            if (downloadManager.IsBusy)
            {
                //todo: inform user about "busy" state?
                return;
            }
            startArgs.Tag = layer; // put layer into tag, we need it in DownloadManager_Completed method
            downloadManager.Start(startArgs);
        }

        #endregion

        #region Plugin operations

        public void Initialize(IMapPluginArgs args)
        {
            if (args == null) throw new ArgumentNullException("args");
            MapArgs = args;

            // Initialize menu
            var btnDownload = new SimpleActionItem("Download", DoDownload)
                                  {
                                      RootKey = TableTabKey,
                                      GroupCaption = "Search",
                                      LargeImage = Properties.Resources.download32
                                  };
            args.AppManager.HeaderControl.Add(btnDownload);

            // Subscribe to events
            MapArgs.Map.LayerAdded += Map_LayerAdded;
            MapArgs.Map.Layers.LayerRemoved += Layers_LayerRemoved;
            args.AppManager.SerializationManager.Deserializing += SerializationManager_Deserializing;
            //----

            Global.PluginEntryPoint = this;
            DownloadManager.Completed += DownloadManager_Completed;
        }

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            MapArgs.Map.LayerAdded -= Map_LayerAdded;
            MapArgs.Map.Layers.LayerRemoved -= Layers_LayerRemoved;
            MapArgs.AppManager.HeaderControl.RemoveItems();
            Global.PluginEntryPoint = null;
            DownloadManager.Completed -= DownloadManager_Completed;

            foreach (var layer in MapArgs.Map.MapFrame.GetAllLayers())
                _searchLayerModifier.RemoveCustomFeaturesFromLayer(layer);

            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }

        #endregion

        #region Private mergods

        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
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
                _searchLayerModifier.AddCustomFeaturesToSearchLayer(layer, (Map) MapArgs.Map);
        }

        private void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            _searchLayerModifier.AddCustomFeaturesToSearchLayer(e.Layer, (Map) MapArgs.Map);

            if (e.Layer is IGroup)
            {
                var group = (IGroup) e.Layer;
                group.LayerAdded += Map_LayerAdded;
                group.LayerRemoved += group_LayerRemoved;
            }
        }

        void group_LayerRemoved(object sender, LayerEventArgs e)
        {
            _searchLayerModifier.RemoveCustomFeaturesFromLayer(e.Layer);
            if (e.Layer is IGroup)
            {
                var group = (IGroup)e.Layer;
                group.LayerAdded -= Map_LayerAdded;
                group.LayerRemoved -= group_LayerRemoved;
            }
        }

        void Layers_LayerRemoved(object sender, LayerEventArgs e)
        {
            _searchLayerModifier.RemoveCustomFeaturesFromLayer(e.Layer);
            if (e.Layer is IGroup)
            {
                var group = (IGroup)e.Layer;
                group.LayerAdded -= Map_LayerAdded;
                group.LayerRemoved -= group_LayerRemoved;
            }
        }

        private void DoDownload(object sender, EventArgs args)
        {
            var hasPointsToDownload = false;
            foreach (var layer in MapArgs.Map.MapFrame.GetAllLayers())
            {
                if (!layer.Checked || !_searchLayerModifier.IsSearchLayer(layer)) continue;

                var featureLayer = (IFeatureLayer) layer;
                if (featureLayer.Selection.Count == 0) continue;
                hasPointsToDownload = true;

                var dataThemeName = featureLayer.LegendText;
                var oneSeriesList = new List<OneSeriesDownloadInfo>(featureLayer.Selection.Count);
                oneSeriesList.AddRange(
                    featureLayer.Selection.ToFeatureList().Select(
                        f => ClassConvertor.IFeatureToOneSeriesDownloadInfo(f, featureLayer)));
                var startArgs = new StartDownloadArg(oneSeriesList, dataThemeName);
                StartDownloading(startArgs, featureLayer);
                break; // todo: what we must do if several layers are selected?
            }

            if (!hasPointsToDownload)
            {
                MessageBox.Show("No sites are selected. Please select sites for downloading data in the map.",
                                "No selected sites", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DownloadManager_Completed(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;

            var dManager = DownloadManager;
            var themeName = dManager.Information.StartArgs.DataTheme.Name;
            var featureSet = _themeManager.GetFeatureSet(themeName);
            featureSet.FillAttributes();

            var sourceLayer = (IFeatureLayer) dManager.Information.StartArgs.Tag;
            _searchLayerModifier.UpdateSearchLayerAfterDownloading(sourceLayer, featureSet, DownloadManager);

            //Refresh list of the time series in the table and graph in the main form
            ((IHydroAppManager) MapArgs.AppManager).SeriesView.SeriesSelector.RefreshSelection();
        }

    #endregion
    }
}

