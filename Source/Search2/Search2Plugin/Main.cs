using System;
using System.ComponentModel;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using HydroDesktop.Search.Extensions;
using System.Collections.Generic;
using DotSpatial.Controls.Docking;
using Hydrodesktop.Common;

namespace HydroDesktop.Search
{
    public class Main : Extension
    {

        #region Variables

        private const string kSearchDock = "kHydroSearchDock";
        
        //the Search user control
        private SearchControl ucSearch = null;

        //the search dockable panel
        DockablePanel searchPanel = null;
        bool searchPanelIsVisible = false;

        Extent _initialExtent = null;

        #endregion Variables

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            //Remove the Search Ribbon Items
            App.HeaderControl.RemoveAll();

            App.DockManager.Remove(kSearchDock);

            //to refresh the map
            RefreshTheMap();

            //necessary for plugin deactivation
            base.Deactivate();
        }

        /// <summary>
        /// Initialize the plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public override void Activate()
        {
            _initialExtent = (Extent)App.Map.ViewExtents.Clone();

            //add the ribbon panels and buttons
            AddRibbonItems();

            //setup the search dock panel
            // (will be activated when user selects search option)
            if (App.Map != null)
            {
                //report progress
                ReportProgress(0, "Loading Search Plugin..");

                //Set the Search control
                ucSearch = new SearchControl(App);
                ucSearch.Dock = DockStyle.Fill;
                searchPanel = new DockablePanel(kSearchDock, "search", ucSearch, DockStyle.Right);

                ReportProgress(50, "Loading Search Plugin..");

                App.Map.ViewExtents = _initialExtent;

                ReportProgress(0, String.Empty);

                //handle project saving event
                App.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);
            }

            base.Activate();
        }

        #endregion

        private void AddSearchPanel()
        {
            if (!searchPanelIsVisible)
            {
                App.DockManager.Add(searchPanel);
            }
            App.DockManager.SelectPanel(kSearchDock);
            searchPanelIsVisible = true;
        }
        private void RemoveSearchPanel()
        {
            if (searchPanel == null) return;

            App.DockManager.Remove(kSearchDock);
            searchPanelIsVisible = false;
        }

        #region Progress

        private void ReportProgress(int percent, string message)
        {
            if (App.ProgressHandler != null)
            {
                App.ProgressHandler.Progress(message, percent, message);
            }
        }

        #endregion Progress

        #region Ribbon

        //to add the ribbon panel and ribbon buttons
        private void AddRibbonItems()
        {
            if (App.HeaderControl != null)
            {
                var header = App.HeaderControl;

                const string KHydroSearch = "kHome";
                const string SearchMenuKey = "kSearch";

                header.Add(new MenuContainerItem(KHydroSearch, SearchMenuKey, "Search") { GroupCaption = "Search", LargeImage = Resources.search2_4 });
                header.Add(new SimpleActionItem(KHydroSearch, SearchMenuKey, "HIS Central", rbtnHISCentral_Click) { GroupCaption = "Search", SmallImage = Resources.OpenSearch });
                header.Add(new SimpleActionItem(KHydroSearch, SearchMenuKey, "Metadata Cache", rbtnMetadataCache_Click) { GroupCaption = "Search", SmallImage = Resources.OpenSearch_1 });
                header.Add(new SimpleActionItem(KHydroSearch, SearchMenuKey, "Advanced Settings", _rbtnAdvancedSettings_Click) { GroupCaption = "Search" });
            }
        }

        //metadata cache ribbon dropdown click event
        private void rbtnMetadataCache_Click(object sender, EventArgs e)
        {
            AddSearchPanel();
            
            ucSearch.SearchMode = SearchMode.LocalMetaDataCache;
            RefreshTheMap();
        }

        //his central ribbon dropdown click event
        private void rbtnHISCentral_Click(object sender, EventArgs e)
        {
            AddSearchPanel();
            
            ucSearch.SearchMode = SearchMode.HISCentral;
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
            App.Map.MapFrame.ResetExtents();
            App.Map.Invalidate();
        }

        #endregion Ribbon

        #region Saving Project
        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            //move the search result layer to the correct folder
            string newDirectory = App.SerializationManager.CurrentProjectDirectory;
            //check if there are search results layers
            string searchGroupName = LayerConstants.SearchGroupName;
            //find the search result group
            IMapGroup grp = App.Map.MapFrame.GetAllGroups().Find(p => p.LegendText == searchGroupName);
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