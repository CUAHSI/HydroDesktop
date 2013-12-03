using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using System.Windows.Forms;

namespace Aggregation_Plugin
{
    /// <summary>
    /// HelloTim for HydroDesktop
    /// </summary>
    public class Main : Extension
    {
        #region Variables
        private SimpleActionItem action;

        #endregion Variables

        #region IExtension Members

        /// <summary>
        /// Fires when the plug-in should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }


        /// <summary>
        /// Initialize the DotSpatial plug-in
        /// </summary>
        public override void Activate()
        {
            action = new SimpleActionItem("CRWR\nAggregation", CRWR_Click);
            action.GroupCaption = "";
            action.ToolTipText = "Aggregate point data in a polygon";
            action.SmallImage = Properties.Resources.CRWR_Logo.GetThumbnailImage(16, 16, null, IntPtr.Zero);
            action.LargeImage = Properties.Resources.CRWR_Logo;
            action.RootKey = HeaderControl.HomeRootItemKey;
            action.ToggleGroupKey = "Hello Tim test";
            action.SortOrder = 120; //give it a high sort order to move the button to the right
            App.HeaderControl.Add(action);

            base.Activate();
        }

        # endregion

        #region Click Events

        private void CRWR_Click(object sender, EventArgs e)
        {
            
            var parFM = new Parameters_form(App);
            if (parFM.Visible == false)
            {
                parFM.Show(App.Map.MapFrame != null ? App.Map.MapFrame.Parent : null);
            }
            //var res = parFM.ShowDialog();
            App.Map.FunctionMode = FunctionMode.Select;

        }

        #endregion Click Events
    }
}
