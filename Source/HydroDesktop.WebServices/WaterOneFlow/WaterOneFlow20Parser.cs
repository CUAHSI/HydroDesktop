using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using HydroDesktop.DataModel;

namespace HydroDesktop.WebServices
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
        /// Parses the xml file returned by GetSites call to a WaterOneFlow
        /// web service
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public IList<Site> ParseGetSitesXml(string xmlFile)
        {
            throw new NotImplementedException("ParseGetSitesXml is not implemented");
        }

        /// <summary>
        /// Parses the xml file returned by the GetVariables() call to a 
        /// WaterOneFlow web service
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public IList<Variable> ParseGetVariablesXml(string xmlFile)
        {
            throw new NotImplementedException("Parse GetVariablesXml is not implemented.");
        }

        /// <summary>
        /// Parses the xml file returned by the GetSite() call to a 
        /// WaterOneFlow web service
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public IList<Series> ParseGetSiteXml(string xmlFile)
        {
            throw new NotImplementedException("ParseGetSiteXml is not implemented.");
        }
        
        
        /// <summary>
        /// Parses a WaterML TimeSeriesResponse XML file
        /// </summary>
        /// <param name="xmlFile"></param>
        public IList<Series> ParseGetValues(string xmlFile)
        {
            throw new NotImplementedException("ParseGetValues is not implemented for WaterML 2.0");
            
            QueryInfo qry = null;
            Site site = null;
            Variable varInfo = null;
            Series series = null;
            
            using (XmlTextReader reader = new XmlTextReader(xmlFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "queryInfo")
                        {
                            //Read the 'Query Info'
                            qry = ReadQueryInfo(reader);
                        }
                        else if (reader.Name == "Source")
                        {
                            //Read the site information
                            site = ReadSite(reader);
                        }
                        else if (reader.Name == "variable")
                        {
                            //Read the variable information
                            varInfo = ReadVariable(reader);
                        }
                        else if (reader.Name == "values")
                        {
                            //Read the time series and data values information
                            series = ReadDataValues(reader);
                            if (varInfo != null)
                            {
                                series.Variable = varInfo;
                            }
                            if (site != null)
                            {
                                series.Site = site;
                            }
                        }
                    }
                }
                return CheckDataSeries(series);
            }
        }

        /// <summary>
        /// Reads the QueryInfo section
        /// </summary>
        private QueryInfo ReadQueryInfo(XmlTextReader r)
        {
            QueryInfo query = new QueryInfo();
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "locationParam")
                    {
                        r.Read();
                        query.LocationParameter = r.Value;
                    }
                    else if (r.Name == "variableParam")
                    {
                        r.Read();
                        query.VariableParameter = r.ReadContentAsString();
                    }
                    else if (r.Name == "beginDateTime")
                    {
                        r.Read();
                        query.BeginDateParameter = Convert.ToDateTime(r.Value);
                    }
                    else if (r.Name == "endDateTime")
                    {
                        r.Read();
                        query.EndDateParameter = Convert.ToDateTime(r.Value);
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "queryInfo")
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
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "siteName")
                    {
                        r.Read();
                        site.Name = r.Value;
                    }
                    else if (r.Name == "geoLocation")
                    {
                        ReadSpatialReference(r, site);
                    }
                    else if (r.Name.IndexOf("siteCode") >= 0)
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
                    else if (r.Name == "verticalDatum")
                    {
                        r.Read();
                        site.VerticalDatum = r.Value;
                    }
                    else if (r.Name == "timeZoneInfo")
                    {
                        site.TimeZone = ReadTimeZoneInfo(r);
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "Source")
                {
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
        /// Reads information about variable
        /// </summary>
        private Variable ReadVariable(XmlTextReader r)
        {
            Variable varInfo = new Variable();
            Unit timeUnit = new Unit();
            Unit valueUnit = new Unit();
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name.IndexOf("variableCode") >= 0)
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
                        break;
                    }
                    
                    switch(r.Name)
                    {
                        //case "variableCode":

                        //    string prefix = r.GetAttribute("vocabulary");
                        //    r.Read();
                        //    string variableCode = r.Value;
                        //    if (!String.IsNullOrEmpty(prefix))
                        //    {
                        //        variableCode = prefix + ":" + variableCode;
                        //    }
                        //    varInfo.Code = variableCode;
                        //    break;
                        
                        case "variableName":
                    
                            r.Read();
                            varInfo.Name = r.Value;
                            break;
                    
                        case "valueType":

                            r.Read();
                            varInfo.ValueType = r.Value;
                            break;
                    
                        case "dataType":

                            r.Read();
                            varInfo.DataType = r.Value;
                            break;
                    
                        case "generalCategory":

                            r.Read();
                            varInfo.GeneralCategory = r.Value;
                            break;
                    
                        case "sampleMedium":

                            r.Read();
                            varInfo.SampleMedium = r.Value;
                            break;

                        case "units":

                            string abrev = r.GetAttribute("unitsAbbreviation");
                            string unitType = r.GetAttribute("unitsType");
                            r.Read();
                            string unitName = r.Value;

                            valueUnit.Abbreviation = abrev;
                            valueUnit.UnitsType = unitType;
                            valueUnit.Name = unitName;
                            break;
                    
                        case "NoDataValue":

                            r.Read();
                            varInfo.NoDataValue = Convert.ToDouble(r.Value);
                            break;
                    
                        case "timeSupport":

                            string isRegular = r.GetAttribute("isRegular");
                            if (!String.IsNullOrEmpty(isRegular))
                            {
                                varInfo.IsRegular = Convert.ToBoolean(isRegular);
                            }
                            break;
                    
                        case "UnitName":

                            r.Read();
                            timeUnit.Name = r.Value;
                            break;
                    
                        case "UnitDescription":

                            r.Read();
                            if (String.IsNullOrEmpty(timeUnit.Name))
                            {
                                timeUnit.Name = r.Value;
                            }
                            break;

                        case "UnitType":

                            r.Read();
                            timeUnit.UnitsType = r.Value;
                            break;
                    
                        case "UnitAbbreviation":

                            r.Read();
                            timeUnit.Abbreviation = r.Value;
                            break;
                    
                        case "timeInterval":

                            r.Read();
                            varInfo.TimeSupport = Convert.ToSingle(r.Value);
                            break;
                    
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "variable")
                {
                    varInfo.TimeUnit = timeUnit;
                    varInfo.VariableUnit = valueUnit;
                    return varInfo;
                }
            }
            return varInfo;
        }

        /// <summary>
        /// Reads the DataValues section
        /// </summary>
        private Series ReadDataValues(XmlTextReader r)
        {
            Series series = new Series();
            IList<DataValue> lst = series.DataValues;

            Dictionary<string, Qualifier> qualifiers = new Dictionary<string, Qualifier>();
            Dictionary<string, Method> methods = new Dictionary<string, Method>();
            Dictionary<string, Source> sources = new Dictionary<string, Source>();
            Dictionary<string, QualityControlLevel> qualityControlLevels = new Dictionary<string, QualityControlLevel>();
            Dictionary<string, Sample> samples = new Dictionary<string,Sample>();
            Dictionary<string, OffsetType> offsets = new Dictionary<string, OffsetType>();

            //lookup for samples, qualifiers and vertical offsets
            var sampleLookup = new Dictionary<DataValue, string>();
            var offsetLookup = new Dictionary<DataValue, string>();
            //var qualifierLookup = new Dictionary<DataValue, string>();
            var methodLookup = new Dictionary<DataValue, string>();
            var sourceLookup = new Dictionary<DataValue, string>();

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "values" && r.HasAttributes)
                    {
                        series.ValueCount = Convert.ToInt32(r.GetAttribute("count"));
                    }
                    else if (r.Name == "value")
                    {
                        DataValue val = series.CreateDataValue();

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
                            val.DateTimeUTC = val.LocalDateTime;
                            val.UTCOffset = 0.0;

                            //method
                            string methodID = r.GetAttribute("methodID");
                            if (String.IsNullOrEmpty(methodID))
                            {
                                methodID = "unknown"; //when a method is unspecified
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
                                val.QualityControlLevel = qualControl;
                                series.QualityControlLevels.Add(qualControl);
                            }
                            else
                            {
                                val.QualityControlLevel = qualityControlLevels[qualityCode];
                            }                           

                            //source
                            string sourceID = r.GetAttribute("sourceID");
                            if (String.IsNullOrEmpty(sourceID))
                            {
                                sourceID = "unknown"; //when a source is unspecified
                            }
                            if (!sources.ContainsKey(sourceID))
                            {
                                sources.Add(sourceID, Source.Unknown);
                            }
                            sourceLookup.Add(val, sourceID);

                            //sample
                            string sampleID = r.GetAttribute("sampleID");
                            if (!String.IsNullOrEmpty(sampleID))
                            {
                                if (!samples.ContainsKey(sampleID))
                                {
                                    samples.Add(sampleID, new Sample());
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

            //to assign relations of each variable
            foreach (DataValue val in lst)
            {
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
                    val.Method = newMethod;
                    if (!series.Methods.Contains(newMethod))
                    {
                        series.Methods.Add(newMethod);
                    }
                }
                if (sourceLookup.ContainsKey(val))
                {
                    Source newSource = sources[sourceLookup[val]];
                    val.Source = newSource;
                    if (!series.Sources.Contains(newSource))
                    {
                        series.Sources.Add(newSource);
                    }
                }
            }

            //to check the qualifiers
            CheckQualifiers(qualifiers);

            return series;
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
                        units.Abbreviation = r.GetAttribute("unitsAbbreviation");
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
        /// Reads information about method
        /// </summary>
        private void ReadMethod(XmlReader r, Dictionary<string, Method> methods)
        {
            string methodID = r.GetAttribute("methodID");
            if (String.IsNullOrEmpty(methodID)) return;
            if (!methods.ContainsKey(methodID)) return;

            Method method = methods[methodID];

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "MethodDescription")
                    {
                        r.Read();
                        method.Description = r.Value;
                    }
                    else if (r.Name == "methodLink")
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

        /// <summary>
        /// Reads information about the source of the data series
        /// </summary>
        private void ReadSource(XmlReader r, Dictionary<string, Source> sources)
        {
            string sourceID = r.GetAttribute("sourceID");
            if (String.IsNullOrEmpty(sourceID)) return;
            if (!sources.ContainsKey(sourceID)) return;

            Source source = sources[sourceID];

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {                       
                        case "Organization":
                            r.Read();
                            source.Organization = r.Value;
                            break;
                        case "ContactName":
                            r.Read();
                            source.ContactName = r.Value;
                            break;
                        case "Phone":
                            r.Read();
                            source.Phone = r.Value;
                            break;
                        case "Email":
                            r.Read();
                            source.Email = r.Value;
                            break;
                        case "Address":
                            r.Read();
                            source.Address = r.Value;
                            break;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && r.Name == "source")
                {
                    return;
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
                        site.SpatialReference.SRSName = r.GetAttribute("srs");
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
        }

        /// <summary>
        /// Checks data series to make sure that the time zone information
        /// is correct. Also check if it is a composite series and if it is composite then
        /// separates it into multiple series.
        /// </summary>
        /// <param name="series"></param>
        private IList<Series> CheckDataSeries(Series series)
        {
            series.BeginDateTime = series.DataValues[0].LocalDateTime;
            series.EndDateTime = series.DataValues[series.DataValues.Count - 1].LocalDateTime;
            series.ValueCount = series.DataValues.Count;
            if (series.Site.TimeZone != TimeZoneInfo.Utc)
            {
                TimeSpan utcOffset = series.Site.TimeZone.BaseUtcOffset;
                double utcOffsetHours = utcOffset.TotalHours;
                series.BeginDateTimeUTC = series.BeginDateTime + utcOffset;
                series.EndDateTimeUTC = series.EndDateTime + utcOffset;
                foreach (DataValue val in series.DataValues)
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

            return series.GetListOfSimpleSeries();
        }
    }
}
