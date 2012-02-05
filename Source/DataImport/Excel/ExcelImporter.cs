using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
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
                       };
        }

        public DataTable GetPreview(IDataImportSettings settings)
        {
            var excelSettings = (ExcelImportSettings)settings;
            return excelSettings.DataSet.Tables[excelSettings.SheetName];
        }

        public DataSet AsDataSet(IDataImportSettings settings)
        {
            var excelSettings = (ExcelImportSettings)settings;

            var fileName = excelSettings.PathToFile;
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
    }

    public class ExcelImportSettings : IDataImportSettings
    {
        public string PathToFile {get;set;}
        public string SheetName { get; set; }
        public DataSet DataSet { get; set; }
    }
}