using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Database;

namespace HydroDesktop.Controls
{
    public partial class frmComplexSelection : Form
    {
        // Fields
        private Button btnCommit;

        //properties
        public string FilterExpression
        {
            get 
            {
                return sqlQueryControl2.ExpressionText;          
            }
            set 
            {
                sqlQueryControl2.ExpressionText = value;
            }
        }

        // Methods
        public frmComplexSelection(DataTable allSeriesTable)
        {
            this.InitializeComponent();
            this.sqlQueryControl2.Table = allSeriesTable;
        }

        //events
        public event EventHandler FilterCommitted;
        
        private void frmComplexSelection_Load(object sender, EventArgs e)
        {
            this.sqlQueryControl2.ExpressionText = "";
            this.sqlQueryControl2.Location = new Point(3, 12);
            this.sqlQueryControl2.Name = "sqlQueryControl1";
            this.sqlQueryControl2.Size = new Size(0x145, 0x152);
            this.sqlQueryControl2.TabIndex = 0;
        }

        private void btnCommit_Click_1(object sender, EventArgs e)
        {
            OnFilterCommitted();
            DialogResult = DialogResult.OK;
        }

        protected void OnFilterCommitted()
        {
            if (FilterCommitted != null) FilterCommitted(this, null);
        }
    }
}
