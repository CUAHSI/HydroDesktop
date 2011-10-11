using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HydroDesktop.Main
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //disable eurekaLog in development version
            //EurekaLogSystem.ExceptionHandler.Activate();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Uncomment next line to run the new ribbon version
            if (IsRunningOnMono())
            {
                //running on Mac or Linux
                MessageBox.Show("The Mac or Linux platform is not supported in this version of HydroDesktop.");
            }
            else
            {
                Application.Run(new mainRibbonForm(args)); 
            }
        }

        //to test if we are running on mono
        public static bool IsRunningOnMono()
        {
            return (Type.GetType("Mono.Runtime") != null);
        }
    }
}
