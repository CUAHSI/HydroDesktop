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

namespace Oatc.OpenMI.Sdk.Backbone
{
	/// <summary>
	/// The Vertex class contains a (x,y,z) coordinate
    /// </summary>
	[Serializable]
	public class Vertex
	{
		private double _x,_y,_z;

		/// <summary>
		/// Constructor
		/// </summary>
		public Vertex()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="z">Z coordinate</param>
		public Vertex(double x,double y,double z) 
		{
			_x = x;
			_y = y;
			_z = z;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The vertex to copy</param>
		public Vertex(Vertex source)
		{
			x = source.x;
			y = source.y;
			z = source.z;
		}

		/// <summary>
		/// Getter and setter for X coordinate
		/// </summary>
		public double x
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		/// <summary>
		/// Getter and setter for X coordinate
		/// </summary>
		public double y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		/// <summary>
		/// Getter and setter for X coordinate
		/// </summary>
		public double z
		{
			get
			{
				return _z;
			}
			set
			{
				_z = value;
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
			Vertex v = (Vertex)obj;
			return (x == v.x && y == v.y && z == v.z);
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			return _x.GetHashCode() + _y.GetHashCode() + _z.GetHashCode();
		}

		///<summary>
		/// String representation of the vertext
		///</summary>
		///<returns></returns>
		public override string ToString()
		{
			return string.Format("Vertex{0},{1},{2}", _x, _y, _z);
		}
	}
}
