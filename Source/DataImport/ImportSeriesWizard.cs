using System.Windows.Forms;
using Wizard.UI;

namespace DataImport
{
    /// <summary>
    /// Data Series Import Wizard
    /// </summary>
    public partial class ImportSeriesWizard : WizardSheet
    {
        #region Fields

        private readonly DataImportContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="ImportSeriesWizard"/>
        /// </summary>
        /// <param name="context">DataImporter</param>
        public ImportSeriesWizard(DataImportContext context)
        {
            _context = context;

            // Add pages
            foreach (var pageCreator in _context.Importer.GePageCreators())
            {
                Pages.Add(pageCreator(context));
            }

            // Wizard display options
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            base.Text = "Time Series Data Import Wizard";
        }

        #endregion
    }
}
