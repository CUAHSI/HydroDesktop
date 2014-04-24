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
	/// Manage State Interface. (To be implemented optionally, in addition to
	/// the linkable component interface.)
	/// </summary>
	public interface IManageState
	{

		/// <summary>
		/// Store the linkable component's current State
		/// </summary>
		/// <returns>State identifier.</returns>
		string KeepCurrentState();

		/// <summary>
		/// Restores the state identified by the parameter stateID. If the state identifier identified by
		/// stateID is not known by the linkable component an exception should be trown.
		/// </summary>
		/// <param name="stateID">State identifier.</param>
		void RestoreState(string stateID);

		/// <summary>
		/// Clears a state from the linkable component's memory. If the state identifier identified by
		/// stateID is not known by the linkable component an exception should be trown.
		/// </summary>
		/// <param name="stateID">State identifier.</param>
		void ClearState(string stateID);
	}

}
