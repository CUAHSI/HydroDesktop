using System;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;

namespace HydroDesktop.Plugins.WaterML2Client
{
    public class WaterML2ClientPlugin : Extension
    {
        public override void Activate()
        {
            base.Activate();

            var head = App.HeaderControl;
            var action = new SimpleActionItem("WaterML2", delegate {
                new WMLDownloadForm().ShowDialog();
            }) {RootKey = SharedConstants.SearchRootkey, SortOrder = 100};
            head.Add(action);
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }
    }
}
