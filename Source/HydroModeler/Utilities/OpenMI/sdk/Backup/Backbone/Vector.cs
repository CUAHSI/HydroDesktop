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
	/// The Vector class contains x,y,z components.
    /// <para>This is a trivial implementation of OpenMI.Standard.IVector, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class Vector : IVector
	{

		private double _x;
		private double _y;
		private double _z;

		/// <summary>
		/// Constructor
		/// </summary>
		public Vector()
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The vector to copy</param>
		public Vector(IVector source)
		{
			XComponent = source.XComponent;
			YComponent = source.YComponent;
			ZComponent = source.ZComponent;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="x">x value</param>
		/// <param name="y">y value</param>
		/// <param name="z">z value</param>
		public Vector (double x, double y, double z)
		{
			_x = x;
			_y = y;
			_z = z;
		}

		/// <summary>
		/// Getter and setter for x component
		/// </summary>
		public double XComponent
		{
			get {return _x;}
			set
			{
				_x = value;
			}
		}

		/// <summary>
		/// Getter and setter for y component
		/// </summary>
		public double YComponent
		{
			get {return _y;}
			set
			{
				_y = value;
			}
		}

		/// <summary>
		/// Getter and setter for z component
		/// </summary>
		public double ZComponent
		{
			get {return _z;}
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
			Vector v = (Vector) obj;
			return (_x==v._x&&_y==v._y&&_z==v._z);
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			return _x.GetHashCode() +
				_y.GetHashCode() +
				_z.GetHashCode();
		}
	}
}
 
