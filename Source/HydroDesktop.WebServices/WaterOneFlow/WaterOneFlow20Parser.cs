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
            using (var fileStream = new FileStream(xmlFile, FileMode.Open))
            {
                return ParseGetValues(fileStream);
            } 
        }

        private IList<Series> ReadDataValues(XmlNodeList observations)
        {
            IList<Series> seriesList = new List<Series>();

            //loop through each series observation
            foreach (XmlNode observation in observations)
            {
                XmlDocument xml = new XmlDocument();
                //xml.ImportNode(observation, false);
                xml.LoadXml(observation.OuterXml);
                XmlNodeList times = xml.GetElementsByTagName("wml2:time");
                XmlNodeList values = xml.GetElementsByTagName("wml2:value");
                Series newSeries = new Series();

                //add begin and end datetime to the series
                try
                {
                    newSeries.BeginDateTime = Convert.ToDateTime(xml.GetElementsByTagName("gml:beginPosition").Item(0).FirstChild.Value);
                    newSeries.EndDateTime = Convert.ToDateTime(xml.GetElementsByTagName("gml:endPosition").Item(0).FirstChild.Value);
                } catch{}

                //add each node to the series
                for (int i = 0; i < times.Count; i++)
                {
                    Double value = Double.Parse(values.Item(i).FirstChild.Value);
                    DateTime time = Convert.ToDateTime(times.Item(i).FirstChild.Value);

                    DataValue dataValue = new DataValue(value, time, 0);
                    newSeries.DataValueList.Add(dataValue);
                    dataValue.Series = newSeries;
                }

                //add parsed series data to list
                seriesList.Add(newSeries);
            }

            return seriesList;
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

        public IList<Series> ParseGetValues(Stream stream)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(stream);
            XmlNodeList observations = xml.GetElementsByTagName("wml2:observationMember");

            //Site site = null;
            //Variable varInfo = null;
            IList<Series> seriesList = null;

            //get the data values and put them into a list of series
            seriesList = ReadDataValues(observations);

            foreach (var series in seriesList)
            {
                //if (varInfo != null)
                //{
                //    series.Variable = varInfo;
                //}
                //if (site != null)
                //{
                //    series.Site = site;
                //}

                //ensure that properties are re-calculated
                series.UpdateSeriesInfoFromDataValues();

                //set the checked and creation date time
                series.CreationDateTime = DateTime.Now;
                series.LastCheckedDateTime = DateTime.Now;
                series.UpdateDateTime = series.LastCheckedDateTime;
            }
            return seriesList ?? (new List<Series>(0));
        }

        IList<Site> IWaterOneFlowParser.ParseGetSites(string xmlFile)
        {
            throw new NotImplementedException();
        }

        IList<Site> IWaterOneFlowParser.ParseGetSites(Stream stream)
        {
            throw new NotImplementedException();
        }

        IList<SeriesMetadata> IWaterOneFlowParser.ParseGetSiteInfo(string xmlFile)
        {
            throw new NotImplementedException();
        }

        IList<SeriesMetadata> IWaterOneFlowParser.ParseGetSiteInfo(Stream stream)
        {
            throw new NotImplementedException();
        }

        IList<Series> IWaterOneFlowParser.ParseGetValues(string xmlFile)
        {
            throw new NotImplementedException();
        }

        IList<Series> IWaterOneFlowParser.ParseGetValues(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
