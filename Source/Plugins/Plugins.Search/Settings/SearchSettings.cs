namespace HydroDesktop.Plugins.Search.Settings
{
    public class SearchSettings
    {
        #region Fields

        private CatalogSettings _catalogSettings;
        private DateSettings _dateSettings;
        private KeywordsSettings _keywordsSettings;
        private WebServicesSettings _webServicesSettings;
        private AreaSettings _areaSettings;
        private static bool _andSearch;

        #endregion

        #region Public properties

        public CatalogSettings CatalogSettings
        {
            get
            {
                return _catalogSettings ??
                       (_catalogSettings =
                           new CatalogSettings
                           {
                               TypeOfCatalog = Properties.Settings.Default.TypeOfCatalog,
                               HISCentralUrl = Properties.Settings.Default.HISCENTRAL_URL
                           });
            }
        }
        
        public DateSettings DateSettings
        {
            get { return _dateSettings ?? (_dateSettings = new DateSettings()); }
        }
        
        public KeywordsSettings KeywordsSettings
        {
            get { return _keywordsSettings?? (_keywordsSettings = new KeywordsSettings(this)); }
        }
        
        public WebServicesSettings WebServicesSettings
        {
            get { return _webServicesSettings??(_webServicesSettings = new WebServicesSettings(this)); }
        }
        
        public AreaSettings AreaSettings
        {
            get { return _areaSettings?? (_areaSettings = new AreaSettings()); }
        }

        public static bool AndSearch
        {
            get { return SearchSettings._andSearch; }
            set { SearchSettings._andSearch = value; }
        }

        #endregion
    }
}
