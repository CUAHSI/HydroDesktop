using System;

namespace Search3.Settings
{
    class SearchSettings
    {
        private SearchSettings()
        {
                
        }

        private static readonly Lazy<SearchSettings> _instance = new Lazy<SearchSettings>(() => new SearchSettings(), true);
        public static SearchSettings Instance
        {
            get { return _instance.Value; }
        }


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

        private readonly Lazy<DateSettings> _dateSettings = new Lazy<DateSettings>(() => new DateSettings());
        public DateSettings DateSettings
        {
            get { return _dateSettings.Value; }
        }

        private readonly Lazy<KeywordsSettings> _keywordsSettings = new Lazy<KeywordsSettings>(() => new KeywordsSettings());
        public KeywordsSettings KeywordsSettings
        {
            get { return _keywordsSettings.Value; }
        }

        private readonly Lazy<WebServicesSettings> _webServicesSettings = new Lazy<WebServicesSettings>(() => new WebServicesSettings(), true);
        public WebServicesSettings WebServicesSettings
        {
            get { return _webServicesSettings.Value; }
        }

        private AreaRectangle _areaRectangle;
        public AreaRectangle AreaRectangle
        {
            get { return _areaRectangle; }
            set
            {
                _areaRectangle = value;
                RaiseAreaRectangleChanged();
            }
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

        public event EventHandler AreaRectangleChanged;

        private void RaiseAreaRectangleChanged()
        {
            var handler = AreaRectangleChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
