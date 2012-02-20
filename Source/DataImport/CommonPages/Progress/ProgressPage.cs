using System.ComponentModel;
using HydroDesktop.Common;
using HydroDesktop.Common.Tools;
using Wizard.UI;

namespace DataImport.CommonPages.Progress
{
    public partial class ProgressPage : InternalWizardPage, IProgressHandler
    {
        #region Fields

        private readonly WizardContext _context;
        private BackgroundWorker _backgroundWorker;

        #endregion

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
            _backgroundWorker.RunWorkerCompleted += delegate
                                         {
                                             SetWizardButtons(WizardButtons.Next);
                                             PressButton(WizardButtons.Next);
                                         };
            _backgroundWorker.DoWork += delegate
                                            {
                                                var ph = (IProgressHandler) this;
                                                ph.ReportMessage("Reading all data into DataTable...");
                                                _context.Importer.UpdateData(_context.Settings);
                                                var importer = _context.Importer.GetImporter();
                                                importer.ProgressHandler = ph;
                                                importer.Import(_context.Settings);
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
