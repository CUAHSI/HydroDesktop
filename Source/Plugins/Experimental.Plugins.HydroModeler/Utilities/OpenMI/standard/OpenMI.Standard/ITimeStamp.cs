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
	/// TimeStamp interface
	/// </summary>

	public interface ITimeStamp : ITime
	{

		/// <summary>
		/// Get TimeStamp expressed as ModifiedJulianDateAndTime (JulianDateAndTime - 2400000.5)
		/// Number of days since 1858/11/17 12:00:00.00, and fraction of 24hr.
		/// See for example http://aa.usno.navy.mil/data/docs/JulianDate.html
		/// </summary>

		double ModifiedJulianDay {get;}

	}

}
