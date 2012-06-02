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
    /// The WaterML should be in the 1.1 version.
    /// </summary>
    public class WaterOneFlow11Parser : WaterOneFlowParser
    {
        /// <summary>
        /// Reads information about time unit or variable unit
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private Unit ReadUnit(XmlReader r)
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
        protected override Variable ReadVariable(XmlReader r)
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
                        varInfo.NoDataValue = Convert.ToDouble(r.Value, CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Reads the DataValues section
        /// </summary>
        protected override IList<Series> ReadDataValues(XmlReader r)
        {
            int valueCount;
            var lst = new List<DataValueWrapper>(Int32.TryParse(r.GetAttribute("count"), out valueCount) ? valueCount : 4);

            var qualifiers = new Dictionary<string, Qualifier>();
            var methods = new Dictionary<string, Method>();
            var sources = new Dictionary<string, Source>();
            var qualityControlLevels = new Dictionary<string, QualityControlLevel>();
            var samples = new Dictionary<string,Sample>();
            var labMethods = new Dictionary<string, LabMethod>();
            var offsets = new Dictionary<string, OffsetType>();
            var seriesDictionary = new Dictionary<string, Series>();

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "value")
                    {
                        //create a new empty data value and add it to the list
                        var wrapper = new DataValueWrapper();
                        var val = new DataValue();
                        wrapper.DataValue = val;
                        lst.Add(wrapper);
                        
                        if (r.HasAttributes)
                        {
                            var censorCode = r.GetAttribute("censorCode");
                            if (!string.IsNullOrEmpty(censorCode))
                            {
                                val.CensorCode = censorCode;
                            }
                            val.LocalDateTime = Convert.ToDateTime(r.GetAttribute("dateTime"), CultureInfo.InvariantCulture);

                            //utcOffset
                            var utcOffset = r.GetAttribute("timeOffset");
                            val.UTCOffset = !String.IsNullOrEmpty(utcOffset) ? ConvertUtcOffset(utcOffset) : 0.0;

                            //dateTimeUtc
                            var dateTimeUTC = r.GetAttribute("dateTimeUTC");
                            val.DateTimeUTC = !String.IsNullOrEmpty(dateTimeUTC) ? Convert.ToDateTime(dateTimeUTC, CultureInfo.InvariantCulture) : val.LocalDateTime;

                            //method
                            var methodID = r.GetAttribute("methodCode");
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
                                var newMethod = Method.Unknown;
                                methods.Add(methodID, newMethod);
                            }
                            wrapper.MethodID = methodID;

                            //quality control level
                            var qualityCode = r.GetAttribute("qualityControlLevelCode");
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
                                var qualControl = QualityControlLevel.Unknown;
                                qualControl.Code = qualityCode;
                                qualControl.Definition = qualityCode;
                                qualControl.Explanation = qualityCode;
                                qualityControlLevels.Add(qualityCode, qualControl); 
                            }
                            wrapper.QualityID = qualityCode;

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
                            wrapper.SourceID = sourceID;
                            wrapper.SeriesCode = SeriesCodeHelper.CreateSeriesCode(methodID, qualityCode, sourceID); //----method-source-qualityControl combination----

                            //sample
                            string sampleCode = r.GetAttribute("labSampleCode");
                            if (!String.IsNullOrEmpty(sampleCode))
                            {
                                if (!samples.ContainsKey(sampleCode))
                                {
                                    samples.Add(sampleCode, Sample.Unknown);
                                }
                            }
                            wrapper.SampleID = sampleCode;

                            //qualifiers
                            string qualifierCodes = r.GetAttribute("qualifiers");
                            if (!String.IsNullOrEmpty(qualifierCodes))
                            {
                                if (!qualifiers.ContainsKey(qualifierCodes))
                                {
                                    var newQualifier = new Qualifier {Code = qualifierCodes};
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
                                string offsetValue = r.GetAttribute("offsetValue");
                                if (!String.IsNullOrEmpty(offsetValue))
                                {
                                    val.OffsetValue = Convert.ToDouble(offsetValue, CultureInfo.InvariantCulture);
                                }
                            }
                            wrapper.OffsetID = offsetCode;
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
            foreach (var wrapper in lst)
            {
                var val = wrapper.DataValue;

                //which series does the data value belong to?
                var seriesCode = wrapper.SeriesCode;
                if (!seriesDictionary.ContainsKey(seriesCode))
                {
                    Series newSeries = new Series();
                    seriesDictionary.Add(seriesCode, newSeries);
                    //assign method, source and qual.control level
                    try
                    {
                        newSeries.Method = methods[SeriesCodeHelper.GetMethodCode(seriesCode)];
                        newSeries.Source = sources[SeriesCodeHelper.GetSourceCode(seriesCode)];
                        newSeries.QualityControlLevel = qualityControlLevels[SeriesCodeHelper.GetQualityCode(seriesCode)];
                    }
                    catch { }
                }

                //add the data value to the correct series
                var series = seriesDictionary[seriesCode];
                series.DataValueList.Add(val);
                val.Series = series;

                Sample sample;
                if (!string.IsNullOrEmpty(wrapper.SampleID) &&
                    samples.TryGetValue(wrapper.SampleID, out sample))
                {
                    val.Sample = sample;
                }
                OffsetType offset;
                if (!string.IsNullOrEmpty(wrapper.OffsetID) &&
                    offsets.TryGetValue(wrapper.OffsetID, out offset))
                {
                    val.OffsetType = offset;
                }
                if (series.Method == null)
                {
                    series.Method = methods[wrapper.MethodID];
                }
                if (series.Source == null)
                {
                    series.Source = sources[wrapper.SourceID];
                }
                if (series.QualityControlLevel == null)
                {
                    series.QualityControlLevel = qualityControlLevels[wrapper.QualityID];
                }
            }
            //to check the qualifiers
            CheckQualifiers(qualifiers);

            return seriesDictionary.Values.ToList();
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
                var newQualifier = new Qualifier();
                newQualifier.Code = qualifierCode;
                qualifiers.Add(qualifierCode, newQualifier);
            }

            var qualifier = qualifiers[qualifierCode];
            r.Read();
            qualifier.Description = r.Value;
        }


        /// <summary>
        /// Reads information about the quality control level and returns the quality control level object
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected override QualityControlLevel ReadQualityControlLevel(XmlReader r)
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
                        qc.Definition = r.Value.Trim();
                    }
                    else if (r.Name == "explanation")
                    {
                        r.Read();
                        qc.Explanation = r.Value.Trim();
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
        protected override Method ReadMethod(XmlReader r)
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
        protected override Source ReadSource(XmlReader r)
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
                minutes = Convert.ToDouble(offsetString.Substring(colonIndex + 1), CultureInfo.InvariantCulture);
                hours = Convert.ToDouble((offsetString.Substring(0, colonIndex)), CultureInfo.InvariantCulture);
            }
            return hours + (minutes / 60.0);
        }
    }
}
