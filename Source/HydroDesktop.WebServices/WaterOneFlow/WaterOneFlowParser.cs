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

                            //ensure that properties are re-calculated
                            series.UpdateSeriesInfoFromDataValues();

                            //set the checked and creation date time
                            series.CreationDateTime = DateTime.Now;
                            series.LastCheckedDateTime = DateTime.Now;
                            series.UpdateDateTime = series.LastCheckedDateTime;
                        }
                    }
                }
            }

            return seriesList ?? (new List<Series>(0));
        }
        
        protected  virtual Site ReadSite(XmlReader r)
        {
            var site = new Site();
            while (r.Read())
            {
                var nodeName = r.Name.ToLower();

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
                    // WaterML 1.0 notes
                    else if (nodeName == "note")
                    {
                        var title = r.GetAttribute("title");
                        if (!String.IsNullOrEmpty(title))
                        {
                            title = title.ToLower();
                            r.Read();
                            var value = r.Value;
                            switch (title)
                            {
                                case "county":
                                    site.County = value;
                                    break;
                                case "state":
                                    site.State = value;
                                    break;
                                case "comments":
                                    site.Comments = value;
                                    break;
                            }
                        }
                    }
                    // WaterML 1.1 site properties
                    else if (nodeName == "siteproperty")
                    {
                        var title = r.GetAttribute("name");
                        if (!String.IsNullOrEmpty(title))
                        {
                            title = title.ToLower();
                            r.Read();
                            var value = r.Value;
                            switch (title)
                            {
                                case "county":
                                    site.County = value;
                                    break;
                                case "state":
                                    site.State = value;
                                    break;
                                case "comments":
                                    site.Comments = value;
                                    break;
                                case "sitetype":
                                    site.SiteType = value;
                                    break;
                                case "country":
                                    site.Country = value;
                                    break;
                                case "posaccuracy_m":
                                    site.PosAccuracy_m = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                                    break;
                            }
                        }
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement &&
                    (nodeName.StartsWith("source") || nodeName.StartsWith("siteinfo")))
                {
                    //ensure that spatial reference is set
                    if (site.SpatialReference == null)
                    {
                        site.SpatialReference = new SpatialReference {SRSID = 0, SRSName = "unknown"};
                    }

                    return site;
                }
            }
            return null;
        }
       
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
                else if (r.NodeType == XmlNodeType.EndElement && nodeName == "timezoneinfo")
                {
                    return defaultTz;
                }
            }
            return defaultTz;
        }
      
        protected virtual QueryInfo ReadQueryInfo(XmlReader reader)
        {
            var query = new QueryInfo();
            while (reader.Read())
            {
                var nodeName = reader.Name.ToLower();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (nodeName)
                    {
                        case "locationparam":
                            reader.Read();
                            query.LocationParameter = reader.Value;
                            break;
                        case "variableparam":
                            reader.Read();
                            query.VariableParameter = reader.ReadContentAsString();
                            break;
                        case "begindatetime":
                            reader.Read();
                            query.BeginDateParameter = Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture);
                            break;
                        case "enddatetime":
                            reader.Read();
                            query.EndDateParameter = Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture);
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && nodeName == "queryinfo")
                {
                    return query;
                }
            }
            return null;
        }

        protected virtual Method ReadMethod(XmlReader reader)
        {
            var method = Method.Unknown;
            var methodID = reader.GetAttribute("methodID");
            if (!String.IsNullOrEmpty(methodID))
            {
                method.Code = Convert.ToInt32(methodID);
            }

            if (reader.IsEmptyElement)
            {
                return method;
            }

            while (reader.Read())
            {
                var nodeName = reader.Name.ToLower();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (nodeName)
                    {
                        case "methodcode":
                            // WaterML 1.1: methodCode
                            reader.Read();
                            method.Code = Convert.ToInt32(reader.Value);
                            break;
                        case "methoddescription":
                            reader.Read();
                            method.Description = reader.Value;
                            break;
                        case "methodlink":
                            reader.Read();
                            method.Link = reader.Value;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && nodeName == "method")
                {
                    break;
                }
            }
            return method;
        }
       
        protected virtual QualityControlLevel ReadQualityControlLevel(XmlReader reader)
        {
            var qc = QualityControlLevel.Unknown;
            var qcID = reader.GetAttribute("qualityControlLevelID");
            if (!String.IsNullOrEmpty(qcID))
            {
                qc.OriginId = Convert.ToInt32(qcID);
            }

            // WaterML 1.0: QualityControlLevelType contains only one attribute without sub-elements
            if (reader.IsEmptyElement)
            {
                return qc;
            }

            // WaterML 1.1: QualityControlLevelType contains additional elements
            while (reader.Read())
            {
                var nodeName = reader.Name.ToLower();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (nodeName)
                    {
                        case "qualitycontrollevelcode":
                            reader.Read();
                            qc.Code = reader.Value;
                            break;
                        case "definition":
                            reader.Read();
                            qc.Definition = reader.Value.Trim();
                            break;
                        case "explanation":
                            reader.Read();
                            qc.Explanation = reader.Value.Trim();
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && nodeName == "qualitycontrollevel")
                {
                    break;
                }
            }
            return qc;
        }

        protected virtual Source ReadSource(XmlReader reader)
        {
            var source = Source.Unknown;
            var sourceID = reader.GetAttribute("sourceID");
            if (!String.IsNullOrEmpty(sourceID))
            {
                source.OriginId = Convert.ToInt32(sourceID);
            }

            if (reader.IsEmptyElement)
            {
                return source;
            }

            while (reader.Read())
            {
                var nodeName = reader.Name.ToLower();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (nodeName)
                    {
                        case "organization": // WML 1.0/1.1
                            reader.Read();
                            source.Organization = reader.Value;
                            break;
                        case "sourcedescription":  // WML 1.0/1.1
                            reader.Read();
                            source.Description = reader.Value;
                            break;
                        case "metadata": // WML 1.0/1.1
                            source.ISOMetadata = ReadISOMetadata(reader);
                            break;
                        case "contactinformation": // WML 1.0/1.1. Note: WML 1.1 supports many "ContactInformation" elements
                            var contact = ReadContactInformtaion(reader);

                            source.ContactName = contact.ContactName;
                            source.Email = contact.Email;
                            source.Phone = contact.Phone;

                            // Convert WaterML address into HD address
                            if (!String.IsNullOrEmpty(contact.Address))
                            {
                                //Complete address: {Address},{City},{State},{ZipCode}
                                var split = contact.Address.Split(new [] {','},
                                                                  StringSplitOptions.RemoveEmptyEntries);
                                if (split.Length > 0)
                                {
                                    source.Address = split[0].Trim();
                                }
                                if (split.Length > 1)
                                {
                                    source.City = split[1].Trim();
                                }
                                if (split.Length > 2)
                                {
                                    source.State = split[2].Trim();
                                }
                                if (split.Length > 3)
                                {
                                    int zipCode;
                                    if (Int32.TryParse(split[3].Trim(), out zipCode))
                                        source.ZipCode = zipCode;
                                }
                            }
                            break;
                        case "sourcelink":  // WML 1.0/1.1.  Note: WML 1.1 supports many "SourceLinks" elements
                            reader.Read();
                            source.Link = reader.Value;
                            break;
                        case "sourcecode": // WML 1.1
                            reader.Read();
                            break;
                        case "citation": // WML 1.1
                            reader.Read();
                            source.Citation = reader.Value;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && nodeName == "source")
                {
                    break;
                }
            }

            return source;
        }

        protected virtual ContactInformationType ReadContactInformtaion(XmlReader reader)
        {
            var contact = new ContactInformationType();

            while (reader.Read())
            {
                var nodeName = reader.Name.ToLower();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (nodeName)
                    {
                        case "contactname":
                            reader.Read();
                            contact.ContactName = reader.Value;
                            break;
                        case "typeofcontact":
                            reader.Read();
                            contact.TypeOfContact = reader.Value;
                            break;
                        case "phone":
                            reader.Read();
                            contact.Phone = reader.Value;
                            break;
                        case "email":
                            reader.Read();
                            contact.Email = reader.Value;
                            break;
                        case "address":
                            reader.Read();
                            contact.Address = reader.Value;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && nodeName == "contactinformation")
                {
                    break;
                }
            }

            return contact;
        }

        protected virtual ISOMetadata ReadISOMetadata(XmlReader reader)
        {
            var result = ISOMetadata.Unknown;

            if (reader.IsEmptyElement)
            {
                return result;
            }

            while (reader.Read())
            {
                var nodeName = reader.Name.ToLower();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (nodeName)
                    {
                        case "topiccategory": // WML 1.0/1.1
                            reader.Read();
                            result.TopicCategory = reader.Value;
                            break;
                        case "title": // WML 1.0/1.1
                            reader.Read();
                            result.Title = reader.Value;
                            break;
                        case "abstract": // WML 1.0/1.1
                            reader.Read();
                            result.Abstract = reader.Value;
                            break;
                        case "profileversion": // WML 1.0/1.1
                            reader.Read();
                            result.ProfileVersion = reader.Value;
                            break;
                        case "metadatalink": // WML 1.0/1.1
                            reader.Read();
                            result.MetadataLink = reader.Value;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && nodeName == "metadata")
                {
                    break;
                }
            }

            return result;
        }

        #endregion

        protected abstract Variable ReadVariable(XmlReader r);
        protected abstract IList<Series> ReadDataValues(XmlReader r);
    }

    /// <summary>
    /// Represents WaterML 1.0/1.1 ContactInformationType
    /// </summary>
    public class ContactInformationType
    {
        public string ContactName { get; set; }
        public string TypeOfContact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
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
