using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Search3.Searching
{
    public partial class SearchProgressForm : Form
    {
        #region Fields

        private readonly Searcher _searcher;

        #endregion

        #region Constructors

        public SearchProgressForm(Searcher searcher)
        {
            if (searcher == null) throw new ArgumentNullException("searcher");
            _searcher = searcher;

            InitializeComponent();

            SubscribeToSearcherEvents();
            Disposed += SearchProgressForm_Disposed;
        }

        #endregion

        #region Private methods

        void SearchProgressForm_Disposed(object sender, EventArgs e)
        {
            UnSubscribeToSearcherEvents();
        }

        private void SubscribeToSearcherEvents()
        {
            _searcher.Completed += searcher_Completed;
            _searcher.OnMessage += searcher_OnMessage;
            _searcher.ProgressChanged += searcher_ProgressChanged;
        }

        private void UnSubscribeToSearcherEvents()
        {
            _searcher.Completed -= searcher_Completed;
            _searcher.OnMessage -= searcher_OnMessage;
            _searcher.ProgressChanged -= searcher_ProgressChanged;
        }

        void searcher_Completed(object sender, CompletedEventArgs e)
        {
            UnSubscribeToSearcherEvents();

            if (!_searcher.IsUIVisible)
            {
                _searcher.ShowUI();
            }
            btnCancel.Enabled = false;
            MessageBox.Show("Search finished!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void searcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // todo: Refactor next line (e.UserState may be not series number)
            ThreadSafeSetText(lbCurrentOperation, "Searching.. " + Convert.ToString(e.UserState) + " Series Found");
            ThreadSafeChangeProgressBarValue(progressBar1, e.ProgressPercentage);
        }

        void searcher_OnMessage(object sender, LogMessageEventArgs e)
        {
            // todo: Show message log in the UI
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_searcher.IsBusy)
            {
                if (MessageBox.Show("Abort search?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    _searcher.Cancel();
                }
            }
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            _searcher.HideUI();
        }

        private static void ThreadSafeChangeProgressBarValue(ProgressBar pb, int value)
        {
            if (pb.InvokeRequired)
            {
                pb.BeginInvoke((Action<ProgressBar, int>)ChangeProgressBarValue, pb, value);
            }
            else
                ChangeProgressBarValue(pb, value);
        }
        private static void ChangeProgressBarValue(ProgressBar pb, int value)
        {
            pb.Value = value;
        }

        private static void ThreadSafeSetText(Label label, string value)
        {
            if (label.InvokeRequired)
            {
                label.BeginInvoke((Action<Label, string>)SetTextToLabel, label, value);
            }
            else
                SetTextToLabel(label, value);
        }
        private static void SetTextToLabel(Label label, string value)
        {
            label.Text = value;
        }

        #endregion
    }
}
