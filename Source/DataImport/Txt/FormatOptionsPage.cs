using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using Wizard.UI;

namespace DataImport.Txt
{
    public partial class FormatOptionsPage : InternalWizardPage
    {
        private readonly DataImportContext _context;
        private readonly TxtImportSettings _settings;

        public FormatOptionsPage(DataImportContext context)
        {
            _context = context;
            _settings = (TxtImportSettings) context.Settings;

            InitializeComponent();
            
            // FileType combo box
            cmbFileType.DataSource = Enum.GetValues(typeof(TxtFileType));
            cmbFileType.Format += delegate(object s, ListControlConvertEventArgs args)
            {
                args.Value = ((TxtFileType)args.ListItem).Description();
            };
            cmbFileType.SelectedValueChanged += CmbFileTypeOnSelectedValueChanged;
            cmbFileType.DataBindings.Clear();
            cmbFileType.DataBindings.Add("SelectedItem", _settings, "FileType", true, DataSourceUpdateMode.OnPropertyChanged);

            // Delimiter
            _settings.Delimiter = delimiterSelector.CurrentDelimiter;
            delimiterSelector.CurrentDelimiterChanged += DelimiterSelectorOnCurrentDelimiterChanged;

            //
            CmbFileTypeOnSelectedValueChanged(cmbFileType, EventArgs.Empty);
        }

        private void DelimiterSelectorOnCurrentDelimiterChanged(object sender, EventArgs eventArgs)
        {
            _settings.Delimiter = delimiterSelector.CurrentDelimiter;
            ShowPreview();
        }

        private void ShowPreview()
        {
            var preview = _context.Importer.GetPreview(_settings);
            dgvPreview.DataSource = preview;
        }

        private void CmbFileTypeOnSelectedValueChanged(object sender, EventArgs eventArgs)
        {
            var txtFileType = (TxtFileType) cmbFileType.SelectedItem;
            _settings.FileType = txtFileType;

            switch (txtFileType)
            {
                case TxtFileType.FixedWidth:
                    delimiterSelector.Visible = false;
                    dgvPreview.Location = new Point(delimiterSelector.Location.X,
                                                    delimiterSelector.Location.Y);
                    dgvPreview.Height = Height - dgvPreview.Location.Y - 20;
                    break;
                case TxtFileType.Delimited:
                    delimiterSelector.Visible = true;
                    dgvPreview.Location = new Point(delimiterSelector.Location.X,
                                                    delimiterSelector.Location.Y + delimiterSelector.Height + 5);
                    dgvPreview.Height = Height - dgvPreview.Location.Y - 20;
                    break;
            }
            ShowPreview();
        }

        private void FormatOptionsPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next);
        }
    }
}
