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

            Entity = null;

            // Set bindings
            tbOrganization.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.Organization);
            tbDescription.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.Description);
            tbLink.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.Link);
            tbContactName.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.ContactName);
            tbPhone.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.Phone);
            tbEmail.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.Email);
            tbAddress.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.Address);
            tbCity.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.City);
            tbState.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.State);
            tbZipCode.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.ZipCode);
            tbCitation.AddBinding<TextBox, Source>(t => t.Text, bindingSource1, s => s.Citation);
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
