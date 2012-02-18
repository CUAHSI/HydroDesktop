using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using ImportFromWaterML;
using Wizard.UI;

namespace DataImport.WaterML
{
    public partial class ProgressPage : InternalWizardPage
    {
        private readonly DataImportContext _context;

        public ProgressPage(DataImportContext context)
        {
            _context = context;
            InitializeComponent();
        }

        private void ProgressPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.None);

            _context.Importer.Import(_context.Settings);

            SetWizardButtons(WizardButtons.Next);
            PressButton(WizardButtons.Next);
        }
    }
}
