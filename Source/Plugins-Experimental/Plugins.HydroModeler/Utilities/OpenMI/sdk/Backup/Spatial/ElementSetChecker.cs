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
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Spatial
{
	/// <summary>
	/// Summary description for ElementSetChecker.
	/// </summary>
	public class ElementSetChecker
	{
    /// <summary>
    /// Static method that validates an object with an IElementSet interface. The method 
    /// raises an Exception in case IElementSet does not describe a valid ElementSet.
    /// The checks made are:
    ///   <p>ElementType: Check</p>
    ///   <p>XYPoint:     Only one vertex in each element.</p>
    ///   <p>XYPolyline:  At least two vertices in each element.</p>
    ///   <p>             All line segments in each element has length > 0</p>
    ///   <p>XYPolygon:   At least three vertices in each element.</p>
    ///   <p>             Area of each element is larger than 0</p>
    ///   <p>             All line segments in each element has length > 0</p>
    ///   <p>             No line segments within an element crosses.</p>
    /// </summary>
    /// 
    /// <param name="elementSet">Object that implement the IElementSet interface</param>
    /// 
    /// <returns>
    /// The method has no return value.
    /// </returns>
    public static void CheckElementSet(IElementSet elementSet)
		{
      try
      {
        if(elementSet.ElementType == ElementType.XYPoint)
        {
          for (int i = 0; i < elementSet.ElementCount; i++)
          {
            try
            {              
              if(elementSet.GetVertexCount(i) != 1)
              {
                throw new System.Exception("Number of vertices in point element is different from 1.");
              }
            }
            catch (System.Exception e)
            {
              throw new System.Exception("ElementID = "+elementSet.GetElementID(i),e);
            }
          }
        }
        else if(elementSet.ElementType == ElementType.XYPolyLine)
        {
          for (int i = 0; i < elementSet.ElementCount; i++)
          {
            try
            {
              XYPolyline xypolyline = new XYPolyline();
              for (int j = 0; j < elementSet.GetVertexCount(i); j++)
              {
                XYPoint xypoint = new XYPoint(elementSet.GetXCoordinate(i,j),elementSet.GetYCoordinate(i,j));
                xypolyline.Points.Add(xypoint);
              }
              xypolyline.Validate();
            }
            catch (System.Exception e)
            {
              throw new System.Exception("ElementID = "+elementSet.GetElementID(i),e);
            }
          }
        }
        else if(elementSet.ElementType == ElementType.XYPolygon)
        {
          for (int i = 0; i < elementSet.ElementCount; i++)
          {
            try
            {
              XYPolygon xypolygon = new XYPolygon();
              for (int j = 0; j < elementSet.GetVertexCount(i); j++)
              {
                XYPoint xypoint = new XYPoint(elementSet.GetXCoordinate(i,j),elementSet.GetYCoordinate(i,j));
                xypolygon.Points.Add(xypoint);
              }
              xypolygon.Validate();
            }         
            catch (System.Exception e)
            {
              throw new System.Exception("ElementID = "+elementSet.GetElementID(i),e);
            }
          }
        }
      }
      catch (System.Exception e)
      {
        throw new System.Exception("ElementSet with ID = "+elementSet.ID+" is invalid",e);
      }
		}
	}
}
