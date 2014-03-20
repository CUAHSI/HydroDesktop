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
using NUnit.Framework;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class VectorTest
	{
		Vector vector;
		[SetUp]
		public void Init()
		{
			vector = new Vector(1.0,2.0,3.0);
		}

		[Test]
		public void Constructor()
		{
			Vector vector2 = new Vector(vector);

			Assert.AreEqual(vector,vector2);
		}

		[Test]
		public void Components()
		{
			Assert.AreEqual(1.0,vector.XComponent);
			Assert.AreEqual(2.0,vector.YComponent);
			Assert.AreEqual(3.0,vector.ZComponent);

			vector.XComponent = 4.0;
			vector.YComponent = 5.0;
			vector.ZComponent = 6.0;

			Assert.AreEqual(4.0,vector.XComponent);
			Assert.AreEqual(5.0,vector.YComponent);
			Assert.AreEqual(6.0,vector.ZComponent);
		}

		[Test]
		public void Equals()
		{
			Vector vector1 = new Vector(1.0,2.0,3.0);
			Assert.IsTrue(vector.Equals(vector1));

			Assert.IsFalse(vector.Equals(null));
			Assert.IsFalse(vector.Equals("string"));
		}

		[Test]
		public void EqualsX()
		{
			Vector vector1 = new Vector(1.1,2.0,3.0);
			Assert.IsFalse(vector.Equals(vector1));
		}

		[Test]
		public void EqualsY()
		{
			Vector vector1 = new Vector(1.0,2.1,3.0);
			Assert.IsFalse(vector.Equals(vector1));
		}

		[Test]
		public void EqualsZ()
		{
			Vector vector1 = new Vector(1.0,2.0,3.1);
			Assert.IsFalse(vector.Equals(vector1));
		}

	}
}
