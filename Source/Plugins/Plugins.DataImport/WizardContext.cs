namespace HydroDesktop.Plugins.DataImport
{
    /// <summary>
    /// Context for wizards
    /// </summary>
    public class WizardContext
    {
        /// <summary>
        /// Wizard importer
        /// </summary>
        public IWizardImporter Importer { get; set; }

        /// <summary>
        /// Wizard settings
        /// </summary>
        public IWizardImporterSettings Settings { get; set; }
    }
}