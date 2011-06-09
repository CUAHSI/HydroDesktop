using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Diagnostics;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;
//using Oatc.OpenMI.Gui.Core;
using System.Data;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Geometries;


namespace SMW
{
    public abstract class Wrapper : Oatc.OpenMI.Sdk.Wrapper.IEngine
    {

        #region Global Objects
        private string _componentID = "Simple_Model_Component";
        private string _componentDescription = "Simple Model Component"; 
        private string _modelID;
        private string _modelDescription;
        private List<InputExchangeItem> _inputs = new List<InputExchangeItem>();
        private List<OutputExchangeItem> _outputs = new List<OutputExchangeItem>();
        private double _simulationStartTime;
        private double _simulationEndTime;
        private double _currentTime;
        private double _timeStep;
        private string _shapefilepath;
        private Dictionary<string,Quantity> _quantities = new Dictionary<string, Quantity>();
        private Dictionary<string,ElementSet> _elementSets = new Dictionary<string,ElementSet>();
        private Dictionary<string, double[]> _vals = new Dictionary<string, double[]>();

        private string _path;





        private DataTable _values = new DataTable();
        Unit _omiUnits;
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Used to Initialize the component.  Performs routines that must be completed prior to simulation start.
        /// </summary>
        /// <param name="properties">properties extracted from the components *.omi file</param>
        public abstract void Initialize(System.Collections.Hashtable properties);
        public abstract bool PerformTimeStep();
        public abstract void Finish();
        #endregion

        public string PATH
        {
            get { return _path; }
        } 
        public string GetComponentID()
        {
            return _componentID;
        }
        public string GetComponentDescription()
        {
            return _modelDescription;
        }
        public string GetModelID()
        {
            return _modelID;
        }
        public string GetModelDescription()
        {
            return _modelDescription;
        }
        public InputExchangeItem GetInputExchangeItem(int exchangeItemIndex)
        {
            return _inputs[exchangeItemIndex];
        }
        public OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex)
        {
            return _outputs[exchangeItemIndex];
        }
        public int GetInputExchangeItemCount()
        {
            if (_inputs == null) return 0;
            else return _inputs.Count;
        }
        public int GetOutputExchangeItemCount()
        {
            if (_outputs == null) return 0;
            else return _outputs.Count;
        }
        public ITimeSpan GetTimeHorizon()
        {
            return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(
                new TimeStamp(_simulationStartTime), new TimeStamp(_simulationEndTime));
        }
        public void Dispose()
        {
        }

        public ITime GetCurrentTime()
        {
            if (_currentTime == 0.0) 
                _currentTime = _simulationStartTime;
            return new TimeStamp(_currentTime);
        }
        public ITimeStamp GetEarliestNeededTime()
        {
            return (ITimeStamp)this.GetCurrentTime();
        }
        public ITime GetInputTime(string QuantityID, string ElementSetID)
        {
            //This method returns the requested input time to the ILinkableEngine class
            return this.GetCurrentTime();
        }

        public double GetMissingValueDefinition()
        {
            return -999;
        }

        /// <summary>
        /// This method is used to extract values from an upstream component.
        /// </summary>
        /// <param name="QuantityID">The input Quantity ID</param>
        /// <param name="ElementSetID">The input Element Set ID</param>
        /// <returns>the values saved under the matching QuantityID and ElementSetID, from an upstream component</returns>
        public IValueSet GetValues(string QuantityID, string ElementSetID)
        {
            string key = QuantityID + "_" + ElementSetID;
            if (_vals.ContainsKey(key))
                return new ScalarSet(_vals[key]);
            else if (_elementSets.ContainsKey(ElementSetID))
                return new ScalarSet(new double[_elementSets[ElementSetID].ElementCount]);
            else
                return new ScalarSet(new double[_outputs[0].ElementSet.ElementCount]);
        }

        public void SetValues(string QuantityID, string ElementSetID, IValueSet values)
        {
            string key = QuantityID + "_" + ElementSetID;
            if (_vals.ContainsKey(key))
                _vals[key] = ((ScalarSet)values).data;
            else
                _vals.Add(key,((ScalarSet)values).data);
        }

        #region Auxilary Methods 

        /// <summary>
        /// This method will advance the components in time, by a single timestep.  
        /// </summary>
        /// <remarks>
        /// This should be called at the end of Perform Time Step.
        /// </remarks>
        public void AdvanceTime()
        {
            TimeStamp ct = (TimeStamp)GetCurrentTime();
            _currentTime = ct.ModifiedJulianDay + _timeStep / 86400.0;
        }

        /// <summary>
        /// Reads the Configuration file, and creates OpenMI exchange items 
        /// </summary>
        /// <param name="configFile">path pointing to the components comfiguration (XML) file</param>
        public void SetVariablesFromConfigFile(string configFile)
        {
            //set output path variable
            _path = Path.GetDirectoryName(configFile) + "\\";
            
            //Read config file
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);

            XmlElement root = doc.DocumentElement;

            XmlNode ID = root.SelectSingleNode("ModelInfo//ID");
            _modelID = ID.InnerText;

            XmlNode Desc = root.SelectSingleNode("ModelInfo//Description");
            _componentDescription = Desc.InnerText;

            XmlNodeList outputExchangeItems = root.SelectNodes("//OutputExchangeItem"); 
            foreach (XmlNode outputExchangeItem in outputExchangeItems)
            {
                CreateExchangeItemsFromXMLNode(outputExchangeItem, "OutputExchangeItem");
            }

            XmlNodeList inputExchangeItems = root.SelectNodes("//InputExchangeItem");
            foreach (XmlNode inputExchangeItem in inputExchangeItems)
            {
                CreateExchangeItemsFromXMLNode(inputExchangeItem, "InputExchangeItem");
            }

            XmlNode timeHorizon = root.SelectSingleNode("//TimeHorizon");
            
            //Set IEngine properties
            this._simulationStartTime = CalendarConverter.Gregorian2ModifiedJulian(Convert.ToDateTime(timeHorizon["StartDateTime"].InnerText));
            this._simulationEndTime = CalendarConverter.Gregorian2ModifiedJulian(Convert.ToDateTime(timeHorizon["EndDateTime"].InnerText));
            this._timeStep = Convert.ToDouble(timeHorizon["TimeStepInSeconds"].InnerText);
        }

        private void CreateExchangeItemsFromXMLNode(XmlNode ExchangeItem, string Identifier)
        {
            //Create Dimensions
            Dimension omiDimensions = new Dimension();
            XmlNodeList dimensions = ExchangeItem.SelectNodes("//Dimensions/Dimension"); // You can filter elements here using XPath
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
            _omiUnits = new Unit();
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
                SMW.Utilities utils = new SMW.Utilities();
                _shapefilepath = elementSet["ShapefilePath"].InnerText;
                omiElementSet = utils.AddElementsFromShapefile(omiElementSet, _shapefilepath);

            }
            catch (Exception)
            {
                Debug.WriteLine("An Element Set has not been declared using AddElementsFromShapefile"); 
            }



            if (Identifier == "OutputExchangeItem")
            {
                //create exchange item
                OutputExchangeItem omiOutputExchangeItem = new OutputExchangeItem();
                omiOutputExchangeItem.Quantity = omiQuantity;
                omiOutputExchangeItem.ElementSet = omiElementSet;

                //add the output exchange item to the list of output exchange items for the component
                this._outputs.Add(omiOutputExchangeItem);
                if (!this._quantities.ContainsKey(omiQuantity.ID)) this._quantities.Add(omiQuantity.ID, omiQuantity);
                if (!this._elementSets.ContainsKey(omiElementSet.ID)) this._elementSets.Add(omiElementSet.ID, omiElementSet);
            }
            else if (Identifier == "InputExchangeItem")
            {
                //create exchange item
                InputExchangeItem omiInputExchangeItem = new InputExchangeItem();
                omiInputExchangeItem.Quantity = omiQuantity;
                omiInputExchangeItem.ElementSet = omiElementSet;


                //add the output exchange item to the list of output exchange items for the component
                this._inputs.Add(omiInputExchangeItem);
                if (!this._quantities.ContainsKey(omiQuantity.ID)) this._quantities.Add(omiQuantity.ID, omiQuantity);
                if (!this._elementSets.ContainsKey(omiElementSet.ID)) this._elementSets.Add(omiElementSet.ID, omiElementSet);
            }
        }

        /// <summary>
        /// Adds columns to hold the components input and output the SMW's global data structure to 
        /// </summary>
        public void SetValuesTableFields()
        {
            _values.Columns.Add("QuantityID", typeof(string));
            _values.Columns.Add("ElementSetID", typeof(string));
            _values.Columns.Add("ValueSet", typeof(IValueSet));

        }

        #endregion


        /// <summary>
        /// Gets the model timestep (constant value)
        /// </summary>
        /// <returns>Modified Julian DateTime</returns>
        public double GetTimeStep()
        {
            return _timeStep;
        }
        /// <summary>
        /// Use to get the shapefile path stored in config.xml
        /// </summary>
        /// <returns>the absolute path to the elementset shapefile</returns>
        public string GetShapefilePath()
        {
            return _shapefilepath;
        }
        /// <summary>
        /// Gets the unit value that the component is implemented over
        /// </summary>
        /// <returns>unitID from config.xml</returns>
        public string GetUnits()
        {
            return _omiUnits.ID;
        }

        public List<OutputExchangeItem> Outputs
        {
            get { return _outputs; }
            set { _outputs = value; }
        }
        public List<InputExchangeItem> Inputs
        {
            get { return _inputs; }
            set { _inputs = value; }
        }
        
    }

}
