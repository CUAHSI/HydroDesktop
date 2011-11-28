using System;

namespace FacetedSearch.Settings
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
                                               HISCentralUrl = HydroDesktop.Configuration.Settings.Instance.SelectedHISCentralURL
                                           };

                    _catalogSettings.PropertyChanged += _catalogSettings_PropertyChanged;
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

        void _catalogSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case CatalogSettings.PROPERTY_HISCentralUrl:
                    HydroDesktop.Configuration.Settings.Instance.SelectedHISCentralURL = _catalogSettings.HISCentralUrl;
                    //todo: save HydroDesktop.Configuration.Settings
                    break;
                case CatalogSettings.PROPERTY_TypeOfCatalog:
                    Properties.Settings.Default.TypeOfCatalog = _catalogSettings.TypeOfCatalog;
                    Properties.Settings.Default.Save();
                    break;
            }
        }
    }
}
