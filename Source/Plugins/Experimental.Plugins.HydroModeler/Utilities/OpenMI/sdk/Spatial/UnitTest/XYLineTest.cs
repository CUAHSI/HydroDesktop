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
	/// Summary description for XYLineTest.
	/// </summary>
	[TestFixture]
	public class XYLineTest
	{
		public XYLineTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[Test]
		public void ConstructionTest()
		{
			XYLine xyLine1 = new XYLine(1,2,3,4);
			Assert.AreEqual((double) 1, xyLine1.P1.X);
			Assert.AreEqual((double) 2, xyLine1.P1.Y);
			Assert.AreEqual((double) 3, xyLine1.P2.X);
			Assert.AreEqual((double) 4, xyLine1.P2.Y);

			XYLine xyLine2 = new XYLine(xyLine1);
			Assert.AreEqual(xyLine1.P1.X, xyLine2.P1.X);
			Assert.AreEqual(xyLine1.P1.Y, xyLine2.P1.Y);
			Assert.AreEqual(xyLine1.P2.X, xyLine2.P2.X);
			Assert.AreEqual(xyLine1.P2.Y, xyLine2.P2.Y);

			XYLine xyLine3 = new XYLine(new XYPoint(1,2),new XYPoint(3,4));
			Assert.AreEqual((double) 1, xyLine3.P1.X);
			Assert.AreEqual((double) 2, xyLine3.P1.Y);
			Assert.AreEqual((double) 3, xyLine3.P2.X);
			Assert.AreEqual((double) 4, xyLine3.P2.Y);
		}


		[Test]
		public void GetLength()
		{
			Assert.AreEqual((double) 5, (new XYLine(1,1,4,5)).GetLength());
			Assert.AreEqual((double) 5, (new XYLine(4,5,1,1)).GetLength());
			Assert.AreEqual((double) 9, (new XYLine(1,5,10,5)).GetLength());
			Assert.AreEqual((double) 6, (new XYLine(4,2,4,8)).GetLength());
		}

		[Test]
		public void GetMidpoint()
		{
			Assert.AreEqual(new XYPoint(4,4),new XYLine(2,3,6,5).GetMidpoint());
			Assert.AreEqual(new XYPoint(4,4),new XYLine(6,5,2,3).GetMidpoint());
			Assert.AreEqual(new XYPoint(4,4),new XYLine(2,4,6,4).GetMidpoint());
			Assert.AreEqual(new XYPoint(4,4),new XYLine(4,2,4,6).GetMidpoint());
		}

    [Test]
    public void Equals()
    {
      XYLine l1 = new XYLine(0, 3, 3, 0);
      XYLine l2 = new XYLine(0, 3, 3, 0);
      XYLine l3 = new XYLine(3, 3, 3, 0);
      XYPolyline pl1 = new XYPolyline();
      pl1.Points.Add(new XYPoint(0, 3));
      pl1.Points.Add(new XYPoint(3, 0));
      
      Assert.AreEqual(true, l1.Equals(l1),"Test1");     
      Assert.AreEqual(true, l1.Equals(l2),"Test2");     
      Assert.AreEqual(false, l1.Equals(l3),"Test3");
      Assert.AreEqual(false, l1.Equals(pl1),"Test4");
    }
	}
}
