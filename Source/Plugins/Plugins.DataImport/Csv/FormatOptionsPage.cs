using System.ComponentModel;
using System.Windows.Forms;
using Wizard.UI;

namespace HydroDesktop.Plugins.DataImport.Csv
{
    /// <summary>
    /// Format options page for Csv
    /// </summary>
    public partial class FormatOptionsPage : InternalWizardPage
    {
        #region Fields
        
        private readonly CsvImportSettings _settings;
        private readonly IWizardImporter _importer;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="FormatOptionsPage"/>
        /// </summary>
        /// <param name="context">Context</param>
        public FormatOptionsPage(WizardContext context)
        {
            _settings = (CsvImportSettings)context.Settings;
            _importer = context.Importer;

            InitializeComponent();
        }

        #endregion

        #region Private methods

        private void ShowPreview()
        {
            _importer.UpdatePreview(_settings);
            dgvPreview.DataSource = _settings.Preview;
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
