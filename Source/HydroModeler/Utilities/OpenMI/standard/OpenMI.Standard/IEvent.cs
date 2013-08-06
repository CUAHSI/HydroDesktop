#region Copyright
/*
    Copyright (c) 2005,2006,2007, OpenMI Association
    "http://www.openmi.org/"

    This file is part of OpenMI.Standard.dll

    OpenMI.Standard.dll is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    OpenMI.Standard.dll is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

namespace OpenMI.Standard
{

	/// <summary>
	/// Enumeration for event types
	/// </summary>
	public enum EventType : int
	{
		/// <summary>
		/// Warning.
		/// </summary>
		Warning = 0,

		/// <summary>
		/// Informative. Any type of information.
		/// </summary>
		Informative = 1,

		/// <summary>
        /// Value out of range. If a LinkableComponent receives values through the GetValues method, 
        /// which are detected by the receiving component as out-of-range an OutOfRange event must 
        /// be send. Alternatively, if the component cannot proceed with the received value or if 
        /// proceeding with the received value will make the component unstable or make the component 
        /// generate erroneous results and exception can be thrown.  
		/// </summary>
		ValueOutOfRange = 2,

		/// <summary>
        /// Global progress. Indicates progress as percentage of global time horizon. It 
        /// is not mandatory for LinkableComponent to provide this event type. 
		/// </summary>
		GlobalProgress = 3,

		/// <summary>
        /// Timestep progress. Indicates progress as % for the current time step. 
        /// It is not mandatory for LinkableComponent to provide this event type. 
		/// </summary>
		TimeStepProgres = 4,

		/// <summary>
        /// Data changed. Events of this event type must be send at least once 
        /// during each period when the LinkableComponent hold the thread if the internal 
        /// state of the component has changed.
		/// </summary>
		DataChanged = 5,

		/// <summary>
        /// Target before GetValues call. Immediately before a LinkableComponent invokes 
        /// the GetValues method in another LinkableComponent an event of type 
        /// TargetBeforeGetValuesCall must be send. 
		/// </summary>
		TargetBeforeGetValuesCall = 6,

		/// <summary>
        /// Source after GetValues call. Immediately when the GetValues method is invoked in 
        /// a LinkableComponent this component must send an event of type SourceAfterGetValuesCall  
		/// </summary>
		SourceAfterGetValuesCall = 7,

		/// <summary>
        /// Source before GetValues return. Immediately before a LinkableComponent in which the GetValues 
        /// method has been invoked returns the thread to the calling component an event of type 
        /// SourceBeforeGetValuesReturn must be send.
		/// </summary>
		SourceBeforeGetValuesReturn = 8,

		/// <summary>
        /// Target after GetValues return. Immediately after a LinkableComponent which has 
        /// invoked the GetValues method in another LinkableComponent receives the thread back from 
        /// this component (after this component returns the values) an event of type 
        /// TargetAfterGetValuesReturn must be send. 
		/// </summary>
		TargetAfterGetValuesReturn = 9,

		/// <summary>
        /// Other. Any other event that is found useful to implement. 
		/// </summary>
		Other = 10,

	/// <summary>
	/// Number of event types
	/// </summary>
		NUM_OF_EVENT_TYPES // added by Jan Curn (3/10/2005)
	}
	/// <summary>
	/// <para>Within modern software systems, events are often applied for all types of messaging. Within
    /// OpenMI a lightweight event mechanism is applied, using a generic Event interface and an enumeration
    /// of event types (OpenMI.Standard.EventType) to allow the implementation of generic tools that
    /// perform monitoring tasks such as logging, tracing, or online visualization. Linkable components
    /// must generate events to which other linkable components or tools can subscribe. In this way, it 
    /// becomes possible to implement these generic tools without requiring any knowledge of the specific 
    /// tools in the components themselves. By adopting the OpenMI event types, system developers can use 
    /// those tools without additional effort. Note that the event mechanism should not be used to pass 
    /// data sets. Data sets should be retrieved through the GetValues() call.</para>
    ///
    /// <para>The event mechanism is also used to facilitate pausing and resuming of the computation 
    /// thread, as the computation process of an entire model chain is rather autonomous and not 
    /// controlled by any master controller. Once a component receives the thread, it must send an 
    /// event, so listeners (e.g. a GUI) can grab and hold the thread, and thus pause the computation 
    /// by not returning control. In normal conditions, the control is returned so the component can 
    /// continue its computation. Of course the computation is also controlled at the level that 
    /// triggers the first component of the chain by means of a GetValues()-call. Stop firing those 
    /// calls will also result in a paused system, although it may take a while before an entire call 
    /// stack completes its processing activity.</para>
	/// </summary>

	public interface IEvent
	{

		/// <summary>
		/// Type of event
		/// </summary>
		EventType Type {get;}


		/// <summary>
		/// Additional descriptive information
		/// </summary>
		string Description {get;}


		/// <summary>
		/// Linkable component that generated the event
		/// </summary>
		ILinkableComponent Sender {get;}


		/// <summary>
		/// Current SimulationTime
		/// </summary>
		ITimeStamp SimulationTime {get;}


		/// <summary>
        /// Get the value of a Key=Value pair, containing additional information on the event.
        /// This method must throw an exception if the key is not recognized.  
		/// </summary>
		object GetAttribute(string key);

	}
}
