using System;
using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages.FieldProperties
{
    /// <summary>
    /// View of <see cref="Unit"/>
    /// </summary>
    public partial class UnitView : UserControl
    {
        #region Fields

        private Unit _entity;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="UnitView"/>
        /// </summary>
        public UnitView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            bindingSource1.DataSource = typeof (Unit);

            // Set bindings
            tbName.AddBinding<TextBox, Unit>(x => x.Text, bindingSource1, x => x.Name);
            tbType.AddBinding<TextBox, Unit>(x => x.Text, bindingSource1, x => x.UnitsType);
            tbAbbreviation.AddBinding<TextBox, Unit>(x => x.Text, bindingSource1, x => x.Abbreviation);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Unit Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object) typeof (Unit);
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
            var unit = Entity;

            string error;
            if (String.IsNullOrEmpty(unit.Name))
                error = "Unit should have a Name";
            else if (String.IsNullOrEmpty(unit.UnitsType))
                error = "Unit should have a Type";
            else if (String.IsNullOrEmpty(unit.Abbreviation))
                error = "Unit should have a Abbreviation";
            else
                error = string.Empty;

            return error;
        }

        #endregion
    }
}
