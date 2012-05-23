using System;

namespace Search3.Settings
{
    public class SearchSettings
    {
        #region Singletone implementation

        private SearchSettings()
        {
                
        }

        private static readonly Lazy<SearchSettings> _instance = new Lazy<SearchSettings>(() => new SearchSettings(), true);
        public static SearchSettings Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        private CatalogSettings _catalogSettings;
        public CatalogSettings CatalogSettings
        {
            get
            {
                if (_catalogSettings == null)
                {
                    _catalogSettings = new CatalogSettings
                                           {
                                               TypeOfCatalog = Properties.Settings.Default.TypeOfCatalog,
                                           };
                }

                return _catalogSettings;
            }
        }

        private readonly DateSettings _dateSettings = new DateSettings();
        public DateSettings DateSettings
        {
            get { return _dateSettings; }
        }

        private readonly KeywordsSettings _keywordsSettings = new KeywordsSettings();
        public KeywordsSettings KeywordsSettings
        {
            get { return _keywordsSettings; }
        }

        private readonly WebServicesSettings _webServicesSettings = new WebServicesSettings();
        public WebServicesSettings WebServicesSettings
        {
            get { return _webServicesSettings; }
        }

        private readonly AreaSettings _areaSettings = new AreaSettings();
        public AreaSettings AreaSettings
        {
            get { return _areaSettings; }
        }
    }
}
