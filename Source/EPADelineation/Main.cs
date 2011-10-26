using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace EPADelineation
{
    /// <summary>
    /// EPA Delineation extension for HydroDesktop
    /// </summary>
    public class Main : Extension
    {
        #region Variables
        private bool isActive;

        #endregion Variables

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }


        /// <summary>
        /// Initialize the DotSpatial plugin
        /// </summary>
        public override void  Activate()
        {
            SimpleActionItem action = new SimpleActionItem("Delineate", _startDelineate_Click);
            action.GroupCaption = "EPA Tool";
            action.ToolTipText = "Using EPA Web Services to Delineate Catchments";
            action.SmallImage = Properties.Resources.Delineation_icon_32.GetThumbnailImage(16, 16, null, IntPtr.Zero);
            action.LargeImage = Properties.Resources.Delineation_icon_32;
            action.RootKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;
            action.ToggleGroupKey = "tDelineateEpaTool";
            App.HeaderControl.Add(action);

            base.Activate();
        }

        # endregion

        #region Click Events

        private void _startDelineate_Click(object sender, EventArgs e)
        {
            if (isActive)
            {
                isActive = false;
                App.Map.Cursor = Cursors.Default;
            }
            else
            {
                isActive = true;
                //Check if any other Map Tools are checked
                App.Map.FunctionMode = FunctionMode.None;

                //Active the Save Watershed Form
                SaveWatershed saveWS = new SaveWatershed(App);
                saveWS.ShowDialog();
            }
        }

        #endregion Click Events
    }
}