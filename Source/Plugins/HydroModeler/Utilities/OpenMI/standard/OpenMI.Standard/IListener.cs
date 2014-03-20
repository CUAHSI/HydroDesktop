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
	/// Listener interface
	/// </summary>
	public interface IListener
	{
		/// <summary>
		/// Method called when event is raised
		/// </summary>
		/// <param name="anEvent">Event that has been raised.</param>
		void OnEvent(IEvent anEvent);

		/// <summary>
		/// Get number of accepted event types
		/// </summary>
		/// <returns>Number of accepted event types.</returns>
		int GetAcceptedEventTypeCount();


		/// <summary>
		/// <para>Get accepted event type with index acceptedEventTypeIndex.</para>
        /// 
        /// <para>If this method is invoked with an argument that is outside the interval
        /// [0, numberOfAcceptedEventsTypes], where numberOfAcceptedEventsTypes is
        /// the values obtained through the method GetAcceptedEventTypeCount(),
        /// an exception must be thrown.</para> 
		/// </summary>
		/// <param name="acceptedEventTypeIndex">index in accepted event types.</param>
		/// <returns>Accepted event type.</returns>
		EventType GetAcceptedEventType(int acceptedEventTypeIndex);

	}
}
