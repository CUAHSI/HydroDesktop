using System;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages.FieldProperties
{
    /// <summary>
    /// Allow to create new Unit
    /// </summary>
    public partial class CreateUnitForm : Form
    {
        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="CreateUnitForm"/>
        /// </summary>
        public CreateUnitForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets current Entity
        /// </summary>
        public Unit Entity
        {
            get { return unitView1.Entity; }
            set { unitView1.Entity = value; }
        }

        #endregion

        #region Private methods

        private void btnOK_Click(object sender, EventArgs e)
        {
            var error = unitView1.EntityValidate();
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
