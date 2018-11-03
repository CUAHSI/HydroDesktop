using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;

namespace HydroDesktop.Plugins.DataImport.CommonPages.FieldProperties
{
    /// <summary>
    /// Shows grid with columns to import
    /// </summary>
    public partial class ColumnsToImportControl : UserControl
    {
        #region Fields

        private IList<ColumnInfo> _columnInfos;

        #endregion

        #region Events

        /// <summary>
        /// Raised when "Remove" button was clicked.
        /// </summary>
        public event EventHandler<ColumnsToProcessEventArgs> RemoveClick;

        /// <summary>
        /// Raised when "Edit..." button was clicked.
        /// </summary>
        public event EventHandler<ColumnsToProcessEventArgs> EditClick;

        /// <summary>
        /// Raised when "Add..." button was clicked.
        /// </summary>
        public event EventHandler AddClick;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="ColumnsToImportControl"/>
        /// </summary>
        public ColumnsToImportControl()
        {
            InitializeComponent();
            
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                                          {
                                              DataPropertyName = NameHelper<ColumnInfoWrapper>.Name(x => x.ColumnName),
                                              HeaderText = "ValueColumn",
                                          });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                                          {
                                              DataPropertyName = NameHelper<ColumnInfoWrapper>.Name(x => x.SiteName),
                                              HeaderText = "Site"
                                          });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                                          {
                                              DataPropertyName = NameHelper<ColumnInfoWrapper>.Name(x => x.VariableName),
                                              HeaderText = "Variable"
                                          });
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set data source (columns to import) to current control.
        /// </summary>
        /// <param name="columnInfos">List of columnInfo.</param>
        public void SetDataSource(IList<ColumnInfo> columnInfos)
        {
            _columnInfos = columnInfos;
            RefreshData();
        }

        /// <summary>
        /// Refresh data
        /// </summary>
        public void RefreshData()
        {
            dataGridView1.DataSource =
                _columnInfos.Where(c => c.ImportColumn).Select(c => new ColumnInfoWrapper(c)).ToList();
        }

        #endregion

        #region Private methods

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedColumnInfos();
            var handler = RemoveClick;
            if (handler != null)
            {
                handler(this, new ColumnsToProcessEventArgs(selected));
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedColumnInfos();
            var handler = EditClick;
            if (handler != null)
            {
                handler(this, new ColumnsToProcessEventArgs(selected));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var handler = AddClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private IList<ColumnInfo> GetSelectedColumnInfos()
        {
            var selected = dataGridView1.SelectedRows.OfType<DataGridViewRow>()
                .Select(r => ((ColumnInfoWrapper)r.DataBoundItem).Source)
                .ToList();
            return selected;
        }

        #endregion

        #region Nested class: ColumnInfoWrapper

        /// <summary>
        /// This class need only for showing second-level properties in data grid view.
        /// </summary>
        private class ColumnInfoWrapper
        {
            public ColumnInfoWrapper(ColumnInfo source)
            {
                Source = source;
            }

            public ColumnInfo Source { get; private set; }

            public string ColumnName
            {
                get { return Source.ColumnName; }
            }

            public string SiteName
            {
                get { return Source.Site != null ? Source.Site.Name : null; }
            }

            public string VariableName
            {
                get { return Source.Variable != null ? Source.Variable.Name : null; }
            }
        }

        #endregion
    }

    /// <summary>
    /// Contains info about columns to process
    /// </summary>
    public class ColumnsToProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Create new instance of <see cref="ColumnsToProcessEventArgs"/>
        /// </summary>
        /// <param name="columnInfos">List of columns to process</param>
        public ColumnsToProcessEventArgs(IList<ColumnInfo> columnInfos)
        {
            Columns = columnInfos;
        }

        /// <summary>
        /// Columns to process
        /// </summary>
        public IList<ColumnInfo> Columns { get; private set; }
    }
}
