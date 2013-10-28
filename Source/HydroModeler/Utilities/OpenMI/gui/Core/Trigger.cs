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
using Oatc.OpenMI.Sdk.DevelopmentSupport;

namespace Oatc.OpenMI.Gui.Core
{

	/// <summary>
	/// Dummy exchange item used by trigger's link.
	/// </summary>
	public class TriggerExchangeItem: IInputExchangeItem
	{
		IQuantity quantity;
		IElementSet elementSet;

		/// <summary>
		/// Gets dummy quantity.
		/// </summary>
		public IQuantity Quantity
		{
			get
			{
				return quantity;
			}
		}

		/// <summary>
		/// Gets dummy element-set..
		/// </summary>
		public IElementSet ElementSet
		{
			get
			{
				return elementSet;
			}
		}


		/// <summary>
		/// Creates new instance of <see cref="TriggerExchangeItem">TriggerExchangeItem</see>
		/// </summary>
		public TriggerExchangeItem()
		{
			quantity =  new Oatc.OpenMI.Sdk.Backbone.Quantity(new Oatc.OpenMI.Sdk.Backbone.Unit("Dummy",1,0,"dummy"),"Anything","TriggerQuantityID",global::OpenMI.Standard.ValueType.Scalar,new Oatc.OpenMI.Sdk.Backbone.Dimension());
			elementSet = new Oatc.OpenMI.Sdk.Backbone.ElementSet("Dummy Element","TriggerElementID",ElementType.IDBased,new Oatc.OpenMI.Sdk.Backbone.SpatialReference());
		}
	}


	/// <summary>
	/// Linkable component which can hold only one input link. It's used to fire the simulation.
	/// </summary>
	public class Trigger : ILinkableComponent
	{
		ILink _link;
		TriggerExchangeItem _inputExchangeItem;
		Oatc.OpenMI.Sdk.Backbone.TimeSpan _timeHorizon;
		TimeStamp _earliestInputTime;

		/// <summary>
		/// Creates a new instance of <see cref="Trigger">Trigger</see> class.
		/// </summary>
		public Trigger()
		{
			_link = null;
			_inputExchangeItem = new TriggerExchangeItem();

			Oatc.OpenMI.Sdk.Backbone.TimeStamp
				start = new TimeStamp( CalendarConverter.Gregorian2ModifiedJulian(new DateTime(1800,1,1)) ),
				end = new TimeStamp( CalendarConverter.Gregorian2ModifiedJulian(new DateTime(2200,1,1)) );
			_timeHorizon = new Oatc.OpenMI.Sdk.Backbone.TimeSpan(start, end);

			_earliestInputTime = end;
		}

	

		/// <summary>
		/// Gets input exchange item.
		/// </summary>
		/// <param name="inputExchangeItemIndex">Index of input exchange item.</param>
		/// <returns>If <c>inputExchangeItemIndex</c> is <c>0</c>, returns
		/// the only one available input exchnage item, otherwise returns <c>null</c>.</returns>
		public IInputExchangeItem GetInputExchangeItem(int inputExchangeItemIndex)
		{
			if( inputExchangeItemIndex == 0 )
				return( _inputExchangeItem );
			else
				return( null );
		}

		/// <summary>
		/// Gets number of input exchange items, i.e. <c>1</c>.
		/// </summary>
		public int InputExchangeItemCount
		{
			get
			{
				return 1;
			}
		}

		
		/// <summary>
		/// Default implementation.
		/// </summary>
		/// <param name="outputExchangeItemIndex"></param>
		/// <returns>Returns <c>null</c>.</returns>
		public IOutputExchangeItem GetOutputExchangeItem(int outputExchangeItemIndex)
		{			
			return null;
		}

		/// <summary>
		/// Gets output exchange items count, i.e. <c>0</c>.
		/// </summary>
		public int OutputExchangeItemCount
		{
			get
			{
				return 0;
			}
		}

		
		/// <summary>
		/// Stores input link which is used to trigger the simulation.
		/// </summary>
		/// <param name="link">Input link.</param>
		/// <remarks>Trigger can have only one input link, if you call this method
		/// more than once, only the last link is used to trigger the simulation.</remarks>
		public void AddLink(ILink link)
		{
			_link = link;
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		public void Dispose()
		{			
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="linkID"></param>
		/// <returns>Returns <c>null</c>.</returns>
		public IValueSet GetValues(ITime time, string linkID)
		{
			return null;
		}

		
		/// <summary>
		/// Default implementation
		/// </summary>
		public void Initialize( IArgument[] arguments )
		{			
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		public void Prepare()
		{			
		}

		/// <summary>
		/// Removes the one link from trigger if IDs are corresponding.
		/// </summary>
		/// <param name="linkID">Link's ID.</param>
		public void RemoveLink(string linkID)
		{
			if( _link!=null )
				if( _link.ID==linkID )
					_link = null;
		}

		/// <summary>
		/// Gets description of trigger.
		/// </summary>
		public string ComponentDescription
		{
			get
			{
				return( "Component implementing trigger model." );
			}
		}

		/// <summary>
		/// Preforms validation of the <see cref="Trigger">Trigger</see> model.
		/// </summary>
		/// <returns>Returns empty string.</returns>
		public string Validate()
		{
			if( _link == null )
				return( "Warning: No model is linked to trigger, simulation won't be fired." );
			else
                return( "" );
		}

		/// <summary>
		/// Gets trigger's time horizon.
		/// </summary>
		public ITimeSpan TimeHorizon
		{
			get
			{
				return( _timeHorizon );
			}
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		public void Finish()
		{			
		}

		/// <summary>
		/// Gets earliest time when next input is needed, typically the trigger invoke time.
		/// </summary>
		public ITimeStamp EarliestInputTime
		{
			get
			{
				return( _earliestInputTime );
			}
		}

		/// <summary>
		/// Gets this component's ID, i.e. <see cref="CompositionManager.TriggerModelID">CompositionManager.TriggerModelID</see> constant.
		/// </summary>
		public string ComponentID
		{
			get
			{				
				return( CompositionManager.TriggerModelID );
			}
		}

		/// <summary>
		/// Gets model description.
		/// </summary>
		public string ModelDescription
		{
			get
			{
				return( "This model is used to trigger whole simulation. It only once invokes GetValues() on one linked model." );
			}
		}

		/// <summary>
		/// Gets this model's ID, i.e. <see cref="CompositionManager.TriggerModelID">CompositionManager.TriggerModelID</see> constant.
		/// </summary>
		public string ModelID
		{
			get
			{
				return( CompositionManager.TriggerModelID );
			}
		}
				

		/// <summary>
		/// Default implementation
		/// </summary>
		/// <param name="Event"></param>
		public void SendEvent(IEvent Event)
		{			
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		/// <returns>Returns <c>0</c>.</returns>
		public int GetPublishedEventTypeCount()
		{
			return 0;
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		/// <param name="listener"></param>
		/// <param name="eventType"></param>
		public void UnSubscribe(IListener listener, EventType eventType)
		{
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		/// <param name="providedEventTypeIndex"></param>
		/// <returns>Returns <see cref="EventType.Other">EventType.Other</see>.</returns>
		public EventType GetPublishedEventType(int providedEventTypeIndex)
		{
			return( EventType.Other );
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		/// <param name="listener"></param>
		/// <param name="eventType"></param>
		public void Subscribe(IListener listener, EventType eventType)
		{
		}
		

		/// <summary>
		/// Invokes <see cref="ILinkableComponent.GetValues">ILinkableComponent.GetValues</see>
		/// method on model linked to this trigger, if any.
		/// </summary>
		/// <param name="runToTime">Time for <see cref="ILinkableComponent.GetValues">ILinkableComponent.GetValues</see> call.</param>
		public void Run(Oatc.OpenMI.Sdk.Backbone.TimeStamp runToTime)
		{
			if( _link!=null )
			{
				_earliestInputTime = runToTime;
				_link.SourceComponent.GetValues(runToTime,_link.ID);
			}
		}
	
		
	}



}

