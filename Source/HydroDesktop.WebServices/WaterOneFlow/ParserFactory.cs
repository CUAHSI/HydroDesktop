using System.Globalization;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    public class ParserFactory
    {
        public IWaterOneFlowParser GetParser(DataServiceInfo dataService)
        {
            IWaterOneFlowParser parser;
            switch (dataService.Version.ToString(CultureInfo.InvariantCulture))
            {
                case "1.0":
                    parser = new WaterOneFlow10Parser();
                    break;
                default:
                    parser = new WaterOneFlow11Parser();
                    break;
            }
            return parser;
        }
    }
}