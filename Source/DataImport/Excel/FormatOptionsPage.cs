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
        private readonly DataImportContext _context;
        private readonly ExcelImportSettings _settings;

        public FormatOptionsPage(DataImportContext context)
        {
            _context = context;
            _settings = (ExcelImportSettings)context.Settings;

            InitializeComponent();

            // FileType combo box
             var ds = ((ExcelImporter)_context.Importer).AsDataSet(_settings);
            _settings.DataSet = ds;
            var excelSheets = new List<string>(ds.Tables.Count);
            excelSheets.AddRange(from DataTable dt in ds.Tables select dt.TableName);
            cmbExcelSheet.DataSource = excelSheets;
            cmbExcelSheet.SelectedValueChanged += CmbFileTypeOnSelectedValueChanged;
          
            //
            CmbFileTypeOnSelectedValueChanged(cmbExcelSheet, EventArgs.Empty);
        }
        private void ShowPreview()
        {
            var preview = _context.Importer.GetPreview(_settings);
            dgvPreview.DataSource = preview;
        }

        private void CmbFileTypeOnSelectedValueChanged(object sender, EventArgs eventArgs)
        {
            _settings.SheetName = (string)cmbExcelSheet.SelectedItem;
            ShowPreview();
        }

        private void FormatOptionsPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next);
        }
    }
}
