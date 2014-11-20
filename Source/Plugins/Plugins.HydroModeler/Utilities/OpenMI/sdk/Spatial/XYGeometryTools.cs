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
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Spatial
{
	/// <summary>
	/// The XYGeometryTools class is a collection of general geometry functions. All functions are 
	/// static methods that performs calculations on input given as parameters and returns a result.
	/// 
	/// The parameters passed to the XYGeometryTools methods are typically of type XYPoint, XYLine, 
	/// XYPolyline or XYPolygon.
	/// </summary>
	public class XYGeometryTools
	{
		private const double EPSILON = 1e-5;

		/// <summary>
		/// Returns the distance between the two points.
		/// </summary>
		/// <param name="p1">Point</param>
		/// <param name="p2">Point</param>
		/// <returns>Point to point distance</returns>
		public static double CalculatePointToPointDistance(XYPoint p1, XYPoint p2)
		{
		return Math.Sqrt( (p1.X-p2.X)*(p1.X-p2.X)+(p1.Y -p2.Y )*(p1.Y -p2.Y ) );
		}
		
		/// <summary>
		/// Returns true if two line segments intersects. The lines are said to intersect if the lines
		/// axctually crosses and not if they only share a point. 
		/// </summary>
		/// <param name="x1">x-coordiante for first point in first line segment</param>
		/// <param name="y1">y-coordinate for first point in first line segment </param>
		/// <param name="x2">x-cooedinate for second point in first line segment</param>
		/// <param name="y2">y-coordinate for second point in first line segment</param>
		/// <param name="x3">x-coordinate for the first point in second line segment</param>
		/// <param name="y3">y-coordinate for the first point in second line segment</param>
		/// <param name="x4">x-coordinate for the second point in the second line segment</param>
		/// <param name="y4">y-coordinate for the second point in the second line segment</param>
		/// <returns>True if the line segments intersects otherwise false.</returns>
    public static bool DoLineSegmentsIntersect(double x1, double y1, double x2,  double y2, double x3,	double y3, double x4, double y4)
		{
			double detP1P2P3, detP1P2P4, detP3P4P1, detP3P4P2;
			bool intersect = false;

      detP1P2P3 = (x2 - x1)*(y3 - y1) - (x3 - x1)*(y2 - y1);
			detP1P2P4 = (x2 - x1)*(y4 - y1) - (x4 - x1)*(y2 - y1);
			detP3P4P1 = (x3 - x1)*(y4 - y1) - (x4 - x1)*(y3 - y1); 
			detP3P4P2 = detP1P2P3 - detP1P2P4 + detP3P4P1;

			if ((detP1P2P3 * detP1P2P4 < 0) && (detP3P4P1 * detP3P4P2 < 0))
			{
				intersect = true;
			}
			return intersect;
		}

		/// <summary>
		/// OverLoad of DoLineSegmentsIntersect(x1, y1, x2, y2, x3, y3, x4, y4)
		/// </summary>
		/// <param name="p1">First point in first line</param>
		/// <param name="p2">Second point in first line</param>
		/// <param name="p3">First point in second line</param>
		/// <param name="p4">Second point in second line</param>
		/// <returns>true if the line segmenst intersects otherwise false</returns>
    public static bool DoLineSegmentsIntersect(XYPoint p1, XYPoint p2, XYPoint p3, XYPoint p4)
		{
			return DoLineSegmentsIntersect(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y);
		}

		/// <summary>
		/// OverLoad of DoLineSegmentsIntersect(x1, y1, x2, y2, x3, y3, x4, y4)
		/// </summary>
		/// <param name="line1">First line</param>
		/// <param name="line2">Second line</param>
		/// <returns>true if the line segmenst intersects otherwise false</returns>
    public static bool DoLineSegmentsIntersect(XYLine line1, XYLine line2)
		{
			return DoLineSegmentsIntersect(line1.P1.X, line1.P1.Y, line1.P2.X, line1.P2.Y, line2.P1.X, line2.P1.Y, line2.P2.X, line2.P2.Y);
		}

		/// <summary>
		/// Calculate intersection point between two line segments.
		/// </summary>
		/// <param name="p1">First point in first line</param>
		/// <param name="p2">Second point in first line</param>
		/// <param name="p3">First point in second line</param>
		/// <param name="p4">Second point in second line</param>
		/// <returns>Intersection point</returns>
		public static XYPoint CalculateIntersectionPoint(XYPoint p1, XYPoint p2, XYPoint p3, XYPoint p4)
		{
			if (!DoLineSegmentsIntersect(p1,p2,p3,p4))
			{
				throw new System.Exception("Attempt to calculate intersection point between non intersecting lines. CalculateIntersectionPoint failed.");
			}
			
			XYPoint interSectionPoint = new XYPoint();
			
			double a = p1.X * p2.Y - p2.X * p1.Y;
			double b = p3.X * p4.Y - p4.X * p3.Y;
			double c = (p1.X - p2.X) * (p3.Y - p4.Y) - (p3.X - p4.X) * (p1.Y - p2.Y);

			interSectionPoint.X = (a * (p3.X - p4.X) - (b * (p1.X - p2.X))) / c;
			interSectionPoint.Y = (a * (p3.Y - p4.Y) - (b * (p1.Y - p2.Y))) / c;

			return interSectionPoint;
		}

		/// <summary>
		/// OverLoad of CalculateIntersectionPoint(XYPoint p1, XYPoint p2, XYPoint p3, XYPoint p4)
		/// </summary>
		/// <param name="line1">First line</param>
		/// <param name="line2">Second line</param>
		/// <returns>Intersection point</returns>
		public static XYPoint CalculateIntersectionPoint(XYLine line1, XYLine line2)
		{
			return CalculateIntersectionPoint(line1.P1, line1.P2, line2.P1, line2.P2);
		}

		/// <summary>
		/// Calculates the length of polyline inside polygon. Lines segments on the edges of 
		/// polygons are included with half their length.
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="polygon">Polygon</param>
		/// <returns>
		/// Length of polyline inside polygon.
		/// </returns>
		public static double CalculateLengthOfPolylineInsidePolygon(XYPolyline polyline, XYPolygon polygon)
		{
			double lengthInside = 0;
			int numberOfLineSegments = polyline.Points.Count - 1;
			for (int i = 0; i < numberOfLineSegments; i++)
			{
				XYLine line = new XYLine(polyline.GetLine(i));
				lengthInside += CalculateLengthOfLineInsidePolygon(line,polygon);
			}
			return lengthInside;
		}

    /// <summary>
    /// Calculates the length that two lines overlap.
    /// </summary>
    /// <param name="lineA">Line</param>
    /// <param name="lineB">Line</param>
    /// <returns>
    /// Length of shared line segment.
    /// </returns>
    protected static double CalculateSharedLength(XYLine lineA, XYLine lineB)
    {
		  if ( Math.Abs(lineA.P2.X-lineA.P1.X)<EPSILON && Math.Abs(lineB.P2.X-lineB.P1.X)<EPSILON &&Math.Abs(lineA.P1.X-lineB.P1.X)<EPSILON)
		  {
			  double YP1A = Math.Min(lineA.P1.Y, lineA.P2.Y);
			  double YP2A = Math.Max(lineA.P1.Y, lineA.P2.Y);
			  double YP1B = Math.Min(lineB.P1.Y, lineB.P2.Y);
			  double YP2B = Math.Max(lineB.P1.Y, lineB.P2.Y);

			  double YP1 = Math.Max(YP1A, YP1B);
			  double YP2 = Math.Min(YP2A, YP2B);
			  if (YP1 < YP2) 
			  {
				  return YP2-YP1;
			  }
			  else
			  {
				  return 0;
			  }
		  }
		  else if(Math.Abs(lineA.P2.X-lineA.P1.X)<EPSILON || Math.Abs(lineB.P2.X-lineB.P1.X)<EPSILON)
		  {
			  return 0;
		  }
		  else
		  {
			  XYPoint P1A = new XYPoint();
			  XYPoint P2A = new XYPoint();
			  if (lineA.P1.X < lineA.P2.X)
			  {
				  P1A = lineA.P1;
				  P2A = lineA.P2;
			  }
			  else
			  {
				  P1A = lineA.P2;
				  P2A = lineA.P1;
			  }
			  XYPoint P1B = new XYPoint();
			  XYPoint P2B = new XYPoint();
			  if (lineB.P1.X < lineB.P2.X)
			  {
				  P1B = lineB.P1;
				  P2B = lineB.P2;
			  }
			  else
			  {
				  P1B = lineB.P2;
				  P2B = lineB.P1;
			  }

			  double alphaA = (P2A.Y - P1A.Y)/(P2A.X - P1A.X);
			  double betaA = -alphaA*P2A.X + P2A.Y;
			  double alphaB = (P2B.Y - P1B.Y)/(P2B.X - P1B.X);
			  double betaB = -alphaA*P2B.X + P2B.Y;
			  if (Math.Abs(alphaA-alphaB)<EPSILON && Math.Abs(betaA-betaB)<EPSILON)
			  {
				  double x1 = Math.Max(P1A.X, P1B.X);
				  double x2 = Math.Min(P2A.X, P2B.X);
				  if (x1 < x2)
				  {
					  XYLine line = new XYLine(x1, alphaA*x1+betaA, x2, alphaA*x2+betaA);
					  return line.GetLength();
				  }
				  else
				  {
					  return 0;
				  }
			  }
			  else
			  {
				  return 0;
			  }
		  }
    }

    /// <summary>
    /// Calculates length of line inside polygon. Parts of the line that is on the edge of 
    /// the polygon only counts with half their length.
    /// </summary>
    /// <param name="line">Line</param>
    /// <param name="polygon">Polygon</param>
    /// <returns>
    /// Length of line inside polygon.
    /// </returns>
    protected static double CalculateLengthOfLineInsidePolygon(XYLine line, XYPolygon polygon)
    {
          ArrayList lineList = new ArrayList();
        lineList.Add(new XYLine(line));
          
          for (int i = 0; i < polygon.Points.Count; i++) // For all lines in the polygon
          {
              for (int n = 0; n < lineList.Count; n++)   
              {
                  if (lineList.Count > 1000)
                  {
                      throw new Exception("Problems in ElementMapper, line has been cut in more than 1000 pieces !!!");
                  }

                  if (DoLineSegmentsIntersect((XYLine)lineList[n], polygon.GetLine(i)))
                  {
                      // Split the intersecting line into two lines
                      XYPoint IntersectionPoint = new XYPoint(CalculateIntersectionPoint((XYLine)lineList[n], polygon.GetLine(i)));
                      lineList.Add(new XYLine(IntersectionPoint, ((XYLine) lineList[n]).P2));
                      ((XYLine) lineList[n]).P2.X = IntersectionPoint.X;
                      ((XYLine) lineList[n]).P2.Y = IntersectionPoint.Y;
                      break;
                  }
              }
          }
	
          for (int i = 0; i < lineList.Count; i++)
          {
              if (lineList.Count > 1000)
              {
                  throw new Exception("Problems in ElementMapper, line has been cuttes in more than 100 pieces !!!");
              }
              for (int j = 0; j < polygon.Points.Count; j++)
              {
                  if (IsPointInLineInterior( polygon.GetLine(j).P1, ((XYLine) lineList[i])))
                  {
                      lineList.Add(new XYLine(polygon.GetLine(j).P1, ((XYLine) lineList[i]).P2));
                      ((XYLine) lineList[i]).P2.X = polygon.GetLine(j).P1.X;
                      ((XYLine) lineList[i]).P2.Y = polygon.GetLine(j).P1.Y;
                  }
              }  
          }
   
          double lengthInside = 0;
          for (int i = 0; i < lineList.Count; i++)
		{
              double sharedLength = 0;
              for (int j = 0; j < polygon.Points.Count; j++)
              {
                  sharedLength += CalculateSharedLength(((XYLine) lineList[i]), polygon.GetLine(j));
              }
              if (sharedLength > EPSILON)
              {
                  lengthInside += sharedLength/2;
              }
              else if (IsPointInPolygon(((XYLine) lineList[i]).GetMidpoint(), polygon))
              {
                  lengthInside += ((XYLine) lineList[i]).GetLength();
              }
          }
          return lengthInside;
		}
     	
    /// <summary>
    /// The method calculates the intersection area of triangle a and b both
    /// of type XYPolygon.
    /// </summary>
    /// <param name="triangleA">Triangle of type XYPolygon</param>
    /// <param name="triangleB">Triangle of type XYPolygon</param>
    /// <returns>
    ///	Intersection area between the triangles triangleA and triAngleB.
    /// </returns>
    protected static double TriangleIntersectionArea(XYPolygon triangleA, XYPolygon triangleB)
    {
      try
      {
        if (triangleA.Points.Count != 3 || triangleB.Points.Count != 3)
        {
          throw new System.Exception("Argument must be a polygon with 3 points");
        }
        int i = 1;       // Index for "next" node in polygon a.
        int j = -1;      // Index for "next" node in polygon b. 
                         // -1 indicates that the first has not yet been found.
        double area = 0; // Intersection area. Returned.
        XYPolygon intersectionPolygon = new XYPolygon(); // Intersection polygon.
        XYPoint pFirst = new XYPoint(); // First intersection point between triangles
        XYPoint p = new XYPoint(); // Latest intersection node found

        p.X = ((XYPoint) triangleA.Points[0]).X;
        p.Y = ((XYPoint) triangleA.Points[0]).Y;
        Intersect(triangleA, triangleB, ref p, ref i, ref j, ref intersectionPolygon); 
        pFirst = p;

        if (j != -1)
        { 
          int jStop = Increase(j, 2);
          bool complete = false;
          int count = 0;
          while (!complete)
          {
            // coordinates for vectors pointing to next triangleA and triangleB point respectively
            double vax= ((XYPoint) triangleA.Points[i]).X - p.X;
            double vay= ((XYPoint) triangleA.Points[i]).Y - p.Y;
            double vbx= ((XYPoint) triangleB.Points[j]).X - p.X;
            double vby= ((XYPoint) triangleB.Points[j]).Y - p.Y;

            if(IsPointInPolygonOrOnEdge(p.X + EPSILON*vax, p.Y + EPSILON*vay, triangleB))
            {
              Intersect(triangleA, triangleB, ref p, ref i, ref j, ref intersectionPolygon);
            }
            else if(IsPointInPolygonOrOnEdge(p.X + EPSILON*vbx, p.Y + EPSILON*vby, triangleA))
            {
              Intersect(triangleB, triangleA, ref p, ref j, ref i, ref intersectionPolygon);
            }
            else // triangleA and triangleB only touches one another but do not intersect
            {
              area = 0;
              return area;
            }
            if (intersectionPolygon.Points.Count > 1)
            {
              complete = (CalculatePointToPointDistance(p, pFirst) < EPSILON);
            }
            count++;
            if ( count > 20 )
            {
              throw new System.Exception("Failed to find intersection polygon");
            }
          }
          area = intersectionPolygon.GetArea();
        }
        else
        {
          XYPoint pa = new XYPoint(); // internal point in triangle a
          XYPoint pb = new XYPoint(); // internal point in triangle b

          pa.X = (triangleA.GetX(0)+triangleA.GetX(1)+triangleA.GetX(2))/3;
          pa.Y = (triangleA.GetY(0)+triangleA.GetY(1)+triangleA.GetY(2))/3;
          pb.X = (triangleB.GetX(0)+triangleB.GetX(1)+triangleB.GetX(2))/3;
          pb.Y = (triangleB.GetY(0)+triangleB.GetY(1)+triangleB.GetY(2))/3;

          if (IsPointInPolygon(pa,triangleB) || IsPointInPolygon(pb,triangleA)) // triangleA is completely inside triangleB
          {
            area = Math.Min(triangleA.GetArea(),triangleB.GetArea());
          }
          else // triangleA and triangleB do dot intersect
          {
            area = 0;  
          }
        }
        return area;
      }
      catch (System.Exception e)
      {
        throw new System.Exception("TriangleIntersectionArea failed",e);
      }
    }

    /// <summary>
    /// The method calculates the intersection points of triangle a and b both
    /// of type XYPolygon.
    /// </summary>
    /// <param name="triangleA">triangle. The search is started along triangleA.</param>
    /// <param name="triangleB">triangle. Intersection with this triangle are sought.</param>
    /// <param name="p">Starting point for the search. p must be part of triangleA.</param>
    /// <param name="i">on input: End index for the first line segment of triangleA in the search.
    /// on output: End index for the last intersected line segment in triangleA.</param>
    /// <param name="j">on input: -1 if vertices before intersection is not to be added to list.
    /// on output: End index for last intersected line segment of triangleB.</param>
    /// <param name="intersectionPolygon">polygon eventuallu describing the 
    /// intersection area between triangleA and triangleB</param>
    /// <returns>
    ///	The p, i, j and intersectionPolygon are called by reference and modified in the method.
    /// </returns>
    private static void Intersect (XYPolygon triangleA, XYPolygon triangleB, 
                                   ref XYPoint p, ref  int i, ref int j, 
                                   ref XYPolygon intersectionPolygon)
    {
      XYLine lineA;
      XYLine lineB;
      int im1 = Decrease(i, 2); // "i-1"
      int count1 = 0;
      bool found = false;

      while ((count1 < 3) && (!found))
      {
        lineA = triangleA.GetLine(im1);
        if (count1 == 0)
        {
          lineA.P1.X = p.X;
          lineA.P1.Y = p.Y;
        }
        double MinDist = -1; // Distance used when a line is crossed more than once
        int jm1 = 0;         // "j-1"
        int jm1Store = -1;
        while (jm1 < 3)
        {
          lineB = triangleB.GetLine(jm1);
          found = IntersectionPoint(lineA, lineB, ref p);
          double Dist = CalculatePointToPointDistance(lineA.P1,p);
          if (Dist < EPSILON)
          {
            found = false;
          }
          if (found)
          {
            if ((MinDist < 0) || (Dist < MinDist))
            {
              MinDist = Dist;
              jm1Store = jm1;
            }
          }
          jm1++;
        }
        if ( jm1Store > -1 )
        {
          lineB = triangleB.GetLine(jm1Store);
          found = IntersectionPoint(lineA, lineB, ref p);          
		      XYPoint HelpCoordinate = new XYPoint(p.X, p.Y);
		      XYPoint HelpNode = new XYPoint(HelpCoordinate);

		      intersectionPolygon.Points.Add(HelpNode);

          j = Increase(jm1Store,2);  
        }
        if (!found)
        {
          count1++;
          im1 = Increase(im1,2);
          i = Increase(i,2);
          if (j!=-1) 
          {
		        XYPoint HelpCoordinate = new XYPoint(lineA.P2.X, lineA.P2.Y);
       			XYPoint HelpNode = new XYPoint(HelpCoordinate);
       			intersectionPolygon.Points.Add(HelpNode);
          }
        }
      }
      lineA = triangleA.GetLine(Decrease(i, 2));
      if ( CalculatePointToPointDistance(p, lineA.P2)<EPSILON )
      {
        i = Increase(i, 2);
      }
      lineB = triangleB.GetLine(Decrease(j, 2));
      if ( CalculatePointToPointDistance(p, lineB.P2)<EPSILON )
      {
        j = Increase(j, 2);
      }
    }
    
    /// <summary>
    /// The method steps to the next index in a circular list 0, 1 ..., n.
    /// </summary>
    /// <param name="i">Index to increase.</param>
    /// <param name="n">Largest index</param>
    /// <returns>
    ///	<p>The increased index.</p>
    /// </returns>
    private static int Increase(int i, int n)
    {
      i++;
      if (i>n)
      {
        i = 0;
      }
      return i;
    }

    /// <summary>
    /// The method steps to the previous index in a circular list 0, 1 ..., n.
    /// </summary>
    /// <param name="i">Index to decrease.</param>
    /// <param name="n">Largest index</param>
    /// <returns>
    ///	<p>The decreased index.</p>
    /// </returns>
    private static int Decrease(int i, int n)
    {
      i--;
      if (i < 0)
      {
        i = n;
      }
      return i;
    }

    /// <summary>
    /// Checks if the lines lineA and lineB shares a point either as a real 
    /// crossing point or as a shared end point or a end point of the one 
    /// line being in the other line.
    /// </summary>
    /// <param name="Linea">Line.</param>
    /// <param name="Lineb">Line.</param>
    /// <param name="intersectionPoint">Point.</param>
    /// <returns>
    ///	<p>True if lineA and lineB has shared point. False otherwise</p>
    ///	<p>The shared point if any is returned in the intersectionPoint 
    ///	parameter that is called by reference</p>
    /// </returns>
    protected static bool IntersectionPoint(XYLine Linea, XYLine Lineb, ref XYPoint intersectionPoint)
    {
      if( DoLineSegmentsIntersect(Linea, Lineb))
      {
        intersectionPoint = CalculateIntersectionPoint(Linea, Lineb);
        return true;
      }
      if( IsPointInLine(Linea.P2, Lineb))
      {
        intersectionPoint = Linea.P2;
        return true;
      }
      if( IsPointInLine(Lineb.P2, Linea))
      {
        intersectionPoint = Lineb.P2;
        return true;
      }
      if( IsPointInLine(Lineb.P1, Linea))
      {
        intersectionPoint = Lineb.P1;
        return true;
      }
      if( IsPointInLine(Linea.P1, Lineb))
      {
        intersectionPoint = Linea.P1;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Determines if a point is included in a line either in the interior 
    /// or as one of the end points.
    /// </summary>
    /// <param name="x">x-coordinate</param>
    /// <param name="y">y-coordinate</param>
    /// <param name="line">Line.</param>
    /// <returns>
    ///	<p>Determines if a point is included in a line.</p>
    /// </returns>
    protected static bool IsPointInLine(double x, double y, XYLine line)
    {
      bool result = false;
      if( line.P1.X-line.P2.X != 0 )
      {
        if ((x >= Math.Min(line.P1.X, line.P2.X)) && (x <= Math.Max(line.P1.X, line.P2.X)))
        {
          if( Math.Abs(y-line.P1.Y-(line.P2.Y-line.P1.Y)/(line.P1.X-line.P2.X)*(line.P1.X-x)) < EPSILON*EPSILON)
          {
            result = true;
          }
        }
      }
      else
      {
        if (line.P1.X == x)
        {
          if ( (y >= Math.Min(line.P1.Y, line.P2.Y)) && (y <= Math.Max(line.P1.Y, line.P2.Y)) )
          {
            result = true;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Determines if a point is included in a line either in the interior 
    /// or as one of the end points.
    /// <p>Overload to: IsPointInLine(double x, double y, XYLine line)</p>
    /// </summary>
    /// 
    /// <param name="point">Point</param>
    /// <param name="line">Line.</param>
    /// 
    /// <returns>
    ///	<p>Determines if a point is included in a line.</p>
    /// </returns>
    protected static bool IsPointInLine(XYPoint point, XYLine line)
    {
      return IsPointInLine( point.X, point.Y, line);
    }

    /// <summary>
    /// Determines if a point is included in a lines interior. I.e. included 
    /// in the line and not an endpoint. 
    /// </summary>
    /// <param name="x">x-coordinate</param>
    /// <param name="y">y-coordinate</param>
    /// <param name="line">Line.</param>
    /// <returns>
    ///	<p>Determines if a point is included in a line.</p>
    /// </returns>
    protected static bool IsPointInLineInterior(double x, double y, XYLine line)
    {
      bool result = false;
      if( line.P1.X-line.P2.X != 0 )  //line is not vertical
      {
        if ((x > Math.Min(line.P1.X, line.P2.X)) && (x < Math.Max(line.P1.X, line.P2.X)))
        {
          if( Math.Abs(y-line.P1.Y-(line.P2.Y-line.P1.Y)/(line.P1.X-line.P2.X)*(line.P1.X-x)) < EPSILON*EPSILON)
          {
            result = true;
          }
        }
      }
      else  //line is vertical
      {
        if (line.P1.X == x)
        {
          if ( (y > Math.Min(line.P1.Y, line.P2.Y)) && (y < Math.Max(line.P1.Y, line.P2.Y)) )
          {
            result = true;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Determines if a point is included in a lines interior. I.e. included 
    /// in the line and not an endpoint. 
    /// <p>Overload to:IsPointInLineInterior(double x, double y, XYLine line)</p>
    /// </summary>
    /// <param name="point">Point.</param>
    /// <param name="line">Line.</param>
    /// <returns>
    ///	<p>Determines if a point is included in a line.</p>
    /// </returns>
    protected static bool IsPointInLineInterior(XYPoint point, XYLine line)
    {
      return IsPointInLineInterior( point.X, point.Y, line);
    }

    /// <summary>
    /// Calculates the distance from a polyline to a point in the plane. 
    /// The algorithm decides weather the point lies besides the line 
    /// segment in which case the distance is the length along a line 
    /// perpendicular to the line. Alternatively the distance is the 
    /// smallest of the distances to either endpoint.
    /// </summary>
    /// <param name="line">Line</param>
    /// <param name="point">Point</param>
    /// <returns>
    ///	<p>Length of the shortest path between the line and the point.</p>
    /// </returns>
    protected static double CalculateLineToPointDistance (XYLine line, XYPoint point)
    {
      double dist = 0;
      double a = Math.Sqrt((line.P2.X-point.X)*(line.P2.X-point.X) + (line.P2.Y-point.Y)*(line.P2.Y-point.Y));
      double b = Math.Sqrt((line.P2.X-line.P1.X)*(line.P2.X-line.P1.X)+(line.P2.Y-line.P1.Y)*(line.P2.Y-line.P1.Y));
      double c = Math.Sqrt((line.P1.X-point.X)*(line.P1.X-point.X)+(line.P1.Y-point.Y)*(line.P1.Y-point.Y));
      if ((a == 0) || (c == 0))
      {
        dist = 0;
      }
      else if (b == 0)
      {
        dist = a;
      }
      else
      {
        double alpha = Math.Acos((b*b+c*c-a*a)/(2*b*c));
        double beta = Math.Acos((a*a+b*b-c*c)/(2*a*b));
        if (Math.Max(alpha,beta)<Math.PI/2)
        {
          dist = Math.Abs((line.P2.X-line.P1.X)*(line.P1.Y-point.Y)-(line.P1.X-point.X)*(line.P2.Y-line.P1.Y))/b;
        }
        else
        {
          dist = Math.Min(a, c);
        }
      }
      return dist;
    }

    /// <summary>
    /// Finds the shortest distance between any line segment of the polyline 
    /// and the point.
    /// </summary>
    /// <param name="polyLine">PolyLine.</param>
    /// <param name="point">Point</param>
    /// <returns>
    ///	<p>Length of the shortest path between the polyline and the point.</p>
    /// </returns>
    public static double CalculatePolylineToPointDistance (XYPolyline polyLine, XYPoint point)
    {
      double dist = 0;
      int i = 0;
      while (i < polyLine.Points.Count - 1) 
      {
        if (i == 0) 
        {
          dist = CalculateLineToPointDistance (polyLine.GetLine(0), point);
        }
        else
        {
          dist = Math.Min(dist, CalculateLineToPointDistance (polyLine.GetLine(i), point));
        }
        i++;
      }
      return dist;
    }

    /// <summary>
    /// Determines if a point in inside or outside a polygon.
    /// Works for both convex and concave polygons (Winding number test)
    /// </summary>
    /// <param name="point">Point</param>
    /// <param name="polygon">Polygon</param>
    /// <returns>
    ///	<p>true:  If the point is inside the polygon</p>
    ///	<p>false: Otherwise.</p>
    /// </returns>
    public static bool IsPointInPolygon(XYPoint point, XYPolygon polygon)
    {
		  return IsPointInPolygon(point.X, point.Y, polygon);
    }  

    /// <summary>
    /// Determines if a point in inside or outside a polygon.
    /// Works for both convex and concave polygons (Winding number test)
    /// </summary>
    /// <param name="x">x-coordinate for the point</param>
    /// <param name="y">y-coordiante for the point</param>
    /// <param name="polygon">Polygon</param>
    /// <returns>
    ///	<p>true:  If the point is inside the polygon</p>
    ///	<p>false: If the point is outside the polygon.</p>
    /// </returns>
    public static bool IsPointInPolygon(double x, double y, XYPolygon polygon)
    {
        double x1,x2,y1,y2;
        double xinters;
        bool isInside = false;
        int  n = polygon.Points.Count;

        for (int i = 0; i < n; i++)
        {
          if (i < n - 1)
          {
			x1 = ((XYPoint)polygon.Points[i]).X;
			x2 = ((XYPoint)polygon.Points[i+1]).X;
			y1 = ((XYPoint)polygon.Points[i]).Y;
		    y2 = ((XYPoint)polygon.Points[i+1]).Y;
        }
        else
        {
			x1 = ((XYPoint)polygon.Points[n-1]).X;
			x2 = ((XYPoint)polygon.Points[0]).X;
			y1 = ((XYPoint)polygon.Points[n-1]).Y;
			y2 = ((XYPoint)polygon.Points[0]).Y;
        }

        if (y > Math.Min(y1,y2))
        {
          if (y <= Math.Max(y1,y2))
          {
            if ( x <= Math.Max(x1,x2))
            {
              if (y1 != y2)
              {
                xinters = (y - y1)*(x2 - x1)/(y2 - y1) + x1;
								
                if (x1 == x2 || x <= xinters)
                {
                  isInside = !isInside;
                }
              }
            }
          }
        }
      }
      return isInside;
    }
    
    /// <summary>
    /// Determines if a point in inside or outside a polygon. Inside
    /// includes on the edge for this method.
    /// Works for both convex and concave polygons (Winding number test)
    /// </summary>
    /// <param name="x">x-coordinate for the point</param>
    /// <param name="y">y-coordiante for the point</param>
    /// <param name="polygon">Polygon</param>
    /// <returns>
    ///	<p>true:  If the point is inside the polygon</p>
    ///	<p>false: If the point is outside the polygon.</p>
    /// </returns>
    protected static bool IsPointInPolygonOrOnEdge(double x, double y, XYPolygon polygon)
    {
      bool result = IsPointInPolygon(x, y, polygon);
      if( result )
      {
        return result;
      }
      else
      {
        int iLine = 0;
        while( (!result) && (iLine < polygon.Points.Count) )
        {
          XYLine line = new XYLine();
          line = polygon.GetLine(iLine);
          result = IsPointInLine(x, y, line);
          iLine++;
        }
      }
      return result;
    }
    
    /// <summary>
    /// The methods calculates the shared area of two arbitrarily shaped 
    /// polygons.
    /// </summary>
    /// <param name="polygonA">Polygon</param>
    /// <param name="polygonB">Polygon</param>
    /// <returns>
    ///	<p>The shared area.</p>
    /// </returns>
    public static double CalculateSharedArea (XYPolygon polygonA, XYPolygon polygonB)
    {
  	  ArrayList triangleListA = polygonA.GetTriangulation();
      ArrayList triangleListB = polygonB.GetTriangulation();
      
      double area = 0;
      for (int ia = 0; ia < triangleListA.Count; ia++)
      {
  	    XYPolygon triangleA = new XYPolygon((XYPolygon)triangleListA[ia]);
        for (int ib = 0; ib < triangleListB.Count; ib++)
        {
    		  XYPolygon triangleB = new XYPolygon((XYPolygon)triangleListB[ib]);
          area = area + TriangleIntersectionArea(triangleA, triangleB);
        }
      }
      return area;
    }      
  }
}
