using System.ComponentModel;
using Wizard.UI;

namespace DataImport.CommonPages.Progress
{
    public partial class ProgressPage : InternalWizardPage
    {
        private readonly WizardContext _context;

        public ProgressPage(WizardContext context)
        {
            _context = context;
            InitializeComponent();
        }

        private void ProgressPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.None);

            _context.Importer.UpdateData(_context.Settings);
            var importer = _context.Importer.GetImporter();
            importer.Import(_context.Settings);

            SetWizardButtons(WizardButtons.Next);
            PressButton(WizardButtons.Next);
        }
    }
}
