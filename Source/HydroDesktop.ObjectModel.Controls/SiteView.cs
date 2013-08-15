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
            tbSiteName.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.Name);
            tbSiteCode.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.Code);
            nudLat.AddBinding<NumericUpDown, Site>(n  => n.Value, bindingSource1, s => s.Latitude);
            nudLng.AddBinding<NumericUpDown, Site>(n => n.Value, bindingSource1, s => s.Longitude);
            nudElevation.AddBinding<NumericUpDown, Site>(n => n.Value, bindingSource1, s => s.Elevation_m);
            tbVertDatum.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.VerticalDatum);
            nudLocalX.AddBinding<NumericUpDown, Site>(n => n.Value, bindingSource1, s => s.LocalX);
            nudLocalY.AddBinding<NumericUpDown, Site>(n => n.Value, bindingSource1, s => s.LocalY);
            nudPosAccuracy.AddBinding<NumericUpDown, Site>(n => n.Value, bindingSource1, s => s.PosAccuracy_m);
            tbState.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.State);
            tbCounty.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.County);
            tbComments.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.Comments);
            tbLocalProjection.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.LocalProjection);
            tbLatLongDatum.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.SpatialReference);
            tbSiteType.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.SiteType);
            tbCountry.AddBinding<TextBox, Site>(t => t.Text, bindingSource1, s => s.Country);
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
