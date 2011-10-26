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
using System.Collections;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class ElementTest
	{
		public ElementTest()
		{
		}

		[Test]
		public void Constructor()
		{
			Element element = new Element("ElementID");
			element.AddVertex(new Vertex(3.0,4.0,5.0));
			element.AddVertex(new Vertex(4.0,5.0,6.0));
			element.AddVertex(new Vertex(5.0,6.0,7.0));
			Assert.AreEqual("ElementID",element.ID);

			Element element2 = new Element(element);

			Assert.AreEqual(element,element2);
		}

		[Test]
		public void ID()
		{
			Element element = new Element();
			element.ID = "ElementID";
			Assert.AreEqual("ElementID",element.ID);
		}

		[Test]
		public void Vertices()
		{
			Element element = new Element();

			element.AddVertex(new Vertex(3.0,4.0,5.0));
			element.AddVertex(new Vertex(4.0,5.0,6.0));
			element.AddVertex(new Vertex(5.0,6.0,7.0));

			Assert.AreEqual(3,element.VertexCount);
			Assert.AreEqual(new Vertex(3.0,4.0,5.0),element.Vertices[0]);
			Assert.AreEqual(new Vertex(4.0,5.0,6.0),element.Vertices[1]);
			Assert.AreEqual(new Vertex(5.0,6.0,7.0),element.Vertices[2]);
		}

		[Test]
		public void Faces()
		{
			int[] index = {1,2,3,4,5};
			Element element = new Element();
			element.AddFace(index);
			Assert.AreEqual(1,element.FaceCount);
			Assert.AreEqual(index,element.GetFaceVertexIndices(0));
		}

		[Test]
		public void Equals()
		{
			Element element1 = new Element("ElementID");

			element1.AddVertex(new Vertex(3.0,4.0,5.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			element1.AddVertex(new Vertex(5.0,6.0,7.0));

			Element element2 = new Element("ElementID");

			element2.AddVertex(new Vertex(3.0,4.0,5.0));
			element2.AddVertex(new Vertex(4.0,5.0,6.0));

			Assert.IsFalse(element1.Equals(element2));

			element2.AddVertex(new Vertex(5.0,6.0,7.0));

			Assert.IsTrue(element1.Equals(element2));

			element1.ID = "ElementID1";

			Assert.IsFalse(element1.Equals(element2));

			Assert.IsFalse(element1.Equals(null));
			Assert.IsFalse(element1.Equals("string"));


		}
	}
}
