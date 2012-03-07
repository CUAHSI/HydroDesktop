using HydroDesktop.Configuration;

namespace HydroDesktop.Search
{
    static class Global
    {
        public static string GetHISCentralURL()
        {
            return Settings.Instance.SelectedHISCentralURL;
        }
    }
}
