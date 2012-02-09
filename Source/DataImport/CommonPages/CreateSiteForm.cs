using System;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages
{
    /// <summary>
    /// Allow create new site
    /// </summary>
    public partial class CreateSiteForm : Form
    {
        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="CreateSiteForm"/>
        /// </summary>
        public CreateSiteForm()
        {
            InitializeComponent();

            siteView1.Entity = new Site { Code = "Site1" };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        public Site Entity
        {
            get { return siteView1.Entity; }
        }

        #endregion

        #region Private methods

        private void btnOK_Click(object sender, EventArgs e)
        {
            var error = siteView1.EntityValidate();
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
