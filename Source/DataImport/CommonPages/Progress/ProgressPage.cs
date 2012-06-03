using System.Collections.Generic;
using System.ComponentModel;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;
using Wizard.UI;
using IProgressHandler = HydroDesktop.Common.IProgressHandler;

namespace DataImport.CommonPages.Progress
{
    /// <summary>
    /// Progress page
    /// </summary>
    public partial class ProgressPage : InternalWizardPage, IProgressHandler
    {
        #region Fields

        private readonly WizardContext _context;
        private BackgroundWorker _backgroundWorker;

        #endregion

        /// <summary>
        /// Create new instance of <see cref="ProgressPage"/>
        /// </summary>
        /// <param name="context"></param>
        public ProgressPage(WizardContext context)
        {
            _context = context;
            InitializeComponent();
            lblInfo.Text = string.Empty;
        }

        private void ProgressPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.None);

            _backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true, WorkerReportsProgress = true};
            _backgroundWorker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                                         {
                                             AfterDataImportsHelper.CreateLayer((IList<Series>)args.Result, this, _context.Settings);

                                             SetWizardButtons(WizardButtons.Next);
                                             PressButton(WizardButtons.Next);
                                         };
            _backgroundWorker.DoWork += delegate(object s, DoWorkEventArgs args)
                                            {
                                                var ph = (IProgressHandler) this;
                                                ph.ReportMessage("Reading all data into DataTable...");
                                                _context.Importer.UpdateData(_context.Settings);
                                                _context.Settings.MaxProgressPercentWhenImport = 97;
                                                var importer = _context.Importer.GetImporter();
                                                importer.ProgressHandler = ph;
                                                var result = importer.Import(_context.Settings);

                                                // Some work need in the UI thread
                                                args.Result = result;
                                            };
            
            _backgroundWorker.RunWorkerAsync();
        }

        #region IProgressHandler implementation

        public void ReportProgress(int persentage, object state)
        {
            progressBar.UIThread(() => progressBar.Value = persentage);
            lblInfo.UIThread(() => lblInfo.Text = state != null ? state.ToString() : string.Empty);
        }

        public void CheckForCancel()
        {
            var bw = _backgroundWorker;
            if (bw != null && bw.WorkerSupportsCancellation)
            {
                bw.CancelAsync();
            }
        }

        public void ReportMessage(string message)
        {
            lblInfo.UIThread(() => lblInfo.Text = message);
        }

        #endregion
    }
}
