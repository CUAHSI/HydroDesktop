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

            Entity = null;

            // Set Bindings
            tbName.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.Name);
            tbCode.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.Code);
            tbVariableUnits.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.VariableUnit);
            tbDataType.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.DataType);
            tbValueType.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.ValueType);
            tbSampleMedium.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.SampleMedium);
            nudTimeSupport.AddBinding<NumericUpDown, Variable>(n => n.Value, bindingSource1, v => v.TimeSupport);
            tbTimeUnits.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.TimeUnit);
            nudNoDataValue.AddBinding<NumericUpDown, Variable>(n => n.Value, bindingSource1, v => v.NoDataValue);
            tbIsRegular.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.IsRegular);
            tbGeneralCategory.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.GeneralCategory);
            tbSpeciation.AddBinding<TextBox, Variable>(t => t.Text, bindingSource1, v => v.Speciation);

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
                tbIsRegular.ReadOnly = value;
                tbGeneralCategory.ReadOnly = value;
                tbSpeciation.ReadOnly = value;
            }
        }


        #endregion
    }
}
