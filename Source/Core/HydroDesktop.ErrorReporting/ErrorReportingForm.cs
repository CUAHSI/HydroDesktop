using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using HydroDesktop.Common;
using HydroDesktop.Common.UserMessage;
using HydroDesktop.ErrorReporting.CodePlex;

namespace HydroDesktop.ErrorReporting
{
    /// <summary>
    /// Error Reporting form is used to show exceptions and send them to developers.
    /// </summary>
    public partial class ErrorReportingForm : Form
    {
        #region Fields

        private readonly ErrorReportingFormArgs _initParams;
        private static string _user;
        private static string _password;

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

            linkRegister.Links.Remove(linkRegister.Links[0]);
            linkRegister.Links.Add(0, linkRegister.Text.Length,
                                   "https://hydrodesktop.codeplex.com/site/register?associate=None&ProjectName=hydrodesktop");
            linkRegister.LinkClicked += delegate(object o, LinkLabelLinkClickedEventArgs ee)
                {
                    var sInfo = new ProcessStartInfo(ee.Link.LinkData.ToString());
                    Process.Start(sInfo);
                };

            tbLogin.Text = _user;
            tbPassword.Text = _password;
        }

        #endregion

        #region Private methods

        private void ErrorReportingForm_Shown(object sender, EventArgs e)
        {
            Text = string.Format("HydroDesktop Error - [{0}]",
                                 _initParams.IsFatal
                                     ? "Fatal error. Application will be closed."
                                     : "Unhandled Exception.");
            tbError.Text = _initParams.Exception != null ? _initParams.Exception.ToString() : null;
            EnableControls(true);
        }

        private void btnCopyError_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbError.Text);
        }

        private void btnZipLog_Click(object sender, EventArgs e)
        {
            var zipFileName = ZipLog();

            // Opens the folder in explorer and selects file
            Process.Start("explorer.exe", @"/select, " + zipFileName);
        }

        private string ZipLog()
        {
            if (String.IsNullOrEmpty(_initParams.LogFile) ||
               !File.Exists(_initParams.LogFile))
            {
                AppContext.Instance.Get<IUserMessage>().Warn("Log file not exists.");
                return null;
            }

            try
            {
                var logFile = _initParams.LogFile;

                // Zip file
                var zipFileName = Path.ChangeExtension(logFile, "zip");
                ZipHelper.AddFileToZip(zipFileName, logFile);
                return zipFileName;
            }
            catch (Exception ex)
            {
                AppContext.Instance.Get<IUserMessage>().Error("Unable to zip log file.", ex);
                return null;
            }
        }

        private void btnSendError_Click(object sender, EventArgs e)
        {
            // Validation
            if (!ValidateLogin())
            {
                tbLogin.Focus();
                return;
            }
            if (!ValidatePassword())
            {
                tbPassword.Focus();
                return;
            }

            // Send error
            EnableControls(false);
            var bworker = new BackgroundWorker();
            bworker.DoWork += delegate(object sender1, DoWorkEventArgs e1)
                {
                    var logFile = ZipLog();
                    var issueTracker = new IssueTracker("hydrodesktop");
                    issueTracker.SignIn(tbLogin.Text, tbPassword.Text);

                    var issue = new Issue
                        {
                            Summary = _initParams.IsFatal ? "Fatal Error" : "Unhandled Exception",
                            Description = "Version: " + AppContext.Instance.ProductVersion + Environment.NewLine +
                                          "Description: " + tbDescribe.Text + Environment.NewLine +
                                          "==============================" + Environment.NewLine +
                                          "Error: " + tbError.Text,
                        };

                    if (!String.IsNullOrEmpty(logFile) && File.Exists(logFile))
                    {
                        try
                        {
                            var fi = new FileInfo(logFile);
                            issue.FileToAttach = fi;
                        }
                        catch (Exception ex)
                        {
                            AppContext.Instance.Get<IUserMessage>().Warn("Unable to attach log file.", ex);
                        }
                    }

                    e1.Result = issueTracker.CreateIssue(issue);
                };
            bworker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args)
                {
                    EnableControls(true);
                    if (args.Error != null)
                    {
                        AppContext.Instance.Get<IUserMessage>().Error("Unable to send error.", args.Error);   
                    }else
                    {
                        AppContext.Instance.Get<IUserMessage>().Info("Error was sent." + Environment.NewLine +
                                                                      "Link to issue: " + args.Result);
                        _user = tbLogin.Text;
                        _password = tbPassword.Text;
                        Close();
                    }
                };
            bworker.RunWorkerAsync();
        }

        private void EnableControls(bool enable)
        {
            paMain.Enabled = enable;
            btnCopyError.Enabled = enable;
            btnZipLog.Enabled = enable;
            btnSendError.Enabled = enable;
            paProgress.Visible = !enable;
        }

        private bool ValidateLogin()
        {
            var error = String.IsNullOrWhiteSpace(tbLogin.Text) ? "Login should be not empty." : string.Empty;
            errorProvider1.SetError(tbLogin, error);
            return error == string.Empty;
        }

        private bool ValidatePassword()
        {
            var error = String.IsNullOrWhiteSpace(tbPassword.Text) ? "Password should be not empty." : string.Empty;
            errorProvider1.SetError(tbPassword, error);
            return error == string.Empty;
        }

        private void tbLogin_Validating(object sender, CancelEventArgs e)
        {
            ValidateLogin();
        }

        private void tbPassword_Validating(object sender, CancelEventArgs e)
        {
            ValidatePassword();
        }

        #endregion
    }
}
