using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.ObjectModel.Controls
{
    /// <summary>
    /// View of <see cref="Source"/>
    /// </summary>
    public partial class SourceView : UserControl
    {
       #region Fields

        private Source _entity;
        private bool _readOnly;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="SourceView"/>
        /// </summary>
        public SourceView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            bindingSource1.DataSource = typeof(Source);

            // Set bindings
            tbOrganization.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).Organization);
            tbDescription.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).Description);
            tbLink.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).Link);
            tbContactName.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).ContactName);
            tbPhone.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).Phone);
            tbEmail.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).Email);
            tbAddress.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).Address);
            tbCity.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).City);
            tbState.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).State);
            tbZipCode.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).ZipCode);
            tbCitation.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Source).Citation);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Entity to View
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Source Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object)typeof(Source);
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

                tbOrganization.ReadOnly = value;
                tbDescription.ReadOnly = value;
                tbLink.ReadOnly = value;
                tbContactName.ReadOnly = value;
                tbPhone.ReadOnly = value;
                tbEmail.ReadOnly = value;
                tbAddress.ReadOnly = value;
                tbCity.ReadOnly = value;
                tbState.ReadOnly = value;
                tbZipCode.ReadOnly = value;
                tbCitation.ReadOnly = value;
            }
        }

        #endregion
    }
}
