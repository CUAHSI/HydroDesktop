using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Web.Services;

using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using DotSpatial.Projections;


namespace EPADelineation
{
    [Plugin("EPA Delineation", Author = "Jingqi Dong", UniqueName = "mw_EPADelineation_1", Version = "1")]
    public class Main : Extension, IMapPlugin
    {
        #region Variables

        //reference to the main application and its UI items
        private IMapPluginArgs _mapArgs;

        private RibbonPanel _rPanelEPADelineation;

        private RibbonButton _btstartDelineate;

        //this variable is used for showing the warning message
        private bool _showWarning = true;

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            if (_mapArgs == null) return;
            
            for (int i = 0; i < _mapArgs.Ribbon.Tabs[0].Panels.Count; i++)
            {
                if (_mapArgs.Ribbon.Tabs[0].Panels[i].Text == "EPA Tool")
                {
                    _mapArgs.Ribbon.Tabs[0].Panels[i].Items.Remove(_btstartDelineate);
                    _mapArgs.Ribbon.Tabs[0].Panels.Remove(_rPanelEPADelineation);
                }
            }

            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }

        protected override void OnActivate()
        {
            // Handle code for switching the page content

            // This line ensures that "Enabled" is set to true.
            base.OnActivate();
        }

        #endregion

        #region IPlugin Members

        /// <summary>
        /// Initialize the DotSpatial 6 plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public void Initialize(IMapPluginArgs args)
        {
            //in case of opening project..
            //string epa_setting = args.AppManager.SerializationManager.GetCustomSetting<string>("epa_setting", "false");
            string epa_setting = "new";
            if (args.AppManager.Ribbon.Tabs[1].Tag != null)
                epa_setting = args.AppManager.Ribbon.Tabs[1].Tag.ToString();
            if (epa_setting == "opening") //hack: to indicate opening project
                _showWarning = false;
            
            _mapArgs = args;
            
            //show warning message..
            bool runTool = true;
            if (_showWarning)
            {
                var frmWarning = new WarningForm();
                args.Ribbon.OrbDropDown.Close();
                if (frmWarning.ShowDialog() == DialogResult.OK)
                {
                    _showWarning = false;
                    runTool = true;
                }
                else
                {
                    runTool = false;
                }
            }
            //exit if user pressed cancel
            if (!runTool)
            {
                this.Deactivate();
                return;
            }
            
            

            //Setup the Panel and Add it to the MapView tab
            _rPanelEPADelineation = new RibbonPanel("EPA Tool", RibbonPanelFlowDirection.Bottom);
            _rPanelEPADelineation.ButtonMoreEnabled = false;
            _rPanelEPADelineation.ButtonMoreVisible = false;
            _mapArgs.Ribbon.Tabs[0].Panels.Add(_rPanelEPADelineation);

            //Setup Delineation Button
            _btstartDelineate = new RibbonButton("Delineate");
            _btstartDelineate.ToolTip = "Using EPA Web Services to Delineate Catchments";
            _btstartDelineate.Image = Properties.Resources.Delineation_icon_32;
            _btstartDelineate.SmallImage = Properties.Resources.Delineation_icon_32.GetThumbnailImage(16, 16, null, IntPtr.Zero);
            _btstartDelineate.CheckOnClick = true;
            _btstartDelineate.Click += new EventHandler(_startDelineate_Click);

            //Add it into the panel
            _rPanelEPADelineation.Items.Add(_btstartDelineate);

        }

        # endregion

        #region Click Events

        void _startDelineate_Click(object sender, EventArgs e)
        {

            if (_btstartDelineate.Checked == false)
            {
                _mapArgs.Map.Cursor = Cursors.Default;
            }

            else
            {
                //Check if any other Map Tools are checked
                for (int i = 0; i < _mapArgs.Ribbon.Tabs[0].Panels[1].Items.Count; i++)
                {
                    if (_mapArgs.Ribbon.Tabs[0].Panels[1].Items[i].Checked == true)
                    {
                        _mapArgs.Ribbon.Tabs[0].Panels[1].Items[i].Checked = false;
                        _mapArgs.Map.FunctionMode = FunctionMode.None;
                    }
                }
                _mapArgs.Map.FunctionMode = FunctionMode.None;

                //Active the Save Watershed Form
                SaveWatershed saveWS = new SaveWatershed(_mapArgs, _btstartDelineate);
                saveWS.ShowDialog();
            }
        }

        #endregion
    }
}
