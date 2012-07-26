using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.ObjectModel.Controls
{
    /// <summary>
    /// View of <see cref="Site"/>
    /// </summary>
    public partial class SiteView : UserControl
    {
        #region Fields

        private bool _readOnly;
        private Site _entity;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="SiteView"/>
        /// </summary>
        public SiteView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            Entity = null;

            // Set Bindings
            tbSiteName.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).Name);
            tbSiteCode.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).Code);
            nudLat.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Site).Latitude);
            nudLng.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Site).Longitude);
            nudElevation.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Site).Elevation_m);
            tbVertDatum.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).VerticalDatum);
            nudLocalX.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Site).LocalX);
            nudLocalY.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Site).LocalY);
            nudPosAccuracy.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Site).PosAccuracy_m);
            tbState.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).State);
            tbCounty.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).County);
            tbComments.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).Comments);
            tbLocalProjection.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).LocalProjection);
            tbLatLongDatum.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).SpatialReference);
            tbSiteType.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).SiteType);
            tbCountry.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Site).Country);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Entity to View
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Site Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object)typeof(Site);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether control is read-only.
        /// </summary>
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
                tbVertDatum.ReadOnly = value;
                nudLocalX.FullReadOnly = value;
                nudLocalY.FullReadOnly = value;
                nudPosAccuracy.FullReadOnly = value;
                tbState.ReadOnly = value;
                tbCounty.ReadOnly = value;
                tbComments.ReadOnly = value;
                tbLocalProjection.ReadOnly = value;
                tbLatLongDatum.ReadOnly = value;
                tbSiteType.ReadOnly = value;
                tbCountry.ReadOnly = value;
            }
        }

        #endregion
    }
}
