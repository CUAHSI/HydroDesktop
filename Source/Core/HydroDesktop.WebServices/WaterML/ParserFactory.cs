using System.Globalization;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterML
{
    public static class ParserFactory
    {
        public static IWaterMLParser GetParser(DataServiceInfo dataService)
        {
            IWaterMLParser parser;
            switch (dataService.Version.ToString("F1", CultureInfo.InvariantCulture))
            {
                case "1.0":
                    parser = new WaterML10Parser();
                    break;
                case "1.1":
                    parser = new WaterML11Parser();
                    break;
                case "2.0":
                    parser = new WaterML20Parser();
                    break;
                default:
                    parser = new WaterML11Parser();
                    break;
            }
            return parser;
        }
    }
}