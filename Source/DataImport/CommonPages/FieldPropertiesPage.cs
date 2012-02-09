using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wizard.UI;

namespace DataImport.CommonPages
{
    public partial class FieldPropertiesPage : InternalWizardPage
    {
        private readonly DataImportContext _context;

        public FieldPropertiesPage(DataImportContext context)
        {
            _context = context;
            InitializeComponent();
        }

        private void FieldPropertiesPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next | WizardButtons.Back);

            dgvPreview.DataSource = _context.Settings.Preview;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var createSiteform = new CreateSiteForm();
            createSiteform.ShowDialog();
            var site = createSiteform.Entity;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var form = new DetailsForm();
            form.ShowDialog();
            var q = form.QualityControlLevel;
        }
    }
}
