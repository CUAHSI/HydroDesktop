using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Hydrodesktop.Common;
using HydroDesktop.Common;
using HydroDesktop.Common.Controls;
using HydroDesktop.Common.Tools;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.LayerInformation;
using HydroDesktop.DataDownload.Properties;
using HydroDesktop.DataDownload.SearchLayersProcessing;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.PluginContracts;
using Msg = HydroDesktop.DataDownload.MessageStrings;
using HydroDesktop.DataDownload.Options;

namespace HydroDesktop.DataDownload
{
    /// <summary>
    /// Plug-in that allows to download Features data.
    /// </summary>
    public class DataDownloadPlugin : Extension
    {
        #region Fields

        private SimpleActionItem _btnDownloadInSearch;
        private SimpleActionItem _btnShowPopups;
        private SimpleActionItem _btnSearchOptions;
        private SimpleActionItem _btnSearchResults;
        private ToolStripItem _seriesControlUpdateValuesMenuItem;
        private SearchLayerInformer _searchLayerInformer;
        private DownloadOptions _downloadOptions = new DownloadOptions {NumberOfValuesPerRequest = 10000};

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
                if (_showPopups == value || DotSpatial.Mono.Mono.IsRunningOnMono()) return;
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

        /// <summary>
        /// Raised when "show search results" changed
        /// </summary>
        public event EventHandler ShowSearchResultsChanged;

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
            startArgs.DownloadOptions = _downloadOptions;
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

            // Subscribe to events
            App.Map.LayerAdded += Map_LayerAdded;
            App.Map.Layers.LayerRemoved += Layers_LayerRemoved;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            DownloadManager.Completed += DownloadManager_Completed;

            App.ExtensionsActivated += AppOnExtensionsActivated;
            App.DockManager.PanelHidden += DockManager_PanelHidden;
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;

            // Update SeriesControl ContextMenu
            _seriesControlUpdateValuesMenuItem = SeriesControl.ContextMenuStrip.Items.Add("Update Values from Server", null, DoSeriesControlUpdateValues);

            base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey.Equals("kDataExplorer"))
            {

                if (!_showSearchResultsPanel)
                {
                    _btnSearchResults.Toggle();
                }
                _showSearchResultsPanel = true;
            }
        }

        void DockManager_PanelHidden(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == "kDataExplorer")
            {

                if (_showSearchResultsPanel)
               {
                    _btnSearchResults.Toggle();
                 
               }
                _showSearchResultsPanel = false;
            }
        }

        /// <summary>
        /// Fires when the plug-in should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            // Unsubscribe from events
            App.Map.LayerAdded -= Map_LayerAdded;
            App.Map.Layers.LayerRemoved -= Layers_LayerRemoved;
            App.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            DownloadManager.Completed -= DownloadManager_Completed;
            App.ExtensionsActivated -= AppOnExtensionsActivated;
            App.DockManager.PanelHidden -= DockManager_PanelHidden;

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

        private void AppOnExtensionsActivated(object sender, EventArgs eventArgs)
        {
            // Add download button into search tab
            if (App.GetExtension("Search3") != null)
            {
                App.HeaderControl.Add(_btnSearchResults = new SimpleActionItem("Show Attribute Table", ShowSearchResults_Click) { RootKey = SharedConstants.SearchRootkey, GroupCaption = Msg.Results, SmallImage = Resources.table_16x16, Enabled = false, ToggleGroupKey = MessageStrings.Search_Results_Tools_Group });
                _btnSearchResults.PropertyChanged += btnSearchResults_enabled;

                // App.HeaderControl.Add(_btnOptions = new SimpleActionItem("Options", SearchOptions_Click) { RootKey = SharedConstants.SearchRootkey, GroupCaption = Msg.Results, SmallImage = Resources.popup_16x16, ToggleGroupKey = Msg.Download_Tools_Group, Enabled = true });

                if (!DotSpatial.Mono.Mono.IsRunningOnMono())
                {
                    App.HeaderControl.Add(_btnShowPopups = new SimpleActionItem("Show Map Popups", ShowPopups_Click) { RootKey = SharedConstants.SearchRootkey, GroupCaption = Msg.Results, SmallImage = Resources.popup_16x16, ToggleGroupKey = Msg.Download_Tools_Group, Enabled = false });
                    _btnShowPopups.Toggling += ShowPopups_Click;
                    _btnShowPopups.Enabled = false;
                }
                _showPopups = false;

                App.HeaderControl.Add(_btnSearchOptions = new SimpleActionItem("Download Settings", Options_Click) { RootKey = SharedConstants.SearchRootkey, GroupCaption = Msg.Results, SmallImage = Resources.settings_16, ToolTipText = Msg.DownloadSettings, Enabled = false });

                App.HeaderControl.Add(_btnDownloadInSearch = new SimpleActionItem(Msg.Download_Selected, DoDownload) { RootKey = SharedConstants.SearchRootkey, GroupCaption = Msg.Results, LargeImage = Resources.download_32, SmallImage = Resources.download_16, ToolTipText = Msg.DownloadTooTip, Enabled = false });

                //App.HeaderControl.Add(_btnUpdate = new SimpleActionItem(Msg.Update, Update_Click) { RootKey = SharedConstants.SearchRootkey, GroupCaption = Msg.Download, LargeImage = Resources.refresh_32x32, SmallImage = Resources.refresh_16x16, Enabled = false });
            }

            if (!_showSearchResultsPanel)
            {
                App.DockManager.HidePanel("kDataExplorer");
            }
            
            _btnSearchResults.Enabled = false;         

        }

        private void btnSearchResults_enabled(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Enabled") && _showSearchResultsPanel)
            {
                _btnSearchResults.Toggle();
            }
        }

        private void Options_Click(object sender, EventArgs e)
        {
            var copyOptions = new DownloadOptions(_downloadOptions);
            using (var optionsDialog = new DownloadOptionsDialog(copyOptions))
            {
                if (optionsDialog.ShowDialog() == DialogResult.OK)
                {
                    _downloadOptions = copyOptions;
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

        //private void ZoomToSelection_Click(object sender, EventArgs e)
        //{
        //    const double distanceX = 2;
        //    const double distanceY = 2;
        //    const double EPS = 1e-7;

        //    IEnvelope envelope = null;
        //    foreach (var layer in ((Map)App.Map).GetAllLayers())
        //    {
        //        var featureLayer = layer as IFeatureLayer;
        //        if (featureLayer == null || !featureLayer.Checked || featureLayer.Selection.Count == 0) continue;

        //        var env = featureLayer.Selection.Envelope;
        //        envelope = envelope == null ? env : envelope.Union(env);
        //    }
        //    if (envelope == null) return;

        //    if (Math.Abs(envelope.Width - 0) < EPS || Math.Abs(envelope.Height - 0) < EPS)
        //    {
        //        envelope.ExpandBy(distanceX, distanceY);
        //    }

        //    if (envelope.Width > EPS && envelope.Height > EPS)
        //    {
        //        envelope.ExpandBy(envelope.Width / 10, envelope.Height / 10); // work item #84
        //    }
        //    else
        //    {
        //        const double zoomInFactor = 0.05; //fixed zoom-in by 10% - 5% on each side
        //        var newExtentWidth = App.Map.ViewExtents.Width*zoomInFactor;
        //        var newExtentHeight = App.Map.ViewExtents.Height*zoomInFactor;
        //        envelope.ExpandBy(newExtentWidth, newExtentHeight);
        //    }

        //    App.Map.ViewExtents = envelope.ToExtent();
        //}




        private bool _showSearchResultsPanel;
        /// <summary>
        /// Gets or sets a boolean indicating whether the search results panel should be shown
        /// </summary>
        public bool ShowSearchResultsPanel
        {
            get { return _showSearchResultsPanel; }
            set
            {
                if (_showSearchResultsPanel == value) return;
                _showSearchResultsPanel = value;

                var handler = ShowSearchResultsChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        private bool ShowAttributes(IFeatureLayer layer)
        {
            bool isActive = false;

            if (layer != null)
            {
                layer.IsSelected = true;
                App.DockManager.SelectPanel("kDataExplorer");
                isActive = true;
            }

            return isActive;
        }

        private void ShowSearchResults_Click(object sender, EventArgs e)
        {
            _showSearchResultsPanel = !_showSearchResultsPanel;

            if (ShowSearchResultsPanel)
            {
                //todo: Copy and paste from AttributeTableManager.AttributeTable_Click
                var featureLayers = App.Map.MapFrame.GetAllLayers()
                 .OfType<IFeatureLayer>()
                 .Reverse().ToList();

                var isActive = false;
                foreach (var fl in featureLayers.Where(l => l.IsSelected))
                {
                    isActive = ShowAttributes(fl);
                    if (isActive)
                        break;
                }

                // No selected layers in feature layers
                if (!isActive)
                {
                    IFeatureLayer toSelect = null;
                    if (featureLayers.Count == 1)
                    {
                        toSelect = featureLayers[0];
                    }
                    else
                    {
                        var sf = new SelectFeatureLayer(featureLayers);
                        if (sf.ShowDialog(App.Map.MapFrame.Parent) == DialogResult.OK)
                        {
                            toSelect = sf.SelectedLayer;
                        }
                    }
                    if (toSelect != null)
                    {
                        App.Legend.ForEachRecursively<ILayer>(d => d.IsSelected = false);
                        toSelect.IsSelected = true;
                        App.Legend.RefreshNodes();
                        isActive = ShowAttributes(toSelect);
                    }
                }

                if (isActive == false)
                {
                    _showSearchResultsPanel = !_showSearchResultsPanel;
                    _btnSearchResults.Toggle();
                }
            }
            else
            {
                App.DockManager.HidePanel("kDataExplorer");
            }
              
        }

        private void DoShowSearchResults(object sender, EventArgs e)
        {
            
            
            //var ext = App.GetExtension("DotSpatial.Plugins.AttributeDataExplorer");
            //if (ext == null) return;

            //if (e.SelectedRootKey == SharedConstants.MetadataRootKey)
            //{
                //if (!ext.IsActive)
                 //   ext.Activate();

                //App.SerializationManager.SetCustomSetting("MetadataRootClicked", true);
                //App.DockManager.SelectPanel("kMap");
            //}
            //else
            //{
                //if (ext.IsActive)
                   // ext.Deactivate();
            //}
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
            //Hack to make it so the Selection Status display would update when a layer was removed.
            App.Map.Layers.SelectedLayer = null;
            App.DockManager.HidePanel("kDataExplorer");

            FunctionMode f = App.Map.FunctionMode;
            App.Map.FunctionMode = f;
           
       
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

                if (_btnDownloadInSearch != null)
                {
                    _btnDownloadInSearch.Enabled = true;
                }
                if (_btnSearchOptions != null)
                {
                    _btnSearchOptions.Enabled = true;
                }
                //if (_btnSearchOptions != null)
                //{
                //    _btnSearchOptions.Enabled = true;
                //}
                if (_btnSearchResults != null)
                {
                    _btnSearchResults.Enabled = true;
                }
                if (_btnShowPopups != null)
                {
                    if (!_btnShowPopups.Enabled)
                    {
                        _btnShowPopups.Enabled = true;
                    }
                    if (!ShowPopups)
                    {
                        _btnShowPopups.Toggle();
                    }
                }

                
                //_btnDownload.Enabled = true;
                //_btnUpdate.Enabled = true;
                //_btnShowPopups.Enabled = true;
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
                MessageBox.Show("No results are selected for download. Please select sites in the map, or select series from the selected layer table.",
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

            //check for update button
            //if (_btnUpdate != null)
            //{
            //    if (SearchLayerModifier.LayerHaveDownlodedData(sourceLayer))
            //    {
            //        _btnUpdate.Enabled = true;
            //    }
            //}

            // Refresh list of the time series in the table and graph in the main form
            SeriesControl.RefreshSelection();
        }

        #endregion
    }
}

