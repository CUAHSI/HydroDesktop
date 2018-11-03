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
	/// The VectorSet class contains a list of vectors.
    /// <para>This is a trivial implementation of OpenMI.Standard.IVectorSet, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class VectorSet  : IVectorSet
	{

		Vector[] _values;

		/// <summary>
		/// Constructor
		/// </summary>
		public VectorSet()
		{
			_values = new Vector[0];
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Values">List of vectors</param>
		public VectorSet(Vector[] Values)
		{
			_values = Values;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The VectorSet to copy</param>
		public VectorSet(IVectorSet source)
		{
			_values = new Vector[source.Count];
			for (int i=0;i<source.Count;i++) 
			{
				IVector vector = source.GetVector(i);
				_values[i] = new Vector(vector.XComponent,vector.YComponent,
					vector.ZComponent);
			}
		}

		/// <summary>
		/// Returns if a element is valid
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <returns>True if the element is valid</returns>
		public virtual bool IsValid(int elementIndex)
		{
			return true;
		}

		/// <summary>
		/// Gets a vector with a given index
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>The vector</returns>
		public IVector GetVector(int index)
		{
			return _values[index];
		}

		/// <summary>
		/// The number of elements in the list
		/// </summary>
		public int Count
		{
			get
			{
				return _values.Length;
			}
		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(Object obj) 
		{
			if (obj is VectorSet) 
			{
				VectorSet sourceSet = (VectorSet)obj;
				if (sourceSet.Count!=Count)
					return false;
				for (int i=0;i<Count;i++) 
				{
					if (!sourceSet.GetVector(i).Equals(GetVector(i)))
						return false;
				}
				return true;
			}

			return base.Equals(obj);
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_values != null) hashCode += _values.GetHashCode();
			return hashCode;
		}
	}
}
 
