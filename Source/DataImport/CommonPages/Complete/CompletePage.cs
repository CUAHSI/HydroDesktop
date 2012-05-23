using System.ComponentModel;
using Wizard.UI;

namespace DataImport.CommonPages.Complete
{
    /// <summary>
    /// Wizard Complete page
    /// </summary>
    public partial class CompletePage : ExternalWizardPage
    {
        /// <summary>
        /// Creates new instance of <see cref="CompletePage"/>
        /// </summary>
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
