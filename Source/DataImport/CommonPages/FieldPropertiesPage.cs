using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using Wizard.UI;

namespace DataImport.CommonPages
{
    public partial class FieldPropertiesPage : InternalWizardPage
    {
        private readonly IColumnDataImportSettings _settings;
        private readonly int _columnHeight;
        private readonly List<Control> _addedControls = new List<Control>();

        public FieldPropertiesPage(DataImportContext context)
        {
            _settings = context.Settings as IColumnDataImportSettings;
            if (_settings == null)
            {
                throw new ArgumentOutOfRangeException();
            }

            InitializeComponent();

            _columnHeight = dgvPreview.ColumnHeadersHeight;
        }

        private void FieldPropertiesPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next | WizardButtons.Back);
            
            foreach (var control in _addedControls)
            {
                dgvPreview.Controls.Remove(control);
                control.Dispose();
            }
            _addedControls.Clear();

            dgvPreview.DataSource = _settings.Preview;
            _settings.ColumnDatas = new List<ColumnData>(dgvPreview.Columns.Count);

            var columnNames = _settings.Preview.Columns.Cast<DataColumn>()
                                                               .Select(c => c.ColumnName)
                                                               .ToArray();
            cmbDateTimeColumn.DataSource = columnNames;
            cmbDateTimeColumn.SelectedItem = FindColumnWithDateTime(_settings.Preview);
            
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvPreview.ColumnHeadersHeight = 3 * _columnHeight;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            for (int i = 0; i < dgvPreview.Columns.Count; i++)
            {
                var column = dgvPreview.Columns[i];
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                
                var columnName = column.Name;
                var columnSize = column.HeaderCell.Size;
                var columnLocation = dgvPreview.GetCellDisplayRectangle(i, -1, true).Location;

                var columnData = new ColumnData
                                     {
                                         ColumnIndex = i,
                                         ColumnName = columnName,
                                         ImportColumn = true
                                     };
                _settings.ColumnDatas.Add(columnData);

                // Label with column name
                var label = new Label {Text = columnName, Visible = true};
                dgvPreview.Controls.Add(label);
                _addedControls.Add(label);
                label.Location = new Point(columnLocation.X, columnLocation.Y);
                label.Size = new Size(columnSize.Width, _columnHeight);

                // "Import column" checkbox
                var checkBox = new CheckBox { Text = "Import", Checked = columnData.ImportColumn, Visible = true };
                dgvPreview.Controls.Add(checkBox);
                _addedControls.Add(checkBox);
                checkBox.Location = new Point(columnLocation.X, columnLocation.Y + _columnHeight);
                checkBox.Size = new Size(columnSize.Width, _columnHeight);
                
                // Additional properties button
                var button = new Button {Text = "...", Visible = true};
                dgvPreview.Controls.Add(button);
                _addedControls.Add(button);
                button.Location = new Point(columnLocation.X, columnLocation.Y + _columnHeight * 2);
                button.Size = new Size(columnSize.Width, _columnHeight);
                button.Click += delegate
                                    {
                                        var index = (int)button.Tag;
                                        var cDatas = _settings.ColumnDatas;

                                        var cData = cDatas[index];
                                        using (var form = new FieldPropertiesForm((ColumnData) cData.Clone()))
                                        {
                                            var res = form.ShowDialog();
                                            if (res != DialogResult.OK) return;

                                            var cd = form.ColumnData;
                                            cDatas[index] = cd;

                                            // Apply site to all columns if need
                                            if (cd.ApplySiteToAllColumns)
                                            {
                                                for (int k = 0; k < cDatas.Count; k++)
                                                {
                                                    if (k == index) continue;

                                                    var option = cDatas[k];
                                                    option.Site = (Site) cd.Site.Clone();
                                                }
                                            }
                                        }
                                    };
                button.Tag = i;

                checkBox.CheckedChanged += delegate
                                               {
                                                   columnData.ImportColumn = checkBox.Checked;
                                                   button.Enabled = checkBox.Checked;
                                               };
            }
        }

        private static string FindColumnWithDateTime(DataTable table)
        {
            if (table.Columns.Count == 0)
                return string.Empty;

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    DateTime dateTime;
                    if (DateTime.TryParse(row[column].ToString(), out dateTime))
                    {
                        return column.ColumnName;
                    }
                }
            }

            return table.Columns[0].ColumnName;
        }

        private void FieldPropertiesPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            _settings.DateTimeColumn = (string) cmbDateTimeColumn.SelectedItem;

            string error = null;
            if (String.IsNullOrEmpty(_settings.DateTimeColumn))
            {
                error = "Specify Date/Time column";
            }
            else if (!_settings.ColumnDatas.Any(c => c.ImportColumn &&
                                                     c.ColumnName != _settings.DateTimeColumn))
            {
                error = "Select at least one column to import";
            }
            else if (_settings.ColumnDatas.Where(c => c.ImportColumn && c.ColumnName != _settings.DateTimeColumn)
                                           .Any(c => c.Site == null || c.Variable == null))
            {
                error = "Specify Site and Variable for all columns to import";
            }

            if (!string.IsNullOrEmpty(error))
            {
                e.Cancel = true;
                MessageBox.Show(this, error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
