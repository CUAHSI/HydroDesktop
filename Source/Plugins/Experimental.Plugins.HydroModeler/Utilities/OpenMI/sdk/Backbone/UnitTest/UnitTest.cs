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
	public class UnitTest
	{
		Unit unit;

		[SetUp]
		public void Init()
		{
			unit = new Unit("ID",3.4,6.7,"description");
		}

		public void Constructor()
		{
			Unit unit2 = new Unit(unit);

			Assert.AreEqual(unit,unit2);
		}

		[Test]
		public void Description ()
		{
			Assert.AreEqual("description",unit.Description);
			unit.Description = "new";
			Assert.AreEqual("new",unit.Description);
		}

		[Test]
		public void ID ()
		{
			Assert.AreEqual("ID",unit.ID);
			unit.ID = "new";
			Assert.AreEqual("new",unit.ID);
		}

		[Test]
		public void ConversionFactorToSI()
		{
			Assert.AreEqual(3.4,unit.ConversionFactorToSI);
			unit.ConversionFactorToSI = 3.5;
			Assert.AreEqual(3.5,unit.ConversionFactorToSI);

		}

		[Test]
		public void OffsetToSI()
		{
			Assert.AreEqual(6.7,unit.OffSetToSI);
			unit.OffSetToSI = 6.9;
			Assert.AreEqual(6.9,unit.OffSetToSI);
		}

		[Test]
		public void Equals()
		{
			Unit unit1 = new Unit("ID",3.4,6.7,"description");
			Assert.IsTrue(unit.Equals(unit1));
			unit1 = new Unit("ID1",3.4,6.7,"description");
			Assert.IsFalse(unit.Equals(unit1));
			unit1 = new Unit("ID",3.5,6.7,"description");
			Assert.IsFalse(unit.Equals(unit1));
			unit1 = new Unit("ID",3.4,6.8,"description");
			Assert.IsFalse(unit.Equals(unit1));
			unit1 = new Unit("ID",3.4,6.7,"description1");
			Assert.IsFalse(unit.Equals(unit1));

			Assert.IsFalse(unit.Equals(null));
			Assert.IsFalse(unit.Equals("string"));
		}
	}
}
