using System;
using System.Collections.Generic;
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

namespace ImportFromWaterML
{
    public class Main : Extension
    {
        #region Variables
        
        private const string TableTabKey = "kHydroTable";

        #endregion

        #region Extension Members

        public override void Activate()
        {
            var btnWaterML = new SimpleActionItem("WaterML", menu_Click);
            btnWaterML.RootKey = TableTabKey;
            btnWaterML.LargeImage = Resources.waterml_import1;
            btnWaterML.GroupCaption = "Data Import";
            App.HeaderControl.Add(btnWaterML);

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
            var importers = new List<IDataImporter>
                                {
                                    new WaterMLImporter(),
                                    new CsvImporter(),
                                    new TxtImporter(),
                                    new ExcelImporter()
                                };

            using(var dialog = new OpenFileDialog())
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
                foreach (var imp in importers.Where(imp => imp.CanImportFromFile(fileName)))
                {
                    var context = new DataImportContext();
                    context.Importer = imp;
                    context.Settings = imp.GetDefaultSettings();
                    context.Settings.PathToFile = fileName;

                    var wizard = new ImportSeriesWizard(context);
                    wizard.ShowDialog();
                    
                    break;
                }
            }
        }

        #endregion
    }
}
