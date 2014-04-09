using System;
using System.Data;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.SeriesView
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
            InitializeComponent();
            sqlQueryControl2.Table = allSeriesTable;
        }

        //events

        private void btnCommit_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
