using System;
using System.ComponentModel;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using Wizard.UI;

namespace DataImport.WaterML
{
    public partial class OptionsPage : InternalWizardPage
    {
        private readonly WaterMLImportSettings _settings;

        public OptionsPage(WizardContext context)
        {
            _settings = (WaterMLImportSettings) context.Settings;

            InitializeComponent();
            
            cbTheme.TextChanged += cbTheme_TextChanged;
        }

        void cbTheme_TextChanged(object sender, EventArgs e)
        {
            var themeName = cbTheme.Text;
            SetWizardButtons(String.IsNullOrEmpty(themeName) ? WizardButtons.None : WizardButtons.Next);
        }

        private void OptionsPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.None);

            //populate combo box
            var themeTable = RepositoryFactory.Instance.Get<IDataThemesRepository>().AsDataTable();

            cbTheme.DataSource = themeTable;
            cbTheme.DisplayMember = "ThemeName";
            cbTheme.ValueMember = "ThemeId";
        }

        private void OptionsPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            _settings.ThemeName = cbTheme.Text;
        }
    }
}
