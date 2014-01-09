using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.ObjectModel.Controls
{
    /// <summary>
    /// View of <see cref="QualityControlLevel"/>
    /// </summary>
    public partial class QualityControlLevelView : UserControl
    {
        #region Fields

        private QualityControlLevel _entity;
        private bool _readOnly;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="QualityControlLevelView"/>
        /// </summary>
        public QualityControlLevelView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            Entity = null;

            // Set bindings
            tbCode.AddBinding<TextBox, QualityControlLevel>(x => x.Text, bindingSource1, x => x.Code);
            tbDefinition.AddBinding<TextBox, QualityControlLevel>(x => x.Text, bindingSource1, x => x.Definition);
            tbExplanation.AddBinding<TextBox, QualityControlLevel>(x => x.Text, bindingSource1, x => x.Explanation);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public QualityControlLevel Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object)typeof(QualityControlLevel);
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

                tbCode.ReadOnly = value;
                tbDefinition.ReadOnly = value;
                tbExplanation.ReadOnly = value;
            }
        }

        #endregion
    }
}
