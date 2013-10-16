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
	/// <para>An IOutputExchangeItem describes an output item that can be delivered by a LinkableComponent.
	/// The item describes on which elementset a quantity can be provided.</para>
	/// <para>An output exchange item may provide data operations (interpolation in time, spatial interpolation etc.) that can
	/// be performed on the output exchange item before the values are delivered to the target ILinkableComponent</para>
	/// </summary>

	public interface IOutputExchangeItem : IExchangeItem
	{
		/// <summary>
		/// The number of data operations that can be performed on the output quantity/elemenset.
		/// </summary>
		int DataOperationCount {get;}


		/// <summary>
		/// Get one of the data operations that can be performed on the output quantity/elemenset.
		/// </summary>
		/// <param name="dataOperationIndex">The index for the data operation [0, DataOperationCount-1].</param>
		/// <returns>The data operation for index dataOperationIndex.</returns>
		IDataOperation GetDataOperation (int dataOperationIndex);

	}
}
