using System;
using System.Globalization;
using System.IO;
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

        public static IWaterMLParser GetParser(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            using(var txtReader = new StreamReader(fileStream))
            {
                while (txtReader.Peek() != -1)
                {
                    var line = txtReader.ReadLine();
                    if (String.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("<wml2:Collection", StringComparison.OrdinalIgnoreCase))
                        return new WaterML20Parser();
                }
            }
            return new WaterML10Parser();
        }
    }
}