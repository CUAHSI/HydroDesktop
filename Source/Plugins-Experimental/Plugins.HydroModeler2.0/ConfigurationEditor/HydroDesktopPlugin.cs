using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using System.Windows.Forms;
using Oatc.OpenMI.Gui.ConfigurationEditor;
using System.Drawing;


namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    [Plugin("HydroModeler 2.0", Author = "CUAHSI at USC", UniqueName = "mw_HydroModeler_2", Version = "2.0")]
    class HydroDesktopPlugin: Extension, IMapPlugin
    {
        #region Variables

        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;

        //the main tab control where map view, graph view and table view are displayed
        private TabControl _mainTabControl = null;

        //the tab page which will be added to the tab control by the plugin
        private TabPage _tabPage = null;

        //a sample toolbar button added by the plugin
        private ToolStripButton btnHydroModelerPlugin = null;

        //a sample menu item added by the plugin
        private ToolStripMenuItem mnuHydroModelerPlugin = null;



        #endregion

        #region IExtension Members

        protected override void OnDeactivate()
        {
            if (_mapArgs.MainToolStrip != null && btnHydroModelerPlugin != null)
            {
                _mapArgs.MainToolStrip.Items.Remove(btnHydroModelerPlugin);
            }
            if (_mapArgs.MainMenu != null && _mapArgs.MainMenu.Items.Find("hydroModeler2MenuItem", true) != null)
            {
                int pluginIndex = 0;
                for (int i = 0; i < _mapArgs.MainMenu.Items.Count; i++)
                {
                    if (_mapArgs.MainMenu.Items[i].Text == "HydroModeler 2.0")
                        pluginIndex = i;
                }
                _mapArgs.MainMenu.Items.RemoveAt(pluginIndex);//.RemoveByKey("hydroModeler2MenuItem");
            }

            if (_mainTabControl != null && _tabPage != null)
            {
                _mainTabControl.TabPages.Remove(_tabPage);
            }
        }
        #endregion

        #region IMapPlugin Members

        public void Initialize(IMapPluginArgs args)
        {
            _mapArgs = args;

            btnHydroModelerPlugin = new ToolStripButton();

            // Add UI features
            btnHydroModelerPlugin.DisplayStyle = ToolStripItemDisplayStyle.Text;
            //btnSamplePlugin.Image = Resources.MySampleIcon1.ToBitmap();
            btnHydroModelerPlugin.Name = "HydroModeler 2.0";
            btnHydroModelerPlugin.ToolTipText = "Launch HydroModeler 2.0";
            btnHydroModelerPlugin.Click += new EventHandler(btnSamplePlugin_Click);

            if (_mapArgs.ToolStripContainer != null)
            {
                // Add the 'Sample Plugin' button
                _mapArgs.MainToolStrip.Items.Add(btnHydroModelerPlugin);

                // Add the 'Added by plugin' tab control
                foreach (Control control in _mapArgs.ToolStripContainer.ContentPanel.Controls)
                {
                    if (control is TabControl)
                    {
                        _mainTabControl = control as TabControl;
                        _tabPage = new TabPage("HydroModeler 2.0");
                        _mainTabControl.TabPages.Add(_tabPage);
                        break;
                    }
                }
            }

            //add some items to the newly created tab control
            if (_tabPage != null)
            {
                //TreeView treeView = new TreeView();
                //treeView.Location = new System.Drawing.Point(0, 0);
                //treeView.Size = new System.Drawing.Size(_tabPage.Width / 3, _tabPage.Height);
                //treeView.Nodes.Add("Model Components");
                //_tabPage.Controls.Add(treeView);

                MainTab mainTab = new MainTab(_mapArgs);
                mainTab.Dock = DockStyle.Fill;
                mainTab.HorizontalScroll.Visible = false;
                mainTab.VerticalScroll.Visible = false;
                mainTab.Location = new System.Drawing.Point(_tabPage.Width / 3, 0);
                mainTab.Size = new System.Drawing.Size(_tabPage.Width * 2 / 3, _tabPage.Height);
                //lbl.Text = "This tab was added by the HydroModeler Plugin.";
                //lbl.Location = new System.Drawing.Point(_tabPage.Width / 2, _tabPage.Height / 2);
                _tabPage.Controls.Add(mainTab);

            }   
        }

        #endregion

        #region UI Events

        void btnSamplePlugin_Click(object sender, EventArgs e)
        {
            //create a new configuration editor instance
            //Oatc.OpenMI.Gui.ConfigurationEditor.MainForm mainOmiEd = new MainForm();

            //Launch the OpenMI OmiED
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.FileName = "./Plugins/HydroModeler_new/Oatc_OpenMI_ConfigurationEditor.exe";

            p.Start();


            //   MessageBox.Show("This plugin doesn't do anything...yet");
        }

        #endregion
    }
}
