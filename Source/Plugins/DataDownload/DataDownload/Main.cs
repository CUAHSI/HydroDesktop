﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Controls.Themes;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.SearchLayersProcessing;
using HydroDesktop.Interfaces;

namespace HydroDesktop.DataDownload
{
    public class Main : Extension, IMapPlugin
    {
        #region Fields

        private const string TableTabKey = "kHome";
        private readonly SearchLayerModifier _searchLayerModifier = new SearchLayerModifier();

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
            MapArgs.AppManager.HeaderControl.Add(btnDownload);


            Global.PluginEntryPoint = this;

            // Subscribe to events
            MapArgs.Map.LayerAdded += Map_LayerAdded;
            MapArgs.Map.Layers.LayerRemoved += Layers_LayerRemoved;
            MapArgs.AppManager.SerializationManager.Deserializing += SerializationManager_Deserializing;
            DownloadManager.Completed += DownloadManager_Completed;
            //----
        }

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            MapArgs.AppManager.HeaderControl.RemoveItems();

            MapArgs.Map.LayerAdded -= Map_LayerAdded;
            MapArgs.Map.Layers.LayerRemoved -= Layers_LayerRemoved;
            MapArgs.AppManager.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            DownloadManager.Completed -= DownloadManager_Completed;

            foreach (var layer in MapArgs.Map.MapFrame.Layers)
                UnattachLayerFromPlugin(layer);

            Global.PluginEntryPoint = null;

            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }

        #endregion

        #region Private mergods

        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            AttachToAllLayers();
        }

        protected override void OnActivate()
        {
            AttachToAllLayers();
            base.OnActivate();
        }

        private void AttachToAllLayers()
        {
            foreach (var layer in MapArgs.Map.MapFrame.Layers)
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
            _searchLayerModifier.AddCustomFeaturesToSearchLayer(layer, (Map)MapArgs.Map);

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
            _searchLayerModifier.RemoveCustomFeaturesFromLayer(layer);

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
            _searchLayerModifier.UpdateSearchLayerAfterDownloading(sourceLayer, featureSet, DownloadManager);

            // Refresh list of the time series in the table and graph in the main form
            ((IHydroAppManager) MapArgs.AppManager).SeriesView.SeriesSelector.RefreshSelection();
        }

        #endregion
    }
}
