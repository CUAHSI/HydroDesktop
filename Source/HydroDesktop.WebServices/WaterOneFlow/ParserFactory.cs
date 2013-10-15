using System.Globalization;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    public class ParserFactory
    {
        public IWaterOneFlowParser GetParser(DataServiceInfo dataService)
        {
            IWaterOneFlowParser parser;
            switch (dataService.Version.ToString("F1", CultureInfo.InvariantCulture))
            {
                case "1.0":
                    parser = new WaterOneFlow10Parser();
                    break;
                case "1.1":
                    parser = new WaterOneFlow11Parser();
                    break;
                case "2.0":
                    parser = new WaterOneFlow20Parser();
                    break;
                default:
                    parser = new WaterOneFlow11Parser();
                    break;
            }
            return parser;
        }
    }
}