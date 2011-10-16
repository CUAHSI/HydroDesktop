using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Search3.Searching
{
    public partial class SearchProgressForm : Form
    {
        private readonly Searcher _searcher;

        public SearchProgressForm(Searcher searcher)
        {
            if (searcher == null) throw new ArgumentNullException("searcher");
            _searcher = searcher;

            InitializeComponent();

            searcher.Completed += searcher_Completed;
            searcher.OnMessage += searcher_OnMessage;
            searcher.ProgressChanged += searcher_ProgressChanged;
        }

        void searcher_Completed(object sender, CompletedEventArgs e)
        {
            btnCancel.Enabled = false;
        }

        void searcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // todo: Refactor next line (e.UserState may be not series number)
            lbCurrentOperation.Text = "Searching.. " + Convert.ToString(e.UserState) + " Series Found";
            progressBar1.Value = e.ProgressPercentage;
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
    }
}
