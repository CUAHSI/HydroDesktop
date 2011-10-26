using HydroDesktop.Configuration;

namespace HydroDesktop.Search
{
    static class Global
    {
        public static string GetHISCentralURL()
        {
            return Settings.Instance.SelectedHISCentralURL;
        }

        internal static readonly string SEARCH_RESULT_LAYER_NAME = Resources.SEARCH_RESULT_LAYER_NAME;
    }
}
