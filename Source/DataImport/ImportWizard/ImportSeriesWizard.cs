using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wizard.UI;

namespace DataImport.ImportWizard
{
    /// <summary>
    /// Data Series Import Wizard
    /// </summary>
    public class ImportSeriesWizard : WizardSheet
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
        }

        #endregion
    }
}
