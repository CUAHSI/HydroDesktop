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
    //TODO: write some more xml comments..
	/// <summary>
	/// Link interface
	/// </summary>

	public interface ILink
	{
	
		/// <summary>
		/// Identification string
		/// </summary>
		string ID {get;}


		/// <summary>
		/// Additional descriptive information
		/// </summary>
		string Description {get;}


		/// <summary>
		/// Number of data operations
		/// </summary>
		int DataOperationsCount {get;}


		/// <summary>
		/// Get the data operation with index DataOperationIndex.
        /// If this method is invoked with a dataOperationIndex, which is outside the interval
        /// [0,DataOperationCount] an exception must be thrown.
		/// </summary>
        ///  <returns>DataOperation according to the argument: dataOperationCount.</returns>
		IDataOperation GetDataOperation(int dataOperationIndex);


		/// <summary>
		/// Target quantity
		/// </summary>
		
		IQuantity TargetQuantity {get;}


		/// <summary>
		/// Target elementset
		/// </summary>

		IElementSet TargetElementSet {get;}


		/// <summary>
		/// Source elementset
		/// </summary>

		IElementSet SourceElementSet {get;}


		/// <summary>
		/// Souce linkable component
		/// </summary>

		ILinkableComponent SourceComponent {get;}


		/// <summary>
		/// Source quantity
		/// </summary>

		IQuantity SourceQuantity {get;}


		/// <summary>
		/// Target linkable component
		/// </summary>

		ILinkableComponent TargetComponent {get;}

	}
}
