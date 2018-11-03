using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.SeriesView
{
    public partial class SeriesShortView : UserControl
    {
         #region Fields

        private Series _entity;
        private bool _readOnly;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="SeriesShortView"/>
        /// </summary>
        public SeriesShortView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            Entity = null;

            // Set bindings
            tbBeginDateTime.AddBinding<TextBox, Series>(x => x.Text, bindingSource1, x => x.BeginDateTime);
            tbBeginDateTimeUTC.AddBinding<TextBox, Series>(x => x.Text, bindingSource1, x => x.BeginDateTimeUTC);
            tbEndDateTime.AddBinding<TextBox, Series>(x => x.Text, bindingSource1, x => x.EndDateTime);
            tbEndDateTimeUTC.AddBinding<TextBox, Series>(x => x.Text, bindingSource1, x => x.EndDateTimeUTC);
            tbValueCount.AddBinding<TextBox, Series>(x => x.Text, bindingSource1, x => x.ValueCount);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Series Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object)typeof(Series);
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

                tbBeginDateTime.ReadOnly = value;
                tbBeginDateTimeUTC.ReadOnly = value;
                tbEndDateTime.ReadOnly = value;
                tbEndDateTimeUTC.ReadOnly = value;
                tbValueCount.ReadOnly = value;
            }
        }

        #endregion
    }
}
