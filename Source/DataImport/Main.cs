using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DataImport;
using DataImport.Csv;
using DataImport.Excel;
using DataImport.Properties;
using DataImport.Txt;
using DataImport.WaterML;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Interfaces;

namespace ImportFromWaterML
{
    public class Main : Extension
    {
        #region Variables
        
        private const string TableTabKey = "kHydroTable";

        #endregion

        #region Properties

        /// <summary>
        /// Series View
        /// </summary>
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; private set; }

        #endregion

        #region Extension Members

        public override void Activate()
        {
            var importButtonTabKey = new SimpleActionItem("Import", menu_Click)
                                 {
                                     RootKey = TableTabKey,
                                     SmallImage = Resources.waterml_import1,
                                     LargeImage = Resources.waterml_import1,
                                     GroupCaption = "Data Import",
                                     ToolTipText = "Import data series into database.",
                                 };
            App.HeaderControl.Add(importButtonTabKey);

            var importButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Import...", menu_Click)
                                   {
                                       GroupCaption = HeaderControl.ApplicationMenuKey,
                                       SmallImage = Resources.waterml_import1,
                                       LargeImage = Resources.waterml_import1,
                                       ToolTipText = "Import data series into database."
                                   };
            App.HeaderControl.Add(importButton);

            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }

        #endregion

        #region Private methods

        void menu_Click(object sender, EventArgs e)
        {
            var importers = new List<IWizardImporter>
                                {
                                    new CsvImporter(),
                                    new ExcelImporter(),
                                    new TxtImporter(),
                                    new WaterMLImporter(),
                                };

            using (var dialog = new OpenFileDialog())
            {
                var filter = string.Join("|", importers.Select(item => item.Filter)) +
                             "|All files (*.*)|*.*";

                dialog.Filter = filter;
                dialog.Title = "Select file to import";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.AutoUpgradeEnabled = true;

                if (dialog.ShowDialog() != DialogResult.OK) return;

                var fileName = dialog.FileName;
                var importer = importers.FirstOrDefault(imp => imp.CanImportFromFile(fileName)) ??
                               importers.OfType<TxtImporter>().First();
                Debug.Assert(importer != null);

                var context = new WizardContext();
                context.Importer = importer;
                context.Settings = importer.GetDefaultSettings();
                context.Settings.PathToFile = fileName;

                var wizard = new ImportSeriesWizard(context);
                wizard.ShowDialog();

                SeriesControl.RefreshSelection();
            }
        }

        #endregion
    }
}
