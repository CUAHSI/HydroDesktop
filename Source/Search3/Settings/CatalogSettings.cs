using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Search3.Settings
{
    /// <summary>
    /// Catalog settings 
    /// </summary>
    public class CatalogSettings : INotifyPropertyChanged
    {
        #region Properties names

        internal const string PROPERTY_HISCentralUrl = "HISCentralUrl";
        internal const string PROPERTY_TypeOfCatalog = "TypeOfCatalog";

        #endregion

        internal const string HISCENTRAL_URL_1 = "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx";
        internal const string HISCENTRAL_URL_2 = "http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";

        private string _hisCentralUrl;
        public string HISCentralUrl
        {
            get { return _hisCentralUrl; }
            set
            {
                _hisCentralUrl = value;
                NotifyPropertyChanged(PROPERTY_HISCentralUrl);
            }
        }

        private TypeOfCatalog _typeOfCatalog;
        public TypeOfCatalog TypeOfCatalog
        {
            get { return _typeOfCatalog; }
            set
            {
                _typeOfCatalog = value;
                NotifyPropertyChanged(PROPERTY_TypeOfCatalog);
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

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
