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
	public class VertexTest
	{
		public VertexTest()
		{
		}

		[Test]
		public void Constructor()
		{
			Vertex vertex = new Vertex(3.0,4.0,5.0);
			Assert.AreEqual(3.0,vertex.x);
			Assert.AreEqual(4.0,vertex.y);
			Assert.AreEqual(5.0,vertex.z);

			Vertex vertex2 = new Vertex(vertex);
			Assert.AreEqual(vertex,vertex2);
		}

		[Test]
		public void X()
		{
			Vertex vertex = new Vertex();
			vertex.x = 8.0;
			Assert.AreEqual(8.0,vertex.x);
		}

		[Test]
		public void Y()
		{
			Vertex vertex = new Vertex();
			vertex.y = 8.0;
			Assert.AreEqual(8.0,vertex.y);
		}

		[Test]
		public void Z()
		{
			Vertex vertex = new Vertex();
			vertex.z = 8.0;
			Assert.AreEqual(8.0,vertex.z);
		}

		[Test]
		public void Equals()
		{
			Vertex vertex1 = new Vertex(2.0,3.0,4.0);
			Vertex vertex2 = new Vertex(2.0,3.0,4.0);
			Assert.IsTrue(vertex1.Equals(vertex2));
			vertex1.x = 1.0;
			Assert.IsFalse(vertex1.Equals(vertex2));
			vertex1.x = 2.0;
			vertex1.y = 2.0;
			Assert.IsFalse(vertex1.Equals(vertex2));
			vertex1.y = 3.0;
			vertex1.z = 5.0;
			Assert.IsFalse(vertex1.Equals(vertex2));
			Assert.IsFalse(vertex1.Equals(null));
			Assert.IsFalse(vertex1.Equals("string"));
		}
	}
}
