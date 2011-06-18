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

namespace Oatc.OpenMI.Sdk.Backbone
{
	/// <summary>
	/// The LinkableComponent provides the OpenMI interface to the wrapped engine.
    /// <para>This is a trivial implementation of OpenMI.Standard.ILinkableComponent, refer there for further details.</para>
    /// </summary>
	public abstract class LinkableComponent:MarshalByRefObject,ILinkableComponent 
	{
		private ArrayList _acceptingLinks = new ArrayList();
		private ArrayList _providingLinks = new ArrayList();
		private ArrayList _inputExchangeItems  = new ArrayList();
		private ArrayList _outputExchangeItems = new ArrayList();
		private Hashtable _eventTable = new Hashtable();

		/// <summary>
		/// Called before computation
		/// </summary>
		public abstract void Prepare();

		/// <summary>
		/// Returns computed values
		/// </summary>
		/// <param name="time">The timestamp/timespan for which to return values</param>
		/// <param name="LinkID">The linkID describing on which link values to return</param>
		/// <returns>The computed values</returns>
		public abstract IValueSet GetValues(ITime time, string LinkID);

		/// <summary>
		/// Adds a link
		/// </summary>
		/// <param name="NewLink">The link</param>
		public virtual void AddLink (ILink NewLink) 
		{
			if (NewLink.SourceComponent==this)
			{
				for (int iNewDO = 0; iNewDO < NewLink.DataOperationsCount; iNewDO++)
				{
					IDataOperation newDataOperation = NewLink.GetDataOperation(iNewDO);
					foreach (ILink link in _providingLinks)
					{
						for (int iExistingDO = 0; iExistingDO < link.DataOperationsCount; iExistingDO++)
						{
							IDataOperation existingDataOperation = link.GetDataOperation(iExistingDO);
							if (newDataOperation == existingDataOperation)
							{
								Event warning = new Event(EventType.Warning);
								warning.Description = "DataOperation " + newDataOperation.ID + " has already been used. " +
									"It's argument values will overrule the values set previously for this operation.";
								warning.Sender = this;
								SendEvent(warning);
							}
						}
					}
				}
				_providingLinks.Add(NewLink);
			}
			if (NewLink.TargetComponent==this)
			{
				_acceptingLinks.Add(NewLink);
			}
		}

		/// <summary>
		/// Removes a link
		/// </summary>
		/// <param name="LinkID">The link ID</param>
		public virtual void RemoveLink(string LinkID)
		{
			ILink Link = GetLink (LinkID);
			if (Link != null) {
				_acceptingLinks.Remove (Link);
				_providingLinks.Remove (Link);
			}
		}

		/// <summary>
		/// Returns the accepting links
		/// </summary>
		/// <returns>The accepting links</returns>
		public virtual ILink[] GetAcceptingLinks() 
		{
			return (ILink[] ) _acceptingLinks.ToArray(typeof(ILink));
		}

		/// <summary>
		/// Returns the providing links
		/// </summary>
		/// <returns>The providing links</returns>
		public virtual ILink[] GetProvidingLinks()
		{
			return (ILink[] ) _providingLinks.ToArray(typeof(ILink));
		}

		/// <summary>
		/// Returns the number of links
		/// </summary>
		public virtual int LinkCount
		{
			get
			{
				return _acceptingLinks.Count+_providingLinks.Count;
			}
		}

		/// <summary>
		/// Gets a link
		/// </summary>
		/// <param name="LinkID">The link ID</param>
		/// <returns>The link</returns>
		public virtual ILink GetLink(string LinkID)
		{
			for (int i = 0; i < _acceptingLinks.Count; i++) 
			{
				if (((ILink)_acceptingLinks[i]).ID.Equals(LinkID)) 
				{
					return (ILink) _acceptingLinks[i];
				}
			}

			for (int i = 0; i < _providingLinks.Count; i++) 
			{
				if (((ILink)_providingLinks[i]).ID.Equals(LinkID)) 
				{
					return (ILink) _providingLinks[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Subscribes to an event
		/// </summary>
		/// <param name="Listener">The listener</param>
		/// <param name="EventType">The event type</param>
		public virtual void Subscribe(IListener Listener, EventType EventType)
		{
			if (!_eventTable.ContainsKey(EventType)) 
			{	
				_eventTable[EventType] = new ArrayList();
			}
			((ArrayList)_eventTable[EventType]).Add(Listener);
		}

		/// <summary>
		/// Unsubscribes to an event
		/// </summary>
		/// <param name="Listener">The listener</param>
		/// <param name="EventType">The event type</param>
		public virtual void UnSubscribe(IListener Listener, EventType EventType)
		{
			if (_eventTable.ContainsKey(EventType))
			{
				ArrayList list = (ArrayList) _eventTable[EventType];
				if (list.Contains(Listener))
					list.Remove(Listener);
			}
		}

		/// <summary>
		/// Sends an event
		/// </summary>
		/// <param name="theEvent">The event</param>
		public virtual void SendEvent(IEvent theEvent)
		{
			EventType EventType = theEvent.Type;

			if (_eventTable.ContainsKey(EventType))
			{
				ArrayList list = (ArrayList) _eventTable[EventType];
				foreach (IListener listener in list)
				{
					listener.OnEvent(theEvent);
				}
			}
		}

		/// <summary>
		/// Returns a published event type
		/// </summary>
		/// <param name="providedEventTypeIndex">The event index</param>
		/// <returns>The published event</returns>
		public abstract EventType GetPublishedEventType(int providedEventTypeIndex);

		/// <summary>
		/// Returns the number of published events
		/// </summary>
		/// <returns>The number of published events</returns>
		public abstract int GetPublishedEventTypeCount();

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="source">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(Object source) 
		{
			if (source is LinkableComponent) 
			{
				LinkableComponent component = (LinkableComponent) source;
				return (NullEquals(ComponentID, component.ComponentID) &&
				        NullEquals(ModelID, component.ModelID));
			}

			return base.Equals (source);
		}

		private static bool NullEquals (object obj1, object obj2) 
		{
			if ((obj1 != null) && (obj2 != null)) 
			{
				return obj1.Equals (obj2);
			}
			else 
			{
				return (obj1 == obj2);
			}
		}

		///<summary>
		/// Dispose function
		///</summary>
		public virtual void Dispose()
		{
		}

		/// <summary>
		/// Returns an input exchange item
		/// </summary>
		/// <param name="inputExchangeItemIndex">The input exchange item index</param>
		/// <returns>The input exchange item</returns>
		public virtual IInputExchangeItem GetInputExchangeItem(int inputExchangeItemIndex)
		{
			return (IInputExchangeItem) _inputExchangeItems[inputExchangeItemIndex];
		}

		/// <summary>
		/// Returns the component description
		/// </summary>
		public abstract string ComponentDescription {get;}

		/// <summary>
		/// Returns if the component is valid (contains valid data)
		/// </summary>
		/// <returns>True if the component is valid</returns>
		public abstract string Validate();

		/// <summary>
		/// Returns the number of input exchange items
		/// </summary>
		public virtual int InputExchangeItemCount
		{
			get
			{
				return _inputExchangeItems.Count;
			}
		}

		/// <summary>
		/// Returns the time horizon, which is the simulation start and stop
		/// time for this component
		/// </summary>
		public abstract ITimeSpan TimeHorizon {get;}

		/// <summary>
		/// Returns the earliest time for which input is needed
		/// </summary>
		public abstract ITimeStamp EarliestInputTime {get;}

		/// <summary>
		/// Returns the component ID
		/// </summary>
		public abstract string ComponentID {get;}


		/// <summary>
		/// Returns an output exchange item
		/// </summary>
		/// <param name="outputExchangeItemIndex">The output exchange item index</param>
		/// <returns>The output exchange item</returns>
		public virtual IOutputExchangeItem GetOutputExchangeItem(int outputExchangeItemIndex)
		{
			return (IOutputExchangeItem) _outputExchangeItems[outputExchangeItemIndex];
		}

		/// <summary>
		/// Adds an input exchange item
		/// </summary>
		/// <param name="exchangeItem">The input exchange item</param>
		public virtual void AddInputExchangeItem(IInputExchangeItem exchangeItem)
		{
			_inputExchangeItems.Add(exchangeItem);
		}

		/// <summary>
		/// Adds an output exchange item
		/// </summary>
		/// <param name="exchangeItem">The output exchange item</param>
		public virtual void AddOutputExchangeItem(IOutputExchangeItem exchangeItem)
		{
			_outputExchangeItems.Add(exchangeItem);
		}

		/// <summary>
		/// Initializes the component with the given arguments
		/// </summary>
		/// <param name="properties">The arguments</param>
		public abstract void Initialize(IArgument[] properties);

		/// <summary>
		/// The model description
		/// </summary>
		public abstract string ModelDescription {get;}

		/// <summary>
		/// The model ID
		/// </summary>
		public abstract string ModelID {get;}

		/// <summary>
		/// The number of output exchange items
		/// </summary>
		public virtual int OutputExchangeItemCount
		{
				get {return _outputExchangeItems.Count;}
		}

		/// <summary>
		/// Finish clears up allocated memory and closes files
		/// After this method is called no other methods should
		/// be called on the LinkableComponent
		/// </summary>
		public abstract void Finish();

		/// <summary>
		/// Returns a string describing this linkable component
		/// </summary>
		/// <returns>The description</returns>
		public override string ToString()
		{
			return ComponentID + " - " + ModelID;
		}

		/// <summary>
		/// Returns true if the linkable component has listeners
		/// </summary>
		/// <returns>True is linkable component has listeners</returns>
		public virtual bool HasListeners()
		{
			foreach (EventType eventType in _eventTable.Keys)
			{
				ArrayList list = (ArrayList) _eventTable[eventType];
				if (list.Count>0)
					return true;
			}
			return false;
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance</returns>
		public override int GetHashCode()
		{
			return _acceptingLinks.GetHashCode() +
					_eventTable.GetHashCode() +
					_inputExchangeItems.GetHashCode()+
					_outputExchangeItems.GetHashCode() +
					_providingLinks.GetHashCode();
		}
	}
}
 
