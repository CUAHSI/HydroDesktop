using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.ObjectModel.Controls
{
    /// <summary>
    /// View of <see cref="Variable"/>
    /// </summary>
    public partial class VariableView : UserControl
    {
        #region Fields

        private Variable _entity;
        private bool _readOnly;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="VariableView"/>
        /// </summary>
        public VariableView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            bindingSource1.DataSource = typeof (Variable);

            // Set Bindings
            tbName.AddBinding(() =>  default(TextBox).Text, bindingSource1, () => default(Variable).Name);
            tbCode.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Variable).Code);
            tbVariableUnits.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Variable).VariableUnit);
            tbDataType.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Variable).DataType);
            tbValueType.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Variable).ValueType);
            tbSampleMedium.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Variable).SampleMedium);
            nudTimeSupport.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Variable).TimeSupport);
            tbTimeUnits.AddBinding(() => default(TextBox).Text, bindingSource1, () => default(Variable).TimeUnit);
            nudNoDataValue.AddBinding(() => default(NumericUpDown).Value, bindingSource1, () => default(Variable).NoDataValue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Entity to View
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Variable Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object)typeof(Variable);
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether all editors is read-only.
        /// </summary>
        [Browsable(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;

                tbName.ReadOnly = value;
                tbCode.ReadOnly = value;
                tbVariableUnits.ReadOnly = value;
                tbDataType.ReadOnly = value;
                tbValueType.ReadOnly = value;
                tbSampleMedium.ReadOnly = value;
                nudTimeSupport.FullReadOnly = value;
                tbTimeUnits.ReadOnly = value;
                nudNoDataValue.FullReadOnly = value;
            }
        }


        #endregion
    }
}
