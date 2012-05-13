using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Parses a WaterML response into a HydroDesktop domain object
    /// The WaterML should be in the 1.0 version.
    /// </summary>
    public class WaterOneFlow10Parser : IWaterOneFlowParser
    {
        #region Variables

        private static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings {IgnoreWhitespace = true,};

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

            using (var reader = XmlReader.Create(xmlFile, _readerSettings))
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

            using (var reader = XmlReader.Create(xmlFile, _readerSettings))
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

            using (var reader =  XmlReader.Create(xmlFile, _readerSettings))
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
        private QueryInfo ReadQueryInfo(XmlReader r)
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
                        query.BeginDateParameter = Convert.ToDateTime(r.Value,CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Reads information about site from the WaterML returned by GetValues
        /// </summary>
        private Site ReadSite(XmlReader r)
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
            string zoneAbbrev = string.Empty;
            string zoneOffset = string.Empty;
            TimeZoneInfo defaultTz = TimeZoneInfo.Utc;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "defaultTimeZone")
                    {
                        if (r.HasAttributes)
                        {
                            while(r.MoveToNextAttribute())
                            {
                                if (r.Name.ToLower() == "zoneabbreviation")
                                {
                                    zoneAbbrev = r.Value;
                                }

                                if (r.Name.ToLower() == "zoneoffset")
                                {
                                    zoneOffset = r.Value;
                                }
                            }
                            r.MoveToElement();
                        }

                        if (!string.IsNullOrEmpty(zoneAbbrev) && !string.IsNullOrEmpty(zoneOffset))
                        {
                            int offsetHours = Convert.ToInt32(zoneOffset.Substring(0, zoneOffset.IndexOf(":")));
                            int offsetMinutes = Convert.ToInt32(zoneOffset.Substring(zoneOffset.IndexOf(":") + 1));
                            TimeSpan offsetTimeSpan = new TimeSpan(offsetHours, offsetMinutes, 0);
                            defaultTz = TimeZoneInfo.CreateCustomTimeZone(zoneAbbrev, offsetTimeSpan, zoneAbbrev, zoneAbbrev);
                            return defaultTz;
                        }
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "timeZoneInfo")
                {
                    return defaultTz;
                }
            }
            return defaultTz;
        }

        /// <summary>
        /// Reads information about variable
        /// </summary>
        private Variable ReadVariable(XmlReader r)
        {
            Variable varInfo = new Variable();
            Unit timeUnit = Unit.Unknown;
            Unit valueUnit = Unit.Unknown;
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
                    else if (nodeName == "units")
                    {
                        string abrev = r.GetAttribute("unitsAbbreviation");
                        if (String.IsNullOrEmpty(abrev))
                        {
                            abrev = "unknown";
                        }

                        string unitType = r.GetAttribute("unitsType");
                        if (String.IsNullOrEmpty(unitType))
                        {
                            unitType = "unknown";
                        }
                        r.Read();
                        string unitName = r.Value;
                        if (String.IsNullOrEmpty(unitName))
                        {
                            unitName = "unknown";
                        }

                        valueUnit.Abbreviation = abrev;
                        valueUnit.UnitsType = unitType;
                        valueUnit.Name = unitName;
                    }
                    else if (nodeName == "nodatavalue")
                    {
                        r.Read();
                        varInfo.NoDataValue = Convert.ToDouble(r.Value, CultureInfo.InvariantCulture);
                    }
                    else if (nodeName == "timesupport")
                    {
                        string isRegular = r.GetAttribute("isRegular");
                        if (!String.IsNullOrEmpty(isRegular))
                        {
                            varInfo.IsRegular = Convert.ToBoolean(isRegular);
                        }
                    }
                    else if (nodeName == "unitname")
                    {
                        r.Read();
                        timeUnit.Name = r.Value;
                    }
                    else if (nodeName == "unitdescription")
                    {
                        r.Read();
                        timeUnit.Name = r.Value;
                    }
                    else if (nodeName == "unittype")
                    {
                        r.Read();
                        timeUnit.UnitsType = r.Value;
                    }
                    else if (nodeName == "unitabbreviation")
                    {
                        r.Read();
                        timeUnit.Abbreviation = r.Value;
                    }
                    else if (nodeName == "timeinterval")
                    {
                        r.Read();
                        varInfo.TimeSupport = Convert.ToSingle(r.Value);
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && nodeName == "variable")
                {
                    varInfo.TimeUnit = timeUnit;
                    varInfo.VariableUnit = valueUnit;
                    return varInfo;
                }
            }
            return varInfo;
        }

        private SeriesMetadata ReadSeriesFromSiteInfo(XmlReader r, Site site)
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
                        series.BeginDateTime = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                        series.BeginDateTimeUTC = series.BeginDateTime;
                    }
                    else if (nodeName == "enddatetime")
                    {
                        r.Read();
                        series.EndDateTime = Convert.ToDateTime(r.Value, CultureInfo.InvariantCulture);
                        series.EndDateTimeUTC = series.EndDateTime;
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
                        //TODO: Read QualityControlLevel
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
        private IList<Series> ReadDataValues(XmlReader r)
        {
            IList<DataValue> lst = new List<DataValue>();

            Dictionary<string, Qualifier> qualifiers = new Dictionary<string, Qualifier>();
            Dictionary<string, Method> methods = new Dictionary<string, Method>();
            Dictionary<string, Source> sources = new Dictionary<string, Source>();
            Dictionary<string, QualityControlLevel> qualityControlLevels = new Dictionary<string, QualityControlLevel>();
            Dictionary<string, Sample> samples = new Dictionary<string,Sample>();
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
                            //fix by Jiri - sometimes the dateTime attribute is uppercase
                            string localDateTime = r.GetAttribute("dateTime");
                            if (string.IsNullOrEmpty(localDateTime))
                            {
                                localDateTime = r.GetAttribute("DateTime");
                            }
                            val.LocalDateTime = Convert.ToDateTime(localDateTime, CultureInfo.InvariantCulture);
                            
                            val.DateTimeUTC = val.LocalDateTime;
                            val.UTCOffset = 0.0;

                            //method
                            string methodID = r.GetAttribute("methodID");
                            if (String.IsNullOrEmpty(methodID))
                            {
                                methodID = "0"; //when a method ID is unspecified
                            }
                            if (!methods.ContainsKey(methodID))
                            {
                                Method newMethod = Method.Unknown;
                                methods.Add(methodID, newMethod);
                            }
                            methodLookup.Add(val, methodID);

                            //quality control level
                            string qualityCode = r.GetAttribute("qualityControlLevel");
                            if (String.IsNullOrEmpty(qualityCode))
                            {
                                qualityCode = "unknown"; //when the quality control level is unspecified
                            }
                            if (!qualityControlLevels.ContainsKey(qualityCode))
                            {
                                QualityControlLevel qualControl = QualityControlLevel.Unknown;
                                qualControl.Code = qualityCode;
                                qualControl.Definition = qualityCode;
                                qualControl.Explanation = qualityCode;
                                qualityControlLevels.Add(qualityCode, qualControl); 
                            }                        

                            //source
                            string sourceID = r.GetAttribute("sourceID");
                            if (String.IsNullOrEmpty(sourceID))
                            {
                                sourceID = "0"; //when a source ID is unspecified
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
                            string sampleID = r.GetAttribute("sampleID");
                            if (!String.IsNullOrEmpty(sampleID))
                            {
                                if (!samples.ContainsKey(sampleID))
                                {
                                    samples.Add(sampleID, Sample.Unknown);
                                    sampleLookup.Add(val, sampleID);
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
                            string offsetID = r.GetAttribute("offsetTypeID");
                            if (!String.IsNullOrEmpty(offsetID))
                            {
                                if (!offsets.ContainsKey(offsetID))
                                {
                                    offsets.Add(offsetID, new OffsetType());
                                }
                                offsetLookup.Add(val, offsetID);

                                string offsetValue = r.GetAttribute("offsetValue");
                                if (!String.IsNullOrEmpty(offsetValue))
                                {
                                    val.OffsetValue = Convert.ToDouble(offsetValue, CultureInfo.InvariantCulture);
                                }
                            }

                            //data value
                            string strVal = r.ReadString();
                            val.Value = Convert.ToDouble(strVal, CultureInfo.InvariantCulture);
                            //val.Value = Convert.ToDouble(r.ReadElementString());
                            //val.Value = Convert.ToDouble(r.ReadInnerXml());
                            //val.Value = Convert.ToDouble(r.Value); //r.ReadElementContentAsDouble();
                        }
                        
                        
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
                        //quality control level seems to be included with each value
                    }
                    else if (r.Name == "qualifier")
                    {
                        ReadQualifier(r, qualifiers);
                    }
                    else if (r.Name == "sample")
                    {
                        ReadSample(r, samples);
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
                    //assign method, source and qual.control level
                    try
                    {
                        newSeries.Method = methods[SeriesCode.GetMethodCode(seriesCode)];
                        //fix by Jiri: fixes the case when sourceID is unspecified sources
                        //has more than one source entry
                        string sourceCode = SeriesCode.GetSourceCode(seriesCode);
                        if (sourceCode == "0" && sources.Count > 0)
                        {
                            foreach (string sc in sources.Keys)
                            {
                                if (sc != "0")
                                {
                                    sourceCode = sc;
                                    break;
                                }
                            }
                        }
                        newSeries.Source = sources[sourceCode];
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

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "offsetDescription")
                    {
                        r.Read();
                        offset.Description = r.Value;
                    }
                    else if (r.Name == "units")
                    {
                        Unit units = new Unit();

                        string abrev = r.GetAttribute("unitsAbbreviation");
                        if (String.IsNullOrEmpty(abrev))
                        {
                            abrev = "unknown";
                        }
                        units.Abbreviation = abrev;

                        string unitsType = r.GetAttribute("unitsType");
                        if (string.IsNullOrEmpty(unitsType))
                        {
                            unitsType = "unknown";
                        }
                        units.UnitsType = unitsType;
                        
                        r.Read();
                        units.Name = r.Value;
                        offset.Unit = units;
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
        private void ReadSample(XmlReader r, Dictionary<string, Sample> samples)
        {
            //not implemented
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
        /// Reads information about method and returns the method object
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private Method ReadMethod(XmlReader r)
        {
            //assign the method Code (method ID) if available
            string methodID = r.GetAttribute("methodID");
            Method method = Method.Unknown;

            if (!String.IsNullOrEmpty(methodID))
            {
                method.Code = Convert.ToInt32(methodID);
            }
            
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name.ToLower() == "methoddescription")
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
                    return method;
                }
            }
            return Method.Unknown;
        }

        /// <summary>
        /// Reads information about method
        /// </summary>
        private void ReadMethod(XmlReader r, Dictionary<string, Method> methods)
        {
            string methodID = r.GetAttribute("methodID");
            
            //special case: if the number of methods is one
            if (methods.Count == 1)
            {
                Method newMethod = ReadMethod(r);
                string methodCode = newMethod.Code.ToString();
                methods[methodCode] = newMethod;
            }
            // otherwise: there are more than one method
            else
            {             
                if (String.IsNullOrEmpty(methodID)) return;
                if (!methods.ContainsKey(methodID)) return;

                Method method = methods[methodID];
                method.Code = Convert.ToInt32(methodID);

                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element)
                    {
                        if (r.Name.ToLower() == "methoddescription")
                        {
                            r.Read();
                            method.Description = r.Value;
                        }
                        else if (r.Name.ToLower() == "methodlink")
                        {
                            r.Read();
                            method.Link = r.Value;
                        }
                    }
                    else if (r.NodeType == XmlNodeType.EndElement && r.Name == "method")
                    {
                        return;
                    }
                }
            }
        }

        private Source ReadSource(XmlReader r)
        {
            //assign the source Code (source ID) if available
            string sourceID = r.GetAttribute("sourceID");
            Source source = Source.Unknown;

            if (!String.IsNullOrEmpty(sourceID))
            {
                source.OriginId = Convert.ToInt32(sourceID);
            }

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    string nodeName = r.Name.ToLower();
                    if (nodeName == "organization")
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

                        //to read the source address
                        string wholeAddress = r.Value;
                        try
                        {
                            if (wholeAddress.Contains("\n"))
                            {
                                char[] separators = new char[] { '\n', ',' };

                                string[] addressParts = wholeAddress.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (addressParts.Length > 2)
                                {
                                    source.State = addressParts[2];
                                }
                                if (addressParts.Length > 1)
                                {
                                    source.City = addressParts[1];
                                }
                                if (addressParts.Length > 0)
                                {
                                    source.Address = addressParts[0];
                                }
                            }
                            else
                            {
                                source.Address = wholeAddress;
                            }
                        }
                        catch
                        {
                            source.Address = wholeAddress;
                        }
                    }
                    else if (nodeName == "sourcedescription")
                    {
                        r.Read();
                        source.Description = r.Value;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name.ToLower() == "source")
                {
                    return source;
                }
            }
            return Source.Unknown;
        }

        /// <summary>
        /// Reads information about the source of the data series
        /// </summary>
        private void ReadSource(XmlReader r, Dictionary<string, Source> sources)
        {
            //special case: if the number of sources is one
            if (sources.Count == 1)
            {
                Source newSource = ReadSource(r);
                string sourceID = newSource.OriginId.ToString();
                sources[sourceID] = newSource;
            }
            // otherwise: there are more than one source
            else
            {
                string sourceID = r.GetAttribute("sourceID");
                if (String.IsNullOrEmpty(sourceID)) return;
                if (!sources.ContainsKey(sourceID)) return;

                Source source = sources[sourceID];

                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element)
                    {
                        string nodeName = r.Name.ToLower();
                        if (nodeName == "organization")
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
                        else if (nodeName == "sourcelink")
                        {
                            r.Read();
                            source.Link = r.Value;
                        }
                    }
                    else if (r.NodeType == XmlNodeType.EndElement && r.Name == "source")
                    {
                        return;
                    }
                }
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
        /// <param name="xmlFileInfo">info about the downloaded xml file</param>
        private void CheckDataSeries(Series series, DataFile xmlFileInfo)
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
                series.BeginDateTimeUTC = series.BeginDateTime - utcOffset;
                series.EndDateTimeUTC = series.EndDateTime - utcOffset;
                foreach (DataValue val in series.DataValueList)
                {
                    val.UTCOffset = utcOffsetHours;
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
    }
}
