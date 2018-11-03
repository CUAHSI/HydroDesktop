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
	/// Publisher interface
	/// </summary>

	public interface IPublisher 
	{

		/// <summary>
		/// Subscribes a listener
		/// </summary>
		/// <param name="listener">The listener.</param>
		/// <param name="eventType">The event type.</param>
		void Subscribe(IListener listener, EventType eventType);


		/// <summary>
		/// Unsubscribes a listener
		/// </summary>
		/// <param name="listener">The listener.</param>
		/// <param name="eventType">The event type.</param>
		void UnSubscribe(IListener listener, EventType eventType);


		/// <summary>
		/// Sends an event to all subscribed listeners
		/// </summary>
		/// <param name="Event">The event.</param>
		void SendEvent(IEvent Event);


		/// <summary>
		/// Get number of published event types
		/// </summary>
		/// <returns>Number of provided event types.</returns>
		int GetPublishedEventTypeCount();


		/// <summary>
		/// Get provided event type with index providedEventTypeIndex
		/// </summary>
		/// <param name="providedEventTypeIndex">index in provided event types.</param>
		/// <returns>Provided event type.</returns>
		EventType GetPublishedEventType(int providedEventTypeIndex);

	}
}
