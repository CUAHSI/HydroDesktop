using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DataImport.CommonPages.FieldProperties;
using HydroDesktop.Common.Tools;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
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
            var unitRepo = RepositoryFactory.Instance.Get<IUnitsRepository>();
            var units = unitRepo.AsQueryable().OrderBy(u => u.Name).ToArray();
            cmbVariableUnits.DataSource = units;
            cmbVariableUnits.DisplayMember = NameHelper.Name<Unit, object>(x => x.Name);

            units = unitRepo.AsQueryable().Where(u => u.UnitsType == "Time").OrderBy(u => u.Name).ToArray();
            cmbTimeUnits.DataSource = units;
            cmbTimeUnits.DisplayMember = NameHelper.Name<Unit, object>(x => x.Name);

            var variablesRepo = RepositoryFactory.Instance.Get<IVariablesRepository>();
            var variables = variablesRepo.GetAll();

            cmbName.DataSource = variables;
            cmbName.DisplayMember = NameHelper.Name<Variable, object>(x => x.Name);
            cmbName.SelectedIndexChanged += delegate
                                                {
                                                    var variable = cmbName.SelectedItem as Variable;
                                                    if (variable != null)
                                                    {
                                                        var ent = Entity;
                                                        if (ent == null) return;
                                                        ent.Code = variable.Code;
                                                        ent.DataType = variable.DataType;
                                                        ent.GeneralCategory = variable.GeneralCategory;
                                                        ent.IsCategorical = variable.IsCategorical;
                                                        ent.IsRegular = variable.IsRegular;
                                                        ent.Name = variable.Name;
                                                        ent.NoDataValue = variable.NoDataValue;
                                                        ent.SampleMedium = variable.SampleMedium;
                                                        ent.Speciation = variable.Speciation;
                                                        ent.TimeSupport = variable.TimeSupport;
                                                        ent.TimeUnit = variable.TimeUnit;
                                                        ent.ValueType = variable.ValueType;
                                                        ent.VariableUnit = variable.VariableUnit;
                                                        ent.VocabularyPrefix = variable.VocabularyPrefix;
                                                        bindingSource1.ResetBindings(false);
                                                    }
                                                };

            cmbValueType.DataSource = Enum.GetValues(typeof (ValueTypeCV));
            cmbValueType.Format += delegate(object s, ListControlConvertEventArgs args)
                                       {
                                           args.Value = ((ValueTypeCV) args.ListItem).Description();
                                       };

            cmbDataType.DataSource = Enum.GetValues(typeof (DataTypeCV));
            cmbDataType.Format += delegate(object s, ListControlConvertEventArgs args)
                                      {
                                          args.Value = ((DataTypeCV) args.ListItem).Description();
                                      };

            cmbSampleMedium.DataSource = Enum.GetValues(typeof(SampleMediumCV));
            cmbSampleMedium.Format += delegate(object s, ListControlConvertEventArgs args)
                                      {
                                          args.Value = ((SampleMediumCV) args.ListItem).Description();
                                      };

            cmbName.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.Name);
            tbCode.AddBinding<TextBox, Variable>(x => x.Text, bindingSource1, x => x.Code);
            cmbVariableUnits.AddBinding<ComboBox, Variable>(x => x.SelectedItem, bindingSource1, x => x.VariableUnit);
            cmbDataType.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.DataType);
            cmbValueType.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.ValueType);
            cmbSampleMedium.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.SampleMedium);
            nudTimeSupport.AddBinding<NumericUpDown, Variable>(x => x.Value, bindingSource1, x => x.TimeSupport);
            cmbTimeUnits.AddBinding<ComboBox, Variable>(x => x.SelectedItem, bindingSource1, x => x.TimeUnit);
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

                // Update Unit Combos
                if (value != null)
                {
                    UpdateUnitCombo(cmbVariableUnits, Entity.VariableUnit);
                    UpdateUnitCombo(cmbTimeUnits, Entity.TimeUnit);
                }
                bindingSource1.DataSource = value ?? (object)typeof(Variable);
            }
        }

        private static void UpdateUnitCombo(ComboBox comboBox, Unit unit)
        {
            var units = (Unit[])comboBox.DataSource;
            if (units != null && unit != null &&
                !units.Contains(unit))
            {
                Array.Resize(ref units, units.Length + 1);
                units[units.Length - 1] = unit;
                comboBox.DataSource = units;
                comboBox.SelectedIndex = units.Length - 1;
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

                cmbName.Enabled = !value;
                tbCode.ReadOnly = value;
                cmbVariableUnits.Enabled = !value;
                cmbDataType.Enabled = !value;
                cmbValueType.Enabled = !value;
                cmbSampleMedium.Enabled = !value;
                nudTimeSupport.FullReadOnly = value;
                cmbTimeUnits.Enabled = !value;
                nudNoDataValue.FullReadOnly = value;

                btnCreateNewTimeUnit.Enabled = !value;
                btnCreateNewVariableUnit.Enabled = !value;
            }
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Validate Current Entity
        /// </summary>
        /// <returns>Error or string.Empty</returns>
        public string EntityValidate()
        {
            var variable = Entity;

            string error;
            if (variable == null)
                error = "Your should specify Variable";
            else if (String.IsNullOrEmpty(variable.Name))
                error = "Variable should have a Name";
            else if (String.IsNullOrEmpty(variable.Code))
                error = "Variable should have a Code";
            else if (String.IsNullOrEmpty(variable.DataType))
                error = "Variable should have a DataType";
            else if (String.IsNullOrEmpty(variable.ValueType))
                error = "Variable should have a ValueType";
            else if (String.IsNullOrEmpty(variable.SampleMedium))
                error = "Variable should have a SampleMedium";
            else
                error = string.Empty;

            return error;
        }

        #endregion

        #region Private methods

        private void btnCreateNewVariableUnit_Click(object sender, EventArgs e)
        {
            using (var form = new CreateUnitForm())
            {
                form.Entity = Unit.Unknown;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Entity.VariableUnit = form.Entity;
                    // Update Unit Combos
                    UpdateUnitCombo(cmbVariableUnits, Entity.VariableUnit);
                    UpdateUnitCombo(cmbTimeUnits, Entity.TimeUnit);
                }
            }
        }

        private void btnCreateNewTimeUnit_Click(object sender, EventArgs e)
        {
            using (var form = new CreateUnitForm())
            {
                form.Entity = Unit.UnknownTimeUnit;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Entity.TimeUnit = form.Entity;
                    // Update Unit Combos
                    UpdateUnitCombo(cmbVariableUnits, Entity.VariableUnit);
                    UpdateUnitCombo(cmbTimeUnits, Entity.TimeUnit);
                }
            }
        }

        #endregion
    }
}
