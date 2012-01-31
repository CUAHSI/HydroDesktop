using System;
using System.ComponentModel;
using HydroDesktop.Common;

namespace Search3.Settings
{
    /// <summary>
    /// Catalog settings 
    /// </summary>
    public class CatalogSettings : ObservableObject<CatalogSettings>
    {
        internal const string HISCENTRAL_URL_1 = "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx";
        internal const string HISCENTRAL_URL_2 = "http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";

        private string _hisCentralUrl;
        public string HISCentralUrl
        {
            get { return _hisCentralUrl; }
            set
            {
                _hisCentralUrl = value;
                NotifyPropertyChanged(x => HISCentralUrl);
            }
        }

        private TypeOfCatalog _typeOfCatalog;
        public TypeOfCatalog TypeOfCatalog
        {
            get { return _typeOfCatalog; }
            set
            {
                _typeOfCatalog = value;
                NotifyPropertyChanged(x => TypeOfCatalog);
            }
        }

        public CatalogSettings()
        {
            TypeOfCatalog = TypeOfCatalog.HisCentral;
            HISCentralUrl = HISCENTRAL_URL_1;
        }

        /// <summary>
        /// Create deep copy of current instance.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public CatalogSettings Copy()
        {
            var result = new CatalogSettings();
            result.Copy(this);
            return result;

        }

        /// <summary>
        /// Create deep from source into current instance.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>must be not null.</exception>
        public void Copy(CatalogSettings source)
        {
            if (source == null) throw new ArgumentNullException("source");

            HISCentralUrl = source.HISCentralUrl;
            TypeOfCatalog = source.TypeOfCatalog;
        }
    }

    public enum TypeOfCatalog
    {
        [Description("HIS Central")]
        HisCentral,
        [Description("Local Metadata Cache")]
        LocalMetadataCache
    }
}
