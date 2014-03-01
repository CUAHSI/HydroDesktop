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
    public class WaterOneFlow10Parser : WaterOneFlowParser
    {
        /// <summary>
        /// Reads information about variable
        /// </summary>
        protected override Variable ReadVariable(XmlReader r)
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
                            //fix by Jiri - sometimes the dateTime attribute is uppercase
                            var localDateTime = r.GetAttribute("dateTime");
                            if (string.IsNullOrEmpty(localDateTime))
                            {
                                localDateTime = r.GetAttribute("DateTime");
                            }
                            val.LocalDateTime = Convert.ToDateTime(localDateTime, CultureInfo.InvariantCulture);
                            val.DateTimeUTC = val.LocalDateTime;
                            val.UTCOffset = 0.0;

                            //method
                            var methodID = r.GetAttribute("methodID");
                            if (String.IsNullOrEmpty(methodID))
                            {
                                methodID = "0"; //when a method ID is unspecified
                            }
                            if (!methods.ContainsKey(methodID))
                            {
                                var newMethod = Method.Unknown;
                                methods.Add(methodID, newMethod);
                            }
                            wrapper.MethodID = methodID;

                            //quality control level
                            string qualityCode = r.GetAttribute("qualityControlLevel");
                            if (String.IsNullOrEmpty(qualityCode))
                            {
                                qualityCode = "unknown"; //when the quality control level is unspecified
                            }
                            if (!qualityControlLevels.ContainsKey(qualityCode))
                            {
                                var qualControl = QualityControlLevel.Unknown;
                                qualControl.Code = qualityCode;
                                qualControl.Definition = qualityCode;
                                qualControl.Explanation = qualityCode;
                                qualityControlLevels.Add(qualityCode, qualControl); 
                            }                        

                            //source
                            var sourceID = r.GetAttribute("sourceID");
                            if (String.IsNullOrEmpty(sourceID))
                            {
                                sourceID = "0"; //when a source ID is unspecified
                            }
                            if (!sources.ContainsKey(sourceID))
                            {
                                sources.Add(sourceID, Source.Unknown);
                            }
                            wrapper.SourceID = sourceID;
                            wrapper.SeriesCode = SeriesCodeHelper.CreateSeriesCode(methodID, qualityCode, sourceID); //----method-source-qualityControl combination----
                            
                            //sample
                            var sampleID = r.GetAttribute("sampleID");
                            if (!String.IsNullOrEmpty(sampleID))
                            {
                                if (!samples.ContainsKey(sampleID))
                                {
                                    samples.Add(sampleID, Sample.Unknown);
                                }
                            }
                            wrapper.SampleID = sampleID;

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
                            var offsetID = r.GetAttribute("offsetTypeID");
                            if (!String.IsNullOrEmpty(offsetID))
                            {
                                if (!offsets.ContainsKey(offsetID))
                                {
                                    offsets.Add(offsetID, new OffsetType());
                                }
                                var offsetValue = r.GetAttribute("offsetValue");
                                if (!String.IsNullOrEmpty(offsetValue))
                                {
                                    val.OffsetValue = Convert.ToDouble(offsetValue, CultureInfo.InvariantCulture);
                                }
                            }
                            wrapper.OffsetID = offsetID;

                            //data value
                            val.Value = Convert.ToDouble(r.ReadString(), CultureInfo.InvariantCulture);
                        }
                        
                        
                   } 
                    else if (r.Name == "method")
                    {
                        var method = ReadMethod(r);
                        var methodCodeKey = method.Code.ToString(CultureInfo.InvariantCulture);
                        if (methods.ContainsKey(methodCodeKey))
                        {
                            methods[methodCodeKey] = method;
                        }
                    }
                    else if (r.Name == "source")
                    {
                        var source = ReadSource(r);
                        var sourceCodeKey = source.OriginId.ToString(CultureInfo.InvariantCulture);
                        if (sources.ContainsKey(sourceCodeKey))
                        {
                            sources[sourceCodeKey] = source;
                        }
                    }
                    else if (r.Name == "qualityControlLevel")
                    {
                        //quality control level seems to be included with each value
                    }
                    else if (r.Name == "qualifier")
                    {
                        ReadQualifier(r, qualifiers);
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
                    var newSeries = new Series();
                    seriesDictionary.Add(seriesCode, newSeries);
                    //assign method, source and qual.control level
                    //assign method, source and qual.control level
                    try
                    {
                        newSeries.Method = methods[SeriesCodeHelper.GetMethodCode(seriesCode)];
                        //fix by Jiri: fixes the case when sourceID is unspecified sources
                        //has more than one source entry
                        string sourceCode = SeriesCodeHelper.GetSourceCode(seriesCode);
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
            }

            //to check the qualifiers
            CheckQualifiers(qualifiers);
            return seriesDictionary.Values.ToList();
        }

        /// <summary>
        /// Read the vertical offset type
        /// </summary>
        private static void ReadOffset(XmlReader r, IDictionary<string, OffsetType> offsets)
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
        /// Reads information about a qualifier
        /// </summary>
        private static void ReadQualifier(XmlReader r, IDictionary<string, Qualifier> qualifiers)
        {
            string qualifierCode = r.GetAttribute("qualifierCode");
            if (String.IsNullOrEmpty(qualifierCode)) return;
            if (!qualifiers.ContainsKey(qualifierCode))
            {
                var newQualifier = new Qualifier {Code = qualifierCode};
                qualifiers.Add(qualifierCode, newQualifier);
            }

            var qualifier = qualifiers[qualifierCode];
            r.Read();
            qualifier.Description = r.Value;
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
    }
}
