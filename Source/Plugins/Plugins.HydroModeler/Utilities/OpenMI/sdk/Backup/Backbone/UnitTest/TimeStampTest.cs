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
	public class TimeStampTest
	{
		TimeStamp timeStamp;
		[SetUp]
		public void Init()
		{
			timeStamp = new TimeStamp(12345.3);
		}

		[Test]
		public void Constructor()
		{
			TimeStamp timeStamp2 = new TimeStamp(timeStamp);
			Assert.AreEqual(timeStamp,timeStamp2);
		}
		[Test]
		public void ModifiedJulianDay()
		{
			Assert.AreEqual(12345.3,timeStamp.ModifiedJulianDay);
			timeStamp.ModifiedJulianDay = 54321.7;
			Assert.AreEqual(54321.7,timeStamp.ModifiedJulianDay);
		}
		[Test]
		public void Equals()
		{
			TimeStamp timeStamp1 = new TimeStamp(12345.3);
			Assert.IsTrue(timeStamp.Equals(timeStamp1));
			timeStamp1.ModifiedJulianDay = 34.0;
			Assert.IsFalse(timeStamp.Equals(timeStamp1));

			Assert.IsFalse(timeStamp.Equals(null));
			Assert.IsFalse(timeStamp.Equals("string"));
		}

		[Test]
		public void CompareTo()
		{
			TimeStamp timeStamp1 = new TimeStamp(12345.3);
			Assert.AreEqual(0.0,timeStamp.CompareTo(timeStamp1));
			timeStamp1.ModifiedJulianDay = 10000;
			Assert.IsTrue(timeStamp.CompareTo(timeStamp1)>0.0);
			Assert.IsTrue(timeStamp1.CompareTo(timeStamp)<0.0);
		}

		[Test]
		public void String()
		{
			Assert.AreEqual("12345.3",timeStamp.ToString());
		}
	}
}
