using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Projections;
using Search3.Area;
using Search3.Extensions;
using Search3.Properties;
using Search3.Searching;
using Search3.Searching.Exceptions;
using Search3.Settings;
using Search3.Settings.UI;
using HydroDesktop.WebServices;

namespace Search3
{
    public class SearchPlugin: Extension
    {
        #region Fields

        const string kHydroSearch3 = "kHydroSearchV3";
        const string TYPE_IN_KEYWORD = "Type-in a Keyword";

        private SimpleActionItem rbServices;
        private SimpleActionItem rbCatalog;
        private TextEntryActionItem rbStartDate;
        private TextEntryActionItem rbEndDate;
        private TextEntryActionItem rbKeyword;
        private SimpleActionItem rbDrawBox;
        private SimpleActionItem rbSelect;

        private RectangleDrawing _rectangleDrawing;
        private Searcher _searcher;
        private bool _isManualKeywordTextEdit = true;

        #endregion

        #region Plugin operations

        public override void Activate()
        {
            AddSearchRibbon();
            base.Activate(); 
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        #endregion

        #region Private methods

        private void AddSearchRibbon()
        {
            var head = App.HeaderControl;
            
            //Search ribbon tab
            var root = new RootItem(kHydroSearch3, "Search");
            //setting the sort order to small positive number to display it to the right of home tab
            root.SortOrder = -1; 
            head.Add(root);

            #region Area group

            const string grpArea = "Area";

            //Draw Box
            rbDrawBox = new SimpleActionItem(kHydroSearch3, "Draw Rectangle", rbDrawBox_Click);
            rbDrawBox.LargeImage = Resources.Draw_Box_32;
            rbDrawBox.SmallImage = Resources.Draw_Box_16;
            rbDrawBox.GroupCaption = grpArea;
            rbDrawBox.ToggleGroupKey = grpArea;
            head.Add(rbDrawBox);
            SearchSettings.Instance.AreaSettings.AreaRectangleChanged += Instance_AreaRectangleChanged;

            //Select
            rbSelect = new SimpleActionItem(kHydroSearch3, "Select Polygons", rbSelect_Click);
            rbSelect.ToolTipText = "Select Region";
            rbSelect.GroupCaption = grpArea;
            rbSelect.LargeImage = Resources.select_poly_32;
            rbSelect.SmallImage = Resources.select_poly_16;
            rbSelect.ToggleGroupKey = grpArea;
            head.Add(rbSelect);
            SearchSettings.Instance.AreaSettings.PolygonsChanged += AreaSettings_PolygonsChanged;

            //AttributeTable
            var rbAttribute = new SimpleActionItem(kHydroSearch3, "Select by Attribute", rbAttribute_Click);
            rbAttribute.ToolTipText = "Select by Attribute";
            rbAttribute.GroupCaption = grpArea;
            rbAttribute.LargeImage = Resources.select_table_32;
            rbAttribute.SmallImage = Resources.select_table_16;
            head.Add(rbAttribute);

            #endregion

            #region Keyword Group

            //Keyword text entry
            const string grpKeyword = "Keyword";
            rbKeyword = new TextEntryActionItem();
            rbKeyword.PropertyChanged += rbKeyword_PropertyChanged;
            rbKeyword.GroupCaption = grpKeyword;
            rbKeyword.RootKey = kHydroSearch3;
            rbKeyword.Width = 150;
            rbKeyword.Enabled = false;
            head.Add(rbKeyword);
            UpdateKeywordsCaption();

            //Keyword more options
            var rbKeyword2 = new SimpleActionItem("Keyword Selection", rbKeyword_Click);       
            rbKeyword2.LargeImage = Resources.keyword_32;
            rbKeyword2.SmallImage = Resources.keyword_16;
            rbKeyword2.GroupCaption = grpKeyword;
            rbKeyword2.ToolTipText = "Show Keyword Ontology Tree";
            rbKeyword2.RootKey = kHydroSearch3;
            head.Add(rbKeyword2);

            #endregion

            #region Dates group

            const string grpDates = "Time Range";
            rbStartDate = new TextEntryActionItem();
            rbStartDate.Caption = "Start";
            rbStartDate.GroupCaption = grpDates;
            rbStartDate.RootKey = kHydroSearch3;
            rbStartDate.Width = 60;
            rbStartDate.PropertyChanged += rbStartDate_PropertyChanged;
            head.Add(rbStartDate);        

            rbEndDate = new TextEntryActionItem();
            rbEndDate.Caption = " End";
            rbEndDate.GroupCaption = grpDates;
            rbEndDate.RootKey = kHydroSearch3;
            rbEndDate.Width = 60;
            head.Add(rbEndDate);
            rbEndDate.PropertyChanged += rbEndDate_PropertyChanged;
            UpdateDatesCaption();

            var rbDate = new SimpleActionItem("Select Time", rbDate_Click);
            rbDate.GroupCaption = grpDates;
            rbDate.RootKey = kHydroSearch3;
            rbDate.LargeImage = Resources.select_date_v1_32;
            rbDate.SmallImage = Resources.select_date_v1_16;
            head.Add(rbDate);

            #endregion

            #region Data Sources

            const string grpDataSources = "Data Sources";
            rbServices = new SimpleActionItem("All Services", rbServices_Click);
            ChangeWebServicesIcon();
            rbServices.ToolTipText = "Select web services (All Services selected)";
            rbServices.GroupCaption = grpDataSources;
            rbServices.RootKey = kHydroSearch3;
            head.Add(rbServices);

            rbCatalog = new SimpleActionItem("HIS Central", rbCatalog_Click);
            rbCatalog.LargeImage = Resources.option_32;
            rbCatalog.SmallImage = Resources.option_16;
            rbCatalog.GroupCaption = grpDataSources;
            rbCatalog.RootKey = kHydroSearch3;
            head.Add(rbCatalog);
            UpdateCatalogCaption();

            #endregion

            #region Search and download buttons

            string grpSearch = "Search";
            var rbSearch = new SimpleActionItem("Run Search", rbSearch_Click);
            rbSearch.LargeImage = Resources.search_32;
            rbSearch.SmallImage = Resources.search_16;
            rbSearch.ToolTipText = "Run Search based on selected criteria";
            rbSearch.GroupCaption = grpSearch;
            rbSearch.RootKey = kHydroSearch3;
            head.Add(rbSearch);
            
            var btnDownload = new SimpleActionItem("Download", rbDownload_Click);
            btnDownload.Enabled = false;
            btnDownload.RootKey = kHydroSearch3;
            btnDownload.GroupCaption = grpSearch;
            btnDownload.LargeImage = Resources.download_32;
            btnDownload.SmallImage = Resources.download_16;
            //App.HeaderControl.Add(btnDownload);

            #endregion

            //map buttons (not added for now)
            //AddMapButtons();
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
        
        void rbDownload_Click(object sender, EventArgs e) { }

        #region Search
        
        void rbSearch_Click(object sender, EventArgs e)
        {
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
            var result = e.Result;
            //We need to reproject the Search results from WGS84 to the projection of the map.
            ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
            foreach (var item in result.ResultItems)
                item.FeatureSet.Projection = wgs84;

            ShowSearchResults(result);
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
                                            string.Format(Properties.Settings.Default.SearchResultNameMask, key.SeriesDataCart.ServCode));
                fs.Filename = filename;
                fs.Save();
                loadedFeatures.Add(new SearchResultItem(key.SeriesDataCart, FeatureSet.OpenFile(filename)));
            }

            var searchLayerCreator = new SearchLayerCreator(App.Map, new SearchResult(loadedFeatures), Resources.SearchGroupName);
            searchLayerCreator.Create();
        }

        #endregion

        #region  Area group

        void Instance_AreaRectangleChanged(object sender, EventArgs e)
        {
            var rectangle = SearchSettings.Instance.AreaSettings.AreaRectangle;
            rbDrawBox.ToolTipText = rectangle != null ? rectangle.ToString() : "Draw Box";
        }

        void rbDrawBox_Click(object Sender, EventArgs e)
        {
            DeactivateSelectAreaByPolygon();

            if (_rectangleDrawing == null)
            {
                _rectangleDrawing = new RectangleDrawing(App.Map);
                _rectangleDrawing.RectangleCreated += rectangleDrawing_RectangleCreated;
            }

            _rectangleDrawing.Activate();
        }

        void rectangleDrawing_RectangleCreated(object sender, EventArgs e)
        {
            if (_rectangleDrawing == null) return;

            var xMin = _rectangleDrawing.RectangleExtent.MinX;
            var yMin = _rectangleDrawing.RectangleExtent.MinY;
            var xMax = _rectangleDrawing.RectangleExtent.MaxX;
            var yMax = _rectangleDrawing.RectangleExtent.MaxY;

            var xy = new[] { xMin, yMin, xMax, yMax };

            string esri = Resources.wgs_84_esri_string;
            var wgs84 = ProjectionInfo.FromEsriString(esri);

            Reproject.ReprojectPoints(xy, new double[] { 0, 0 }, App.Map.Projection, wgs84, 0, 2);

            // Save rectangle
            var xmin = xy[0];
            var ymin = xy[1];
            var xmax = xy[2];
            var ymax = xy[3];
            var rectangle = new Box(xmin, xmax, ymin, ymax);
            SearchSettings.Instance.AreaSettings.AreaRectangle = rectangle;
        }

        void AreaSettings_PolygonsChanged(object sender, EventArgs e)
        {
            var fsPolygons = SearchSettings.Instance.AreaSettings.Polygons;
            var caption = fsPolygons != null && fsPolygons.Features.Count > 0
                                       ? string.Format("{0} polygons selected", fsPolygons.Features.Count)
                                       : "Select Polygons";
            rbSelect.Caption = caption;
            rbSelect.ToolTipText = caption;
        }

        void rbSelect_Click(object sender, EventArgs e)
        {
            DeactivateSelectAreaByPolygon();
            DeactivateDrawBox();

            App.Map.FunctionMode = FunctionMode.Select;

            AreaHelper.SelectFirstVisiblePolygonLayer((Map)App.Map);
            App.Map.SelectionChanged += Map_SelectionChanged;
        }
        
        private void DeactivateSelectAreaByPolygon()
        {
            App.Map.SelectionChanged -= Map_SelectionChanged;
            SearchSettings.Instance.AreaSettings.Polygons = null;
        }

        void Map_SelectionChanged(object sender, EventArgs e)
        {
            foreach (var polygonLayer in AreaHelper.GetAllSelectedPolygonLayers((Map)App.Map))
            {
                var polyFs = new FeatureSet(DotSpatial.Topology.FeatureType.Polygon);
                foreach (var f in polygonLayer.Selection.ToFeatureList())
                {
                    polyFs.Features.Add(f);
                }
                polyFs.Projection = App.Map.Projection;
                SearchSettings.Instance.AreaSettings.Polygons = polyFs;


                //todo: which layer should be saved, if there are several active layers?
                break;
            }
        }

        private void DeactivateDrawBox()
        {
            if (_rectangleDrawing == null) return;

            _rectangleDrawing.Deactivate();
            SearchSettings.Instance.AreaSettings.AreaRectangle = null;
        }

        void rbAttribute_Click(object sender, EventArgs e)
        {
            DeactivateDrawBox();
            DeactivateSelectAreaByPolygon();

            AreaHelper.SelectFirstVisiblePolygonLayer((Map)App.Map);
            SelectAreaByAttributeDialog.ShowDialog((Map)App.Map);
            Map_SelectionChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Keywords

        void rbKeyword_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!_isManualKeywordTextEdit) return;
            if (e.PropertyName != "Text") return;
            
            //todo: implement tooltip with keyword mathcing
            rbKeyword.ToolTipText = "Matches....";
        }

        private void UpdateKeywordsCaption()
        {
            var keywords = SearchSettings.Instance.KeywordsSettings.SelectedKeywords.ToList();
            var sbKeywords = new StringBuilder();
            const string separator = "; ";
            foreach(var key in keywords)
            {
                sbKeywords.Append(key + separator);
            }
            // Remove last separator
            if (sbKeywords.Length > 0)
            {
                sbKeywords.Remove(sbKeywords.Length - separator.Length, separator.Length);
            }

            _isManualKeywordTextEdit = false;
            rbKeyword.Text = sbKeywords.Length > 0 ? sbKeywords.ToString() : TYPE_IN_KEYWORD;
            _isManualKeywordTextEdit = true;
            rbKeyword.ToolTipText = rbKeyword.Text;
        }

        void rbKeyword_Click(object Sender, EventArgs e)
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
                var items = webservicesSettings.WebServices.Where((w) => w.Checked).ToList();
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
            rbStartDate.Text = SearchSettings.Instance.DateSettings.StartDate.ToShortDateString();
            rbEndDate.Text = SearchSettings.Instance.DateSettings.EndDate.ToShortDateString();
        }

        void rbDate_Click(object Sender, EventArgs e)
        {
            if (DateSettingsDialog.ShowDialog(SearchSettings.Instance.DateSettings) == DialogResult.OK)
            {
                UpdateDatesCaption();
            }
        }

        void rbEndDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;

            DateTime result;
            if (DateTime.TryParse(rbEndDate.Text, out result))
                SearchSettings.Instance.DateSettings.EndDate = result;
        }

        void rbStartDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;

            DateTime result;
            if (DateTime.TryParse(rbStartDate.Text, out result))
                SearchSettings.Instance.DateSettings.StartDate = result;
        }

        #endregion

        #region Catalog

        void rbCatalog_Click(object Sender, EventArgs e)
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
        }

        #endregion

        #endregion
    }
}
