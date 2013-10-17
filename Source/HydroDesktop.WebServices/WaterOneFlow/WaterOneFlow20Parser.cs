﻿using System;
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
        /// <param name="XmlNodeList"></param>
        private IList<Series> ReadDataValues(XmlDocument wmlDoc)
        {
            IList<Series> seriesList = new List<Series>();
            XmlNodeList observations = wmlDoc.GetElementsByTagName("wml2:observationMember");

            if (observations.Count == 0)
            {
                Series newSeries = new Series();
                newSeries = ReadDataSeries(wmlDoc);

                //add parsed series data to list
                if (newSeries.GetValueCount() > 0)
                    seriesList.Add(newSeries);
            }
            else
            {
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
            }

            return seriesList;
        }

        /// <summary>
        /// Reads DataValues from a Series
        /// </summary>
        /// <param name="XmlNodeList"></param>
        private Series ReadDataSeries(XmlDocument xml)
        {
            Series newSeries = new Series();
            XmlNodeList tvps = xml.GetElementsByTagName("wml2:MeasurementTVP");

            //add Begin and End datetime to the series
            try
            {
                XmlNode begin = xml.GetElementsByTagName("gml:beginPosition").Item(0);
                XmlNode end = xml.GetElementsByTagName("gml:endPosition").Item(0);
                newSeries.BeginDateTime = Convert.ToDateTime(begin.FirstChild.Value);
                newSeries.EndDateTime = Convert.ToDateTime(end.FirstChild.Value);
            }
            catch { }

            //add each Time-Value-Pair to the series
            foreach (XmlNode tvp in tvps)
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
            return newSeries;
        }

        /// <summary>
        /// Reads DataValues from a WaterML2.0 XML file
        /// </summary>
        /// <param name="XmlNodeList"></param>
        private Site ReadSite(XmlDocument wmlDoc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads DataValues from a WaterML2.0 XML file
        /// </summary>
        /// <param name="XmlNodeList"></param>
        private Variable ReadVariable(XmlDocument wmlDoc)
        {
            throw new NotImplementedException();
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
    }
}
