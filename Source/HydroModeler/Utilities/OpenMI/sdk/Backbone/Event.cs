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
	/// Events are used to send informative and warning messages.
    /// <para>This is a trivial implementation of OpenMI.Standard.IEvent, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class Event:IEvent
	{
		private Hashtable _attributeTable = new Hashtable();
		private ILinkableComponent _sender;
		private ITimeStamp _simulationTime;
		private EventType _type=EventType.Informative;
		private string _description="";

		/// <summary>
		/// Constructor
		/// </summary>
		public Event()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">Event type</param>
		public Event(EventType type)
		{
			_type = type;	
		}

		/// <summary>
		/// Getter and setter for the event type
		/// </summary>
		public EventType Type
		{
			get {return _type;}
			set {_type = value;}
		}

		/// <summary>
		/// Getter and setter for the sender
		/// </summary>
		public ILinkableComponent Sender
		{
			get {return _sender;}
			set {_sender = value;}
		}

		/// <summary>
		/// Getter and setter for the simulation time
		/// </summary>
		public ITimeStamp SimulationTime
		{
			get {return _simulationTime;}
			set {_simulationTime = value;}
		}

		/// <summary>
		/// Getter and setter for the description
		/// </summary>
		public string Description
		{
			get {return _description;}
			set {_description = value;}
		}

		/// <summary>
		/// Sets an attribute for the event with a (key,value) pair
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="val">The value</param>
		public void SetAttribute(string key, object val)
		{
			_attributeTable.Add(key,val);

		}
		/// <summary>
		/// Gets an attribute for a given key
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The attribute</returns>
		public object GetAttribute(string key)
		{
			return (_attributeTable[key]);

		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(Object obj) 
		{
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			Event e = (Event) obj;
			if (!Description.Equals(e.Description))
				return false;
			if (!Type.Equals(e.Type))
				return false;
			if (!SimulationTime.Equals(e.SimulationTime))
				return false;
			if (Sender!=e.Sender)
				return false;
			ICollection Keys = _attributeTable.Keys;
			ICollection eKeys = e._attributeTable.Keys;
			if (Keys.Count!=eKeys.Count)
				return false;
			IEnumerator enumerator = Keys.GetEnumerator();
			while (enumerator.MoveNext()) 
			{	string key = (string) enumerator.Current;
				string val = (string) _attributeTable[key];

				if (!e._attributeTable.ContainsKey(key))
					return false;
				if (!e._attributeTable[key].Equals(val))
					return false;
			}
			return true;
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_sender != null) hashCode += _sender.GetHashCode();
			hashCode += _type.GetHashCode();
			if (_simulationTime != null) hashCode += _simulationTime.GetHashCode();
			return hashCode;
		}
	}
}
 
