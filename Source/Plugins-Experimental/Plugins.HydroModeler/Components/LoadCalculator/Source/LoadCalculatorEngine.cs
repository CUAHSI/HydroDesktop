using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.Wrapper;
using System.Collections;
using System.Xml;
using System.Diagnostics;
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;
using SharpMap.Geometries;

namespace LoadCalculator
{

    public class LoadCalculatorLinkableEngine : LinkableRunEngine
    {
        public Dictionary<string, ILink> _links; 
        private List<IInputExchangeItem> _inputExchangeItems;
        private List<IInputExchangeItem> _inputExchangeItemsTransformed; 
        private List<IOutputExchangeItem> _outputExchangeItems;
        private double _earliestInputTime;
        private double start;
        private double end;
        private double _currentTime;
        private int _timeIncrement = 300; //in seconds (should be specifiec by the user)

        public LoadCalculatorLinkableEngine()
        {
            _links = new Dictionary<string, ILink>();
            _inputExchangeItems = new List<IInputExchangeItem>();
            _inputExchangeItemsTransformed = new List<IInputExchangeItem>();
            _outputExchangeItems = new List<IOutputExchangeItem>();
            start = -999;
            end = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(2011,01,01,00,00,00));
            _earliestInputTime = end;
            _currentTime = -999;
            _timeIncrement = 20;
        }

        public override IInputExchangeItem GetInputExchangeItem(int index)
        {
            return  _inputExchangeItems[index];
        }

        public override IOutputExchangeItem GetOutputExchangeItem(int index)
        {
            return _outputExchangeItems[index];
        }

        public override int InputExchangeItemCount
        {
            get { return _inputExchangeItems.Count; }
        }

        public override string ModelDescription
        {
            get { return "Calculates the loading by matching time series data"; }
        }

        public override string ModelID
        {
            get { return "Load Calculator"; }
        }

        public override int OutputExchangeItemCount
        {
            get { return _outputExchangeItems.Count; }
        }

        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new LoadCalculatorEngine(_timeIncrement, this.ModelID);
        }

        public override void Initialize(IArgument[] properties)
        {
            //create a new instance of the Engine Class
            //_engine = new testEngine();
            
            System.Collections.Hashtable hashtable = new Hashtable();

            int option = 0;
            string configPath = null;
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Key == "ConfigFile")
                {
                    configPath = properties[i].Value;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(configPath);
                    XmlElement root = doc.DocumentElement;
                    XmlNode timeHorizon = root.SelectSingleNode("//TimeHorizon");
                    this._timeIncrement = Convert.ToInt32(timeHorizon["TimeStepInSeconds"].InnerText);
                    doc = null;

                    option = 1;
                    break;
                }
                else if (properties[i].Key == "StartDateTime")
                {
                    this.start = CalendarConverter.Gregorian2ModifiedJulian(Convert.ToDateTime(properties[i].Value));
                    this._currentTime = this.start;
                }
                else if (properties[i].Key == "TimeStepInSeconds")
                {
                    this._timeIncrement = Convert.ToInt32(properties[i].Value);
                }
                else if(properties[i].Key.Contains("Input"))
                {
                    hashtable.Add(i.ToString() + ":" + properties[i].Key, properties[i].Value);
                }
                else if (properties[i].Key.Contains("Output"))
                { 
                    hashtable.Add(i.ToString() + ":" + properties[i].Key, properties[i].Value);
                }
            }


            //initialize the IRunEngine class
            SetEngineApiAccess();
            this._engineWasAssigned = true;
            //_engineApiAccess.Initialize(hashtable);

            if (!_engineWasAssigned)
            {
                throw new System.Exception("The Initialize method in the SmartWrapper cannot be invoked before the EngineApiAccess is assigned");
            }

            _initializeWasInvoked = true;


            //initialize this the LinkableRunEngine class
            if(option == 0)
                this.Initialize(hashtable);
            else if(option == 1)
                SetVariablesFromConfigFile(configPath);
        }

        public void Initialize(System.Collections.Hashtable properties)
        {
            Dictionary<string, List<string>> groupItems = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> OutputGroupItems = new Dictionary<string, List<string>>();

            //extract argument(s) from OMI file
            foreach (string Key in properties.Keys)
            {
                if (Key.Split(':')[1] == "InputTimeSeries")
                {
                    if (groupItems.ContainsKey(properties[Key].ToString()))
                    {
                        groupItems[properties[Key].ToString()].Add(Key.Split(':')[1] + (groupItems[properties[Key].ToString()].Count + 1).ToString());
                    }
                    else
                    {
                        List<string> l = new List<string>();
                        l.Add(Key.Split(':')[1] + "1");
                        groupItems.Add(properties[Key].ToString(), l);
                    }
                }
                else if (Key.Split(':')[1] == "OutputTimeSeries")
                {
                    if (OutputGroupItems.ContainsKey(properties[Key].ToString()))
                    {
                        OutputGroupItems[properties[Key].ToString()].Add(Key.Split(':')[1] + (groupItems[properties[Key].ToString()].Count + 1).ToString());
                    }
                    else
                    {
                        List<string> l = new List<string>();
                        l.Add(Key.Split(':')[1] + "1");
                        OutputGroupItems.Add(properties[Key].ToString(), l);
                    }


                    System.Collections.Hashtable hashtable = new Hashtable();
                    hashtable.Add("OutputExchangeItem", properties[Key]);
                    
                    //pass this info to the IRunEngine, via Intitialize
                    _engineApiAccess.Initialize(hashtable);
                }
            }

            //create input and output exchange items from the groups defined above.
            int group = 0;
            foreach (KeyValuePair<string, List<string>> item in groupItems)
            {
                group++;
                for (int i = 0; i <= item.Value.Count - 1; i++)
                {
                    string seriesID = item.Value[i];
                    string[] details = item.Key.ToString().Split(':');

                    Unit u = new Unit();
                    u.Description = details[1];
                    u.ID = details[1];

                    Quantity q = new Quantity();
                    q.Description = details[0];
                    q.ID = details[0];
                    q.ValueType = OpenMI.Standard.ValueType.Scalar;
                    q.Unit = u;

                    ElementSet eset = new ElementSet();
                    eset.Description = seriesID;
                    eset.ID = seriesID;
                    eset.ElementType = ElementType.IDBased;

                    InputExchangeItem input = new InputExchangeItem();
                    input.Quantity = q;
                    input.ElementSet = eset;
                    _inputExchangeItems.Add(input);
                }
            }

            foreach (KeyValuePair<string, List<string>> item in OutputGroupItems)
            {
                for (int i = 0; i <= item.Value.Count - 1; i++)
                {
                    string seriesID = item.Value[i];
                    string[] details = item.Key.ToString().Split(':');

                    Unit u = new Unit();
                    u.Description = details[2];
                    u.ID = details[2];

                    Quantity q = new Quantity();
                    q.Description = details[1];
                    q.ID = details[1];
                    q.ValueType = OpenMI.Standard.ValueType.Scalar;
                    q.Unit = u;

                    ElementSet eset = new ElementSet();
                    eset.Description = details[0];
                    eset.ID = details[0];
                    eset.ElementType = ElementType.IDBased;
                    try
                    {
                        int elementCount = Convert.ToInt32(details[3].Split('=')[1]);
                        for (int k = 0; k <= elementCount - 1; k++)
                        {
                            Element e = new Element();
                            eset.AddElement(e);
                        }
                    }
                    catch (Exception) { }

                    OutputExchangeItem input = new OutputExchangeItem();
                    input.Quantity = q;
                    input.ElementSet = eset;
                    _outputExchangeItems.Add(input);
                }
            }
        }

        public override ITimeSpan TimeHorizon
        {
            get { return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(start), new TimeStamp(end)); }
        }

        public override void AddLink(ILink link)
        {
            base.AddLink(link);

            //save the link
            //

            //get the id of the target element set
            string id = link.TargetElementSet.ID;

            //get the corresponding input and output exchange item index
            int inIndex = 0; int outIndex = 0;
            for (int i = 0; i <= _inputExchangeItems.Count - 1; i++)
            {
                if (_inputExchangeItems[i].ElementSet.ID == id)
                {
                    inIndex = i; break;
                }
            }
            for (int i = 0; i <= _outputExchangeItems.Count - 1; i++)
            {
                if (_outputExchangeItems[i].ElementSet.ID == id)
                {
                    outIndex = i; break;
                }
            }

            //update input and output exchange item elementsets and quantities
            //Unit u = new Unit();
            
            //u.Description = link.SourceQuantity.Unit.Description;
            //u.ID = link.SourceQuantity.Unit.ID;

            Quantity q = new Quantity();
            q = (Quantity)link.SourceQuantity;
            //q.Description = link.SourceQuantity.Description;
            //q.ID = link.SourceQuantity.ID;
            //q.ValueType = link.SourceQuantity.ValueType;
            //q.Unit = link.SourceQuantity.Unit;

            ElementSet eset = new ElementSet();
            //eset.Description = link.SourceElementSet.Description;
            //eset.ID = link.SourceElementSet.ID;
            //eset.ElementType = link.SourceElementSet.ElementType;
            eset = (ElementSet)link.SourceElementSet;

            //HACK:  Right now this assumes there is only one outputexchangeitem!
            //HACK:  Also assumes that the output elementset will contain the same elements as the element set of the first link
            //check to see if the output exchange item has been defined yet
            if (_outputExchangeItems[0].ElementSet.GetElementID(0) == "Empty")
            {
                OutputExchangeItem Output = (OutputExchangeItem)_outputExchangeItems[0];
                //Quantity OutputQ = (Quantity)Output.Quantity;
                ElementSet OutputE = (ElementSet)Output.ElementSet;
                OutputE.ElementType = eset.ElementType;
                
                //int min = 100000;
                //int index = 0;
                //for (int o = 0; o <= _inputExchangeItems.Count - 1; o++)
                //    if (_inputExchangeItems[o].ElementSet != OutputE)
                //        if (_inputExchangeItems[o].ElementSet.ElementCount < min)
                //        {
                //            min = _inputExchangeItems[o].ElementSet.ElementCount;
                //            index = o;
                //        }


                Element e = OutputE.GetElement(0);
                e.ID = eset.GetElementID(0);
                Vertex v = new Vertex();
                e.Vertices[0].x = eset.GetXCoordinate(0, 0);
                e.Vertices[0].y = eset.GetYCoordinate(0, 0);
                //e.AddVertex(v);

                for (int o = 1; o <= eset.Elements.Length - 1; o++)
                {
                    e = new Element(eset.GetElementID(o));
                    v = new Vertex();
                    v.x = eset.GetXCoordinate(o, 0);
                    v.y = eset.GetYCoordinate(o, 0);
                    e.AddVertex(v);
                    OutputE.AddElement(e);
                }

                Output.ElementSet = OutputE;
                //_outputExchangeItems.Add(output);

            }

            //generate an output exchange item based on the input its getting
            OutputExchangeItem output = new OutputExchangeItem();
            output.Quantity = q;
            output.ElementSet = eset;
            if (!_outputExchangeItems.Contains(output))
            {
                eset.Description = "Time Matched: " + output.Quantity.ID;
                output.ElementSet = eset;
                _outputExchangeItems.Add(output);
            }

            //create a link to reflect the info we're getting from the source component
            Link l = new Link();
            l.ID = link.ID;
            l.SourceComponent = link.SourceComponent;
            l.SourceElementSet = link.SourceElementSet;
            l.SourceQuantity = link.SourceQuantity;
            l.TargetComponent = link.TargetComponent;
            l.TargetElementSet = eset;
            l.TargetQuantity = q;

            //get the earliest time (this will determine a start time based on source component, unless one is provided in the omi)
            if(this.start == -999)
                if (_currentTime < l.SourceComponent.TimeHorizon.Start.ModifiedJulianDay)
                    _currentTime = l.SourceComponent.TimeHorizon.Start.ModifiedJulianDay;

            //add updated link instead of the placeholder one
            _links.Add(link.ID, l);

            //Subscribe to events


            System.Collections.Hashtable hashtable = new Hashtable();
            if (link.SourceComponent != this)
            {
                ////get the exchange item info
                //string value = link.SourceQuantity.ID + "," + link.SourceElementSet.ID;
                //hashtable.Add("OutputExchangeItem", value);

                string value = link.TargetQuantity.ID + "," + link.TargetElementSet.ID;
                hashtable.Add("InputExchangeItem", value);

                //add the time horizon info
                hashtable.Add("TimeHorizon",link.SourceComponent.TimeHorizon);

                //add the link info
                hashtable.Add("Link", link);

                //pass this info to the IRunEngine, via Intitialize
                _engineApiAccess.Initialize(hashtable);

                //set the time horizon for the LinkableRunEngine
                Oatc.OpenMI.Sdk.Backbone.TimeSpan timehorizon = (Oatc.OpenMI.Sdk.Backbone.TimeSpan)link.SourceComponent.TimeHorizon;
                if (timehorizon.Start.ModifiedJulianDay > start)
                    start = timehorizon.Start.ModifiedJulianDay;
                if (timehorizon.End.ModifiedJulianDay < end)
                    end = timehorizon.End.ModifiedJulianDay;
            }
            else
            {
                //get the exchange item info
                string value = link.SourceQuantity.ID + "," + link.SourceElementSet.ID;
                hashtable.Add("OutputExchangeItem", value);

                //add the link info
                hashtable.Add("Link", link);

                //pass this info to the IRunEngine, via Intitialize
                _engineApiAccess.Initialize(hashtable);

                //lc = link.TargetComponent;
            }


            
        }

        public override void RemoveLink(string LinkID)
        {
            base.RemoveLink(LinkID);

            Link link = (Link)_links[LinkID];
            Quantity q = new Quantity();
            q = (Quantity)link.SourceQuantity;


            ElementSet eset = new ElementSet();
            eset = (ElementSet)link.SourceElementSet;

            InputExchangeItem input = new InputExchangeItem();
            input.Quantity = q;
            input.ElementSet = eset;

            //generate an output exchange item based on the input its getting
            OutputExchangeItem output = new OutputExchangeItem();
            output.Quantity = q;
            output.ElementSet = eset;

            //remove link
            if (_outputExchangeItems.Contains(output))
                _outputExchangeItems.Remove(output);

            if (_inputExchangeItems.Contains(input))
                _inputExchangeItems.Remove(input);

            _links.Remove(LinkID);
        }
        public override IValueSet GetValues(ITime time, string LinkID)
        {
            Link l = (Link)_links[LinkID];
            if (l.TargetComponent.ModelID != "Oatc.OpenMI.Gui.Trigger")
            {
                string quantity = l.SourceQuantity.ID;
                string elementset = l.SourceElementSet.ID;

                ScalarSet ss = (ScalarSet)EngineApiAccess.GetValues(quantity, elementset);

                return ss;
            }
            else
            {
                return base.GetValues(time, LinkID);
            }
        }

        /// <summary>
        /// Reads the Configuration file, and creates OpenMI exchange items 
        /// </summary>
        /// <param name="configFile">path pointing to the components comfiguration (XML) file</param>
        public void SetVariablesFromConfigFile(string configFile)
        {
            //Read config file
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlElement root = doc.DocumentElement;
            XmlNodeList outputExchangeItems = root.SelectNodes("//OutputExchangeItem");

            foreach (XmlNode outputExchangeItem in outputExchangeItems)
            {
                OutputExchangeItem o = (OutputExchangeItem)CreateExchangeItemsFromXMLNode(outputExchangeItem, "OutputExchangeItem");
                _outputExchangeItems.Add(o);
                string Key = o.ElementSet.ID + ":" + o.Quantity.ID;
                System.Collections.Hashtable hashtable = new Hashtable();
                hashtable.Add("OutputExchangeItem", Key);

                //pass this info to the IRunEngine, via Intitialize
                _engineApiAccess.Initialize(hashtable);
            }

            XmlNodeList inputExchangeItems = root.SelectNodes("//InputExchangeItem");
            foreach (XmlNode inputExchangeItem in inputExchangeItems)
            {
                InputExchangeItem i = (InputExchangeItem) CreateExchangeItemsFromXMLNode(inputExchangeItem, "InputExchangeItem");
                _inputExchangeItems.Add(i);
            }

            XmlNode timeHorizon = root.SelectSingleNode("//TimeHorizon");
            this.start = CalendarConverter.Gregorian2ModifiedJulian(Convert.ToDateTime(timeHorizon["StartDateTime"].InnerText));
        }

        private IExchangeItem CreateExchangeItemsFromXMLNode(XmlNode ExchangeItem, string Identifier)
        {
            XmlNodeList dimensions = null;
            //get the Dimensions node by iterating through some nodes.
            foreach (XmlNode child in ExchangeItem.ChildNodes)
            {
                if(child.Name == "Quantity")
                {
                    foreach (XmlNode node in child.ChildNodes)
                    {
                        if(node.Name == "Dimensions")
                            dimensions = node.ChildNodes;
                    }
                }
            }
            
            //Create Dimensions
            Dimension omiDimensions = new Dimension();
            
            //XmlNodeList dimensions = ExchangeItem.SelectNodes("//Dimensions/Dimension"); // You can filter elements here using XPath
            foreach (XmlNode dimension in dimensions)
            {
                if (dimension["Base"].InnerText == "Length")
                {
                    omiDimensions.SetPower(DimensionBase.Length, Convert.ToDouble(dimension["Power"].InnerText));
                }
                else if (dimension["Base"].InnerText == "Time")
                {
                    omiDimensions.SetPower(DimensionBase.Time, Convert.ToDouble(dimension["Power"].InnerText));
                }
                else if (dimension["Base"].InnerText == "AmountOfSubstance")
                {
                    omiDimensions.SetPower(DimensionBase.AmountOfSubstance, Convert.ToDouble(dimension["Power"].InnerText));
                }
                else if (dimension["Base"].InnerText == "Currency")
                {
                    omiDimensions.SetPower(DimensionBase.Currency, Convert.ToDouble(dimension["Power"].InnerText));
                }
                else if (dimension["Base"].InnerText == "ElectricCurrent")
                {
                    omiDimensions.SetPower(DimensionBase.ElectricCurrent, Convert.ToDouble(dimension["Power"].InnerText));
                }
                else if (dimension["Base"].InnerText == "LuminousIntensity")
                {
                    omiDimensions.SetPower(DimensionBase.LuminousIntensity, Convert.ToDouble(dimension["Power"].InnerText));
                }
                else if (dimension["Base"].InnerText == "Mass")
                {
                    omiDimensions.SetPower(DimensionBase.Mass, Convert.ToDouble(dimension["Power"].InnerText));
                }
                else if (dimension["Base"].InnerText == "Temperature")
                {
                    omiDimensions.SetPower(DimensionBase.Temperature, Convert.ToDouble(dimension["Power"].InnerText));
                }
            }

            //Create Units
            Unit _omiUnits = new Unit();
            XmlNode units = ExchangeItem.SelectSingleNode("Quantity/Unit");
            _omiUnits.ID = units["ID"].InnerText;
            if (units["Description"] != null) _omiUnits.Description = units["Description"].InnerText;
            if (units["ConversionFactorToSI"] != null) _omiUnits.ConversionFactorToSI = Convert.ToDouble(units["ConversionFactorToSI"].InnerText);
            if (units["OffSetToSI"] != null) _omiUnits.OffSetToSI = Convert.ToDouble(units["OffSetToSI"].InnerText);

            //Create Quantity
            Quantity omiQuantity = new Quantity();
            XmlNode quantity = ExchangeItem.SelectSingleNode("Quantity");
            omiQuantity.ID = quantity["ID"].InnerText;
            if (quantity["Description"] != null) omiQuantity.Description = quantity["Description"].InnerText;
            omiQuantity.Dimension = omiDimensions;
            omiQuantity.Unit = _omiUnits;
            if (quantity["ValueType"] != null)
            {
                if (quantity["ValueType"].InnerText == "Scalar")
                {
                    omiQuantity.ValueType = OpenMI.Standard.ValueType.Scalar;
                }
                else if (quantity["ValueType"].InnerText == "Vector")
                {
                    omiQuantity.ValueType = OpenMI.Standard.ValueType.Vector;
                }
            }

            //Create Element Set
            ElementSet omiElementSet = new ElementSet();
            XmlNode elementSet = ExchangeItem.SelectSingleNode("ElementSet");
            omiElementSet.ID = elementSet["ID"].InnerText;
            if (elementSet["Description"] != null) omiElementSet.Description = elementSet["Description"].InnerText;

            try
            {
                //add elements from shapefile to element set
                string _shapefilepath = elementSet["ShapefilePath"].InnerText;
                omiElementSet = AddElementsFromShapefile(omiElementSet, _shapefilepath);

            }
            catch (Exception)
            {
                try
                {
                    //add elements from shapefile to element set
                    int numElements = Convert.ToInt32(elementSet["NumberOfElements"].InnerText);
                    omiElementSet.ElementType = ElementType.IDBased;
                    for (int i = 0; i <= numElements-1; i++)
                    {
                        Element e = new Element();
                        Vertex v = new Vertex();
                        e.AddVertex(v);
                        omiElementSet.AddElement(e);
                    }
                }
                catch (Exception)
                { 
                    Debug.WriteLine("An Element Set has not been declared using AddElementsFromShapefile");

                    //make sure that all output exchange items have at least 1 element
                    if (Identifier == "OutputExchangeItem")
                    {
                        //create at least one element
                        omiElementSet.ElementType = ElementType.IDBased;

                        Element e = new Element("Empty");
                        Vertex v = new Vertex();
                        e.AddVertex(v);
                        omiElementSet.AddElement(e);
                    }
                }
                
            }


            if (Identifier == "OutputExchangeItem")
            {
                //create exchange item
                OutputExchangeItem omiOutputExchangeItem = new OutputExchangeItem();
                omiOutputExchangeItem.Quantity = omiQuantity;
                omiOutputExchangeItem.ElementSet = omiElementSet;

                return omiOutputExchangeItem;
                
                //add the output exchange item to the list of output exchange items for the component
                //this._outputs.Add(omiOutputExchangeItem);
                //if (!this._quantities.ContainsKey(omiQuantity.ID)) this._quantities.Add(omiQuantity.ID, omiQuantity);
                //if (!this._elementSets.ContainsKey(omiElementSet.ID)) this._elementSets.Add(omiElementSet.ID, omiElementSet);
            }
            else if (Identifier == "InputExchangeItem")
            {
                //create exchange item
                InputExchangeItem omiInputExchangeItem = new InputExchangeItem();
                omiInputExchangeItem.Quantity = omiQuantity;
                omiInputExchangeItem.ElementSet = omiElementSet;

                return omiInputExchangeItem;

                //add the output exchange item to the list of output exchange items for the component
                //this._inputs.Add(omiInputExchangeItem);
                //if (!this._quantities.ContainsKey(omiQuantity.ID)) this._quantities.Add(omiQuantity.ID, omiQuantity);
                //if (!this._elementSets.ContainsKey(omiElementSet.ID)) this._elementSets.Add(omiElementSet.ID, omiElementSet);
            }
            else
            {
                throw new Exception(" \"" + Identifier + "\" is not a valid exchange item identifier");
            }
            
        }
        public ElementSet AddElementsFromShapefile(ElementSet omiElementSet, string shapefilePath)
        {
            //this uses the free SharpMap API for reading a shapefile
            VectorLayer myLayer = new VectorLayer("elements_layer");
            myLayer.DataSource = new ShapeFile(shapefilePath);
            myLayer.DataSource.Open();

            //set spatial reference from shapefile
            SpatialReference sprf = new SpatialReference();
            sprf.ID = myLayer.DataSource.SRID.ToString();
            omiElementSet.SpatialReference = sprf;

            //add elements to elementset from shapefile
            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); ++i)
            {

                FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
                string GeometryType = Convert.ToString(
                    feat.Geometry.AsText().Substring(
                    0, feat.Geometry.AsText().IndexOf(' ')));

                Element e = new Element();

                if (feat.Table.Columns.IndexOf("HydroCode") != -1)
                    e.ID = feat.ItemArray[feat.Table.Columns.IndexOf("HydroCode")].ToString();

                if (GeometryType == "POINT")
                {
                    omiElementSet.ElementType = ElementType.XYPoint;
                    Point p = (Point)feat.Geometry;
                    Vertex v = new Vertex();
                    v.x = p.X;
                    v.y = p.Y;
                    e.AddVertex(v);
                }
                if (GeometryType == "POLYGON")
                {
                    omiElementSet.ElementType = ElementType.XYPolygon;
                    Polygon p = (Polygon)feat.Geometry;
                    LinearRing lr = p.ExteriorRing;

                    //Only loop until lr.Vertices.Count-2 b/c the first element is the same
                    // as the last element within the exterior ring.  This will thrown an error
                    // within the OATC element mapper, when trying to map elements.  Also this
                    // loop arranges the vertices of the exterior ring in counter clockwise order
                    // as needed for the element mapping.
                    for (int j = lr.Vertices.Count - 2; j >= 0; j--)
                    {
                        Vertex v = new Vertex();
                        v.x = lr.Vertices[j].X;
                        v.y = lr.Vertices[j].Y;
                        e.AddVertex(v);
                    }
                }
                if (GeometryType == "LINESTRING")
                {
                    omiElementSet.ElementType = ElementType.XYPolyLine;
                    LineString ls = (LineString)feat.Geometry;
                    //Point endpt = ls.EndPoint;
                    //Point startpt = ls.StartPoint;
                    for (int j = 0; j < ls.Vertices.Count; j++)
                    {
                        Vertex v = new Vertex();
                        v.x = ls.Vertices[j].X;
                        v.y = ls.Vertices[j].Y;
                        e.AddVertex(v);
                    }

                }
                omiElementSet.AddElement(e);
            }
            return omiElementSet;
        }

    }


    class LoadCalculatorEngine : IRunEngine
    {
        //private Dictionary<string, ILink> _links; 
        private Dictionary<string, List<string>> _inputExchangeItems;
        private Dictionary<string, List<string>> _outputExchangeItems;
        private Dictionary<string, ScalarSet> _values;
        private Dictionary<string, Link> _links;
        private System.Collections.Hashtable _properties;
        private double _earliestInputTime;
        private double start;
        private double end;
        private double _currentTime;
        private int _timeIncrement; //in seconds (should be specifiec by the user)
        private string _modelID;
        private List<string> _results = new List<string>();

        public LoadCalculatorEngine(int timeIncrement, string modelID)
        {
            _inputExchangeItems = new Dictionary<string, List<string>>();
            _outputExchangeItems = new Dictionary<string, List<string>>();
            _values = new Dictionary<string, ScalarSet>();
            _links = new Dictionary<string, Link>();

            start = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(1900,01,01,00,00,00));
            end = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(2011,01,01,00,00,00));
            _earliestInputTime = end;
            _currentTime = -999;
            this._timeIncrement = timeIncrement;
            this._modelID = modelID;
        }


        #region IRunEngine Members

        public void Dispose()
        {
        }
        public void Finish()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("./LoadCalculatorOutput.csv");
            for (int i = 0; i <= _results.Count - 1; i++)
            {
                sw.WriteLine(_results[i]);
            }
            sw.Close();
        }
        public string GetComponentDescription()
        {
                return "ComponentDescription: Test";
        }
        public string GetComponentID()
        {
                return "ComponentID: Test";
        }
        public ITime GetCurrentTime()
        {
            if (_currentTime == -999)
            {
                _currentTime = start;
            }
            return new TimeStamp(_currentTime);
        }
        public ITimeStamp GetEarliestNeededTime()
        {
            return new TimeStamp(this.start);
        }
        public ITime GetInputTime(string QuantityID, string ElementSetID)
        {
            return new TimeStamp(this._currentTime);
        }
        public double GetMissingValueDefinition()
        {
            return -999;
        }
        public void Initialize(System.Collections.Hashtable properties)
        {
            //check if the input arg are and Input or Output Exchange Item
            if (properties.ContainsKey("InputExchangeItem"))
            {
                //get the input quantity and element set ids
                string value = properties["InputExchangeItem"].ToString();
                //save them
                if(_inputExchangeItems.ContainsKey(value.Split(',')[0]))
                    _inputExchangeItems[value.Split(',')[0]].Add( value.Split(',')[1] );
                else
                    _inputExchangeItems.Add(value.Split(',')[0], new List<string>(){value.Split(',')[1]});

                ////get the output quantity and element set ids
                //value = properties["OutputExchangeItem"].ToString();
                ////save them
                //_outputExchangeItems.Add["OutputExchangeItem

                //get the time horizon info
                Oatc.OpenMI.Sdk.Backbone.TimeSpan timehorizon = (Oatc.OpenMI.Sdk.Backbone.TimeSpan)properties["TimeHorizon"];
                //adjust the start and end times
                if (timehorizon.Start.ModifiedJulianDay > start)
                    start = timehorizon.Start.ModifiedJulianDay;
                if (timehorizon.End.ModifiedJulianDay < end)
                    end = timehorizon.End.ModifiedJulianDay;


                Link l = (Link)properties["Link"];
                //get the link info
                _links.Add(l.ID, l);

                ////get the link info
                //_links.Add(value.Split(',')[0] + ":" + value.Split(',')[1], (Link)properties["Link"]);

            }
            else
            {
                //get the quantity and element set ids
                string value = properties["OutputExchangeItem"].ToString();

                if (value.Contains(':')) //output defined in omi or config file
                {
                    
                    string elementSet = value.Split(':')[0];
                    string quantity = value.Split(':')[1];
                    //save them
                    if (_outputExchangeItems.ContainsKey(quantity))
                        _outputExchangeItems[quantity].Add(elementSet);
                    else
                        _outputExchangeItems.Add(quantity, new List<string>() { elementSet });


                }
                else //ouput defined by links
                {
                    //save them
                    if (_outputExchangeItems.ContainsKey(value.Split(',')[0]))
                        _outputExchangeItems[value.Split(',')[0]].Add(value.Split(',')[1]);
                    else
                        _outputExchangeItems.Add(value.Split(',')[0], new List<string>() { value.Split(',')[1] });

                    Link l = (Link)properties["Link"];
                    //get the link info
                    _links.Add(l.ID, l);
                }



            }
        }
        public IValueSet GetValues(string QuantityID, string ElementSetID)
        {
            string key = QuantityID + ":" + ElementSetID;
            //check to see if value exists
            if (_values.ContainsKey(key))
            {
                return _values[key];
            }
            else
            {
                //if no value exists, then return nothing
                return new ScalarSet(new double[0]);
            }
        }
        public bool PerformTimeStep()
        {
            //request values for all input exchange items
            List<ScalarSet> invals = new List<ScalarSet>();
            List<string> inIds = new List<string>();
            Dictionary<string, ScalarSet> invalues = new Dictionary<string, ScalarSet>();
            List<Link> links = new List<Link>();

            bool setvals = true;
            foreach (KeyValuePair<string, Link> link in _links)
            {
                Link l = link.Value;

                //get data from all incoming links
                if (l.TargetComponent.ModelID == this._modelID)
                {

                    string quantity = l.SourceQuantity.ID;
                    string elementset = l.SourceElementSet.ID;
                    
                    ScalarSet ss = (ScalarSet)this.GetValues(l.TargetQuantity.ID, l.TargetElementSet.ID);

                    if (invalues.ContainsKey(quantity+":"+elementset))
                    {
                        invalues[quantity + ":" + elementset] = ss;
                    }
                    else
                    {
                        invalues.Add(quantity + ":" + elementset, ss);
                    }
                    invals.Add(ss);
                    links.Add(l);

                    if (ss.data.Length == 0)
                        setvals = false;
                }
            }
            string outstring = null;
            //Perform Loading Calculation
            if (setvals)
            {
                outstring = this._currentTime.ToString() + ",";
                double[] loading = new double[invals[0].data.Length];
                //loop through the lesser of invals[0].data.length and invals[1].data.length
                int max = 0;
                for (int i = 0; i <= invals.Count - 1; i++)
                    if (invals[i].data.Length > max)
                        max = invals[i].data.Length;

                for (int i = 0; i <= max - 1; i++)
                {
                    try
                    {
                        //string id1 = links[0].SourceElementSet.GetElementID(i);
                        //int id2 = links[1].SourceElementSet.GetElementIndex(id1);

                        loading[i] = invals[0].data[i] * invals[1].data[i] * 86400;

                        //check to see if either input value was unknown (i.e. zero)
                        if (loading[i] == 0)
                            loading[i] = -1;

                        //HACK: Convert from m3/s to ft3/s
                        //loading[i] *= 0.02831685;

                        outstring += loading[i].ToString() + ",";
                    }
                    catch (IndexOutOfRangeException) { }
                }

                //set output values
                foreach (KeyValuePair<string, Link> link in _links)
                {
                    Link l = link.Value;
                    if (l.SourceComponent.ModelID == this._modelID)
                    {
                        string quantity = l.SourceQuantity.ID;
                        string elementset = l.SourceElementSet.ID;
                        string out_quantity = l.TargetQuantity.ID;
                        string out_elementset = l.TargetElementSet.ID;

                        //if (_inputExchangeItems.ContainsKey(quantity))
                        if (invalues.ContainsKey(quantity + ":" + elementset))
                        {
                            try
                            {
                                this.SetValues(quantity, elementset, invalues[quantity + ":" + elementset]);
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            this.SetValues(quantity, elementset, new ScalarSet(loading));
                        }
                    }

                }
            }
            else
            {
                foreach (KeyValuePair<string, Link> link in _links)
                {
                    Link l = link.Value;
                    if (l.SourceComponent.ModelID == this._modelID)
                    {
                        string quantity = l.SourceQuantity.ID;
                        string elementset = l.SourceElementSet.ID;

                        this.SetValues(quantity, elementset, new ScalarSet(new double[0])); //set null
                    }
                }
            }

            //save the results locally
            if (outstring!=null)
                this._results.Add(outstring);

            //advance time
            DateTime newTime = CalendarConverter.ModifiedJulian2Gregorian(_currentTime).AddSeconds(_timeIncrement);
            this._currentTime = CalendarConverter.Gregorian2ModifiedJulian(newTime);

            return true;
        }
        public void SetValues(string QuantityID, string ElementSetID, IValueSet values)
        {
            string key = QuantityID + ":" + ElementSetID;

            //check to see if value exists
            if (_values.ContainsKey(key))
            {
                //replace existing value
                _values[key] = (ScalarSet)values;
            }
            else
            {
                //create a new entry
                _values.Add(key, (ScalarSet)values);
            }
        }

        #endregion


    }

}
