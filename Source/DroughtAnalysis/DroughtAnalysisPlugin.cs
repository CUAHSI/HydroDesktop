using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Extensions;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using System.Windows.Forms;

namespace DroughtAnalysis
{
    public class DroughtAnalysisPlugin:Extension
    {
        const string droughtAnalysisPluginKey = "kDroughtAnalysis";
        const string droughtAnalysisPluginName = "Drought Analysis";

        private DroughtSettings UserSettings = new DroughtSettings();
        
        public override void Activate()
        {
            var droughtMenu = new MenuContainerItem(HeaderControl.HomeRootItemKey, droughtAnalysisPluginKey, "Drought Analysis");
            droughtMenu.GroupCaption = droughtAnalysisPluginName;
            droughtMenu.LargeImage = Properties.Resources.drought_fire_32;
            
            App.HeaderControl.Add(droughtMenu);

            var btnMeteoDrought = new SimpleActionItem(HeaderControl.HomeRootItemKey, droughtAnalysisPluginKey, "Meteorological Drought", btnMeteoDrought_Click);
            btnMeteoDrought.GroupCaption = droughtMenu.GroupCaption;
            App.HeaderControl.Add(btnMeteoDrought);

            var btnHydroDrought = new SimpleActionItem(HeaderControl.HomeRootItemKey, droughtAnalysisPluginKey, "Hydrological Drought", btnHydroDrought_Click);
            btnHydroDrought.GroupCaption = droughtMenu.GroupCaption;
            App.HeaderControl.Add(btnHydroDrought);
            
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        void btnMeteoDrought_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Meteorological Drought");
            StationFinder sf = new StationFinder(App.Map);
            var res = sf.FindSuitableStations();

            

            if (res.Count == 0)
                MessageBox.Show("No Sites with Temperature and precipitation found. Please download data first.");

            try
            {
                UserSettings.SuitableSites = res;
                SelectStationForm frm = new SelectStationForm(UserSettings);
                frm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        void btnHydroDrought_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This operation is not yet implemented");
        }
    }
}
