using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Wizard.UI;

namespace DataImport.Excel
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

            ShowPreview();

            // FileType combo box
            var ds = _settings.DataSet;
            var excelSheets = new List<string>(ds.Tables.Count);
            excelSheets.AddRange(from DataTable dt in ds.Tables select dt.TableName);
            cmbExcelSheet.DataSource = excelSheets;
            cmbExcelSheet.SelectedValueChanged += CmbFileTypeOnSelectedValueChanged;
        }

        #endregion

        #region Private methods

        private void ShowPreview()
        {
            _importer.SetPreview(_settings);
            dgvPreview.DataSource = _settings.Preview;
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

        #endregion
    }
}
