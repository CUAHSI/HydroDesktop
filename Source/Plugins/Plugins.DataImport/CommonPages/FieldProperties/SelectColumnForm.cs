using System.Collections.Generic;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;

namespace HydroDesktop.Plugins.DataImport.CommonPages.FieldProperties
{
    /// <summary>
    /// Form that allow to specify column to import
    /// </summary>
    public partial class SelectColumnForm : Form
    {
        /// <summary>
        /// Create new instance of <see cref="SelectColumnForm"/>
        /// </summary>
        /// <param name="columns">List of available columns</param>
        public SelectColumnForm(IList<ColumnInfo> columns)
        {
            InitializeComponent();

            cmbColumns.DisplayMember = NameHelper<ColumnInfo>.Name(x => x.ColumnName);
            cmbColumns.DataSource = columns;
        }

        /// <summary>
        /// Selected column
        /// </summary>
        public ColumnInfo SelectedItem
        {
            get { return (ColumnInfo)cmbColumns.SelectedItem; }
        }
    }
}
