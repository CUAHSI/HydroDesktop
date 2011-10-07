using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using Search3.Area;
using Search3.Extensions;
using Search3.Properties;
using Search3.Settings.UI;

namespace Search3
{
    public class SearchPlugin: Extension
    {
        //constants
        //root key
        const string kHydroSearch3 = "kHydroSearchV3";

        private SimpleActionItem rbServices;
        private SimpleActionItem rbCatalog;
        private TextEntryActionItem rbStartDate;
        private TextEntryActionItem rbEndDate;
        private RectangleDrawing _rectangleDrawing;

        public override void Activate()
        {
            AddSearchRibbon();
            base.Activate(); 
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            App.DockManager.Remove("kFacetedSearch");
            
            base.Deactivate();
        }

        public void AddSearchRibbon()
        {
            var head = App.HeaderControl;
            
            //Search ribbon tab
            var root = new RootItem(kHydroSearch3, "Search");
            root.SortOrder = -100;
            head.Add(root);

            #region Area group

            const string grpArea = "Area";

            //Draw Box
            var rbDrawBox = new SimpleActionItem("Draw Box", rbDrawBox_Click);
            rbDrawBox.LargeImage = Resources.draw_box_32_a;
            rbDrawBox.SmallImage = Resources.draw_box_16_a;
            rbDrawBox.GroupCaption = grpArea;
            rbDrawBox.ToggleGroupKey = grpArea;
            rbDrawBox.RootKey = kHydroSearch3;
            head.Add(rbDrawBox);

            //Select
            var rbSelect = new SimpleActionItem(kHydroSearch3, "Select Polygons", rbSelect_Click);
            rbSelect.ToolTipText = "Select Region";
            rbSelect.GroupCaption = grpArea;
            rbSelect.LargeImage = Resources.select;
            rbSelect.SmallImage = Resources.select_16;
            rbSelect.ToggleGroupKey = grpArea;
            head.Add(rbSelect);

            //AttributeTable
            var rbAttribute = new SimpleActionItem(kHydroSearch3, "Select by Attribute", rbAttribute_Click);
            rbAttribute.ToolTipText = "Select by Attribute";
            rbAttribute.GroupCaption = grpArea;
            rbAttribute.LargeImage = Resources.attribute_table;
            rbAttribute.SmallImage = Resources.attribute_table_16;
            head.Add(rbAttribute);

            #endregion


            //do not implement these for now - use attribute table selection instead

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

            #region Keyword Group

            //Keyword text entry
            const string grpKeyword = "Keyword";
            var rbKeyword = new TextEntryActionItem();
            rbKeyword.PropertyChanged += rbKeyword_PropertyChanged;
            rbKeyword.GroupCaption = grpKeyword;
            rbKeyword.RootKey = kHydroSearch3;
            rbKeyword.Width = 150;
            head.Add(rbKeyword);
            rbKeyword.Text = "Type-in a Keyword";
            UpdateKeywordsCaption();

            //Keyword more options
            var rbKeyword2 = new SimpleActionItem("Keyword Selection", rbKeyword_Click);       
            rbKeyword2.LargeImage = Resources.keyword_v2_32;
            rbKeyword2.SmallImage = Resources.keyword_v2_16;
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

            var rbDate = new SimpleActionItem("Select Dates", rbDate_Click);
            rbDate.GroupCaption = grpDates;
            rbDate.RootKey = kHydroSearch3;
            rbDate.LargeImage = Resources.select_date_v1_32;
            rbDate.SmallImage = Resources.select_date_v1_16;
            head.Add(rbDate);

            #endregion

            #region Web Services group

            const string grpServices = "Web Services";
            rbServices = new SimpleActionItem("All Services", rbServices_Click);
            rbServices.LargeImage = Resources.web_services_v1_32;
            rbServices.SmallImage = Resources.web_services_v1_16;
            rbServices.ToolTipText = "Select web services (All Services selected)";
            rbServices.GroupCaption = grpServices;
            rbServices.RootKey = kHydroSearch3;
            head.Add(rbServices);
            UpdateWebServicesCaption();

            #endregion

            #region Catalog group

            const string grpCatalog = "Catalog";
            rbCatalog = new SimpleActionItem("HIS Central", rbCatalog_Click);
            rbCatalog.LargeImage = Resources.catalog_v2_32;
            rbCatalog.SmallImage = Resources.catalog_v2_32;
            rbCatalog.GroupCaption = grpCatalog;
            rbCatalog.RootKey = kHydroSearch3;
            head.Add(rbCatalog);
            UpdateCatalogCaption();

            #endregion

            //search and download buttons
            string grpSearch = "Search";
            var rbSearch = new SimpleActionItem("Run Search", rbSearch_Click);
            rbSearch.LargeImage = Resources.search2_3;
            rbSearch.SmallImage = Resources.search2_3;
            rbSearch.ToolTipText = "Run Search based on selected criteria";
            rbSearch.GroupCaption = grpSearch;
            rbSearch.RootKey = kHydroSearch3;
            head.Add(rbSearch);

            var btnDownload = new SimpleActionItem("Download", rbDownload_Click)
            {
                RootKey = kHydroSearch3,
                GroupCaption = grpSearch,
                LargeImage = Resources.download32
            };
            App.HeaderControl.Add(btnDownload);

            //map buttons
            AddMapButtons();
        }
      
        void AddMapButtons()
        {
            string kHomeRoot = kHydroSearch3;
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

        #region event handlers
        
        void rbPan_Click(object sender, EventArgs e) { }
        void rbZoomIn_Click(object sender, EventArgs e) { }
        void rbZoomOut_Click(object sender, EventArgs e) { }
        void rbFullExtent_Click(object sender, EventArgs e) { }
        void rbSearch_Click(object sender, EventArgs e) { }
        void rbDownload_Click(object sender, EventArgs e) { }

        #region  Area group

        void rbDrawBox_Click(object Sender, EventArgs e)
        {
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
            var wgs84 = new ProjectionInfo();
            wgs84.ReadEsriString(esri);

            Reproject.ReprojectPoints(xy, new double[] { 0, 0 }, App.Map.Projection, wgs84, 0, 2);

            //todo: save/show selected rectangle coords
            /*listBox4.Items.Clear();
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[0]));
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[1]));
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[2]));
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[3]));
             */ 
        }

        void rbSelect_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.Select;
        }

        void rbAttribute_Click(object sender, EventArgs e)
        {
            var featureLayerIsSelected = false;

            foreach (var lay in ((Map)App.Map).GetAllLayers())
            {
                var ori_fl = lay as IMapFeatureLayer;
                if (ori_fl == null) continue;
                if (ori_fl.IsSelected)
                {
                    featureLayerIsSelected = true;
                    App.Map.FunctionMode = FunctionMode.Select;
                    ori_fl.ShowAttributes();
                }
            }
            //if no layer is selected, inform the user
            if (!featureLayerIsSelected)
            {
                MessageBox.Show("Please select a layer in the legend.", "Information", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Keywords

        private void UpdateKeywordsCaption()
        {
            //todo: Implememnt me
        }

        void rbKeyword_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;
            //todo: implement tooltip with keyword mathcing
        }

        void rbKeyword_Click(object Sender, EventArgs e)
        {
            KeywordsDialog.ShowDialog(Settings.PluginSettings.Instance.KeywordsSettings);
            UpdateKeywordsCaption();
        }

        #endregion

        #region WebServices

        void rbServices_Click(object Sender, EventArgs e)
        {
            WebServicesDialog.ShowDialog(Settings.PluginSettings.Instance.WebServicesSettings);
            UpdateWebServicesCaption();
        }

        private void UpdateWebServicesCaption()
        {
            var webWServicesSettings = Settings.PluginSettings.Instance.WebServicesSettings;
            var checkedCount = webWServicesSettings.CheckedCount;
            var totalCount = webWServicesSettings.TotalCount;

            string caption;
            if (checkedCount == totalCount)
            {
                caption = "All services";
            }else if (checkedCount == 1)
            {
                // Get single checked item
                var items = webWServicesSettings.WebServices.Where((w) => w.Checked).ToList();
                Debug.Assert(items.Count == 1);
                caption = items[0].Title;
                // todo: Change button icon
            }
            else
            {
                caption = string.Format("{0} services selected", checkedCount);
            }

            rbServices.Caption = caption;
            // todo: Uncomment next line when will be fixed http://dotspatial.codeplex.com/workitem/351
            // rbServices.ToolTipText = string.Format("Select web services ({0} selected)", caption);
        }

        #endregion

        #region Dates

        private void UpdateDatesCaption()
        {
            rbStartDate.Text = Settings.PluginSettings.Instance.DateSettings.StartDate.ToShortDateString();
            rbEndDate.Text = Settings.PluginSettings.Instance.DateSettings.EndDate.ToShortDateString();
        }

        void rbDate_Click(object Sender, EventArgs e)
        {
            DateSettingsDialog.ShowDialog(Settings.PluginSettings.Instance.DateSettings);
            UpdateDatesCaption();
        }

        void rbEndDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;

            DateTime result;
            if (DateTime.TryParse(rbEndDate.Text, out result))
                Settings.PluginSettings.Instance.DateSettings.EndDate = result;
        }

        void rbStartDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Text") return;

            DateTime result;
            if (DateTime.TryParse(rbStartDate.Text, out result))
                Settings.PluginSettings.Instance.DateSettings.StartDate = result;
        }

        #endregion

        #region Catalog

        void rbCatalog_Click(object Sender, EventArgs e)
        {
            SearchCatalogSettingsDialog.ShowDialog(Settings.PluginSettings.Instance.CatalogSettings);
            UpdateCatalogCaption();
        }

        private void UpdateCatalogCaption()
        {
            rbCatalog.Caption = Settings.PluginSettings.Instance.CatalogSettings.TypeOfCatalog.Description();
        }

        #endregion

        #endregion
    }
}
