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

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
	/// <summary>
	/// This interface allows generic querying of an object's properties.
	/// An aggregate serves as an "in between" object between a source (containing the actual information) and a querier (asking for properties, e.g. XmlFile).
	/// </summary>
	public interface IAggregate
	{
		/// <summary>
		/// The underlying object which holds the actual information.
		/// </summary>
		object Source {get;}

		/// <summary>
		/// List of properties which can be queried in a generic way
		/// </summary>
		string[] Properties {get;} 

		/// <summary>
		/// Gets the class type of one of the properties
		/// </summary>
		/// <param name="property">Property name</param>
		/// <returns>The property type</returns>
		Type GetType (string property);

		/// <summary>
		/// Tells whether a value can be assigned to the property
		/// </summary>
		/// <param name="property">Property name</param>
		/// <returns>Boolean indicating writable</returns>
		bool CanWrite (string property);

		/// <summary>
		/// Tells whether a value can be retrieved from the property
		/// </summary>
		/// <param name="property">Property name</param>
		/// <returns>Boolean indicating readable</returns>
		bool CanRead (string property);

		/// <summary>
		/// Gets the value of a property
		/// </summary>
		/// <param name="property">Property name</param>
		/// <returns>The property value</returns>
		object GetValue (string property);

		/// <summary>
		/// Sets the value of a property
		/// </summary>
		/// <param name="property">Property name</param>
		/// <param name="target">The new property value</param>
		void SetValue (string property, object target);

		/// <summary>
		/// Gets a property value by reference.
		/// A reference isn't necessary a property, but can be any string, as long as it can be interpreted by the aggregate
		/// </summary>
		/// <param name="reference">Reference</param>
		/// <returns>The referenced value</returns>
		object GetReferencedValue (string reference); 

		/// <summary>
		/// Tells the aggregate to process all information passed with SetValue calls
		/// </summary>
		void UpdateSource();

		/// <summary>
		/// Tells the aggregate to prepare for subsequent GetValue calls
		/// </summary>
		void UpdateAggregate();
	}
}
