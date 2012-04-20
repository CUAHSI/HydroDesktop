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
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using Search3.Area;
using Search3.Properties;
using Search3.Searching;
using Search3.Searching.Exceptions;
using Search3.Settings;
using Search3.Settings.UI;
using HydroDesktop.WebServices;

namespace Search3
{
    public class SearchPlugin : Extension, IWebServicesStore, ISearchPlugin
    {
        #region Fields

        const string kHydroSearch3 = "kHydroSearchV3";
        const string TYPE_IN_KEYWORD = "Type-in a Keyword";

        private SimpleActionItem rbServices;
        private SimpleActionItem rbCatalog;
        private TextEntryActionItem rbStartDate;
        private TextEntryActionItem rbEndDate;
        private DropDownActionItem rbKeyword;
        private SimpleActionItem rbDrawBox;
        private SimpleActionItem rbSelect;
        private SimpleActionItem rbAttribute;

        private RectangleDrawing _rectangleDrawing;
        private Searcher _searcher;

        private readonly string _datesFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        #endregion

        #region Plugin operations

        public override void Activate()
        {
            AddSearchRibbon();
            base.Activate();

            App.SerializationManager.Serializing += SerializationManager_Serializing;
        }

        public override void Deactivate()
        {
            App.SerializationManager.Serializing -= SerializationManager_Serializing;

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
            var root = new RootItem(kHydroSearch3, "Search") {SortOrder = -10};
            try
            {
                head.Add(root);
            }
            catch(ArgumentException)
            {
                //catch exception in case the root item has been already added
            } 

            #region Area group

            const string grpArea = "Area";

            //to get area select mode
            App.Map.FunctionModeChanged +=Map_FunctionModeChanged;
            App.Map.SelectionChanged += Map_SelectionChanged;

            //Draw Box
            rbDrawBox = new SimpleActionItem(kHydroSearch3, "Draw Rectangle", rbDrawBox_Click)
                            {
                                LargeImage = Resources.Draw_Box_32,
                                SmallImage = Resources.Draw_Box_16,
                                GroupCaption = grpArea,
                                ToggleGroupKey = grpArea
                            };
            head.Add(rbDrawBox);
            SearchSettings.Instance.AreaSettings.AreaRectangleChanged += Instance_AreaRectangleChanged;

            //Select
            rbSelect = new SimpleActionItem(kHydroSearch3, "Select Polygons", rbSelect_Click)
                           {
                               ToolTipText = "Select Region",
                               LargeImage = Resources.select_poly_32,
                               SmallImage = Resources.select_poly_16,
                               GroupCaption = grpArea,
                               ToggleGroupKey = grpArea
                           };
            head.Add(rbSelect);
            SearchSettings.Instance.AreaSettings.PolygonsChanged += AreaSettings_PolygonsChanged;

            //AttributeTable
            rbAttribute = new SimpleActionItem(kHydroSearch3, "Select by Attribute", rbAttribute_Click)
                              {
                                  ToolTipText = "Select by Attribute",
                                  GroupCaption = grpArea,
                                  ToggleGroupKey = grpArea,
                                  LargeImage = Resources.select_table_32,
                                  SmallImage = Resources.select_table_16
                              };
            head.Add(rbAttribute);

            #endregion

            #region Keyword Group

            RefreshKeywordDropDown();
            
            SearchSettings.Instance.KeywordsSettings.KeywordsChanged += delegate { RefreshKeywordDropDown(); };

            rbKeyword.SelectedValueChanged += rbKeyword_SelectedValueChanged;
            UpdateKeywordsCaption();

            //Keyword more options
            var rbKeyword2 = new SimpleActionItem("Keyword Selection", rbKeyword_Click)
                                 {
                                     LargeImage = Resources.keyword_32,
                                     SmallImage = Resources.keyword_16,
                                     GroupCaption = "Keyword",
                                     ToolTipText = "Show Keyword Ontology Tree",
                                     RootKey = kHydroSearch3
                                 };
            head.Add(rbKeyword2);

            #endregion

            #region Dates group

            const string grpDates = "Time Range";
            rbStartDate = new TextEntryActionItem
                              {Caption = "Start", GroupCaption = grpDates, RootKey = kHydroSearch3, Width = 60};
            rbStartDate.PropertyChanged += rbStartDate_PropertyChanged;
            head.Add(rbStartDate);

            rbEndDate = new TextEntryActionItem
                            {Caption = " End", GroupCaption = grpDates, RootKey = kHydroSearch3, Width = 60};
            head.Add(rbEndDate);
            rbEndDate.PropertyChanged += rbEndDate_PropertyChanged;
            UpdateDatesCaption();

            var rbDate = new SimpleActionItem("Select Time", rbDate_Click)
                             {
                                 GroupCaption = grpDates,
                                 RootKey = kHydroSearch3,
                                 LargeImage = Resources.select_date_v1_32,
                                 SmallImage = Resources.select_date_v1_16
                             };
            head.Add(rbDate);

            #endregion

            #region Data Sources

            const string grpDataSources = "Data Sources";
            rbServices = new SimpleActionItem("All Data Sources", rbServices_Click);
            ChangeWebServicesIcon();
            rbServices.ToolTipText = "Select data sources (All web services selected)";
            rbServices.GroupCaption = grpDataSources;
            rbServices.RootKey = kHydroSearch3;
            head.Add(rbServices);

            rbCatalog = new SimpleActionItem("HIS Central", rbCatalog_Click)
                            {
                                LargeImage = Resources.option_32,
                                SmallImage = Resources.option_16,
                                ToolTipText = "Select the Search Catalog",
                                GroupCaption = grpDataSources,
                                RootKey = kHydroSearch3
                            };
            head.Add(rbCatalog);
            UpdateCatalogCaption();

            #endregion

            #region Search and download buttons

            const string grpSearch = "Search";
            var rbSearch = new SimpleActionItem("Run Search", rbSearch_Click)
                               {
                                   LargeImage = Resources.search_32,
                                   SmallImage = Resources.search_16,
                                   ToolTipText = "Run Search based on selected criteria",
                                   GroupCaption = grpSearch,
                                   RootKey = kHydroSearch3
                               };
            head.Add(rbSearch);

            /*
            var btnDownload = new SimpleActionItem("Download", rbDownload_Click)
                                  {
                                      Enabled = false,
                                      RootKey = kHydroSearch3,
                                      GroupCaption = grpSearch,
                                      LargeImage = Resources.download_32,
                                      SmallImage = Resources.download_16
                                  };*/
            //App.HeaderControl.Add(btnDownload);

            #endregion

            App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;

            //map buttons (not added for now)
            //AddMapButtons();
        }

        void RefreshKeywordDropDown()
        {
            if (rbKeyword !=null) App.HeaderControl.Remove(rbKeyword.Key);

            //Keyword text entry
            if (rbKeyword == null)
            {
                const string grpKeyword = "Keyword";
                rbKeyword = new DropDownActionItem
                                {
                                    AllowEditingText = true,
                                    GroupCaption = grpKeyword,
                                    RootKey = kHydroSearch3,
                                    Width = 150,
                                    Enabled = false,
                                    NullValuePrompt = TYPE_IN_KEYWORD
                                };
            }

            // Populate items by keywords
            PopulateKeywords();

            App.HeaderControl.Add(rbKeyword);
            //SearchSettings.Instance.KeywordsSettings.KeywordsChanged += delegate { PopulateKeywords(); };
        }
        
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == kHydroSearch3)
            {
                App.SerializationManager.SetCustomSetting("SearchRootClicked", true);
                App.DockManager.SelectPanel("kMap");
            }
        }

        //private void AddMapButtons()
        //{
        //    string kHomeRoot = kHydroSearch3;
        //    string rpMapTools = "Map Tools";
        //    string kHydroMapTools = "kHydroMapToolsSearch";
        //    var head = App.HeaderControl;
            
        //    //Pan
        //    var _rbPan = new SimpleActionItem(kHomeRoot, "Pan", rbPan_Click);
        //    _rbPan.GroupCaption = rpMapTools;
        //    _rbPan.LargeImage = Properties.Resources.pan;
        //    _rbPan.SmallImage = Properties.Resources.pan_16;
        //    _rbPan.ToolTipText = "Pan - Move the Map";
        //    _rbPan.ToggleGroupKey = kHydroMapTools;
        //    head.Add(_rbPan);

        //    //ZoomIn
        //    var _rbZoomIn = new SimpleActionItem(kHomeRoot, "Zoom In", rbZoomIn_Click);
        //    _rbZoomIn.ToolTipText = "Zoom In";
        //    _rbZoomIn.GroupCaption = rpMapTools;
        //    _rbZoomIn.LargeImage = Properties.Resources.zoom_in;
        //    _rbZoomIn.SmallImage = Properties.Resources.zoom_in_16;
        //    _rbZoomIn.ToggleGroupKey = kHydroMapTools;
        //    head.Add(_rbZoomIn);

        //    //ZoomOut
        //    var _rbZoomOut = new SimpleActionItem(kHomeRoot, "Zoom Out", rbZoomOut_Click);
        //    _rbZoomOut.ToolTipText = "Zoom Out";
        //    _rbZoomOut.GroupCaption = rpMapTools;
        //    _rbZoomOut.LargeImage = Properties.Resources.zoom_out;
        //    _rbZoomOut.SmallImage = Properties.Resources.zoom_out_16;
        //    _rbZoomOut.ToggleGroupKey = kHydroMapTools;
        //    head.Add(_rbZoomOut);

        //    //ZoomToFullExtent
        //    var _rbMaxExtents = new SimpleActionItem(kHomeRoot, "MaxExtents", rbFullExtent_Click);
        //    _rbMaxExtents.ToolTipText = "Maximum Extents";
        //    _rbMaxExtents.GroupCaption = rpMapTools;
        //    _rbMaxExtents.LargeImage = Properties.Resources.full_extent;
        //    _rbMaxExtents.SmallImage = Properties.Resources.full_extent_16;
        //    head.Add(_rbMaxExtents);

            
        //}
        
        //void rbPan_Click(object sender, EventArgs e) { }
        //void rbZoomIn_Click(object sender, EventArgs e) { }
        //void rbZoomOut_Click(object sender, EventArgs e) { }
        //void rbFullExtent_Click(object sender, EventArgs e) { }
        //void rbDownload_Click(object sender, EventArgs e) { }

        #region Search
        
        private DateTime? ValidateDateEdit(TextEntryActionItem item, string itemName, string dateFormat, bool showMessage)
        {
            var validateDate = (Func<string, DateTime?>)delegate(string str)
            {
                try
                {
                    return DateTime.ParseExact(str, dateFormat, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    return null;
                }
            };
            var result = validateDate(item.Text);
            if (result == null && showMessage)
            {
                MessageBox.Show(string.Format("{0} is in incorrect format. Please enter {1} in the format {2}", itemName, itemName.ToLower(), dateFormat),
                                string.Format("{0} validation", itemName), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return result;
        }

        void rbSearch_Click(object sender, EventArgs e)
        {
            // Validation of Start/End date
            if (ValidateDateEdit(rbStartDate, "Start Date", _datesFormat, true) == null ||
                ValidateDateEdit(rbEndDate, "End Date", _datesFormat, true) == null)
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

                MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            ShowSearchResults(result);

            // Activate metadata ribbon tab
            App.HeaderControl.SelectRoot(SharedConstants.MetadataRootKey);
        }

        /// <summary>
        /// Displays search results (all data series and sites complying to the search criteria)
        /// </summary>
        private void ShowSearchResults(SearchResult searchResult)
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
            searchLayerCreator.Create();
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
            None,
            DrawBox,
            SelectPolygons,
            SelectAttribute
        }

        void Instance_AreaRectangleChanged(object sender, EventArgs e)
        {
            var rectangle = SearchSettings.Instance.AreaSettings.AreaRectangle;
            rbDrawBox.ToolTipText = rectangle != null ? rectangle.ToString() : "Draw Box";
        }

        void rbDrawBox_Click(object sender, EventArgs e)
        {
            CurrentAreaSelectMode = AreaSelectMode.DrawBox;

            DeactivateSelectAreaByPolygon();

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

            var xMin = _rectangleDrawing.RectangleExtent.MinX;
            var yMin = _rectangleDrawing.RectangleExtent.MinY;
            var xMax = _rectangleDrawing.RectangleExtent.MaxX;
            var yMax = _rectangleDrawing.RectangleExtent.MaxY;

            SearchSettings.Instance.AreaSettings.SetAreaRectangle(new Box(xMin, xMax, yMin, yMax), App.Map.Projection);
        }

        void AreaSettings_PolygonsChanged(object sender, EventArgs e)
        {
            var fsPolygons = SearchSettings.Instance.AreaSettings.Polygons;
            var caption = "Select Polygons";
            if (fsPolygons != null && fsPolygons.Features.Count > 0)
            {
                int numPolygons = fsPolygons.Features.Count;
                caption = numPolygons > 1
                    ? String.Format("{0} polygons selected", fsPolygons.Features.Count)
                    : "1 polygon selected";
            }
            
            rbSelect.Caption = caption;
            rbSelect.ToolTipText = caption;
        }

        void rbSelect_Click(object sender, EventArgs e)
        {
            CurrentAreaSelectMode = AreaSelectMode.SelectPolygons;

            DeactivateDrawBox();
            
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
                                var polyFs2 = new FeatureSet(DotSpatial.Topology.FeatureType.Polygon);
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

                var polyFs = new FeatureSet(DotSpatial.Topology.FeatureType.Polygon);
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
            SearchSettings.Instance.AreaSettings.SetAreaRectangle(null, null);
            _isDeactivatingDrawBox = false;
        }

        void rbAttribute_Click(object sender, EventArgs e)
        {
            CurrentAreaSelectMode = AreaSelectMode.SelectAttribute;

            DeactivateDrawBox();
            DeactivateSelectAreaByPolygon();

            AreaHelper.SelectFirstVisiblePolygonLayer((Map)App.Map, false);
            SelectAreaByAttributeDialog.ShowDialog((Map)App.Map);
            Map_SelectionChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Keywords

        private const string KEYWORDS_SEPARATOR = ";";

        private void PopulateKeywords()
        {
            // Populate items by keywords
            rbKeyword.Items.Clear();
            rbKeyword.Items.AddRange(SearchSettings.Instance.KeywordsSettings.Keywords);
            
            //rbKeyword.NullValuePrompt = TYPE_IN_KEYWORD;
        }

        void rbKeyword_SelectedValueChanged(object sender, SelectedValueChangedEventArgs e)
        {
            if (_keywordsUpdating) return;
            
            if (e.SelectedItem == null)
            {
                SearchSettings.Instance.KeywordsSettings.SelectedKeywords = null;
            }
            else
            {
                var keywords = e.SelectedItem.ToString().Split(new[] { KEYWORDS_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // Replace keywords by synonyms
                if (SearchSettings.Instance.KeywordsSettings.Synonyms != null)
                {
                    for (int i = 0; i < keywords.Count; i++)
                    {
                        var strNode = keywords[i];
                        foreach (var ontoPath in SearchSettings.Instance.KeywordsSettings.Synonyms)
                        {
                            if (string.Equals(ontoPath.SearchableKeyword, strNode,
                                              StringComparison.InvariantCultureIgnoreCase))
                            {
                                keywords[i] = ontoPath.ConceptName;
                                break;
                            }
                        }
                    }
                }

                SearchSettings.Instance.KeywordsSettings.SelectedKeywords = keywords;
            }
            UpdateKeywordsCaption();
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

                if (sbKeywords.Length > 0)
                {
                    rbKeyword.SelectedItem = sbKeywords.ToString();
                }
                //rbKeyword.SelectedItem = sbKeywords.Length > 0 ? sbKeywords.ToString() : TYPE_IN_KEYWORD;
                //rbKeyword.ToolTipText = rbKeyword.SelectedItem.ToString();
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
            if (WebServicesDialog.ShowDialog(SearchSettings.Instance.WebServicesSettings) == DialogResult.OK)
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
                caption = "All services";
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
                caption = string.Format("{0} services selected", checkedCount);
                hint = string.Format("{0} services", checkedCount);
            }

            rbServices.Caption = caption;
            rbServices.ToolTipText = string.Format("Select web services ({0} selected)", hint);
            ChangeWebServicesIcon(webServiceNode);
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

            var result = ValidateDateEdit(rbEndDate, "End Date", _datesFormat, false);
            if (result != null)
            {
                SearchSettings.Instance.DateSettings.EndDate = result.Value;
            }
        }

        void rbStartDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;

            var result = ValidateDateEdit(rbStartDate, "Start Date", _datesFormat, false);
            if (result != null)
            {
                SearchSettings.Instance.DateSettings.StartDate = result.Value;
            }
        }

        #endregion

        #region Catalog

        void rbCatalog_Click(object sender, EventArgs e)
        {
            if (SearchCatalogSettingsDialog.ShowDialog(SearchSettings.Instance.CatalogSettings,
                                                       SearchSettings.Instance.WebServicesSettings,
                                                       SearchSettings.Instance.KeywordsSettings) == DialogResult.OK)
            {
                UpdateCatalogCaption();
                UpdateWebServicesCaption();
                UpdateKeywordsCaption();
            }
        }

        private void UpdateCatalogCaption()
        {
            rbCatalog.Caption = SearchSettings.Instance.CatalogSettings.TypeOfCatalog.Description();
            rbCatalog.ToolTipText = "Select search catalog (selected catalog: " + SearchSettings.Instance.CatalogSettings.TypeOfCatalog.Description();
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

    }
}
