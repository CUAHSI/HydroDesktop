using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using Oatc.OpenMI.Sdk.Spatial;
using Oatc.OpenMI.Sdk.Wrapper;
using WaterOneFlow.Schema.v1;
using Oatc.OpenMI.Sdk.Buffer;
using System.Linq;

namespace CUAHSI.HIS
{
    public class HydroLink : ILinkableComponent
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

        //dictionary that contains a smartbuffer for each link
        private Dictionary<string, SmartBuffer> _buffervalues = new Dictionary<string, SmartBuffer>();

        private string _fullPath; //path to the .dll

        

        #region ILinkableComponent Members

        public void AddLink(ILink link)
        {
            _links.Add(link.ID, link);
            SmartBuffer sb = new SmartBuffer();
            sb = CreateBuffer(link.SourceElementSet.ToString());

            _buffervalues.Add(link.ID, sb);
            //AddToBuffer(link.SourceElementSet.ToString());

        }

        private SmartBuffer CreateBuffer(string elementSet)
        {
            Dictionary<DateTime, ArrayList> dict = new Dictionary<DateTime, ArrayList>();
            SmartBuffer smartbuffer = new SmartBuffer();

            //Move to the .dll directory
            try
            {
                Directory.SetCurrentDirectory(_fullPath);
            }
            catch (System.IO.IOException) { }


            elementSet = _dbPath + "\\" + elementSet;
            string[] files = Directory.GetFiles(elementSet);

            

            //read all files within element set
            foreach (string file in files)
            {

                //load the first file in the directory as an XML document
                XmlDocument xmldoc = new XmlDocument();

                // load the first xml file in the directory
                StreamReader sr = new StreamReader(file);

                //deserialize
                XmlSerializer xml_reader = new XmlSerializer(typeof(TimeSeriesResponseType));
                TimeSeriesResponseType tsr = (TimeSeriesResponseType)xml_reader.Deserialize(sr);

                
                ValueSingleVariable[] values = tsr.timeSeries.values.value;

                foreach (ValueSingleVariable value in values)
                {
                    DateTime dt = value.dateTime;
                    double v = Convert.ToDouble(value.Value);

                    //check to see if time/value combination has been already added
                    if (dict.ContainsKey(dt))
                    {
                        ArrayList a = dict[dt];
                        a.Add(v);
                    }
                    //Add key to dictionary
                    else
                    {
                        ArrayList a = new ArrayList();
                        a.Add(v);
                        dict.Add(dt, a);
                    }

                }

            }

            //put values in oder, starting with the earliest time (from http://dotnetperls.com/sort-dictionary-values)
            var items = from k in dict.Keys
                        orderby dict[k] ascending
                        select k;

            //load values into the smart buffer
            foreach (KeyValuePair<DateTime, ArrayList> kvp in dict)
            {
                ITimeStamp time_stmp = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(kvp.Key));
                double[] valueset = (double[])kvp.Value.ToArray(typeof(double));
                smartbuffer.AddValues(time_stmp, new ScalarSet(valueset));
            }
            return smartbuffer;

        }

        public string ComponentDescription
        {
            get { return "HydroLink 1.0"; }
        }

        public string ComponentID
        {
            get { return "HydroLink"; }
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
            _smartBuffer.Clear(this.TimeHorizon);
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
            
            // covert time to a DateTime data type
            TimeStamp timestamp = (TimeStamp)time;
            DateTime dt = CalendarConverter.ModifiedJulian2Gregorian(
                (double)timestamp.ModifiedJulianDay);

            // write datetime to log file
            StreamWriter sr = new StreamWriter("hydrolink_log.txt", true);
            sr.WriteLine("get values request for time: " + dt.ToLongDateString() 
                + " " + dt.ToLongTimeString());
            sr.Flush();

            //get scalar set
            IValueSet values = _buffervalues[linkID].GetValues(time);
            
            // write value set to log file
            ScalarSet ss = (ScalarSet)values;
            sr.Write("values set: ");
            sr.Flush();
            for (int i = 0; i < ss.Count; ++i)
            {
                sr.Write(" " + ss.GetScalar(i).ToString() + " ");
                sr.Flush();
            }
            sr.Write("\n");
            sr.Close();
            return values; 
        }

        public void Initialize(IArgument[] properties)
        {

            // The code loops through the WaterMLdb and builds OpenMI exchange items based on 
            // the folder contents. It is assumed that the WaterMLdb has the following directory 
            // structure:
            //
            // - db [Folder]
            //   - exchange_item_1_name [Folder]
            //     - watermlfile1.xml [File]
            //     - watermlfile2.xml [File]
            //     ...
            //   - exchange_item_2_name 
            //     - watermlfile1.xml 
            //     - watermlfile2.xml
            //     ...
            //   ...
            //
            // All WaterML files within an exchange item folder must have the same quantity because
            // OpenMI defines an exchange item as being for one quantity and one or more locations.
            // A WaterML file contains only one "time series", that is measurements of one variable
            // through time at only one location.


            //extract argument(s) from OMI file
            foreach (IArgument property in properties)
            {
                if (property.Key == "WaterMLdb") { _dbPath = property.Value; };
                //default value for relationFactor is 1;
                if (property.Key == "Relaxation") { _smartBuffer.RelaxationFactor = Convert.ToDouble(property.Value); };
            }

            _fullPath = Directory.GetCurrentDirectory();
            

            
            string[] subdirs = Directory.GetDirectories(_dbPath);
            foreach (string subdir in subdirs)
            {
                try
                {
                    OutputExchangeItem exchangeitem = buildExchangeItemFromFolder(subdir);
                    if (exchangeitem != null) { _outputExchangeItems.Add(exchangeitem); };
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Could not read exchange item directory " + subdir + ". " +
                        "Error message: " + ex.Message);
                }
             }
        }

        public int InputExchangeItemCount
        {
            get { return _inputExchangeItems.Count; }
        }

        public string ModelDescription
        {
            get { return "HydroLink"; }
        }

        public string ModelID
        {
            get { return "HydroLink"; }
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
            _links.Remove(linkID);
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
            return EventType.Other;
        }

        public int GetPublishedEventTypeCount()
        {
            return 0;
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

        private OutputExchangeItem buildExchangeItemFromFolder(string folder)
        {
            //The routine will construct an OpenMI exchange item from a folder of WaterML files.
            //assumptions: (1) all files within the folder are waterML files
            //             (2) all files within folder have the same variable element
            //
            // --- MAPPING BETWEEN OPENMI AND WATERML 
            // Qunatity <-- from the first file in the directory
            //   ID [REQUIRED] = WaterML's variableParam element inner text 
            //   Description [optional] = WaterML's variableName element inner text 
            //   ValueType [hard coded] = Scalar
            //   Unit 
            //     ID [optional]= WaterML's units element attribute unitsAbbreviation 
            //     ConversionFactortoSI [optional] = not in WaterML
            //     ConverstionOffsettoSI [optional] = not in WaterML
            //   Dimension 
            //     Power [optional] = not in WaterML
            // ElementSet
            //   ID [REQUIRED] = folder name
            //   Description [REQUIRED] = folder relative path
            //   ElementType [hard coded] = XYPoint or IDBased  
            //   Element
            //     ID [optional]= WaterML's SiteCode element inner text [changing to locationParam]
            //     Vertex
            //       X = WaterML's longitude element inner text
            //       Y = WaterML's latitude element inner text
            //   ...
            // TimeHorizon <-- union of all file-level time horizons
            
            //get list of files within the folder 
            string[] files = Directory.GetFiles(folder);

            //load the first file in the directory as an XML document
            XmlDocument xmldoc = new XmlDocument();

            // load the first xml file in the directory
            StreamReader sr = new StreamReader(files[0]);

            //deserialize
            XmlSerializer xml_reader = new XmlSerializer(typeof(TimeSeriesResponseType));
            TimeSeriesResponseType tsr = (TimeSeriesResponseType)xml_reader.Deserialize(sr);
            
            Quantity quantity = new Quantity();
            Unit unit = new Unit();
            Dimension dimension = new Dimension();
            ElementSet elementset = new ElementSet();
            OutputExchangeItem outputexchangeitem = new OutputExchangeItem();

            //Quantity ID -- REQUIRED
            try {quantity.ID = tsr.queryInfo.criteria.variableParam;}
            catch {throw new Exception("waterML document must contain a variableParam element"); }

            //Quantity Description -- optional
            try { quantity.Description = tsr.timeSeries.variable.variableName; }
            catch { quantity.Description = ""; }

            //Quantity Variable Type -- hard coded
            quantity.ValueType = global::OpenMI.Standard.ValueType.Scalar;

            //Unit ID -- optional
            try { unit.ID = tsr.timeSeries.variable.units.unitsAbbreviation; }
            catch { unit.ID = ""; }

            //Unit Converstion Factor to SI
            //TODO WaterML does not include conversion factors to SI
            //unit.ConversionFactorToSI = 0;
            
            //Unit Converstion Offset to SI
            //TODO WaterML does not include conversion offest to SI
            //unit.OffSetToSI = 0;

            quantity.Unit = unit;

            //Dimension Powers -- optional
            //TODO WaterML does not include dimension info for units
            //Examples below ...
            //dimension.SetPower(DimensionBase.Length, 3);
            //dimension.SetPower(DimensionBase.Time, -1);

            quantity.Dimension = dimension;

            //ElementSet ID -- folder name
            elementset.ID = new DirectoryInfo(folder).Name;

            //ElementSet Description -- folder relative path
            elementset.Description = folder;

            //ElementSet ElementType -- hard coded
            elementset.ElementType = ElementType.XYPoint;

            // -------------------------------------------------------------------
            // The remaining objects require access to all files in the directory.
            // -------------------------------------------------------------------

            foreach (string fileName in files)
            {
                //load the first file in the directory as an XML document
                sr = new StreamReader(fileName);
                tsr = (TimeSeriesResponseType)xml_reader.Deserialize(sr);

                Element element = new Element();
                Vertex vertex = new Vertex();

                //Element ID -- optional
                try { element.ID = tsr.queryInfo.criteria.locationParam; }
                catch { element.ID = ""; }

                //Vertex X and Y -- optional
                //tsr.timeSeries. TODO fix this.
                //if (xml_location != null && xml_location["longitude"] != null && xml_location["latitude"] != null)
                //{
                //    vertex.x = Convert.ToDouble(xml_location["longitude"].InnerText);
                //    vertex.y = Convert.ToDouble(xml_location["latitude"].InnerText);
                //}
                //else { vertex.x = double.NaN; vertex.y = double.NaN; elementset.ElementType = ElementType.IDBased; }
                element.AddVertex(vertex);

                elementset.AddElement(element);

                //TimeHorizon -- REQUIRED
//if (_earliestInputTime == 0.0)
//               {
                    string beginDateTimeString;
                    try { beginDateTimeString = tsr.queryInfo.criteria.timeParam.beginDateTime; }
                    catch { throw new Exception("waterML document must contain a beginDateTime element"); }

                    string endDateTimeString;
                    try { endDateTimeString = tsr.queryInfo.criteria.timeParam.endDateTime; }
                    catch { throw new Exception("waterML document must contain an endDateTime element"); }

                    DateTime beginDateTime = Convert.ToDateTime(beginDateTimeString);
                    DateTime endDateTime = Convert.ToDateTime(endDateTimeString);
                    double beginDateTimeDouble = CalendarConverter.Gregorian2ModifiedJulian(beginDateTime);
                    double endDateTimeDouble = CalendarConverter.Gregorian2ModifiedJulian(endDateTime);

                    //update time horizon to be inclusive of this time horizon
                    if (_earliestInputTime == 0.0) { _earliestInputTime = beginDateTimeDouble; }
                    if (beginDateTimeDouble < _earliestInputTime) { _earliestInputTime = beginDateTimeDouble; };
                    if (endDateTimeDouble > _latestInputTime) { _latestInputTime = endDateTimeDouble; };
  //              }
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

        private IValueSet buildScalarSetFromFolder(string folder, DateTime datetime)
        {
            string[] files = Directory.GetFiles(folder);

            //create array list to store location values
            List<double> locationValues = new List<double>();

            //loop through all files in directory to create element set
            foreach (string fileName in files)
            {
                StreamReader sr = new StreamReader(fileName);

                //deserialize
                XmlSerializer xml_reader = new XmlSerializer(typeof(TimeSeriesResponseType));
                TimeSeriesResponseType tsr = (TimeSeriesResponseType)xml_reader.Deserialize(sr);
            
                //load the first file in the directory as an XML document
                DateTime dt;
                double value;
                for (int i = 0; i < Convert.ToInt32(tsr.timeSeries.values.count); ++i)
                {
                    dt = tsr.timeSeries.values.value[i].dateTime;
                    if (dt == datetime)
                    {
                        dt = tsr.timeSeries.values.value[i].dateTime;
                        value = Convert.ToDouble(tsr.timeSeries.values.value[i].Value);
                    }
                }
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(fileName);
                System.Xml.XmlNamespaceManager xmlnsManager
                    = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
                xmlnsManager.AddNamespace("wtr", "http://www.cuahsi.org/waterML/1.0/");

                //lookup the values from the waterml file
                XmlNode xml_value = xmldoc.SelectSingleNode(
                    "//wtr:values//wtr:value[@dateTime='" + datetime.ToString("s") + "']",
                    xmlnsManager);

                if (xml_value != null && xml_value.InnerText != null)
                {
                    //convert to a double.  use -999 for missing values.
                    double num;
                    if (xml_value != null) num = Convert.ToDouble(xml_value.InnerText); 
                    else num = -999; 

                    locationValues.Add(num);
                }

            }

            return new ScalarSet(locationValues.ToArray());
        }
    }
}

