using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Xml;
using DataImport.CommonPages;
using HydroDesktop.Common;
using Wizard.UI;

namespace DataImport.Txt
{
    class TxtImporter : IDataImporter
    {
        public string Filter
        {
            get { return "Text File (*.txt)|*.txt"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            return string.Equals(Path.GetExtension(pathToFile), ".txt", StringComparison.InvariantCultureIgnoreCase);
        }

        public IDataImportSettings GetDefaultSettings()
        {
            return new TxtImportSettings();
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
            var txtSettings = (TxtImportSettings) settings;
            var fileName = txtSettings.PathToFile;

            DataTable result;
            if (!File.Exists(fileName))
            {
                result = new DataTable();
            }
            else
            {
                switch (txtSettings.FileType)
                {
                    case TxtFileType.Delimited:
                        result = GetPreviewForDelimitedFile(txtSettings);
                        break;
                    case TxtFileType.FixedWidth:
                        result = GetPreviewForFixedWidthFile(txtSettings);
                        break;
                    default:
                        result = new DataTable();
                        break;
                }
            }
            settings.Preview = result;
        }

        #region Private methods

        private DataTable GetPreviewForDelimitedFile(TxtImportSettings settings)
        {
            var fileName = settings.PathToFile;

            var result = new DataTable();
            const int MAX_ROWS_COUNT = 10;
            using (var streamReader = new StreamReader(fileName))
            {
                while (streamReader.Peek() != -1 &&
                       result.Rows.Count < MAX_ROWS_COUNT)
                {
                    var line = streamReader.ReadLine();
                    if (String.IsNullOrEmpty(line)) continue;

                    var splitValues = line.Split(new[] { settings.Delimiter }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitValues.Length == 0) continue;

                    if (result.Columns.Count == 0)
                    {
                        foreach (var splitValue in splitValues)
                        {
                            result.Columns.Add(splitValue);
                        }
                    }
                    else
                    {
                        var row = result.NewRow();
                        var minColumn = Math.Min(splitValues.Length, result.Columns.Count);
                        for (int i = 0; i < minColumn; i++)
                        {
                            row[i] = splitValues[i];
                        }
                        result.Rows.Add(row);
                    }
                }
            }

            return result;
        }

        private DataTable GetPreviewForFixedWidthFile(TxtImportSettings settings)
        {
            var fileName = settings.PathToFile;

            var result = new DataTable();
            const int MAX_ROWS_COUNT = 10;
            int[] columnLengths = null;
            using (var streamReader = new StreamReader(fileName))
            {
                while (streamReader.Peek() != -1 &&
                       result.Rows.Count < MAX_ROWS_COUNT)
                {
                    var line = streamReader.ReadLine();
                    if (String.IsNullOrEmpty(line)) continue;

                    if (columnLengths == null)
                    {
                        var columns = line.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                        if (columns.Length == 0) continue;
                        
                        columnLengths = new int[columns.Length];
                        foreach (var column in columns)
                        {
                            result.Columns.Add(column);
                        }
                        for(int i = columns.Length -1; i>=0; i--)
                        {
                            var lastIndex = line.LastIndexOf(columns[i], StringComparison.Ordinal);
                            var columnLength = line.Length - lastIndex;
                            columnLengths[i] = columnLength;
                            line = line.Remove(lastIndex);
                        }
                    }
                    else
                    {
                        var row = result.NewRow();
                        for(int i = 0; i<columnLengths.Length; i++)
                        {
                            if (line.Length == 0)
                            {
                                break;
                            }
                            var curLen = Math.Min(columnLengths[i], line.Length);
                            var value = line.Substring(0, curLen).Trim();
                            row[i] = value;
                            line = line.Substring(curLen);
                        }
                        result.Rows.Add(row);
                    }
                }
            }

            return result;
        }

        #endregion
    }

    public class TxtImportSettings : IDataImportSettings
    {
        public string PathToFile{get; set;}
        public TxtFileType FileType { get; set; }
        public string Delimiter { get; set; }
        public DataTable Preview { get; set; }
    }

    public enum TxtFileType
    {
        [Description("Fixed width")]
        FixedWidth,

        [Description("Delimited")]
        Delimited,
    }
}