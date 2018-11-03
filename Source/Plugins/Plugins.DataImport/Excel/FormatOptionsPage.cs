using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Wizard.UI;

namespace HydroDesktop.Plugins.DataImport.Excel
{
    /// <summary>
    /// Format options page for Excel
    /// </summary>
    public partial class FormatOptionsPage : InternalWizardPage
    {
        #region Fields
        
        private readonly ExcelImportSettings _settings;
        private readonly IWizardImporter _importer;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="FormatOptionsPage"/>
        /// </summary>
        /// <param name="context">Context</param>
        public FormatOptionsPage(WizardContext context)
        {
            _settings = (ExcelImportSettings)context.Settings;
            _importer = context.Importer;

            InitializeComponent();
            
            cmbExcelSheet.SelectedValueChanged += CmbFileTypeOnSelectedValueChanged;
        }

        #endregion

        #region Private methods

        private void ShowPreview()
        {
            _importer.UpdatePreview(_settings);
            dgvPreview.DataSource = _settings.Preview;

            // Excel sheet combo box
            if (cmbExcelSheet.DataSource == null)
            {
                var ds = _settings.DataSet;
                var excelSheets = new List<string>(ds.Tables.Count);
                excelSheets.AddRange(from DataTable dt in ds.Tables select dt.TableName);
                cmbExcelSheet.DataSource = excelSheets;
            }
        }

        private void CmbFileTypeOnSelectedValueChanged(object sender, EventArgs eventArgs)
        {
            _settings.SheetName = (string)cmbExcelSheet.SelectedItem;
            ShowPreview();
        }

        private void FormatOptionsPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next);

            ShowPreview();
        }

        private void FormatOptionsPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSeparator.Text))
            {
                MessageBox.Show(this, "Decimal separator should be non-empty.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
            _settings.ValuesNumberDecimalSeparator = tbSeparator.Text;
        }

        #endregion
    }
}
