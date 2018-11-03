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
	/// <para>Within and outside modelling exercises, many situations occur where ‘raw’ data is desired at
    /// the (discrete) time stamp as it is available in the source component. A typical example is
    /// the comparison of computation results with monitoring data, or a computational core that wants
    /// to adhere to the time stepping of its data source. To keep the values fixed to the discrete
    /// times as they are available in the source component, the IDiscreteTimes interface has been
    /// defined. This interface can provide a list of time stamps for which values of a quantity on
    /// an element set are available.</para>
    /// 
    /// <para>Note that the IDiscreteTimes interface is an optional interface to provide more detailed
    /// information on the temporal discretization of available data. It is not required to implement
    /// the IDiscreteTimes interface in order to claim OpenMI compliance for a Component. However, if
    /// the IDiscreteTimes interface is implemented it must be implemented according to the definitions
    /// given below.</para>
	/// </summary>
	public interface IDiscreteTimes
	{
		/// <summary>
        /// Returns true if the component can provide discrete times for the specific exchange
        /// item defined by the arguments quantity and elementSet 
		/// </summary>
		bool HasDiscreteTimes(IQuantity quantity, IElementSet elementSet);

		
		/// <summary>
        /// Returns the number of discrete time steps for a specific combination of ElementSet and Quantity
		/// </summary>
		int GetDiscreteTimesCount(IQuantity quantity, IElementSet elementSet);
		

		/// <summary>
        /// Get n-th discrete time stamp or time span for a specific combination of ElementSet and Quantity.
    /// This method must accept values of discreteTimeIndex in the interval [0, GetDiscreteTimesCount - 1].  
    /// If the discreteTimeIndex is outside this interval an exception must be thrown.
		/// </summary>
		/// <param name="quantity">The quantity.</param>
		/// <param name="elementSet">The element.</param>
		/// <param name="discreteTimeIndex">index of timeStep.</param>
		/// <returns>Discrete time stamp or time span.</returns>
		ITime GetDiscreteTime(IQuantity quantity, IElementSet elementSet, int discreteTimeIndex);
	}
}
