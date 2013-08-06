using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using HydroDesktop.Help;
using HydroDesktop.Common;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    public class Program : Extension
    {
        #region Variables

        // Local variables
        mainTab hydroModelerControl;
        private string ImagePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).CodeBase).Remove(0,6) + "/icons";
        private string _defaultPath = HydroModeler.Properties.Resources.startpath;
        private readonly string _localHelpUri = HydroModeler.Settings.Default.localHelpUri;
        private RootItem root = null;
        private bool ignoreRootSelected = false; //used by synchronizing between ribbon and docking

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
            //detach events
            App.DockManager.ActivePanelChanged -= HM_Panel_Selected;
            App.HeaderControl.RootItemSelected -= HeaderControl_RootItemSelected;
            
            // Remove ribbon tab
            App.HeaderControl.RemoveAll();

            // Remove the plugin panel
            //App.DockManager.Remove(kHydroModelerDock);

            // this line ensures that 'enabled' is set to false
            base.Deactivate();
        }

        /// <summary>
        /// Occurs when the HydroModeler plugin is loaded
        /// </summary>
        public override void Activate()
        {           
            root = new RootItem(KHydroModeler, _pluginName);
            root.SortOrder = 150;
            root.Visible = true;
            App.HeaderControl.Add(root);

            // add buttons to the ribbon
            rps_dict = BuildRibbonPanel();

            // Add a dockable panel
            Add_HM_Panel();      

            // activate plugin
            base.Activate();
        }

        void Add_HM_Panel()
        {
            // Add a dockable panel
            hydroModelerControl = new mainTab(App, rps_dict, ((TextEntryActionItem)rps_dict["dirbox"]).Text);
            var hmDockPanel = new DockablePanel(kHydroModelerDock, _pluginName, hydroModelerControl, DockStyle.Fill);
            hmDockPanel.DefaultSortOrder = 1000; //HydroModeler should be the last dockable panel by default
            App.DockManager.Add(hmDockPanel);

            // set the initial text for the dirbox
            try
            {              
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                string start_path = Path.GetFullPath(HydroModeler.Properties.Resources.startpath);
                if (Directory.Exists(start_path))
                    ((TextEntryActionItem)rps_dict["dirbox"]).Text = start_path;
                else
                    ((TextEntryActionItem)rps_dict["dirbox"]).Text = "C:\\";
            }
            catch
            {
                ((TextEntryActionItem)rps_dict["dirbox"]).Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\HydroModeler\\"); //C:\\";
            }

            // update filelist
            string text = ((TextEntryActionItem)rps_dict["dirbox"]).Text;
            hydroModelerControl.filelist_update(text);

            // set pan mouse image
            hydroModelerControl.Image_Path = ImagePath;

            // add event for when HM panel is selected
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.DockablePanelEventArgs>(HM_Panel_Selected);

            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
        }

        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (ignoreRootSelected) return;

            if (e.SelectedRootKey == "RootRibbonHydroModeler")
            {
                App.DockManager.SelectPanel(kHydroModelerDock);

                //hide panels
                App.DockManager.HidePanel("kLegend");
                App.DockManager.HidePanel(HydroDesktop.Common.SharedConstants.SeriesViewKey);
            }
            else if (e.SelectedRootKey == SharedConstants.SearchRootkey || e.SelectedRootKey == HeaderControl.HomeRootItemKey)
            {
                App.DockManager.SelectPanel("kLegend");
                App.DockManager.ShowPanel(HydroDesktop.Common.SharedConstants.SeriesViewKey);
            }
            else if (e.SelectedRootKey == "kHydroGraph_01" || e.SelectedRootKey == SharedConstants.TableRootKey || e.SelectedRootKey == "kHydroEditView" || e.SelectedRootKey == "kHydroR")
            {
                App.DockManager.SelectPanel(HydroDesktop.Common.SharedConstants.SeriesViewKey);
                App.DockManager.ShowPanel("kLegend");
            }
        }
        #endregion

        #region IMapPlugin Members

        private Dictionary<string, object> BuildRibbonPanel()
        {

            //Create a new Ribbon Panel
            List<SimpleActionItem> btns = new List<SimpleActionItem>();
            Dictionary<string, object> rps = new Dictionary<string, object>();

            #region menu panel
            //Open Composition
            var rb = new SimpleActionItem("Open", OpenComp_Click);
            rb.ToolTipText = "Open a saved model configuration";
            rb.LargeImage = HydroModeler.Properties.Resources.open.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.open.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Model";
            rb.RootKey = KHydroModeler;
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("open", rb);

            //save
            rb = new SimpleActionItem("Save", Save_Click);
            rb.ToolTipText = "Save model configuration";
            rb.LargeImage = HydroModeler.Properties.Resources.save.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.save.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Model";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("save", rb);

            //save as
            rb = new SimpleActionItem("Save As...", SaveAs_Click);
            rb.ToolTipText = "Save model configuration as...";
            rb.LargeImage = HydroModeler.Properties.Resources.saveas.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.saveas.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Model";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("saveas", rb);

            #endregion

            #region model_panel
            //Add Model
            rb = new SimpleActionItem("Add Component", AddModel_Click);
            rb.ToolTipText = "Click to add a model to the composition";
            rb.LargeImage = HydroModeler.Properties.Resources.add_model.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.add_model.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("component", rb);

            //Add Trigger
            rb = new SimpleActionItem("Add Trigger", AddTrigger_Click);
            rb.ToolTipText = "Click to add a trigger to the composition";
            rb.LargeImage = HydroModeler.Properties.Resources.trigger.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.trigger.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("trigger", rb);

            //Add Connection
            rb = new SimpleActionItem("Add Connection", AddConnection_Click);
            rb.ToolTipText = "Click to add a connection to the composition";
            rb.LargeImage = HydroModeler.Properties.Resources.add_connection.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.add_connection.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("connection", rb);

            //Run
            rb = new SimpleActionItem("Run", Run_Click);
            rb.ToolTipText = "Run model simulation";
            rb.LargeImage = HydroModeler.Properties.Resources.run.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.run.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("run", rb);

            //Clear Composition
            rb = new SimpleActionItem("Clear Composition", this.clear_Click);
            rb.ToolTipText = "Clear all items from the model canvas";
            rb.LargeImage = HydroModeler.Properties.Resources.delete_icon.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.delete_icon.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Composition";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("clear", rb);

            #endregion

            #region dir_panel
            var rtb = new TextEntryActionItem();
            rtb.ToolTipText = "The current working directory";
            rtb.Width = 300;
            rtb.GroupCaption = "Directory";
            rtb.RootKey = KHydroModeler;
            rtb.Caption = "Current Path: ";
            rtb.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(rtb_PropertyChanged);
            App.HeaderControl.Add(rtb);
            rps.Add("dirbox", rtb); 
            #endregion

            #region view_panel
            rb = new SimpleActionItem("Pan", this.set_pan);
            rb.ToolTipText = "Click to activate pan cursor on the model canvas";
            rb.LargeImage = HydroModeler.Properties.Resources.pan1.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.pan1.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "View";
            rb.ToggleGroupKey = "View";
            rb.RootKey = KHydroModeler; 
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("pan", rb);

            rb = new SimpleActionItem("Select", this.set_select);
            rb.ToolTipText = "Click to activate the select cursor on the model canvas";
            rb.LargeImage = HydroModeler.Properties.Resources.select.GetThumbnailImage(32, 32, null, IntPtr.Zero);
            rb.SmallImage = HydroModeler.Properties.Resources.select.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "View";
            rb.ToggleGroupKey = "View";
            rb.RootKey = KHydroModeler;
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("select", rb);

            #endregion

            #region help_panel
            rb = new SimpleActionItem("", this.getHelp);
            rb.ToolTipText = "Click to launch the HydroModeler help documentation";
            rb.SmallImage = HydroModeler.Properties.Resources.help.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            rb.GroupCaption = "Help";
            rb.RootKey = KHydroModeler;
            App.HeaderControl.Add(rb);
            btns.Add(rb);
            rps.Add("help", rb);
            #endregion

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
            ((SimpleActionItem)rps_dict["run"]).Enabled = run;

            //enable or disble connection button
            ((SimpleActionItem)rps_dict["connection"]).Enabled = connection;

        }

        #endregion

        #region UI Events
        void HM_Panel_Selected(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == kHydroModelerDock)
            {
                App.DockManager.SelectPanel("RootRibbonHydroModeler");
                ignoreRootSelected = true;
                App.HeaderControl.SelectRoot(KHydroModeler);
                ignoreRootSelected = false;

                //hide panels
                App.DockManager.HidePanel("kLegend");
                App.DockManager.HidePanel(HydroDesktop.Common.SharedConstants.SeriesViewKey);
            }
        }

        private void set_pan(object sender, EventArgs e)
        {
            hydroModelerControl.Ispan = true;
            SimpleActionItem s = ((SimpleActionItem)rps_dict["pan"]);
        }
        private void set_select(object sender, EventArgs e)
        {
            hydroModelerControl.Ispan = false;
            SimpleActionItem s = ((SimpleActionItem)rps_dict["select"]);
        }
        private void ribbonTextBox_textChanged(TextEntryActionItem textbox)
        {
            //get path from ribbon textbox
            string path = textbox.Text;

            //replace path slashes
            path = path.Replace('/', '\\');

            //update file list
            hydroModelerControl.filelist_update(path);
        }
        public void ribbonTextBox_update(string path)
        {           
            //update the ribbon textbox text
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
            try
            {
                LocalHelp.OpenHelpFile(_localHelpUri);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open help file at " + _localHelpUri + "\n" + ex.Message, "Could not open help", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
