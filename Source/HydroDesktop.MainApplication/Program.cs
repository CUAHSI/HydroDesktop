using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using HydroDesktop.Common;
using HydroDesktop.Common.Logging;

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

            var log = AppContext.Instance.Get<ILog>();
            Application.ApplicationExit += delegate
                {
                    log.Info("Application Exit");
                    Trace.Flush(); // todo: move this into TraceLogger.Dispose() and check that Unity releases all registered services.
                };
            // Log all unhandled exceptions
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => ProcessUnhandled(e.Exception, false);
            AppDomain.CurrentDomain.UnhandledException +=
                    delegate(object sender, UnhandledExceptionEventArgs e)
                        {
                            ProcessUnhandled((Exception)e.ExceptionObject, true);
                            Trace.Flush();
                        };
            
            log.Info("Application started");
            var form = new MainForm();
            if (args.Length > 0)
                if (System.IO.File.Exists(args[0]))
                    form.appManager.SerializationManager.OpenProject(args[0]);

            Application.Run(form);
        }

        private static void ProcessUnhandled(Exception ex, bool isFatal)
        {
            var log = AppContext.Instance.Get<ILog>();
            log.Error(isFatal? "fatal" : "unhandled", ex);
            var caption = isFatal ? "Fatal error" : "Unhandled Exception";
            var message = "Message: " + ex.Message + Environment.NewLine + "Details saved to: " + log.Destination;
            if (isFatal)
            {
                message += Environment.NewLine + "Application will be closed.";
            }

            // todo: show custom form to allow user submit bug into codeplex
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

