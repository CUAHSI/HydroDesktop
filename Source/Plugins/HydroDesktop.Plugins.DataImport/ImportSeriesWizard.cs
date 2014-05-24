using System.Windows.Forms;
using Wizard.UI;

namespace HydroDesktop.Plugins.DataImport
{
    /// <summary>
    /// Data Series Import Wizard
    /// </summary>
    public partial class ImportSeriesWizard : WizardSheet
    {
        #region Fields

        private readonly WizardContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="ImportSeriesWizard"/>
        /// </summary>
        /// <param name="context">DataImporter</param>
        public ImportSeriesWizard(WizardContext context)
        {
            _context = context;

            // Add pages
            foreach (var page in _context.Importer.GetWizardPages(context))
            {
                Pages.Add(page);
            }

            // Wizard display options
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            base.Text = "Time Series Data Import Wizard";
        }

        #endregion
    }
}
