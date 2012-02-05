using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
                       };
        }
    }

    public class TxtImportSettings : ObservableObject<TxtImportSettings>, IDataImportSettings
    {
        public string PathToFile{get; set;}
        private TxtFileType _fileType;
        public TxtFileType FileType
        {
            get { return _fileType; }
            set
            {
                _fileType = value;
                NotifyPropertyChanged(x => x.FileType);
            }
        }
    }

    public enum TxtFileType
    {
        [Description("Fixed width")]
        FixedWidth,
        [Description("Delimited")]
        Delimited,
    }
}