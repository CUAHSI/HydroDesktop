using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using HydroDesktop.Database;
using DataImport;

//using HydroDesktop.Database.Model;
namespace ImportFromWaterML
{
    [Plugin("Import From WaterML", Author = "ISU", UniqueName = "mw_ImportFromWaterML_1", Version = "1")]
    public class Main : Extension, IMapPlugin
    {
        #region Variables

        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;

        //a sample menu item added by the plugin
        private ToolStripMenuItem mnu1 = null;
        private ToolStripMenuItem mnu2 = null;
        
        #endregion

        #region Ribbon Variables
        RibbonPanel _ribbonPanel = null;
        RibbonButton _btnWaterML = null;
        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {           
            if (_mapArgs.MainMenu != null && mnu1 != null)
            {
                _mapArgs.MainMenu.Items.Remove(mnu1);
            }

            //to remove the data import ribbon panel
            if (_mapArgs.Ribbon != null)
            {
                if (_ribbonPanel != null)
                {
                    if (_btnWaterML != null)
                    {
                        _ribbonPanel.Items.Remove(_btnWaterML);
                    }

                    if (_ribbonPanel.Items.Count == 0)
                    {
                        _mapArgs.Ribbon.Tabs[1].Panels.Remove(_ribbonPanel);
                    }
                }
            }
            base.OnDeactivate();
        }

        #endregion

        #region IPlugin Members

        /// <summary>
        /// Initialize the DotSpatial 6 plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public void Initialize(IMapPluginArgs args)
        {
            _mapArgs = args;

            //non-ribbon:
            if (_mapArgs.MainToolStrip != null)
            {

                //to add the menu
                mnu1 = new ToolStripMenuItem("Data Import");
                mnu2 = new ToolStripMenuItem("Import From WaterML");
                mnu1.DropDownItems.Add(mnu2);

                if (_mapArgs.MainMenu != null)
                {
                    _mapArgs.MainMenu.Items.Add(mnu1);
                    mnu2.Click += new EventHandler(menu_Click);
                }
            }
            //ribbon:
            else if (_mapArgs.Ribbon != null)
            {
                _ribbonPanel = new RibbonPanel("Data Import",RibbonPanelFlowDirection.Bottom);
				_ribbonPanel.ButtonMoreVisible = false;
                _mapArgs.Ribbon.Tabs[1].Panels.Add(_ribbonPanel);
                _btnWaterML = new RibbonButton("WaterML");
                _ribbonPanel.Items.Add(_btnWaterML);
                _btnWaterML.Image = Resources.waterml_import1;
                _btnWaterML.Click +=new EventHandler(menu_Click);
            }
        }

        #endregion

        #region Event Handlers

        void menu_Click(object sender, EventArgs e)
        {
            ImportDialog dlg = new ImportDialog();
            dlg.ShowDialog();
        }

        #endregion
    }
}
