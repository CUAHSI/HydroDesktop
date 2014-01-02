using System;

using DotSpatial.Extensions.SplashScreens;

namespace HydroDesktop.SplashScreenManager
{
    public class SplashScreenPlugin : ISplashScreenManager
    {

        public void ProcessCommand(Enum cmd, object arg)
        {
            SplashScreen.UdpateStatusText(arg.ToString());
        }

        public void Activate()
        {
            SplashScreen.ShowSplashScreen();
           
        }

        public void Deactivate()
        {
            SplashScreen.CloseSplashScreen();
          
        }
    }
}
