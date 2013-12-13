using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using HydroDesktop.Interfaces.ObjectModel;
using System.Globalization;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Parses a WaterML response into a HydroDesktop domain object
    /// The WaterML should be in the 2.0 version.
    /// </summary>
    public class WaterOneFlow20Parser : IWaterOneFlowParser
    {
        /// <summary>
        /// Reads DataValues from a WaterML2.0 XML file
        /// </summary>
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

        /// <summary>
        /// Reads DataValues from a Series
        /// </summary>
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
                        if (child.Name.ToLower() == "wml2:metadata" && child.InnerText != "")
                            GetNodeMetadata(child, newSeries);
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
            if(tvps.Count == 0)
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

            if(meta.Count != 0)
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

        private void GetNodeMetadata(XmlNode xmlNode, Series newSeries)
        {
            foreach (XmlNode child in xmlNode.FirstChild.ChildNodes)
            {

            }
        }


        public IList<Site> ParseGetSites(Stream stream)
        {
            throw new NotImplementedException();
        }
        

        public IList<SeriesMetadata> ParseGetSiteInfo(Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses a WaterML TimeSeriesResponse XML file
        /// </summary>
        /// <param name="xmlFile"></param>
        public IList<Series> ParseGetValues(string xmlFile)
        {
            using (var fileStream = new FileStream(xmlFile, FileMode.Open))
            {
                return ParseGetValues(fileStream);
            }
        }
    
        public IList<Series> ParseGetValues(Stream stream)
        {
            XmlDocument wmlDoc = new XmlDocument();
            wmlDoc.Load(stream);

            Site site = null;
            Variable varInfo = null;
            IList<Series> seriesList = null;

            //get the data values and put them into a list of series
            seriesList = ReadDataValues(wmlDoc);

            //site = readSite(wmlDoc);
            //varInfo = readVariable(wmlDoc);

            foreach (var series in seriesList)
            {
                if (varInfo != null)
                {
                    series.Variable = varInfo;
                }
                if (site != null)
                {
                    series.Site = site;
                }

                //ensure that properties are re-calculated
                series.UpdateSeriesInfoFromDataValues();

                //set the checked and creation date time
                series.CreationDateTime = DateTime.Now;
                series.LastCheckedDateTime = DateTime.Now;
                series.UpdateDateTime = series.LastCheckedDateTime;
            }
            return seriesList ?? (new List<Series>(0));
        }

        /// <summary>
        /// Converts the 'UTC Offset' value to a double digit in hours
        /// </summary>
        /// <param name="offsetString"></param>
        /// <returns></returns>
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
    }
}
