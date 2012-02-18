using System;
using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages
{
    /// <summary>
    /// View of <see cref="Site"/>
    /// </summary>
    public partial class SiteView : UserControl
    {
        #region Fields

        private Site _entity;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="SiteView"/>
        /// </summary>
        public SiteView()
        {
            InitializeComponent();

            if (DesignMode) return;
            
            Entity = new Site();

            // Set Bindings
            tbSiteName.AddBinding<TextBox, Site>(x => x.Text, bindingSource1, x => x.Name);
            tbSiteCode.AddBinding<TextBox, Site>(x => x.Text, bindingSource1, x => x.Code);
            nudLat.AddBinding<NumericUpDown, Site>(x => x.Value, bindingSource1, x => x.Latitude);
            nudLng.AddBinding<NumericUpDown, Site>(x => x.Value, bindingSource1, x => x.Longitude);
            nudElevation.AddBinding<NumericUpDown, Site>(x => x.Value, bindingSource1, x => x.Elevation_m);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Site Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;

                bindingSource1.DataSource = value;
            }
        }

        private bool _readOnly;
        [Browsable(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;

                tbSiteName.ReadOnly = value;
                tbSiteCode.ReadOnly = value;
                nudLat.FullReadOnly = value;
                nudLng.FullReadOnly = value;
                nudElevation.FullReadOnly = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Validate Current Entity
        /// </summary>
        /// <returns>Error or string.Empty</returns>
        public string EntityValidate()
        {
            var site = Entity;
            const double EPSILON = 0.00001;

            string error;
            if (String.IsNullOrEmpty(site.Name))
                error = "Site should have a Name";
            else if (String.IsNullOrEmpty(site.Code))
                error = "Site should have a Code";
            else if (Math.Abs(site.Latitude - 0.0) < EPSILON)
                error = "Site should have a non-zero Latitude";
            else if (Math.Abs(site.Longitude - 0.0) < EPSILON)
                error = "Site should have a non-zero Longitude";
            else
                error = string.Empty;

            return error;
        }

        #endregion
    }
}
