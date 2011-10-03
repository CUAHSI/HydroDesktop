using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
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
            RootItem root = new RootItem(kHydroSearch3, "Search");
            root.SortOrder = -100;
            head.Add(root);

            //Area group
            string grpArea = "Area";

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
            rbSelect.LargeImage = Properties.Resources.select;
            rbSelect.SmallImage = Properties.Resources.select_16;
            rbSelect.ToggleGroupKey = grpArea;
            head.Add(rbSelect);

            //AttributeTable
            var rbAttribute = new SimpleActionItem(kHydroSearch3, "Select by Attribute", rbAttribute_Click);
            rbAttribute.ToolTipText = "Select by Attribute";
            rbAttribute.GroupCaption = grpArea;
            rbAttribute.LargeImage = Properties.Resources.attribute_table;
            rbAttribute.SmallImage = Properties.Resources.attribute_table_16;
            head.Add(rbAttribute);

            

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

            

            //Keyword Group
            //Keyword text entry
            string grpKeyword = "Keyword";
            var rbKeyword = new TextEntryActionItem();
            rbKeyword.GroupCaption = grpKeyword;
            
            rbKeyword.RootKey = kHydroSearch3;
            rbKeyword.Width = 150;
            head.Add(rbKeyword);
            rbKeyword.Text = "Type-in a Keyword";

            //Keyword more options
            var rbKeyword2 = new SimpleActionItem("Keyword Selection", rbKeyword_Click);       
            rbKeyword2.LargeImage = Resources.keyword_v2_32;
            rbKeyword2.SmallImage = Resources.keyword_v2_16;
            rbKeyword2.GroupCaption = grpKeyword;
            rbKeyword2.ToolTipText = "Show Keyword Ontology Tree";
            rbKeyword2.RootKey = kHydroSearch3;
            head.Add(rbKeyword2);

            //Dates group
            string grpDates = "Time Range";
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

            //Web Services group
            string grpServices = "Web Services";
            rbServices = new SimpleActionItem("All Services", rbServices_Click);
            rbServices.LargeImage = Resources.web_services_v1_32;
            rbServices.SmallImage = Resources.web_services_v1_16;
            rbServices.ToolTipText = "Select web services (All Services selected)";
            rbServices.GroupCaption = grpServices;
            rbServices.RootKey = kHydroSearch3;
            head.Add(rbServices);

            //Catalog group
            string grpCatalog = "Catalog";
            rbCatalog = new SimpleActionItem("HIS Central", rbCatalog_Click);
            rbCatalog.LargeImage = Resources.catalog_v2_32;
            rbCatalog.SmallImage = Resources.catalog_v2_32;
            rbCatalog.ToolTipText = "Select web services (All Services selected)";
            rbCatalog.GroupCaption = grpCatalog;
            rbCatalog.RootKey = kHydroSearch3;
            head.Add(rbCatalog);
            UpdateCatalogCaption();

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
                LargeImage = Properties.Resources.download32
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
        void rbSelect_Click(object sender, EventArgs e) { }
        void rbAttribute_Click(object sender, EventArgs e) { }
        
        void rbSearch_Click(object sender, EventArgs e) { }
        void rbDownload_Click(object sender, EventArgs e) { }


        void rbDrawBox_Click(object Sender, EventArgs e)
        {

        }
        void rbKeyword_Click(object Sender, EventArgs e)
        {

        }
        void rbServices_Click(object Sender, EventArgs e)
        {
            rbServices.Caption = "Little Bear River..";
        }



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
    }
}
