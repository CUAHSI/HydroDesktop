using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;
using HydroDesktop.Interfaces;

namespace HydroR
{
    public class HydroRPlugin : Extension
    {
        #region Variables

        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        [Import("SeriesControl", typeof(ISeriesSelector))]
        private ISeriesSelector _seriesSelector { get; set; }

        private const string _panelName = "HydroR";
        private const string kHydroR = "kHydroR";
        private readonly string _seriesViewKey = SharedConstants.SeriesViewKey;

        private RootItem _hydroRTab; 

        private SimpleActionItem _btnR;
        private SimpleActionItem _btnSettings;

        private cRCommandView _hydroRControl;

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            App.DockManager.Remove(kHydroR);

            if (Shell is Form)
            {
                ((Form)Shell).FormClosing -= MainForm_FormClosing;
            }
 
            base.Deactivate();
        }

        public override void Activate()
        {
            // todo: export Shell in MapWindow as Form to avoid type casting
            if (Shell is Form)
            {
                ((Form)Shell).FormClosing += MainForm_FormClosing;
            }

            // Add panel to the docking manager
            if (_seriesSelector != null)
            {
                _hydroRControl = new cRCommandView(_seriesSelector);
                
                AddHydroRPanel();

                AddHydroRRibbon();

                

                //when the HydroR panel is selected - activate SeriesView and HydroR ribbon tab
                App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(DockManager_ActivePanelChanged);

                App.DockManager.SelectPanel(_seriesViewKey);
                App.HeaderControl.SelectRoot(kHydroR);

                _hydroRControl.RChanged += new cRCommandView.REventHandler(_hydroRControl_RChanged);

                //when the HydroR root item is selected
                App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            }

            base.Activate();
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.PathToR = _hydroRControl.PathToR;
            Properties.Settings.Default.Save();
        }

        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == kHydroR)
            {
                App.DockManager.SelectPanel(kHydroR);
            }
        }

        //Add a HydroR root item
        void AddHydroRRibbon()
        {
            _hydroRTab = new RootItem(kHydroR, _panelName);
            _hydroRTab.SortOrder = 60;
            App.HeaderControl.Add(_hydroRTab);

            string rGroupCaption = _panelName + " Tools";
            string rScriptCaption = "Script";

            _btnR = new SimpleActionItem(kHydroR, "Start R", _hydroRControl.btnR_Click);
            _btnR.Key = "kBtnR";
            _btnR.LargeImage = Properties.Resources.Ricon;
            _btnR.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(_btnR);

            var btnGenR = new SimpleActionItem(kHydroR, "Generate R Code", _hydroRControl.txtGenR_Click);
            btnGenR.LargeImage = Properties.Resources.GenerateR;
            btnGenR.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnGenR);

            var btnSendLine = new SimpleActionItem(kHydroR, "Send Line", _hydroRControl.btnSend_Click);
            btnSendLine.LargeImage = Properties.Resources.SendLine;
            btnSendLine.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnSendLine);

            var btnSendSel = new SimpleActionItem(kHydroR, "Send Selection", _hydroRControl.btnSendSel_Click);
            btnSendSel.LargeImage = Properties.Resources.SendSelection;
            btnSendSel.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnSendSel);

            var btnSendAll = new SimpleActionItem(kHydroR, "Send All", _hydroRControl.btnSendAll_Click);
            btnSendAll.LargeImage = Properties.Resources.SendScript;
            btnSendAll.GroupCaption = rGroupCaption;
            App.HeaderControl.Add(btnSendAll);

            var btnOpen = new SimpleActionItem(kHydroR, "Open Script", _hydroRControl.btnOpen_Click);
            btnOpen.LargeImage = Properties.Resources.OpenFile;
            btnOpen.GroupCaption = rScriptCaption;
            App.HeaderControl.Add(btnOpen);

            var btnSave = new SimpleActionItem(kHydroR, "Save Script", _hydroRControl.btnSave_Click);
            btnSave.LargeImage = Properties.Resources.SaveFile;
            btnSave.GroupCaption = rScriptCaption;
            App.HeaderControl.Add(btnSave);

            _btnSettings = new SimpleActionItem(kHydroR, "Path to R", _hydroRControl.btnSettings_Click);
            _btnSettings.LargeImage = Properties.Resources.RSettings_32;
            _btnSettings.SmallImage = Properties.Resources.RSettings_16;
            _btnSettings.GroupCaption = "Settings";
            _btnSettings.Click += new EventHandler(_hydroRControl.btnSettings_Click);
            App.HeaderControl.Add(_btnSettings);
        }

        void AddHydroRPanel()
        {
            //HydroR dock panel should be preferentially added after "graph"
            var dp = new DockablePanel(kHydroR, _panelName, _hydroRControl, DockStyle.Fill);
            dp.DefaultSortOrder = 40;
            App.DockManager.Add(dp);
        }

        void _hydroRControl_RChanged(EventArgs e)
        {
            if (_hydroRControl.RIsRunning)
            {
                _btnR.Caption = "Close R";
                _btnSettings.Enabled = false;
            }
            else
            {
                _btnR.Caption = "Start R";
                _btnSettings.Enabled = true;
            }
        }

        //void DockManager_PanelAdded(object sender, DockablePanelEventArgs e)
        //{           
        //    if (e.ActivePanelKey == "kMap")
        //    {
        //        //HydroR dock panel should be preferentially added after "map"
        //        AddHydroRPanel();
        //        App.DockManager.SelectPanel("kMap");
        //    }
        //}

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == kHydroR)
            {
                App.DockManager.SelectPanel(_seriesViewKey);
                App.HeaderControl.SelectRoot(kHydroR);
            }
        }

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