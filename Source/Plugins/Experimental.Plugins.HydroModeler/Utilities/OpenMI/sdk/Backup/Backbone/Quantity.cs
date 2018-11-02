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
using OpenMI.Standard;
using ValueType=OpenMI.Standard.ValueType;

namespace Oatc.OpenMI.Sdk.Backbone
{

	/// <summary>
	/// The Quantity class contains a unit, description, id, and dimension.
    /// <para>This is a trivial implementation of OpenMI.Standard.IQuantity, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class Quantity : IQuantity 
	{	
		private IUnit _unit = new Unit();
		string _description = "";
		private string _id = "";
		private IDimension _dimension=new Dimension();
		private ValueType _valueType=ValueType.Scalar;

		/// <summary>
		/// Constructor
		/// </summary>
		public Quantity()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">ID</param>
		public Quantity(String id)
		{
			_id = id;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The quantity to copy</param>
		public Quantity(IQuantity source)
		{
			Description = source.Description;
			Dimension = source.Dimension;
			ID = source.ID;
			Unit = source.Unit;
			ValueType = source.ValueType;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="unit">Unit</param>
		/// <param name="Description">Description</param>
		/// <param name="ID">ID</param>
		public Quantity(IUnit unit, string Description, string ID)
		{	_unit = unit;
			_description = Description;
			_id = ID;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="unit">Unit</param>
		/// <param name="Description">Description</param>
		/// <param name="ID">ID</param>
		/// <param name="valueType">Value type (vector or scalar)</param>
		public Quantity(IUnit unit, string Description, string ID,ValueType valueType)
		{
			_unit = unit;
			_description = Description;
			_id = ID;
			_valueType = valueType;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="unit">Unit</param>
		/// <param name="Description">Description</param>
		/// <param name="ID">ID</param>
		/// <param name="valueType">Value type (vector or scalar)</param>
		/// <param name="Dimension">Dimension</param>
		public Quantity(IUnit unit, string Description, string ID,ValueType valueType,
			IDimension Dimension)
		{
			_unit = unit;
			_description = Description;
			_id = ID;
			_valueType = valueType;
			_dimension = Dimension;
		}

		/// <summary>
		/// Getter and setter for ID
		/// </summary>
		public string ID
		{
			get {return _id;}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Getter and setter for Dimension
		/// </summary>
		public IDimension Dimension
		{
			get {return _dimension;}
			set
			{
				_dimension = value;
			}
		}

		/// <summary>
		/// Getter and setter for description
		/// </summary>
		public string Description
		{
			get {return _description;}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Getter and setter for unit
		/// </summary>
		public IUnit Unit
		{
			get {return _unit;}
			set
			{
				_unit = value;
			}
		}

		/// <summary>
		/// Returns the ID
		/// </summary>
		/// <returns>ID</returns>
		public override String ToString() 
		{
			return ID;
		}

		/// <summary>
		/// Getter and setter for value type (scalar/vector)
		/// </summary>
		public ValueType ValueType 
		{
			get { return _valueType;}
			set
			{
				_valueType = value;
			}
		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(Object obj) 
		{
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			Quantity q = (Quantity) obj;
			if (!ID.Equals(q.ID))
				return false;
			if (!Unit.Equals(q.Unit))
				return false;
			if (!ValueType.Equals(q.ValueType))
				return false;
			if (!Description.Equals(q.Description))
				return false;
			if (!Dimension.Equals(q.Dimension))
				return false;
			return true;
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_id != null) hashCode += _id.GetHashCode();
			if (_unit != null) hashCode += _unit.GetHashCode();
			if (_dimension != null) hashCode += _dimension.GetHashCode();
			return hashCode;
		}
	}
} 
