using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using DataImport.CommonPages;
using DataImport.CommonPages.Complete;
using DataImport.CommonPages.Progress;
using DataImport.CommonPages.SelectLayer;
using DataImport.DataTableImport;
using Wizard.UI;

namespace DataImport.Txt
{
    class TxtImporter : IWizardImporter
    {
        public string Filter
        {
            get { return "Text File (*.txt)|*.txt"; }
        }

        public bool CanImportFromFile(string pathToFile)
        {
            return string.Equals(Path.GetExtension(pathToFile), ".txt", StringComparison.InvariantCultureIgnoreCase);
        }

        public IWizardImporterSettings GetDefaultSettings()
        {
            return new TxtImportSettings();
        }

        public IImporter GetImporter()
        {
            return new DataTableImporterImpl();
        }

        public ICollection<WizardPage> GetWizardPages(WizardContext context)
        {
            return new Collection<WizardPage>
                       {
                           new FormatOptionsPage(context),
                           new FieldPropertiesPage(context),
                           new DataLayerPage(context),
                           new ProgressPage(context),
                           new CompletePage(),
                       };
        }

        public void UpdatePreview(IWizardImporterSettings settings)
        {
            var txtSettings = (TxtImportSettings)settings;
            var table = ReadData(txtSettings, 10);
            txtSettings.Preview = table;
        }

        public void UpdateData(IWizardImporterSettings settings)
        {
            var txtSettings = (TxtImportSettings) settings;
            var table = ReadData(txtSettings, -1);
            txtSettings.Data = table;
        }

        #region Private methods

        private DataTable ReadData(TxtImportSettings settings, int maxRowsCount)
        {
            var fileName = settings.PathToFile;

            DataTable result;
            if (!File.Exists(fileName))
            {
                result = new DataTable();
            }
            else
            {
                switch (settings.FileType)
                {
                    case TxtFileType.Delimited:
                        result = GetPreviewForDelimitedFile(settings, maxRowsCount);
                        break;
                    case TxtFileType.FixedWidth:
                        result = GetPreviewForFixedWidthFile(settings, maxRowsCount);
                        break;
                    default:
                        result = new DataTable();
                        break;
                }
            }

            return result;
        }

        private static DataTable GetPreviewForDelimitedFile(TxtImportSettings settings, int maxRowsCount)
        {
            var fileName = settings.PathToFile;

            var result = new DataTable();
            using (var streamReader = new StreamReader(fileName))
            {
                while (streamReader.Peek() != -1 &&
                       (maxRowsCount < 0 ||
                        result.Rows.Count < maxRowsCount))
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

        private static DataTable GetPreviewForFixedWidthFile(TxtImportSettings settings, int maxRowsCount)
        {
            var fileName = settings.PathToFile;

            var result = new DataTable();
            int[] columnLengths = null;
            using (var streamReader = new StreamReader(fileName))
            {
                while (streamReader.Peek() != -1 &&
                       (maxRowsCount < 0 ||
                        result.Rows.Count < maxRowsCount))
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
}