using System;
using System.Windows.Forms;
using HydroDesktop.Common;
using HydroDesktop.Common.Logging;
using HydroDesktop.ErrorReporting;
using System.IO;
using System.Reflection;

namespace HydroDesktop.MainApplication
{
    static class Program
    {
        private static MainForm _mainForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Hack described in https://hydrodesktop.codeplex.com/workitem/8676
            AppDomain.CurrentDomain.AssemblyResolve += LoadAssembly;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var log = HydroDesktop.Common.AppContext.Instance.Get<ILog>();
            Application.ApplicationExit += delegate
                {
                    log.Info("Application Exit");
                    HydroDesktop.Common.AppContext.Instance.Dispose();
                };
            // Log all unhandled exceptions
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => ProcessUnhandled(e.Exception, false);
            AppDomain.CurrentDomain.UnhandledException +=
                    delegate(object sender, UnhandledExceptionEventArgs e)
                        {
                            ProcessUnhandled((Exception)e.ExceptionObject, true);
                            HydroDesktop.Common.AppContext.Instance.Dispose();
                        };

            log.Info("Application Started");
            _mainForm = new MainForm();
            if (args.Length > 0 && System.IO.File.Exists(args[0]))
            {
                _mainForm.appManager.SerializationManager.OpenProject(args[0]);
            }

            Application.Run(_mainForm);
        }

        private static void ProcessUnhandled(Exception ex, bool isFatal)
        {
            var log = HydroDesktop.Common.AppContext.Instance.Get<ILog>();
            log.Error(isFatal ? "Fatal" : "Unhandled", ex);
            var errorForm = new ErrorReportingForm(new ErrorReportingFormArgs
                {
                    Exception = ex,
                    IsFatal = isFatal,
                    LogFile = log.Destination
                });
            errorForm.ShowDialog(_mainForm);
        }


        //This method is used to load the correct System.Data.SQlite dlls.
        //See HydroDesktop Issue 8676 for more information: https://hydrodesktop.codeplex.com/workitem/8676
        static Assembly LoadAssembly(object sender, ResolveEventArgs args)
        {
            //If this isn't a SQLite DLL we don't want/need to execute this code.
            if (!args.Name.Contains("SQLite")) { return null; }

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Support", DotSpatial.Mono.Mono.IsRunningOnMono() ? "Mono" : "Windows");
            var assemblyPath = Path.Combine(filePath, new AssemblyName(args.Name).Name + ".dll");
            return !File.Exists(assemblyPath) ? null : Assembly.LoadFrom(assemblyPath);
        }
    }
}

