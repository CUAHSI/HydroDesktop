using System;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;

namespace EPADelineation
{
    /// <summary>
    /// EPA Delineation extension for HydroDesktop
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

            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }


        /// <summary>
        /// Initialize the DotSpatial plug-in
        /// </summary>
        public override void  Activate()
        {
            action = new SimpleActionItem("Delineate", _startDelineate_Click);
            action.GroupCaption = "EPA Tool";
            action.ToolTipText = "Using EPA Web Services to Delineate Catchments";
            action.SmallImage = Properties.Resources.Delineation_icon_32.GetThumbnailImage(16, 16, null, IntPtr.Zero);
            action.LargeImage = Properties.Resources.Delineation_icon_32;
            action.RootKey = HeaderControl.HomeRootItemKey;
            action.ToggleGroupKey = "tDelineateEpaTool";
            App.HeaderControl.Add(action);

            base.Activate();
        }

        # endregion

        #region Click Events

        private void _startDelineate_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.None;
            
            var saveWS = new SaveWatershed(App);
            var res = saveWS.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                saveWS.Completed += saveWS_Completed;
            }
            else
            {
                App.Map.FunctionMode = FunctionMode.Select;
            }
        }

        void saveWS_Completed(object sender, EventArgs e)
        {
            var saveWS = (SaveWatershed) sender;
            saveWS.Completed -= saveWS_Completed;
            App.Map.FunctionMode = FunctionMode.Select;
        }

        #endregion Click Events
    }
}