using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages
{
    /// <summary>
    /// Allow to modify properties of field to import
    /// </summary>
    public partial class FieldPropertiesForm : Form
    {
        #region Fields

        private readonly ColumnData _columnData;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="FieldPropertiesForm"/>
        /// </summary>
        /// <param name="columnData">Column data</param>
        public FieldPropertiesForm(ColumnData columnData)
        {
            if (columnData == null) throw new ArgumentNullException("columnData");
            Contract.EndContractBlock();

            _columnData = columnData;
            InitializeComponent();

            if (DesignMode) return;

            // Set bindings.......

            // Site
            siteView1.ReadOnly = true;
            cmbSites.SelectedIndexChanged += CmbSitesOnSelectedIndexChanged;
            var sitesRepo = RepositoryFactory.Instance.Get<ISitesRepository>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);
            var sites = sitesRepo.GetAll();
            if (_columnData.Site != null &&
                !Array.Exists(sites, site => _columnData.Site == site))
            {
                Array.Resize(ref sites, sites.Length + 1);
                sites[sites.Length - 1] = _columnData.Site;
            }
            sitesBindingSource.DataSource = sites; 
            cmbSites.DataSource = sitesBindingSource;
            cmbSites.DisplayMember = NameHelper.Name<Site, object>(s => s.Name);
            if (_columnData.Site != null)
                cmbSites.SelectedItem = _columnData.Site;
            
            // Variable
            variableView1.ReadOnly = true;
            cmbVariables.SelectedIndexChanged += CmbVariablesOnSelectedIndexChanged;
            var variablesRepo = RepositoryFactory.Instance.Get<IVariablesRepository>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);
            var variables = variablesRepo.GetAll();
            if (_columnData.Variable != null &&
               !Array.Exists(variables, v => _columnData.Variable == v))
            {
                Array.Resize(ref variables, variables.Length + 1);
                variables[variables.Length - 1] = _columnData.Variable;
            }
            variablesBindingSource.DataSource = variables;
            cmbVariables.DataSource = variablesBindingSource;
            cmbVariables.DisplayMember = NameHelper.Name<Variable, object>(s => s.Name);
            if (_columnData.Variable != null)
                cmbVariables.SelectedItem = _columnData.Variable;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current column data
        /// </summary>
        public ColumnData ColumnData
        {
            get { return _columnData; }
        }

        #endregion

        #region Private methods

        private void CmbVariablesOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var variablesRepo = RepositoryFactory.Instance.Get<IVariablesRepository>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);

            var currentVar = (Variable)cmbVariables.SelectedItem;
            variableView1.Entity = currentVar;
            variableView1.ReadOnly = currentVar == null || variablesRepo.Exists(currentVar);
        }

        private void CmbSitesOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            var sitesRepo = RepositoryFactory.Instance.Get<ISitesRepository>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);

            var currentSite = (Site)cmbSites.SelectedItem;
            siteView1.Entity = currentSite;
            siteView1.ReadOnly = currentSite == null || sitesRepo.Exists(currentSite);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var error = siteView1.EntityValidate();
            if (string.IsNullOrEmpty(error))
                error = variableView1.EntityValidate();

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
            //Note: model form case
            /*
            Site site;
            using (var form = new CreateSiteForm())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                site = form.Entity;
            }*/

            var site =  new Site { Name = "NewSite", Code = "Site1" };
            var sites = new Site[sitesBindingSource.Count + 1];
            sitesBindingSource.CopyTo(sites, 0);
            sites[sites.Length - 1] = site;
            
            sitesBindingSource.DataSource = sites;
            cmbSites.SelectedIndex = sites.Length - 1;
            if (sites.Length == 1)
            {
                // In this case need manually to fire SelectedIndexChanged event
                CmbSitesOnSelectedIndexChanged(cmbSites, EventArgs.Empty);
            }
        }

        private void btnCreateNewVariable_Click(object sender, EventArgs e)
        {
            //Note: modal form case
            /*
            Variable variable;
            using (var form = new CreateVariableForm())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                variable = form.Entity;
            }
            */

            var variable = new Variable { Name = "NewVariable", Code = "Variable1" };
            var variables = new Variable[variablesBindingSource.Count + 1];
            variablesBindingSource.CopyTo(variables, 0);
            variables[variables.Length - 1] = variable;

            variablesBindingSource.DataSource = variable;
            cmbVariables.SelectedIndex = variables.Length - 1;
            if (variables.Length == 1)
            {
                // In this case need manually to fire SelectedIndexChanged event
                CmbVariablesOnSelectedIndexChanged(cmbSites, EventArgs.Empty);
            }
        }

        #endregion
    }
}
