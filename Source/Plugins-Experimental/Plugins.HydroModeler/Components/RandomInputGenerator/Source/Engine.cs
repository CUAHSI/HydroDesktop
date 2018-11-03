#region MetaData
//Developed By: Anthony Castronova
//Last Edited: 02/01/2010

//----- Component Description -----
//This component generates a set of random numbers and passes them to other components.  This is used for
//perfomance analysis in which an arbitrary set of input values is sufficient for the model simulation.

//----- Additional Notes -----
//This component has only been developed to supply values to one other component within a model configuration.
//Additional development should focus on making this more general.

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMW;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using System.Data;

namespace RandomInputGenerator
{
    public class InputGenerator : ILinkableComponent
    {


        private int _numElements = 0;

        private Dictionary<string, ILink> _links = new Dictionary<string, ILink>();
        private List<IInputExchangeItem> _inputExchangeItems = new List<IInputExchangeItem>();
        private List<IOutputExchangeItem> _outputExchangeItems = new List<IOutputExchangeItem>();
        private DataSet outputVals = new DataSet();
        public DataTable ExchangeItemInfo = new DataTable();
        private DataTable _values = new DataTable();

        private double _earliestInputTime;
        private double _latestInputTime;

        #region ILinkableComponent Members

        public void Finish()
        {
        }
        public void AddLink(ILink link)
        {
            _links.Add(link.ID, link);

            //create new table in dataset
            outputVals.Tables.Add(link.ID);

            //create columns in outputvals table to save values
            outputVals.Tables[link.ID].Columns.Add("Time", typeof(ITime));
            outputVals.Tables[link.ID].Columns.Add("Value", typeof(IValueSet));
            outputVals.Tables[link.ID].Columns.Add("Quantity", typeof(string));
            outputVals.Tables[link.ID].Columns.Add("ElementSet", typeof(string));

            OutputExchangeItem output = new OutputExchangeItem();
            output.ElementSet = link.TargetElementSet;
            output.Quantity = link.TargetQuantity;

            _numElements = output.ElementSet.ElementCount;
        }


        public string ComponentDescription
        {
            get { return "RandomInputGenerator 1.0"; }
        }

        public string ComponentID
        {
            get { return "RandomInputGenerator"; }
        }

        public void Dispose()
        {

        }

        public ITimeStamp EarliestInputTime
        {
            get { return new TimeStamp(_earliestInputTime); }
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
            Random random = new Random();

            double[] output = new double[_numElements];
            for (int i = 0; i < _numElements; i++)
                output[i] = random.Next(20);

            IValueSet vals = new ScalarSet(output);
            return vals;
        }

        public void Initialize(IArgument[] properties)
        {
            //get the number of elements from the omi file
            //int _numElements = -1;
            //foreach (Argument property in properties)
            //{
            //    if(property.Key == "ElementCount")
            //        _numElements = Convert.ToInt32(property.Value);
            //}

            //if (_numElements == -1)
            //    throw new Exception("Invalid ElementCount, please fix in *.omi file");

            //create output exchange item (so that this component can be linked to others)
            Quantity quantity = new Quantity();
            Oatc.OpenMI.Sdk.Backbone.Unit unit = new Oatc.OpenMI.Sdk.Backbone.Unit();
            Dimension dimension = new Dimension();
            ElementSet elementset = new ElementSet();
            OutputExchangeItem outputexchangeitem = new OutputExchangeItem();

            quantity.ID = "Input Generator";
            quantity.Description = "Supplies random numbers as input values";
            quantity.ValueType = global::OpenMI.Standard.ValueType.Scalar;
            unit.ID = "any units";

            quantity.Unit = unit;
            quantity.Dimension = dimension;
            elementset.ID = "any elementset";
            elementset.Description = "AllSites";

            elementset.ElementType = ElementType.IDBased;

            string beginDateTimeString = "1/1/1900 12:00:00 AM";
            string endDateTimeString = "1/1/2500 12:00:00 AM";

            DateTime beginDateTime = Convert.ToDateTime(beginDateTimeString);
            DateTime endDateTime = Convert.ToDateTime(endDateTimeString);
            _earliestInputTime = CalendarConverter.Gregorian2ModifiedJulian(beginDateTime);
            _latestInputTime = CalendarConverter.Gregorian2ModifiedJulian(endDateTime);


            outputexchangeitem.Quantity = quantity;
            outputexchangeitem.ElementSet = elementset;
            if (outputexchangeitem != null) { _outputExchangeItems.Add(outputexchangeitem); }

        }

        public int InputExchangeItemCount
        {
            get { return _inputExchangeItems.Count; }
        }

        public string ModelDescription
        {
            get { return "RandomInputGenerator"; }
        }

        public string ModelID
        {
            get { return "RandomInputGenerator"; }
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
            return EventType.TimeStepProgres;
        }

        public int GetPublishedEventTypeCount()
        {
            return InputExchangeItemCount;
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

        //#region IListener Members

        //public EventType GetAcceptedEventType(int acceptedEventTypeIndex)
        //{
        //    switch (acceptedEventTypeIndex)
        //    {
        //        case 0:
        //            return EventType.DataChanged;
        //        case 1:
        //            return EventType.TargetBeforeGetValuesCall;
        //        case 2:
        //            return EventType.SourceAfterGetValuesCall;
        //        case 3:
        //            return EventType.TargetBeforeGetValuesCall;
        //        case 4:
        //            return EventType.TargetAfterGetValuesReturn;
        //        case 5:
        //            return EventType.Informative;
        //        default:
        //            throw new Exception("Iligal index in GetPublishedEventType()");
        //    }
        //}

        //public int GetAcceptedEventTypeCount()
        //{
        //    return 6;
        //}

        //public void OnEvent(IEvent anEvent)
        //{
        //    if (anEvent.Type == EventType.TargetBeforeGetValuesCall)
        //    {
        //        //set quantity and element set
        //        _quantity = anEvent.Sender.GetInputExchangeItem(0).Quantity.Description;
        //        _elementset = anEvent.Sender.GetInputExchangeItem(0).ElementSet.ID;
        //    }
        //}

        //#endregion
    }
}
