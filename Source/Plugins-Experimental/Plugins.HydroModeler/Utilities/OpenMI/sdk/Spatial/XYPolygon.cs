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

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// <p>The XYPolygon class defines a polygon in the XY plane (no z coordinate)</p>
  /// <p></p>
  /// <p>The XYPolygon class has a number of usefull methods and XYPolygon objects
  ///  are used as argument in a number of the methods in the Oatc.OpenMI.Sdk.Spatial
  ///  namespace.</p>
  /// <p></p>
  /// <p>The XYPolygon class is inhereted from the org.OpenMI.Utilities.BackBone.Element class.</p>
  /// <p></p>
  /// <p>It is possible to upcaste the XYPoint objects to Element objects.</p>
  /// <p></p>
  /// <p>If you already have a Element object that defines a polygon you can construct
  /// a new XYPoint using the Element objects as argument in the constructor.</p>
  /// 
  /// </summary>
  public class XYPolygon : XYPolyline
	{	
    /// <summary>
    /// Constructor.
    /// </summary>
	  public XYPolygon()
	  {
	  }	
	  
    /// <summary>
    /// Constructor. Copies the contents of the xyPolygon parameter.
    /// </summary>
    /// <param name="xyPolygon">Polygon to copy.</param>
    /// <returns>None</returns>
    public XYPolygon(XYPolygon xyPolygon)
		{
		//	Points = new ArrayList();
			foreach (XYPoint xypoint in xyPolygon.Points)
			{
				Points.Add(new XYPoint(xypoint.X, xypoint.Y));
			}
		}

    /// <summary>
    /// Calcualtes area of polygon. 
    /// </summary>
    /// <returns>Polygon area.</returns>
    public double GetArea()
		{
			double x1, x2, y1, y2, xN, x0, yN, y0, area;
			area = 0;
			for (int i = 0; i < Points.Count - 1; i++)
			{
				x1 = ((XYPoint) Points[i]).X;
				x2 = ((XYPoint) Points[i+1]).X;
				y1 = ((XYPoint) Points[i]).Y;
				y2 = ((XYPoint) Points[i+1]).Y;
				area += x1*y2 - x2*y1;
			}
			xN = ((XYPoint) Points[Points.Count - 1]).X;
			x0 = ((XYPoint) Points[0]).X;
			yN = ((XYPoint) Points[Points.Count - 1]).Y;
			y0 = ((XYPoint) Points[0]).Y;

			area += xN * y0 - x0 * yN;
			area = 0.5 * area;

			return area;
		}

		/// <summary>
		/// Returns the XYline that connects XYPoint LineNumber and the next 
		/// number (i.e. LineNumber+1 or 0).</summary>
		/// <param name="lineNumber">0-based line number.</param>
		/// <returns>
		/// The XYLine starting at node lineNumber.
		/// </returns>
		public new XYLine GetLine(int lineNumber)
		{
			int index2;

			if (lineNumber == Points.Count - 1)
			{
				index2 = 0;
			}
			else
			{
				index2 = lineNumber + 1;
			}
			return new XYLine(((XYPoint) Points[lineNumber]).X, ((XYPoint) Points[lineNumber]).Y, ((XYPoint) Points[index2]).X, ((XYPoint) Points[index2]).Y);
		}
    
    /// <summary>
    /// Finds a set of three concecutive points that form a triangle, that 
    /// is not intersected by other parts of the polygon.
    /// </summary>
    /// <param></param>
    /// <returns>
    ///	<p>i: Index for the "midlle" point of triangle that forms an ear. 
    ///	The ear is formed by P(i-1), P(i) and P(i+1), where P are points 
    ///	included in the polygon.</p>
    /// </returns>
    protected int FindEar()
    {
      int i = 0;
      int n = Points.Count - 1;
      bool found = false;
      while ((i < n-1) && (!found))
      {
        if ((IsConvex(i) == true) && (IsIntersected(i) == false))
        {
          found = true;
        }
        else
        {
          i++;
        }
      }
      return i;
    }      

    /// <summary>
    /// The method decides if the triangle formed by  P(i-1), P(i) and 
    /// P(i+1) from Polygon are intersected by any of the other points 
    /// of the polygon.
    /// </summary>
    /// <param name="i">Middle index for the three points that forms the triangle</param>
    /// <returns>
    ///	<p>true: If the triangle P(i-1), P(i), P(i+1) is intersected by other parts of Polygon</p>
    ///	<p>false: otherwise</p>
    /// </returns>
    protected bool IsIntersected(int i)
    {
      double x = 0;
      double y = 0;
      int n = Points.Count;
      
      int im1 = i-1;
      int ip1 = i+1;
      if (i == 0)
      {
        im1 = n-1;
      }
      else if (i == n-1)
      {
        ip1 = 0;
      }
      
      XYPoint nodeim1 = new XYPoint((XYPoint) Points[im1]);
      XYPoint nodei   = new XYPoint((XYPoint) Points[i]);
      XYPoint nodeip1 = new XYPoint((XYPoint) Points[ip1]);
      XYPolygon localPolygon = new XYPolygon();
      localPolygon.Points.Add(nodeim1);
      localPolygon.Points.Add(nodei);
      localPolygon.Points.Add(nodeip1);

      int j = 0;
      bool intersected = false;
      while (((j < n-1) && (!intersected)))
      {
        x = ((XYPoint) Points[j]).X;
        y = ((XYPoint) Points[j]).Y;

        if (((((j!=im1) && (j!=i)) && (j!=ip1)) && XYGeometryTools.IsPointInPolygon(x,y,localPolygon)))
        {
          return true;
        }
        else
        {
          j++;
        }
      }
      return false;
    }      

    /// <summary>
    /// Returns an ArrayList of triangles of type XYPolygon describing the 
    /// triangalation of the polygon.
    /// </summary>
    /// <param></param>
    /// <returns>
    /// A triangulation of the polygon.
    /// </returns>
    public ArrayList GetTriangulation()
    {
      int i = 0;
      int im1 = 0;
      int ip1 = 0;
      int n = 0;
      
      XYPolygon LocalPolygon = new XYPolygon(this);
  	  ArrayList TriangleList = new ArrayList();
      while (LocalPolygon.Points.Count > 3)
      {
        i = LocalPolygon.FindEar();
        n = LocalPolygon.Points.Count;
        im1 = i-1;
        ip1 = i+1;
        if (i == 0)
        {
          im1 = n-1;
        }
        else if (i == n-1)
        {
          ip1 = 0;
        }  
     		XYPoint Nodeim1 = new XYPoint((XYPoint)LocalPolygon.Points[im1]);
    		XYPoint Nodei   = new XYPoint((XYPoint)LocalPolygon.Points[i]);
    		XYPoint Nodeip1 = new XYPoint((XYPoint)LocalPolygon.Points[ip1]);    
        XYPolygon Triangle = new XYPolygon();
    		Triangle.Points.Add(Nodeim1);
    		Triangle.Points.Add(Nodei);
    		Triangle.Points.Add(Nodeip1);
     		TriangleList.Add(Triangle);
     		LocalPolygon.Points.RemoveAt(i);
      }
      TriangleList.Add(LocalPolygon);
      return TriangleList;
    }

    /// <summary>
    /// Decides if the angle at i´th point is convex or concave.
    /// </summary>
    /// <param name="pointIndex">Index</param>
    /// <returns>
    /// <p>True if angle at the i´th point is convex.</p>
    /// <p>False if angle at the i´th point is concave.</p>
    /// </returns>
    protected bool IsConvex(int pointIndex)
    {
      bool   isConvex = true;
      int    im1      = pointIndex - 1 < 0 ? Points.Count - 1 : pointIndex - 1;  //previous point index
      int    ip1      = pointIndex + 1 > Points.Count - 1 ? 0 : pointIndex + 1;  //next point index

     	double xim1     = ((XYPoint) Points[im1]).X;
   		double yim1     = ((XYPoint) Points[im1]).Y;

		  double xi       = ((XYPoint) Points[pointIndex]).X;
		  double yi       = ((XYPoint) Points[pointIndex]).Y;
		
		  double xip1     = ((XYPoint) Points[ip1]).X;
		  double yip1     = ((XYPoint) Points[ip1]).Y;
			
      if ((xip1-xim1)*(yi-yim1)-(xi-xim1)*(yip1-yim1)>0)
      {
        isConvex = false;
      }
      else
	    {
		    isConvex = true;
      }
      return isConvex;
    }
    
    /// <summary>
    /// Compares the object type and the coordinates of the object and the 
    /// object passed as parameter.
    /// </summary>
    /// <returns>True if object type is XYPolygon and the coordinates are 
    /// equal to to the coordinates of the current object. False otherwise.</returns>
    public override bool Equals(Object obj) 
    {
      if (obj == null || GetType() != obj.GetType()) 
      {
        return false;
      }
      XYPolyline e = (XYPolygon) obj;
      if (Points.Count!=e.Points.Count)
      {  
        return false;
      }
      for (int i=0;i<Points.Count;i++)
      {
        if (!((XYPoint) Points[i]).Equals(e.Points[i]))
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Get hash code.
    /// </summary>
    /// <returns>Hash Code for the current instance.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// The validate method check if the XYPolyline is valid. The checks 
    /// made are: 
    ///   - is number of points >= 3
    ///   - is the length of all line segments positiv
    ///   - do any lines cross
    ///   - is the area positiv
    /// Exception is raised if the constraints are not met.  
    /// </summary>
    public new void Validate()
    {
      if(Points.Count < 3)
      {
        throw new System.Exception("Number of vertices in polygon element is less than 3.");
      }
      if (GetArea() <= 0)
      {
        throw new System.Exception("Area of polygon is negative or zero. XYPolygons must be ordered counter clockwise.");
      }
      for (int j = 0; j < Points.Count; j++)
      {
        if (GetLine(j).GetLength() == 0)
        {
          throw new System.Exception("Length of line segment no: "+
            j.ToString()+" (0-based) of XYPolygon is zero.");
        }
      }
      for (int j = 0; j < Points.Count; j++)
      {
        for (int m = 0; m < j; m++)
        {
          if (XYGeometryTools.DoLineSegmentsIntersect(GetLine(j),GetLine(m)))
          {
            throw new System.Exception("Line no: "+j.ToString()+" and line no: "+
              m.ToString()+" of XYPolygon crosses.");
          }
        }
      }
    }
  }
}
