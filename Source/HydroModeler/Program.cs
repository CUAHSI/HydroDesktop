using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Compatibility;
using System.Windows.Forms;
using Oatc.OpenMI.Gui.ConfigurationEditor;
using System.Drawing;
using System.IO;
using System.Reflection;
using HydroDesktop.Help;
using DotSpatial.Controls.Docking;
//using DotSpatial.Controls.RibbonControls; //must be replaced by DotSpatial.Controls.Header
using DotSpatial.Controls.Header;
using HydroDesktop.Interfaces;
using System.ComponentModel.Composition;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    public class Program : Extension
    {
        #region Variables

        //reference to the main application and it's UI items
        //private AppManager _mapArgs;
        //public RibbonTab _ribbonTab;
        //public RibbonButton _ribbonBtn;
        //private ITabManager _t;
        //public string _panelName = "HydroModeler";
        //List<RibbonPanel> rps;

        // Local variables
        mainTab hydroModelerControl;
        private string ImagePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).CodeBase).Remove(0,6) + "/icons";
        private string _defaultPath = HydroModeler.Properties.Resources.startpath;
        private readonly string _localHelpUri = HydroModeler.Settings.Default.localHelpUri;
        private RootItem root = null;

        // reference to the main series view panel
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; private set; }

        // plugin info
        private const string _pluginName = "HydroModeler";
        private const string kHydroModelerDock = "kHydroModelerDock";
        private const string KHydroModeler = "RootRibbonHydroModeler";


        private Dictionary<string, object> rps_dict = new Dictionary<string, object>();
        #endregion


        #region IExtension Members

        /// <summary>
        /// Occurs when the HydroModeler plugin is unloaded
        /// </summary>
        public override void Deactivate()
        {
            // Remove ribbon tab
            App.HeaderControl.RemoveAll();

            // Remove the plugin panel
            App.DockManager.Remove(kHydroModelerDock);

            // this line ensures that 'enabled' is set to false
            base.Deactivate();
        }

        /// <summary>
        /// Occurs when the HydroModeler plugin is loaded
        /// </summary>
        public override void Activate()
        {
            // add 'ribbon' button to the 'view' panel in 'home' ribbon tab
            root = new RootItem(KHydroModeler, _pluginName);
            //root.SortOrder = 100;
            root.SortOrder = 150;
            App.HeaderControl.Add(root);

            // add buttons
            rps_dict = BuildRibbonPanel();
            //foreach (RibbonPanel rp in rps)
            //    _ribbonTab.Panels.Add(rp);

            

            //set map args
            //_mapArgs = App;

            // Initialize the Ribbon controls in the "Ribbon" ribbon tab
            //_ribbonTab = new RibbonTab();
            //_ribbonTab.ActiveChanged += new EventHandler(_ribbonTab_ActiveChanged);

            //Add Buttons to the Ribbon Panel
            
            // Add a dockable panel
            if (SeriesControl != null)
            {
                Add_HM_Panel();
                //hydroModelerControl = new mainTab(App, rps_dict, ((TextEntryActionItem)rps_dict["dirbox"]).Text);
                //App.DockManager.Add(new DockablePanel(kHydroModelerDock, _pluginName, hydroModelerControl, DockStyle.Fill));
                
                
            }

            ////specify tab window
            ////hydroModelerControl = new mainTab(App, rps, ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText);
            ////_t.AddTab(_panelName, hydroModelerControl, bmp);
            ////hydroModelerControl.Dock = DockStyle.Fill;
            ////App.DockManager.Add(new DockablePanel(kHydroModelerDock, "HydroModeler", hydroModelerControl, DockStyle.Fill));

            ////activate tab
            ////_ribbonBtn_Click(this, EventArgs.Empty);


            //// set the initial text for the dirbox
            //string start_path = Path.GetFullPath(HydroModeler.Properties.Resources.startpath);
            //if (Directory.Exists(start_path))
            //     ((TextEntryActionItem)rps_dict["dirbox"]).Text =start_path;
            //else
            //    ((TextEntryActionItem)rps_dict["dirbox"]).Text = "C:\\";

            //// update filelist
            ////string text = ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText;
            //string text = ((TextEntryActionItem)rps_dict["dirbox"]).Text;
            //hydroModelerControl.filelist_update(text);

            //// set pan mouse image
            //hydroModelerControl.Image_Path = ImagePath;

            //// add event for when HM panel is selected
            //App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(HM_Panel_Selected);

            // activate plugin
            base.Activate();
        }

        void Add_HM_Panel()
        {
            //if (e.ActivePanelKey != "kHydroModelerDock") return;
            
            // Add a dockable panel
            if (SeriesControl != null)
            {
                hydroModelerControl = new mainTab(App, rps_dict, ((TextEntryActionItem)rps_dict["dirbox"]).Text);
                App.DockManager.Add(new DockablePanel(kHydroModelerDock, _pluginName, hydroModelerControl, DockStyle.Fill));
            }

            //specify tab window
            //hydroModelerControl = new mainTab(App, rps, ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText);
            //_t.AddTab(_panelName, hydroModelerControl, bmp);
            //hydroModelerControl.Dock = DockStyle.Fill;
            //App.DockManager.Add(new DockablePanel(kHydroModelerDock, "HydroModeler", hydroModelerControl, DockStyle.Fill));

            //activate tab
            //_ribbonBtn_Click(this, EventArgs.Empty);


            // set the initial text for the dirbox
            string start_path = Path.GetFullPath(HydroModeler.Properties.Resources.startpath);
            if (Directory.Exists(start_path))
                ((TextEntryActionItem)rps_dict["dirbox"]).Text = start_path;
            else
                ((TextEntryActionItem)rps_dict["dirbox"]).Text = "C:\\";

            // update filelist
            //string text = ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText;
            string text = ((TextEntryActionItem)rps_dict["dirbox"]).Text;
            hydroModelerControl.filelist_update(text);

            // set pan mouse image
            hydroModelerControl.Image_Path = ImagePath;

            // add event for when HM panel is selected
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(HM_Panel_Selected);
        }
        #endregion

        //public List<RibbonPanel> Rps
        //{
        //    get { return rps; }
        //    set { rps = value; }
        //}

        //public List<SimpleActionItem> Rps
        //{
        //    get { return rps; }
        //    set { rps = value; }
        //}

        //public System.Collections.Hashtable hash = new System.Collections.Hashtable();
        

        #region IMapPlugin Members

        private Dictionary<string, object> BuildRibbonPanel()
        {

            //Create a new Ribbon Panel
            //RibbonPanel menu_panel = new RibbonPanel("Model",RibbonPanelFlowDirection.Bottom);
            //RibbonPanel model_tools = new RibbonPanel("Composition", RibbonPanelFlowDirection.Bottom);
            //RibbonPanel dir_panel = new RibbonPanel("Current Directory", RibbonPanelFlowDirection.Bottom);
            //RibbonPanel utility_panel = new RibbonPanel("Utilities", RibbonPanelFlowDirection.Bottom);
            //RibbonPanel help_panel = new RibbonPanel("Help", RibbonPanelFlowDirection.Bottom);
            //RibbonPanel view_panel = new RibbonPanel("Pan", RibbonPanelFlowDirection.Bottom);


            List<SimpleActionItem> btns = new List<SimpleActionItem>();
            Dictionary<string, object> rps = new Dictionary<string, object>();

            #region menu panel
            //menu_panel.ButtonMoreVisible = false;

            //Open Composition
            /*
            RibbonButton rb = new RibbonButton("Open");
            menu_panel.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.open.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.open.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(OpenComp_Click);
            rb.Dispose();
             */
            var rb = new SimpleActionItem("Open", OpenComp_Click);
            rb.LargeImage = HydroModeler.Properties.Resources.open.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.open.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Model";
            rb.RootKey = KHydroModeler;
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("open", rb);

            //save
            /*rb = new RibbonButton("Save");
            menu_panel.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.save.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.save.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(Save_Click);
            rb.Dispose();
            */
            rb = new SimpleActionItem("Save", Save_Click);
            rb.LargeImage = HydroModeler.Properties.Resources.save.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.save.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Model";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("save", rb);

            //save as
            /*rb = new RibbonButton();
            rb.Text = "Save As...";
            menu_panel.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.saveas.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.saveas.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(SaveAs_Click);
            rb.Dispose();*/
            rb = new SimpleActionItem("Save As...", SaveAs_Click);
            rb.LargeImage = HydroModeler.Properties.Resources.saveas.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.saveas.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Model";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("saveas", rb);

            #endregion

            #region model_panel
            //model_tools.ButtonMoreVisible = false;

            //Add Model
            /*rb = new RibbonButton("Add Component");
            model_tools.Items.Add(rb);
            rb.ToolTip = "Click to add a model to the composition";
            rb.Image = HydroModeler.Properties.Resources.add_model.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.add_model.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(AddModel_Click);
            rb.Dispose();*/
            rb = new SimpleActionItem("Add Component", AddModel_Click);
            //rb.ToolTip = "Click to add a model to the composition";
            rb.LargeImage = HydroModeler.Properties.Resources.add_model.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.add_model.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("component", rb);

            //Add Trigger
            /*rb = new RibbonButton("Add Trigger");
            model_tools.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.trigger.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.trigger.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(AddTrigger_Click);
            rb.Dispose();*/
            rb = new SimpleActionItem("Add Trigger", AddTrigger_Click);
            //rb.ToolTip = "Click to add a trigger to the composition";
            rb.LargeImage = HydroModeler.Properties.Resources.trigger.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.trigger.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("trigger", rb);

            //Add Connection
            /*rb = new RibbonButton("Add Connection");
            rb.Enabled = false;
            model_tools.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.add_connection.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.add_connection.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(AddConnection_Click);
            rb.Dispose();*/
            rb = new SimpleActionItem("Add Connectiob", AddConnection_Click);
            //rb.ToolTip = "Click to add a connection to the composition";
            rb.LargeImage = HydroModeler.Properties.Resources.add_connection.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.add_connection.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("connection", rb);

            //Run
            /*rb = new RibbonButton("Run");
            rb.Enabled = false;
            model_tools.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.run.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.run.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(Run_Click);
            rb.Dispose();*/
            rb = new SimpleActionItem("Run", Run_Click);
            rb.LargeImage = HydroModeler.Properties.Resources.run.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.run.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("run", rb);

            //Clear Composition
            /*rb = new RibbonButton("Clear Composition");
            model_tools.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.delete_icon.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.delete_icon.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(this.clear_Click);
            rb.Dispose();*/
            rb = new SimpleActionItem("Clear Composition", this.clear_Click);
            rb.LargeImage = HydroModeler.Properties.Resources.delete_icon.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.delete_icon.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("clear", rb);

            #endregion

            #region dir_panel
            //dir_panel.ButtonMoreVisible = false;

            //change directory
            /*rb = new RibbonButton();
            rb.MaxSizeMode = RibbonElementSizeMode.Compact;
            rb.Image = HydroModeler.Properties.Resources.change_dir.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.change_dir.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.MinSizeMode = RibbonElementSizeMode.Compact;
            rb.Click += new EventHandler(this.dirItem_click);
            rb.Dispose();*/

            //directory text box
            /*RibbonTextBox rtb = new RibbonTextBox();
            rtb.Text = "Current Directory";
            rtb.TextBoxText = Path.GetFullPath(HydroModeler.Properties.Resources.startpath);
            rtb.TextBoxWidth = 300;
            rtb.TextBoxTextChanged += new System.EventHandler(ribbonTextBox_textChanged);*/

            //rb = new SimpleActionItem("", this.dirItem_click);
            //rb.SmallImage = HydroModeler.Properties.Resources.change_dir.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            //rb.GroupCaption = "Current Directory";
            //rb.RootKey = KHydroModeler;
            //rb.SortOrder = 100;
            //App.HeaderControl.Add(rb);
            //btns.Add(rb);
            //rps.Add("directory", rb);

            var rtb = new TextEntryActionItem();
            rtb.Width = 300;
            rtb.GroupCaption = "Directory";
            rtb.RootKey = KHydroModeler;
            rtb.Caption = "Current Path: ";
            //if (System.IO.Directory.Exists(System.IO.Path.Combine(new string[2]{Directory.GetCurrentDirectory(),_defaultPath})))
            //    rtb.Text = _defaultPath;
            //else
            //    rtb.Text = "C:/";
            rtb.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(rtb_PropertyChanged);
            App.HeaderControl.Add(rtb);
            rps.Add("dirbox", rtb); 
            
            //add to ribbon 
            //RibbonItemGroup rig = new RibbonItemGroup();
            //rig.Items.Add(rtb);
            //rig.Items.Add(rb);
            //dir_panel.Items.Add(rig);
            #endregion

            #region view_panel
            //view_panel.ButtonMoreVisible = false;

            /*rb = new RibbonButton();
            view_panel.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.pan1.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.pan1.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(this.set_pan);
            rb.Dispose();*/
            rb = new SimpleActionItem("Pan", this.set_pan);
            rb.LargeImage = HydroModeler.Properties.Resources.pan1.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.pan1.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "View";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("pan", rb);
            #endregion

            #region help_panel
            //help
            //help_panel.ButtonMoreVisible = false;

            /*rb = new RibbonButton();
            help_panel.Items.Add(rb);
            rb.Image = HydroModeler.Properties.Resources.help.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.help.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.Click += new EventHandler(this.getHelp);            
            rb.Dispose();*/
            rb = new SimpleActionItem("", this.getHelp);
            rb.SmallImage = HydroModeler.Properties.Resources.help.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Help";
            rb.RootKey = KHydroModeler;
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("help", rb);
            #endregion

            /*List<RibbonPanel> l = new List<RibbonPanel>();
            l.Add(menu_panel);
            l.Add(model_tools);
            l.Add(dir_panel);
            l.Add(view_panel);
            l.Add(help_panel);

            return l;*/

            return rps;
        }

        void rtb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TextEntryActionItem te = (TextEntryActionItem)rps_dict["dirbox"];
            ribbonTextBox_textChanged(te);
        }

        void reloadButtons(bool run, bool connection)
        {

            //enable or disable run button
            //rps[1].Items[3].Enabled = run;
            ((SimpleActionItem)rps_dict["run"]).Enabled = run;


            //enable or disble connection button
            //rps[1].Items[2].Enabled = connection;
            ((SimpleActionItem)rps_dict["connection"]).Enabled = connection;

        }

        #endregion



        #region UI Events
        void HM_Panel_Selected(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == kHydroModelerDock)
            {
                App.DockManager.SelectPanel("RootRibbonHydroModeler");
                App.HeaderControl.SelectRoot(KHydroModeler);
                root.Visible = true;
            }
            else
            {
                root.Visible = false;
            }
        }
        private void set_pan(object sender, EventArgs e)
        {
            if (hydroModelerControl.Ispan)
            {
                //hydroModelerControl.Ispan = false;
                //RibbonButton s = (RibbonButton)sender;
                //s.Checked = false;

            }
            else
            {
                //RibbonButton s = (RibbonButton)sender;
                //s.Checked = true;
                //hydroModelerControl.Ispan = true;
            }
        }
        //private void _ribbonTab_ActiveChanged(object sender, EventArgs e)
        //{

        //    if(_ribbonTab.Selected)
        //        _ribbonBtn_Click(sender, e);
        //}

        private void ribbonTextBox_textChanged(TextEntryActionItem textbox)
        {
            //get path from ribbon textbox
            //string path = ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText;
            //TextEntryActionItem textbox = (TextEntryActionItem)rps_dict["dirbox"];
            string path = textbox.Text;

            //replace path slashes
            path = path.Replace('/', '\\');

            //update file list
            hydroModelerControl.filelist_update(path);
        }
        public void ribbonTextBox_update(string path)
        {
            
            //update the ribbon textbox text
            //((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText = path;
            ((TextEntryActionItem)rps_dict["dirbox"]).Text = path;
        }
        public void dirItem_click(object sender, EventArgs e)
        {
            string path = hydroModelerControl.changeDir_Click(sender, e);

            if(path != null)
            {
                ribbonTextBox_update(path);
            }
        }
        void _ribbonBtn_Click(object sender, EventArgs e)
        {   
            //TODO change selected dock panel
            //if (_t.SelectedTabName == _panelName)
            //{
            //    if (!_mapArgs.Ribbon.Tabs.Contains(_ribbonTab))
            //    {
            //        _mapArgs.Ribbon.Tabs.Add(_ribbonTab);
            //        _t.SelectedTabName = _panelName;
            //        _mapArgs.Ribbon.ActiveTab = _mapArgs.Ribbon.Tabs[tabID];
            //    }
            //}
            //else
            //{
            //    if (!_mapArgs.Ribbon.Tabs.Contains(_ribbonTab))
            //    {
            //        _mapArgs.Ribbon.Tabs.Add(_ribbonTab);
            //        tabID = _mapArgs.Ribbon.Tabs.Count - 1;
            //        _t.SelectedTabName = _panelName;
            //        _mapArgs.Ribbon.ActiveTab = _mapArgs.Ribbon.Tabs[tabID];
            //    }
            //    else
            //    {
            //        hydroModelerControl.mainTab_KeyPress(this,new KeyPressEventArgs((char)System.Windows.Forms.Keys.LButton));
            //        _t.SelectedTabName = _panelName;
            //        _mapArgs.Ribbon.ActiveTab = _mapArgs.Ribbon.Tabs[tabID];
            //    }
            //} 
        }
        void keypressed(Object o, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
            }
        }
        void OpenComp_Click(object sender, EventArgs e)
        {
            //call open composition event
            hydroModelerControl.menuFileOpen_Click(sender, e);
        }
        void Save_Click(object sender, EventArgs e)
        {
            //call save event
            hydroModelerControl.menuFileSave_Click(sender, e);
        }
        void SaveAs_Click(object sender, EventArgs e)
        {
            //call save as event
            hydroModelerControl.menuFileSaveAs_Click(sender, e);
         
        }
        void AddModel_Click(object sender, EventArgs e)
        {
            //call add model event
            hydroModelerControl.menuEditModelAdd_Click(sender, e);
        }
        void AddConnection_Click(object sender, EventArgs e)
        {
            //call add connection event
            hydroModelerControl.menuEditConnectionAdd_Click(sender, e);
        }
        void AddTrigger_Click(object sender, EventArgs e)
        {
            //call add trigger event
            hydroModelerControl.menuEditTriggerAdd_Click(sender, e);
        }
        void Run_Click(object sender, EventArgs e)
        {
            //call run event
            hydroModelerControl.menuDeployRun_Click(sender, e);
        }
        void clear_Click(object sender, EventArgs e)
        {
            DialogResult result =  MessageBox.Show("Are you sure you want to remove all models from the composition window?", "Important Question", MessageBoxButtons.YesNo); 
            if(result == DialogResult.Yes)
                hydroModelerControl.composition_clear();
        }
        void getHelp(object sender, EventArgs e)
        {
            LocalHelp.OpenHelpFile(_localHelpUri);
        }
        #endregion

    }
}
