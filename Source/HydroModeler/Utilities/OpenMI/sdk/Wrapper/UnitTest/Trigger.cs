#region Copyright
/*
* Copyright (c) 2005,2006,2007, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion 
using System;
using System.Collections;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for Trigger.
	/// </summary>
	public class Trigger  	: ILinkableComponent
	{
		private ILink _link;
		private Oatc.OpenMI.Sdk.Buffer.SmartBuffer _resultsBuffer;
		private TimeStamp _earliestInputTime;

		public Trigger()
		{
			_resultsBuffer = new SmartBuffer();
			_earliestInputTime = new TimeStamp(0);
		}

		public SmartBuffer ResultsBuffer
		{
			get
			{
				return _resultsBuffer;
			}
		}

		public void Finish()
		{
		}

		public  ITimeStamp EarliestInputTime
		{
			get
			{
				return _earliestInputTime;
			}
		}

		public  void AddLink(ILink link)
		{
			_link = link;
		}

		public void Dispose()
		{
			// TODO:  Add Trigger.Dispose implementation
		}

		public  IValueSet GetValues(ITime time, string linkID)
		{
			// TODO:  Add Trigger.GetValues implementation
			return null;
		}

		public string ComponentDescription
		{
			get
			{
				// TODO:  Add Trigger.Description getter implementation
				return null;
			}
		}

   
		public string ComponentID
		{
			get
			{
				// TODO:  Add Trigger.ID getter implementation
				return null;
			}
		}

	
		public void Initialize(IArgument[] properties)
		{
			// TODO:  Add Trigger.Initialize implementation
		}

		public string ModelID
		{
			get
			{
				// TODO:  Add Trigger.ID getter implementation
				return "Trigger";
			}
		}

		public string ModelDescription
		{
			get
			{
				// TODO:  Add Trigger.ID getter implementation
				return null;
			}
		}

		
		public void Prepare()
		{
			// TODO:  Add Trigger.PrepareForComputation implementation
		}

		public void RemoveLink(string linkID)
		{
			// TODO:  Add Trigger.RemoveLink implementation
		}

		public   int InputExchangeItemCount
		{
			get
			{
				return 1;
			}
		}

		public   int OutputExchangeItemCount
		{
			get
			{
				return 0;
			}
		}

		public   IInputExchangeItem GetInputExchangeItem(int index)
		{
			
			// -- create a flow quanitity --
			Dimension flowDimension = new Dimension();
			flowDimension.SetPower(DimensionBase.Length,3);
			flowDimension.SetPower(DimensionBase.Time,-1);
			Unit literPrSecUnit = new Unit("LiterPrSecond",0.001,0,"Liters pr Second");
			Quantity flowQuantity = new Quantity(literPrSecUnit,"Flow","Flow",global::OpenMI.Standard.ValueType.Scalar,flowDimension);

			Element element = new Element();
			element.ID = "DummyElement";
			ElementSet elementSet = new ElementSet("Dummy ElementSet","DummyElementSet",ElementType.IDBased,new SpatialReference("no reference"));
			elementSet.AddElement(element);

			InputExchangeItem inputExchangeItem = new InputExchangeItem();
			inputExchangeItem.ElementSet = elementSet;
			inputExchangeItem.Quantity   = flowQuantity;

			return inputExchangeItem;
		}
		

		public   IOutputExchangeItem GetOutputExchangeItem(int index)
		{

			return null;
			

		}

		public   ITimeSpan TimeHorizon
		{
			get
			{
				return null;
			}
		}

		public  string Validate()
		{
			//TODO: Inplement this method correctly
			return "";
		}

		public void Run(ITime[] GetValuesTimes)
		{
			for (int i = 0; i < GetValuesTimes.Length; i++)
			{
				_resultsBuffer.AddValues(GetValuesTimes[i],(IScalarSet) _link.SourceComponent.GetValues(GetValuesTimes[i],_link.ID));
				_earliestInputTime.ModifiedJulianDay = ((ITimeStamp) GetValuesTimes[i]).ModifiedJulianDay;
			}
		}

		public void Run(ITimeStamp time)
		{
			//IScalarSet scalarSet = new ScalarSet();
			
			ScalarSet scalarSet = new ScalarSet((IScalarSet) _link.SourceComponent.GetValues(time,_link.ID));
			_earliestInputTime.ModifiedJulianDay = time.ModifiedJulianDay;
			_resultsBuffer.AddValues(time,scalarSet);
		}

		public int GetResultsCount()
		{
			return _resultsBuffer.ValuesCount;
		}

		public double GetResult(int index)
		{
			return ((ScalarSet)_resultsBuffer.GetValuesAt(_resultsBuffer.TimesCount - 1)).GetScalar(index);
		}


		#region IPublisher Members

		public void SendEvent(IEvent Event)
		{
			// TODO:  Add Trigger.SendEvent implementation
		}

		public void UnSubscribe(IListener listener, EventType eventType)
		{
			// TODO:  Add Trigger.UnSubscribe implementation
		}

		public EventType GetPublishedEventType(int providedEventTypeIndex)
		{
			// TODO:  Add Trigger.GetPublishedEventType implementation
			return EventType.Informative;
		}

		public void Subscribe(IListener listener, EventType eventType)
		{
			// TODO:  Add Trigger.Subscribe implementation
		}

		public int GetPublishedEventTypeCount()
		{
			// TODO:  Add Trigger.GetPublishedEventTypeCount implementation
			return 0;
		}

		#endregion
	}
}
