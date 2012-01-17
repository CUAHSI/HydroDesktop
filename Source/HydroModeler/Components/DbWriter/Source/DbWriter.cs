using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Spatial;
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Database;
using System.IO;
using SharpMap.Geometries;
using System.Data.SqlClient;
using HydroDesktop.Interfaces;
using HydroDesktop.Configuration;

namespace CUAHSI.HIS
{
    public class DbWriter : ILinkableComponent, IListener
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
        private SmartBuffer _smartBuffer = new SmartBuffer();
        //private string _fullPath; //path to the .dll
        public Dictionary<string, Series> serieses = new Dictionary<string, Series>();
        public Dictionary<string, Theme> themes = new Dictionary<string, Theme>();
        public Dictionary<string, Series> dataSeries = new Dictionary<string, Series>();
        string conn = null;
        double _ignore = -999;
        int new_series_count = 0;
        public Dictionary<string, string> dbargs;
        public Dictionary<int, string> series2link;
        private Dictionary<string, List<DateTime>> _timestep = new Dictionary<string,List<DateTime>>();

        private bool getStartTime = true;
        private double _start = 0;

        //private DbOperations _db;

        #region ILinkableComponent Members

        /// <summary>
        /// This method is called when links are created by the component
        /// </summary>
        /// <param name="link">OpenMI link object</param>
        public void AddLink(ILink link)
        {
            //subscribe to events
            ILinkableComponent LC = link.SourceComponent;
            for (int i = 0; i < GetAcceptedEventTypeCount(); i++)
            {
                EventType ev = GetAcceptedEventType(i);
                LC.Subscribe(this, ev);
            }

            //build HD data model object to store time-series
            CreateSeries(link);

            //store the link for future reference
            _links.Add(link.ID, link);

            // create timestep list
            //_timestep.Add(link.ID, new List<DateTime>());
        }

        /// <summary>
        /// Returns the description of the component
        /// </summary>
        public string ComponentDescription
        {
            get { return "DbWriter 1.0"; }
        }

        /// <summary>
        /// Returns the components id
        /// </summary>
        public string ComponentID
        {
            get { return "DbWriter"; }
        }

        /// <summary>
        /// not implemented
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns the earliest need time to execute calculation.  This is used to remove "old" values from memory during simulation
        /// </summary>
        public ITimeStamp EarliestInputTime
        {
            get { return new TimeStamp(_earliestInputTime); }
        }

        /// <summary>
        /// Writes HD time series data object to the data repository
        /// </summary>
        public void Finish()
        {
            RepositoryManagerSQL db = null; 
            
            //check to see if the database path is overridden
            if (conn != null)
                db = new RepositoryManagerSQL(DatabaseTypes.SQLite, conn);
            else
                db = new RepositoryManagerSQL(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);

            //write each series to the database
            foreach (Series series in serieses.Values)
            {

                //-- get the theme
                Theme theme = series.ThemeList[0];

                //-- need to adjust the series values back by one time step
                Dictionary<DateTime, double> new_data = new Dictionary<DateTime, double>();
                

                // determine the timestep using the first two values (assumes uniform timstep)
                double timestep = _timestep[theme.Name][1].Subtract(_timestep[theme.Name][0]).TotalSeconds;

                // change the data value times back 1 time step 
                // this is necessary b/c each model advances it's time before dbwriter gets the data
                for (int i = 0; i <= series.ValueCount - 1; i++)
                {

                    // subtract 1 for the timestep advancement
                    // subtract 1 for the 1-timestep delay that OpenMI creates
                    series.DataValueList[i].DateTimeUTC = series.DataValueList[i].DateTimeUTC.AddSeconds(-2*timestep);
                    series.DataValueList[i].LocalDateTime = series.DataValueList[i].LocalDateTime.AddSeconds(-2*timestep);

                    // remove data value if less than start
                    if (series.DataValueList[i].LocalDateTime < CalendarConverter.ModifiedJulian2Gregorian(_start).AddSeconds(-timestep))
                        series.DataValueList[i].Value = series.GetNoDataValue();
                }

                //-- save data series
                db.SaveSeriesAsCopy(series, theme);

            }
        
            //clear all values in the buffer
            _smartBuffer.Clear(this.TimeHorizon);
        }

        /// <summary>
        /// Used to retrieve input exchange items by index
        /// </summary>
        /// <param name="inputExchangeItemIndex">index</param>
        /// <returns>input exchange item</returns>
        public IInputExchangeItem GetInputExchangeItem(int inputExchangeItemIndex)
        {
            return _inputExchangeItems[inputExchangeItemIndex];
        }

        /// <summary>
        /// Used to retrieve output exchange items by index
        /// </summary>
        /// <param name="outputExchangeItemIndex">index</param>
        /// <returns>output exchange item</returns>
        public IOutputExchangeItem GetOutputExchangeItem(int outputExchangeItemIndex)
        {
            return _outputExchangeItems[outputExchangeItemIndex];
        }

        /// <summary>
        /// Not implemented. Returns an empty data because this component should not be used to supply data to other models
        /// </summary>
        /// <param name="time">requested time</param>
        /// <param name="linkID">requested link</param>
        /// <returns>empty scalarset</returns>
        public IValueSet GetValues(ITime time, string linkID)
        {
            return new ScalarSet();
        }

        /// <summary>
        /// This method is used to construct the component
        /// </summary>
        /// <param name="properties">arguments stored in the *.omi file</param>
        public void Initialize(IArgument[] properties)
        {

            //extract argument(s) from OMI file
            foreach (IArgument property in properties)
            {
                //overwrite the connection string, if one is given in the *.omi
                if (property.Key == "DbPath") 
                    _dbPath = property.Value;

                //default value for relationFactor is 1;
                if (property.Key == "Relaxation") { _smartBuffer.RelaxationFactor = Convert.ToDouble(property.Value); }
                if (property.Key == "IgnoreValue") { _ignore = Convert.ToDouble(property.Value); }
            }

            //---- set database to default if dbpath is invalid
            string fullpath = "";
            //-- first check if dbpath is null
            bool pass = true;
            if (String.IsNullOrWhiteSpace(_dbPath))
                pass = false;
            //-- next, check that dbpath points to an actual file
            else
            {
                //-- if relative path is given
                if (!Path.IsPathRooted(_dbPath))
                {
                    fullpath = System.IO.Path.GetFullPath(System.IO.Directory.GetCurrentDirectory() + _dbPath);
                }
                //-- if absolute path
                else
                {
                    fullpath = System.IO.Path.GetFullPath(_dbPath);
                }

                if (!File.Exists(fullpath))
                {
                    pass = false;

                    //-- warn the user that the database could not be found
                    System.Windows.Forms.MessageBox.Show("The database supplied in DbWriter.omi could not be found. As a result the DbWriter will connect to the current HydroDesktop database." +
                                "\n\n--- The following database could not be found --- \n" + fullpath,
                        "An Error Occurred While Loading Database...",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }


            //-- set the connection string
            if (!pass)
                conn = Settings.Instance.DataRepositoryConnectionString;
            else
            {
                //FileInfo fi = new FileInfo(fullpath);
                //conn = @"Data Source = " + fi.FullName + ";New=False;Compress=True;Version=3";
                conn = @"Data Source = " + fullpath + ";New=False;Compress=True;Version=3";
            }


            //---- read db info provided by omi
            dbargs = ReadDbArgs(properties);
            
            //---- create generic input and output exchange items
            InputExchangeItem inExchangeItem = new InputExchangeItem();
            inExchangeItem.ElementSet = new ElementSet("any element set", "any element set", ElementType.IDBased, new Oatc.OpenMI.Sdk.Backbone.SpatialReference("1"));
            inExchangeItem.Quantity = new Quantity("any quantity");
            _inputExchangeItems.Add(inExchangeItem);

            OutputExchangeItem outExchangeItem = new OutputExchangeItem();
            outExchangeItem.ElementSet = new ElementSet("dummy element set", "dummy element set", ElementType.IDBased, new Oatc.OpenMI.Sdk.Backbone.SpatialReference("1"));
            outExchangeItem.Quantity = new Quantity("dummy quantity");
            _outputExchangeItems.Add(outExchangeItem);

            //---- define arbitrary start and end times
            _earliestInputTime = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(1900,1,1));
            _latestInputTime = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(2100, 12, 31));

        }

        /// <summary>
        /// The number of input exchange items
        /// </summary>
        public int InputExchangeItemCount
        {
            get { return _inputExchangeItems.Count; }
        }

        /// <summary>
        /// description of the model component
        /// </summary>
        public string ModelDescription
        {
            get { return "DbWriter"; }
        }

        /// <summary>
        /// The id of the model.  This is the name shown up in the configuration window
        /// </summary>
        public string ModelID
        {
            get { return "DbWriter"; }
        }

        /// <summary>
        /// number of output exchange items
        /// </summary>
        public int OutputExchangeItemCount
        {
            get { return _outputExchangeItems.Count; }
        }

        /// <summary>
        /// not implemented
        /// </summary>
        public void Prepare()
        {
            
        }

        /// <summary>
        /// This is called when a link is removed from the compostion.
        /// </summary>
        /// <param name="linkID">id of the link that was removed</param>
        public void RemoveLink(string linkID)
        {
            _links.Remove(linkID);
        }

        /// <summary>
        /// Defines the model time horizon
        /// </summary>
        public ITimeSpan TimeHorizon
        {
            get { return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(_earliestInputTime), new TimeStamp(_latestInputTime)); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns>validation token</returns>
        public string Validate()
        {
            return "Validate is not implemented";
        }

        #endregion

        #region IPublisher Members

        /// <summary>
        /// returns all of the events that are recognized by this component
        /// </summary>
        /// <param name="providedEventTypeIndex">index</param>
        /// <returns>an event</returns>
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
                    throw new Exception("Iligal index in GetPublishedEventType()");
            }
        }

        /// <summary>
        /// Defines the number of recognized events
        /// </summary>
        /// <returns>6</returns>
        public int GetPublishedEventTypeCount()
        {
            return 6;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="Event">the event that is sent</param>
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

        #region IListener Members

        /// <summary>
        /// defines events that are accepted by this component
        /// </summary>
        /// <param name="acceptedEventTypeIndex">index</param>
        /// <returns>an event</returns>
        public EventType GetAcceptedEventType(int acceptedEventTypeIndex)
        {
            switch (acceptedEventTypeIndex)
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
                    throw new Exception("Iligal index in GetPublishedEventType()");
            }       
        }

        /// <summary>
        /// the number of accepted events
        /// </summary>
        /// <returns>6</returns>
        public int GetAcceptedEventTypeCount()
        {
            return 6;
        }

        /// <summary>
        /// Defines the actions that occur every time an event is recognized
        /// </summary>
        /// <param name="anEvent">the event that was triggered</param>
        public void OnEvent(IEvent anEvent)
        {
            
            if (anEvent.Type == EventType.DataChanged)
            {

                //get the current time
                TimeStamp ts = (TimeStamp)anEvent.SimulationTime;

                // get the simulation start time
                if (getStartTime)
                    _start = ts.ModifiedJulianDay; getStartTime = false;

                //get values
                ScalarSet vals = new ScalarSet();
                Link link = new Link();
                string linkID = null;
                foreach (string key in this._links.Keys)
                {
                    link = (Link)_links[key];

                    //make sure it gets values from the link that sent the event
                    if(link.TargetComponent == this && link.SourceComponent.ModelID == anEvent.Sender.ModelID)
                    {
                        //define theme
                        string themeDescription = link.SourceElementSet.Description;
                        string themeName = link.SourceElementSet.ID;
                        Theme theme = new Theme(themeName, themeDescription);

                        //save dt info for the timestep
                        if (!_timestep.ContainsKey(themeName))
                            _timestep.Add(themeName, new List<DateTime>());
                        if (_timestep[themeName].Count < 2)
                            _timestep[themeName].Add(CalendarConverter.ModifiedJulian2Gregorian(ts.ModifiedJulianDay));

                        //get link values
                        vals = (ScalarSet)anEvent.Sender.GetValues(ts, key);

                        //save link theme
                        if (!themes.ContainsKey(link.ID))
                            themes.Add(link.ID, theme);

                        //save link id
                        linkID = link.SourceElementSet.ID;

                        break;
                    }
                }

                //if values are found, then store them 
                if (vals.data.Length > 0)
                {
                    int j = 0;
                    try
                    {
                        while (j <= serieses.Count - 1)
                        {
                            //for(int k=0;k<= vals.data.Length-1;k++)
                            //{

                                //checks to see if the value should be ignored.  This is defined in omi.
                                if (vals.data[j] != _ignore) 
                                {
                                    //get the site name
                                    string id = link.SourceElementSet.GetElementID(j);
                                    if (id == "")
                                        id = link.SourceElementSet.ID+j.ToString();

                                    string sql = "SELECT s.SiteName " +
                                                    "FROM Sites s " +
                                                    "INNER JOIN DataSeries ds ON s.SiteID = ds.SiteID " +
                                                    "WHERE ds.SeriesID= '" + id + "' ";

                                    //"ORDER BY dv.DataValue ASC";
                                    DbOperations _db  = new DbOperations(conn, DatabaseTypes.SQLite);
                                    System.Data.DataTable tbl = _db.LoadTable("values", sql);

                                    //string siteName = linkID + j.ToString();
                                    string siteName = null;
                                    if (tbl.Rows.Count > 0)
                                        siteName = tbl.Rows[0].ItemArray[0].ToString();
                                    else
                                        siteName = (link.SourceElementSet.ID +"_"+ link.SourceComponent.ModelID +"_loc"+ j.ToString()).Replace(' ', '_');


                                    //check to see if series exists
                                    if (serieses.ContainsKey(siteName))
                                    {

                                        //-- get the series
                                        Series series = serieses[siteName];

                                        //-- store the associated theme
                                        if(!series.ThemeList.Contains(themes[link.ID]))
                                            series.ThemeList.Add(themes[link.ID]);

                                        //-- save data values
                                        series.AddDataValue(CalendarConverter.ModifiedJulian2Gregorian(ts.ModifiedJulianDay), vals.data[j]);
                                    }
                                } 
                            //}
                            j++;
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                }
            }
        }
        #endregion

        /// <summary>
        /// build HD data model to store time-series data
        /// </summary>
        /// <param name="link">the link that was added to the composition</param>
        public void CreateSeries(ILink link)
        {

            #region Create DataModel Objects [HACK]

            //---- create variable unit
            HydroDesktop.Interfaces.ObjectModel.Unit VarUnit = new HydroDesktop.Interfaces.ObjectModel.Unit();
            VarUnit.Name = link.SourceQuantity.Unit.Description;    //defined by link
            VarUnit.Abbreviation = link.SourceQuantity.Unit.ID;     //defined by link
            VarUnit.UnitsType = link.SourceQuantity.ID;             //defined by link

            //---- create time unit
            HydroDesktop.Interfaces.ObjectModel.Unit TimeUnit = new HydroDesktop.Interfaces.ObjectModel.Unit();
            TimeUnit.Name = "second";           //default value (cannot be changed) 
            TimeUnit.Abbreviation = "s";        //default value (cannot be changed)
            TimeUnit.UnitsType = "Time";        //default value (cannot be changed)

            //create unit
            //HydroDesktop.Interfaces.ObjectModel.Unit unit = new HydroDesktop.Interfaces.ObjectModel.Unit();
            //unit.Abbreviation = link.SourceQuantity.Unit.ID;

            //---- create method
            HydroDesktop.Interfaces.ObjectModel.Method method = new Method();
            method.Link = link.SourceComponent.ModelID;
            method.Link = "none";                                           //*default value
            method.Description = link.SourceComponent.ModelDescription;     //*default value
            if (link.SourceComponent.ComponentDescription == null)
                method.Description = "none";

            //---- define data service info
            DataServiceInfo dataservice = new DataServiceInfo();
            dataservice.Abstract = "none";                                  //*default value
            dataservice.Citation = "none";                                  //*default value
            dataservice.ContactEmail = "none";                              //*default value
            dataservice.ContactName = "none";                               //*default value
            dataservice.EastLongitude = -999;                               //*default value
            dataservice.HarveDateTime = DateTime.Now;                       //*default value
            dataservice.Id = -999;                                          //*default value
            dataservice.NorthLatitude = -999;                               //*default value
            dataservice.HISCentralID = -999;                                //*default value
            dataservice.ServiceCode = "none";                               //*default value
            dataservice.DescriptionURL = "none";                            //*default value
            dataservice.EndpointURL = "none";                               //*default value
            dataservice.ServiceName = "none";                               //*default value
            dataservice.Protocol = "none";                                  //*default value
            dataservice.ServiceTitle = "none";                              //*default value
            dataservice.ServiceType = "none";                               //*default value
            dataservice.Version = -999;                                     //*default value
            dataservice.SiteCount = link.SourceElementSet.ElementCount;     //defined by link

            //---- create metadata
            ISOMetadata meta = new ISOMetadata();
            meta.Abstract = "none";                                         //*default value
            meta.Id = -999;                                                 //*default value
            meta.ProfileVersion = "none";                                   //*default value
            meta.Title = "none";                                            //*default value
            meta.TopicCategory = "none";                                    //*default value
            meta.MetadataLink = "none";                                     //*default value

            //---- create source
            Source source = new Source();
            source.Organization = "University of South Carolina";           //*default value
            source.Address = "300 Main St.";                                //*default value
            source.Citation = "none";                                       //*default value
            source.City = "Columbia";                                       //*default value
            source.ContactName = "none";                                    //*default value
            source.Description = "none";                                    //*default value
            source.Email = "none";                                          //*default value
            source.Id = -999;                                               //*default value
            source.Link = "none";                                           //*default value
            source.OriginId = -999;                                         //*default value
            source.Phone = "none";                                          //*default value
            source.State = "SC";                                            //*default value
            source.ZipCode = 29206;                                         //*default value
            source.ISOMetadata = meta;

            //---- create variable
            Variable variable = new Variable();
            variable.Code = link.SourceQuantity.Description;                //defined by link
            variable.Name = link.SourceQuantity.ID;                         //defined by link
            variable.VariableUnit = VarUnit;                                //defined by link
            variable.TimeUnit = TimeUnit;                                   //defined by link
            variable.Speciation = "H20";                                    //*default value
            variable.GeneralCategory = "Hydrology";                         //*default value
            variable.NoDataValue = -999;                                    //*default value
            variable.SampleMedium = "Surface Water";                        //*default value
            variable.TimeSupport = 1;                                       //TODO: determine in finish
            variable.VocabularyPrefix = "none";                             //*default value
            variable.ValueType = "Model Simulation Result";                 //*default value
            variable.DataType = "Incremental";                              //*default value

            //---- create qualControl
            QualityControlLevel qualControl = new QualityControlLevel();
            qualControl.Code = "qual1";                                     //*default value
            qualControl.Definition = "Quality control level 1";             //*default value
            qualControl.Id = 1;                                             //*default value
            qualControl.Explanation = "unknown";                            //*default value
            qualControl.OriginId = -999;                                    //*default value

            #endregion


            #region Build Sites

            RepositoryManagerSQL db = null;

            //check to see if the database path is overridden
            if (conn != null)
            {
                db = new RepositoryManagerSQL(DatabaseTypes.SQLite, conn);
            }
            else
            {
                db = new RepositoryManagerSQL(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);
                //conn = Settings.Instance.DataRepositoryConnectionString;
            }

            //---- override default db info with those provided by omi
            //-- standard omi args
            if (dbargs.ContainsKey("Method.Description")) { method.Description = dbargs["Method.Description"];}
            if (dbargs.ContainsKey("Source.Organization")) { source.Organization = dbargs["Source.Organization"]; }
            if (dbargs.ContainsKey("Source.Address")) { source.Address = dbargs["Source.Address"]; }
            if (dbargs.ContainsKey("Source.City")) { source.City = dbargs["Source.City"]; }
            if (dbargs.ContainsKey("Source.State")) { source.State = dbargs["Source.State"]; }
            if (dbargs.ContainsKey("Source.Zip")) { source.ZipCode = Convert.ToInt32(dbargs["Source.Zip"]); }
            if (dbargs.ContainsKey("Source.Contact")) { source.ContactName = dbargs["Source.Contact"]; }
            if (dbargs.ContainsKey("Variable.Category")) { variable.GeneralCategory = dbargs["Variable.Category"]; }
            if (dbargs.ContainsKey("Variable.SampleMedium")) { variable.SampleMedium = dbargs["Variable.SampleMedium"];}

            //-extra omi args
            if (dbargs.ContainsKey("Method.Link")) { method.Link = dbargs["Method.Link"]; }
            if (dbargs.ContainsKey("DataService.Abstract")) { dataservice.Abstract = dbargs["DataService.Abstract"]; }
            if (dbargs.ContainsKey("DataService.Citation")) { dataservice.Citation = dbargs["DataService.Citation"]; }
            if (dbargs.ContainsKey("DataService.ContactEmail")) { dataservice.ContactEmail = dbargs["DataService.ContactEmail"]; }
            if (dbargs.ContainsKey("DataService.ContactName")) { dataservice.ContactName = dbargs["DataService.ContactName"]; }
            if (dbargs.ContainsKey("DataService.EastLongitude")) { dataservice.EastLongitude = Convert.ToDouble(dbargs["DataService.EastLongitude"]); }
            if (dbargs.ContainsKey("DataService.HarveDateTime")) { dataservice.HarveDateTime = Convert.ToDateTime(dbargs["DataService.HarveDateTime"]); }
            if (dbargs.ContainsKey("DataService.ID")) { dataservice.Id = Convert.ToInt64(dbargs["DataService.ID"]); }
            if (dbargs.ContainsKey("DataService.NorthLatitude")) { dataservice.NorthLatitude = Convert.ToDouble(dbargs["DataService.NorthLatitude"]); }
            if (dbargs.ContainsKey("DataService.HISCentralID")) { dataservice.HISCentralID = Convert.ToInt32(dbargs["DataService.HISCentralID"]); }
            if (dbargs.ContainsKey("DataService.ServiceCode")) { dataservice.ServiceCode = dbargs["DataService.ServiceCode"]; }
            if (dbargs.ContainsKey("DataService.DescriptionURL")) { dataservice.DescriptionURL = dbargs["DataService.DescriptionURL"]; }  
            if (dbargs.ContainsKey("DataService.EndpointURL")) { dataservice.EndpointURL = dbargs["DataService.EndpointURL"]; }
            if (dbargs.ContainsKey("DataService.ServiceName")) { dataservice.ServiceName = dbargs["DataService.ServiceName"]; }
            if (dbargs.ContainsKey("DataService.Protocol")) { dataservice.Protocol = dbargs["DataService.Protocol"]; }
            if (dbargs.ContainsKey("DataService.ServiceTitle")) { dataservice.ServiceTitle = dbargs["DataService.ServiceTitle"]; }
            if (dbargs.ContainsKey("DataService.ServiceType")) { dataservice.ServiceType = dbargs["DataService.ServiceType"]; }
            if (dbargs.ContainsKey("DataService.Version")) { dataservice.Version = Convert.ToDouble(dbargs["DataService.Version"]); }
                
            if (dbargs.ContainsKey("ISOMetadata.Abstract")) { meta.Abstract = dbargs["ISOMetadata.Abstract"]; }
            if (dbargs.ContainsKey("ISOMetadata.ID")) { meta.Id = Convert.ToInt64(dbargs["ISOMetadata.ID"]); }
            if (dbargs.ContainsKey("ISOMetadata.ProfileVersion")) { meta.ProfileVersion = dbargs["ISOMetadata.ProfileVersion"]; }
            if (dbargs.ContainsKey("ISOMetadata.Title")) { meta.Title = dbargs["ISOMetadata.Title"]; }
            if (dbargs.ContainsKey("ISOMetadata.TopicCategory")) { meta.TopicCategory = dbargs["ISOMetadata.TopicCategory"]; }
            if (dbargs.ContainsKey("ISOMetadata.MetadataLink")) { meta.MetadataLink = dbargs["ISOMetadata.MetadataLink"]; }
                
            if (dbargs.ContainsKey("Source.Citation")) { source.Citation = dbargs["Source.Citation"]; }
            if (dbargs.ContainsKey("Source.Description")) { source.Description = dbargs["Source.Description"]; }
            if (dbargs.ContainsKey("Source.Email")) { source.Email = dbargs["Source.Email"]; }
            if (dbargs.ContainsKey("Source.ID")) { source.Id = Convert.ToInt64(dbargs["Source.ID"]); }
            if (dbargs.ContainsKey("Source.Link")) { source.Link = dbargs["Source.Link"]; }
            if (dbargs.ContainsKey("Source.OrginID")) { source.OriginId = Convert.ToInt32(dbargs["Source.OrginID"]); } 
            if (dbargs.ContainsKey("Source.Phone")) { source.Phone = dbargs["Source.Phone"]; }

            if (dbargs.ContainsKey("Variable.Code")) { variable.Code = dbargs["Variable.Code"]; }
            if (dbargs.ContainsKey("Variable.Name")) { variable.Name = dbargs["Variable.Name"]; }
            if (dbargs.ContainsKey("Variable.Speciation")) { variable.Speciation = dbargs["Variable.Speciation"]; }
            if (dbargs.ContainsKey("Variable.NoDataValue")) { variable.NoDataValue = Convert.ToDouble(dbargs["Variable.NoDataValue"]); }
            if (dbargs.ContainsKey("Variable.VocabPrefix")) { variable.VocabularyPrefix = dbargs["Variable.VocabPrefix"]; }
            if (dbargs.ContainsKey("Variable.ValueType")) { variable.ValueType = dbargs["Variable.ValueType"]; }
            if (dbargs.ContainsKey("Variable.DataValue")) { variable.DataType = dbargs["Variable.DataValue"]; }

            if (dbargs.ContainsKey("QualityControl.Code")) { qualControl.Code = dbargs["QualityControl.Code"]; }
            if (dbargs.ContainsKey("QualityControl.Definition")) { qualControl.Definition = dbargs["QualityControl.Definition"]; }
            if (dbargs.ContainsKey("QualityControl.ID")) { qualControl.Id = Convert.ToInt64(dbargs["QualityControl.ID"]); }
            if (dbargs.ContainsKey("QualityControl.Explanation")) { qualControl.Explanation = dbargs["QualityControl.Explanation"]; }
            if (dbargs.ContainsKey("QualityControl.OriginId")) { qualControl.OriginId = Convert.ToInt32(dbargs["QualityControl.OriginId"]); }

            #region create sites
            Dictionary<long, Site> sites = new Dictionary<long, Site>();

            //loop through all elements in the source components element set
            for (int i = 0; i < link.SourceElementSet.ElementCount; ++i)
            {
                //TODO: Get spatial reference from elementset
                //---- define spatial reference
                HydroDesktop.Interfaces.ObjectModel.SpatialReference spatial = new HydroDesktop.Interfaces.ObjectModel.SpatialReference();
                spatial.Id = 18;                                                    //*
                spatial.Notes = "NAD27-UTM zone 17N projected coordinate";          //*
                spatial.SRSID = 26717;                                              //*
                spatial.SRSName = "NAD27 / UTM zone 17N";                           //*

                //--- create site ---
                Site site = new Site();

                //create a unique site name [variable_model_location]
                site.Name = (link.SourceElementSet.ID + "_" + link.SourceComponent.ModelID + "_loc" + i.ToString()).Replace(' ', '_');

                //check if a sitename already exists in the repository
                string sql = "SELECT s.SiteCode, s.SiteID " +
                                                    "FROM Sites s " +
                                                    "WHERE s.SiteName= '" + site.Name + "' ";

                DbOperations _db = new DbOperations(conn, DatabaseTypes.SQLite);
                System.Data.DataTable tbl = _db.LoadTable("values", sql);

                if (tbl.Rows.Count > 0)
                {
                    site.Code = tbl.Rows[0].ItemArray[0].ToString();
                    site.Id = Convert.ToInt32(tbl.Rows[0].ItemArray[1]);
                }
                else
                {
                    //create a new site
                    sql = "SELECT s.SiteCode, s.SiteID FROM Sites s ";

                    _db = new DbOperations(conn, DatabaseTypes.SQLite);
                    tbl = _db.LoadTable("values", sql);
                    int last_row = tbl.Rows.Count - 1;

                    site.Code = site.Name;
                    //-- if the database is not blank
                    if (last_row >= 0)
                    {
                        site.Id = Convert.ToInt32(tbl.Rows[last_row].ItemArray[1]) + 2 + new_series_count;
                    }
                    else
                        site.Id = new_series_count++;

                    //add 1 to new series count so that the same site code isn't selected twice
                    new_series_count++;
                }

                site.SpatialReference = spatial;
                site.Comments = "none";
                site.County = "none";
                site.Elevation_m = -999;
                site.Latitude = -999;
                site.LocalProjection = spatial;
                site.LocalX = -999;
                site.LocalY = -999;
                site.Longitude = -999;


                //--- Attempt to spatially define elements 7-15-2010 ---
                try
                {

                    //save site latitude and longitude
                    if (link.SourceElementSet.ElementType == ElementType.XYPoint)
                    {
                        site.Latitude = link.SourceElementSet.GetYCoordinate(i, 0);
                        site.Longitude = link.SourceElementSet.GetXCoordinate(i, 0);
                        site.LocalX = 0;
                        site.LocalY = 0;
                    }
                    else
                    {
                        ////List<Point> points = new List<Point>();
                        ////for(int p=0; p<= link.SourceElementSet.GetVertexCount(i)-1; p++)
                        ////{
                        ////        Point point = new Point(link.SourceElementSet.GetXCoordinate(i, p),
                        ////            link.SourceElementSet.GetYCoordinate(i, p));

                        ////        points.Add(point);
                        ////}
                        //////create polyline
                        ////LineString ls = new LineString(points);

                        ////new SharpMap.CoordinateSystems.ProjectedCoordinateSystem()
                        ////ls.SpatialReference = 

                        site.Latitude = link.SourceElementSet.GetYCoordinate(i, 0);
                        site.Longitude = link.SourceElementSet.GetXCoordinate(i, 0);

                        //link.SourceElementSet.SpatialReference.ID;
                        site.LocalX = 0;
                        site.LocalY = 0;



                    }
                }
                catch (Exception) { }


                site.NetworkPrefix = "none";
                site.PosAccuracy_m = -999;
                site.State = "SC";                  //*

                //site.TimeZone = new TimeZoneInfo.TransitionTime();

                site.VerticalDatum = "unknown";     //*

                if (!sites.ContainsKey(site.Id))
                {
                    //add site and series to dictionary if they don't already exist
                    sites.Add(site.Id, site);
                    Series series = new Series(site, variable, method, qualControl, source);
                    series.Id = i;
                    if (!serieses.ContainsKey(site.Name)) { serieses.Add(site.Name, series); }
                    //else{serieses.Add(site.Name, series);}

                }
            }
            #endregion

            #endregion

        }

        public Dictionary<string, string> ReadDbArgs(IArgument[] arguments)
        {
            //---- enumerate over arguments
            var e = arguments.AsEnumerable();
            Dictionary<string, string> dict = new Dictionary<string,string>();

            //---- store each argument in a dictionary
            foreach (object obj in e)
                if (!String.IsNullOrWhiteSpace(((Argument)obj).Value)) //ignore missing values
                    dict.Add(((Argument)obj).Key, ((Argument)obj).Value);

            return dict;
        }
    }
}
