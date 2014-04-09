using System;

namespace FacetedSearch3.Settings
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



        private readonly DateSettings _dateSettings = new DateSettings();
        public DateSettings DateSettings
        {
            get { return _dateSettings; }
        }



        private readonly AreaSettings _areaSettings = new AreaSettings();
        public AreaSettings AreaSettings
        {
            get { return _areaSettings; }
        }
    }
}
