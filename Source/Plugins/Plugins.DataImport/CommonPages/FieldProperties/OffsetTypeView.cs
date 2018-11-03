using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.DataImport.CommonPages.FieldProperties
{
    /// <summary>
    /// View of <see cref="OffsetType"/>
    /// </summary>
    public partial class OffsetTypeView : UserControl
    {
        #region Fields

        private OffsetType _entity;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="OffsetTypeView"/>
        /// </summary>
        public OffsetTypeView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            bindingSource1.DataSource = typeof(OffsetType);

            // Set bindings
            tbDescription.AddBinding<TextBox, OffsetType>(x => x.Text, bindingSource1, x => x.Description);

            var unitRepo = RepositoryFactory.Instance.Get<IUnitsRepository>();
            var units = unitRepo.GetAll().OrderBy(u => u.Name).ToArray();
            cmbUnits.DataSource = units;
            cmbUnits.DisplayMember = NameHelper<Unit>.Name(x => x.Name);
        }

        #endregion


        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OffsetType Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;

                // Update Unit Combos
                if (value != null)
                {
                    UpdateUnitCombo(cmbUnits, Entity.Unit);
                }
                bindingSource1.DataSource = value ?? (object)typeof(OffsetType);
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
            var offsetType = Entity;

            string error;
            if (offsetType == null)
                error = "You should specify OffsetType";
            else if (String.IsNullOrEmpty(offsetType.Description))
                error = "OffsetType should have a Description";
            else
                error = string.Empty;

            return error;
        }

        #endregion

        #region Private methods

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

        #endregion

        private void btnCreateNewTimeUnit_Click(object sender, EventArgs e)
        {
            using (var form = new CreateUnitForm())
            {
                form.Entity = Unit.Unknown;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Entity.Unit = form.Entity;
                    // Update Unit Combos
                    UpdateUnitCombo(cmbUnits, Entity.Unit);
                }
            }
        }
    }
}
