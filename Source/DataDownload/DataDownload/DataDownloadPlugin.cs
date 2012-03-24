using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using HydroDesktop.DataDownload.LayerInformation;
using HydroDesktop.Common.Tools;

namespace HydroDesktop.DataDownload
{
    /// <summary>
    /// Plugin that allows to download Features data.
    /// </summary>
    public class DataDownloadPlugin : Extension
    {
        #region Fields

        private SimpleActionItem btnDownload;
        private ToolStripItem _seriesControlUpdateValuesMenuItem;

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
        private DownloadManager _downloadManager;
        private DownloadManager DownloadManager
        {
            get { return _downloadManager?? (_downloadManager = new DownloadManager()); }
        }

        private SearchLayerInformer _searchLayerInformer;

        #endregion

        #region Public methods

        /// <summary>
        /// Starts downloading
        /// </summary>
        /// <param name="startArgs">Start arguments</param>
        /// <param name="layer">Layer, which contains points to download</param>
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

        /// <summary>
        /// Starts downloading
        /// </summary>
        /// <param name="featureLayer">Layer with features to download</param>
        /// <param name="featuresToDownload">Features to download. If this null, then all features from layer will be processed.</param>
        public void StartDownloading(IFeatureLayer featureLayer, IEnumerable<IFeature> featuresToDownload = null)
        {
            if (featuresToDownload == null)
            {
                featuresToDownload = featureLayer.DataSet.Features;
            }
            var dataThemeName = featureLayer.LegendText;
            var oneSeriesList = featuresToDownload.Select(f => ClassConvertor.IFeatureToOneSeriesDownloadInfo(f, featureLayer)).ToList();
            var startArgs = new StartDownloadArg(oneSeriesList, dataThemeName);
            StartDownloading(startArgs, featureLayer);
        }

        #endregion

        #region Plugin operations

        /// <ingeritdoc/>
        public override void Activate()
        {
            if (App == null) throw new Exception("App");

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

            // Update SeriesControl ContextMenu
            _seriesControlUpdateValuesMenuItem = SeriesControl.ContextMenuStrip.Items.Add("Update Values from Server", null, DoSeriesControlUpdateValues);

            base.Activate();
        }
       
        /// <summary>
        /// Fires when the plug-in should become inactive
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
            if (_searchLayerInformer != null)
            {
                _searchLayerInformer.Stop();
                _searchLayerInformer = null;
            }

            Global.PluginEntryPoint = null;

            // Remove added menu items from SeriesControl ContextMenu
            SeriesControl.ContextMenuStrip.Items.Remove(_seriesControlUpdateValuesMenuItem);

            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }

        #endregion

        #region Private methods

        private void DoSeriesControlUpdateValues(object sender, EventArgs e)
        {
            var selectedSeriesID = SeriesControl.SelectedSeriesID;
            if (selectedSeriesID == 0) return;

            // Find by selectedSeriesID layer and feature to update.
            // We can do it, because value of SeriesID column is unique for all features in the map.
            foreach (var layer in App.Map.MapFrame.GetAllLayers())
            {
                if (!SearchLayerModifier.IsSearchLayer(layer)) continue;

                var featureLayer = (IFeatureLayer)layer;
                if (featureLayer.DataSet.GetColumn("SeriesID") == null) continue;

                var featureToDownload = featureLayer.DataSet.Features
                    .FirstOrDefault(feature => feature.DataRow["SeriesID"] != DBNull.Value &&
                                               feature.DataRow["SeriesID"].ToString() == selectedSeriesID.ToString(CultureInfo.InvariantCulture));
                if (featureToDownload != null)
                {
                    StartDownloading(featureLayer, new[] { featureToDownload });
                }
            }
        }

        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            foreach (var layer in App.Map.MapFrame.Layers)
                AttachLayerToPlugin(layer, true);
        }

        private void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            if (e.Layer == null) return; //occurs when moving layer
            
            AttachLayerToPlugin(e.Layer);
        }
        void Layers_LayerRemoved(object sender, LayerEventArgs e)
        {
            UnattachLayerFromPlugin(e.Layer);
        }

        private void AttachLayerToPlugin(ILayer layer, bool isDeserializing = false)
        {
            if (SearchLayerModifier.IsSearchLayer(layer))
            {
                if (_searchLayerInformer == null)
                {
                    // Create popup-informer
                    var extractor = new HISCentralInfoExtractor(new Lazy<Dictionary<string, string>>(() => HisCentralServices.Services));
                    _searchLayerInformer = new SearchLayerInformer(extractor, (Map) App.Map);
                }

                var lm = SearchLayerModifier.Create(layer, (Map) App.Map, this);
                Debug.Assert(lm != null);

                if (!isDeserializing)
                {
                    lm.UpdateLabeling();
                    lm.UpdateSymbolizing();
                }
                lm.UpdateContextMenu();

                btnDownload.Enabled = true;
            }

            var group = layer as IGroup;
            if (group != null)
            {
                group.LayerAdded += Map_LayerAdded;
                group.LayerRemoved += Layers_LayerRemoved;

                foreach (var child in group.GetLayers())
                    AttachLayerToPlugin(child, isDeserializing);
            }
        }

        private void UnattachLayerFromPlugin(ILayer layer)
        {
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

                StartDownloading(featureLayer, featureLayer.Selection.ToFeatureList());
                break;
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

            var _themeManager = new ThemeManager();
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
            var lm = SearchLayerModifier.Create(sourceLayer, (Map) App.Map, this);
            Debug.Assert(lm != null);

            lm.UpdateDataTable(featureSet, DownloadManager);
            lm.UpdateSymbolizing();
            lm.UpdateContextMenu();

            // Check for DataAggregation 
            var aggPlugin = App.GetExtension<IDataAggregationPlugin>();
            if (aggPlugin != null)
            {
                aggPlugin.AttachLayerToPlugin(sourceLayer);
            }

            // Refresh list of the time series in the table and graph in the main form
            SeriesControl.RefreshSelection();
        }

        #endregion
    }
}

