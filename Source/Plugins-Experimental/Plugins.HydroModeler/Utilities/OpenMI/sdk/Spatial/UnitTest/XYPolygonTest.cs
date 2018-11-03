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
using System.Collections;
using Oatc.OpenMI.Sdk.Spatial;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Spatial.UnitTest
{
	/// <summary>
	/// Summary description for XYPolygonTest.
	/// </summary>
  public class AXYPolygon: XYPolygon
  {    
    public int AFindEar()
    {
      return FindEar();
    }

    public bool AIsIntersected(int i)
    {
      return IsIntersected(i);
    }

    public bool AIsConvex(int i)
    {
      return IsConvex(i);
    }
  }
  
  [TestFixture]
	public class XYPolygonTest
	{
		[Test]
		public void GetArea()
		{
			// -- Rectangle --
			XYPolygon xypolygon = new XYPolygon();
			xypolygon.Points.Add(new XYPoint(1,1));
			xypolygon.Points.Add(new XYPoint(9,1));
			xypolygon.Points.Add(new XYPoint(9,6));
			xypolygon.Points.Add(new XYPoint(1,6));
			Assert.AreEqual((double) 40, xypolygon.GetArea());

			// -- Triangle -- 
			XYPolygon xypolygon2 = new XYPolygon();
			xypolygon2.Points.Add(new XYPoint(1,1));
			xypolygon2.Points.Add(new XYPoint(9,1));
			xypolygon2.Points.Add(new XYPoint(9,6));
			Assert.AreEqual((double) 20, xypolygon2.GetArea());

			// -- concave --
			XYPolygon xypolygon3 = new XYPolygon();
			xypolygon3.Points.Add(new XYPoint(1,1));
			xypolygon3.Points.Add(new XYPoint(5,3));
			xypolygon3.Points.Add(new XYPoint(9,1));
			xypolygon3.Points.Add(new XYPoint(9,6));
			xypolygon3.Points.Add(new XYPoint(1,6));
			Assert.AreEqual((double) 32, xypolygon3.GetArea());

			// -- concave --
			XYPolygon xypolygon4 = new XYPolygon();
			xypolygon4.Points.Add(new XYPoint(1,1));
			xypolygon4.Points.Add(new XYPoint(9,1));
			xypolygon4.Points.Add(new XYPoint(5,5));
			xypolygon4.Points.Add(new XYPoint(5,3));
			xypolygon4.Points.Add(new XYPoint(3,3));
			xypolygon4.Points.Add(new XYPoint(3,8));
			xypolygon4.Points.Add(new XYPoint(9,8));
			xypolygon4.Points.Add(new XYPoint(9,11));
			xypolygon4.Points.Add(new XYPoint(1,11));
			Assert.AreEqual((double) 50, xypolygon4.GetArea());
		}

		[Test] public void GetLine()
		{
			// -- Triangle -- 
			XYPolygon xypolygon = new XYPolygon();
			xypolygon.Points.Add(new XYPoint(1,2));
			xypolygon.Points.Add(new XYPoint(4,3));
			xypolygon.Points.Add(new XYPoint(2,5));

			Assert.AreEqual(new XYLine(1,2,4,3),xypolygon.GetLine(0));
			Assert.AreEqual(new XYLine(4,3,2,5),xypolygon.GetLine(1));
			Assert.AreEqual(new XYLine(2,5,1,2),xypolygon.GetLine(2));

			
			// -- concave polygon --
			XYPolygon xypolygon4 = new XYPolygon();
			xypolygon4.Points.Add(new XYPoint(1,1));
			xypolygon4.Points.Add(new XYPoint(9,1));
			xypolygon4.Points.Add(new XYPoint(5,5));
			xypolygon4.Points.Add(new XYPoint(5,3));
			xypolygon4.Points.Add(new XYPoint(3,3));
			xypolygon4.Points.Add(new XYPoint(3,8));
			xypolygon4.Points.Add(new XYPoint(9,8));
			xypolygon4.Points.Add(new XYPoint(9,11));
			xypolygon4.Points.Add(new XYPoint(1,11));

			Assert.AreEqual(new XYLine(1, 1, 9,  1),xypolygon4.GetLine(0));
			Assert.AreEqual(new XYLine(9, 1, 5,  5),xypolygon4.GetLine(1));
			Assert.AreEqual(new XYLine(5, 5, 5,  3),xypolygon4.GetLine(2));
			Assert.AreEqual(new XYLine(5, 3, 3,  3),xypolygon4.GetLine(3));
			Assert.AreEqual(new XYLine(3, 3, 3,  8),xypolygon4.GetLine(4));
			Assert.AreEqual(new XYLine(3, 8, 9,  8),xypolygon4.GetLine(5));
			Assert.AreEqual(new XYLine(9, 8, 9, 11),xypolygon4.GetLine(6));
			Assert.AreEqual(new XYLine(9,11, 1 ,11),xypolygon4.GetLine(7));
			Assert.AreEqual(new XYLine(1,11, 1 , 1),xypolygon4.GetLine(8));
		}

    [Test]
    public void Protected_FindEar()
    {
      AXYPolygon p1 = new AXYPolygon();
      p1.Points.Add(new XYPoint(0, 3));
      p1.Points.Add(new XYPoint(3, 0));
      p1.Points.Add(new XYPoint(8, 0));
      p1.Points.Add(new XYPoint(8, 2));
      p1.Points.Add(new XYPoint(3, 1));
      p1.Points.Add(new XYPoint(3, 3));
      p1.Points.Add(new XYPoint(8, 3));
      p1.Points.Add(new XYPoint(4, 7));
      Assert.AreEqual(2, p1.AFindEar(),"Test1");
    }

    [Test]
    public void Protected_IsIntersected()
    {
      AXYPolygon p1 = new AXYPolygon();
      p1.Points.Add(new XYPoint(0, 3));
      p1.Points.Add(new XYPoint(3, 0));
      p1.Points.Add(new XYPoint(8, 0));
      p1.Points.Add(new XYPoint(8, 2));
      p1.Points.Add(new XYPoint(3, 1));
      p1.Points.Add(new XYPoint(3, 3));
      p1.Points.Add(new XYPoint(8, 3));
      p1.Points.Add(new XYPoint(4, 7));
      Assert.AreEqual(true, p1.AIsIntersected(0),"Test1");     
      Assert.AreEqual(true, p1.AIsIntersected(1),"Test2");     
      Assert.AreEqual(false, p1.AIsIntersected(2),"Test3");     
    }

    [Test]
    public void Protected_IsConvex()
    {
      AXYPolygon xypolygon4 = new AXYPolygon();
      xypolygon4.Points.Add(new XYPoint(1,1));
      xypolygon4.Points.Add(new XYPoint(9,1));
      xypolygon4.Points.Add(new XYPoint(5,5));
      xypolygon4.Points.Add(new XYPoint(5,3));
      xypolygon4.Points.Add(new XYPoint(3,3));
      xypolygon4.Points.Add(new XYPoint(3,8));
      xypolygon4.Points.Add(new XYPoint(9,8));
      xypolygon4.Points.Add(new XYPoint(9,11));
      xypolygon4.Points.Add(new XYPoint(1,11));
		
      Assert.AreEqual( true, xypolygon4.AIsConvex(0));
      Assert.AreEqual( true, xypolygon4.AIsConvex(1));
      Assert.AreEqual( true, xypolygon4.AIsConvex(2));
      Assert.AreEqual( false, xypolygon4.AIsConvex(3));
      Assert.AreEqual( false, xypolygon4.AIsConvex(4));
      Assert.AreEqual( false, xypolygon4.AIsConvex(5));
      Assert.AreEqual( true, xypolygon4.AIsConvex(6));
      Assert.AreEqual( true, xypolygon4.AIsConvex(7));
      Assert.AreEqual( true, xypolygon4.AIsConvex(8));
    }

    [Test]
    public void GetTriangulation()
    {
      XYPolygon p1 = new XYPolygon();
      p1.Points.Add(new XYPoint(0, 3));
      p1.Points.Add(new XYPoint(3, 0));
      p1.Points.Add(new XYPoint(8, 0));
      p1.Points.Add(new XYPoint(8, 2));
      p1.Points.Add(new XYPoint(3, 1));
      p1.Points.Add(new XYPoint(3, 3));
      p1.Points.Add(new XYPoint(8, 3));
      p1.Points.Add(new XYPoint(4, 7));
      ArrayList triangleList = p1.GetTriangulation();
      XYPolygon refTriangle1 = new XYPolygon();
      refTriangle1.Points.Add(new XYPoint(3,0));
      refTriangle1.Points.Add(new XYPoint(8,0));
      refTriangle1.Points.Add(new XYPoint(8,2));
      XYPolygon refTriangle2 = new XYPolygon();
      refTriangle2.Points.Add(new XYPoint(3,0));
      refTriangle2.Points.Add(new XYPoint(8,2));
      refTriangle2.Points.Add(new XYPoint(3,1));
      XYPolygon refTriangle3 = new XYPolygon();
      refTriangle3.Points.Add(new XYPoint(0,3));
      refTriangle3.Points.Add(new XYPoint(3,0));
      refTriangle3.Points.Add(new XYPoint(3,1));
      XYPolygon refTriangle4 = new XYPolygon();
      refTriangle4.Points.Add(new XYPoint(0,3));
      refTriangle4.Points.Add(new XYPoint(3,1));
      refTriangle4.Points.Add(new XYPoint(3,3));
      XYPolygon refTriangle5 = new XYPolygon();
      refTriangle5.Points.Add(new XYPoint(4,7));
      refTriangle5.Points.Add(new XYPoint(0,3));
      refTriangle5.Points.Add(new XYPoint(3,3));
      XYPolygon refTriangle6 = new XYPolygon();
      refTriangle6.Points.Add(new XYPoint(3,3));
      refTriangle6.Points.Add(new XYPoint(8,3));
      refTriangle6.Points.Add(new XYPoint(4,7));
      Assert.AreEqual(refTriangle1 ,(XYPolygon) triangleList[0]);
      Assert.AreEqual(refTriangle2 ,(XYPolygon) triangleList[1]);
      Assert.AreEqual(refTriangle3 ,(XYPolygon) triangleList[2]);
      Assert.AreEqual(refTriangle4 ,(XYPolygon) triangleList[3]);
      Assert.AreEqual(refTriangle5 ,(XYPolygon) triangleList[4]);
      Assert.AreEqual(refTriangle6 ,(XYPolygon) triangleList[5]);
    }	
	
    [Test]
    public void Equals()
    {
      XYPolygon p1 = new XYPolygon();
      p1.Points.Add(new XYPoint(0, 3));
      p1.Points.Add(new XYPoint(3, 0));
      p1.Points.Add(new XYPoint(8, 0));
      p1.Points.Add(new XYPoint(8, 2));
      p1.Points.Add(new XYPoint(3, 1));
      p1.Points.Add(new XYPoint(3, 3));
      p1.Points.Add(new XYPoint(8, 3));
      p1.Points.Add(new XYPoint(4, 7));

      XYPolygon p2 = new XYPolygon();
      p2.Points.Add(new XYPoint(0, 3));
      p2.Points.Add(new XYPoint(3, 0));
      p2.Points.Add(new XYPoint(8, 0));
      p2.Points.Add(new XYPoint(8, 2));
      p2.Points.Add(new XYPoint(3, 1));
      p2.Points.Add(new XYPoint(3, 3));
      p2.Points.Add(new XYPoint(8, 3));
      p2.Points.Add(new XYPoint(4, 7));
      
      XYPolygon p3 = new XYPolygon();
      p3.Points.Add(new XYPoint(0, 3));
      p3.Points.Add(new XYPoint(3, 0));
      p3.Points.Add(new XYPoint(8, 0));
      p3.Points.Add(new XYPoint(8, 2));
      p3.Points.Add(new XYPoint(3, 1.1));
      p3.Points.Add(new XYPoint(3, 3));
      p3.Points.Add(new XYPoint(8, 3));
      p3.Points.Add(new XYPoint(4, 7));

      XYPolygon p4 = new XYPolygon();
      p4.Points.Add(new XYPoint(0, 3));
      p4.Points.Add(new XYPoint(3, 0));
      p4.Points.Add(new XYPoint(8, 0));
      p4.Points.Add(new XYPoint(8, 2));
      p4.Points.Add(new XYPoint(3, 1));
      p4.Points.Add(new XYPoint(3, 3));
      p4.Points.Add(new XYPoint(8, 3));
 
      XYPolyline p5 = new XYPolyline();
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
