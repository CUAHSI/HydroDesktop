using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DotSpatial.Data;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using HydroDesktop.Interfaces;
using HydroDesktop.Configuration;


namespace HydroDesktop.Search
{
    [Plugin("Search V2", Author = "ISU", UniqueName = "mw_Search_2", Version = "2")]
    public class Main : Extension, IMapPlugin
    {
        #region Variables

        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;

        //a menu item added by the plugin
        private ToolStripMenuItem mnuSearch = null;

        //the Search split container
        private SplitContainer spcSearch = null;

        //the Search user control
        private SearchControl ucSearch = null;

        //controls the width of search panel control
        const int SEARCH_PANEL_WIDTH = 350;

        Extent _initialExtent = null; 

        #endregion

        #region Ribbon Variables
        //buttons and panels for the Ribbon
        private RibbonButton _searchRibbonButton = null;
        private RibbonButton _rbtnHISCentral = null;
        private RibbonButton _rbtnMetadataCache = null;
        private RibbonButton _rbtnAdvancedSettings = null;
        private RibbonPanel _searchRibbonPanel = null;

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            //Remove the Search Ribbon Items
            RemoveRibbonItems();
            
            if (_mapArgs.MainMenu != null && mnuSearch != null)
            {
                _mapArgs.MainMenu.Items.Remove(mnuSearch);
            }

            //remove the split container
            try
            {
                spcSearch.Panel2Collapsed = true;
                spcSearch.Panel2.Controls.Clear();

                Control mapControl = _mapArgs.Map as Control;
                if (mapControl != null)
                {
                    Control mapParentControl = mapControl.Parent.Parent;

                    //temporarily remove the map control
                    mapParentControl.Controls.Remove(mapControl);

                    Control newMapParentControl = mapParentControl.Parent;
                    newMapParentControl.Controls.Remove(mapParentControl);
                    newMapParentControl.Controls.Add(mapControl);

                    //to refresh the map
                    RefreshTheMap();
                }   
            }
            catch { }

            //necessary for plugin deactivation
            base.OnDeactivate();
        }

        #endregion

        #region IPlugin Members

        /// <summary>
        /// Initialize the plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public void Initialize(IMapPluginArgs args)
        {
            _mapArgs = args;

            _initialExtent = (Extent)_mapArgs.Map.ViewExtents.Clone();

            //add the ribbon panels and buttons
            AddRibbonItems();
            
            //to add the menu with one submenu item
            mnuSearch = new ToolStripMenuItem("Search (V2)");
            ToolStripMenuItem mnuHISCentral = new ToolStripMenuItem("HIS Central", Resources.OpenSearch);
            ToolStripMenuItem mnuMetadataCache = new ToolStripMenuItem("Metadata Cache", Resources.OpenSearch_1);
            mnuSearch.DropDownItems.Add(mnuHISCentral);
            mnuSearch.DropDownItems.Add(mnuMetadataCache);

            //check if the application has a menu bar
            if (_mapArgs.MainMenu != null)
            {
                //add the 'Search' menu
                _mapArgs.MainMenu.Items.Add(mnuSearch);

                // Subscribe the button click events
                mnuHISCentral.Click += new EventHandler(mnuHISCentral_Click);
                mnuMetadataCache.Click += new EventHandler(mnuMetadataCache_Click);
            }

            //add the search UI to the main application
            if (_mapArgs.Map != null)
            {
                //report progress
                ReportProgress(0,"Loading Search Plugin..");
                
                Control mapControl = _mapArgs.Map as Control;
                if (mapControl != null)
                {
                    Control mapParentControl = mapControl.Parent;

                    //temporarily remove the map control
                    mapParentControl.Controls.Remove(mapControl);

                    //create a split container
                    spcSearch = new SplitContainer();
                    spcSearch.Name = "spcSearch";
                    spcSearch.Dock = DockStyle.Fill;
                    spcSearch.Width = mapParentControl.Width;

                    //spcSearch.SplitterDistance = spcSearch.Width - SEARCH_PANEL_WIDTH;
                    spcSearch.FixedPanel = FixedPanel.Panel2;
                    spcSearch.IsSplitterFixed = false;
                    spcSearch.Panel2MinSize = SEARCH_PANEL_WIDTH - 10;
                    spcSearch.SplitterDistance = spcSearch.Width - SEARCH_PANEL_WIDTH;
                    
                    spcSearch.Panel2Collapsed = true;

                    //move map to Panel1 of the split container
                    spcSearch.Panel1.Controls.Add(mapControl);

                    //Set the Search control
                    ucSearch = new SearchControl(_mapArgs, spcSearch);
                    ucSearch.Dock = DockStyle.Fill;

                    //trial change to some other figure (for reducing the size of xml search summary)//
                    int summaryHeight = 170;//210;// 177;
                    ucSearch.spcHor1.SplitterDistance = ucSearch.spcHor1.Height - summaryHeight;
                    ucSearch.spcHor1.FixedPanel = FixedPanel.Panel2;
                    ucSearch.spcHor1.Panel2MinSize = summaryHeight;

                    spcSearch.Panel2.Controls.Add(ucSearch);

                    //add the split container in the place of the map

                    mapParentControl.Controls.Add(spcSearch);

                    //horizontal splitter
                    //ucSearch.spcHor1.SplitterDistance = ucSearch.spcHor1.Height - ucSearch.spcHor1.Panel2.Height;

                    ReportProgress(50, "Loading Search Plugin..");

                    //execute the HIS CentralClick event 
                    mnuHISCentral_Click(null, null);
                    _mapArgs.Map.ViewExtents = _initialExtent;
                }
            }
            
            //to test the Attribute table click event
            //Legend leg = _mapArgs.Legend as Legend;
            //leg.
        }

        void _searchRibbonButton_Click(object sender, EventArgs e)
        {
            spcSearch.Panel2Collapsed = false;
            //ucSearch.MainImage = Resources.OpenSearch;
            ucSearch.SearchMode = "HIS Central";
            RefreshTheMap();
        }

        //click - HIS Central Search button
        void mnuMetadataCache_Click(object sender, EventArgs e)
        {
            spcSearch.Panel2Collapsed = false;
            //ucSearch.MainImage = Resources.OpenSearch_1;
            ucSearch.SearchMode = "Local Metadata Cache";
            RefreshTheMap();
        }

        //click - Metadata Cache Search button
        void mnuHISCentral_Click(object sender, EventArgs e)
        {
            ReportProgress(70, "Loading Search Plugin");
            
            spcSearch.Panel2Collapsed = false;
            //ucSearch.MainImage = Resources.OpenSearch;
            ucSearch.SearchMode = "HIS Central";
            RefreshTheMap();
        }

        #endregion

        #region Progress
        void ReportProgress(int percent, string message)
        {
            if (_mapArgs.ProgressHandler != null)
            {
                _mapArgs.ProgressHandler.Progress(message, percent, message);
            }
        }
        #endregion

        #region Ribbon
        //to add the ribbon panel and ribbon buttons
        void AddRibbonItems()
        {
            //check for the Ribbon
            if (_mapArgs.Ribbon != null)
            {
                //Search Ribbon Button
                _searchRibbonButton = new RibbonButton("Search");
                //_mapArgs.Ribbon.Tabs[0].Panels[0].Items.Add(_searchRibbonButton);
                RibbonPanel rpSearch = new RibbonPanel();
				rpSearch.ButtonMoreVisible = false;
                _mapArgs.Ribbon.Tabs[0].Panels.Insert(0, rpSearch);
                _mapArgs.Ribbon.Tabs[0].Panels[0].Items.Insert(0, _searchRibbonButton);
                _searchRibbonButton.Image = Resources.search2_4;

                //Add the HIS Central and Metadata Cache dropdown items
                _rbtnHISCentral = new RibbonButton("HIS Central");
                _rbtnMetadataCache = new RibbonButton("Metadata Cache");
                _rbtnAdvancedSettings = new RibbonButton("Advanced Settings");

                _searchRibbonButton.DropDownItems.Add(_rbtnHISCentral);
                _searchRibbonButton.DropDownItems.Add(_rbtnMetadataCache);
                _searchRibbonButton.DropDownItems.Add(_rbtnAdvancedSettings);

                _rbtnHISCentral.SmallImage = Resources.OpenSearch;
                _rbtnMetadataCache.SmallImage = Resources.OpenSearch_1;
                _rbtnAdvancedSettings.SmallImage = Resources.OpenSearch;

                //set the style of search button to dropDown
                _searchRibbonButton.Style = RibbonButtonStyle.DropDown;

                //attach the click events
                _searchRibbonButton.Click +=new EventHandler(_searchRibbonButton_Click);
                _rbtnHISCentral.Click += new EventHandler(rbtnHISCentral_Click);
                _rbtnMetadataCache.Click += new EventHandler(rbtnMetadataCache_Click);
                _rbtnAdvancedSettings.Click += new EventHandler(_rbtnAdvancedSettings_Click);

                //to add the active changed event
                RibbonTab mapRibbonTab = _mapArgs.Ribbon.Tabs[1];
                if (mapRibbonTab != null)
                {
                    //mapRibbonTab.ActiveChanged += new EventHandler(mapRibbonTab_ActiveChanged);
                }
                RibbonTab searchRibbonTab = _mapArgs.Ribbon.Tabs[0];
                if (searchRibbonTab != null)
                {
                    searchRibbonTab.ActiveChanged += new EventHandler(searchRibbonTab_ActiveChanged);
                }
            }
        }


        //When user activates the "Search" Ribbon Tab
        void searchRibbonTab_ActiveChanged(object sender, EventArgs e)
        {
            if (_mapArgs.Ribbon.Tabs[0].Active == true)
            {
                //commented out by Jiri Kadlec - to fix bug #6795.
                //the visibility of the search panel needs to be maintained
                //when switching tabs.
                //spcSearch.Panel2Collapsed = false;
                _mapArgs.Map.MapFrame.ResetExtents();
            }
        }

        ////When user activates the "Map" Ribbon Tab
        //void mapRibbonTab_ActiveChanged(object sender, EventArgs e)
        //{
        //    if (_mapArgs.Ribbon.Tabs[1].Active == true)
        //    {
        //        spcSearch.Panel2Collapsed = true;
        //        _mapArgs.Map.MapFrame.ResetExtents();
        //    }
        //}

        //metadata cache ribbon dropdown click event
        void rbtnMetadataCache_Click(object sender, EventArgs e)
        {
            spcSearch.Panel2Collapsed = false;
            //ucSearch.MainImage = Resources.OpenSearch_1;
            ucSearch.SearchMode = "Local Metadata Cache";
            ucSearch.lblServerValue.Text = ucSearch.Label3.Text;
            //moved to search control load
            ucSearch.dateTimePickStart.Value = DateTime.Now.Date.AddYears(-100);//change suggested by Dan (range 100 years)
            ucSearch.dateTimePickEnd.Value = DateTime.Now.Date;
            ucSearch.groupBox3.Visible = false;
            RefreshTheMap();
        }
        //his central ribbon dropdown click event
        void rbtnHISCentral_Click(object sender, EventArgs e)
        {
            spcSearch.Panel2Collapsed = false;
            //ucSearch.MainImage = Resources.OpenSearch;
            ucSearch.SearchMode = "HIS Central";
            ucSearch.lblServerValue.Text = ucSearch.Label3.Text;
            //moved to search control load
            ucSearch.dateTimePickStart.Value = DateTime.Now.Date.AddYears(-100);//change suggested by Dan (range 100 years)
            ucSearch.dateTimePickEnd.Value = DateTime.Now.Date;
            ucSearch.groupBox3.Visible = true;
            RefreshTheMap();
        }

        //advanced options
        void _rbtnAdvancedSettings_Click(object sender, EventArgs e)
        {
            _searchRibbonButton.CloseDropDown();
            
            //to set the advanced option
           AdvancedSettingsDialog dialog = new AdvancedSettingsDialog(ucSearch);

            dialog.ShowDialog();
        }


        //to remove the ribbon panel and ribbon buttons
        void RemoveRibbonItems()
        {
            if (_mapArgs.Ribbon != null)
            {
                _mapArgs.Ribbon.Tabs[0].Panels[0].Items.Remove(_searchRibbonButton);
                _mapArgs.Ribbon.Tabs[0].Panels.Remove(_mapArgs.Ribbon.Tabs[0].Panels[0]);
            }

            if (_mapArgs.Ribbon.Tabs.Count > 1 && _searchRibbonPanel != null)
            {
                _mapArgs.Ribbon.Tabs[1].Panels.Remove(_searchRibbonPanel);
            }
        }

        //force  the map to redraw
        void RefreshTheMap()
        {
            if (_mapArgs.PanelManager != null)
            {
                if (_mapArgs.PanelManager.SelectedTabName != "MapView")
                {
                    _mapArgs.PanelManager.SelectedTabName = "MapView";
                }
            }
            
            _mapArgs.Map.MapFrame.ResetExtents();
            _mapArgs.Map.Invalidate();
            //_mapArgs.Map.ViewExtents = _initialExtent;
        }


        #endregion
    }
}
