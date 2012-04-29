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
using System.IO;
using Oatc.OpenMI.Sdk.Spatial;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Spatial.UnitTest

{
	/// <summary>
	/// Summary description for XYGeometryToolsTest.
	/// </summary>
  public class AXYGeometryTools: XYGeometryTools
  {    
    public static double ACalculateLengthOfLineInsidePolygon(XYLine line, XYPolygon polygon)
    {
      return CalculateLengthOfLineInsidePolygon(line, polygon);
    }
    
    public static double ACalculateLineToPointDistance(XYLine line, XYPoint point)
    {
      return CalculateLineToPointDistance(line, point);
    }
    
    public static double ACalculateSharedLength(XYLine lineA, XYLine lineB)
    {
      return CalculateSharedLength(lineA, lineB);
    }
        
    public static bool AIntersectionPoint(XYLine lineA, XYLine lineB, ref XYPoint intersectionPoint)
    {
      return IntersectionPoint(lineA, lineB, ref intersectionPoint);
    }

    public static bool AIsPointInLine(XYPoint point, XYLine line)
    {
      return IsPointInLine(point, line);
    }
    
    public static bool AIsPointInLineInterior(XYPoint point, XYLine line)
    {
      return IsPointInLineInterior(point, line);
    } 

    public static bool AIsPointInPolygonOrOnEdge(double x, double y, XYPolygon polygon)
    {
      return IsPointInPolygonOrOnEdge(x, y, polygon);
    }
   
    public static double ATriangleIntersectionArea(XYPolygon triangleA, XYPolygon triangleB)
    {
      return TriangleIntersectionArea(triangleA, triangleB);
    }
  }

  [TestFixture]
	public class XYGeometryToolsTest
	{
    [Test]
    public void CalculateIntersectionPoint()
    {
      // Testing two overloaded methods with the same lines.
      Assert.AreEqual(new XYPoint(5,2.5),XYGeometryTools.CalculateIntersectionPoint(new XYPoint(2,3.5), new XYPoint(8,1.5), new XYPoint(2,1), new XYPoint(8,4)));
      Assert.AreEqual(new XYPoint(5,2.5),XYGeometryTools.CalculateIntersectionPoint(new XYLine(2, 3.5, 8, 1.5), new XYLine (2, 1, 8, 4)));

      // Intersection between horizontal and vertical lines
      Assert.AreEqual(new XYPoint(5,1),XYGeometryTools.CalculateIntersectionPoint(new XYPoint(1,1),new XYPoint(9,1), new XYPoint(5,5),new XYPoint(5,0)));
    }

    [Test]
    [ExpectedException(typeof(System.Exception))]
    public void CalculateIntersectionPoint_Exception_1()
    {
      XYGeometryTools.CalculateIntersectionPoint(new XYPoint(2,3.5), new XYPoint(8,1.5), new XYPoint(2,2.5), new XYPoint(8,0.5));
    }
    [Test]
    [ExpectedException(typeof(System.Exception))]
    public void CalculateIntersectionPoint_Exception_2()
    {
      XYGeometryTools.CalculateIntersectionPoint(new XYPoint(0,1), new XYPoint(1,1.5), new XYPoint(1,1.5), new XYPoint(8,0.5));
    }

    [Test]
    public void Protected_CalculateLengthOfLineInsidePolygon()
    {
      XYPolygon xypolygon = new XYPolygon();
      xypolygon.Points.Add(new XYPoint(1,1));
      xypolygon.Points.Add(new XYPoint(9,1));
      xypolygon.Points.Add(new XYPoint(5,5));
      xypolygon.Points.Add(new XYPoint(5,3));
      xypolygon.Points.Add(new XYPoint(3,3));
      xypolygon.Points.Add(new XYPoint(3,8));
      xypolygon.Points.Add(new XYPoint(9,8));
      xypolygon.Points.Add(new XYPoint(9,11));
      xypolygon.Points.Add(new XYPoint(1,11));

      Assert.AreEqual(0, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,12,11,12),xypolygon),"Test1");
      Assert.AreEqual(4, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,11,11,11),xypolygon),"Test2");
      Assert.AreEqual(8, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,10,11,10),xypolygon),"Test3");
      Assert.AreEqual(8, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,9,11,9),xypolygon),"Test4");
      
      Assert.AreEqual(5, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,8,11,8),xypolygon),"Test5");
      Assert.AreEqual(2, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,7,11,7),xypolygon),"Test6");
      Assert.AreEqual(2, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,5,11,5),xypolygon),"Test7");
      Assert.AreEqual(3, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,4,11,4),xypolygon),"Test8");

      Assert.AreEqual(3, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,4,11,4),xypolygon),"Test9");
      Assert.AreEqual(5, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,3,11,3),xypolygon),"Test10");
      Assert.AreEqual(7, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,2,11,2),xypolygon),"Test11");
      Assert.AreEqual(4, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,1,11,1),xypolygon),"Test12");
      Assert.AreEqual(0, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,0,11,0),xypolygon),"Test13");

      Assert.AreEqual(10, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(2,12,2,0),xypolygon),"Test14");
      Assert.AreEqual(6,  AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(6,12,6,0),xypolygon),"Test15");
      Assert.AreEqual(Math.Sqrt(8)+1.5*Math.Sqrt(0.5),AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(1,0.5,10,9.5),xypolygon),"Test16");
      Assert.AreEqual(1, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(-2,4,2,4),xypolygon),"Test17");
      Assert.AreEqual(5, AXYGeometryTools.ACalculateLengthOfLineInsidePolygon(new XYLine(4,12,4,0),xypolygon),"Test18");
    }

    [Test]
    public void CalculateLengthOfPolylineInsidePolygon()
    {
        XYPolygon xypolygon = new XYPolygon();
        xypolygon.Points.Add(new XYPoint(1,1));
        xypolygon.Points.Add(new XYPoint(9,1));
        xypolygon.Points.Add(new XYPoint(5,5));
        xypolygon.Points.Add(new XYPoint(5,3));
        xypolygon.Points.Add(new XYPoint(3,3));
        xypolygon.Points.Add(new XYPoint(3,8));
        xypolygon.Points.Add(new XYPoint(9,8));
        xypolygon.Points.Add(new XYPoint(9,11));
        xypolygon.Points.Add(new XYPoint(1,11));

        XYPolyline xypolyline = new XYPolyline();
        xypolyline.Points.Add(new XYPoint(9,13));
        xypolyline.Points.Add(new XYPoint(7,12));
        xypolyline.Points.Add(new XYPoint(7,10));
        xypolyline.Points.Add(new XYPoint(2,10));
        xypolyline.Points.Add(new XYPoint(2,3));

        Assert.AreEqual(13,XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(xypolyline, xypolygon));

        XYPolygon rectangle = new XYPolygon();
        rectangle.Points.Add(new XYPoint(10,10));
        rectangle.Points.Add(new XYPoint(20,10));
        rectangle.Points.Add(new XYPoint(20,40));
        rectangle.Points.Add(new XYPoint(10,40));

        XYPolyline line1 = new XYPolyline();
        line1.Points.Add(new XYPoint(0,20));
        line1.Points.Add(new XYPoint(30,20));
        Assert.AreEqual(10, XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(line1, rectangle));  // horizontal line crossing
        
        XYPolyline line2 = new XYPolyline();
        line2.Points.Add(new XYPoint(10,20));
        line2.Points.Add(new XYPoint(20,20));
        Assert.AreEqual(10, XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(line2, rectangle));  // fits inside

        XYPolyline line3 = new XYPolyline();
        line3.Points.Add(new XYPoint(0,40));
        line3.Points.Add(new XYPoint(30,40));
        Assert.AreEqual(5, XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(line3, rectangle));

        XYPolyline line4 = new XYPolyline();
        line4.Points.Add(new XYPoint(20,40));
        line4.Points.Add(new XYPoint(20,0));
        
        Assert.AreEqual(15, XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(line4, rectangle));

        XYPolyline line5 = new XYPolyline();
        line5.Points.Add(new XYPoint(20,40));
        line5.Points.Add(new XYPoint(20,10));
        Assert.AreEqual(15, XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(line5, rectangle));

        XYPolyline line6 = new XYPolyline();
        line6.Points.Add(new XYPoint(10,40));
        line6.Points.Add(new XYPoint(30,40));
        Assert.AreEqual(5, XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(line6, rectangle));

        XYPolyline line7 = new XYPolyline();
        line7.Points.Add(new XYPoint(10,20));
        line7.Points.Add(new XYPoint(30,20));
        Assert.AreEqual(10, XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(line7, rectangle));


        



    }

    [Test]
    public void Protected_CalculateLineToPointDistance()
    {
      Assert.AreEqual(Math.Sqrt(2),AXYGeometryTools.ACalculateLineToPointDistance(new XYLine(0,2,2,0),new XYPoint(2,2)),1e-12,"Test1");
      Assert.AreEqual(0,AXYGeometryTools.ACalculateLineToPointDistance(new XYLine(0,2,2,0),new XYPoint(1,1)),1e-12,"Test2");
      Assert.AreEqual(Math.Sqrt(2),AXYGeometryTools.ACalculateLineToPointDistance(new XYLine(0,2,2,0),new XYPoint(3,1)),1e-12,"Test3");
      Assert.AreEqual(1,AXYGeometryTools.ACalculateLineToPointDistance(new XYLine(0,2,2,0),new XYPoint(3,0)),1e-12,"Test4");
      Assert.AreEqual(Math.Sqrt(2),AXYGeometryTools.ACalculateLineToPointDistance(new XYLine(0,2,2,0),new XYPoint(3,-1)),1e-12,"Test5");
      Assert.AreEqual(1,AXYGeometryTools.ACalculateLineToPointDistance(new XYLine(0,2,2,0),new XYPoint(2,-1)),1e-12,"Test6");
      Assert.AreEqual(Math.Sqrt(2),AXYGeometryTools.ACalculateLineToPointDistance(new XYLine(0,2,2,0),new XYPoint(1,-1)),1e-12,"Test1");
    }
   
    [Test]
    public void CalculatePointToPointDistance()
    {
      Assert.AreEqual((double) 5, XYGeometryTools.CalculatePointToPointDistance(new XYPoint(1,1),new XYPoint(4,5)));
      Assert.AreEqual((double) 5, XYGeometryTools.CalculatePointToPointDistance(new XYPoint(4,5),new XYPoint(1,1)));
      Assert.AreEqual((double) 9, XYGeometryTools.CalculatePointToPointDistance(new XYPoint(1,5),new XYPoint(10,5)));
      Assert.AreEqual((double) 6, XYGeometryTools.CalculatePointToPointDistance(new XYPoint(4,2),new XYPoint(4,8)));
    }

    [Test]
    public void CalculatePolylineToPointDistance()
    {
      XYPolyline polyline = new XYPolyline();
      polyline.Points.Add(new XYPoint(0,0));
      polyline.Points.Add(new XYPoint(1,1));
      polyline.Points.Add(new XYPoint(2,2));
      polyline.Points.Add(new XYPoint(4,2));
      polyline.Points.Add(new XYPoint(6,0));
      Assert.AreEqual(Math.Sqrt(2),XYGeometryTools.CalculatePolylineToPointDistance(polyline, new XYPoint(-1,-1)),1e-12,"Test1");
      Assert.AreEqual(Math.Sqrt(2),XYGeometryTools.CalculatePolylineToPointDistance(polyline, new XYPoint(2,0)),1e-12,"Test2");
      Assert.AreEqual(0,XYGeometryTools.CalculatePolylineToPointDistance(polyline, new XYPoint(2,2)),1e-12,"Test3");
      Assert.AreEqual(1,XYGeometryTools.CalculatePolylineToPointDistance(polyline, new XYPoint(3,1)),1e-12,"Test4");
      Assert.AreEqual(1,XYGeometryTools.CalculatePolylineToPointDistance(polyline, new XYPoint(3,3)),1e-12,"Test5");
      Assert.AreEqual(2,XYGeometryTools.CalculatePolylineToPointDistance(polyline, new XYPoint(3,4)),1e-12,"Test6");
      Assert.AreEqual(0,XYGeometryTools.CalculatePolylineToPointDistance(polyline, new XYPoint(6,0)),1e-12,"Test7");
    }
    
    [Test]
    public void CalculateSharedArea()
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
      p2.Points.Add(new XYPoint(3, 3));
      p2.Points.Add(new XYPoint(4, 3));
      p2.Points.Add(new XYPoint(4, 4));
      p2.Points.Add(new XYPoint(3, 4));

      XYPolygon p3 = new XYPolygon();
      p3.Points.Add(new XYPoint(0, 0));
      p3.Points.Add(new XYPoint(8, 0));
      p3.Points.Add(new XYPoint(8, 8));
      p3.Points.Add(new XYPoint(0, 8));

      XYPolygon p4 = new XYPolygon();
      p4.Points.Add(new XYPoint(-2, 0));
      p4.Points.Add(new XYPoint(3, 0));
      p4.Points.Add(new XYPoint(3, 2));
      p4.Points.Add(new XYPoint(0, 2));
      p4.Points.Add(new XYPoint(0, 5));
      p4.Points.Add(new XYPoint(4, 5));
      p4.Points.Add(new XYPoint(4, 7));
      p4.Points.Add(new XYPoint(-2, 7));

      Assert.AreEqual(p1.GetArea(),XYGeometryTools.CalculateSharedArea(p1,p1),1e-12,"Test1 - Polygon1 in Polygon1");  
      Assert.AreEqual(p2.GetArea(),XYGeometryTools.CalculateSharedArea(p2,p2),1e-12,"Test2 - Polygon1 in Polygon1");  
      Assert.AreEqual(p4.GetArea(),XYGeometryTools.CalculateSharedArea(p4,p4),1e-12,"Test3 - Polygon1 in Polygon1");  
      Assert.AreEqual(p2.GetArea(),XYGeometryTools.CalculateSharedArea(p1,p2),1e-12,"Test4 - Polygon2 in Polygon1");  
      Assert.AreEqual(p1.GetArea(),XYGeometryTools.CalculateSharedArea(p1,p3),1e-12,"Test5 - Polygon1 in Polygon2");  
      Assert.AreEqual(4,XYGeometryTools.CalculateSharedArea(p1,p4),1e-12,"Test6 - Polygon1 in Polygon3");  
    }

    [Test]
    public void Protected_CalculateSharedLength()
    {
        Assert.AreEqual(Math.Sqrt(2),AXYGeometryTools.ACalculateSharedLength(new XYLine(0, 0, 1, 1), new XYLine(0, 0, 1, 1)),"Test1");
        Assert.AreEqual(0,AXYGeometryTools.ACalculateSharedLength(new XYLine(0, 0, 1, 1), new XYLine(10, 10, 11, 11)),"Test2");
        Assert.AreEqual(Math.Sqrt(2)/2,AXYGeometryTools.ACalculateSharedLength(new XYLine(0, 0, 1, 1), new XYLine(0.5, 0.5, 10, 10)),"Test3");
	    Assert.AreEqual(0,AXYGeometryTools.ACalculateSharedLength(new XYLine(1, 1, 1, 3), new XYLine(1, 4, 1, 5)),"Test4 vertical lines");
		Assert.AreEqual(1,AXYGeometryTools.ACalculateSharedLength(new XYLine(1, 1, 1, 3), new XYLine(1, 2, 1, 5)),"Test4");
		Assert.AreEqual(0,AXYGeometryTools.ACalculateSharedLength(new XYLine(1, 1, 1, 3), new XYLine(2, 1, 2, 5)),"Test5");
		Assert.AreEqual(2,AXYGeometryTools.ACalculateSharedLength(new XYLine(7, 3, 10, 3), new XYLine(8, 3, 11, 3)),"Test5");
		Assert.AreEqual(0,AXYGeometryTools.ACalculateSharedLength(new XYLine(7, 3, 10, 3), new XYLine(11, 3, 13, 3)),"Test6");
        Assert.AreEqual(30,AXYGeometryTools.ACalculateSharedLength(new XYLine(20, 40, 20, 0), new XYLine(20, 40, 20, 10)),"Test7");
        Assert.AreEqual(30,AXYGeometryTools.ACalculateSharedLength(new XYLine(20, 40, 20, 10), new XYLine(20, 40, 20, 10)),"Test8");
        Assert.AreEqual(30,AXYGeometryTools.ACalculateSharedLength(new XYLine(20, 10, 20, 40), new XYLine(20, 10, 20, 40)),"Test9");
        Assert.AreEqual(0,AXYGeometryTools.ACalculateSharedLength(new XYLine(10, 40, 20, 40), new XYLine(20, 40, 20, 10)),"Test10");
    }

    [Test]
    public void DoLineSegmentsIntersect()
    {
      // crossing (using three overloaded methods for the same lines)
      Assert.AreEqual(true, XYGeometryTools.DoLineSegmentsIntersect(2,1,5,2,3,4,4,1));
      Assert.AreEqual(true, XYGeometryTools.DoLineSegmentsIntersect(new XYPoint(2,1), new XYPoint(5,2), new XYPoint(3,4), new XYPoint(4,1)));
      Assert.AreEqual(true, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,5,2),new XYLine(3,4,4,1)));
			
      // too short to cross( using three overloaded methods for the same lines)
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(2,1,5,2,3,4,4,2));
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYPoint(2,1), new XYPoint(5,2), new XYPoint(3,4), new XYPoint(4,2)));
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,5,2), new XYLine(3,4,4,2)));

      // Long enough but wrong direction to cross (using three overloaded methods for the same lines)
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(2,1,5,2,3,4,9,1));
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYPoint(2,1), new XYPoint(5,2), new XYPoint(3,4), new XYPoint(9,1)));
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,5,2), new XYLine(3,4,9,1)));

      // parallel lines (using three overloaded methods for the same lines)
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(2,1,4,1,2,2,4,2));
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYPoint(2,1), new XYPoint(4,1), new XYPoint(2,2), new XYPoint(4,2)));
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,4,1), new XYLine(2,2,4,2)));

      // Two identical lines
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,4,1), new XYLine(2,1,4,1))); //horizontal lines
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,10,2,3), new XYLine(2,10,2,3))); // vertical lines
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,2,10,10), new XYLine(2,2,10,10)));


      // Two line on top of each other but with different length
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(0,1,5,1), new XYLine(2,1,4,1))); //Horizontal lines
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,4,1),new XYLine(0,1,5,1))); //Horizontal lines
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,3,4),new XYLine(2,1,3,3))); //Vertical lines
      Assert.AreEqual(false, XYGeometryTools.DoLineSegmentsIntersect(new XYLine(2,1,3,3),new XYLine(2,1,3,4))); //Vertical lines
    }

    [Test]
    public void Protected_IsPointInLine()
    {
      XYPoint point = new XYPoint();
      Assert.AreEqual(true,AXYGeometryTools.AIsPointInLine(new XYPoint(0,0), new XYLine(0, 0, 1, 1)),"Test1");
      Assert.AreEqual(true,AXYGeometryTools.AIsPointInLine(new XYPoint(0.5,0.5), new XYLine(0, 0, 1, 1)),"Test2");
      Assert.AreEqual(true,AXYGeometryTools.AIsPointInLine(new XYPoint(1,1), new XYLine(0, 0, 1, 1)),"Test3");
      Assert.AreEqual(false,AXYGeometryTools.AIsPointInLine(new XYPoint(0.5,0), new XYLine(0, 0, 1, 1)),"Test4");
    }
    
    [Test]
    public void Protected_IsPointInLineInterior()
    {
      XYPoint point = new XYPoint();
      Assert.AreEqual(false,AXYGeometryTools.AIsPointInLineInterior(new XYPoint(0,0), new XYLine(0, 0, 1, 1)),"Test1");
      Assert.AreEqual(true,AXYGeometryTools.AIsPointInLineInterior(new XYPoint(0.5,0.5), new XYLine(0, 0, 1, 1)),"Test2");
      Assert.AreEqual(false,AXYGeometryTools.AIsPointInLineInterior(new XYPoint(1,1), new XYLine(0, 0, 1, 1)),"Test3");
      Assert.AreEqual(false,AXYGeometryTools.AIsPointInLineInterior(new XYPoint(0.5,0), new XYLine(0, 0, 1, 1)),"Test4");
      Assert.AreEqual(false,AXYGeometryTools.AIsPointInLineInterior(new XYPoint(20,40), new XYLine(20, 40, 20, 0)),"Test5");
    }
    
    [Test]
		public void IsPointInPolygon()
		{
			XYPolygon xypolygon = new XYPolygon();
			xypolygon.Points.Add(new XYPoint(1,1));
			xypolygon.Points.Add(new XYPoint(9,1));
			xypolygon.Points.Add(new XYPoint(5,5));
			xypolygon.Points.Add(new XYPoint(5,3));
			xypolygon.Points.Add(new XYPoint(3,3));
			xypolygon.Points.Add(new XYPoint(3,8));
			xypolygon.Points.Add(new XYPoint(9,8));
			xypolygon.Points.Add(new XYPoint(9,11));
			xypolygon.Points.Add(new XYPoint(1,11));

			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(2,2,xypolygon));
			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(2,4,xypolygon));
			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(2,10,xypolygon));
			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(7,10,xypolygon));
			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(4,2,xypolygon));
			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(7,2,xypolygon));
			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(6,3.5,xypolygon));
			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(7.5,2,xypolygon));

			Assert.AreEqual(false, XYGeometryTools.IsPointInPolygon(0,0,xypolygon));
			Assert.AreEqual(false, XYGeometryTools.IsPointInPolygon(4,4,xypolygon));
			Assert.AreEqual(false, XYGeometryTools.IsPointInPolygon(4,5,xypolygon));
			Assert.AreEqual(false, XYGeometryTools.IsPointInPolygon(10,8,xypolygon));
			Assert.AreEqual(false, XYGeometryTools.IsPointInPolygon(9,12,xypolygon));

			Assert.AreEqual(true, XYGeometryTools.IsPointInPolygon(new XYPoint(7.5,2),xypolygon));
			Assert.AreEqual(false, XYGeometryTools.IsPointInPolygon(new XYPoint(0,0),xypolygon));
		}

    [Test]
    public void Protected_IsPointInPolygonOrOnEdge()
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
      Assert.AreEqual(true,  AXYGeometryTools.AIsPointInPolygonOrOnEdge(0,3,p1),"Test1");
      Assert.AreEqual(true,  AXYGeometryTools.AIsPointInPolygonOrOnEdge(1,3,p1),"Test2");
      Assert.AreEqual(false, AXYGeometryTools.AIsPointInPolygonOrOnEdge(1,5,p1),"Test3");
      Assert.AreEqual(true,  AXYGeometryTools.AIsPointInPolygonOrOnEdge(3,2,p1),"Test4");
      Assert.AreEqual(true,  AXYGeometryTools.AIsPointInPolygonOrOnEdge(3,3,p1),"Test5");
      Assert.AreEqual(true,  AXYGeometryTools.AIsPointInPolygonOrOnEdge(6,1,p1),"Test6");
      Assert.AreEqual(false, AXYGeometryTools.AIsPointInPolygonOrOnEdge(6,2,p1),"Test7");
      Assert.AreEqual(false, AXYGeometryTools.AIsPointInPolygonOrOnEdge(6,7,p1),"Test8");
    }
    
		[Test]
		public void Protected_TriangleIntersectionArea()
		{
			XYPolygon t1 = new XYPolygon();
			t1.Points.Add(new XYPoint(0.0, 0.5));
			t1.Points.Add(new XYPoint(6.0, 0.5));
			t1.Points.Add(new XYPoint(1.0, 7.0));

			XYPolygon t2 = new XYPolygon();
      t2.Points.Add(new XYPoint(1,1));
			t2.Points.Add(new XYPoint(5,1));
			t2.Points.Add(new XYPoint(1,5));

			XYPolygon t3 = new XYPolygon();
      t3.Points.Add(new XYPoint(1,1));
			t3.Points.Add(new XYPoint(3,1));
			t3.Points.Add(new XYPoint(1,3));

			XYPolygon t4 = new XYPolygon();
      t4.Points.Add(new XYPoint(1,2));
			t4.Points.Add(new XYPoint(3,2));
			t4.Points.Add(new XYPoint(3,4));

			XYPolygon t5 = new XYPolygon();
      t5.Points.Add(new XYPoint(6.5,3.5));
			t5.Points.Add(new XYPoint(9.5,3.4));
			t5.Points.Add(new XYPoint(7,5));

      XYPolygon t6 = new XYPolygon();
      t6.Points.Add(new XYPoint(-2,0));
      t6.Points.Add(new XYPoint(3,0));
      t6.Points.Add(new XYPoint(3,2));

			//t2 is fully inside t1	
			Assert.AreEqual(8,AXYGeometryTools.ATriangleIntersectionArea(t2,t1),"t2, t1");
      Assert.AreEqual(8,AXYGeometryTools.ATriangleIntersectionArea(t1,t2),"t1, t2");

			// t4 is partly inside t2
			Assert.AreEqual((double)7/ (double) 4, AXYGeometryTools.ATriangleIntersectionArea(t2,t4),"t2, t4");
      Assert.AreEqual((double)7/ (double) 4, AXYGeometryTools.ATriangleIntersectionArea(t4,t2),"t4, t2");

			// t3 is inside t2 but is sharing two edges
    	Assert.AreEqual(2, AXYGeometryTools.ATriangleIntersectionArea(t2,t3),"t2, t3");
    	Assert.AreEqual(2, AXYGeometryTools.ATriangleIntersectionArea(t3,t2),"t3, t2");

			// t1 and t5 has no overlap
			Assert.AreEqual(0, AXYGeometryTools.ATriangleIntersectionArea(t1,t5),"t1, t5");
			Assert.AreEqual(0, AXYGeometryTools.ATriangleIntersectionArea(t5,t1),"t5, t1");

      // two times t6
      Assert.AreEqual(t6.GetArea(), AXYGeometryTools.ATriangleIntersectionArea(t6,t6),"t6, t6");
		}
	}
}
