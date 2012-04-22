using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using HydroDesktop.Common;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.Properties;
using HydroDesktop.DataDownload.SearchLayersProcessing;
using HydroDesktop.Interfaces;
using HydroDesktop.DataDownload.LayerInformation;
using HydroDesktop.Common.Tools;
using Hydrodesktop.Common;
using Msg = HydroDesktop.DataDownload.MessageStrings;

namespace HydroDesktop.DataDownload
{
    /// <summary>
    /// Plug-in that allows to download Features data.
    /// </summary>
    public class DataDownloadPlugin : Extension
    {
        #region Fields

        private SimpleActionItem _btnDownload1;
        private SimpleActionItem _btnDownload2;
        private SimpleActionItem _btnUpdate;
        private ToolStripItem _seriesControlUpdateValuesMenuItem;
        private SimpleActionItem _btnZoomNext;
        private SimpleActionItem _btnZoomPrevious;
        private SearchLayerInformer _searchLayerInformer;

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

        private bool _showPopups;
        /// <summary>
        /// Gets or sets a boolean indicating whether pop-ups should be shown in the map
        /// </summary>
        public bool ShowPopups
        {
            get { return _showPopups; }
            set
            {
                if (_showPopups == value) return;
                _showPopups = value;

                var handler = ShowPopupsChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when <see cref="ShowPopups"/> changed
        /// </summary>
        public event EventHandler ShowPopupsChanged;

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
            if (ShowIfBusy()) return;

            startArgs.FeatureLayer = layer;
            DownloadManager.Start(startArgs);
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

            var metadataRootKey = SharedConstants.MetadataRootKey;
            var searchRootKey = SharedConstants.SearchRootkey;
            var header = App.HeaderControl;
            SimpleActionItem showPopups;

            //add the "metadata" tab
            header.Add(new RootItem(metadataRootKey, Msg.Metadata) { SortOrder = -5 });
            
            // if the search root item is not present, add it
            try{header.Add(new RootItem(searchRootKey, Msg.Search) { SortOrder = -10 });} catch(ArgumentException) { } //in this case root item has been already added
            
            header.Add(_btnDownload1 = new SimpleActionItem(Msg.Download, DoDownload){ RootKey = searchRootKey, GroupCaption = Msg.Search, LargeImage = Resources.download_32, SmallImage = Resources.download_16, ToolTipText = Msg.DownloadTooTip, Enabled = false});
            header.Add(_btnDownload2 = new SimpleActionItem(Msg.Download, DoDownload){ RootKey = metadataRootKey, GroupCaption = Msg.Download, LargeImage = Resources.download_32, SmallImage = Resources.download_16, ToolTipText = Msg.DownloadTooTip, Enabled = false});
            header.Add(_btnUpdate = new SimpleActionItem(Msg.Update, Update_Click) { RootKey = metadataRootKey, GroupCaption = Msg.Download, LargeImage = Resources.refresh_32x32, SmallImage = Resources.refresh_16x16, Enabled = false });
            header.Add(showPopups = new SimpleActionItem(Msg.ShowPopups, ShowPopups_Click) { RootKey = metadataRootKey, GroupCaption = Msg.Download, LargeImage = Resources.popup_32x32, SmallImage = Resources.popup_16x16, ToggleGroupKey = Msg.Download_Tools_Group });
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Pan, PanTool_Click) { GroupCaption = Msg.View_Group, SmallImage = Resources.hand_16x16, LargeImage = Resources.hand_32x32, ToggleGroupKey = Msg.Map_Tools_Group});
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Zoom_In, ZoomIn_Click) { GroupCaption = Msg.Zoom_Group, ToolTipText = Msg.Zoom_In_Tooltip, SmallImage = Resources.zoom_in_16x16, LargeImage = Resources.zoom_in_32x32, ToggleGroupKey = Msg.Map_Tools_Group });
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Zoom_Out, ZoomOut_Click) { GroupCaption = Msg.Zoom_Group, ToolTipText = Msg.Zoom_Out_Tooltip, SmallImage = Resources.zoom_out_16x16, LargeImage = Resources.zoom_out_32x32, ToggleGroupKey = Msg.Map_Tools_Group });
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Zoom_To_Extents, ZoomToMaxExtents_Click) { GroupCaption = Msg.Zoom_Group, ToolTipText = Msg.Zoom_To_Extents_Tooltip, SmallImage = Resources.zoom_extend_16x16, LargeImage = Resources.zoom_extend_32x32 });
            header.Add(_btnZoomPrevious = new SimpleActionItem(metadataRootKey, Msg.Zoom_Previous, ZoomPrevious_Click) { GroupCaption = Msg.Zoom_Group, ToolTipText = Msg.Zoom_Previous_Tooltip, SmallImage = Resources.zoom_to_previous_16, LargeImage = Resources.zoom_to_previous, Enabled = false });
            header.Add(_btnZoomNext = new SimpleActionItem(metadataRootKey, Msg.Zoom_Next, ZoomNext_Click) { GroupCaption = Msg.Zoom_Group, ToolTipText = Msg.Zoom_Next_Tooltip, SmallImage = Resources.zoom_to_next_16, LargeImage = Resources.zoom_to_next, Enabled = false });
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Zoom_To_Layer, ZoomToLayer_Click) { GroupCaption = Msg.Zoom_Group, SmallImage = Resources.zoom_layer_16x16, LargeImage = Resources.zoom_layer_32x32 });

            header.Add(new SimpleActionItem(metadataRootKey, Msg.Select, SelectionTool_Click) { GroupCaption = Msg.Map_Tools_Group, SmallImage = Resources.select_16x16, LargeImage = Resources.select_32x32, ToggleGroupKey = Msg.Map_Tools_Group });
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Deselect, DeselectAll_Click) { GroupCaption = Msg.Map_Tools_Group, SmallImage = Resources.deselect_16x16, LargeImage = Resources.deselect_32x32 });
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Identify, IdentifierTool_Click) { GroupCaption = Msg.Map_Tools_Group, SmallImage = Resources.info_rhombus_16x16, LargeImage = Resources.info_rhombus_32x32, ToggleGroupKey = Msg.Map_Tools_Group });
            header.Add(new SimpleActionItem(metadataRootKey, Msg.Zoom_To_Selection, ZoomToSelection_Click) { GroupCaption = Msg.Map_Tools_Group, SmallImage = Resources.zoom_selection_16x16, LargeImage = Resources.zoom_selection_32x32 });

            showPopups.Toggling += ShowPopups_Click;
            showPopups.Toggle();

            // Subscribe to events
            App.Map.LayerAdded += Map_LayerAdded;
            App.Map.Layers.LayerRemoved += Layers_LayerRemoved;
            App.Map.MapFrame.ViewExtentsChanged += MapFrame_ViewExtentsChanged;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            DownloadManager.Completed += DownloadManager_Completed;
            App.HeaderControl.RootItemSelected += RootItemSelected;
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
            App.Map.MapFrame.ViewExtentsChanged -= MapFrame_ViewExtentsChanged;
            App.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            DownloadManager.Completed -= DownloadManager_Completed;
            App.HeaderControl.RootItemSelected -= RootItemSelected;

            foreach (var layer in App.Map.MapFrame.Layers)
                UnattachLayerFromPlugin(layer);
            if (_searchLayerInformer != null)
            {
                _searchLayerInformer.Deactivate();
                _searchLayerInformer = null;
            }

            // Remove added menu items from SeriesControl ContextMenu
            SeriesControl.ContextMenuStrip.Items.Remove(_seriesControlUpdateValuesMenuItem);

            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }

        #endregion

        #region Private methods

        private void MapFrame_ViewExtentsChanged(object sender, ExtentArgs e)
        {
            var mapFrame = sender as MapFrame;
            if (mapFrame == null) return;
            _btnZoomNext.Enabled = mapFrame.CanZoomToNext();
            _btnZoomPrevious.Enabled = mapFrame.CanZoomToPrevious();
        }

        /// <summary>
        /// Identifier Tool
        /// </summary>
        private void IdentifierTool_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.Info;
        }

        /// <summary>
        /// Select or deselect Features
        /// </summary>
        private void SelectionTool_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.Select;
        }

        /// <summary>
        /// Deselect all features in all layers
        /// </summary>
        private void DeselectAll_Click(object sender, EventArgs e)
        {
            foreach (IMapLayer layer in App.Map.MapFrame.GetAllLayers())
            {
                var mapFeatureLayer = layer as IMapFeatureLayer;
                {
                    if (mapFeatureLayer != null)
                        mapFeatureLayer.UnSelectAll();
                }
            }
        }

        private void ShowPopups_Click(object sender, EventArgs e)
        {
            ShowPopups = !ShowPopups;
        }

        private readonly IList<IFeatureLayer> _layersToUpdate = new List<IFeatureLayer>();
        private void Update_Click(object sender, EventArgs e)
        {
            if (ShowIfBusy()) return;

            var dataSitesGroup = App.Map.GetDataSitesLayer();
            if (dataSitesGroup == null) return;

            _layersToUpdate.Clear();
            foreach (var layer in dataSitesGroup.Layers)
            {
                var featureLayer = layer as IFeatureLayer;
                if (!layer.Checked || !SearchLayerModifier.IsSearchLayer(layer)) continue;
                _layersToUpdate.Add(featureLayer);
            }
            
            DownloadManager.Completed += OnDownloadManagerOnCompleted;
            ProcessUpdate();
        }

        private void OnDownloadManagerOnCompleted(object o, RunWorkerCompletedEventArgs args)
        {
            if (args.Cancelled)
            {
                _layersToUpdate.Clear();
            }
            ProcessUpdate();
        }

        private void ProcessUpdate()
        {
            if (_layersToUpdate.Count == 0)
            {
                DownloadManager.Completed -= OnDownloadManagerOnCompleted;
                return;
            }

            var layerPos = _layersToUpdate.Count - 1;
            var layer = _layersToUpdate[layerPos];
            _layersToUpdate.RemoveAt(layerPos);
            StartDownloading(layer);
        }

        private void ZoomToSelection_Click(object sender, EventArgs e)
        {
            var dataSitesGroup = App.Map.GetDataSitesLayer();
            if (dataSitesGroup == null) return;

            const double distanceX = 2;
            const double distanceY = 2;
            const double EPS = 1e-7;

            IEnvelope envelope = null;
            foreach (var layer in dataSitesGroup.Layers)
            {
                var featureLayer = layer as IFeatureLayer;
                if (featureLayer == null || !featureLayer.Checked || featureLayer.Selection.Count == 0) continue;

                var env = featureLayer.Selection.Envelope;
                envelope = envelope == null ? env : envelope.Union(env);
            }
            if (envelope == null) return;

            if (Math.Abs(envelope.Width - 0) < EPS || Math.Abs(envelope.Height - 0) < EPS)
            {
                envelope.ExpandBy(distanceX, distanceY);
            }

            if (envelope.Width > EPS && envelope.Height > EPS)
            {
                envelope.ExpandBy(envelope.Width / 10, envelope.Height / 10); // work item #84
            }
            else
            {
                const double zoomInFactor = 0.05; //fixed zoom-in by 10% - 5% on each side
                var newExtentWidth = App.Map.ViewExtents.Width*zoomInFactor;
                var newExtentHeight = App.Map.ViewExtents.Height*zoomInFactor;
                envelope.ExpandBy(newExtentWidth, newExtentHeight);
            }

            App.Map.ViewExtents = envelope.ToExtent();
        }

        /// <summary>
        /// Move (Pan) the map
        /// </summary>
        private void PanTool_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.Pan;
        }

        /// <summary>
        /// Zoom In
        /// </summary>
        private void ZoomIn_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.ZoomIn;
        }

        /// <summary>
        /// Zoom to previous extent
        /// </summary>
        private void ZoomNext_Click(object sender, EventArgs e)
        {
            App.Map.MapFrame.ZoomToNext();
        }

        /// <summary>
        /// Zoom Out
        /// </summary>
        private void ZoomOut_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.ZoomOut;
        }

        /// <summary>
        /// Zoom to previous extent
        /// </summary>
        private void ZoomPrevious_Click(object sender, EventArgs e)
        {
            App.Map.MapFrame.ZoomToPrevious();
        }

        /// <summary>
        /// Zoom to maximum extents
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ZoomToMaxExtents_Click(object sender, EventArgs e)
        {
            App.Map.ZoomToMaxExtent();
        }


        /// <summary>
        /// Zoom to the currently selected layer
        /// </summary>
        private void ZoomToLayer_Click(object sender, EventArgs e)
        {
            var layer = App.Map.Layers.SelectedLayer;
            if (layer != null)
            {
                ZoomToLayer(layer);
            }
        }

        private void ZoomToLayer(IMapLayer layerToZoom)
        {
            const double eps = 1e-7;
            var layerEnvelope = layerToZoom.Extent.ToEnvelope();
            if (layerEnvelope.Width > eps && layerEnvelope.Height > eps)
            {
                layerEnvelope.ExpandBy(layerEnvelope.Width / 10, layerEnvelope.Height / 10); // work item #84
            }
            else
            {
                const double zoomInFactor = 0.05; //fixed zoom-in by 10% - 5% on each side
                double newExtentWidth = App.Map.ViewExtents.Width * zoomInFactor;
                double newExtentHeight = App.Map.ViewExtents.Height * zoomInFactor;
                layerEnvelope.ExpandBy(newExtentWidth, newExtentHeight);
            }

            App.Map.ViewExtents = layerEnvelope.ToExtent();
        }


        private void RootItemSelected(object sender, RootItemEventArgs e)
        {
            var ext = App.GetExtension("DotSpatial.Plugins.AttributeDataExplorer");
            if (ext == null) return;

            if (e.SelectedRootKey == SharedConstants.MetadataRootKey)
            {
                if (!ext.IsActive)
                    ext.Activate();
            }
            else
            {
                if (ext.IsActive)
                    ext.Deactivate();
            }
        }

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
                    var extractor = new HISCentralInfoExtractor(new Lazy<Dictionary<string, string>>(() => new HisCentralServices(App).Services));
                    _searchLayerInformer = new SearchLayerInformer(this, extractor, (Map) App.Map);
                }

                var lm = SearchLayerModifier.Create(layer, (Map) App.Map, this);
                Debug.Assert(lm != null);

                if (!isDeserializing)
                {
                    lm.UpdateLabeling();
                    lm.UpdateSymbolizing();
                }
                lm.UpdateContextMenu();

                _btnDownload1.Enabled = true;
                _btnDownload2.Enabled = true;
                _btnUpdate.Enabled = true;
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

        private bool ShowIfBusy()
        {
            var downloadManager = DownloadManager;
            if (downloadManager.IsBusy)
            {
                downloadManager.ShowUI();
                return true;
            }
            return false;
        }

        private void DoDownload(object sender, EventArgs args)
        {
            if (ShowIfBusy()) return;

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

        private void DownloadManager_Completed(object sender, RunWorkerCompletedEventArgs e)
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

            var sourceLayer = dManager.Information.StartArgs.FeatureLayer;
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

