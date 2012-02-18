using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using Wizard.UI;

namespace DataImport.WaterML
{
    public partial class OptionsPage : InternalWizardPage
    {
        private readonly WizardContext _context;
        private readonly WaterMLImportSettings _settings;

        public OptionsPage(WizardContext context)
        {
            _context = context;
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
            var dbTools = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
            var themeTable = dbTools.LoadTable("themes", "select * from DataThemeDescriptions");

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
