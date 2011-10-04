using System;

namespace Search3.Settings
{
    class PluginSettings
    {
        private PluginSettings()
        {
                
        }

        private static readonly Lazy<PluginSettings> _instance = new Lazy<PluginSettings>(() => new PluginSettings(), true);
        public static PluginSettings Instance
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

        public string WebServicesXmlFileName
        {
            get { return Properties.Settings.Default.WebServicesFileName; }
        }


        private readonly Lazy<WebServicesSettings> _webServicesSettings = new Lazy<WebServicesSettings>(() => new WebServicesSettings(), true);
        public WebServicesSettings WebServicesSettings
        {
            get { return _webServicesSettings.Value; }
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
