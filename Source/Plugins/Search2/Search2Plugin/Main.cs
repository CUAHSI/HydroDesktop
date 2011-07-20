using System;
using System.ComponentModel;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using HydroDesktop.Search.Extensions;
using System.Collections.Generic;

namespace HydroDesktop.Search
{
    [Plugin("Search V2", Author = "ISU", UniqueName = "mw_Search_2", Version = "2")]
    public class Main : Extension, IMapPlugin
    {

        #region Variables

        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;

        //the Search user control
        private SearchControl ucSearch = null;

        Extent _initialExtent = null;

        #endregion Variables

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            //Remove the Search Ribbon Items
            _mapArgs.AppManager.HeaderControl.RemoveItems();

            //to refresh the map
            RefreshTheMap();

            //necessary for plugin deactivation
            base.OnDeactivate();
        }

        #endregion IExtension Members

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

            var mnuHISCentral = new ToolStripMenuItem("HIS Central", Resources.OpenSearch);
            var mnuMetadataCache = new ToolStripMenuItem("Metadata Cache", Resources.OpenSearch_1);
            //add the search UI to the main application
            if (_mapArgs.Map != null)
            {
                //report progress
                ReportProgress(0, "Loading Search Plugin..");

                //Set the Search control
                ucSearch = new SearchControl(_mapArgs);
                ucSearch.Dock = DockStyle.Fill;
                _mapArgs.AppManager.DockManager.Add("SearchControl", ucSearch, DockStyle.Right);

                ReportProgress(50, "Loading Search Plugin..");

                //execute the HIS CentralClick event
                mnuHISCentral_Click(null, null);
                _mapArgs.Map.ViewExtents = _initialExtent;

                ReportProgress(0, String.Empty);

                //handle project saving event
                _mapArgs.AppManager.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);
            }
        }

        

        private void _searchRibbonButton_Click(object sender, EventArgs e)
        {
            ucSearch.SearchMode = SearchMode.HISCentral;
            RefreshTheMap();
        }

        //click - HIS Central Search button
        private void mnuMetadataCache_Click(object sender, EventArgs e)
        {
            ucSearch.SearchMode = SearchMode.LocalMetaDataCache;
            RefreshTheMap();
        }

        //click - Metadata Cache Search button
        private void mnuHISCentral_Click(object sender, EventArgs e)
        {
            ReportProgress(70, "Loading Search Plugin");

            ucSearch.SearchMode = SearchMode.HISCentral;
            RefreshTheMap();
        }

        #endregion IPlugin Members

        #region Progress

        private void ReportProgress(int percent, string message)
        {
            if (_mapArgs.ProgressHandler != null)
            {
                _mapArgs.ProgressHandler.Progress(message, percent, message);
            }
        }

        #endregion Progress

        #region Ribbon

        //to add the ribbon panel and ribbon buttons
        private void AddRibbonItems()
        {
            if (_mapArgs.AppManager.HeaderControl != null)
            {
                var header = _mapArgs.AppManager.HeaderControl;

                const string KHydroSearch = "kHome";
                const string SearchMenuKey = "kSearch";

                header.Add(new MenuContainerItem(KHydroSearch, SearchMenuKey, "Search") { GroupCaption = "Search", LargeImage = Resources.search2_4 });
                header.Add(new SimpleActionItem(KHydroSearch, SearchMenuKey, "HIS Central", rbtnHISCentral_Click) { GroupCaption = "Search", SmallImage = Resources.OpenSearch });
                header.Add(new SimpleActionItem(KHydroSearch, SearchMenuKey, "Metadata Cache", rbtnMetadataCache_Click) { GroupCaption = "Search", SmallImage = Resources.OpenSearch_1 });
                header.Add(new SimpleActionItem(KHydroSearch, SearchMenuKey, "Advanced Settings", _rbtnAdvancedSettings_Click) { GroupCaption = "Search" });
            }

            //RibbonTab searchRibbonTab = _mapArgs.Ribbon.Tabs[0];
            //if (searchRibbonTab != null)
            //{
            //    searchRibbonTab.ActiveChanged += new EventHandler(searchRibbonTab_ActiveChanged);
            //}
        }

        //metadata cache ribbon dropdown click event
        private void rbtnMetadataCache_Click(object sender, EventArgs e)
        {
            //ucSearch.MainImage = Resources.OpenSearch_1;
            ucSearch.SearchMode = SearchMode.LocalMetaDataCache;
            ucSearch.lblServerValue.Text = ucSearch.SearchMode.Description();
            //moved to search control load
            ucSearch.dateTimePickStart.Value = DateTime.Now.Date.AddYears(-100);//change suggested by Dan (range 100 years)
            ucSearch.dateTimePickEnd.Value = DateTime.Now.Date;
            ucSearch.groupBox3.Visible = false;
            RefreshTheMap();
        }

        //his central ribbon dropdown click event
        private void rbtnHISCentral_Click(object sender, EventArgs e)
        {
            //ucSearch.MainImage = Resources.OpenSearch;
            ucSearch.SearchMode = SearchMode.HISCentral;
            ucSearch.lblServerValue.Text = ucSearch.SearchMode.Description();
            //moved to search control load
            ucSearch.dateTimePickStart.Value = DateTime.Now.Date.AddYears(-100);//change suggested by Dan (range 100 years)
            ucSearch.dateTimePickEnd.Value = DateTime.Now.Date;
            ucSearch.groupBox3.Visible = true;
            RefreshTheMap();
        }

        //advanced options
        private void _rbtnAdvancedSettings_Click(object sender, EventArgs e)
        {
           new AdvancedSettingsDialog(ucSearch).ShowDialog();
        }

        //force  the map to redraw
        private void RefreshTheMap()
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

        #endregion Ribbon

        #region Saving Project
        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            //move the search result layer to the correct folder
            string newDirectory = _mapArgs.AppManager.SerializationManager.CurrentProjectDirectory;
            //check if there are search results layers
            string searchGroupName = Global.SEARCH_RESULT_LAYER_NAME;
            //find the search result group
            IMapGroup grp = _mapArgs.Map.MapFrame.GetAllGroups().Find(p => p.LegendText == searchGroupName);
            if (grp == null) return; //no search result groups
            foreach (IMapLayer layer in grp.Layers)
            {
                IFeatureSet fs = layer.DataSet as IFeatureSet;
                if (fs != null)
                {
                    try
                    {
                        if (System.IO.File.Exists(fs.Filename))
                        {
                            fs.Filename = System.IO.Path.Combine(newDirectory, System.IO.Path.GetFileName(fs.Filename));
                            fs.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        //todo log the error
                        string msg = ex.Message;
                    }
                }
            }
        }
        #endregion
    }

    public enum SearchMode
    {
        //TODO: implement localizable attribute if need

        [Description("HIS Central")]
        HISCentral,
        [Description("Local Metadata Cache")]
        LocalMetaDataCache
    }
}