using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Plugins.DataImport.CommonPages.FieldProperties;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using Wizard.UI;

namespace HydroDesktop.Plugins.DataImport.CommonPages
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

            columnsToImportControl1.RemoveClick += ColumnsToImportControlOnRemoveClick;
            columnsToImportControl1.EditClick += ColumnsToImportControlOnEditClick;
            columnsToImportControl1.AddClick += ColumnsToImportControlOnAddClick;
        }

        #endregion

        #region Private methods

        private void ColumnsToImportControlOnEditClick(object sender, ColumnsToProcessEventArgs args)
        {
            if (args.Columns.Count == 0)
                return;

            DoDetailsItemOnClick(args.Columns[0]);
        }

        private void ColumnsToImportControlOnRemoveClick(object sender, ColumnsToProcessEventArgs args)
        {
            foreach (var info in args.Columns)
            {
                info.ImportColumn = false;
                dgvPreview.InvalidateCell(info.ColumnIndex, -1);
            }
            columnsToImportControl1.RefreshData();
        }

        private void ColumnsToImportControlOnAddClick(object sender, EventArgs eventArgs)
        {
            var dateTimeColumn = (string)cmbDateTimeColumn.SelectedItem;
            var availableColumns = _settings.ColumnDatas.Where(c => !c.ImportColumn &&
                                                                    c.ColumnName != dateTimeColumn).ToList();
            using (var selectForm = new SelectColumnForm(availableColumns))
            {
                if (selectForm.ShowDialog(this) == DialogResult.OK)
                {
                    var item = selectForm.SelectedItem;
                    if (item != null)
                    {
                        item.ImportColumn = true;
                        var res = DoDetailsItemOnClick(item);
                        if (res != DialogResult.OK)
                        {
                            item.ImportColumn = false;
                        }
                        dgvPreview.InvalidateCell(item.ColumnIndex, -1);
                    }
                }
            }
        }

        private void DgvPreviewOnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var hitTest = dgvPreview.HitTest(e.X, e.Y);
            if (hitTest.Type == DataGridViewHitTestType.ColumnHeader)
            {
                var index = hitTest.ColumnIndex;
                var cData = _settings.ColumnDatas[index];
                

                var detailsItem = new ToolStripButton("Details...");
                detailsItem.Click += delegate { DoDetailsItemOnClick(cData); };

                var importItem = new ToolStripMenuItem("Import");
                importItem.CheckOnClick = true;
                importItem.Checked = cData.ImportColumn;
                importItem.CheckedChanged += delegate
                                                 {
                                                     cData.ImportColumn = importItem.Checked;
                                                     detailsItem.Enabled = importItem.Checked;
                                                     dgvPreview.InvalidateCell(cData.ColumnIndex, -1);
                                                    
                                                     columnsToImportControl1.RefreshData();
                                                 };
                detailsItem.Enabled = importItem.Checked;

                var popup = new ContextMenuStrip();
                popup.Items.Add(importItem);
                popup.Items.Add(detailsItem);

                popup.Show(dgvPreview.PointToScreen(e.Location));
            }
        }

        private DialogResult DoDetailsItemOnClick(ColumnInfo cData)
        {
            // Prepare parameters to pass into FieldPropertiesForm
            var cDataClone = (ColumnInfo) cData.Clone();
            var dataSources = new DataSources();
            var variablesRepo = RepositoryFactory.Instance.Get<IVariablesRepository>();
            var variables = variablesRepo.GetAll().ToList();
            foreach (var data in _settings.ColumnDatas)
            {
                if (data.Variable != null && !variables.Contains(data.Variable))
                {
                    variables.Add(data.Variable);
                }
            }
            dataSources.Variables = variables;

            // Show form
            using (var form = new FieldPropertiesForm(cDataClone, dataSources))
            {
                var res = form.ShowDialog();
                if (res != DialogResult.OK) return res;

                var cDatas = _settings.ColumnDatas;
                var index = cData.ColumnIndex;
                var cd = form.ColumnData;

                cDatas[index] = cd;

                // Apply site/variable/source/method/qualityControl to all columns if need
                for (int k = 0; k < cDatas.Count; k++)
                {
                    if (k == index) continue;

                    var option = cDatas[k];

                    if (cd.ApplySiteToAllColumns)
                    {
                        option.Site = (Site) cd.Site.Clone();
                    }
                    if (cd.ApplyVariableToAllColumns)
                    {
                        option.Variable = (Variable) cd.Variable.Clone();
                    }
                    if (cd.ApplySourceToAllColumns)
                    {
                        option.Source = (Source) cd.Source.Clone();
                    }
                    if (cd.ApplyMethodToAllColumns)
                    {
                        option.Method = (Method) cd.Method.Clone();
                    }
                    if (cd.ApplyQualityControlToAllColumns)
                    {
                        option.QualityControlLevel = (QualityControlLevel) cd.QualityControlLevel.Clone();
                    }
                    if (cd.ApplyOffsetToAllColumns)
                    {
                        option.OffsetType = (OffsetType)cd.OffsetType.Clone();
                        option.OffsetValue = cd.OffsetValue;
                    }
                }

                columnsToImportControl1.RefreshData();
                return res;
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
            columnsToImportControl1.SetDataSource(_settings.ColumnDatas);

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
                    ImportColumn = false,
                    Method = Method.Unknown,
                    Source = Source.Unknown,
                    QualityControlLevel = QualityControlLevel.Unknown,
                    OffsetType = OffsetType.Unknown,
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
