using System;
using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages
{
    /// <summary>
    /// View of <see cref="Variable"/>
    /// </summary>
    public partial class VariableView : UserControl
    {
        #region Fields

        private Variable _entity;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="VariableView"/>
        /// </summary>
        public VariableView()
        {
            InitializeComponent();

            if (DesignMode) return;

            Entity = new Variable
                         {
                             VariableUnit = Unit.Unknown,
                             TimeUnit = Unit.UnknownTimeUnit,
                         };

            // Set Bindings
            cmbName.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.Name);
            tbCode.AddBinding<TextBox, Variable>(x => x.Text, bindingSource1, x => x.Code);
            cmbVariableUnits.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.VariableUnit.Name);
            tbDataType.AddBinding<TextBox, Variable>(x => x.Text, bindingSource1, x => x.DataType);
            tbValueType.AddBinding<TextBox, Variable>(x => x.Text, bindingSource1, x => x.ValueType);
            nudTimeSupport.AddBinding<NumericUpDown, Variable>(x => x.Value, bindingSource1, x => x.TimeSupport);
            cmbTimeUnits.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.TimeUnit.Name);
            nudNoDataValue.AddBinding<NumericUpDown, Variable>(x => x.Value, bindingSource1, x => x.NoDataValue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Variable Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;

                bindingSource1.DataSource = value;
            }
        }

        private bool _readOnly;
        [Browsable(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;

                cmbName.Enabled = !value;
                tbCode.ReadOnly = value;
                cmbVariableUnits.Enabled = !value;
                tbDataType.ReadOnly = value;
                tbValueType.ReadOnly = value;
                nudTimeSupport.FullReadOnly = value;
                cmbTimeUnits.Enabled = !value;
                nudNoDataValue.FullReadOnly = value;
            }
        }


        #endregion

        /// <summary>
        /// Validate Current Entity
        /// </summary>
        /// <returns>Error or string.Empty</returns>
        public string EntityValidate()
        {
            var variable = Entity;

            string error;
            if (String.IsNullOrEmpty(variable.Name))
                error = "Variable should have a Name";
            else if (String.IsNullOrEmpty(variable.Code))
                error = "Variable should have a Code";
            else
                error = string.Empty;

            return error;
        }
    }
}
