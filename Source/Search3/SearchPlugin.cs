using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Projections;
using HydroDesktop.Common;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Interfaces.PluginContracts;
using HydroDesktop.WebServices;
using Search3.Area;
using Search3.Properties;
using Search3.Searching;
using Search3.Searching.Exceptions;
using Search3.Settings;
using Search3.Settings.UI;
using Msg = Search3.MessageStrings;
using DotSpatial.Topology;
using DotSpatial.Symbology;

namespace Search3
{
    public class SearchPlugin : Extension, ISearchPlugin
    {
        #region Fields

        private SimpleActionItem rbServices;
        private TextEntryActionItem rbStartDate;
        private TextEntryActionItem rbEndDate;
        private DropDownActionItem _rbKeyword;
        private SimpleActionItem _rbAddMoreKeywords;
        private SimpleActionItem rbDrawBox;
        private SimpleActionItem rbSelect;
        private RectangleDrawing _rectangleDrawing;
        private SimpleActionItem _currentView;
        private bool _useCurrentView;
        private Searcher _searcher;
        //private SearchStatusDisplay searchSummary;

        private readonly string _datesFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        private readonly string _searchKey = SharedConstants.SearchRootkey;
        private const string KEYWORDS_SEPARATOR = ";";

        #endregion

        #region Plugin operations

        public override void Activate()
        {
            AddSearchRibbon();
            //searchSummary = new SearchStatusDisplay(App);
            base.Activate();

            App.SerializationManager.Serializing += SerializationManager_Serializing;
            App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;
            App.Map.FunctionModeChanged += Map_FunctionModeChanged;
            App.Map.SelectionChanged += Map_SelectionChanged;
        }

        public override void Deactivate()
        {
            App.SerializationManager.Serializing -= SerializationManager_Serializing;
            App.HeaderControl.RootItemSelected -= HeaderControl_RootItemSelected;
            App.Map.FunctionModeChanged -= Map_FunctionModeChanged;
            App.Map.SelectionChanged -= Map_SelectionChanged;

            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        #endregion

        #region Private methods

        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            // Note: quick fix of http://hydrodesktop.codeplex.com/workitem/8360
            // DS do not serialize DotSpatial.Data.FeatureSet.

            // Remove the "Area rectangle" layer
            if (_rectangleDrawing != null)
            {
                _rectangleDrawing.Deactivate();
            }
        }

        private void AddSearchRibbon()
        {
            var head = App.HeaderControl;
            
            //Search ribbon tab
            //setting the sort order to small positive number to display it to the right of home tab
            head.Add(new RootItem(_searchKey, Msg.Search) { SortOrder = -10 });

            #region Area group

            head.Add(rbDrawBox = new SimpleActionItem(_searchKey, Msg.Draw_Rectangle, rbDrawBox_Click){LargeImage = Resources.Draw_Box_32, SmallImage = Resources.Draw_Box_16, GroupCaption = Msg.Area, ToggleGroupKey = Msg.Area});
            SearchSettings.Instance.AreaSettings.AreaRectangleChanged += Instance_AreaRectangleChanged;

            head.Add(new SimpleActionItem(_searchKey, Msg.Select_By_Attribute, rbAttribute_Click) { GroupCaption = Msg.Area, SmallImage = Resources.select_table_16 });
            
            head.Add(rbSelect = new SimpleActionItem(_searchKey, Msg.Select_Features, rbSelect_Click){ToolTipText = Msg.Select_Features_Tooltip, SmallImage = Resources.select_poly_16, GroupCaption = Msg.Area,ToggleGroupKey = Msg.Area, });
            SearchSettings.Instance.AreaSettings.PolygonsChanged += AreaSettings_PolygonsChanged;

            head.Add(new SimpleActionItem(_searchKey, Msg.Deselect_All, DeselectAll_Click) { GroupCaption = Msg.Area, ToolTipText = Msg.Deselect_All_Tooltip, SmallImage = Resources.deselect_16x16 });
            //head.Add(new SimpleActionItem(_searchKey, Msg.Zoom_Selected, ZoomSelected_Click) { GroupCaption = Msg.Area, ToolTipText = Msg.Zoom_Selected_Tooltip, SmallImage = Resources.zoom_selection_16x16 });
            
            head.Add(new SimpleActionItem(_searchKey, Msg.Pan, PanTool_Click) { GroupCaption = Msg.Area, SmallImage = Resources.hand_16x16, ToggleGroupKey = Msg.Area });
            head.Add(new SimpleActionItem(_searchKey, Msg.Zoom_In, ZoomIn_Click) { GroupCaption = Msg.Area, ToolTipText = Msg.Zoom_In_Tooltip, SmallImage = Resources.zoom_in_16x16, ToggleGroupKey = Msg.Area });
            head.Add(new SimpleActionItem(_searchKey, Msg.Zoom_Out, ZoomOut_Click) { GroupCaption = Msg.Area, ToolTipText = Msg.Zoom_Out_Tooltip, SmallImage = Resources.zoom_out_16x16, ToggleGroupKey = Msg.Area });

            head.Add(_currentView = new SimpleActionItem(_searchKey, Msg.Current_View, delegate { _useCurrentView = !_useCurrentView; }) { GroupCaption = Msg.Area, ToggleGroupKey = Msg.Current_View, ToolTipText = Msg.Current_View_Tooltip });
            _currentView.Toggling += delegate { _useCurrentView = !_useCurrentView; };
            _currentView.Toggle();

            #endregion

            #region Keyword Group

            RecreateKeywordGroup();
            SearchSettings.Instance.KeywordsSettings.KeywordsChanged += delegate { RecreateKeywordGroup(); };

            #endregion

            #region Dates group
            
            rbStartDate = new TextEntryActionItem { Caption = "Start", GroupCaption = Msg.Time_Range, RootKey = _searchKey, Width = 70 };
            rbStartDate.PropertyChanged += rbStartDate_PropertyChanged;
            head.Add(rbStartDate);

            rbEndDate = new TextEntryActionItem { Caption = " End", GroupCaption = Msg.Time_Range, RootKey = _searchKey, Width = 70 };
            head.Add(rbEndDate);
            rbEndDate.PropertyChanged += rbEndDate_PropertyChanged;
            UpdateDatesCaption();

            head.Add(new SimpleActionItem(_searchKey, Msg.Select_Dates, rbDate_Click) { GroupCaption = Msg.Time_Range, LargeImage = Resources.select_date_v1_32, SmallImage = Resources.select_date_v1_16 });

            #endregion

            #region Data Sources

            var grpDataSources = SharedConstants.SearchDataSourcesGroupName;
            rbServices = new SimpleActionItem("Select Data Sources", rbServices_Click);
            ChangeWebServicesIcon();
            rbServices.ToolTipText = "Select data sources (All web services selected)";
            rbServices.GroupCaption = grpDataSources;
            rbServices.RootKey = _searchKey;
            head.Add(rbServices);

            #endregion

            head.Add(new SimpleActionItem(_searchKey, Msg.Run_Search, rbSearch_Click) {GroupCaption = Msg.Search, LargeImage = Resources.search_32, SmallImage = Resources.search_16, ToolTipText = Msg.Run_Search_Tooltip, });
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
        /// Zoom Out
        /// </summary>
        private void ZoomOut_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.ZoomOut;
        }

        private void ZoomSelected_Click(object sender, EventArgs e)
        {
            const double distanceX = 2;
            const double distanceY = 2;
            const double EPS = 1e-7;

            IEnvelope envelope = null;
            foreach (var layer in ((Map)App.Map).GetAllLayers())
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
        /// Deselect All
        /// </summary>
        private void DeselectAll_Click(object sender, EventArgs e)
        {
            IEnvelope env = new Envelope();
            App.Map.MapFrame.ClearSelection(out env);
        }

        private void RecreateKeywordGroup()
        {
            HeaderItem dummy = null;
            if (_rbKeyword != null || _rbAddMoreKeywords != null)
            {
                // This need to save buttons group from removing by HeaderControl (it removes groups with zero HeaderItems).
                dummy = new SimpleActionItem(_searchKey, "Dummy", null) {GroupCaption = Msg.Keyword};
                App.HeaderControl.Add(dummy);
            }

            if (_rbKeyword != null)
            {
                App.HeaderControl.Remove(_rbKeyword.Key);
            }else
            {
                _rbKeyword = new DropDownActionItem
                                 {
                                     AllowEditingText = true,
                                     GroupCaption = Msg.Keyword,
                                     RootKey = _searchKey,
                                     Width = 165,
                                     Enabled = false,
                                     NullValuePrompt = Msg.Type_In_Keyword
                                 };
                _rbKeyword.SelectedValueChanged += rbKeyword_SelectedValueChanged;
            }
            if (_rbAddMoreKeywords != null)
            {
                App.HeaderControl.Remove(_rbAddMoreKeywords.Key);
            }
            else
            {
                _rbAddMoreKeywords = new SimpleActionItem(_searchKey, Msg.Add_More_Keywords, rbKeyword_Click)
                                         {
                                             LargeImage = Resources.keyword_32,
                                             SmallImage = Resources.keyword_16,
                                             GroupCaption = Msg.Keyword,
                                             ToolTipText = "Show Keyword Ontology Tree"
                                         };
            }

            // Populate items by keywords
            _rbKeyword.Items.Clear();
            _rbKeyword.Items.AddRange(SearchSettings.Instance.KeywordsSettings.Keywords);

            App.HeaderControl.Add(_rbKeyword);
            App.HeaderControl.Add(_rbAddMoreKeywords);
            if (dummy != null)
            {
                App.HeaderControl.Remove(dummy.Key);
                App.HeaderControl.SelectRoot(_searchKey);
            }

            UpdateKeywordsCaption();
        }
        
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == _searchKey)
            {
                App.SerializationManager.SetCustomSetting("SearchRootClicked", true);
                App.DockManager.SelectPanel("kMap");

                //searchSummary.ShowSearchStatus = true;
                //searchSummary.UpdateStatus();
            }
            else
            {
                //searchSummary.ShowSearchStatus = false;
            }
        }

        #region Search
        
        private DateTime? ValidateDateEdit(TextEntryActionItem item, string itemName, string dateFormat, bool showMessage)
        {
            DateTime? result = null;
            string error = null;
            try
            {
                var date = DateTime.ParseExact(item.Text, dateFormat, CultureInfo.CurrentCulture);

                var minDate = new DateTime(1753, 1, 1);
                var maxDate = DateTime.MaxValue;
                if (date < minDate || date > maxDate)
                {
                    throw new Exception(string.Format("Date must be between {0} and {1}", minDate.ToShortDateString(), maxDate.ToShortDateString()));
                }

                result = date;
            }
            catch (ArgumentNullException)
            {
                error = string.Format("{0} should be non-empty. Please enter {1} in the format {2}", itemName,
                                      itemName.ToLower(), dateFormat);
            }
            catch (FormatException)
            {
                error = string.Format("{0} is in incorrect format. Please enter {1} in the format {2}", itemName,
                                      itemName.ToLower(), dateFormat);
            }catch(Exception ex)
            {
                error = ex.Message;
            }

            if (!string.IsNullOrEmpty(error) && showMessage)
            {
                MessageBox.Show(error, string.Format("{0} validation", itemName), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return result;
        }

        private bool ValidateStartDate(bool showMessage)
        {
            var result = ValidateDateEdit(rbStartDate, "Start Date", _datesFormat, showMessage);
            if (result != null)
            {
                var date = result.Value;
                // Additional validation for start date
                if (SearchSettings.Instance.DateSettings.EndDate < date)
                {
                    if (showMessage)
                    {
                        MessageBox.Show("End Date must be greater than Start Date.", "Start Date validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return false;
                }

                SearchSettings.Instance.DateSettings.StartDate = date;
                return true;
            }
            return false;
        }

        private bool ValidateEndDate(bool showMessage)
        {
            var result = ValidateDateEdit(rbEndDate, "End Date", _datesFormat, showMessage);
            if (result != null)
            {
                SearchSettings.Instance.DateSettings.EndDate = result.Value;
                return true;
            }
            return false;
        }

        void rbSearch_Click(object sender, EventArgs e)
        {
            // Validation of Start/End date. 
            // First should be validated EndDate, because StartDate validation depends from EndDate
            if (!ValidateEndDate(true) || !ValidateStartDate(true))
            {
                return;
            }
            // end of validation

            if (_searcher == null)
            {
                _searcher = new Searcher();
                _searcher.Completed += _searcher_Completed;
            }

            try
            {
                if (!_searcher.IsUIVisible && _searcher.IsBusy)
                {
                    _searcher.ShowUI();
                }
                else
                {
                    if (_useCurrentView)
                    {
                        var extent = App.Map.ViewExtents;
                        var areaKm2 = extent.ToEnvelope().Area()/1e6;
                        if (areaKm2 > 1e6)
                        {
                            if (MessageBox.Show("Current view is too large. Search can take a long time. Do you want to continue?", 
                                "Continue search", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                            {
                                return;
                            }
                        }
                        SearchSettings.Instance.AreaSettings.SetAreaRectangle(extent, App.Map.Projection);
                    }
                    _searcher.Run(SearchSettings.Instance);
                }
            }
            catch (SearchSettingsException sex)
            {
                string message;
                if (sex is NoSelectedKeywordsException)
                    message = "Please provide at least one Keyword for search.";
                else if (sex is NoWebServicesException)
                    message = "Please provide at least one Web Service for search.";
                else if (sex is NoAreaToSearchException)
                    message = "Please provide at least one Target Area for search.";
                else
                    message = sex.Message;

                MessageBox.Show(message, Msg.Information, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Msg.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void _searcher_Completed(object sender, CompletedEventArgs e)
        {
            DeactivateDrawBox();

            if (e.Result == null) return;
            e.ProgressHandler.ReportMessage("Adding Sites to Map...");
            var result = e.Result;
            //We need to reproject the Search results from WGS84 to the projection of the map.
            ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
            foreach (var item in result.ResultItems)
                item.FeatureSet.Projection = wgs84;

            var layers = ShowSearchResults(result);
            // Select first search result layer
            var first = layers.FirstOrDefault();
            if (first != null)
            {
                first.IsSelected = true;
            }
            // Deselect "Map Layers" legend item (http://hydrodesktop.codeplex.com/workitem/8458)
            App.Legend.RootNodes
                .Where(r => r.IsSelected && r.LegendText == "Map Layers")
                .ToList()
                .ForEach(r => r.IsSelected = false);
        }

        /// <summary>
        /// Displays search results (all data series and sites complying to the search criteria)
        /// </summary>
        private IEnumerable<IMapPointLayer> ShowSearchResults(SearchResult searchResult)
        {
            //try to save the search result layer and re-add it
            var hdProjectPath = HydroDesktop.Configuration.Settings.Instance.CurrentProjectDirectory;

            var loadedFeatures = new List<SearchResultItem>(searchResult.ResultItems.Count());
            foreach (var key in searchResult.ResultItems)
            {
                var fs = key.FeatureSet;
                var filename = Path.Combine(hdProjectPath,
                                            string.Format(Properties.Settings.Default.SearchResultNameMask, key.ServiceCode));
                fs.Filename = filename;
                fs.Save();
                loadedFeatures.Add(new SearchResultItem(key.ServiceCode, FeatureSet.OpenFile(filename)));
            }

            var searchLayerCreator = new SearchLayerCreator(App.Map, new SearchResult(loadedFeatures));
            return searchLayerCreator.Create();
        }

        #endregion

        #region  Area group

        void Map_FunctionModeChanged(object sender, EventArgs e)
        {
            if (App.Map.FunctionMode == FunctionMode.Select && CurrentAreaSelectMode != AreaSelectMode.DrawBox)
            {
                CurrentAreaSelectMode = AreaSelectMode.SelectPolygons;
                rbSelect.Toggle();
            }
        }

        private AreaSelectMode CurrentAreaSelectMode
        {
            get; set;
        }

        private enum AreaSelectMode
        {
            DrawBox,
            SelectPolygons,
            SelectAttribute
        }

        void Instance_AreaRectangleChanged(object sender, EventArgs e)
        {
            var rectangle = SearchSettings.Instance.AreaSettings.AreaRectangle;
            rbDrawBox.ToolTipText = rectangle != null ? rectangle.ToString() : Msg.Draw_Box;

            //searchSummary.AreaStatus = "Rectangle " + rectangle != null ? rectangle.ToString() : Msg.Draw_Box;
            //searchSummary.AreaStatus = "Rectangle";
            //searchSummary.UpdateStatus();
        }

        void rbDrawBox_Click(object sender, EventArgs e)
        {
            CurrentAreaSelectMode = AreaSelectMode.DrawBox;

            DeactivateSelectAreaByPolygon();
            DeactivateCurrentView();

            if (_rectangleDrawing == null)
            {
                _rectangleDrawing = new RectangleDrawing((Map) App.Map);
                _rectangleDrawing.RectangleCreated += rectangleDrawing_RectangleCreated;
                _rectangleDrawing.Deactivated += _rectangleDrawing_Deactivated;
            }

            _rectangleDrawing.Activate();
        }

        void _rectangleDrawing_Deactivated(object sender, EventArgs e)
        {
            if (_isDeactivatingDrawBox) return;
            rbSelect_Click(this, EventArgs.Empty);
        }

        void rectangleDrawing_RectangleCreated(object sender, EventArgs e)
        {
            if (_rectangleDrawing == null) return;
            SearchSettings.Instance.AreaSettings.SetAreaRectangle(_rectangleDrawing.RectangleExtent, App.Map.Projection);
        }

        void AreaSettings_PolygonsChanged(object sender, EventArgs e)
        {
            var fsPolygons = SearchSettings.Instance.AreaSettings.Polygons;

            var caption = "0 features selected ";
            
            //var caption = "Select Polygons";
            if (fsPolygons != null && fsPolygons.Features.Count > 0)
            {
                int numPolygons = fsPolygons.Features.Count;
                caption = numPolygons > 1
                    ? String.Format("{0} features selected", fsPolygons.Features.Count)
                    : "1 feature selected";
            }

            //searchSummary.AreaStatus = SearchSettings.Instance.AreaSettings.AreaRectangle != null ? "Rectangle" : caption;
            //searchSummary.UpdateStatus();
            //rbSelect.Caption = caption;
            //rbSelect.ToolTipText = caption;
        }

        void rbSelect_Click(object sender, EventArgs e)
        {
            CurrentAreaSelectMode = AreaSelectMode.SelectPolygons;

            DeactivateDrawBox();
            DeactivateCurrentView();
            
            App.Map.FunctionMode = FunctionMode.Select;

            var isWorldTemplate = App.SerializationManager.GetCustomSetting("world_template", "false");
            AreaHelper.SelectFirstVisiblePolygonLayer((Map)App.Map, Convert.ToBoolean(isWorldTemplate));
            //App.Map.MapFrame.IsSelected = true;
        }
        
        private void DeactivateSelectAreaByPolygon()
        {
            SearchSettings.Instance.AreaSettings.Polygons = null;
        }

        void Map_SelectionChanged(object sender, EventArgs e)
        {
            if (CurrentAreaSelectMode == AreaSelectMode.SelectPolygons ||
                CurrentAreaSelectMode == AreaSelectMode.SelectAttribute)
            {
                var polygonLayer = AreaHelper.GetAllSelectedPolygonLayers((Map) App.Map).FirstOrDefault();
                if (polygonLayer == null)
                {
                    //special case: if the map layers or the group is selected
                    if (App.Map.MapFrame.IsSelected)
                    {
                        IEnumerable<IMapPolygonLayer> polygonLayers = AreaHelper.GetAllPolygonLayers((Map) App.Map).Reverse();
                        foreach(IMapPolygonLayer polyLayer in polygonLayers)
                        {
                            if (polyLayer.IsVisible && polyLayer.Selection.Count > 0)
                            {
                                var polyFs2 = new FeatureSet(FeatureType.Polygon);
                                foreach (var f in polyLayer.Selection.ToFeatureList())
                                {
                                    polyFs2.Features.Add(f);
                                }
                                polyFs2.Projection = App.Map.Projection;
                                SearchSettings.Instance.AreaSettings.Polygons = polyFs2;
                                return;
                            }

                        }
                    
                    }
                    return;
                }

                var polyFs = new FeatureSet(FeatureType.Polygon);
                foreach (var f in polygonLayer.Selection.ToFeatureList())
                {
                    polyFs.Features.Add(f);
                }
                polyFs.Projection = App.Map.Projection;
                SearchSettings.Instance.AreaSettings.Polygons = polyFs;
            }
        }

        private bool _isDeactivatingDrawBox;
        private void DeactivateDrawBox()
        {
            if (_rectangleDrawing == null) return;

            _isDeactivatingDrawBox = true;
            _rectangleDrawing.Deactivate();
            SearchSettings.Instance.AreaSettings.SetAreaRectangle((Box)null, null);
            _isDeactivatingDrawBox = false;
        }

        void rbAttribute_Click(object sender, EventArgs e)
        {
            CurrentAreaSelectMode = AreaSelectMode.SelectAttribute;

            DeactivateDrawBox();
            DeactivateSelectAreaByPolygon();
            DeactivateCurrentView();

            AreaHelper.SelectFirstVisiblePolygonLayer((Map)App.Map, false);
            SelectAreaByAttributeDialog.ShowDialog((Map)App.Map);
            Map_SelectionChanged(this, EventArgs.Empty);
            
            //App.Map.FunctionMode = FunctionMode.Select;
        }

        private void DeactivateCurrentView()
        {
            if (_useCurrentView)
            {
                _currentView.Toggle();
            }
        }

        #endregion

        #region Keywords

        void rbKeyword_SelectedValueChanged(object sender, SelectedValueChangedEventArgs e)
        {
            if (_keywordsUpdating) return;

            IList<string> keywords;
            if (e.SelectedItem == null)
            {
                keywords = null;
            }
            else
            {
                keywords = e.SelectedItem.ToString().Split(new[] { KEYWORDS_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // Replace keywords by synonyms
                var synonyms = SearchSettings.Instance.KeywordsSettings.Synonyms;
                if (synonyms != null)
                {
                    for (int i = 0; i < keywords.Count; i++)
                    {
                        var strNode = keywords[i];
                        foreach (var ontoPath in synonyms)
                        {
                            if (string.Equals(ontoPath.SearchableKeyword, strNode, StringComparison.OrdinalIgnoreCase))
                            {
                                keywords[i] = ontoPath.ConceptName;
                                break;
                            }
                        }
                    }
                }
            }

            SearchSettings.Instance.KeywordsSettings.SelectedKeywords = keywords;
        }

        private bool _keywordsUpdating;
        private void UpdateKeywordsCaption()
        {
            _keywordsUpdating = true;
            try
            {
                var keywords = SearchSettings.Instance.KeywordsSettings.SelectedKeywords.ToList();
                var sbKeywords = new StringBuilder();
                const string separator = KEYWORDS_SEPARATOR + " ";
                foreach (var key in keywords)
                {
                    sbKeywords.Append(key + separator);
                }
                // Remove last separator
                if (sbKeywords.Length > 0)
                {
                    sbKeywords.Remove(sbKeywords.Length - separator.Length, separator.Length);
                }

                var selectedItem = sbKeywords.Length > 0 ? sbKeywords.ToString() : null;
                _rbKeyword.SelectedItem = selectedItem;
                _rbKeyword.ToolTipText = selectedItem;
            }
            finally
            {
                _keywordsUpdating = false;
            }
        }

        void rbKeyword_Click(object sender, EventArgs e)
        {        
            if (KeywordsDialog.ShowDialog(SearchSettings.Instance.KeywordsSettings) == DialogResult.OK)
            {
                UpdateKeywordsCaption();
            }
        }

        #endregion

        #region WebServices

        void rbServices_Click(object Sender, EventArgs e)
        {
            if (WebServicesDialog.ShowDialog(SearchSettings.Instance.WebServicesSettings, 
                                             SearchSettings.Instance.CatalogSettings,
                                             SearchSettings.Instance.KeywordsSettings,
                                             App.GetExtension<IMetadataFetcherPlugin>()
                                             ) == DialogResult.OK)
            {
                UpdateWebServicesCaption();
            }
        }

        private void UpdateWebServicesCaption()
        {
            var webservicesSettings = SearchSettings.Instance.WebServicesSettings;
            var checkedCount = webservicesSettings.CheckedCount;
            var totalCount = webservicesSettings.TotalCount;

            string caption;
            string hint;
            WebServiceNode webServiceNode = null;
            if (checkedCount == totalCount)
            {
                caption = "All sources";
                hint = caption;
            }else if (checkedCount == 1)
            {
                // Get single checked item
                var items = webservicesSettings.WebServices.Where(w => w.Checked).ToList();
                Debug.Assert(items.Count == 1);
                webServiceNode = items[0];
                caption = items[0].Title;
                hint = caption;
            }
            else
            {
                caption = string.Format("{0} sources selected", checkedCount);
                hint = string.Format("{0} sources", checkedCount);
            }

            //rbServices.Caption = caption;
            rbServices.ToolTipText = string.Format("Select data sources ({0} selected)", hint);
            //ChangeWebServicesIcon(webServiceNode);
            
            //searchSummary.DataSourceStatus = caption;
            //searchSummary.UpdateStatus();
        }

        private void ChangeWebServicesIcon(WebServiceNode webServiceNode = null)
        {
            if (webServiceNode == null || 
                string.IsNullOrEmpty(webServiceNode.ServiceCode))
            {
                rbServices.LargeImage = Resources.server_32;
                rbServices.SmallImage = Resources.server_16;
                return;
            }

            try
            {
                var imageHelper = new ServiceIconHelper(SearchSettings.Instance.CatalogSettings.HISCentralUrl);
                var image = imageHelper.GetImageForService(webServiceNode.ServiceCode);
                rbServices.LargeImage = rbServices.SmallImage = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to change icon." + Environment.NewLine +
                                "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Dates

        private void UpdateDatesCaption()
        {
            rbStartDate.Text = SearchSettings.Instance.DateSettings.StartDate.ToString(_datesFormat);
            rbEndDate.Text = SearchSettings.Instance.DateSettings.EndDate.ToString(_datesFormat);
        }

        void rbDate_Click(object sender, EventArgs e)
        {
            if (DateSettingsDialog.ShowDialog(SearchSettings.Instance.DateSettings) == DialogResult.OK)
            {
                UpdateDatesCaption();
            }
        }

        void rbEndDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;
            ValidateEndDate(false);
        }

        void rbStartDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;
            ValidateStartDate(false);
        }

        #endregion

        #endregion

        public IList<DataServiceInfo> GetWebServices()
        {
            var webServices = SearchSettings.Instance.WebServicesSettings.WebServices;
            var result = new List<DataServiceInfo>(webServices.Count);
            result.AddRange(webServices.Select(wsInfo => new DataServiceInfo
                                                             {
                                                                 EndpointURL = wsInfo.ServiceUrl,
                                                                 DescriptionURL = wsInfo.DescriptionUrl,
                                                                 ServiceTitle = wsInfo.Title,
                                                                 HISCentralID = wsInfo.ServiceID
                                                             }));

            return result;
        }

        public void AddFeatures(List<Tuple<string, IFeatureSet>> featuresPerCode)
        {
            var loadedFeatures = new List<SearchResultItem>(featuresPerCode.Count());
            loadedFeatures.AddRange(featuresPerCode.Select(item => new SearchResultItem(item.Item1, item.Item2)));

            var searchLayerCreator = new SearchLayerCreator(App.Map, new SearchResult(loadedFeatures));
            searchLayerCreator.Create();
        }

        public string HisCentralUrl
        {
            get { return SearchSettings.Instance.CatalogSettings.HISCentralUrl; }
        }
    }
}
