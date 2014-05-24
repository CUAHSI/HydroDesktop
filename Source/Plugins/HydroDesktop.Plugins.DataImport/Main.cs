using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Plugins.DataImport;
using HydroDesktop.Plugins.DataImport.Csv;
using HydroDesktop.Plugins.DataImport.Excel;
using HydroDesktop.Plugins.DataImport.Properties;
using HydroDesktop.Plugins.DataImport.Txt;
using HydroDesktop.Plugins.DataImport.WaterML;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Plugins.DataImport
{
    public class Main : Extension
    {
        #region Variables

        private readonly string TableTabKey = SharedConstants.TableRootKey;

        #endregion

        #region Properties

        /// <summary>
        /// Series View
        /// </summary>
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; private set; }

        #endregion

        #region Extension Members

        /// <summary>
        /// Activates this provider
        /// </summary>
        public override void Activate()
        {
            var importButtonTabKey = new SimpleActionItem("Import", menu_Click)
                                 {
                                     RootKey = TableTabKey,
                                     SmallImage = Resources.import_16,
                                     LargeImage = Resources.import,
                                     GroupCaption = "Data Import",
                                     ToolTipText = "Import data series into database.",
                                 };
            App.HeaderControl.Add(importButtonTabKey);

            var importButton = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "Import...", menu_Click)
                                   {
                                       GroupCaption = HeaderControl.ApplicationMenuKey,
                                       SmallImage = Resources.import_16,
                                       LargeImage = Resources.import,
                                       ToolTipText = "Import data series into database."
                                   };
            App.HeaderControl.Add(importButton);

            base.Activate();
        }

        /// <summary>
        /// Deactivates this provider
        /// </summary>
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
                context.Settings.SeriesSelector = SeriesControl;
                context.Settings.Map = (Map) App.Map;

                var wizard = new ImportSeriesWizard(context);
                wizard.ShowDialog();
            }
        }

        #endregion
    }
}
