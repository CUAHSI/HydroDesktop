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
    /// The IArgument interface defines a key – value pair. If the property ReadOnly is
    /// false the value is editable otherwise it is read-only. 
	/// </summary>
	public interface IArgument
	{
		/// <summary>
        /// The key (string) in key-value pair.
		/// </summary>
		string Key {get;}

		/// <summary>
        /// <para>The value (double) in key-value pair.</para> 
        /// 
        /// <para>If the ReadOnly property is true and the property is attempted to be changed 
        /// from outside an exception must be thrown.</para>
		/// </summary>
		string Value 
		{
			get;
			set;
		}

		/// <summary>
        /// Defines whether the Values property may be edited from outside. 
		/// </summary>
		bool ReadOnly {get;}


		/// <summary>
        /// Description of the key-value pair.
		/// </summary>
		string Description {get;}
	}
}

