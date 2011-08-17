using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using System.Windows.Forms;

namespace Toolbox
{
    [Plugin("Toolbox", Author = "MapWindow", UniqueName = "mw_Toolbox_1", Version = "1")]
    public class Main: Extension, IMapPlugin
    {
        private IMapPluginArgs _mapWin;
        private string _panelName = "Toolbox";

        #region Ribbon
        //to add the 'Toolbox' ribbon button
        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            //remove the added tab or panel
            try
            {
                _mapWin.PanelManager.RemoveTab(_panelName);
            }
            catch { }

            //necessary line for plugin deactivation
            base.OnDeactivate();
        }

        #endregion

        #region IPlugin Members

        /// <summary>
        /// Initialize the mapWindow 6 plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public void Initialize(IMapPluginArgs args)
        {
            _mapWin = args;

            if (_mapWin.ToolStripContainer != null)
            {
                // Add the 'Added by plugin' tab control
                cToolbox uc = new cToolbox();
                uc.Dock = DockStyle.Fill;
                uc.ToolManager.Legend = _mapWin.Legend;
                _mapWin.PanelManager.AddTab(_panelName,uc );
            }
        }

        #endregion
    }
}
