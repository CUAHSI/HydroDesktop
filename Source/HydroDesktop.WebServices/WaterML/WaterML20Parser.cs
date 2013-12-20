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
                        var subReader = reader.ReadSubtree();
                        while (subReader.Read())
                        {
                            if (subReader.NodeType == XmlNodeType.Element &&
                                subReader.Name.ToLower() == "om:result")
                            {
                                var sub2 = subReader.ReadSubtree();
                                while (sub2.Read())
                                {
                                    if (sub2.NodeType == XmlNodeType.Element &&
                                        sub2.Name.ToLower() == "wml2:timeseries")
                                        yield return ReadOneSeries(sub2.ReadSubtree());  
                                }
                                subReader.Skip();
                            }
                        }
                        reader.Skip();
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
                                        series.DataValueList.Add(new DataValue());
                                }
                            }
                        }

                        reader.Skip();
                    }
                }
            }

            //ensure that properties are re-calculated
            series.UpdateSeriesInfoFromDataValues();

            //set the checked and creation date time
            series.CreationDateTime = DateTime.Now;
            series.LastCheckedDateTime = DateTime.Now;
            series.UpdateDateTime = series.LastCheckedDateTime;

            return series;
        }

        #region Old

        public IList<Series> ParseGetValuesOld(Stream stream)
        {
            XmlDocument wmlDoc = new XmlDocument();
            wmlDoc.Load(stream);

            //get the data values and put them into a list of series
            var seriesList = ReadDataValues(wmlDoc);

            //site = readSite(wmlDoc);
            //varInfo = readVariable(wmlDoc);

            foreach (var series in seriesList)
            {
                //ensure that properties are re-calculated
                series.UpdateSeriesInfoFromDataValues();

                //set the checked and creation date time
                series.CreationDateTime = DateTime.Now;
                series.LastCheckedDateTime = DateTime.Now;
                series.UpdateDateTime = series.LastCheckedDateTime;
            }
            return seriesList;
        }

        private IList<Series> ReadDataValues(XmlDocument wmlDoc)
        {
            IList<Series> seriesList = new List<Series>();
            XmlNodeList observations = wmlDoc.GetElementsByTagName("wml2:observationMember");

            if (observations.Count == 0)
                observations = wmlDoc.GetElementsByTagName("om:OM_Observation");

            if (observations.Count == 0)
            {
                Series newSeries = new Series();
                newSeries = ReadDataSeries(wmlDoc);

                //add parsed series data to list
                if (newSeries.GetValueCount() > 0)
                    seriesList.Add(newSeries);
            }

            //loop through each series observation
            foreach (XmlNode observation in observations)
            {
                Series newSeries = new Series();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(observation.OuterXml);
                newSeries = ReadDataSeries(xml);

                //add parsed series data to list
                if (newSeries.GetValueCount() > 0)
                    seriesList.Add(newSeries);
            }

            return seriesList;
        }

        private Series ReadDataSeries(XmlDocument xml)
        {
            Series newSeries = new Series();
            XmlNodeList tvps = GetTVPList(xml);
            GetSeriesMetadata(xml, newSeries);
            GetDefaultNodeMetadata(xml, newSeries);

            //add Begin and End datetime to the series
            try
            {
                XmlNode begin = xml.GetElementsByTagName("gml:beginPosition").Item(0);
                XmlNode end = xml.GetElementsByTagName("gml:endPosition").Item(0);

                if (begin.InnerText.Length > 19)
                    newSeries.BeginDateTime = Convert.ToDateTime(begin.InnerText.Remove(19));
                else
                    newSeries.BeginDateTime = Convert.ToDateTime(begin.InnerText);

                if (end.InnerText.Length > 19)
                    newSeries.EndDateTime = Convert.ToDateTime(end.InnerText.Remove(19));
                else
                    newSeries.EndDateTime = Convert.ToDateTime(end.InnerText);
            }
            catch { }

            //add each Time-Value-Pair to the series
            foreach (XmlNode tvp in tvps)
            {
                Double value = -9999;
                DateTime time = new DateTime();
                double utcOffset = 0;

                //parse Time-Value-Pair
                foreach (XmlNode child in tvp.ChildNodes)
                {
                    if (!String.IsNullOrEmpty(child.Name))
                    {
                        if (child.Name.ToLower() == "wml2:value" && child.InnerText != "")
                            value = Double.Parse(child.InnerText);
                        if (child.Name.ToLower() == "wml2:time" && child.InnerText != "")
                        {
                            String dateTime = child.InnerText;
                            String utcoffset = "Z";
                            if (dateTime.Length > 19)
                            {
                                utcoffset = dateTime.Substring(19);
                                dateTime = dateTime.Remove(19);
                            }
                            time = Convert.ToDateTime(dateTime);
                            if (utcoffset.ToLower() == "z")
                                utcOffset = 0;
                            else
                                utcOffset = ConvertUtcOffset(utcoffset);
                        }
                    }
                }

                //add parsed Time-Value-Pair to series
                DataValue dataValue = new DataValue(value, time, utcOffset);
                newSeries.DataValueList.Add(dataValue);
                dataValue.Series = newSeries;
            }
            return newSeries;
        }

        private XmlNodeList GetTVPList(XmlDocument xml)
        {
            XmlNodeList tvps = xml.GetElementsByTagName("wml2:MeasurementTVP");
            if (tvps.Count == 0)
                tvps = xml.GetElementsByTagName("wml2:CategoricalTVP");
            if (tvps.Count == 0)
                tvps = xml.GetElementsByTagName("wml2:TimeValuePair");

            return tvps;
        }

        private void GetSeriesMetadata(XmlDocument xml, Series newSeries)
        {
            XmlNodeList meta = xml.GetElementsByTagName("wml2:MeasurementTimeseriesMetadata");
            if (meta.Count == 0)
                meta = xml.GetElementsByTagName("wml2:TimeseriesMetadata");
            if (meta.Count == 0)
                meta = xml.GetElementsByTagName("wml2:CategoricalTimeseriesMetadata");
        }

        private void GetDefaultNodeMetadata(XmlDocument xml, Series newSeries)
        {
            XmlNodeList meta = xml.GetElementsByTagName("wml2:DefaultTVPMeasurementMetadata");
            if (meta.Count == 0)
                meta = xml.GetElementsByTagName("wml2:DefaultTVPCategoricalMetadata");
            if (meta.Count == 0)
                meta = xml.GetElementsByTagName("wml2:DefaultTVPMetadata");

            if (meta.Count != 0)
            {
                foreach (XmlNode child in meta.Item(0).ChildNodes)
                {
                    if (child.Name == "wml2:quality")
                    {

                    }
                    if (child.Name == "wml2:qualifier")
                    {

                    }
                    if (child.Name == "wml2:processing")
                    {
                    }
                    if (child.Name == "wml2:uom")
                    {

                    }
                    if (child.Name == "wml2:interpolationType")
                    {
                        foreach (XmlAttribute attribute in child.Attributes)
                        {
                            if (attribute.Name == "xlink:href")
                            {
                                string interpolationType = attribute.InnerText.Split('/').Last();
                                if (newSeries.Variable == null)
                                    newSeries.Variable = new Variable();
                                newSeries.Variable.DataType = interpolationType;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Converts the 'UTC Offset' value to a double digit in hours
        /// </summary>
        private double ConvertUtcOffset(string offsetString)
        {
            int colonIndex = offsetString.IndexOf(":", StringComparison.Ordinal);
            double minutes = 0.0;
            double hours = 0.0;
            if (colonIndex > 0 && colonIndex < offsetString.Length - 1)
            {
                minutes = Convert.ToDouble(offsetString.Substring(colonIndex + 1), CultureInfo.InvariantCulture);
                hours = Convert.ToDouble((offsetString.Substring(0, colonIndex)), CultureInfo.InvariantCulture);
            }
            return hours + (minutes / 60.0);
        }

        #endregion
    }
}
