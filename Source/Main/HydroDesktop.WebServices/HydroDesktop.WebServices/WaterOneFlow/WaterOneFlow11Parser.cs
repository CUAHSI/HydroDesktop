using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Parses a WaterML response into a HydroDesktop domain object
    /// The WaterML should be in the 1.1 version.
    /// </summary>
    public class WaterOneFlow11Parser : IWaterOneFlowParser
    {
        #region Variables

        #endregion

        /// <summary>
        /// Parses the xml file returned by GetSites call to a WaterOneFlow
        /// web service
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public IList<Site> ParseGetSitesXml(string xmlFile)
        {
            IList<Site> siteList = new List<Site>();
            
            using (XmlTextReader reader = new XmlTextReader(xmlFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string readerName = reader.Name.ToLower();

                        if (readerName == "site")
                        {
                            //Read the site information
                            Site site = ReadSite(reader);
                            if (site != null)
                            {
                                siteList.Add(site);
                            }
                        }
                    }
                }
            }
            return siteList;
        }

        /// <summary>
        /// Parses a WaterML SiteInfo XML file
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public IList<SeriesMetadata> ParseGetSiteInfo(string xmlFile)
        {
            IList<SeriesMetadata> seriesList = new List<SeriesMetadata>();
            Site site = null;

            using (XmlTextReader reader = new XmlTextReader(xmlFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string readerName = reader.Name.ToLower();

                        if (readerName == "siteinfo")
                        {
                            //Read the site information
                            site = ReadSite(reader);   
                        }
                        else if (site != null && readerName == "series")
                        {
                            SeriesMetadata newSeries = ReadSeriesFromSiteInfo(reader, site);
                            seriesList.Add(newSeries);
                        }
                    }
                }
            }
            return seriesList;
        }
        
        /// <summary>
        /// Parses a WaterML TimeSeriesResponse XML file
        /// </summary>
        /// <param name="xmlFile"></param>
        public IList<Series> ParseGetValues(string xmlFile)
        {
            DataFile xmlFileInfo = GetDataFileInfo(xmlFile);
            
            QueryInfo qry = null;
            Site site = null;
            Variable varInfo = null;
            IList<Series> seriesList = null;
            
            using (XmlTextReader reader = new XmlTextReader(xmlFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string readerName = reader.Name.ToLower();
                        
                        if (readerName == "queryinfo")
                        {
                            //Read the 'Query Info'
                            qry = ReadQueryInfo(reader);
                            xmlFileInfo.QueryInfo = qry;
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
                            foreach (Series series in seriesList)
                            {
                                if (varInfo != null)
                                {
                                    series.Variable = varInfo;
                                }
                                if (site != null)
                                {
                                    series.Site = site;
                                }
                                CheckDataSeries(series, xmlFileInfo);
                            }
                        }
                    }
                }

                return seriesList;
            }
        }

        /// <summary>
        /// Gets information about the xml (WaterML) file
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        private DataFile GetDataFileInfo(string xmlFileName)
        {
            //create a 'data file' object
            DataFile xmlFileInfo = new DataFile();
            xmlFileInfo.FileDescription = "WaterML File";
            xmlFileInfo.FileName = System.IO.Path.GetFileName(xmlFileName);
            xmlFileInfo.FilePath = System.IO.Path.GetDirectoryName(xmlFileName);
            xmlFileInfo.FileType = "xml";
            xmlFileInfo.LoadDateTime = DateTime.Now;
            xmlFileInfo.LoadMethod = "WaterML download";
            return xmlFileInfo;
        }

        /// <summary>
        /// Reads the QueryInfo section
        /// </summary>
        private QueryInfo ReadQueryInfo(XmlTextReader r)
        {
            QueryInfo query = new QueryInfo();
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
                        query.BeginDateParameter = Convert.ToDateTime(r.Value);
                    }
                    else if (nodeName == "enddatetime")
                    {
                        r.Read();
                        query.EndDateParameter = Convert.ToDateTime(r.Value);
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && nodeName == "queryinfo")
                {
                    return query;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads information about site from the WaterML returned by GetValues
        /// </summary>
        private Site ReadSite(XmlTextReader r)
        {
            Site site = new Site();
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
                    else if (nodeName.IndexOf("sitecode") >= 0)
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
                        site.Elevation_m = Convert.ToDouble(r.Value);
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
        /// Reads information about time zone
        /// </summary>
        private TimeZoneInfo ReadTimeZoneInfo(XmlReader r)
        {
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "defaultTimeZone")
                    {
                        string zoneAbbrev = r.GetAttribute("ZoneAbbreviation");
                        string zoneOffset = r.GetAttribute("ZoneOffset");
                        int offsetHours = Convert.ToInt32(zoneOffset.Substring(0, zoneOffset.IndexOf(":")));
                        int offsetMinutes = Convert.ToInt32(zoneOffset.Substring(zoneOffset.IndexOf(":") + 1));
                        TimeSpan offsetTimeSpan = new TimeSpan(offsetHours, offsetMinutes, 0);
                        TimeZoneInfo defaultTz = TimeZoneInfo.CreateCustomTimeZone(zoneAbbrev, offsetTimeSpan, zoneAbbrev, zoneAbbrev);
                        return defaultTz;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "timeZoneInfo")
                {
                    return TimeZoneInfo.Utc;
                }
            }
            return TimeZoneInfo.Utc;
        }

        /// <summary>
        /// Reads information about time unit or variable unit
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private Unit ReadUnit(XmlTextReader r)
        {
            Unit unitInfo = Unit.Unknown;
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    string nodeName = r.Name.ToLower();
                    if (nodeName == "unitname")
                    {
                        r.Read();
                        unitInfo.Name = r.Value;
                    }
                    else if (nodeName == "unittype")
                    {
                        r.Read();
                        unitInfo.UnitsType = r.Value;
                    }
                    else if (nodeName == "unitabbreviation")
                    {
                        r.Read();
                        unitInfo.Abbreviation = r.Value;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "unit")
                {
                    return unitInfo;
                }
            }
            return unitInfo;
        }

        /// <summary>
        /// Reads information about variable
        /// </summary>
        private Variable ReadVariable(XmlTextReader r)
        {
            Variable varInfo = new Variable();
            //Unit timeUnit = Unit.Unknown;
            //Unit valueUnit = Unit.Unknown;
            varInfo.Speciation = "Not Applicable";
            varInfo.DataType = "Unknown";
            varInfo.GeneralCategory = "Unknown";
            varInfo.SampleMedium = "Unknown";
            varInfo.ValueType = "Unknown";
            
            while (r.Read())
            {
                string nodeName = r.Name.ToLower();
                
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (nodeName.IndexOf("variablecode") >= 0)
                    {
                        string prefix = r.GetAttribute("vocabulary");
                        if (string.IsNullOrEmpty(prefix))
                        {
                            prefix = r.GetAttribute("network");
                        }
                        r.Read();
                        string variableCode = r.Value;
                        if (!String.IsNullOrEmpty(prefix))
                        {
                            variableCode = prefix + ":" + variableCode;
                            varInfo.VocabularyPrefix = prefix;
                        }
                        varInfo.Code = variableCode;
                    }
                    else if (nodeName == "variablename")
                    {
                        r.Read();
                        varInfo.Name = r.Value;
                    }
                    else if (nodeName.IndexOf("valuetype") >= 0)
                    {
                        r.Read();
                        varInfo.ValueType = r.Value;
                    }
                    else if (nodeName.IndexOf("datatype") >= 0)
                    {
                        r.Read();
                        varInfo.DataType = r.Value;
                    }
                    else if (nodeName == "generalcategory")
                    {
                        r.Read();
                        varInfo.GeneralCategory = r.Value;
                    }
                    else if (nodeName == "samplemedium")
                    {
                        r.Read();
                        varInfo.SampleMedium = r.Value;
                    }
                    else if (nodeName == "speciation")
                    {
                        r.Read();
                        varInfo.Speciation = r.Value;
                    }
                    else if (nodeName == "unit")
                    {
                        Unit u = ReadUnit(r);
                        if (u.UnitsType.ToLower() == "time")
                        {
                            varInfo.TimeUnit = u;
                        }
                        else
                        {
                            varInfo.VariableUnit = u;
                        }
                    }
                    else if (nodeName == "units")
                    {
                        string unitAbbreviation = r.GetAttribute("unitsAbbreviation");
                        if (!String.IsNullOrEmpty(unitAbbreviation))
                        {
                            Unit u = Unit.Unknown;
                            u.Abbreviation = unitAbbreviation;
                            u.Name = unitAbbreviation;
                            varInfo.VariableUnit = u;
                            varInfo.TimeUnit = Unit.UnknownTimeUnit;
                        }
                        else
                        {
                            varInfo.VariableUnit = Unit.Unknown;
                            varInfo.TimeUnit = Unit.UnknownTimeUnit;
                        }
                    }

                    else if (nodeName == "nodatavalue")
                    {
                        r.Read();
                        varInfo.NoDataValue = Convert.ToDouble(r.Value);
                    }
                    else if (nodeName == "timescale") //some versions of WaterML 1.1 use 'timescale'
                    {
                        string isRegular = r.GetAttribute("isRegular");
                        if (!String.IsNullOrEmpty(isRegular))
                        {
                            varInfo.IsRegular = Convert.ToBoolean(isRegular);
                        }
                    }
                    else if (nodeName == "timesupport") //other versions use 'timesupport'
                    {
                        r.Read();
                        try
                        {
                            varInfo.TimeSupport = Convert.ToSingle(r.Value);
                        }
                        catch
                        { }
                    }
                    else if (nodeName == "timeinterval")
                    {
                        r.Read();
                        varInfo.TimeSupport = Convert.ToSingle(r.Value);
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && nodeName == "variable")
                {
                    return varInfo;
                }
            }
            return varInfo;
        }

        private SeriesMetadata ReadSeriesFromSiteInfo(XmlTextReader r, Site site)
        {
            SeriesMetadata series = new SeriesMetadata();
            series.Site = site;

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
                        series.BeginDateTime = Convert.ToDateTime(r.Value);
                        series.BeginDateTimeUTC = series.BeginDateTime;
                    }
                    else if (nodeName == "enddatetime")
                    {
                        r.Read();
                        series.EndDateTime = Convert.ToDateTime(r.Value);
                        series.EndDateTimeUTC = series.EndDateTime;
                    }
                    else if (nodeName == "begindatetimeutc")
                    {
                        r.Read();
                        series.BeginDateTimeUTC = Convert.ToDateTime(r.Value);
                    }
                    else if (nodeName == "enddatetimeutc")
                    {
                        r.Read();
                        series.EndDateTimeUTC = Convert.ToDateTime(r.Value);
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

        /// <summary>
        /// Reads the DataValues section
        /// </summary>
        private IList<Series> ReadDataValues(XmlTextReader r)
        {
            IList<DataValue> lst = new List<DataValue>();

            Dictionary<string, Qualifier> qualifiers = new Dictionary<string, Qualifier>();
            Dictionary<string, Method> methods = new Dictionary<string, Method>();
            Dictionary<string, Source> sources = new Dictionary<string, Source>();
            Dictionary<string, QualityControlLevel> qualityControlLevels = new Dictionary<string, QualityControlLevel>();
            Dictionary<string, Sample> samples = new Dictionary<string,Sample>();
            Dictionary<string, LabMethod> labMethods = new Dictionary<string, LabMethod>();
            Dictionary<string, OffsetType> offsets = new Dictionary<string, OffsetType>();
            Dictionary<string, Series> seriesDictionary = new Dictionary<string, Series>();
            
            //lookup for unique source, method, quality control level combination
            Dictionary<DataValue, string> seriesCodeLookup = new Dictionary<DataValue, string>();
            
            //lookup for samples, qualifiers and vertical offsets
            var sampleLookup = new Dictionary<DataValue, string>();
            var offsetLookup = new Dictionary<DataValue, string>();
            var qualifierLookup = new Dictionary<DataValue, string>();

            var methodLookup = new Dictionary<DataValue, string>();
            var sourceLookup = new Dictionary<DataValue, string>();
            var qualityControlLookup = new Dictionary<DataValue, string>();

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "value")
                    {
                        //create a new empty data value and add it to the list
                        DataValue val = new DataValue();
                        lst.Add(val);

                        //the default value of censor code is 'nc'
                        val.CensorCode = "nc";
                        
                        if (r.HasAttributes)
                        {
                            string censorCode = r.GetAttribute("censorCode");
                            if (!string.IsNullOrEmpty(censorCode))
                            {
                                val.CensorCode = r.GetAttribute("censorCode");
                            }
                            val.LocalDateTime = Convert.ToDateTime(r.GetAttribute("dateTime"));

                            //utcOffset
                            string utcOffset = r.GetAttribute("timeOffset");
                            if (!String.IsNullOrEmpty(utcOffset))
                            {
                                val.UTCOffset = ConvertUtcOffset(utcOffset);
                            }
                            else
                            {
                                val.UTCOffset = 0.0;
                            }

                            //dateTimeUtc
                            string dateTimeUTC = r.GetAttribute("dateTimeUTC");
                            if (!String.IsNullOrEmpty(dateTimeUTC))
                            {
                                val.DateTimeUTC = Convert.ToDateTime(dateTimeUTC);
                            }
                            else
                            {
                                val.DateTimeUTC = val.LocalDateTime;
                            }

                            //method
                            string methodID = r.GetAttribute("methodCode");
                            if (String.IsNullOrEmpty(methodID))
                            {
                                //try methodID instead of methodCode
                                methodID = r.GetAttribute("methodID");
                                if (String.IsNullOrEmpty(methodID))
                                {
                                    methodID = "unknown"; //when a method is unspecified
                                }
                            }
                            if (!methods.ContainsKey(methodID))
                            {
                                Method newMethod = Method.Unknown;
                                methods.Add(methodID, newMethod);
                            }
                            methodLookup.Add(val, methodID);

                            //quality control level
                            string qualityCode = r.GetAttribute("qualityControlLevelCode");
                            if (String.IsNullOrEmpty(qualityCode))
                            {
                                qualityCode = r.GetAttribute("qualityControlLevelID");
                                if (string.IsNullOrEmpty(qualityCode))
                                {
                                    qualityCode = "unknown"; //when the quality control level is unspecified
                                }
                            }
                            if (!qualityControlLevels.ContainsKey(qualityCode))
                            {
                                QualityControlLevel qualControl = QualityControlLevel.Unknown;
                                qualControl.Code = qualityCode;
                                qualControl.Definition = qualityCode;
                                qualControl.Explanation = qualityCode;
                                qualityControlLevels.Add(qualityCode, qualControl); 
                            }
                            qualityControlLookup.Add(val, qualityCode);

                            //source
                            string sourceID = r.GetAttribute("sourceCode");
                            if (string.IsNullOrEmpty(sourceID))
                            {
                                sourceID = r.GetAttribute("sourceID");
                                if (String.IsNullOrEmpty(sourceID))
                                {
                                    sourceID = "unknown"; //when a source is unspecified
                                }
                            }
                            if (!sources.ContainsKey(sourceID))
                            {
                                sources.Add(sourceID, Source.Unknown);
                            }
                            sourceLookup.Add(val, sourceID);

                            //----method-source-qualityControl combination----
                            string seriesCode = SeriesCode.CreateSeriesCode(methodID, qualityCode, sourceID);
                            seriesCodeLookup.Add(val, seriesCode);

                            //sample
                            string sampleCode = r.GetAttribute("labSampleCode");
                            if (!String.IsNullOrEmpty(sampleCode))
                            {
                                if (!samples.ContainsKey(sampleCode))
                                {
                                    samples.Add(sampleCode, Sample.Unknown);
                                    sampleLookup.Add(val, sampleCode);
                                }
                            }

                            //qualifiers
                            string qualifierCodes = r.GetAttribute("qualifiers");
                            if (!String.IsNullOrEmpty(qualifierCodes))
                            {
                                if (!qualifiers.ContainsKey(qualifierCodes))
                                {
                                    Qualifier newQualifier = new Qualifier();
                                    newQualifier.Code = qualifierCodes;
                                    qualifiers.Add(qualifierCodes, newQualifier);
                                    val.Qualifier = newQualifier;
                                }
                                else
                                {
                                    val.Qualifier = qualifiers[qualifierCodes];
                                }
                            }
                            
                            //vertical offset
                            string offsetCode = r.GetAttribute("offsetTypeCode");
                            if (string.IsNullOrEmpty(offsetCode))
                            {
                                offsetCode = r.GetAttribute("offsetTypeID");
                            }
                            if (!String.IsNullOrEmpty(offsetCode))
                            {
                                if (!offsets.ContainsKey(offsetCode))
                                {
                                    offsets.Add(offsetCode, new OffsetType());
                                }
                                offsetLookup.Add(val, offsetCode);

                                string offsetValue = r.GetAttribute("offsetValue");
                                if (!String.IsNullOrEmpty(offsetValue))
                                {
                                    val.OffsetValue = Convert.ToDouble(offsetValue);
                                }
                            }
                        }
                        
                        //data value
                        r.Read();
                        val.Value = r.ReadContentAsDouble();

                    }
                    else if (r.Name == "method")
                    {
                        ReadMethod(r, methods);
                    }
                    else if (r.Name == "source")
                    {
                        ReadSource(r, sources);
                    }
                    else if (r.Name == "qualityControlLevel")
                    {
                        ReadQualityControlLevel(r, qualityControlLevels);
                    }
                    else if (r.Name == "qualifier")
                    {
                        ReadQualifier(r, qualifiers);
                    }
                    else if (r.Name == "sample")
                    {
                        ReadSample(r, samples, labMethods);
                    }
                    else if (r.Name == "offset")
                    {
                        ReadOffset(r, offsets);
                    }
                }
            }

            //to assign special properties of each data value
            foreach (DataValue val in lst)
            {
                //which series does the data value belong to?
                string seriesCode = seriesCodeLookup[val];
                if (!seriesDictionary.ContainsKey(seriesCode))
                {
                    Series newSeries = new Series();
                    seriesDictionary.Add(seriesCode, newSeries);
                    //assign method, source and qual.control level
                    try
                    {
                        newSeries.Method = methods[SeriesCode.GetMethodCode(seriesCode)];
                        newSeries.Source = sources[SeriesCode.GetSourceCode(seriesCode)];
                        newSeries.QualityControlLevel = qualityControlLevels[SeriesCode.GetQualityCode(seriesCode)];
                    }
                    catch { }
                }

                //add the data value to the correct series
                Series series = seriesDictionary[seriesCode];
				
                

                series.DataValueList.Add(val);
                val.Series = series;
                
                if (sampleLookup.ContainsKey(val))
                {
                    val.Sample = samples[sampleLookup[val]];
                }
                if (offsetLookup.ContainsKey(val))
                {
                    val.OffsetType = offsets[offsetLookup[val]];
                }
                if (methodLookup.ContainsKey(val))
                {
                    Method newMethod = methods[methodLookup[val]];
                    
                    if (series.Method == null)
                    {
                        series.Method = newMethod;
                    }
                }
                if (sourceLookup.ContainsKey(val))
                {
                    Source newSource = sources[sourceLookup[val]];
                    
                    if (series.Method == null)
                    {
                        series.Source = newSource;
                    }
                }
                if (qualityControlLookup.ContainsKey(val))
                {
                    QualityControlLevel newQC = qualityControlLevels[qualityControlLookup[val]];

                    if (series.QualityControlLevel == null)
                    {
                        series.QualityControlLevel = newQC;
                    }
                }
            }

            //to check the qualifiers
            CheckQualifiers(qualifiers);

            return seriesDictionary.Values.ToList<Series>();
        }

        /// <summary>
        /// Read the vertical offset type
        /// </summary>
        private void ReadOffset(XmlReader r, Dictionary<string, OffsetType> offsets)
        {
            string offsetID = r.GetAttribute("offsetTypeID");
            if (String.IsNullOrEmpty(offsetID)) return;
            if (!offsets.ContainsKey(offsetID)) return;

            OffsetType offset = offsets[offsetID];
            offset.Unit = Unit.Unknown;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "offsetDescription")
                    {
                        r.Read();
                        offset.Description = r.Value;
                    }
                    else if (r.Name == "unitName")
                    {
                        r.Read();
                        offset.Unit.Name = r.Value;
                    }
                    else if (r.Name == "unitType")
                    {
                        r.Read();
                        offset.Unit.UnitsType = r.Value;
                    }
                    else if (r.Name == "unitAbbreviation")
                    {
                        r.Read();
                        offset.Unit.Abbreviation = r.Value;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "offset")
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Reads information about a sample
        /// </summary>
        private void ReadSample(XmlReader r, Dictionary<string, Sample> samples, Dictionary<string, LabMethod> labMethods)
        {
            Sample sample = Sample.Unknown;
            sample.LabMethod = LabMethod.Unknown;

            LabMethod newLabMethod = LabMethod.Unknown;

            string labSampleCode = String.Empty;
            
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    string nodeName = r.Name.ToLower();
                    if (nodeName == "labsamplecode")
                    {
                        r.Read();
                        labSampleCode = r.Value;
                        sample.LabSampleCode = labSampleCode;
                    }
                    else if (nodeName == "sampletype")
                    {
                        r.Read();
                        sample.SampleType = r.Value;
                    }
                    else if (nodeName == "labname")
                    {
                        r.Read();
                        newLabMethod.LabName = r.Value;
                    }
                    else if (nodeName == "laborganization")
                    {
                        r.Read();
                        newLabMethod.LabOrganization = r.Value;
                    }
                    else if (nodeName == "labmethodname")
                    {
                        r.Read();
                        newLabMethod.LabMethodName = r.Value;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name.ToLower() == "sample")
                {
                    
                    //assign the lab method
                    string labMethodKey = newLabMethod.LabName + "|" + newLabMethod.LabMethodName;
                    if (!labMethods.ContainsKey(labMethodKey))
                    {
                        labMethods.Add(labMethodKey, newLabMethod);
                        sample.LabMethod = newLabMethod;     
                    }
                    else
                    {
                        sample.LabMethod = null;
                        sample.LabMethod = labMethods[labMethodKey];
                    }

                    //check existing sample
                    if (samples.ContainsKey(labSampleCode))
                    {
                        samples[labSampleCode] = null;
                        samples[labSampleCode] = sample;
                    }
                    return;
                }
            }
            return;
        }

        /// <summary>
        /// Reads information about a qualifier
        /// </summary>
        private void ReadQualifier(XmlReader r, Dictionary<string, Qualifier> qualifiers)
        {
            string qualifierCode = r.GetAttribute("qualifierCode");
            if (String.IsNullOrEmpty(qualifierCode)) return;
            if (!qualifiers.ContainsKey(qualifierCode))
            {
                Qualifier newQualifier = new Qualifier();
                newQualifier.Code = qualifierCode;
                qualifiers.Add(qualifierCode, newQualifier);
            }

            Qualifier qualifier = qualifiers[qualifierCode];
            r.Read();
            qualifier.Description = r.Value;
        }


        /// <summary>
        /// Reads information about the quality control level and returns the quality control level object
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private QualityControlLevel ReadQualityControlLevel(XmlReader r)
        {
            QualityControlLevel qc = QualityControlLevel.Unknown;

            string qcID = r.GetAttribute("methodID");
            string qcCode = String.Empty;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name.ToLower() == "qualitycontrollevelcode")
                    {
                        r.Read();
                        qcCode = r.Value;
                    }
                    else if (r.Name.ToLower() == "definition")
                    {
                        r.Read();
                        qc.Definition = r.Value;
                    }
                    else if (r.Name == "explanation")
                    {
                        r.Read();
                        qc.Explanation = r.Value;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name.ToLower() == "qualitycontrollevel")
                {
                    if (!String.IsNullOrEmpty(qcCode))
                    {
                        qc.Code = qcCode;           
                    }
                    if (!String.IsNullOrEmpty(qcID))
                    {
                        qc.OriginId = Convert.ToInt32(qcID);
                    }
                    return  qc;
                }
            }
            return qc;
        }

        private void ReadQualityControlLevel(XmlReader r, Dictionary<string, QualityControlLevel> qcLevels)
        {
            QualityControlLevel qcLevel = ReadQualityControlLevel(r);
            string qcCodeKey = qcLevel.Code;
            if (qcLevels.ContainsKey(qcCodeKey))
            {
                qcLevels[qcCodeKey] = null;
                qcLevels[qcCodeKey] = qcLevel;
            }
        }

        /// <summary>
        /// Reads information about method and returns the method object
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private Method ReadMethod(XmlReader r)
        {
            Method method = Method.Unknown;

            string methodID = r.GetAttribute("methodID");
            string methodCode = String.Empty;
            
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name.ToLower() == "methodcode")
                    {
                        r.Read();
                        methodCode = r.Value;
                    }
                    else if (r.Name.ToLower() == "methoddescription")
                    {
                        r.Read();
                        method.Description = r.Value;
                    }
                    else if (r.Name == "methodlink")
                    {
                        r.Read();
                        method.Link = r.Value;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name.ToLower() == "method")
                {
                    if (!String.IsNullOrEmpty(methodCode))
                    {
                        method.Code = Convert.ToInt32(methodCode);
                    }
                    else if (!String.IsNullOrEmpty(methodID))
                    {
                        method.Code = Convert.ToInt32(methodID);
                    }
                    return method;
                }
            }
            return method;
        }

        /// <summary>
        /// Reads information about method
        /// </summary>
        private void ReadMethod(XmlReader r, Dictionary<string, Method> methods)
        {
            Method method = ReadMethod(r);
            string methodCodeKey = method.Code.ToString();
            if (methods.ContainsKey(methodCodeKey))
            {
                methods[methodCodeKey] = null;
                methods[methodCodeKey] = method;
            }
        }

        /// <summary>
        /// Reads information about source
        /// </summary>
        /// <param name="r">the xml reader</param>
        /// <returns>The Source object</returns>
        private Source ReadSource(XmlReader r)
        {
            string sourceID = r.GetAttribute("sourceID");
            string sourceCode = String.Empty;

            Source source = Source.Unknown;
            
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    string nodeName = r.Name.ToLower();
                    if (r.Name.ToLower() == "sourceCode")
                    {
                        r.Read();
                        sourceCode = r.Value;
                    }
                    else if (nodeName == "organization")
                    {
                        r.Read();
                        source.Organization = r.Value;
                    }
                    else if (nodeName == "contactname")
                    {
                        r.Read();
                        source.ContactName = r.Value;
                    }
                    else if (nodeName == "phone")
                    {
                        r.Read();
                        source.Phone = r.Value;
                    }
                    else if (nodeName == "email")
                    {
                        r.Read();
                        source.Email = r.Value;
                    }
                    else if (nodeName == "address")
                    {
                        r.Read();
                        source.Address = r.Value;
                    }
                    else if (nodeName == "sourcedescription")
                    {
                        r.Read();
                        source.Description = r.Value;
                    }
                    else if (nodeName == "citation")
                    {
                        r.Read();
                        source.Citation = r.Value;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name.ToLower() == "source")
                {
                    if (sourceCode != String.Empty)
                    {
                        source.OriginId = Convert.ToInt32(sourceCode);
                    }
                    else if (sourceID != String.Empty)
                    {
                        source.OriginId = Convert.ToInt32(sourceID);
                    }
                    return source;   
                }
            }
            return source;
        }

        /// <summary>
        /// Reads information about the source of the data series
        /// </summary>
        private void ReadSource(XmlReader r, Dictionary<string, Source> sources)
        {
            Source source = ReadSource(r);
            string sourceCodeKey = source.OriginId.ToString();
            if (sources.ContainsKey(sourceCodeKey))
            {
                sources[sourceCodeKey] = null;
                sources[sourceCodeKey] = source;
            }
        }


        /// <summary>
        /// Reads the spatial reference information
        /// </summary>
        private void ReadSpatialReference(XmlReader r, Site site)
        {
            SpatialReference spatialReference = new SpatialReference();
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
        /// Check compound qualifiers
        /// </summary>
        /// <param name="qualifiers"></param>
        private void CheckQualifiers(Dictionary<string, Qualifier> qualifiers)
        {
            foreach (Qualifier qual in qualifiers.Values)
            {
                if (qual.IsCompositeQualifier) //it's a 'compound qualifier'
                {
                    string[] codes = qual.Code.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    
                    string description = "";
                    
                    foreach (string code in codes)
                    {
                        Qualifier matchingQual = qualifiers[code];
                        description += matchingQual.Description + ", ";
                    }
                    description = description.Remove(description.LastIndexOf(","));
                    qual.Description = description;
                }
            }

            foreach (Qualifier qual in qualifiers.Values)
            {
                if (String.IsNullOrEmpty(qual.Description))
                {
                    qual.Description = "unknown";
                }
            }
        }

        /// <summary>
        /// Checks data series to make sure that the time zone information
        /// is correct. Also check if it is a composite series and if it is composite then
        /// separates it into multiple series.
        /// </summary>
        /// <param name="series">the data series to be checked</param>
        private void CheckDataSeries(Series series, DataFile xmlFileInfo)
        {
			//ensure that properties are re-calculated
			series.UpdateProperties ();

			if ( series.Site.DefaultTimeZone == null )
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

            //set the data file and queryInfo information
            foreach (DataValue val in series.DataValueList)
            {
                val.DataFile = xmlFileInfo;
            }
        }

        /// <summary>
        /// Converts the 'UTC Offset' value to a double digit in hours
        /// </summary>
        /// <param name="offsetString"></param>
        /// <returns></returns>
        private double ConvertUtcOffset(string offsetString)
        {
            int colonIndex = offsetString.IndexOf(":");
            double minutes = 0.0;
            double hours = 0.0;
            if (colonIndex > 0 && colonIndex < offsetString.Length - 1)
            {
                minutes = Convert.ToDouble(offsetString.Substring(colonIndex + 1));
                hours = Convert.ToDouble(offsetString.Substring(0, colonIndex));
            }
            return hours + (minutes / 60.0);
        }
    }
}
