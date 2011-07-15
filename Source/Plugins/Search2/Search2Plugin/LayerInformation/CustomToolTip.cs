using System.Windows.Forms;

namespace HydroDesktop.Search.LayerInformation
{
    class CustomToolTip : ToolTip
    {
        public CustomToolTip()
        {
            // Default properties
            IsBalloon = true;
            UseAnimation = false;
            ShowAlways = false;
            InitialDelay = 0;
            AutoPopDelay = 5000;

            //
            Popup += ToolTipEx_Popup;
            timer.Interval = AutoPopDelay;
            timer.Elapsed += timer_Elapsed;
        }

        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private bool isVisible;

        void timer_Elapsed ( object sender, System.Timers.ElapsedEventArgs e )
        {
            timer.Stop ( );
            isVisible = false;
        }

        void ToolTipEx_Popup ( object sender, PopupEventArgs e )
        {
            isVisible = true;
            timer.Start ( );
        }

        public bool IsVisible
        {
            get { return isVisible; }
        }
        
    }
}
