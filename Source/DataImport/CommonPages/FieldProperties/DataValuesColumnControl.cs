using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataImport.CommonPages.FieldProperties
{
    public partial class DataValuesColumnControl : UserControl
    {
        private bool _isLastColumn;

        public event EventHandler MoreColumnsClicked;
        public event EventHandler DetailsClicked;
        public event EventHandler ColumnChanged;

        public DataValuesColumnControl()
        {
            InitializeComponent();

            cmbColumns.SelectedIndexChanged += delegate
                                                   {
                                                       var handler = ColumnChanged;
                                                       if (handler != null)
                                                       {
                                                           handler(this, EventArgs.Empty);
                                                       }
                                                   };

            IsLastColumn = true;
        }

        public void SetColumns(IList<string> columns)
        {
            var currentColumn = SelectedColumn;

            cmbColumns.DataSource = columns;

            if (currentColumn != null && 
                columns.Contains(currentColumn))
            {
                cmbColumns.SelectedItem = currentColumn;
            }
        }

        public string SelectedColumn
        {
            get { return (string)cmbColumns.SelectedItem; }
            set { cmbColumns.SelectedItem = value; }
        }

        public bool IsLastColumn
        {
            get { return _isLastColumn; }
            set 
            { 
                _isLastColumn = value;
                btnMore.Visible = _isLastColumn;
            }
        }

        public void SetColumnNumber(int number)
        {
            lblColumnCount.Text = "Specify Data values column" + (number > 0 ? string.Format(" {0}", number) : string.Empty);
        }

        #region Private methods

        private void btnMore_Click(object sender, EventArgs e)
        {
            var handler = MoreColumnsClicked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            var handler = DetailsClicked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
