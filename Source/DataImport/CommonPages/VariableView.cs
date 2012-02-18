using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Configuration;
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
            var unitRepo = RepositoryFactory.Instance.Get<IUnitsRepository>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);
            var units = unitRepo.GetAll();

            cmbVariableUnits.DataSource = units;
            cmbVariableUnits.DisplayMember = NameHelper.Name<Unit, object>(x => x.Name);
            if (Entity.VariableUnit != null)
                cmbVariableUnits.SelectedItem = Entity.VariableUnit;

            cmbTimeUnits.DataSource = units.Select(u => u).ToArray();
            cmbTimeUnits.DisplayMember = NameHelper.Name<Unit, object>(x => x.Name);
            if (Entity.TimeUnit != null)
                cmbTimeUnits.SelectedItem = Entity.TimeUnit;

            var variablesRepo = RepositoryFactory.Instance.Get<IVariablesRepository>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);
            var variables = variablesRepo.GetAll();

            cmbName.DataSource = variables;
            cmbName.DisplayMember = NameHelper.Name<Variable, object>(x => x.Name);
            cmbName.SelectedIndexChanged += delegate
                                                {
                                                    var variable = cmbName.SelectedItem as Variable;
                                                    if (variable != null)
                                                    {
                                                        var ent = Entity;
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

            cmbName.AddBinding<ComboBox, Variable>(x => x.Text, bindingSource1, x => x.Name);
            tbCode.AddBinding<TextBox, Variable>(x => x.Text, bindingSource1, x => x.Code);
            cmbVariableUnits.AddBinding<ComboBox, Variable>(x => x.SelectedItem, bindingSource1, x => x.VariableUnit);
            tbDataType.AddBinding<TextBox, Variable>(x => x.Text, bindingSource1, x => x.DataType);
            tbValueType.AddBinding<TextBox, Variable>(x => x.Text, bindingSource1, x => x.ValueType);
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
