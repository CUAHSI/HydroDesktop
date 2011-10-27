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
	/// Summary description for XYPolylineTest.
	/// </summary>
	[TestFixture]
	public class XYPolylineTest
	{
		public XYPolylineTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[Test]
		public void GetLength()
		{
			XYPolyline xyPolyline = new XYPolyline();
			xyPolyline.Points.Add(new XYPoint(6,2));
			xyPolyline.Points.Add(new XYPoint(2,2));
			xyPolyline.Points.Add(new XYPoint(8,2));
			xyPolyline.Points.Add(new XYPoint(8,4));
			xyPolyline.Points.Add(new XYPoint(5,4));
			xyPolyline.Points.Add(new XYPoint(9,7));
			
			Assert.AreEqual((double) 20, xyPolyline.GetLength()); 
		}

		[Test]
		public void GetLine()
		{
			XYPolyline xyPolyline = new XYPolyline();
			xyPolyline.Points.Add(new XYPoint(6,2));
			xyPolyline.Points.Add(new XYPoint(2,2));
			xyPolyline.Points.Add(new XYPoint(8,2));
			xyPolyline.Points.Add(new XYPoint(8,4));
			xyPolyline.Points.Add(new XYPoint(5,4));
			xyPolyline.Points.Add(new XYPoint(9,7));
	
			Assert.AreEqual(new XYLine(6,2,2,2),xyPolyline.GetLine(0));
			Assert.AreEqual(new XYLine(2,2,8,2),xyPolyline.GetLine(1));
			Assert.AreEqual(new XYLine(8,2,8,4),xyPolyline.GetLine(2));
			Assert.AreEqual(new XYLine(8,4,5,4),xyPolyline.GetLine(3));
			Assert.AreEqual(new XYLine(5,4,9,7),xyPolyline.GetLine(4));
		}
    
    [Test]
    public void Equals()
    {
      XYPolyline p1 = new XYPolyline();
      p1.Points.Add(new XYPoint(0, 3));
      p1.Points.Add(new XYPoint(3, 0));
      p1.Points.Add(new XYPoint(8, 0));
      p1.Points.Add(new XYPoint(8, 2));
      p1.Points.Add(new XYPoint(3, 1));
      p1.Points.Add(new XYPoint(3, 3));
      p1.Points.Add(new XYPoint(8, 3));
      p1.Points.Add(new XYPoint(4, 7));

      XYPolyline p2 = new XYPolyline();
      p2.Points.Add(new XYPoint(0, 3));
      p2.Points.Add(new XYPoint(3, 0));
      p2.Points.Add(new XYPoint(8, 0));
      p2.Points.Add(new XYPoint(8, 2));
      p2.Points.Add(new XYPoint(3, 1));
      p2.Points.Add(new XYPoint(3, 3));
      p2.Points.Add(new XYPoint(8, 3));
      p2.Points.Add(new XYPoint(4, 7));
      
      XYPolyline p3 = new XYPolyline();
      p3.Points.Add(new XYPoint(0, 3));
      p3.Points.Add(new XYPoint(3, 0));
      p3.Points.Add(new XYPoint(8, 0));
      p3.Points.Add(new XYPoint(8, 2));
      p3.Points.Add(new XYPoint(3, 1.1));
      p3.Points.Add(new XYPoint(3, 3));
      p3.Points.Add(new XYPoint(8, 3));
      p3.Points.Add(new XYPoint(4, 7));

      XYPolyline p4 = new XYPolyline();
      p4.Points.Add(new XYPoint(0, 3));
      p4.Points.Add(new XYPoint(3, 0));
      p4.Points.Add(new XYPoint(8, 0));
      p4.Points.Add(new XYPoint(8, 2));
      p4.Points.Add(new XYPoint(3, 1));
      p4.Points.Add(new XYPoint(3, 3));
      p4.Points.Add(new XYPoint(8, 3));
 
      XYPolygon p5 = new XYPolygon();
      p5.Points.Add(new XYPoint(0, 3));
      p5.Points.Add(new XYPoint(3, 0));
      p5.Points.Add(new XYPoint(8, 0));
      p5.Points.Add(new XYPoint(8, 2));
      p5.Points.Add(new XYPoint(3, 1.1));
      p5.Points.Add(new XYPoint(3, 3));
      p5.Points.Add(new XYPoint(8, 3));
      p5.Points.Add(new XYPoint(4, 7));
      
      Assert.AreEqual(true, p1.Equals(p1),"Test1");     
      Assert.AreEqual(true, p1.Equals(p2),"Test2");     
      Assert.AreEqual(false, p1.Equals(p3),"Test3");
      Assert.AreEqual(false, p1.Equals(p4),"Test4");
      Assert.AreEqual(false, p1.Equals(p5),"Test5");
    }
	}
}
