using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using OpenMI.Standard2;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    public partial class AdaptedArguments : Form
    {
        DataTable _table;
        IList<IArgument> _arguments;
        DataGridViewCell _cellInEdit = null;

        public AdaptedArguments()
        {
            InitializeComponent();
        }

        public void Initialise(IList<IArgument> args)
        {
            _arguments = args;

            _table = new DataTable();

            DataColumn column = new DataColumn();

            column.DataType = typeof(String);
            column.ColumnName = "ID";
            column.ReadOnly = true;
            column.Unique = true;

            _table.Columns.Add(column);

            column = new DataColumn();

            column.DataType = typeof(String);
            column.ColumnName = "Caption";
            column.ReadOnly = true;
            column.Unique = false;

            _table.Columns.Add(column);

            column = new DataColumn();

            column.DataType = typeof(String);
            column.ColumnName = "Value";
            column.ReadOnly = false;
            column.Unique = false;

            _table.Columns.Add(column);

            column = new DataColumn();

            column.DataType = typeof(String);
            column.ColumnName = "Type";
            column.ReadOnly = true;
            column.Unique = false;

            _table.Columns.Add(column);

            column = new DataColumn();

            column.DataType = typeof(String);
            column.ColumnName = "Default";
            column.ReadOnly = true;
            column.Unique = false;

            _table.Columns.Add(column);

            column = new DataColumn();

            column.DataType = typeof(String);
            column.ColumnName = "Description";
            column.ReadOnly = true;
            column.Unique = false;

            _table.Columns.Add(column);
            DataRow row;

            foreach (IArgument arg in args)
            {
                row = _table.NewRow();
                row["ID"] = arg.Id;
                row["Caption"] = arg.Caption;
                row["Value"] = arg.Value;
                row["Type"] = arg.ValueType.ToString();
                row["Default"] = arg.DefaultValue.ToString();
                row["Description"] = arg.Description;
                _table.Rows.Add(row);
            }

            dataGridView1.DataSource = _table;

            dataGridView1.CellBeginEdit += new DataGridViewCellCancelEventHandler(CellBeginEdit);
            dataGridView1.CellEndEdit += new DataGridViewCellEventHandler(CellEndEdit);

            dataGridView1.Columns[0].Visible = false; // hide Id's

            for (int nR = 0; nR < dataGridView1.Rows.Count; ++nR)
                for (int nC = 0; nC < dataGridView1.Rows[nR].Cells.Count; ++nC)
                {
                    if (dataGridView1.Rows[nR].Cells[nC].ReadOnly)
                        dataGridView1.Rows[nR].Cells[nC].Style.BackColor = Color.LightGray;
                }
        }

        public void UpdateArgumentsFromForm()
        {
            for (int n = 0; n < _arguments.Count; ++n)
                _arguments[n].ValueAsString = (string)_table.Rows[n][1];
        }

        void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Debug.Assert(_cellInEdit == null);
            _cellInEdit = ((DataGridView)sender).CurrentCell;
        }

        void CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            EditValue();
        }

        void EditValue()
        {
            Debug.Assert(_cellInEdit != null);

            if (!_cellInEdit.ReadOnly)
                UpdateArgFromCell();

            _cellInEdit = null;
        }

        void UpdateArgFromCell()
        {
            string value = (string)_cellInEdit.Value;
            string oldValue = _arguments[_cellInEdit.RowIndex].ValueAsString;

            try
            {
                _arguments[_cellInEdit.RowIndex].ValueAsString = value;
            }
            catch (Exception e)
            {
                _arguments[_cellInEdit.RowIndex].Value = oldValue;
                _cellInEdit.Value = oldValue;

                string s = string.Format(
                    "Invalid Value \"{0}\"\r\n"
                    + "Value should be of type {1}",
                        value,
                        _arguments[_cellInEdit.RowIndex].ValueType.ToString());

                MessageBox.Show(s, e.Message, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (_cellInEdit != null)
            {
                dataGridView1.NotifyCurrentCellDirty(true);

                EditValue();
            }
        }
    }
}
