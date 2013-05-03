using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages
{
    /// <summary>
    /// View of <see cref="Source"/>
    /// </summary>
    public partial class SourceView : UserControl
    {
       #region Fields

        private Source _entity;

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
            tbOrganization.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.Organization);
            tbDescription.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.Description);
            tbLink.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.Link);
            tbContactName.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.ContactName);
            tbPhone.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.Phone);
            tbEmail.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.Email);
            tbAddress.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.Address);
            tbCity.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.City);
            tbState.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.State);
            tbZipCode.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.ZipCode);
            tbCitation.AddBinding<TextBox, Source>(x => x.Text, bindingSource1, x => x.Citation);
            // todo: cmbMetadata
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
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

        #endregion
    }
}
