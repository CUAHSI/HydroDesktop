﻿using System;
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
using FacetedSearch3.Area;
using FacetedSearch3.Extensions;
using FacetedSearch3.Properties;
using FacetedSearch3.Searching;
using FacetedSearch3.Searching.Exceptions;
using FacetedSearch3.Settings;
using FacetedSearch3.Settings.UI;

namespace FacetedSearch3
{
    public class FacetedSearchPlugin: Extension
    {
        #region Fields

        const string kHydroFacetedSearch3 = "kHydroFacetedSearchV3";
        const string TYPE_IN_KEYWORD = "Type-in a Keyword";

        // private SimpleActionItem rbServices;
        // private SimpleActionItem rbCatalog;
        private TextEntryActionItem rbStartDate;
        private TextEntryActionItem rbEndDate;
        // private TextEntryActionItem rbKeyword;
        private SimpleActionItem rbDrawBox;
        private SimpleActionItem rbSelect;
        private SimpleActionItem rbAttribute;

        private FacetedSearchControl fsc;     

        private RectangleDrawing _rectangleDrawing;
        // private Searcher _searcher;
        // private bool _isManualKeywordTextEdit = true;

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
            App.DockManager.Remove(kHydroFacetedSearch3);
            base.Deactivate();
        }

        #endregion

        #region Private methods

        private void AddSearchRibbon()
        {
            var head = App.HeaderControl;
            
            //Search ribbon tab
            var root = new RootItem(kHydroFacetedSearch3, "Faceted Search");
            //setting the sort order to small positive number to display it to the right of home tab
            root.SortOrder = -1; 
            head.Add(root);

            #region Area group

            const string grpArea = "Area";

            //to get area select mode
            App.Map.FunctionModeChanged += new EventHandler(Map_FunctionModeChanged);
            App.Map.SelectionChanged += Map_SelectionChanged;

            //Draw Box
            rbDrawBox = new SimpleActionItem(kHydroFacetedSearch3, "Draw Rectangle", rbDrawBox_Click);
            rbDrawBox.LargeImage = Resources.Draw_Box_32;
            rbDrawBox.SmallImage = Resources.Draw_Box_16;
            rbDrawBox.GroupCaption = grpArea;
            rbDrawBox.ToggleGroupKey = grpArea;
            head.Add(rbDrawBox);
            SearchSettings.Instance.AreaSettings.AreaRectangleChanged += Instance_AreaRectangleChanged;

            //Select
            rbSelect = new SimpleActionItem(kHydroFacetedSearch3, "Select Polygons", rbSelect_Click);
            rbSelect.ToolTipText = "Select Region";
            rbSelect.LargeImage = Resources.select_poly_32;
            rbSelect.SmallImage = Resources.select_poly_16;
            rbSelect.GroupCaption = grpArea;
            rbSelect.ToggleGroupKey = grpArea;
            head.Add(rbSelect);
            SearchSettings.Instance.AreaSettings.PolygonsChanged += AreaSettings_PolygonsChanged;

            //AttributeTable
            rbAttribute = new SimpleActionItem(kHydroFacetedSearch3, "Select by Attribute", rbAttribute_Click);
            rbAttribute.ToolTipText = "Select by Attribute";
            rbAttribute.GroupCaption = grpArea;
            rbAttribute.ToggleGroupKey = grpArea;
            rbAttribute.LargeImage = Resources.select_table_32;
            rbAttribute.SmallImage = Resources.select_table_16;
            head.Add(rbAttribute);

            #endregion

            #region do not implement these for now - use attribute table selection instead

            ////Select Layer
            //var rbSelectLayer = new DropDownActionItem("kSearch3LayerDropDown", "Layer");
            //rbSelectLayer.GroupCaption = grpArea;
            //rbSelectLayer.AllowEditingText = true;
            //rbSelectLayer.Items.Add("Countries");
            //rbSelectLayer.Items.Add("U.S. States");
            //rbSelectLayer.Items.Add("u.S. HUC");
            //rbSelectLayer.Width = 120;
            //rbSelectLayer.RootKey = kHydroSearch3;
            //head.Add(rbSelectLayer);

            ////Select Field
            //var rbSelectField = new DropDownActionItem("kSearch3FieldDropDown", "Field_");
            //rbSelectField.GroupCaption = grpArea;
            //rbSelectField.AllowEditingText = true;
            //rbSelectField.Items.Add("Name");
            //rbSelectField.Items.Add("Population");
            //rbSelectField.Items.Add("FIPS");
            //rbSelectField.Width = 120;
            ////rbSelectLayer.SelectedItem = "Countries";
            //rbSelectField.RootKey = kHydroSearch3;
            //head.Add(rbSelectField);

            ////Select Value
            //var rbSelectVal = new DropDownActionItem("kSearch3ValueDropDown", "Value ");
            //rbSelectVal.GroupCaption = grpArea;
            //rbSelectVal.AllowEditingText = true;
            //rbSelectVal.Items.Add("Afghanistan");
            //rbSelectVal.Items.Add("Australia");
            //rbSelectVal.Items.Add("Austria");
            //rbSelectVal.Width = 120;
            ////rbSelectLayer.SelectedItem = "Countries";
            //rbSelectVal.RootKey = kHydroSearch3;
            //head.Add(rbSelectVal);

            //rbSelectLayer.SelectedItem = "Countries";
            //rbSelectField.SelectedItem = "Name";

            #endregion

            #region Keyword Group

            // Keyword text entry
            /*
            const string grpKeyword = "Keyword";
            rbKeyword = new TextEntryActionItem();
            rbKeyword.PropertyChanged += rbKeyword_PropertyChanged;
            rbKeyword.GroupCaption = grpKeyword;
            rbKeyword.RootKey = kHydroFacetedSearch3;
            rbKeyword.Width = 150;
            head.Add(rbKeyword);
            UpdateKeywordsCaption();

            //Keyword more options
            var rbKeyword2 = new SimpleActionItem("Keyword Selection", rbKeyword_Click);       
            rbKeyword2.LargeImage = Resources.keyword_32;
            rbKeyword2.SmallImage = Resources.keyword_16;
            rbKeyword2.GroupCaption = grpKeyword;
            rbKeyword2.ToolTipText = "Show Keyword Ontology Tree";
            rbKeyword2.RootKey = kHydroFacetedSearch3;
            head.Add(rbKeyword2);
            */
            #endregion

            #region Dates group

            const string grpDates = "Time Range";
            rbStartDate = new TextEntryActionItem();
            rbStartDate.Caption = "Start";
            rbStartDate.GroupCaption = grpDates;
            rbStartDate.RootKey = kHydroFacetedSearch3;
            rbStartDate.Width = 60;
            rbStartDate.PropertyChanged += rbStartDate_PropertyChanged;
            head.Add(rbStartDate);        

            rbEndDate = new TextEntryActionItem();
            rbEndDate.Caption = " End";
            rbEndDate.GroupCaption = grpDates;
            rbEndDate.RootKey = kHydroFacetedSearch3;
            rbEndDate.Width = 60;
            head.Add(rbEndDate);
            rbEndDate.PropertyChanged += rbEndDate_PropertyChanged;
            UpdateDatesCaption();

            var rbDate = new SimpleActionItem("Select Dates", rbDate_Click);
            rbDate.GroupCaption = grpDates;
            rbDate.RootKey = kHydroFacetedSearch3;
            rbDate.LargeImage = Resources.select_date_v1_32;
            rbDate.SmallImage = Resources.select_date_v1_16;
            head.Add(rbDate);

            #endregion

            #region Web Services group
            /*
            const string grpServices = "Web Services";
            rbServices = new SimpleActionItem("All Services", rbServices_Click);
            ChangeWebServicesIcon();
            rbServices.ToolTipText = "Select web services (All Services selected)";
            rbServices.GroupCaption = grpServices;
            rbServices.RootKey = kHydroFacetedSearch3;
            head.Add(rbServices);
            */
            #endregion

            #region Catalog group
            /*
            const string grpCatalog = "Catalog";
            rbCatalog = new SimpleActionItem("HIS Central", rbCatalog_Click);
            rbCatalog.LargeImage = Resources.catalog_v2_32;
            rbCatalog.SmallImage = Resources.catalog_v2_32;
            rbCatalog.GroupCaption = grpCatalog;
            rbCatalog.RootKey = kHydroFacetedSearch3;
            head.Add(rbCatalog);
            UpdateCatalogCaption();
            */
            #endregion

            #region Search and download buttons
            
            string grpSearch = "Search";
            var rbSearch = new SimpleActionItem("Initialize Faceted Search", rbSearch_Click);
            rbSearch.LargeImage = Resources.search_32;
            rbSearch.SmallImage = Resources.search_16;
            rbSearch.ToolTipText = "Choose facets based on selected spatial and temporal criteria";
            rbSearch.GroupCaption = grpSearch;
            rbSearch.RootKey = kHydroFacetedSearch3;
            head.Add(rbSearch);
            
            // var cBShowSpatial = new SimpleActionItem("

            /*
            var btnDownload = new SimpleActionItem("Download", rbDownload_Click);
            btnDownload.RootKey = kHydroFacetedSearch3;
            btnDownload.GroupCaption = grpSearch;
            btnDownload.LargeImage = Resources.download_32;
            btnDownload.SmallImage = Resources.download_16;
            App.HeaderControl.Add(btnDownload);
            */
            #endregion

            //map buttons
            AddMapButtons();

            #region Faceted Search Control

            fsc = new FacetedSearchControl(App);
            App.DockManager.Add(new DotSpatial.Controls.Docking.DockablePanel(kHydroFacetedSearch3, "Faceted Search", fsc, DockStyle.Left));

            #endregion
        }
      
        private void AddMapButtons()
        {
            string kHomeRoot = kHydroFacetedSearch3;
            string rpMapTools = "Map Tools";
            string kHydroMapTools = "kHydroMapToolsSearch";
            var head = App.HeaderControl;
            
            //Pan
            var _rbPan = new SimpleActionItem(kHomeRoot, "Pan", rbPan_Click);
            _rbPan.GroupCaption = rpMapTools;
            _rbPan.LargeImage = Properties.Resources.pan;
            _rbPan.SmallImage = Properties.Resources.pan_16;
            _rbPan.ToolTipText = "Pan - Move the Map";
            _rbPan.ToggleGroupKey = kHydroMapTools;
            head.Add(_rbPan);

            //ZoomIn
            var _rbZoomIn = new SimpleActionItem(kHomeRoot, "Zoom In", rbZoomIn_Click);
            _rbZoomIn.ToolTipText = "Zoom In";
            _rbZoomIn.GroupCaption = rpMapTools;
            _rbZoomIn.LargeImage = Properties.Resources.zoom_in;
            _rbZoomIn.SmallImage = Properties.Resources.zoom_in_16;
            _rbZoomIn.ToggleGroupKey = kHydroMapTools;
            head.Add(_rbZoomIn);

            //ZoomOut
            var _rbZoomOut = new SimpleActionItem(kHomeRoot, "Zoom Out", rbZoomOut_Click);
            _rbZoomOut.ToolTipText = "Zoom Out";
            _rbZoomOut.GroupCaption = rpMapTools;
            _rbZoomOut.LargeImage = Properties.Resources.zoom_out;
            _rbZoomOut.SmallImage = Properties.Resources.zoom_out_16;
            _rbZoomOut.ToggleGroupKey = kHydroMapTools;
            head.Add(_rbZoomOut);

            //ZoomToFullExtent
            var _rbMaxExtents = new SimpleActionItem(kHomeRoot, "MaxExtents", rbFullExtent_Click);
            _rbMaxExtents.ToolTipText = "Maximum Extents";
            _rbMaxExtents.GroupCaption = rpMapTools;
            _rbMaxExtents.LargeImage = Properties.Resources.full_extent;
            _rbMaxExtents.SmallImage = Properties.Resources.full_extent_16;
            head.Add(_rbMaxExtents);

            


            ////ZoomToPrevious
            //_rbZoomToPrevious = new SimpleActionItem(kHomeRoot, "Previous", rbZoomToPrevious_Click);
            //_rbZoomToPrevious.ToolTipText = "Go To Previous Map Extent";
            //_rbZoomToPrevious.GroupCaption = rpMapTools;
            //_rbZoomToPrevious.LargeImage = Properties.Resources.zoom_to_previous;
            //_rbZoomToPrevious.SmallImage = Properties.Resources.full_extent_16;
            //applicationManager1.HeaderControl.Add(_rbZoomToPrevious);

            //if (_previousExtents.Count == 0)
            //    _rbZoomToPrevious.Enabled = false;

            ////ZoomToNext
            //_rbZoomToNext = new SimpleActionItem(kHomeRoot, "Next", rbZoomToNext_Click);
            //_rbZoomToNext.ToolTipText = "Go To Next Map Extent";
            //_rbZoomToNext.GroupCaption = rpMapTools;
            //_rbZoomToNext.LargeImage = Properties.Resources.zoom_to_next;
            //_rbZoomToNext.SmallImage = Properties.Resources.zoom_to_next_16;
            //applicationManager1.HeaderControl.Add(_rbZoomToNext);

            //if ((_mCurrentExtents < _previousExtents.Count - 1) != true)
            //    _rbZoomToNext.Enabled = false;

            //_rbZoomToNext.Click += new EventHandler(rbZoomToNext_Click);

            ////Separator
            //var mapTools = new SeparatorItem();
            //mapTools.GroupCaption = rpMapTools;
            //mapTools.RootKey = kHomeRoot;
            //head.Add(mapTools);

            ////Add
            //_rbAdd = new SimpleActionItem(kHomeRoot, "Add", rbAdd_Click);
            //_rbAdd.ToolTipText = "Add Layer To The Map";
            //_rbAdd.GroupCaption = rpMapTools;
            //_rbAdd.LargeImage = Properties.Resources.add;
            //_rbAdd.SmallImage = Properties.Resources.add_16;
            //applicationManager1.HeaderControl.Add(_rbAdd);

            ////Identifier
            //_rbIdentifier = new SimpleActionItem(kHomeRoot, "Identify", rbIdentifier_Click);
            //_rbIdentifier.ToolTipText = "Identify";
            //_rbIdentifier.SmallImage = Properties.Resources.identifier_16;
            //_rbIdentifier.GroupCaption = rpMapTools;
            //_rbIdentifier.LargeImage = Properties.Resources.identifier;
            //_rbIdentifier.ToggleGroupKey = kHydroMapTools;
            //applicationManager1.HeaderControl.Add(_rbIdentifier);

            

            ////AttributeTable
            //var _rbAttribute = new SimpleActionItem(kHomeRoot, "Attribute", rbAttribute_Click);
            //_rbAttribute.ToolTipText = "Attribute Table";
            //_rbAttribute.GroupCaption = rpMapTools;
            //_rbAttribute.LargeImage = Properties.Resources.attribute_table;
            //_rbAttribute.SmallImage = Properties.Resources.attribute_table_16;
            //head.Add(_rbAttribute);
        }
        
        void rbPan_Click(object sender, EventArgs e) { }
        void rbZoomIn_Click(object sender, EventArgs e) { }
        void rbZoomOut_Click(object sender, EventArgs e) { }
        void rbFullExtent_Click(object sender, EventArgs e) { }
        void rbDownload_Click(object sender, EventArgs e) { }

        #region Search
        
        void rbSearch_Click(object sender, EventArgs e)
        {            
            //there must be at least one layer in the map
            if (App.Map.Layers.Count == 0)
            {
                MessageBox.Show("Please add at least one layer to the map.");
                return;
            }
            
            //reproject the area parameter to wgs84 and then run search.
            if (SearchSettings.Instance.AreaSettings.Polygons != null)
            {
                Extent extentInWgs84 = AreaHelper.ReprojectExtentToWGS84(SearchSettings.Instance.AreaSettings.Polygons.Extent, App.Map.Projection);
                fsc.SetSearchParameters(extentInWgs84, SearchSettings.Instance.DateSettings.StartDate, SearchSettings.Instance.DateSettings.EndDate);
                fsc.InitializeFacetedSearch();
            }
            else if (SearchSettings.Instance.AreaSettings.AreaRectangle != null)
            {
                Extent areaRectangleExtent = new Extent(SearchSettings.Instance.AreaSettings.AreaRectangle.XMin, SearchSettings.Instance.AreaSettings.AreaRectangle.YMin, SearchSettings.Instance.AreaSettings.AreaRectangle.XMax, SearchSettings.Instance.AreaSettings.AreaRectangle.YMax);
                Extent areaRectangelExtentInWgs84 = AreaHelper.ReprojectExtentToWGS84(areaRectangleExtent, App.Map.Projection);
                
                fsc.SetSearchParameters(areaRectangelExtentInWgs84, SearchSettings.Instance.DateSettings.StartDate, SearchSettings.Instance.DateSettings.EndDate);
                fsc.InitializeFacetedSearch();    
            }
            else
            {
                MessageBox.Show("Please specify spatial and temporal constraints prior to initializing faceted search.");
            }                 
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
            get;
            set;
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
                _rectangleDrawing = new RectangleDrawing((Map)App.Map);
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

            string isWorldTemplate = App.SerializationManager.GetCustomSetting<string>("world_template", "false");
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
                var polygonLayer = AreaHelper.GetAllSelectedPolygonLayers((Map)App.Map).FirstOrDefault();
                if (polygonLayer == null)
                {
                    //special case: if the map layers or the group is selected
                    if (App.Map.MapFrame.IsSelected)
                    {
                        IEnumerable<IMapPolygonLayer> polygonLayers = AreaHelper.GetAllPolygonLayers((Map)App.Map).Reverse();
                        foreach (IMapPolygonLayer polyLayer in polygonLayers)
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
        /*
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
        */

        #endregion

        #region WebServices
        /*
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

            // rbServices.Caption = caption;
            // rbServices.ToolTipText = string.Format("Select web services ({0} selected)", hint);
            ChangeWebServicesIcon(webServiceNode);
        }

        private void ChangeWebServicesIcon(WebServiceNode webServiceNode = null)
        {
            if (webServiceNode == null || 
                string.IsNullOrEmpty(webServiceNode.ServiceCode))
            {
                // rbServices.LargeImage = Resources.web_services_v1_32;
                // rbServices.SmallImage = Resources.web_services_v1_16;
                return;
            }

            try
            {
                var imageHelper = new ServiceIconHelper(SearchSettings.Instance.CatalogSettings.HISCentralUrl);
                var image = imageHelper.GetImageForService(webServiceNode.ServiceCode);
                // rbServices.LargeImage = rbServices.SmallImage = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to change icon." + Environment.NewLine +
                                "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
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
        /*
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
            // rbCatalog.Caption = SearchSettings.Instance.CatalogSettings.TypeOfCatalog.Description();
        }
        */
        #endregion

        #endregion
    }
}
