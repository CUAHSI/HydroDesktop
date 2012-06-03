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
	/// XYPolyline is a collection of points (at least 2) connected with straigth lines.
	/// Polylines are typically used for presentation of 1D data, river networks e.t.c.
	/// </summary>
  public class XYPolyline
  {
    private ArrayList _points;


    /// <summary>
    /// Constructor.
    /// </summary>
    public XYPolyline()
	  {
		  _points = new ArrayList();
	  }

    /// <summary>
    /// Constructor. Copies the contents of the xyPolyline parameter.
    /// </summary>
    /// <param name="xyPolyline">Polyline to copy.</param>
    /// <returns>None</returns>
    public XYPolyline(XYPolyline xyPolyline)
	  {
		  _points = new ArrayList();

		  foreach (XYPoint xypoint in xyPolyline.Points)
		  {
			  _points.Add(new XYPoint(xypoint.X, xypoint.Y));
		  }

	  }

    /// <summary>
    /// Read only property holding the list of points.
    /// </summary>
    public ArrayList Points
	  {
		  get
		  {
			  return _points;
		  }
	  }

    /// <summary>
    /// Retrieves the x-coordinate of the index´th line point.
    /// </summary>
    /// <param name="index">Index number of the point.</param>
    /// <returns>X-coordinate of the index´th point in the polyline.</returns>
    public double GetX(int index)
	  {
		  return ((XYPoint) _points[index]).X;
	  }

    /// <summary>
    /// Retrieves the y-coordinate of the index´th line point.
    /// </summary>
    /// <param name="index">Index number of the point.</param>
    /// <returns>Y-coordinate of the index´th point in the polyline.</returns>
    public double GetY(int index)
	  {
	    return ((XYPoint) _points[index]).Y;
	  }
    
    /// <summary>
    /// Retrieves the lineNumber´th line segment of the polyline. The index 
    /// list is zero based.
    /// </summary>
    /// <param name="lineNumber">Index number of the line to retrieve.</param>
    /// <returns>The lineNumber´th line segment of the polyline.</returns>
    public XYLine GetLine(int lineNumber)
    {
 		  return new XYLine((XYPoint)_points[lineNumber], (XYPoint)_points[lineNumber+1]);
    }

    /// <summary>
    /// Calculates the length of the polyline.
    /// </summary>
    /// <returns>Length of the polyline.</returns>
    public double GetLength()
    {
		  double length = 0;	
		  for (int i = 0; i < _points.Count - 1; i++)
		  {
			  length += GetLine(i).GetLength();
  		}	
      return length;  
    }

    /// <summary>
    /// Compares the object type and the coordinates of the object and the 
    /// object passed as parameter.
    /// </summary>
    /// <returns>True if object type is XYPolyline and the coordinates are 
    /// equal to to the coordinates of the current object. False otherwise.</returns>
    public override bool Equals(Object obj) 
    {
      if (obj == null || GetType() != obj.GetType()) 
      {
        return false;
      }
      XYPolyline e = (XYPolyline) obj;
      if (_points.Count!=e.Points.Count)
      {  
        return false;
      }
      for (int i=0;i<_points.Count;i++)
      {
        if (!((XYPoint) _points[i]).Equals(e.Points[i]))
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
    ///   - is number of points >= 2
    ///   - is the length of all line segments positiv
    /// Exception is raised if the constraints are not met.
    /// </summary>
    public void Validate()
    {
      if(_points.Count < 2)
      {
        throw new System.Exception("Number of vertices in polyline element is less than 2.");
      }
      for (int j = 0; j < _points.Count-1; j++)
      {
        if (GetLine(j).GetLength() == 0)
        {
          throw new System.Exception("Length of line segment no: "+
            j.ToString()+" (0-based) of XYPolyline is zero.");
        }
      }

    }
  }
}
