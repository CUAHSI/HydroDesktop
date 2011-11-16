using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.SearchLayersProcessing;
using HydroDesktop.Interfaces;


namespace HydroDesktop.DataDownload
{
    public class DataDownloadPlugin : Extension
    {
        #region Fields

        private const string TableTabKey = "kHome";
        private SimpleActionItem btnDownload;

        #endregion

        #region Properties

        /// <summary>
        /// Series View
        /// </summary>
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; private set; }

        /// <summary>
        /// Download manager
        /// </summary>
        private DownloadManager DownloadManager
        {
            get { return DownloadManager.Instance; }
        }

        private SearchLayerModifier _searchLayerModifier;
        private SearchLayerModifier SearchLayerModifier
        {
            get { return _searchLayerModifier ?? (_searchLayerModifier = new SearchLayerModifier((Map) App.Map)); }
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

        public override void Activate()
        {
            if (App == null) throw new ArgumentNullException("App");

            // Initialize menu
            btnDownload = new SimpleActionItem("Download", DoDownload)
                                  {
                                      RootKey = "kHydroSearchV3",
                                      GroupCaption = "Search",
                                      LargeImage = Properties.Resources.download_32,
                                      SmallImage = Properties.Resources.download_16,
                                      ToolTipText = "Click to download all selected series",
                                      Enabled = false
                                  };
            App.HeaderControl.Add(btnDownload);


            Global.PluginEntryPoint = this;

            // Subscribe to events
            App.Map.LayerAdded += Map_LayerAdded;
            App.Map.Layers.LayerRemoved += Layers_LayerRemoved;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            DownloadManager.Completed += DownloadManager_Completed;
            //----

            base.Activate();
        }

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            App.Map.LayerAdded -= Map_LayerAdded;
            App.Map.Layers.LayerRemoved -= Layers_LayerRemoved;
            App.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            DownloadManager.Completed -= DownloadManager_Completed;

            foreach (var layer in App.Map.MapFrame.Layers)
                UnattachLayerFromPlugin(layer);
            SearchLayerModifier.DisablePopupInformer();

            Global.PluginEntryPoint = null;

            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }

        #endregion

        #region Private methods

        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            AttachToAllLayers();
        }

        private void AttachToAllLayers()
        {
            foreach (var layer in App.Map.MapFrame.Layers)
                AttachLayerToPlugin(layer);
        }

        private void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            AttachLayerToPlugin(e.Layer);
        }
        void Layers_LayerRemoved(object sender, LayerEventArgs e)
        {
            UnattachLayerFromPlugin(e.Layer);
        }

        private void AttachLayerToPlugin(ILayer layer)
        {
            if (SearchLayerModifier.AddCustomFeaturesToSearchLayer(layer))
            {
                btnDownload.Enabled = true;
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

        private void UnattachLayerFromPlugin(ILayer layer)
        {
            SearchLayerModifier.RemoveCustomFeaturesFromLayer(layer);

            var group = layer as IGroup;
            if (group != null)
            {
                group.LayerAdded -= Map_LayerAdded;
                group.LayerRemoved -= Layers_LayerRemoved;

                foreach (var child in group.GetLayers())
                    UnattachLayerFromPlugin(child);
            }
        }

        private void DoDownload(object sender, EventArgs args)
        {
            var hasPointsToDownload = false;
            foreach (var layer in App.Map.MapFrame.GetAllLayers())
            {
                if (!layer.Checked || !SearchLayerModifier.IsSearchLayer(layer)) continue;

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

            var _themeManager = new ThemeManager(DatabaseManager.Instance.GetDbOperationsForCurrentProject());
            IFeatureSet featureSet;
            try
            {
                featureSet = _themeManager.GetFeatureSet(themeName);
            }
            catch (ArgumentException)
            {
                // No such theme in the database
                featureSet = null;
            }
            if (featureSet == null)
            {
                // in theory this condition always will be false  
                if (dManager.Information.WithError != dManager.Information.TotalSeries)
                {
                    MessageBox.Show("Theme not found, but some series was saved!", "Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                return;
            }

            featureSet.FillAttributes();

            var sourceLayer = (IFeatureLayer) dManager.Information.StartArgs.Tag;
            SearchLayerModifier.UpdateSearchLayerAfterDownloading(sourceLayer, featureSet, DownloadManager);

            // Refresh list of the time series in the table and graph in the main form
            SeriesControl.RefreshSelection();
        }

        #endregion
    }
}

