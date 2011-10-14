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

            searcher.OnMessage += searcher_OnMessage;
            searcher.ProgressChanged += searcher_ProgressChanged;
        }

        void searcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbCurrentOperation.Text = string.Format("Comleted {0} %", e.UserState);
            progressBar1.Value = e.ProgressPercentage;
        }

        void searcher_OnMessage(object sender, LogMessageEventArgs e)
        {
            
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
