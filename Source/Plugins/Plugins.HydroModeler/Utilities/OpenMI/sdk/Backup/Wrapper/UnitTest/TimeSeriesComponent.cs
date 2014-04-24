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
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.DevelopmentSupport;


namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for TimeSeriesComponent.
	/// </summary>
	public class TimeSeriesComponent : Oatc.OpenMI.Sdk.Backbone.LinkableComponent
	{
		Oatc.OpenMI.Sdk.Buffer.SmartBuffer buffer = new SmartBuffer();

		public TimeSeriesComponent()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	
		public override IOutputExchangeItem GetOutputExchangeItem(int outputExchangeItemIndex)
		{
			Quantity quantity = new Quantity(new Unit("literprSecond",0.001,0,"lprsec"),"flow","flow", global::OpenMI.Standard.ValueType.Scalar,new Dimension());
			ElementSet elementSet = new ElementSet("oo","ID",ElementType.IDBased,new SpatialReference("no"));
			Element element = new Element("ElementID");
			elementSet.AddElement(element);
			OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
			outputExchangeItem.ElementSet = elementSet;
			outputExchangeItem.Quantity   = quantity;

			return outputExchangeItem;
		}
	
		public override IValueSet GetValues(ITime time, string LinkID)
		{
			return buffer.GetValues(time);
		}
	
		public override string ComponentDescription
		{
			get
			{
				return "ComponentDescription";
			}
		}
	
		public override string ComponentID
		{
			get
			{
				return "TimeSeriesComponentID";
			}
		}
	
		public override ITimeStamp EarliestInputTime
		{
			get
			{
				return null;
			}
		}
	
		public override int InputExchangeItemCount
		{
			get
			{
				return 0;
			}
		}
	
		public override string ModelDescription
		{
			get
			{
				return "TimeSeriesCompnent used for testing";
			}
		}
	
		public override string ModelID
		{
			get
			{
				return "TimeSeriesComponentID";
			}
		}
	
		public override int OutputExchangeItemCount
		{
			get
			{
				return 1;
			}
		}
	
		public override ITimeSpan TimeHorizon
		{
			get
			{
				TimeStamp start = new TimeStamp( ((ITimeSpan)(buffer.GetTimeAt(0))).Start );
				TimeStamp end   = new TimeStamp( ((ITimeSpan)(buffer.GetTimeAt(buffer.TimesCount - 1))).End);
				return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(start, end);
			}
		}
	
		public override EventType GetPublishedEventType(int providedEventTypeIndex)
		{
				return EventType.Other;
		}
	
		public override int GetPublishedEventTypeCount()
		{
			return 0;
		}
	
		public override void Initialize(IArgument[] properties)
		{
			double start = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(2004,12,31,0,0,0));

			for (int i = 0; i < 30; i++)
			{
				buffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(start + i),new TimeStamp(start + i + 1)), new ScalarSet(new double[] {(double)i}));
			}
		}
	
		public override string Validate()
		{
			return null;
		}
	
		public override void Finish()
		{

		}
	
		public override void Prepare()
		{

		}
	}
}
