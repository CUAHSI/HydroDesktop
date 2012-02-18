using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages
{
    public partial class CreateVariableForm : Form
    {
        public CreateVariableForm()
        {
            InitializeComponent();

            variableView1.Entity = new Variable { Name = "Variable1" };
        }

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        public Variable Entity
        {
            get { return variableView1.Entity; }
        }

        #endregion

        #region Private methods

        private void btnOK_Click(object sender, EventArgs e)
        {
            var error = variableView1.EntityValidate();
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}
