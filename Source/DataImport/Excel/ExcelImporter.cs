using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using DataImport.CommonPages;
using Excel;
using Wizard.UI;

namespace DataImport.Excel
{
    class ExcelImporter : IDataImporter
    {
        public string Filter
        {
            get { return "Excel File (*.xls;*.xlsx)|*.xls;*.xlsx"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            var extension = Path.GetExtension(pathToFile);
            return string.Equals(extension, ".xlsx", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(extension, ".xls", StringComparison.InvariantCultureIgnoreCase);
        }

        public IDataImportSettings GetDefaultSettings()
        {
            return new ExcelImportSettings();
        }

        public void Import(IDataImportSettings settings)
        {
            throw new NotImplementedException();
        }

        public ICollection<Func<DataImportContext, WizardPage>> GePageCreators()
        {
            return new Collection<Func<DataImportContext, WizardPage>>
                       {
                           c => new FormatOptionsPage(c),
                           c => new FieldPropertiesPage(c),
                       };
        }

        public void SetPreview(IDataImportSettings settings)
        {
            var excelSettings = (ExcelImportSettings)settings;
            if (excelSettings.DataSet == null)
            {
                var ds = AsDataSet(settings);
                excelSettings.DataSet = ds;
                if (ds.Tables.Count > 0)
                {
                    excelSettings.SheetName = ds.Tables[0].TableName;
                }
            }

            var result = excelSettings.DataSet.Tables.Contains(excelSettings.SheetName)
                             ? excelSettings.DataSet.Tables[excelSettings.SheetName]
                             : new DataTable();
            settings.Preview = result;
        }

        #region Private methods

        private DataSet AsDataSet(IDataImportSettings settings)
        {
            var fileName = settings.PathToFile;

            if (!File.Exists(fileName))
            {
                return new DataSet();
            }

            var extension = Path.GetExtension(fileName);
            IExcelDataReader reader = null;
            var stream = File.Open(fileName, FileMode.Open);
            try
            {
                switch (extension)
                {
                    case ".xls":
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        break;
                    case ".xlsx":
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        break;
                    default:
                        goto case ".xls";
                }

                reader.IsFirstRowAsColumnNames = true;
                var ds = reader.AsDataSet();
                return ds;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                stream.Close();
            }
        }

        #endregion
    }

    public class ExcelImportSettings : IDataImportSettings
    {
        public string PathToFile {get;set;}
        public string SheetName { get; set; }
        public DataSet DataSet { get; set; }
        public DataTable Preview { get; set; }
    }
}