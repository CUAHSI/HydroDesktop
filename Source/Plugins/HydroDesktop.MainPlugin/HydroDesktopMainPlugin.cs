namespace HydroDesktop.MainPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using System.ComponentModel.Composition;
    using DotSpatial.Controls.Docking;

    public class SeriesViewPlugin : Extension
    {
        //public const string SeriesViewKey = "kHydroSeriesView";

        private WelcomeScreen _welcomeScreen;
        private bool _isNewProject = false;
        
        public override void Activate()
        {
            //App.DockManager.Add(new DockablePanel(SeriesViewKey, "time series", (SeriesSelector)MainSeriesSelector, DockStyle.Left));

            ShowWelcomeScreen();

            base.Activate();
        }

        public override void  Deactivate()
        {
 	        //App.HeaderControl.RemoveAll();
            //App.DockManager.Remove(SeriesViewKey);
            
            base.Deactivate();
        }


        private void ShowWelcomeScreen()
        {
            _welcomeScreen = new WelcomeScreen(App);
            _welcomeScreen.StartPosition = FormStartPosition.Manual;

            //int x = this.Location.X + this.Width / 2 - _welcomeScreen.Width / 2;
            //int y = this.Location.Y + this.Height / 2 - _welcomeScreen.Height / 2;
            //_welcomeScreen.Location = new System.Drawing.Point(x, y);

            if (_welcomeScreen.ShowDialog() == DialogResult.OK)
            {
                bool _isNewProject = _welcomeScreen.NewProjectCreated;
            }         
        }
    }
}
