using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using HydroDesktop.Common;
using HydroDesktop.Common.UserMessage;

namespace HydroDesktop.ErrorReporting
{
    /// <summary>
    /// Error Reporting form is used to show exceptions and send them to developers.
    /// </summary>
    public partial class ErrorReportingForm : Form
    {
        #region Fields

        private readonly ErrorReportingFormArgs _initParams;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of <see cref="ErrorReportingForm"/>
        /// </summary>
        /// <paramref name="initParams">Init params.</paramref>>
        public ErrorReportingForm(ErrorReportingFormArgs initParams)
        {
            if (initParams == null) throw new ArgumentNullException("initParams");

            _initParams = initParams;
            InitializeComponent();
        }

        #endregion

        #region Private methods

        private void ErrorReportingForm_Shown(object sender, EventArgs e)
        {
            hdLink.Links.Remove(hdLink.Links[0]);
            hdLink.Links.Add(0, hdLink.Text.Length, "http://hydrodesktop.codeplex.com/WorkItem/Create");
            hdLink.LinkClicked += delegate(object o, LinkLabelLinkClickedEventArgs ee)
            {
                var sInfo = new ProcessStartInfo(ee.Link.LinkData.ToString());
                Process.Start(sInfo);
            };

            lblInfo.Text = string.Format("HydroDesktop has encountered an {0}" + Environment.NewLine + "We are sorry for the inconvenience.",
                _initParams.IsFatal ? "Fatal error. Application will be closed." :
                "Unhandled Exception.");
            tbError.Text = _initParams.Exception != null ? _initParams.Exception.ToString() : null;
        }

        private void btnCopyError_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbError.Text);
        }

        private void btnZipLog_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_initParams.LogFile) ||
                !File.Exists(_initParams.LogFile))
            {
                AppContext.Instance.Get<IUserMessage>().Warn("Log file not exists.");
                return;
            }

            try
            {
                var logFile = _initParams.LogFile;

                // Zip file
                var zipFileName = Path.ChangeExtension(logFile, "zip");
                ZipHelper.AddFileToZip(zipFileName, logFile);

                // Opens the folder in explorer and selects file
                Process.Start("explorer.exe", @"/select, " + zipFileName);
                 
            }catch(Exception ex)
            {
                AppContext.Instance.Get<IUserMessage>().Error("Unable to zip log file.", ex);
            }
        }

        #endregion
    }
}
