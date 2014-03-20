using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterML
{
    /// <summary>
    /// Parses a WaterML 2.0 files to HydroDesktop domain objects
    /// </summary>
    public class WaterML20Parser : IWaterMLParser
    {
        private static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings { IgnoreWhitespace = true, DtdProcessing = DtdProcessing.Parse };

        public IList<Site> ParseGetSites(Stream stream)
        {
            throw new NotImplementedException();
        }
        

        public IList<SeriesMetadata> ParseGetSiteInfo(Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses a WaterML 2.0 timeseries XML file
        /// </summary>
        public IList<Series> ParseGetValues(string xmlFile)
        {
            using (var fileStream = new FileStream(xmlFile, FileMode.Open))
            {
                return ParseGetValues(fileStream);
            }
        }

        public IList<Series> ParseGetValues(Stream stream)
        {
            var txtReader = new StreamReader(stream);
            using (var reader = XmlReader.Create(txtReader, _readerSettings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        var name = reader.Name.ToLower();
                        if (name == "wml2:collection")
                        {
                            var subReader = reader.ReadSubtree();
                            return ReadWMLTimeSeriesCollection(subReader).ToList();
                        }
                    }
                }
                return Enumerable.Empty<Series>().ToList();
            }
        }

        private IEnumerable<Series> ReadWMLTimeSeriesCollection(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var name = reader.Name.ToLower();
                    if (name == "wml2:observationmember")
                    {
                        Series series = null;

                        var subReader = reader.ReadSubtree();
                        while (subReader.Read())
                        {
                            if (subReader.NodeType == XmlNodeType.Element)
                            {
                                switch (subReader.Name.ToLower())
                                {
                                    case "om:result":
                                        var sub2 = subReader.ReadSubtree();
                                        while (sub2.Read())
                                        {
                                            if (sub2.NodeType == XmlNodeType.Element)
                                            {
                                                switch (sub2.Name.ToLower())
                                                {
                                                    case "wml2:timeseries":
                                                    case "wml2:measurementtimeseries":
                                                        series = ReadOneSeries(sub2.ReadSubtree());
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }

                        if (series != null) yield return series;
                    }
                }
            }
        }

        private Series ReadOneSeries(XmlReader reader)
        {
            var series = new Series();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.ToLower() == "wml2:point")
                    {
                        var subReader = reader.ReadSubtree();
                        while (subReader.Read())
                        {
                            if (subReader.NodeType == XmlNodeType.Element)
                            {
                                switch (subReader.Name.ToLower())
                                {
                                    case "wml2:timevaluepairmeasure":
                                    case "wml2:measurementtvp":
                                        var dv = ReadDataValue(subReader.ReadSubtree());
                                        series.DataValueList.Add(dv);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            
            // Set variable
            var varInfo = new Variable
            {
                Speciation = "Not Applicable",
                DataType = "Unknown",
                GeneralCategory = "Unknown",
                SampleMedium = "Unknown",
                ValueType = "Unknown",
                TimeUnit = Unit.UnknownTimeUnit,
                VariableUnit = Unit.Unknown,
                Code = "Unknown",
                Name = "Unknown",
                NoDataValue = -9999
            };
            series.Variable = varInfo;

            // Set site
            var site = new Site
            {
                Name = "Unknown",
                Code = "Unknown"
            };
            series.Site = site;

            //ensure that properties are re-calculated
            series.UpdateSeriesInfoFromDataValues();

            //set the checked and creation date time
            series.CreationDateTime = DateTime.Now;
            series.LastCheckedDateTime = DateTime.Now;
            series.UpdateDateTime = series.LastCheckedDateTime;

            return series;
        }

        private static DataValue ReadDataValue(XmlReader reader)
        {
            double value = 0;
            double utcOffset = 0;
            var dateTime = DateTime.MinValue;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "wml2:value":
                            reader.Read();
                            value = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                            break;
                        case "wml2:time":
                            reader.Read();
                            var dStr = reader.Value;
                            // without utc offset : 2008-04-05T23:00:00
                            // with utc offset    : 2005-08-05T22:30:00-07:00
                            var withUtcOffset = dStr.Count(d => d == ':') == 3;

                            const string baseFormat = "yyyy-MM-ddTHH:mm:ss";
                            var dateStr = dStr.Substring(0, baseFormat.Length);
                            dateTime = DateTime.ParseExact(dateStr, baseFormat, CultureInfo.InvariantCulture);
                            if (withUtcOffset)
                            {
                                utcOffset = ParserHelper.ConvertUtcOffset(dStr.Substring(baseFormat.Length));
                            }
                            
                            break;

                    }
                }
            }

            return new DataValue(value, dateTime, utcOffset);
        }
    }
}
