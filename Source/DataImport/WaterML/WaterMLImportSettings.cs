using System.Collections.Generic;
using System.Data;

namespace DataImport.WaterML
{
    /// <summary>
    /// WaterML Import Settings
    /// </summary>
    public class WaterMLImportSettings : IWizardImporterSettings
    {
        public string PathToFile { get; set; }

        /// <summary>
        /// Theme name
        /// </summary>
        public string ThemeName { get; set; }

        public IList<ColumnInfo> ColumnDatas
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public string DateTimeColumn
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public DataTable Data
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public DataTable Preview
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}