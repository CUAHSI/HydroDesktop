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
using Oatc.OpenMI.Sdk.Spatial;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Spatial.UnitTest
{
	/// <summary>
	/// Summary description for XYPointTest.
	/// </summary>
	[TestFixture]
	public class XYPointTest
	{
    [Test]
    public void Equals()
    {
      XYPoint p1 = new XYPoint(2,3);
      XYPoint p2 = new XYPoint(2,3);
      XYPoint p3 = new XYPoint(2,-3);
      XYLine l1 = new XYLine(2,3,3,4);
      Assert.AreEqual(true, p1.Equals(p1),"Test1");
      Assert.AreEqual(true, p1.Equals(p2),"Test2");
      Assert.AreEqual(false, p1.Equals(p3),"Test3");
      Assert.AreEqual(false, p1.Equals(l1),"Test4");
    }

		[Test]
		public void PropertyTest()
		{
			XYPoint xypoint = new XYPoint(2,3);
			Assert.AreEqual((double) 2, xypoint.X);
			Assert.AreEqual((double) 3, xypoint.Y);

			xypoint.X = 6;
			xypoint.Y = 7;
			Assert.AreEqual((double) 6, xypoint.X);
			Assert.AreEqual((double) 7, xypoint.Y);

		}
	}
}
