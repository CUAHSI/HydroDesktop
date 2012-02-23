using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using Wizard.UI;

namespace DataImport.CommonPages
{
    /// <summary>
    /// Properties page
    /// </summary>
    public partial class FieldPropertiesPage : InternalWizardPage
    {
        #region Fields

        private readonly IWizardImporterSettings _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="FieldPropertiesPage"/>
        /// </summary>
        /// <param name="context">Context</param>
        public FieldPropertiesPage(WizardContext context)
        {
            _settings = context.Settings;
            InitializeComponent();

            dgvPreview.MouseDown += DgvPreviewOnMouseDown;
            dgvPreview.CellPainting += DgvPreviewOnCellPainting;
        }

        #endregion

        #region Private methods

        private void DgvPreviewOnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var hitTest = dgvPreview.HitTest(e.X, e.Y);
            if (hitTest.Type == DataGridViewHitTestType.ColumnHeader)
            {
                var index = hitTest.ColumnIndex;
                var cData = _settings.ColumnDatas[index];

                var detailsItem = new ToolStripButton("Details...");
                detailsItem.Click += delegate
                                         {
                                             using (var form = new FieldPropertiesForm((ColumnInfo)cData.Clone()))
                                             {
                                                 var res = form.ShowDialog();
                                                 if (res != DialogResult.OK) return;

                                                 var cDatas = _settings.ColumnDatas;

                                                 var cd = form.ColumnData;
                                                 cDatas[index] = cd;

                                                 // Apply site to all columns if need
                                                 if (cd.ApplySiteToAllColumns)
                                                 {
                                                     for (int k = 0; k < cDatas.Count; k++)
                                                     {
                                                         if (k == index) continue;

                                                         var option = cDatas[k];
                                                         option.Site = (Site)cd.Site.Clone();
                                                     }
                                                 }
                                             }
                                         };

                var importItem = new ToolStripMenuItem("Import");
                importItem.CheckOnClick = true;
                importItem.Checked = cData.ImportColumn;
                importItem.CheckedChanged += delegate
                                                 {
                                                     cData.ImportColumn = importItem.Checked;
                                                     detailsItem.Enabled = importItem.Checked;
                                                     dgvPreview.InvalidateColumn(cData.ColumnIndex);
                                                 };
                detailsItem.Enabled = importItem.Checked;

                var popup = new ContextMenuStrip();
                popup.Items.Add(importItem);
                popup.Items.Add(detailsItem);

                popup.Show(dgvPreview.PointToScreen(e.Location));
            }
        }

        private void DgvPreviewOnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                if (e.ColumnIndex >= 0 &&
                    _settings.ColumnDatas[e.ColumnIndex].ImportColumn)
                {
                    e.Graphics.FillRectangle(Brushes.LightGreen, e.CellBounds);
                    e.Paint(e.ClipBounds, (DataGridViewPaintParts.All & ~DataGridViewPaintParts.Background));
                    e.Handled = true;
                }
            }
        }
       
        private void FieldPropertiesPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next | WizardButtons.Back);
            
            dgvPreview.DataSource = _settings.Preview;
            _settings.ColumnDatas = new List<ColumnInfo>(dgvPreview.Columns.Count);

            var columnNames = _settings.Preview.Columns.Cast<DataColumn>()
                                                               .Select(c => c.ColumnName)
                                                               .ToArray();
            cmbDateTimeColumn.DataSource = columnNames;
            cmbDateTimeColumn.SelectedItem = FindColumnWithDateTime(_settings.Preview);
            
            for (int i = 0; i < dgvPreview.Columns.Count; i++)
            {
                var column = dgvPreview.Columns[i];
                var columnName = column.Name;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

                var columnData = new ColumnInfo
                {
                    ColumnIndex = i,
                    ColumnName = columnName,
                    ImportColumn = false
                };
                _settings.ColumnDatas.Add(columnData);
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

        #endregion

       
    }
}
