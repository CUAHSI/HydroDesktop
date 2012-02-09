using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Wizard.UI;

namespace DataImport.Excel
{
    public partial class FormatOptionsPage : InternalWizardPage
    {
        #region Fields

        private readonly DataImportContext _context;
        private readonly ExcelImportSettings _settings;

        #endregion

        public FormatOptionsPage(DataImportContext context)
        {
            _context = context;
            _settings = (ExcelImportSettings)context.Settings;

            InitializeComponent();

            ShowPreview();

            // FileType combo box
            var ds = _settings.DataSet;
            var excelSheets = new List<string>(ds.Tables.Count);
            excelSheets.AddRange(from DataTable dt in ds.Tables select dt.TableName);
            cmbExcelSheet.DataSource = excelSheets;
            cmbExcelSheet.SelectedValueChanged += CmbFileTypeOnSelectedValueChanged;
        }
        private void ShowPreview()
        {
            _context.Importer.SetPreview(_settings);
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
    }
}
