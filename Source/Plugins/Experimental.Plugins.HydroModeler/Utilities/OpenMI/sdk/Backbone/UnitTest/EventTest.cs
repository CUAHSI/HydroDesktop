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
	public class EventTest
	{
		Event e;

		[SetUp]
		public void Init()
		{
			e = new Event(EventType.Informative);
			e.Description = "Description";
			e.SimulationTime = new TimeStamp(44060.0);
			e.SetAttribute("key1","value1");
			e.SetAttribute("key2","value2");
			e.SetAttribute("key3","value3");
		}

		[Test]
		public void Type()
		{
			Assert.AreEqual(EventType.Informative,e.Type);
			e.Type = EventType.Warning;
			Assert.AreEqual(EventType.Warning,e.Type);
		}

		[Test]
		public void Description()
		{
			Assert.AreEqual("Description",e.Description);
		}

		[Test]
		public void SimulationTime()
		{
			Assert.AreEqual(new TimeStamp(44060.0),e.SimulationTime);
		}

		[Test]
		public void Attribute()
		{
			Assert.AreEqual("value1",e.GetAttribute("key1"));
			Assert.AreEqual("value2",e.GetAttribute("key2"));
			Assert.AreEqual("value3",e.GetAttribute("key3"));
		}

		[Test]
		public void Equals()
		{
			Event e1 = new Event(EventType.Informative);
			e1.Description = "Description";
			e1.SimulationTime = new TimeStamp(44060.0);
			e1.SetAttribute("key1","value1");
			e1.SetAttribute("key2","value2");
			e1.SetAttribute("key3","value3");

			Assert.IsTrue(e1.Equals(e));

			Assert.IsFalse(e.Equals(null));
			Assert.IsFalse(e.Equals("string"));
		}

		[Test]
		public void EqualsType()
		{
			Event e1 = new Event(EventType.DataChanged);
			e1.Description = "Description";
			e1.SimulationTime = new TimeStamp(44060.0);
			e1.SetAttribute("key1","value1");
			e1.SetAttribute("key2","value2");
			e1.SetAttribute("key3","value3");

			Assert.IsFalse(e1.Equals(e));
		}

		[Test]
		public void EqualsDescription()
		{
			Event e1 = new Event(EventType.Informative);
			e1.Description = "Description2";
			e1.SimulationTime = new TimeStamp(44060.0);
			e1.SetAttribute("key1","value1");
			e1.SetAttribute("key2","value2");
			e1.SetAttribute("key3","value3");

			Assert.IsFalse(e1.Equals(e));
		}

		[Test]
		public void EqualsSimulationTime()
		{
			Event e1 = new Event(EventType.Informative);
			e1.Description = "Description";
			e1.SimulationTime = new TimeStamp(44061.0);
			e1.SetAttribute("key1","value1");
			e1.SetAttribute("key2","value2");
			e1.SetAttribute("key3","value3");

			Assert.IsFalse(e1.Equals(e));
		}

		[Test]
		public void EqualsAttribute()
		{
			Event e1 = new Event(EventType.Informative);
			e1.Description = "Description";
			e1.SimulationTime = new TimeStamp(44060.0);
			e1.SetAttribute("key1","value1");
			e1.SetAttribute("key2","value2");

			Assert.IsFalse(e1.Equals(e));

			e1.SetAttribute("key3","value4");

			Assert.IsFalse(e1.Equals(e));
		}
	}
}
