using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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

        public IFileImportSettings GetDefaultSettings()
        {
            return new ExcelImportSettings();
        }

        public void Import(IFileImportSettings settings)
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

        public void SetPreview(IFileImportSettings settings)
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
            excelSettings.Preview = result;
        }

        public void SetData(IFileImportSettings settings)
        {
            var excelSettings = (ExcelImportSettings)settings;
            excelSettings.Data = excelSettings.Preview;
        }

        #region Private methods

        private DataSet AsDataSet(IFileImportSettings settings)
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
}