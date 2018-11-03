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
using System.Runtime.Remoting; 
using OpenMI.Standard;  
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;

 
namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
    /// The LinkableRunEngine  implements the run time part of the ILinkableComponent interface. 
    /// The remaining methods are implemented in the derived LinkableEngine class. There are 
    /// historical reasons for splitting the functionality between the two classes. 
    /// The LinkableRunEngine class and the LinkableEngine class could be merged, 
    /// but for the time being these are keeps as they are in order to support backward compatibility. 
	/// </summary>
	[Serializable]
	public  abstract class LinkableRunEngine : LinkableComponent
	{

		//TODO: The elementset version number should be checked and the elementmapper 
		//called in order to update the mapping a matrix when the version has changed

        /// <summary>
        /// List of SmartInputLinks
        /// </summary>
        protected ArrayList          _smartInputLinks;

        /// <summary>
        /// List of SmartOutput Links
        /// </summary>
        protected ArrayList          _smartOutputLinks;

		/// <summary>
		/// Reference to the engine. Must be assigned in the derived class
		/// </summary>
        protected IRunEngine         _engineApiAccess;

		/// <summary>
		/// True if the _engineApiAccess was assigned
		/// </summary>
        protected bool               _engineWasAssigned;
		
        /// <summary>
        /// True if the Initialize method was invoked
        /// </summary>
        protected bool	             _initializeWasInvoked;

		/// <summary>
		/// True if the Prepare method was invoked
		/// </summary>
        protected bool               _prepareForCompotationWasInvoked;

        /// <summary>
        /// True if the component is gathering data from other LinkableComponents
        /// </summary>
		protected bool               _isBusy;
		
        /// <summary>
        /// Arraylist of published event types
        /// </summary>
        protected ArrayList          _publishedEventTypes;

		/// <summary>
		/// used when comparing time in the IsLater method (see property TimeEpsilon)
		/// </summary>
        protected double             _timeEpsilon; // used when comparing time in the IsLater method (see property TimeEpsilon)

		/// <summary>
		/// Current validation string from the Validate method
		/// </summary>
        protected ArrayList          _validationWarningMessages;

        /// <summary>
        /// The current validateion error message
        /// </summary>
		protected ArrayList          _validationErrorMessages;

	
		/// <summary>
		/// Constructor method for the LinkableRunEngine class
		/// </summary>
		public LinkableRunEngine()
		{
			_engineWasAssigned                = false;
			_initializeWasInvoked             = false;
			_prepareForCompotationWasInvoked  = false;
            _timeEpsilon                      = 0.10 * 1.0 / (3600.0 * 24.0);


			_publishedEventTypes = new ArrayList();
			_publishedEventTypes.Add(EventType.DataChanged);
			_publishedEventTypes.Add(EventType.Informative);
			_publishedEventTypes.Add(EventType.SourceAfterGetValuesCall);
			_publishedEventTypes.Add(EventType.SourceBeforeGetValuesReturn);
			_publishedEventTypes.Add(EventType.TargetAfterGetValuesReturn);
			_publishedEventTypes.Add(EventType.TargetBeforeGetValuesCall);

			_validationWarningMessages = new ArrayList();
			_validationErrorMessages   = new ArrayList();

            _smartInputLinks  = new ArrayList();
            _smartOutputLinks = new ArrayList();
			
		}

		/// <summary>
		/// Implementation of the same method in the 
		/// OpenMI.Standard.ILinkableComponent interface
		/// </summary>
		public override ITimeStamp EarliestInputTime
		{
			get
			{
				return (_engineApiAccess.GetEarliestNeededTime());
			}
		}

		/// <summary>
		/// This _timeEpsilon variable is used when comparing the current time in the engine with
		/// the time specified in the parameters for the GetValue method. 
		/// if ( requestedTime > engineTime + _timeEpsilon) then PerformTimestep()..
		/// The default values for _timeEpsilon is double.Epsilon = 4.94065645841247E-324
		/// The default value may be too small for some engines, in which case the _timeEpsilon can
		/// be changed the class that you have inherited from LinkableRunEngine og LinkableEngine.
		/// </summary>
		public double TimeEpsilon
		{
			get
			{
				return _timeEpsilon;
			}
			set
			{
				_timeEpsilon = value;
			}
		}

		/// <summary>
		/// Add a link to the LinkableComponent
		/// </summary>
		/// <param name="newLink">The Link</param>
        public override void AddLink(ILink newLink)
		{
			try
			{
				if (!_initializeWasInvoked)
				{
					throw new System.Exception("AddLink method in the SmartWrapper cannot be invoked before the Initialize method has been invoked");
				}
				if (_prepareForCompotationWasInvoked)
				{
					throw new System.Exception("AddLink method in the SmartWrapper cannot be invoked after the PrepareForComputation method has been invoked");
				}

				if(newLink.TargetComponent == this)
				{
                    _smartInputLinks.Add (this.CreateInputLink(this._engineApiAccess, newLink));
				}
				else if(newLink.SourceComponent == this)
				{
                    this._smartOutputLinks.Add (this.CreateOutputLink(this._engineApiAccess, newLink));
				}
				else
				{
					throw new System.Exception("SourceComponent.ID or TargetComponent.ID in Link does not match the Component ID for the component to which the Link was added");
				}
			}
			catch (System.Exception e)
			{
				string message = "Exception in LinkableComponent. ";
				message += "ComponentID: " + this.ComponentID + "\n";
				throw new System.Exception(message,e);
			}
		}

		/// <summary>
		/// Creates a new input link 
		/// </summary>
		/// <param name="engine">The engine</param>
		/// <param name="link">The link</param>
		/// <returns>The new input link</returns>
		public virtual SmartInputLink CreateInputLink(IRunEngine engine, ILink link) 
		{
			return new SmartInputLink (engine, link);
		}

		/// <summary>
		/// Creates a new output link 
		/// </summary>
		/// <param name="engine">The engine</param>
		/// <param name="link">The link</param>
		/// <returns>The new output link</returns>
		public virtual SmartOutputLink CreateOutputLink(IRunEngine engine, ILink link) 
		{
            SmartOutputLink smartOutputLink = new SmartOutputLink (engine, link);
            smartOutputLink.Initialize();
            return smartOutputLink;
		}

		/// <summary>
		/// Implementaion of the same method in the
		/// OpenMI.Standard.ILinkableComponent
		/// </summary>
		public override void Dispose()
		{
			_engineApiAccess.Dispose();
		}

		/// <summary>
		/// Implementation of the same method in
		/// OpenMI.Standard.ILInkableComponent
		/// </summary>
		/// <param name="time">Time (ITimeSpan or ITimeStamp) for which values are requested</param>
		/// <param name="LinkID">LinkID associated to the requested values</param>
		/// <returns>The values</returns>
		public override IValueSet GetValues(ITime time, string LinkID)
		{
			try
			{
				CheckTimeArgumentInGetvaluesMethod(time);
				SendSourceAfterGetValuesCallEvent(time, LinkID);
				IValueSet engineResult = new ScalarSet();
 
                int outputLinkIndex = -999;
                for (int i = 0; i < _smartOutputLinks.Count; i++)
                {
                    if ( ((SmartOutputLink) _smartOutputLinks[i]).link.ID == LinkID)
                    {
                        outputLinkIndex = i;
                        break;
                    }
                }
			
				if (_isBusy==false)
				{
					//while(IsLater(time,_engineApiAccess.GetCurrentTime()))
                    while(IsLater(time, ((SmartOutputLink) _smartOutputLinks[outputLinkIndex]).GetLastBufferedTime()))
					{
						_isBusy=true;

						//Update input links
                        foreach(SmartInputLink smartInputLink in _smartInputLinks)
                        {
                            smartInputLink.UpdateInput();
                        }
						_isBusy=false; 

						//Perform Timestep
						if(_engineApiAccess.PerformTimeStep()) 
						{
							//Update buffer with engine values, Time is timestamp
                            foreach (SmartOutputLink smartOutputLink in _smartOutputLinks)
                            {
                                smartOutputLink.UpdateBuffer();
                            }

							SendEvent(EventType.DataChanged);
						}
					}
				}

                engineResult = ((SmartOutputLink)_smartOutputLinks[outputLinkIndex]).GetValue(time);
    
				SendEvent(EventType.SourceBeforeGetValuesReturn);
				return engineResult;
		
			}
			catch (System.Exception e)
			{
				string message = "Exception in LinkableComponent. ComponentID: ";
				message += this.ComponentID;
				throw new System.Exception(message,e);
			}
		}

	
		/// <summary>
		/// Description of the component
		/// </summary>
        public override string ComponentDescription
		{
			get
			{
				return _engineApiAccess.GetComponentDescription();
			}
		}


		/// <summary>
		/// ID for the component
		/// </summary>
        public override string ComponentID
		{
			get
			{
				if (_engineApiAccess != null) 
				{
					return _engineApiAccess.GetComponentID();
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Finish
		/// </summary>
        public override void Finish()
		{
			_engineApiAccess.Finish();

		}

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="properties">Initialization parameters</param>
        public override void Initialize(IArgument[] properties)
		{
			System.Collections.Hashtable hashtable =new Hashtable();
			for(int i = 0; i < properties.Length;i++)
			{
				hashtable.Add(properties[i].Key,properties[i].Value);
			}

			SetEngineApiAccess();
			this._engineWasAssigned = true;
			_engineApiAccess.Initialize(hashtable);
			
			if (!_engineWasAssigned)
			{
				throw new System.Exception("The Initialize method in the SmartWrapper cannot be invoked before the EngineApiAccess is assigned" );
			}

			_initializeWasInvoked = true;
		}

		/// <summary>
		/// Prepare. This method will be invoked after end of configuration and before the first GetValues call
		/// </summary>
        public override void Prepare()
		{
			try
			{
				if (!_engineWasAssigned)
				{
					throw new System.Exception("PrepareForComputation method in SmartWrapper cannot be invoked before the EngineApiAccess has been assigned");
				}

				if (!_initializeWasInvoked)
				{
					throw new System.Exception("PrepareForComputation method in SmartWrapper cannot be invoked before the Initialize method has been invoked");
				}

				Validate();

				if (_validationErrorMessages.Count > 0)
				{
					string errorMessage = "";
					foreach (string str in _validationErrorMessages)
					{
						errorMessage += "Error: " + str + ". ";
					}

					throw new Exception(errorMessage);

				}

                foreach (SmartOutputLink smartOutputLink in _smartOutputLinks)
                {
                    smartOutputLink.UpdateBuffer();
                }

				_prepareForCompotationWasInvoked = true;
			}
			catch (System.Exception e)
			{
				string message = "Exception in LinkableComponent. ";
				message += "ComponentID: " + this.ComponentID + "\n";
				throw new System.Exception(message,e);
			}
		}

		/// <summary>
		/// Remove a link
		/// </summary>
		/// <param name="LinkID">Link ID for the link to be removed</param>
        public override void RemoveLink(string LinkID)
		{
			try
			{
				if (!_initializeWasInvoked)
				{
					throw new Exception("Illegal invocation of RemoveLink method before invocation of Initialize method");
				}

				if (_prepareForCompotationWasInvoked)
				{
					throw new Exception("Illegal invocation of RemoveLink method after invocation of Prepare method");
				}

  
                int index = -999;
                for (int i = 0; i < _smartInputLinks.Count; i++)
                {
                    if (((SmartInputLink)_smartInputLinks[i]).link.ID == LinkID)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -999)
                {
                    _smartInputLinks.RemoveAt(index);
                }
                else
                {
                    for (int i = 0; i < _smartOutputLinks.Count; i++)
                    {
                        if(((SmartOutputLink) _smartOutputLinks[i]).link.ID == LinkID)
                        {
                            index = i;
                            break;
                        }
                    }
                    _smartOutputLinks.RemoveAt(index);
                }

                if (index == -999)
                {
                    throw new Exception("Failed to find link.ID in internal link lists in method RemoveLink()");
                }
               
    		}
			catch (System.Exception e)
			{
				string message = "Exception in LinkableComponent. ";
				message += "ComponentID: " + this.ComponentID + "\n";
				throw new System.Exception(message,e);
			}
		}

		/// <summary>
		/// Returns an array of input ILink which contains links already added to this component.
		/// </summary>
		/// <returns>Returns an array of ILink which contains links already added to this component</returns>
        public override ILink[] GetAcceptingLinks()
		{
			ArrayList links = new ArrayList();

			foreach (SmartInputLink smartLink in _smartInputLinks) 
			{
				links.Add (smartLink.link);
			}

			return (ILink[]) links.ToArray(typeof(ILink));
		}

		/// <summary>
		/// Returns an array of output ILink which contains links already added to this component.
		/// </summary>
		/// <returns>Returns an array of output ILink which contains links already added to this component.</returns>
        public override ILink[] GetProvidingLinks()
		{
			ArrayList links = new ArrayList();

			foreach (SmartOutputLink smartLink in _smartOutputLinks) 
			{
				links.Add (smartLink.link);
			}

			return (ILink[]) links.ToArray(typeof(ILink));
		}


		/// <summary>
		/// Get the reference to the engine
		/// </summary>
        public IRunEngine EngineApiAccess
		{
			get
			{
				return _engineApiAccess;
			}
		}

        /// <summary>
        /// Set reference to the engine
        /// </summary>
        protected abstract void SetEngineApiAccess();
		
		/// <summary>
		/// Keep Curren state
		/// </summary>
		/// <returns>ID for the state keept</returns>
        public virtual string KeepCurrentState()
		{
			if (_engineApiAccess is IManageState)
			{
				string stateID;
				stateID = ((IManageState) _engineApiAccess).KeepCurrentState();
				foreach (SmartOutputLink smartOutputLink in _smartOutputLinks)
				{
					smartOutputLink.KeepCurrentBufferState(stateID);
				}
				return stateID;
			}
			else
			{
				throw new Exception("KeepCurrentState was called but the engine does not implement IManageState");
			}
		}

		/// <summary>
		/// Restore a state
		/// </summary>
		/// <param name="stateID">ID for the state to restore</param>
        public virtual void RestoreState(string stateID)
		{
			if (_engineApiAccess is IManageState)
			{
				((IManageState) _engineApiAccess).RestoreState(stateID);

				foreach (SmartOutputLink smartOutputLink in _smartOutputLinks)
				{
					smartOutputLink.RestoreBufferState(stateID);
				}
			}
			else
			{
				throw new Exception("RestoreState was called but the engine does not implement IManageState");
			}
			
		}

		/// <summary>
		/// Clear a state
		/// </summary>
		/// <param name="stateID">ID for the state to clear</param>
        public virtual void ClearState(string stateID)
		{
			if (_engineApiAccess is IManageState)
			{
				((IManageState) _engineApiAccess).ClearState(stateID);
				
				foreach (SmartOutputLink smartOutputLink in _smartOutputLinks)
				{
					smartOutputLink.ClearBufferState(stateID);
				}
			}
			else
			{
				throw new Exception("ClearState was called but the engine does not implement IManageState");
			}
		}

		
        /// <summary>
        /// Get the published event types.
        /// </summary>
        /// <param name="providedEventTypeIndex">index for the requested event type</param>
        /// <returns>the requested event type</returns>
        public override EventType GetPublishedEventType(int providedEventTypeIndex)
		{
			return (EventType) _publishedEventTypes[providedEventTypeIndex];
		}

		/// <summary>
		/// Get the number of published event types
		/// </summary>
		/// <returns>Number of published event types</returns>
        public override int GetPublishedEventTypeCount()
		{
			return _publishedEventTypes.Count;
		}

		/// <summary>
		/// Convert a ITime object to a ITimeStamp.
		/// </summary>
		/// <param name="time">The ITime object to convert</param>
		/// <returns>The converted time</returns>
        public static Oatc.OpenMI.Sdk.Backbone.TimeStamp TimeToTimeStamp(ITime time)
		{
			Oatc.OpenMI.Sdk.Backbone.TimeStamp t;

			if (time is ITimeStamp)
			{
				t = new Oatc.OpenMI.Sdk.Backbone.TimeStamp(((ITimeStamp) time).ModifiedJulianDay);
			}
			else
			{
				t = new Oatc.OpenMI.Sdk.Backbone.TimeStamp(((ITimeSpan) time).End.ModifiedJulianDay);
			}

			return t;
		}

		/// <summary>
		/// Will compare two times. If the first argument t1, is later than the second argument t2
		/// the method will return true. Otherwise false will be returned. t1 and t2 can be of types
		/// ITimeSpan or ITimeStamp.
		/// </summary>
		/// <param name="t1">First time</param>
		/// <param name="t2">Second time</param>
		/// <returns>isLater</returns>
		protected bool IsLater(ITime t1, ITime t2)
		{
			double mt1, mt2;
			bool isLater = false;

			mt1 = TimeToTimeStamp(t1).ModifiedJulianDay;
			mt2 = TimeToTimeStamp(t2).ModifiedJulianDay;

			if (mt1 > mt2 + _timeEpsilon)
			{
				isLater = true;
			}
			else
			{
				isLater = false;
			}

			return isLater;

		}
		
        /// <summary>
        /// Converts a ITime object to a formatted string
        /// </summary>
        /// <param name="time">The time to convert</param>
        /// <returns>The formatted string</returns>
        public static string ITimeToString(ITime time)
		{
			string timeString;

			if (time is ITimeStamp)
			{
				timeString = (CalendarConverter.ModifiedJulian2Gregorian(((ITimeStamp) time).ModifiedJulianDay)).ToString();
			}
			else if (time is ITimeSpan)
			{
				timeString = "[" + (CalendarConverter.ModifiedJulian2Gregorian(((ITimeSpan) time).Start.ModifiedJulianDay)).ToString() + ", " +  (CalendarConverter.ModifiedJulian2Gregorian(((ITimeSpan) time).End.ModifiedJulianDay)).ToString() + "]";
			}
			else
			{
				throw new System.Exception("Illigal type used for time, must be OpenMI.Standard.ITimeStamp or OpenMI.Standard.TimeSpan");
			}

			return timeString;
		}

		/// <summary>
		/// Model descscription
		/// </summary>
        public override abstract string ModelDescription
		{
			get;
		}

		/// <summary>
		/// Model ID
		/// </summary>
        public override abstract string ModelID
		{
			get;
		}

		/// <summary>
		/// Time Horizon
		/// </summary>
        public override abstract ITimeSpan TimeHorizon
		{
			get;
		}

		/// <summary>
		/// Number of input exchange items
		/// </summary>
        public override abstract int InputExchangeItemCount
		{
			get;
		}

		/// <summary>
		/// number of output exchange items
		/// </summary>
        public override abstract int OutputExchangeItemCount
		{
			get;
		}

		/// <summary>
		/// get an input exchange item
		/// </summary>
		/// <param name="index">index number for the requested input exchange item</param>
		/// <returns>the requested input exchange item</returns>
        public override abstract IInputExchangeItem GetInputExchangeItem(int index);
		

		/// <summary>
		/// get an output exchange item.
		/// </summary>
		/// <param name="index">index number for the requested exchange item</param>
		/// <returns>the requested exchange item</returns>
        public override abstract IOutputExchangeItem GetOutputExchangeItem(int index);
		
		private ArrayList GetAllLinks()
		{
			ArrayList links = new ArrayList();

			foreach (SmartInputLink inputLink in _smartInputLinks)
			{
				links.Add(inputLink.link);
			}

			foreach (SmartOutputLink outputLink in _smartOutputLinks)
			{
				links.Add(outputLink.link);
			}
			return links;
		}

        /// <summary>
        /// Validate the component
        /// </summary>
        /// <returns>Empty string if no warnings were issued, or a description if there were warnings</returns>
        public override string Validate()
		{
			_validationErrorMessages.Clear();
			_validationWarningMessages.Clear();

			foreach (SmartLink link in _smartInputLinks) 
			{
				_validationErrorMessages.AddRange (link.GetErrors());
				_validationWarningMessages.AddRange (link.GetWarnings());
			}

			foreach (SmartLink link in _smartOutputLinks) 
			{
				_validationErrorMessages.AddRange (link.GetErrors());
				_validationWarningMessages.AddRange (link.GetWarnings());
			}

			string validationString = "";
			foreach (string str in _validationErrorMessages)
			{
				validationString += "Error: " + str + " ";
			}

			foreach (string str in _validationWarningMessages)
			{
				validationString += "Warning: " + str + ". ";
			}

			return validationString;
		}

		private void CheckTimeArgumentInGetvaluesMethod(ITime time)
		{
			if (time is ITimeSpan)
			{
				if (this._engineApiAccess is IEngine)
				{
                    if (IsLater(((IEngine)this._engineApiAccess).GetTimeHorizon().Start, ((ITimeSpan)time).Start))
                    {
                        throw new Exception("GetValues method was invoked using a time argument that representes a time before the allowed time horizon");
                    }
                    if (IsLater(((ITimeSpan)time).End, ((IEngine)this._engineApiAccess).GetTimeHorizon().End))
                    {
                        throw new Exception("GetValues method was invoked using a time argument that representes a time that is after the allowed time horizon");
                    }
                }
			}
			else if (time is ITimeStamp)
			{
				if (this._engineApiAccess is IEngine)
				{
                    if (IsLater(((IEngine)this._engineApiAccess).GetTimeHorizon().Start, (ITimeStamp)time))
                    {
                        throw new Exception("GetValues method was invoked using a time argument that representes a time before the allowed time horizon");
                    }
                    if (IsLater((ITimeStamp)time, ((IEngine)this._engineApiAccess).GetTimeHorizon().End))
                    {
                        throw new Exception("GetValues method was invoked using a time argument that representes a time that is after the allowed time horizon");
                    }
                }
			}
			else
			{
				throw new Exception("Illegal data type for time was used in argument to GetValues method. Type must be OpenMI.Standard.ITimeStamp or ITimeSpan");
			}
		}

		private void SendSourceAfterGetValuesCallEvent(ITime time, string LinkID)
		{
			Oatc.OpenMI.Sdk.Backbone.Event eventA = new Oatc.OpenMI.Sdk.Backbone.Event(EventType.SourceAfterGetValuesCall);
			eventA.Description = "GetValues(t = " + ITimeToString(time) + ", ";
            eventA.Description += "LinkID: " + LinkID; //TODO: QS = " + _smartOutputLinkSet.GetLink(LinkID).SourceQuantity.ID + " ,QT = " + _smartOutputLinkSet.GetLink(LinkID).TargetQuantity.ID;
            eventA.Description += ") <<<===";
			eventA.Sender = this;
			eventA.SimulationTime = TimeToTimeStamp(_engineApiAccess.GetCurrentTime());
			eventA.SetAttribute("GetValues time argument : ",ITimeToString(time));
			SendEvent(eventA);
		}

		private void SendEvent( EventType eventType)
		{
			Oatc.OpenMI.Sdk.Backbone.Event eventD = new Oatc.OpenMI.Sdk.Backbone.Event(eventType);
			eventD.Description = eventType.ToString();
			eventD.Sender = this;
			eventD.SimulationTime = TimeToTimeStamp(_engineApiAccess.GetCurrentTime());
			SendEvent(eventD);
		}
	}
}
