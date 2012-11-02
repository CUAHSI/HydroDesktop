using System;
using System.Windows.Forms;
using HydroDesktop.Common;
using HydroDesktop.Common.Logging;
using HydroDesktop.ErrorReporting;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var log = AppContext.Instance.Get<ILog>();
            Application.ApplicationExit += delegate
                {
                    log.Info("Application Exit");
                    AppContext.Instance.Dispose();
                };
            // Log all unhandled exceptions
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => ProcessUnhandled(e.Exception, false);
            AppDomain.CurrentDomain.UnhandledException +=
                    delegate(object sender, UnhandledExceptionEventArgs e)
                        {
                            ProcessUnhandled((Exception)e.ExceptionObject, true);
                            AppContext.Instance.Dispose();
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
            var log = AppContext.Instance.Get<ILog>();
            log.Error(isFatal ? "Fatal" : "Unhandled", ex);
            var errorForm = new ErrorReportingForm(new ErrorReportingFormArgs
                {
                    Exception = ex,
                    IsFatal = isFatal,
                    LogFile = log.Destination
                });
            errorForm.ShowDialog(_mainForm);
        }
    }
}

