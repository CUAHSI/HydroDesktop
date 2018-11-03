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
using NUnit.Framework;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class VectorSetTest
	{
		VectorSet vectorSet;
		[SetUp]
		public void Init()
		{
			Vector vector1 = new Vector(1.0,2.0,3.0);
			Vector vector2 = new Vector(4.0,5.0,6.0);
			Vector vector3 = new Vector(7.0,8.0,9.0);

			Vector[] data = {vector1,vector2,vector3};

			vectorSet = new VectorSet(data);
		}

		[Test]
		public void Constructor()
		{
			VectorSet vectorSet2 = new VectorSet(vectorSet);
			Assert.AreEqual(vectorSet,vectorSet2);
		}

		[Test]
		public void GetVector()
		{
			Assert.AreEqual(new Vector(1.0,2.0,3.0),vectorSet.GetVector(0));
			Assert.AreEqual(new Vector(4.0,5.0,6.0),vectorSet.GetVector(1));
			Assert.AreEqual(new Vector(7.0,8.0,9.0),vectorSet.GetVector(2));
		}

		[Test]
		public void Count()
		{
			Assert.AreEqual(3,vectorSet.Count);
		}

		[Test]
		public void Equals()
		{
			Vector vector1 = new Vector(1.0,2.0,3.0);
			Vector vector2 = new Vector(4.0,5.0,6.0);
			Vector vector3 = new Vector(7.0,8.0,9.0);

			Vector[] data = {vector1,vector2,vector3};

			VectorSet vectorSet2 = new VectorSet(data);

			Assert.IsTrue(vectorSet.Equals(vectorSet2));
		}

		[Test]
		public void EqualsVector()
		{
			Vector vector1 = new Vector(1.0,2.0,3.0);
			Vector vector2 = new Vector(4.0,6.0,6.0);
			Vector vector3 = new Vector(7.0,8.0,9.0);

			Vector[] data = {vector1,vector2,vector3};

			VectorSet vectorSet2 = new VectorSet(data);

			Assert.IsFalse(vectorSet.Equals(vectorSet2));
		}

	}
}
