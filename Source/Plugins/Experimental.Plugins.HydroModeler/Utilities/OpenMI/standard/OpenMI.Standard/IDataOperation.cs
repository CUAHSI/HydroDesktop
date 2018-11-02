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
	/// DataOperation interface
	/// </summary>

	public interface IDataOperation
	{
		/// <summary>
        /// This method should not be a part of the standard since it is not required 
        /// to be invoked by any outside component. However, in order to avoid changing 
        /// the standard it will remain in the IDataOperation interface. 
        /// It is recommended simply to make an empty implementation of this method.
		/// </summary>
		void Initialize(IArgument[] properties);

		/// <summary>
        /// <para>Identification string for the data operation.</para>
        /// 
        /// <para>Two or more data operations provided by one OutputExchangeItem may not have the same ID.</para> 
		/// 
        /// <para>EXAMPLE:</para>
        /// <para>"Mean value", "Max value", "Spatially averaged", "Accumulated", "linear conversion"</para>
        /// </summary>
		string ID {get;}


		/// <summary>
		/// Number of arguments for this data operation
		/// </summary>
		int ArgumentCount {get;}


		/// <summary>
        /// <para>Gets the argument object (instance of class implementing IArgument) as
        /// identified by the argumentIndex parameter.</para>
        /// </summary>
        /// 
        /// <param name="argumentIndex">
        /// <para>The index-number of the requested DataOperation(indexing starts from zero)</para>
        /// <para>This method must accept values of argumentIndex in the interval [0, ArgumentCount - 1].
        /// If the argumentIndex is outside this interval an exception must be thrown.</para>.</param>
        /// 
        /// <returns>The Argument as identified by argumentIndex.</returns>
		IArgument GetArgument(int argumentIndex);

		/// <summary>
        /// Validates a specific combination of InputExchangeItem, OutputExchangeItem and a 
        /// selection of DataOperations. If this combination is valid true should be 
        /// returned otherwise false should be returned.
		/// </summary>
        /// 
		/// <param name="inputExchangeItem">The input exchange item.</param>
        /// 
		/// <param name="outputExchangeItem">The output exchange item.</param>
        /// 
		/// <param name="selectedDataOperations">The already selected data operations.</param>
        /// 
        /// <returns>True if the combination of InputExchangeItem, OutputExchangeItem, and the array 
        /// of dataOperations provided in the methods argument is valid, otherwise false.</returns>
		bool IsValid(IInputExchangeItem inputExchangeItem,IOutputExchangeItem outputExchangeItem,
			IDataOperation[] selectedDataOperations);
	}
}
