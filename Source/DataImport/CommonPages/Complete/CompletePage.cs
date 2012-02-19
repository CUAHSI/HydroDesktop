using System.ComponentModel;
using Wizard.UI;

namespace DataImport.CommonPages.Complete
{
    public partial class CompletePage : ExternalWizardPage
    {
        public CompletePage()
        {
            InitializeComponent();
        }

        private void CompletePage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Finish);
        }
    }
}
