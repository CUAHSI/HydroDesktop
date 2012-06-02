using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    public abstract class WaterOneFlowParser : IWaterOneFlowParser
    {
        #region Fields

        private static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings { IgnoreWhitespace = true, };

        #endregion

        #region Implementation of IWaterOneFlowParser

        public IList<Site> ParseGetSites(string xmlFile)
        {
            using (var fileStream = new FileStream(xmlFile, FileMode.Open))
            {
                return ParseGetSites(fileStream);
            }
        }

        public IList<Site> ParseGetSites(Stream stream)
        {
            var txtReader = new StreamReader(stream);
            using (var reader = XmlReader.Create(txtReader, _readerSettings))
            {
                return ReadSites(reader);
            }
        }

        public IList<SeriesMetadata> ParseGetSiteInfo(string xmlFile)
        {
            using (var fileStream = new FileStream(xmlFile, FileMode.Open))
            {
                return ParseGetSiteInfo(fileStream);
            }
        }

        public IList<SeriesMetadata> ParseGetSiteInfo(Stream stream)
        {
            var txtReader = new StreamReader(stream);
            using (var reader = XmlReader.Create(txtReader, _readerSettings))
            {
                return ReadSeriesMetadata(reader);
            }
        }

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
                return ReadValues(reader);
            }
        }

        #endregion

        #region Protected methods

        protected virtual IList<Site> ReadSites(XmlReader reader)
        {
            var siteList = new List<Site>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var readerName = reader.Name.ToLower();

                    if (readerName == "site")
                    {
                        //Read the site information
                        var site = ReadSite(reader);
                        if (site != null)
                        {
                            siteList.Add(site);
                        }
                    }
                }
            }
            return siteList;
        }

        protected virtual IList<SeriesMetadata> ReadSeriesMetadata(XmlReader reader)
        {
            IList<SeriesMetadata> seriesList = new List<SeriesMetadata>();
            Site site = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var readerName = reader.Name.ToLower();

                    if (readerName == "siteinfo")
                    {
                        //Read the site information
                        site = ReadSite(reader);
                    }
                    else if (site != null && readerName == "series")
                    {
                        var newSeries = ReadSeriesFromSiteInfo(reader, site);
                        seriesList.Add(newSeries);
                    }
                }
            }

            return seriesList;
        }

        protected virtual SeriesMetadata ReadSeriesFromSiteInfo(XmlReader r, Site site)
        {
            var series = new SeriesMetadata {Site = site};

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    string nodeName = r.Name.ToLower();
                    if (nodeName == "variable")
                    {
                        series.Variable = ReadVariable(r);
                    }
                    else if (nodeName == "valuecount")
                    {
                        r.Read();
                        series.ValueCount = Convert.ToInt32(r.Value);
                    }
                    else if (nodeName == "begindatetime")
                    {
                        r.Read();
                        series.BeginDateTime = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                        series.BeginDateTimeUTC = series.BeginDateTime;
                    }
                    else if (nodeName == "enddatetime")
                    {
                        r.Read();
                        series.EndDateTime = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                        series.EndDateTimeUTC = series.EndDateTime;
                    }
                    else if (nodeName == "begindatetimeutc")
                    {
                        r.Read();
                        series.BeginDateTimeUTC = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                    }
                    else if (nodeName == "enddatetimeutc")
                    {
                        r.Read();
                        series.EndDateTimeUTC = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                    }
                    else if (nodeName == "method")
                    {
                        series.Method = ReadMethod(r);
                    }
                    else if (nodeName == "source")
                    {
                        series.Source = ReadSource(r);
                    }
                    else if (nodeName == "qualitycontrollevel")
                    {
                        series.QualityControlLevel = ReadQualityControlLevel(r);
                    }
                }
                else
                {
                    if (r.NodeType == XmlNodeType.EndElement && r.Name == "series")
                    {
                        return series;
                    }
                }
            }
            return series;
        }

        private IList<Series> ReadValues(XmlReader reader)
        {
            Site site = null;
            Variable varInfo = null;
            IList<Series> seriesList = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string readerName = reader.Name.ToLower();

                    if (readerName == "queryinfo")
                    {
                        //Read the 'Query Info'
                        //var qry = ReadQueryInfo(reader);
                        //xmlFileInfo.QueryInfo = qry;
                    }
                    else if (readerName == "source" || readerName == "sourceinfo")
                    {
                        //Read the site information
                        site = ReadSite(reader);
                    }
                    else if (readerName == "variable")
                    {
                        //Read the variable information
                        varInfo = ReadVariable(reader);
                    }
                    else if (readerName == "values")
                    {
                        //Read the time series and data values information
                        seriesList = ReadDataValues(reader);
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
                            CheckDataSeries(series);
                        }
                    }
                }
            }

            return seriesList;
        }

        /// <summary>
        /// Reads information about site from the WaterML returned by GetValues
        /// </summary>
        protected  virtual Site ReadSite(XmlReader r)
        {
            var site = new Site();
            while (r.Read())
            {
                string nodeName = r.Name.ToLower();

                if (r.NodeType == XmlNodeType.Element)
                {
                    if (nodeName == "sitename")
                    {
                        r.Read();
                        site.Name = r.Value;
                    }
                    else if (nodeName == "geolocation")
                    {
                        ReadSpatialReference(r, site);
                    }
                    else if (nodeName.IndexOf("sitecode", StringComparison.Ordinal) >= 0)
                    {
                        string networkPrefix = r.GetAttribute("network");
                        r.Read();
                        string siteCode = r.Value;
                        if (!String.IsNullOrEmpty(networkPrefix))
                        {
                            siteCode = networkPrefix + ":" + siteCode;
                        }
                        site.Code = siteCode;
                        site.NetworkPrefix = networkPrefix;
                    }
                    else if (nodeName == "verticaldatum")
                    {
                        r.Read();
                        site.VerticalDatum = r.Value;
                    }
                    else if (nodeName == "timezoneinfo")
                    {
                        site.DefaultTimeZone = ReadTimeZoneInfo(r);
                    }

                    else if (nodeName == "elevation_m")
                    {
                        r.Read();
                        site.Elevation_m = Convert.ToDouble(r.Value, CultureInfo.InvariantCulture);
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement &&
                    (nodeName.StartsWith("source") || nodeName.StartsWith("siteinfo")))
                {
                    //ensure that spatial reference is set
                    if (site.SpatialReference == null)
                    {
                        site.SpatialReference = new SpatialReference();
                        site.SpatialReference.SRSID = 0;
                        site.SpatialReference.SRSName = "unknown";
                    }

                    return site;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the spatial reference information
        /// </summary>
        protected virtual void ReadSpatialReference(XmlReader r, Site site)
        {
            while (r.Read())
            {
                //lat long datum (srs)
                if (r.NodeType == XmlNodeType.Element && r.Name == "geogLocation")
                {
                    if (r.HasAttributes)
                    {
                        site.SpatialReference = new SpatialReference();
                        string srsName = r.GetAttribute("srs");
                        if (String.IsNullOrEmpty(srsName))
                        {
                            srsName = "unknown";
                        }
                        site.SpatialReference.SRSName = srsName;
                    }
                }

                //latitude
                if (r.NodeType == XmlNodeType.Element && r.Name == "latitude")
                {
                    r.Read();
                    site.Latitude = r.ReadContentAsDouble();
                }

                //longitude
                if (r.NodeType == XmlNodeType.Element && r.Name == "longitude")
                {
                    r.Read();
                    site.Longitude = r.ReadContentAsDouble();
                }

                //local projection
                if (r.NodeType == XmlNodeType.Element && r.Name == "localSiteXY" && r.HasAttributes)
                {
                    site.LocalProjection = new SpatialReference();
                    site.LocalProjection.SRSName = r.GetAttribute("projectionInformation");
                }

                if (r.NodeType == XmlNodeType.Element && r.Name == "X")
                {
                    r.Read();
                    site.LocalX = r.ReadContentAsDouble();
                }

                if (r.NodeType == XmlNodeType.Element && r.Name == "Y")
                {
                    r.Read();
                    site.LocalY = r.ReadContentAsDouble();
                }
                if (r.NodeType == XmlNodeType.EndElement && r.Name == "geoLocation")
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Reads information about time zone
        /// </summary>
        protected TimeZoneInfo ReadTimeZoneInfo(XmlReader r)
        {
            var defaultTz = TimeZoneInfo.Utc;
            while (r.Read())
            {
                var nodeName = r.Name.ToLower();
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (nodeName == "defaulttimezone")
                    {
                        var zoneAbbrev = r.GetAttribute("zoneabbreviation");
                        var zoneOffset = r.GetAttribute("zoneoffset");

                        if (!string.IsNullOrEmpty(zoneAbbrev) && !string.IsNullOrEmpty(zoneOffset))
                        {
                            int offsetHours = Convert.ToInt32(zoneOffset.Substring(0, zoneOffset.IndexOf(":", StringComparison.Ordinal)));
                            int offsetMinutes = Convert.ToInt32(zoneOffset.Substring(zoneOffset.IndexOf(":", StringComparison.Ordinal) + 1));
                            var offsetTimeSpan = new TimeSpan(offsetHours, offsetMinutes, 0);
                            defaultTz = TimeZoneInfo.CreateCustomTimeZone(zoneAbbrev, offsetTimeSpan, zoneAbbrev, zoneAbbrev);
                            return defaultTz;
                        }
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "timezoneinfo")
                {
                    return defaultTz;
                }
            }
            return defaultTz;
        }

        /// <summary>
        /// Reads the QueryInfo section
        /// </summary>
        protected virtual QueryInfo ReadQueryInfo(XmlReader r)
        {
            var query = new QueryInfo();
            while (r.Read())
            {
                string nodeName = r.Name.ToLower();

                if (r.NodeType == XmlNodeType.Element)
                {
                    if (nodeName == "locationparam")
                    {
                        r.Read();
                        query.LocationParameter = r.Value;
                    }
                    else if (nodeName == "variableparam")
                    {
                        r.Read();
                        query.VariableParameter = r.ReadContentAsString();
                    }
                    else if (nodeName == "begindatetime")
                    {
                        r.Read();
                        query.BeginDateParameter = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                    }
                    else if (nodeName == "enddatetime")
                    {
                        r.Read();
                        query.EndDateParameter = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && nodeName == "queryinfo")
                {
                    return query;
                }
            }
            return null;
        }

        #endregion

        #region Private methods


        /// <summary>
        /// Checks data series to make sure that the time zone information
        /// is correct. Also check if it is a composite series and if it is composite then
        /// separates it into multiple series.
        /// </summary>
        /// <param name="series">the data series to be checked</param>
        private void CheckDataSeries(Series series)
        {
            //ensure that properties are re-calculated
            series.UpdateProperties();

            if (series.Site.DefaultTimeZone == null)
            {
                series.Site.DefaultTimeZone = TimeZoneInfo.Utc;
            }

            //check the time zone and assign the 'UTC Offset'
            if (series.Site.DefaultTimeZone != TimeZoneInfo.Utc)
            {
                TimeSpan utcOffset = series.Site.DefaultTimeZone.BaseUtcOffset;
                double utcOffsetHours = utcOffset.TotalHours;
                series.BeginDateTimeUTC = series.BeginDateTime + utcOffset;
                series.EndDateTimeUTC = series.EndDateTime + utcOffset;
                foreach (DataValue val in series.DataValueList)
                {
                    val.UTCOffset = utcOffsetHours;
                    val.DateTimeUTC = val.LocalDateTime + utcOffset;
                }
            }
            else
            {
                series.BeginDateTimeUTC = series.BeginDateTime;
                series.EndDateTimeUTC = series.EndDateTime;
            }

            //set the checked and creation date time
            series.CreationDateTime = DateTime.Now;
            series.LastCheckedDateTime = DateTime.Now;
            series.UpdateDateTime = series.LastCheckedDateTime;
        }

        #endregion

        protected abstract Variable ReadVariable(XmlReader r);
        protected abstract Method ReadMethod(XmlReader r);
        protected abstract Source ReadSource(XmlReader r);
        protected abstract QualityControlLevel ReadQualityControlLevel(XmlReader r);
        protected abstract IList<Series> ReadDataValues(XmlReader r);
    }

    class DataValueWrapper
    {
        public DataValue DataValue { get; set; }
        public string SeriesCode { get; set; }
        public string SourceID { get; set; }
        public string MethodID { get; set; }
        public string OffsetID { get; set; }
        public string SampleID { get; set; }
        public string QualityID { get; set; }
    }
}