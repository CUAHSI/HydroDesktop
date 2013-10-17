using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using HydroDesktop.Interfaces.ObjectModel;
//using HydroDesktop.DataModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Parses a WaterML response into a HydroDesktop domain object
    /// The WaterML should be in the 2.0 version.
    /// </summary>
    public class WaterOneFlow20Parser : IWaterOneFlowParser
    {

        #region Variables

        #endregion      

        /// <summary>
        /// Reads DataValues from a WaterML2.0 XML file
        /// </summary>
        /// <param name="xmlFile"></param>
        private IList<Series> ReadDataValues(XmlNodeList observations)
        {
            IList<Series> seriesList = new List<Series>();

            //loop through each series observation
            foreach (XmlNode observation in observations)
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(observation.OuterXml);

                XmlNodeList tvps = xml.GetElementsByTagName("wml2:MeasurementTVP");
                Series newSeries = new Series();

                //add Begin and End datetime to the series
                try
                {
                    newSeries.BeginDateTime = Convert.ToDateTime(xml.GetElementsByTagName("gml:beginPosition").Item(0).FirstChild.Value);
                    newSeries.EndDateTime = Convert.ToDateTime(xml.GetElementsByTagName("gml:endPosition").Item(0).FirstChild.Value);
                } catch{}

                //add each Time-Value-Pair to the series
                foreach(XmlNode tvp in tvps)
                {
                    Double value = -9999;
                    DateTime time = new DateTime();

                    //parse Time-Value-Pair
                    foreach (XmlNode child in tvp.ChildNodes)
                    {
                        if (!String.IsNullOrEmpty(child.Name))
                        {
                            if (child.Name.ToLower() == "wml2:value")
                                value = Double.Parse(child.FirstChild.Value);
                            if (child.Name.ToLower() == "wml2:time")
                                time = Convert.ToDateTime(child.FirstChild.Value);
                        }
                    }

                    //add parsed Time-Value-Pair to series
                    DataValue dataValue = new DataValue(value, time, 0);
                    newSeries.DataValueList.Add(dataValue);
                    dataValue.Series = newSeries;
                }

                //add parsed series data to list
                if(newSeries.GetValueCount() > 0)
                    seriesList.Add(newSeries);
            }

            return seriesList;
        }

        public IList<Site> ParseGetSites(string xmlFile)
        {
            throw new NotImplementedException();
        }

        public IList<Site> ParseGetSites(Stream stream)
        {
            throw new NotImplementedException();
        }

        public IList<SeriesMetadata> ParseGetSiteInfo(string xmlFile)
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

        /// <summary>
        /// Parses a WaterML TimeSeriesResponse XML file
        /// </summary>
        /// <param name="xmlFile"></param>
        public IList<Series> ParseGetValues(Stream stream)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(stream);

            Site site = null;
            Variable varInfo = null;
            IList<Series> seriesList = null;

            //get the data values and put them into a list of series
            XmlNodeList observations = xml.GetElementsByTagName("wml2:observationMember");
            seriesList = ReadDataValues(observations);

            //site = parseSite();
            //varInfo = parseVariable();

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
    }
}
