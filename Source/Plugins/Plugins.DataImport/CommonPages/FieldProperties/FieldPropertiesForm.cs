using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.DataImport.CommonPages
{
    /// <summary>
    /// Allow to modify properties of field to import
    /// </summary>
    public partial class FieldPropertiesForm : Form
    {
        #region Fields

        private readonly ColumnInfo _columnData;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="FieldPropertiesForm"/>
        /// </summary>
        /// <param name="columnData">Column data</param>
        /// <param name="dataSources">Data sources for comboboxes, etc...</param>
        public FieldPropertiesForm(ColumnInfo columnData, DataSources dataSources)
        {
            if (columnData == null) throw new ArgumentNullException("columnData");
            if (dataSources == null) throw new ArgumentNullException("dataSources");
            Contract.EndContractBlock();

            _columnData = columnData;
            InitializeComponent();
            
            if (this.IsDesignMode()) return;

            base.Text = string.Format("Column properties - {0}", _columnData.ColumnName);

            // Set bindings.......
            chApplySiteToAllCoumns.AddBinding(c => c.Checked, _columnData, c => c.ApplySiteToAllColumns);
            chApplyVariableToAllColumns.AddBinding(c => c.Checked, _columnData, c => c.ApplyVariableToAllColumns);
            chApplySourceToAllColumns.AddBinding(c => c.Checked, _columnData, c => c.ApplySourceToAllColumns);
            chApplyMethodToAllColumns.AddBinding(c => c.Checked, _columnData, c => c.ApplyMethodToAllColumns);
            chApplyQualityControlToAllColumns.AddBinding(c => c.Checked, _columnData, c => c.ApplyQualityControlToAllColumns);
            chApplyOffsetToAllColumns.AddBinding(c => c.Checked, _columnData, c => c.ApplyOffsetToAllColumns);

            // Site
            siteView1.ReadOnly = true;
            cmbSites.SelectedIndexChanged += CmbSitesOnSelectedIndexChanged;
            var sitesRepo = RepositoryFactory.Instance.Get<ISitesRepository>();
            var sites = sitesRepo.GetAll();
            if (_columnData.Site != null &&
                !sites.Contains(_columnData.Site))
            {
                sites.Add(_columnData.Site);
            }
            sitesBindingSource.DataSource = sites; 
            cmbSites.DataSource = sitesBindingSource;
            cmbSites.DisplayMember = NameHelper<Site>.Name(s => s.Name);
            if (_columnData.Site != null)
                cmbSites.SelectedItem = _columnData.Site;
            
            // Variable
            variableView1.ReadOnly = true;
            cmbVariables.SelectedIndexChanged += CmbVariablesOnSelectedIndexChanged;
            variablesBindingSource.DataSource = dataSources.Variables;
            cmbVariables.DataSource = variablesBindingSource;
            cmbVariables.DisplayMember = NameHelper<Variable>.Name(s => s.Name);
            if (_columnData.Variable != null)
                cmbVariables.SelectedItem = _columnData.Variable;

            // Source
            sourceView1.Entity = _columnData.Source;

            // Method
            methodView1.Entity = _columnData.Method;

            // Quality Control
            qualityControlLevelView1.Entity = _columnData.QualityControlLevel;

            // Offset
            offsetTypeView1.Entity = _columnData.OffsetType;
            nudOffsetValue.AddBinding<NumericUpDown, ColumnInfo>(x => x.Value, _columnData, c => c.OffsetValue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current column data
        /// </summary>
        public ColumnInfo ColumnData
        {
            get { return _columnData; }
        }

        #endregion

        #region Private methods

        private void CmbVariablesOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var variablesRepo = RepositoryFactory.Instance.Get<IVariablesRepository>();

            var currentVar = (Variable)cmbVariables.SelectedItem;
            variableView1.Entity = currentVar;
            variableView1.ReadOnly = currentVar == null || variablesRepo.Exists(currentVar);
        }

        private void CmbSitesOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            var sitesRepo = RepositoryFactory.Instance.Get<ISitesRepository>();

            var currentSite = (Site)cmbSites.SelectedItem;
            siteView1.Entity = currentSite;
            siteView1.ReadOnly = currentSite == null || sitesRepo.Exists(currentSite);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var error = siteView1.EntityValidate();
            if (string.IsNullOrEmpty(error))
                error = variableView1.EntityValidate();
            if (string.IsNullOrEmpty(error))
                error = offsetTypeView1.EntityValidate();

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            _columnData.Site = siteView1.Entity;
            _columnData.Variable = variableView1.Entity;

            DialogResult = DialogResult.OK;
        }

        private void btnCreateNewSite_Click(object sender, EventArgs e)
        {
            var site = new Site
                           {
                               Name = "NewSite",
                               Code = GetUniqueName(sitesBindingSource.List.Cast<Site>(), "Site", v => v.Code),
                           };
            AddNewItemInBindingSource(sitesBindingSource, site);
            cmbSites.SelectedIndex = sitesBindingSource.Count - 1;
            if (sitesBindingSource.Count == 1)
            {
                // In this case need manually to fire SelectedIndexChanged event
                CmbSitesOnSelectedIndexChanged(cmbSites, EventArgs.Empty);
            }
        }

        private void btnCreateNewVariable_Click(object sender, EventArgs e)
        {
            var variable = new Variable
                               {
                                   Name = "NewVariable",
                                   Code = GetUniqueName(variablesBindingSource.List.Cast<Variable>(), "Variable", v => v.Code),
                                   Speciation = "Not Applicable",
                                   SampleMedium = "Unknown",
                                   ValueType = "Unknown",
                                   DataType = "Unknown",
                                   GeneralCategory = "Unknown",
                                   TimeUnit = Unit.UnknownTimeUnit,
                                   VariableUnit = Unit.Unknown,
                                   NoDataValue = -9999,
                               };
            AddNewItemInBindingSource(variablesBindingSource, variable);
            cmbVariables.SelectedIndex = variablesBindingSource.Count - 1;
            if (variablesBindingSource.Count == 1)
            {
                // In this case need manually to fire SelectedIndexChanged event
                CmbVariablesOnSelectedIndexChanged(cmbVariables, EventArgs.Empty);
            }
        }

        private static void AddNewItemInBindingSource(BindingSource bindingSource, object newItem)
        {
            var newDataSource = new object[bindingSource.Count + 1];
            bindingSource.CopyTo(newDataSource, 0);
            newDataSource[newDataSource.Length - 1] = newItem;
            bindingSource.DataSource = newDataSource;
        }

        private static string GetUniqueName<T>(IEnumerable<T> enumerable, string initial, Func<T, string> nameGetter)
        {
            var list = enumerable.ToList();
            var uniqueNumber = 1;
            string uniqueName;
            do
            {
                uniqueName = initial + uniqueNumber;
                uniqueNumber++;
            } while (list.Any(item => nameGetter(item) == uniqueName));

            return uniqueName;
        }

        #endregion
    }

    public class DataSources
    {
        public IList<Variable> Variables { get; set; }
    }
}
