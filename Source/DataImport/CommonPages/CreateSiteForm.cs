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
        #region Fields

        private readonly Site _site;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="CreateSiteForm"/>
        /// </summary>
        public CreateSiteForm()
        {
            InitializeComponent();

            _site = new Site{Code = "Site1"};

            //Set bindings
            tbSiteName.DataBindings.Clear();
            tbSiteName.DataBindings.Add(new Binding("Text", _site, "Name", true, DataSourceUpdateMode.OnPropertyChanged));

            tbSiteCode.DataBindings.Clear();
            tbSiteCode.DataBindings.Add(new Binding("Text", _site, "Code", true, DataSourceUpdateMode.OnPropertyChanged));

            nudLat.DataBindings.Clear();
            nudLat.DataBindings.Add(new Binding("Value", _site, "Latitude", true, DataSourceUpdateMode.OnPropertyChanged));

            nudLng.DataBindings.Clear();
            nudLng.DataBindings.Add(new Binding("Value", _site, "Longitude", true, DataSourceUpdateMode.OnPropertyChanged));

            nudElevation.DataBindings.Clear();
            nudElevation.DataBindings.Add(new Binding("Value", _site, "Elevation_m", true, DataSourceUpdateMode.OnPropertyChanged));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Entity that current form create
        /// </summary>
        public Site Entity
        {
            get { return _site; }
        }

        #endregion

        #region Private methods

        private void btnOK_Click(object sender, EventArgs e)
        {
            var error = EntityValidate(_site);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private string EntityValidate(Site site)
        {
            const double EPSILON = 0.00001;

            string error;
            if (String.IsNullOrEmpty(site.Name))
                error = "Site should have a Name";
            else if (String.IsNullOrEmpty(site.Code))
                error = "Site should have a Code";
            else if (Math.Abs(site.Latitude - 0.0) < EPSILON)
                error = "Site should have a not-zero Latitude";
            else if (Math.Abs(site.Longitude - 0.0) < EPSILON)
                error = "Site should have a not-zero Longitude";
            else
                error = string.Empty;

            return error;
        }

        #endregion
    }
}
