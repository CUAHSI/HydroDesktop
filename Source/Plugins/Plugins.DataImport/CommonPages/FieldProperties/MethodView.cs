using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.DataImport.CommonPages
{
    /// <summary>
    /// View of <see cref="Method"/>
    /// </summary>
    public partial class MethodView : UserControl
    {
        #region Fields

        private Method _entity;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="MethodView"/>
        /// </summary>
        public MethodView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;
            
            bindingSource1.DataSource = typeof (Method);

            // Set bindings
            tbDescription.AddBinding<TextBox, Method>(x => x.Text, bindingSource1, x => x.Description);
            tbLink.AddBinding<TextBox, Method>(x => x.Text, bindingSource1, x => x.Link);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
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

        #endregion
    }
}
