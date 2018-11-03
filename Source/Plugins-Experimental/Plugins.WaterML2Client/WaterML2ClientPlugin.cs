using System;
using System.ComponentModel.Composition;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Plugins.WaterML2Client
{
    public class WaterML2ClientPlugin : Extension
    {
        /// <summary>
        /// Series View
        /// </summary>
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; private set; }

        public override void Activate()
        {
            base.Activate();

            var head = App.HeaderControl;
            var action = new SimpleActionItem("WaterML2",
                delegate
                {
                    new WMLDownloadForm(SeriesControl).ShowDialog();
                })
            {RootKey = SharedConstants.SearchRootkey, SortOrder = 100};
            head.Add(action);
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }
    }
}
