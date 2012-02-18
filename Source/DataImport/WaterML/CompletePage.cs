using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wizard.UI;

namespace DataImport.WaterML
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
