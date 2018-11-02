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
	public class TimeSpanTest
	{
		TimeSpan timeSpan;

		[SetUp]
		public void Init()
		{	
			timeSpan = new TimeSpan(new TimeStamp(1.0),new TimeStamp(2.0));
		}

		[Test]
		public void Constructor ()
		{
			TimeSpan timeSpan2 = new TimeSpan(timeSpan);
			Assert.AreEqual(timeSpan,timeSpan2);
		}

		[Test]
		public void Start()
		{
			Assert.AreEqual(new TimeStamp(1.0),timeSpan.Start);
			timeSpan.Start = new TimeStamp(2.0);
			Assert.AreEqual(new TimeStamp(2.0),timeSpan.Start);
		}

		[Test]
		public void End()
		{
			Assert.AreEqual(new TimeStamp(2.0),timeSpan.End);
			timeSpan.End = new TimeStamp(3.0);
			Assert.AreEqual(new TimeStamp(3.0),timeSpan.End);
		}

		[Test]
		public void Equals()
		{
			Assert.IsTrue(timeSpan.Equals(new TimeSpan(new TimeStamp(1.0),new TimeStamp(2.0))));
			Assert.IsFalse(timeSpan.Equals(null));
			Assert.IsFalse(timeSpan.Equals("string"));
		}

		[Test]
		public void EqualsStart()
		{
			Assert.IsFalse(timeSpan.Equals(new TimeSpan(new TimeStamp(1.1),new TimeStamp(2.0))));
		}

		[Test]
		public void EqualsEnd()
		{
			Assert.IsFalse(timeSpan.Equals(new TimeSpan(new TimeStamp(1.0),new TimeStamp(2.1))));
		}
	}
}
