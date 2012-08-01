using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.ObjectModel.Controls
{
    /// <summary>
    /// View of <see cref="Method"/>
    /// </summary>
    public partial class MethodView : UserControl
    {
        #region Fields

        private Method _entity;
        private bool _readOnly;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="MethodView"/>
        /// </summary>
        public MethodView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            Entity = null;

            // Set bindings
            tbDescription.AddBinding<TextBox, Method>(t => t.Text, bindingSource1, m => m.Description);
            tbLink.AddBinding<TextBox, Method>(t => t.Text, bindingSource1, m => m.Link);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Entity to View
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Method Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object)typeof(Method);
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

                tbDescription.ReadOnly = value;
                tbLink.ReadOnly = value;
            }
        }

        #endregion
    }
}
