// -----------------------------------------------------------------------
// <copyright file="MainPlugin.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace HydroDesktop.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using System.Windows.Forms;

    [System.AddIn.AddIn("HydroDesktop.Core", Description = "Shows a welcome screen.")]
    public class Main : Extension, IMapPlugin
    {
        public void Initialize(IMapPluginArgs args)
        {
            //args.AppManager.HeaderControl.Add(new SimpleActionItem("MyButton", ButtonClick));


            if (IsRunningOnMono())
            {
                //running on Mac or Linux
                MessageBox.Show("Neither the Mac or Linux platform is supported in this version of HydroDesktop.");
            }
            else
            {
                ShowWelcomeScreen(args.AppManager);
            }
        }
        public static void ShowWelcomeScreen(AppManager app)
        {
            WelcomeScreen form = new WelcomeScreen(app);
            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog() == DialogResult.OK)
            {
                bool isNewProject = form.NewProjectCreated;
            }
            //reset the progress handler
            //app.ProgressHandler = mwStatusStrip1;
        }

        //to test if we are running on mono
        public static bool IsRunningOnMono()
        {
            return (Type.GetType("Mono.Runtime") != null);
        }
    }
}
