using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel.Composition;

using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using DotSpatial.Controls.Header;

namespace HydroR
{
    public class HydroRPlugin : Extension
    {
        #region Variables

        [Import("SeriesSelector", typeof(ISeriesSelector))]
        private ISeriesSelector _seriesSelector;

        private string _panelName = "HydroR";
        private const string _tabKey = "kHydroR";
        private const string kHydroRDock = "kHydroRDock";

        private SimpleActionItem _btnR;

        private cRCommandView _hydroRControl;

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveItems();

            App.DockManager.Remove(kHydroRDock);
 
            base.Deactivate();
        }

        public override void Activate()
        {
            // Handle code for switching the page content
            //_hydroRControl.RChanged += new cRCommandView.REventHandler(ribbonBnt_TextChanged);

            // Add panel to the docking manager
            _hydroRControl = new cRCommandView(_seriesSelector);
            App.DockManager.Add(kHydroRDock, "HydroR", _hydroRControl, DockStyle.Fill);

            //Add a HydroR root item
            App.HeaderControl.Add(new RootItem(_tabKey, _panelName));

            string rGroupCaption = _panelName + " Tools";
            string rScriptCaption = "Script";

            _btnR = new SimpleActionItem("StartR", _hydroRControl.btnR_Click);
            _btnR.Key = "kBtnR";
            _btnR.RootKey = _tabKey;
            _btnR.LargeImage = Properties.Resources.Ricon;
            _btnR.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(_btnR);

            var btnGenR = new SimpleActionItem("Generate R Code", _hydroRControl.txtGenR_Click);
            btnGenR.RootKey = _tabKey;
            btnGenR.LargeImage = Properties.Resources.GenerateR;
            btnGenR.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnGenR);

            var btnSendLine = new SimpleActionItem("Send Line", _hydroRControl.btnSend_Click);
            btnSendLine.RootKey = _tabKey;
            btnSendLine.LargeImage = Properties.Resources.SendLine;
            btnSendLine.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnSendLine);

            var btnSendSel = new SimpleActionItem("Send Selection", _hydroRControl.btnSendSel_Click);
            btnSendSel.RootKey = _tabKey;
            btnSendSel.LargeImage = Properties.Resources.SendSelection;
            btnSendSel.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnSendSel);

            var btnSendAll = new SimpleActionItem("Send All", _hydroRControl.btnSendAll_Click);
            btnSendAll.RootKey = _tabKey;
            btnSendAll.LargeImage = Properties.Resources.SendScript;
            btnSendAll.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnSendAll);

            var btnOpen = new SimpleActionItem("Open Script", _hydroRControl.btnOpen_Click);
            btnOpen.RootKey = _tabKey;
            btnOpen.LargeImage = Properties.Resources.OpenFile;
            btnOpen.GroupCaption = rScriptCaption;
            App.HeaderControl.Add(btnOpen);

            var btnSave = new SimpleActionItem("Save Script", _hydroRControl.btnSave_Click);
            btnSave.RootKey = _tabKey;
            btnSave.LargeImage = Properties.Resources.SaveFile;
            btnSave.GroupCaption = rScriptCaption;
            App.HeaderControl.Add(btnSave);

            base.Activate();
        }

        #endregion



        //workaround method - changing the ribbon tab changes the main content
        //void Ribbon_ActiveTabChanged(object sender, EventArgs e)
        //{
        //    RibbonTab myTab = App.Ribbon.Tabs.Find(t => t.Text == _panelName);

        //    if (myTab.Active)
        //    {
        //        if (App.TabManager != null)
        //        {
        //            App.TabManager.SelectedTabName = "Series View";
        //            _seriesView.VisiblePanelName = _panelName;
        //        }
        //    }
        //}

        #region Event Handlers

        //private void ribbonBnt_TextChanged(EventArgs e)
        //{
        //    if (_hydroRControl.RIsRunning)
        //        _btnR.Caption = "Close R";
        //    else
        //        _btnR.Caption = "Start R";
        //}

        ////Workaround method to find the corresponding StartR ribbon button
        //private RibbonButton FindBtnR()
        //{
        //    RibbonTab hydroRTab = App.Ribbon.Tabs.Find(t => t.Text == _panelName);
        //    RibbonButton btnR = hydroRTab.Panels[0].Items[0] as RibbonButton;
        //    return btnR;
        //}

        #endregion
    }
}












/*using System.Windows.Forms;
using DotSpatial.Components;
using DotSpatial.Map;
using DotSpatial.Plugins;
using System;
using HydroDesktop.Data.Plugins;

namespace HydroR
{
    [Plugin("HydroR", Author = "Utah State University", UniqueName = "HydroR_1", Version = "1.0")]
    public class Main : Extension, IHydroPlugin
    {
        #region Variables

        //reference to the main application and it's UI items
        private IHydroPluginArgs _mapArgs;

        string PageName = "HydroR";
        //the main tab control where map view, graph view and table view are displayed
        private TabControl _mainTabControl = null;        

        //the tab page which will be added to the tab control by the plugin
        private TabPage _hydroRTabPage = null;        

        private cRCommandView _hydroRControl = null;

        #endregion

        #region Ribbon Variables
        private RibbonTab _ribbonTab;
        private RibbonPanel _ribbonPanel;

        private string _panelName = "HydroR";

        private ITabManager _t;

        private RibbonButton ribbonBnt;

        #endregion        

        #region IPlugin Members

        /// <summary>
        /// Initialize the mapWindow 6 plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        
        public void Initialize(IHydroPluginArgs args)
        {
            //set the reference to the MapWindow application
            _mapArgs = args;
            if (args.MainToolStrip != null)
            {                
                _mainTabControl = FindMainTabControl();
                
                //add some items to the newly created tab control
                if (_mainTabControl != null)
                {
                    _hydroRTabPage = new TabPage(PageName);
                    _mainTabControl.TabPages.Add(_hydroRTabPage);

                    //add the cRCommandView control to the main tab page
                    _hydroRControl = new cRCommandView(_mapArgs);
                    _hydroRTabPage.Controls.Add(_hydroRControl);
                    _hydroRControl.Dock = DockStyle.Fill;
                    _mainTabControl.SelectedIndexChanged += new EventHandler(mainTabControl_SelectedIndexChanged);
                }
            }
                
            else//ribbon
            {                                
                _ribbonTab = new RibbonTab(args.Ribbon, _panelName);                
                args.Ribbon.Tabs.Add(_ribbonTab);                      
                //args.Ribbon.Tabs[0].Panels[0].Items.Add(ribbonBnt);
                _ribbonTab.ActiveChanged += new EventHandler(ribbonBnt_Click);
                //this.Shown += new EventHandler(mainRibbonForm_Shown);

                _hydroRControl = new cRCommandView(_mapArgs);

                _ribbonPanel = new RibbonPanel(_panelName+" Tools");
               
                _mapArgs.SeriesView.AddPanel(_panelName + " Tools", _hydroRControl);
                _ribbonTab.Panels.Add(_ribbonPanel);

                ribbonBnt = new RibbonButton("Start R");                
                ribbonBnt.Image =Properties.Resources.Ricon ;
                //ribbonBnt.Click += new EventHandler(ribbonBnt_Click);
                ribbonBnt.Click += new EventHandler(_hydroRControl.btnR_Click);
                _ribbonPanel.Items.Add(ribbonBnt);
                

                RibbonButton btnGenR = new RibbonButton("Generate R Code");
                btnGenR.Image = Properties.Resources.GenerateR;
                btnGenR.Click += new EventHandler(_hydroRControl.txtGenR_Click);
                _ribbonPanel.Items.Add(btnGenR);

                RibbonButton btnSendLine = new RibbonButton("Send Line");
                btnSendLine.Image = Properties.Resources.SendLine;
                btnSendLine.Click += new EventHandler(_hydroRControl.btnSend_Click);
                _ribbonPanel.Items.Add(btnSendLine);

                RibbonButton btnSendSel = new RibbonButton("Send Selection");
                btnSendSel.Image = Properties.Resources.SendSelection;
                btnSendSel.Click += new EventHandler(_hydroRControl.btnSendSel_Click);
                _ribbonPanel.Items.Add(btnSendSel);

                RibbonButton btnSendAll = new RibbonButton("Send All");
                btnSendAll.Image = Properties.Resources.SendScript;
                btnSendAll.Click += new EventHandler(_hydroRControl.btnSendAll_Click);
                _ribbonPanel.Items.Add(btnSendAll);

                RibbonPanel pnlScript = new RibbonPanel("Script");
                _ribbonTab.Panels.Add(pnlScript);

                RibbonButton btnOpen = new RibbonButton("Open Script");
                btnOpen.Image = Properties.Resources.OpenFile;
                btnOpen.Click += new EventHandler(_hydroRControl.btnOpen_Click);
                pnlScript.Items.Add(btnOpen);

                RibbonButton btnSave = new RibbonButton("Save Script");
                btnSave.Image = Properties.Resources.SaveFile;
                btnSave.Click += new EventHandler(_hydroRControl.btnSave_Click);
                pnlScript.Items.Add(btnSave);
                                
                _t = _mapArgs.PanelManager;                

                //// Clear all tabpages
                //for (int i = 0; i < t.TabCount; i++)
                //{
                //    t.RemoveTabAt(0);
                //}
                // Add the 'Added by plugin' tab control
                
                _t.AddTab(_panelName, _hydroRControl);
                _t.SelectedIndexChanged += new EventHandler(t_SelectedIndexChanged);
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
        //when the shown tab page is changed (Ribbon version)
        void t_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_t.SelectedTabName == _panelName)
            {
                _hydroRControl.RefreshView();
            }
        }
        void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mainTabControl.SelectedTab == _hydroRTabPage)
            {
                _hydroRControl.RefreshView();
            }
        }


        protected override void OnActivate()
        {
            base.OnActivate();
            _hydroRControl.RChanged += new cRCommandView.REventHandler(helloWorld);
            //this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
            //this.ParentForm.ResizeEnd += new EventHandler(pnlR_Resized);
        }
        private void helloWorld(EventArgs e)
        {
            if (_hydroRControl.RIsRunning)
                ribbonBnt.Text = "Close R";
            else
                ribbonBnt.Text = "Start R";
   
        }
      
        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            if (_mapArgs.MainToolStrip != null)
            {

                if (_mainTabControl != null && _hydroRTabPage != null)
                {
                    _mainTabControl.TabPages.Remove(_hydroRTabPage);
                    _mainTabControl.SelectedIndexChanged -= mainTabControl_SelectedIndexChanged;
                }
            }
            else
            {
                _mapArgs.Ribbon.Tabs.Remove(_ribbonTab);            
                _mapArgs.Ribbon.Tabs[0].Panels[0].Items.Remove(ribbonBnt);
                if (_t.Contains(_panelName))
                    _t.RemoveTab(_panelName);
            }
            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }
         void ribbonBnt_Click(object sender, EventArgs e)
        {
            //if (_t.TabCount > 0)
           // {                
             if(_ribbonTab.Active){
                _t.SelectedTabName = _panelName;
                _hydroRControl.RefreshView();
            }
            
         }
    }
        #endregion
    
}
*/