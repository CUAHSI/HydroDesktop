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

namespace Oatc.OpenMI.Sdk.Backbone
{
	/// <summary>
	/// Argument is a class that contains (key,value) pairs.
    /// <para>This is a trivial implementation of OpenMI.Standard.IArgument, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class Argument : IArgument
	{
		private string _key="";
		private string _value="";
		private bool _readOnly = false;
		private string _description="";

		/// <summary>
		/// Empty constructor
		/// </summary>
		public Argument()
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">Source argument to copy</param>
		public Argument(IArgument source)
		{
			Key = source.Key;
			Value = source.Value;
			ReadOnly = source.ReadOnly;
			Description = source.Description;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="Value">Value</param>
		/// <param name="ReadOnly">Is argument read-only?</param>
		/// <param name="Description">Description</param>
		public Argument(string key,string Value,bool ReadOnly,string Description)
		{
			_key = key;
			_value = Value;
			_readOnly = ReadOnly;
			_description = Description;
		}
		#region IArgument Members

		///<summary>
		/// TODO: comment
		///</summary>
		public string Key
		{
			get
			{
				return _key;
			}
			set 
			{
				_key = value;
			}
		}

		///<summary>
		/// TODO: comment
		///</summary>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		///<summary>
		/// TODO: comment
		///</summary>
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
			}
		}

		///<summary>
		/// TODO: comment
		///</summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		#endregion

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(Object obj) 
		{
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			Argument d = (Argument)obj;
			return (Value.Equals(d.Value)&&Key.Equals(d.Key));
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_key != null) hashCode += _key.GetHashCode();
			if (_value != null) hashCode += _value.GetHashCode();
			return hashCode;
		}
	}
}
