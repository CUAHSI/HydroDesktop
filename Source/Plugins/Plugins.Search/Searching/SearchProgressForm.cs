using System;
using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;

namespace HydroDesktop.Plugins.Search.Searching
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
            _searcher.OnMessage += searcher_OnMessage;
            _searcher.ProgressChanged += searcher_ProgressChanged;
        }

        private void UnSubscribeToSearcherEvents()
        {
            _searcher.OnMessage -= searcher_OnMessage;
            _searcher.ProgressChanged -= searcher_ProgressChanged;
        }

        public void DoSearchFinished(CompletedEventArgs e)
        {
            UnSubscribeToSearcherEvents();


            if (!_searcher.IsUIVisible && !checkBox1.Checked)
            {
                _searcher.ShowUI();
            }

            if (checkBox1.Checked)
            {
                _searcher.HideUI();
            }

            btnCloseCancel.Text = "Close";
            btnCloseCancel.Enabled = true;
            btnCloseCancel.Click -= btnCancel_Click;
            btnCloseCancel.Click += btnHide_Click;
            checkBox1.Click += checkBox1_CheckedChanged; 
           // Text = "Search Finished";

            string message;
            switch (e.Reason)
            {
                case CompletedReasones.Cancelled:
                    message = "Search has been canceled successfully.";
                    break;
                case CompletedReasones.Faulted:
                    message = "Search has been faulted due to an unhandled exception.";
                    break;
                case CompletedReasones.NormalCompleted:
                    message = e.Result == null || e.Result.IsEmpty()
                                  ? "No results were found. Please change the search criteria."
                                  : "Search finished successfully.";
                    break;
                default:
                    message = "Search was finished for unknown reason.";
                    break;
            }

            searcher_OnMessage(_searcher, new LogMessageEventArgs(message));
        }
      
        void searcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ThreadSafeSetText(lbCurrentOperation, Convert.ToString(e.UserState));
            ThreadSafeChangeProgressBarValue(progressBar1, e.ProgressPercentage);
        }

        void searcher_OnMessage(object sender, LogMessageEventArgs e)
        {
            var split = e.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            split[0] = DateTime.Now.ToLongTimeString() + " " + split[0];
            foreach (var mes in split)
                ThreadSafeAddItemToLog(lbOutput, mes);

            if (e.Exception != null)
            {
                var message = string.Format("Exception details:" + Environment.NewLine +
                                            "Message: {0}" + Environment.NewLine +
                                            "Stacktrace: {1}", e.Exception.Message,
                                            e.Exception.StackTrace);
                split = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var mes in split)
                    ThreadSafeAddItemToLog(lbOutput, mes);

                if (e.Exception.InnerException != null)
                    ThreadSafeAddItemToLog(lbOutput, "Inner exception: " + e.Exception.InnerException.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_searcher.IsBusy)
            {
                if (MessageBox.Show("Are you sure you want to cancel search?", "Cancel search", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) ==
                    DialogResult.OK)
                {
                    _searcher.Cancel();
                    btnCloseCancel.Enabled = false;
                }
            }
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            _searcher.HideUI();
        }

        private void ThreadSafeAddItemToLog(ListBox listBox, object value)
        {
            listBox.UIThread(delegate
                                 {
                                     listBox.Items.Add(value);

                                     // scroll to last item
                                     listBox.SelectedIndex = listBox.Items.Count - 1;
                                 });
        }
        
        private static void ThreadSafeChangeProgressBarValue(ProgressBar pb, int value)
        {
            pb.UIThread(() => pb.Value = value);
        }

        private static void ThreadSafeSetText(Control label, string value)
        {
            label.UIThread(() => label.Text = value);
        }

        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
          if(checkBox1.Checked)
             _searcher.HideUI();
        }
    }
}
