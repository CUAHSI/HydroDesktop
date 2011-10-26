using System.Windows.Forms;
using MapWindow.Components;
using MapWindow.Map;
using MapWindow.Plugins;
using System;

namespace HydroR
{
    [MapWindowPlugin("HydroR", Author = "USU", UniqueName = "HydroR_1", Version = "1")]
    public class Main : Extension, IMapPlugin
    {
        #region Variables

        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;

        //the main tab control where map view, graph view and table view are displayed
        private TabControl _mainTabControl = null;

        //the tab page which will be added to the tab control by the plugin
        private TabPage _hydroRTabPage = null;

        private cRCommandView _hydroRControl = null;

        
        
        #endregion

        #region IExtension Members

       

        #endregion

        #region IPlugin Members

        /// <summary>
        /// Initialize the mapWindow 6 plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public void Initialize(IMapPluginArgs args)
        {
            //set the reference to the MapWindow application
            _mapArgs = args;

            string tabPageName = "HydroR";
            _mainTabControl = FindMainTabControl();



            //add some items to the newly created tab control
            if (_mainTabControl != null)
            {
                _hydroRTabPage = new TabPage(tabPageName);
                _mainTabControl.TabPages.Add(_hydroRTabPage);

                //add the cRCommandView control to the main tab page
                _hydroRControl = new cRCommandView();
                _hydroRTabPage.Controls.Add(_hydroRControl);
                _hydroRControl.Dock = DockStyle.Fill;
                _mainTabControl.SelectedIndexChanged += new EventHandler(mainTabControl_SelectedIndexChanged);
          
            }
        }

        private TabControl FindMainTabControl()
        {
            ToolStripContentPanel mainContentPanel = _mapArgs.ToolStripContainer.ContentPanel;

            foreach (Control control in mainContentPanel.Controls)
            {
                if (control is TabControl)
                {
                    return control as TabControl;
                }
            }
            return null;
        }
        #endregion

        #region Event Handlers
        void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mainTabControl.SelectedTab == _hydroRTabPage)
            {
                _hydroRControl.RefreshView();
            }
        }
        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {                      

            if (_mainTabControl != null && _hydroRTabPage != null)
            {
                _mainTabControl.TabPages.Remove(_hydroRTabPage);
                _mainTabControl.SelectedIndexChanged -= mainTabControl_SelectedIndexChanged;
            }

        }
        

        #endregion
    }
}
