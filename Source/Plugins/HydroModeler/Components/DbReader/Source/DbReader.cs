using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Data;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Spatial;
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using HydroDesktop.Database;
using System.Text.RegularExpressions;
using System.Linq;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;


namespace CUAHSI.HIS
{
    public class DbReader : ILinkableComponent
    {

        private Dictionary<string, ILink> _links
            = new Dictionary<string, ILink>();
        private List<IInputExchangeItem> _inputExchangeItems
            = new List<IInputExchangeItem>();
        private List<IOutputExchangeItem> _outputExchangeItems
            = new List<IOutputExchangeItem>();
        private double _earliestInputTime;
        private double _latestInputTime;
        private string _dbPath;
        private string _fullPath; //path to the .dll
        private DbOperations _db;
        private Dictionary<string, SmartBuffer> _buffer = new Dictionary<string, SmartBuffer>();
        private Dictionary<string, int> _elementCount = new Dictionary<string,int>();
        private Dictionary<string, TimeStamp> _endTimes = new Dictionary<string, TimeStamp>();
        private Dictionary<string, List<double>> _times = new Dictionary<string,List<double>>();
        private Dictionary<string, int> _lastIndex = new Dictionary<string, int>();
        
        private int _searchDistance = -999;

        //set the relaxation factor to -999 initially.  This will be modified if a value is specified in the omi file.
        double _relaxationFactor = -999;
        bool _exactMatch = false;
        int _range = -999;

        private Dictionary<string, ElementMapper> mapper = new Dictionary<string, ElementMapper>();

        #region ILinkableComponent Members

        public void AddLink(ILink link)
        {
            _links.Add(link.ID, link);
            AddToBuffer(link);
           
        }

        private void AddToBuffer(ILink link)
        {
            // ----------------------------------------------------------------------------------------------------
            // The method adds values associated with the link.source to a SmartBuffer. 
            // ----------------------------------------------------------------------------------------------------

            //dictionary of values indexed by their date\time
            SortedDictionary<DateTime, ArrayList> dict = new SortedDictionary<DateTime, ArrayList>();

            //create a buffer instance to temporarily store data
            SmartBuffer _smartBuffer = new SmartBuffer();
            //set the relaxation factor, if specifed in *.omi file.
            if (_relaxationFactor > 0)
                _smartBuffer.RelaxationFactor = _relaxationFactor;

            //get link.source quantity and elementset
            IQuantity sourceQuantity = link.SourceQuantity;
            IElementSet sourceElementSet = link.SourceElementSet;

            string sql = "SELECT DISTINCT ds.SeriesID " +
                    "FROM DataValues dv " +
                    "INNER JOIN DataSeries ds ON dv.SeriesID = ds.SeriesID " +
                    "INNER JOIN DataThemes dt ON dv.SeriesID = dt.SeriesID " +
                    "INNER JOIN Sites s ON ds.SiteID = s.SiteID " +
                    "INNER JOIN DataThemeDescriptions dtd On dt.ThemeID = dtd.ThemeID " +
                    "WHERE dtd.ThemeName = '" + sourceElementSet.ID.ToString() + "' " +
                    "ORDER BY s.SiteName ASC";

            DataTable tbl = _db.LoadTable("values", sql);

            //get the number of series' in this theme
            Dictionary<string, int> sites = new Dictionary<string, int>();

            //get the number of sites in this series
            int k = 0;
            foreach (DataRow row in tbl.Rows)
            {
                if (!sites.ContainsKey(Convert.ToString(row["SeriesID"])))
                {
                    sites.Add(Convert.ToString(row["SeriesID"]), k);
                    k++;
                }

            }


            //query the db for values associated with source quantity and elementset
            //TODO: LOOKUP BY THEMENAME, NOT THEMEID
            sql = "SELECT ds.SeriesID, dv.LocalDateTime, dv.DataValue " +
                    "FROM DataValues dv " +
                    "INNER JOIN DataSeries ds ON dv.SeriesID = ds.SeriesID " +
                    "INNER JOIN DataThemes dt ON dv.SeriesID = dt.SeriesID " +
                    "INNER JOIN DataThemeDescriptions dtd On dt.ThemeID = dtd.ThemeID " +
                    "WHERE dtd.ThemeName = '" + sourceElementSet.ID.ToString() + "' " +
                    "ORDER BY dv.LocalDateTime ASC";
                    //"ORDER BY dv.DataValue ASC";

            tbl = _db.LoadTable("values", sql);

            //get the number of series' in this theme
            List<DateTime> t = new List<DateTime>();
            Dictionary<DateTime, double[]> Times = new Dictionary<DateTime, double[]>();

            //get the number of sites in this series
            //int k = 0;
            //foreach (DataRow row in tbl.Rows)
            //{
            //    if (!sites.ContainsKey(Convert.ToString(row["SeriesID"])))
            //    {
            //        sites.Add(Convert.ToString(row["SeriesID"]), k);
            //        k++;
            //    }

            //    if(!t.Contains(Convert.ToDateTime(row["LocalDateTime"])))
            //        t.Add(Convert.ToDateTime(row["LocalDateTime"]));
            //}
            //initialize a dictionary to hold the times and values 
            foreach (DataRow row in tbl.Rows)
            {
                if (!Times.ContainsKey(Convert.ToDateTime(row["LocalDateTime"])))
                    Times.Add(Convert.ToDateTime(row["LocalDateTime"]), new double[sites.Count]);
            }
            //Times.OrderBy<pair,
            Times.OrderBy(pair => pair.Value);
            foreach (DataRow row in tbl.Rows)
            {
                double v = Convert.ToDouble(row["DataValue"]);
                string id = Convert.ToString(row["SeriesID"]);
                DateTime dt = Convert.ToDateTime(row["LocalDateTime"]);
                Times[dt][sites[id]] = v;
            }
                

           for( int i=0; i<= t.Count-1; i++)
           {
                double[] vals = new double[sites.Count];
                DateTime dt = t[i];
                foreach (DataRow row in tbl.Rows)
                {
                    double v = Convert.ToDouble(row["DataValue"]);
                    string id = Convert.ToString(row["SeriesID"]);
                    
                    //add v to vals in the location defined by its site id
                    vals[sites[id]] = v;
                }

                ArrayList a = new ArrayList();
                a.Add(vals);
                dict.Add(t[i], a);

           }
            ////check to see if time/value combination has been already added
            //if (dict.ContainsKey(dt))
            //{
            //    //if yes, add value to existing dictionary
            //    ArrayList a = dict[dt];
            //    a.Add(v);
            //}
            //else
            //{
            //    //if not, add value to new dictionary
            //    ArrayList a = new ArrayList();
            //    a.Add(v);
            //    dict.Add(dt, a);
            //}
            
            //double[] valueset = null;
            //ITimeStamp time_stmp = null;
            ////add dictionary to the smart buffer
            //foreach (KeyValuePair<DateTime, ArrayList> kvp in dict)
            //{
            //    time_stmp = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(kvp.Key));
            //    valueset = (double[])kvp.Value.ToArray(typeof(double));
            //    _smartBuffer.AddValues(time_stmp, new ScalarSet(valueset));
            //}


           // //sort the dictionary
           //var sortDict = from keys in Times.Keys
           //           orderby Times[keys] ascending
           //           select keys;

           ////Times = (Dictionary<DateTime, double[]>)sortDict;

           //foreach (KeyValuePair<DateTime, double[]> kvp in Times.OrderBy(key => key.Value))
           //{

           //    time_stmp = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(kvp.Key));
           //    valueset = kvp.Value;
           //    _smartBuffer.AddValues(time_stmp, new ScalarSet(valueset));
           //}

           //add dictionary to the smart buffer
           double[] valueset = null;
           ITimeStamp time_stmp = null;
           foreach (KeyValuePair<DateTime, double[]> kvp in Times)
           {
               time_stmp = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(kvp.Key));
               valueset = kvp.Value;
               _smartBuffer.AddValues(time_stmp, new ScalarSet(valueset));
           }

           //if ExactMatch is requested, then save the times for using in the GetValues method
           try
           {
               if (_exactMatch)
               {
                   List<double> times = new List<double>();
                   foreach (KeyValuePair<DateTime, double[]> kvp in Times)
                   {

                       time_stmp = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(kvp.Key));
                       times.Add(time_stmp.ModifiedJulianDay);
                   }
                   _times.Add(link.ID, times);
               }
           }
           catch (Exception) { }

            ////if ExactMatch is requested, then save the times for using in the GetValues method
            //try
            //{
            //    if (_exactMatch)
            //    {
            //        List<double> times = new List<double>();
            //        foreach (KeyValuePair<DateTime, ArrayList> kvp in dict)
            //        {

            //            time_stmp = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(kvp.Key));
            //            times.Add(time_stmp.ModifiedJulianDay);
            //        }
            //        _times.Add(link.ID, times);
            //    }
            //}
            //catch (Exception) { }

            //store the number of elements for this link
            _elementCount.Add(link.ID, valueset.Length);

            //store the lastest known time for this link
            _endTimes.Add(link.ID, (TimeStamp)time_stmp);
            //store the smart buffer based on linkID
            _buffer.Add(link.ID, _smartBuffer);

            //initialize the last index variable
            _lastIndex.Add(link.ID, 0);

            //adjust start time based on target component
            if (link.TargetComponent.TimeHorizon.Start.ModifiedJulianDay > this.EarliestInputTime.ModifiedJulianDay)
                this._earliestInputTime = link.TargetComponent.TimeHorizon.Start.ModifiedJulianDay;


            #region Initialize Element Mapper

            try
            {
                //get the first (stored) data operation
                IDataOperation dataOp = link.GetDataOperation(0);
                //get dataOperation description
                string dataOpDesc = dataOp.GetArgument(1).Value;
                //add a element mapper instance to the mapper dictionary
                mapper.Add(link.ID, new ElementMapper());
                //initialize the element mapper and create a mapping matrix
                mapper[link.ID].Initialise(dataOpDesc, link.SourceElementSet, link.TargetElementSet);
            }
            catch (Exception e) { }

            #endregion
        }

        public string ComponentDescription
        {
            get { return "DbReader 1.0"; }
        }

        public string ComponentID
        {
            get { return "DbReader"; }
        }

        public void Dispose()
        {

        }

        public ITimeStamp EarliestInputTime
        {
            get { return new TimeStamp(_earliestInputTime); }
        }

        public void Finish()
        {
            //clear all values in the buffer
            foreach (KeyValuePair<string, SmartBuffer> kvp in _buffer)
                kvp.Value.Clear(this.TimeHorizon);
            //_smartBuffer.Clear(this.TimeHorizon);
        }

        public IInputExchangeItem GetInputExchangeItem(int inputExchangeItemIndex)
        {
            return _inputExchangeItems[inputExchangeItemIndex];
        }

        public IOutputExchangeItem GetOutputExchangeItem(int outputExchangeItemIndex)
        {
            return _outputExchangeItems[outputExchangeItemIndex];
        }

        public IValueSet GetValues(ITime time, string linkID)
        {
            // ----------------------------------------------------------------------------------------------------
            // The method queries a HydroDesktop database and builds an IValueSet object for the given a time 
            // and link.  
            // ----------------------------------------------------------------------------------------------------
            Link l = (Link)_links[linkID];

            IValueSet values;
            if (_exactMatch)
            {

                //
                // Perform Exact Match Routine
                //

                // covert ITime to a DateTime data type
                TimeStamp timestamp = (TimeStamp)time;
                DateTime dt = CalendarConverter.ModifiedJulian2Gregorian(
                    (double)timestamp.ModifiedJulianDay);

                //check to see if the buffer contains this time
                if (_times[linkID].Contains(timestamp.ModifiedJulianDay))
                {
                    values = new ScalarSet(new double[_elementCount[linkID]]);
                    values = _buffer[linkID].GetValues(time);
                    //convert the values to the correct units
                    values = ConvertUnit(values, l);
                }
                else
                    values = new ScalarSet(new double[0]);
            }
            else if (_range != -999)
            {
                //
                // Perform Range Searching Routine
                //

                List<ScalarSet> FoundVals = new List<ScalarSet>();

                // covert ITime to a DateTime data type
                TimeStamp timestamp = (TimeStamp)time;
                DateTime dt = CalendarConverter.ModifiedJulian2Gregorian(
                    (double)timestamp.ModifiedJulianDay);

                values = new ScalarSet(new double[_elementCount[linkID]]);

                //check to see if the current time is within the known time span
                if (timestamp.ModifiedJulianDay <= _endTimes[linkID].ModifiedJulianDay)
                {
                    TimeStamp currTime = (TimeStamp)time;
                    DateTime dateTime = CalendarConverter.ModifiedJulian2Gregorian(currTime.ModifiedJulianDay);
                    //dateTime = dateTime.AddSeconds(_range);
                    //get the earliest and latest times based on the specified time range
                    TimeStamp latestTime = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(dateTime.AddSeconds(_range/2)));
                    TimeStamp earliestTime = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(dateTime.AddSeconds(-1*_range/2)));

                    //check to make sure tht the latest time is less than the last time in the buffer
                    if (latestTime.ModifiedJulianDay > ((TimeStamp)_buffer[linkID].GetTimeAt(_buffer[linkID].TimesCount - 1)).ModifiedJulianDay)
                        values = new ScalarSet(new double[0]);

                    //check to make sure tht the earliest time is greater than the first time in the buffer
                    else if (earliestTime.ModifiedJulianDay < ((TimeStamp)_buffer[linkID].GetTimeAt(0)).ModifiedJulianDay)
                        values = new ScalarSet(new double[0]);

                    
                    else
                    {
                        for (int i=_lastIndex[linkID]; i <= _buffer[linkID].TimesCount - 1; i++)
                        {

                            ////loop through the times to find the start and end indices
                            if (((TimeStamp)_buffer[linkID].GetTimeAt(i)).ModifiedJulianDay >= earliestTime.ModifiedJulianDay
                                && ((TimeStamp)_buffer[linkID].GetTimeAt(i)).ModifiedJulianDay <= latestTime.ModifiedJulianDay)
                            {
                                ScalarSet ss = (ScalarSet)_buffer[linkID].GetValues(_buffer[linkID].GetTimeAt(i));
                                if (ss.data.Length > 0)
                                    FoundVals.Add(ss);
                            }
                            //if the latest time has been passed then break
                            else if (((TimeStamp)_buffer[linkID].GetTimeAt(i)).ModifiedJulianDay > latestTime.ModifiedJulianDay)
                            {
                                //save the index that should be used to start with on the next iteration
                                _lastIndex[linkID] = Convert.ToInt32(Math.Floor(Convert.ToDouble(i) - Convert.ToDouble(_lastIndex[linkID])/2.0));
                                break;
                            }

                        }
                        if (FoundVals.Count == 1)
                        {
                            values = new ScalarSet(FoundVals[0].data);

                            //convert the values to the correct units
                            values = ConvertUnit(values, l);
                        }
                        else if (FoundVals.Count > 1)
                        {
                            double[] ave = FoundVals[0].data;
                            int[] divider = new int[ave.Length];

                            for (int i = 0; i <= ave.Length - 1; i++)
                                if (ave[i] != 0)
                                    divider[i]++;

                            //HACK:  I'm taking the average of all the values.  Probably should do this smarter.
                            for (int i = 0; i <= FoundVals[0].data.Length - 1; i++)
                            {
                                for (int j = 1; j <= FoundVals.Count - 1; j++)
                                {
                                    if (FoundVals[j].data[i] != 0)
                                    {
                                        ave[i] += FoundVals[j].data[i];
                                        divider[i]++;
                                    }
                                }
                            }

                            for (int i = 0; i <= ave.Length - 1; i++)
                                if (divider[i] != 0)
                                    ave[i] = ave[i] / (divider[i]);

                            values = new ScalarSet(ave);

                            //convert the values to the correct units
                            values = ConvertUnit(values, l);
                        }
                        else
                            values = new ScalarSet(new double[0]);
                    }

                }
                else
                {
                    values = new ScalarSet(new double[0]);
                }
             
            }
            else
            {
                //
                //Perform Relaxation Interpolation Routine
                //

                // covert ITime to a DateTime data type
                TimeStamp timestamp = (TimeStamp)time;
                DateTime dt = CalendarConverter.ModifiedJulian2Gregorian(
                    (double)timestamp.ModifiedJulianDay);



                values = new ScalarSet(new double[_elementCount[linkID]]);

                //check to see if the current time is within the known time span
                if (timestamp.ModifiedJulianDay <= _endTimes[linkID].ModifiedJulianDay)
                {
                    //get scalar set
                    //IValueSet values =  _smartBuffer.GetValues(time);
                    values = _buffer[linkID].GetValues(time);

                    //convert the values to the correct units
                    values = ConvertUnit(values, l);

                }

            }

            #region Map Values
            if (mapper.Count > 0)
            {
                IValueSet mappedValues = mapper[linkID].MapValues(values);
                // return values 
                return mappedValues;
            }
            #endregion

            //else return unmapped values (i.e. index based)
            return values;
            
            
        }

        public void Initialize(IArgument[] properties)
        {
            // ----------------------------------------------------------------------------------------------------
            // The method queries a HydroDesktop database and builds OpenMI exchange items based on
            // the themes present in the database. 
            //
            // If omi file has a DbPath attribute, that will set the path to the HydrDesktop database.  If the omi
            // file does not have a DbPath attribute, the path will be set as the current database selected by
            // HydroDesktop.
            // ----------------------------------------------------------------------------------------------------

            //extract argument(s) from OMI file
            foreach (IArgument property in properties)
            {
                if (property.Key == "DbPath") { _dbPath = property.Value; };

                //default value for relationFactor is 1;
                if (property.Key == "Relaxation") 
                {
                    //check to see if the property is a string
                    if (property.Value.GetType().Name == "String")
                    {
                        if (property.Value == "ExactMatch")
                        {
                            _exactMatch = true;
                        }
                    }
                    else
                        _relaxationFactor = Convert.ToDouble(property.Value); 
                }
                else if (property.Key == "RangeInSeconds")
                {
                    _range = Convert.ToInt32(property.Value);
                }
            }

            //get database
            string conn = null;
            if (_dbPath == null)
            {
                //conn = HydroDesktop.Database.Config.DataRepositoryConnectionString;
                conn = Settings.Instance.DataRepositoryConnectionString;
            }
            else
            {
                FileInfo fi = new FileInfo(_dbPath);
                conn = @"Data Source = " + fi.FullName + ";New=False;Compress=True;Version=3";
            }
            _db = new DbOperations(conn, DatabaseTypes.SQLite);
 
            //build list of output exchange items from db themes
            DataTable themes = _db.LoadTable("themes", "SELECT ThemeID, ThemeName from DataThemeDescriptions");
            foreach (DataRow theme in themes.Rows)
            {
                IOutputExchangeItem outExchangeItem = buildExchangeItemFromTheme(theme["ThemeID"].ToString(), theme["ThemeName"].ToString());
                _outputExchangeItems.Add(outExchangeItem);
            }
        }

        public int InputExchangeItemCount
        {
            get { return _inputExchangeItems.Count; }
        }

        public string ModelDescription
        {
            get { return "DbReader"; }
        }

        public string ModelID
        {
            get { return "DbReader"; }
        }

        public int OutputExchangeItemCount
        {
            get { return _outputExchangeItems.Count; }
        }

        public void Prepare()
        {

        }

        public void RemoveLink(string linkID)
        {
            //remove the linkID from the _links dictionary
            _links.Remove(linkID);

            //remove the data associated with this link from the smartbuffer
            _buffer.Remove(linkID);

            //remove linkID element from the _lastIndex dictionary
            _lastIndex.Remove(linkID);

            //remove linkID element from the elementCount dictionary
            _elementCount.Remove(linkID);

            //remove the LinkID entry from the _endTimes dictionary
            _endTimes.Remove(linkID);

            //remove the LinkID entry from the _times dictionary
            _times.Remove(linkID);

        }

        public ITimeSpan TimeHorizon
        {
            get { return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(_earliestInputTime), new TimeStamp(_latestInputTime)); }
        }

        public string Validate()
        {
            return "Validate is not implemented";
        }

        #endregion

        #region IPublisher Members

        public EventType GetPublishedEventType(int providedEventTypeIndex)
        {
            switch (providedEventTypeIndex)
            {
                case 0:
                    return EventType.DataChanged;
                case 1:
                    return EventType.TargetBeforeGetValuesCall;
                case 2:
                    return EventType.SourceAfterGetValuesCall;
                case 3:
                    return EventType.TargetBeforeGetValuesCall;
                case 4:
                    return EventType.TargetAfterGetValuesReturn;
                case 5:
                    return EventType.Informative;
                default:
                    throw new Exception("Illegal index in GetPublishedEventType()");
            }
        }

        public int GetPublishedEventTypeCount()
        {
            return 6;
        }

        public void SendEvent(IEvent Event)
        {
            

        }

        public void Subscribe(IListener listener, EventType eventType)
        {
            
        }

        public void UnSubscribe(IListener listener, EventType eventType)
        {

        }

        #endregion

        private OutputExchangeItem buildExchangeItemFromTheme(string themeID, string themeName)
        {
            // ----------------------------------------------------------------------------------------------------
            // The function queries a HydroDesktop database and builds OpenMI exchange item based on theme id 
            // and name.  
            // ----------------------------------------------------------------------------------------------------

            // Mapping from db to OpenMI
            // 
            // db                                           OpenMI
            // ---                                          -------
            // Variables.VariableName                       quantity.ID
            // Variables.VariableCode                       quantity.Description
            // [Assume OpenMI.Standard.ValueType.Scalar]    quantity.ValueType
            // Units.UnitAbbreviation                       unit.ID
            // [Not Impemented]                             unit.ConversionFactorToSI
            // [Not Implemented]                            unit.OffsetToSI
            // [Not Implemented]                            unit.dimension
            // DataThemeDiscriptions.ThemeName              elementset.ID
            // DataThemeDiscriptions.ThemeDescription       elementset.Description
            // [Assume OpenMI.Standard.ElementType.XYPoint] elementset.ElementType           
            // DataSeries.SeriesID                          element.ID     
            // DataSeries.BeginDateTime                     time horizon start
            // DataSeries.EndDateTime                       time horizon end

            Quantity quantity = new Quantity();
            Unit unit = new Unit();
            Dimension dimension = new Dimension();
            ElementSet elementset = new ElementSet();
            OutputExchangeItem outputexchangeitem = new OutputExchangeItem();

            DataTable dtSeries = null;
            try
            {
                //query db to gather required information based on theme id (including dimension, conversion, offset)
                string sql = "SELECT ds.SeriesID, v.VariableName, v.VariableCode, u.UnitsAbbreviation, td.ThemeName, " +
                        "td.ThemeDescription, ds.BeginDateTime, ds.EndDateTime, ds.SiteID, ds.Dimension, ds.ConversionToSI, ds.OffsetToSI " +
                        "FROM DataThemeDescriptions td " +
                        "INNER JOIN DataThemes t ON td.ThemeID = t.ThemeID " +
                        "INNER JOIN DataSeries ds ON t.SeriesID = ds.SeriesID " +
                        "INNER JOIN Variables v ON ds.VariableID = v.VariableID " +
                        "INNER JOIN Sites s ON ds.SiteID = s.SiteID " +
                        "INNER JOIN Units u ON v.VariableUnitsID = u.UnitsID " +
                        "WHERE t.themeID = '" + themeID.ToString() + "' " +
                        "ORDER BY s.SiteName ASC";

                dtSeries = _db.LoadTable("series", sql);
            }
            catch (Exception)
            {
                //query db to gather required information based on theme id (omitting dimension, conversion, offset)
                string sql = "SELECT ds.SeriesID, v.VariableName, v.VariableCode, u.UnitsAbbreviation, td.ThemeName, " +
                    "td.ThemeDescription, ds.BeginDateTime, ds.EndDateTime, ds.SiteID " +
                    "FROM DataThemeDescriptions td " +
                    "INNER JOIN DataThemes t ON td.ThemeID = t.ThemeID " +
                    "INNER JOIN DataSeries ds ON t.SeriesID = ds.SeriesID " +
                    "INNER JOIN Variables v ON ds.VariableID = v.VariableID " +
                    "INNER JOIN Sites s ON ds.SiteID = s.SiteID " +
                    "INNER JOIN Units u ON v.VariableUnitsID = u.UnitsID " +
                    "WHERE t.themeID = '" + themeID.ToString() + "' " +
                    "ORDER BY s.SiteName ASC";

                dtSeries = _db.LoadTable("series", sql);
            }
                    
            foreach (DataRow row in dtSeries.Rows)
            {
                string seriesID = Convert.ToString(row["SeriesID"]);
                string variableName = Convert.ToString(row["VariableName"]);
                string variableCode = Convert.ToString(row["VariableCode"]);
                string unitsAbbreviation = Convert.ToString(row["UnitsAbbreviation"]);
                //string themeName = Convert.ToString(row["ThemeName"]); PASSED IN AS ARGUMENT
                string themeDescription = Convert.ToString(row["ThemeDescription"]); 
                DateTime beginDateTime = Convert.ToDateTime(row["BeginDateTime"]);
                DateTime endDateTime = Convert.ToDateTime(row["EndDateTime"]);
                string siteID = Convert.ToString(row["SiteID"]);

                //try to get the Dimension, Conversion, and Offset
                string Dimension = null;
                double Conversion2Si = 1;
                double Offset2SI = 0;
                try
                {
                    Dimension = Convert.ToString(row["Dimension"]);
                    Conversion2Si = Convert.ToDouble(row["ConversionToSI"]);
                    Offset2SI = Convert.ToDouble(row["OffsetToSI"]);
                }
                catch (Exception) { }

                if (!String.IsNullOrEmpty(Dimension))
                {
                    string[] Dimensions = Regex.Split(Dimension.ToUpper(), @"([A-Z])([^A-Z]+)");
                    //for (int i = 0; i <= Dimension.Length - 2; i++)
                    //{

                        for (int j = 0; j <= Dimensions.Length - 2; j++)
                        {
                            char dim;
                            int pow;
                            if (Dimensions[j].Length == 0)
                            {
                                dim = 'M';
                                pow = 0;
                            }
                            else
                            {
                                dim = Dimensions[j][0];
                                if (Char.IsLetter(Dimensions[j + 1][0]))
                                    pow = 1;
                                else if (Dimensions[j+1].Length == 0)
                                {
                                    pow = 1;
                                    j++;
                                }
                                else
                                {
                                    pow = Convert.ToInt32(Dimensions[j + 1]);
                                    j++;
                                }
                            }
                            //int pow = Convert.ToInt32(Regex.Split(Dimensions[i], @"/(^[A-Z]+)/"));

                            if (dim == 'M')
                                dimension.SetPower(DimensionBase.Mass, pow);
                            else if (dim == 'L')
                                dimension.SetPower(DimensionBase.Length, pow);
                            else if (dim == 'D')
                                dimension.SetPower(DimensionBase.Temperature, pow);
                            else if (dim == 'T')
                                dimension.SetPower(DimensionBase.Time, pow);
                            else if (dim == 'E')
                                dimension.SetPower(DimensionBase.ElectricCurrent, pow);
                            else if (dim == 'C')
                                dimension.SetPower(DimensionBase.Currency, pow);
                            else
                                throw new Exception(dim + " is an Invalid Unit Dimension!");

                        }
                        
                    //}
                    //string Dimensions[] = Dimension.Split('/A-Z/');
                }

                //will be updated for each row, but each row should be the same so it is OK.
                //TODO: include check to make sure these values are the same for each row.
                quantity.ID = variableName;
                quantity.Description = variableCode;
                quantity.ValueType = global::OpenMI.Standard.ValueType.Scalar;
                unit.ID = unitsAbbreviation;
                unit.ConversionFactorToSI = Conversion2Si;
                unit.OffSetToSI = Offset2SI;
                quantity.Unit = unit;
                //TODO: ignoring unit dimensions for now.
                //Examples below ...
                //dimension.SetPower(DimensionBase.Length, 3);
                //dimension.SetPower(DimensionBase.Time, -1);
                quantity.Dimension = dimension;
                elementset.ID = themeName;
                elementset.Description = themeDescription;
                elementset.ElementType = ElementType.XYPoint;
                Element element = new Element();
                element.ID = seriesID;

                string get_lat_lon = "SELECT s.SiteID, s.Latitude, s.Longitude " +
                                    "FROM Sites s " +
                                    "INNER JOIN DataSeries ds ON ds.SiteID = s.SiteID " +
                                    "WHERE ds.SiteID = " +siteID ;

                DataTable t = _db.LoadTable("values", get_lat_lon);

                Vertex vertex = new Vertex();
                //TODOD: Add x, y for vertex
                vertex.x = Convert.ToDouble(t.Rows[0]["Longitude"]);
                vertex.y = Convert.ToDouble(t.Rows[0]["Latitude"]);

                element.AddVertex(vertex);
                elementset.AddElement(element);

                //update time horizon to be inclusive of this time horizon
                double beginDateTimeDouble = CalendarConverter.Gregorian2ModifiedJulian(beginDateTime);
                double endDateTimeDouble = CalendarConverter.Gregorian2ModifiedJulian(endDateTime);
                if (_earliestInputTime == 0.0) { _earliestInputTime = beginDateTimeDouble; }
                if (beginDateTimeDouble < _earliestInputTime) { _earliestInputTime = beginDateTimeDouble; };
                if (endDateTimeDouble > _latestInputTime) { _latestInputTime = endDateTimeDouble; };
            }

            outputexchangeitem.Quantity = quantity;
            outputexchangeitem.ElementSet = elementset;

            // add data operations and return
            return addDataOperations(outputexchangeitem);
        }
    
        private OutputExchangeItem addDataOperations(OutputExchangeItem outputexchangeitem)
        {
             //Add dataoperations to outputexchangeitems
            ElementMapper elementMapper = new ElementMapper();
            ArrayList dataOperations = new ArrayList();
            dataOperations = elementMapper.GetAvailableDataOperations(outputexchangeitem.ElementSet.ElementType);
            bool spatialDataOperationExists;
            bool linearConversionDataOperationExists;
            bool smartBufferDataOperationExists;
            foreach (IDataOperation dataOperation in dataOperations)
            {
                spatialDataOperationExists = false;
                foreach (IDataOperation existingDataOperation in outputexchangeitem.DataOperations)
                {
                    if (dataOperation.ID == existingDataOperation.ID)
                    {
                        spatialDataOperationExists = true;
                    }
                }

                if (!spatialDataOperationExists)
                {
                    outputexchangeitem.AddDataOperation(dataOperation);
                }
            }

            IDataOperation linearConversionDataOperation = new LinearConversionDataOperation();
            linearConversionDataOperationExists = false;
            foreach (IDataOperation existingDataOperation in outputexchangeitem.DataOperations)
            {
                if (linearConversionDataOperation.ID == existingDataOperation.ID)
                {
                    linearConversionDataOperationExists = true;
                }
            }

            if (!linearConversionDataOperationExists)
            {
                outputexchangeitem.AddDataOperation(new LinearConversionDataOperation());
            }

            IDataOperation smartBufferDataOperaion = new SmartBufferDataOperation();
            smartBufferDataOperationExists = false;
            foreach (IDataOperation existingDataOperation in outputexchangeitem.DataOperations)
            {
                if (smartBufferDataOperaion.ID == existingDataOperation.ID)
                {
                    smartBufferDataOperationExists = true;
                }
            }

            if (!smartBufferDataOperationExists)
            {
                outputexchangeitem.AddDataOperation(new SmartBufferDataOperation());
            }

            return outputexchangeitem;
        }

        /// <summary>
        /// Convert the units according the what is specified in the link.  I took this from Oatc.OpenMI.SDK.Wrapper.SmartOutputLink
        /// </summary>
        /// <param name="values">The values</param>
        /// <returns>The unit converted values</returns>
        private IValueSet ConvertUnit(IValueSet values, Link link)
        {
            double aSource = link.SourceQuantity.Unit.ConversionFactorToSI;
            double bSource = link.SourceQuantity.Unit.OffSetToSI;
            double aTarget = link.TargetQuantity.Unit.ConversionFactorToSI;
            double bTarget = link.TargetQuantity.Unit.OffSetToSI;

            if (aSource != aTarget || bSource != bTarget)
            {
                if (values is IScalarSet)
                {
                    double[] x = new double[values.Count];

                    for (int i = 0; i < values.Count; i++)
                    {
                        x[i] = (((IScalarSet)values).GetScalar(i) * aSource + bSource - bTarget) / aTarget;
                    }

                    return new ScalarSet(x);
                }
                else if (values is IVectorSet)
                {
                    ArrayList vectors = new ArrayList();

                    for (int i = 0; i < values.Count; i++)
                    {
                        double x = (((IVectorSet)values).GetVector(i).XComponent * aSource + bSource - bTarget) / aTarget;
                        double y = (((IVectorSet)values).GetVector(i).YComponent * aSource + bSource - bTarget) / aTarget;
                        double z = (((IVectorSet)values).GetVector(i).ZComponent * aSource + bSource - bTarget) / aTarget;

                        Vector newVector = new Vector(x, y, z);
                        vectors.Add(newVector);
                    }

                    return new VectorSet((Vector[])vectors.ToArray(typeof(Vector)));
                }
                else
                {
                    throw new Exception("Type " + values.GetType().FullName + " not suppported for unit conversion");
                }
            }

            return values;
        }


    }
}

