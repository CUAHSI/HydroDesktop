using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HydroDesktop.MainApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm form = new MainForm();

            if (args.Length > 0)
                if (System.IO.File.Exists(args[0]))
                    form.appManager.SerializationManager.OpenProject(args[0]);

            Application.Run(form);
        }
    }
}

