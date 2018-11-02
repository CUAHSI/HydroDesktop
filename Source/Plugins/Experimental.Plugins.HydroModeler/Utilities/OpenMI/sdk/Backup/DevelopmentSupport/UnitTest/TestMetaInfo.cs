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
using Oatc.OpenMI.Sdk.DevelopmentSupport;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// Summary description for TestMetaInfo.
	/// </summary>
	[TestFixture]
	public class TestMetaInfo
	{
		public TestMetaInfo()
		{
		}

		[SetUp] public void Init() 
		{
			MetaInfo.SetAttribute (typeof (Element), "subject1", "e1");
			MetaInfo.SetAttribute (typeof (Element), "property1", "subject1", "ep1");
			MetaInfo.SetAttribute (typeof (Element), "property2", "subject1", "ep2");
			MetaInfo.SetAttribute (typeof (Element), "property3", "subject1", "ep3");

			MetaInfo.SetAttribute (typeof (Node), "subject2", "n1");
			MetaInfo.SetAttribute (typeof (Node), "property1", "subject2", "np1");
			MetaInfo.SetAttribute (typeof (Node), "property2", "subject1", null);
			MetaInfo.SetAttribute (typeof (Node), "property3", "subject1", "np3");
		}

		[Test] public void Attribute() 
		{
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "subject1"), "e1");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property1", "subject1"), "ep1");
			Assert.AreEqual (MetaInfo.GetAttributeDefault (typeof (Element), "property1", "subject1", "def"), "ep1");
			Assert.AreEqual (MetaInfo.GetAttributeDefault (typeof (Element), "property1", "subject2", "def"), "def");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property1", "subject2"), null);
		}

		[Test] public void AttributeInheritance() 
		{
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "subject1"), "e1");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "subject2"), "n1");

			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property1", "subject1"), "ep1");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property1", "subject2"), "np1");

			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property3", "subject1"), "ep3");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property3", "subject1"), "np3");

			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property2", "subject1"), "ep2");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property2", "subject1"), null, "null overridden");
		}
	}
}
