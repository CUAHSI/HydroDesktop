using System.Collections.Generic;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;

namespace DataImport.CommonPages.FieldProperties
{
    public partial class SelectColumnForm : Form
    {
        public SelectColumnForm(IList<ColumnInfo> columns)
        {
            InitializeComponent();

            cmbColumns.DisplayMember = NameHelper.Name<ColumnInfo, string>(x => x.ColumnName);
            cmbColumns.DataSource = columns;
        }

        public ColumnInfo SelectedItem
        {
            get { return (ColumnInfo)cmbColumns.SelectedItem; }
        }
    }
}
