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
using System.Collections;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Spatial
{
	/// <summary>
	/// The ElementMapper converts one ValueSet (inputValues) associated one ElementSet (fromElements)
	/// to a new ValuesSet (return value of MapValue) that corresponds to another ElementSet 
	/// (toElements). The conversion is a two step procedure where the first step (Initialize) is 
	/// executed at initialisation time only, whereas the MapValues is executed during time stepping.
	/// 
	/// <p>The Initialize method will create a conversion matrix with the same number of rows as the
	/// number of elements in the ElementSet associated to the accepting component (i.e. the toElements) 
	/// and the same number of columns as the number of elements in the ElementSet associated to the 
	/// providing component (i.e. the fromElements).</p>
	/// 
	/// <p>Mapping is possible for any zero-, one- and two-dimensional elemets. Zero dimensional 
	/// elements will always be points, one-dimensional elements will allways be polylines and two-
	/// dimensional elements will allways be polygons.</p>
	/// 
  /// <p>The ElementMapper contains a number of methods for mapping between the different element types.
  /// As an example polyline to polygon mapping may be done either as Weighted Mean or as Weighted Sum.
  /// Typically the method choice will depend on the quantity mapped. Such that state variables such as 
  /// water level will be mapped using Weighted Mean whereas flux variables such as seepage from river 
  /// to groundwater will be mapped using Weighted Sum. The list of available methods for a given 
  /// combination of from and to element types is obtained using the GetAvailableMethods method.</p>
  /// </summary>
  public class ElementMapper
  {

    private const int NUMBER_OF_AVAILABLE_METHODS = 15;
    private struct eMethods
    {
      public enum PointToPoint : int
      {
        Nearest = 100,
        Inverse = 101
      }
      
      public enum PointToPolyline : int
      {
        Nearest = 200,
        Inverse = 201,
      }
      
      public enum PointToPolygon : int
      {
        Mean = 300,
        Sum  = 301,
      }

      public enum PolylineToPoint : int
      {
        Nearest = 400,
        Inverse = 401,
      }
      
      public enum PolylineToPolygon : int
      {
        WeightedMean = 500,
        WeightedSum = 501,
      }

      public enum PolygonToPoint : int
      {
        Value = 600,
      }

      public enum PolygonToPolyline : int
      {
        WeightedMean = 700,
        WeightedSum = 701,
      }
     
      public enum PolygonToPolygon : int
      {
        WeightedMean = 800,
        WeightedSum = 801,
      }
    }

    private struct sMethod
    {
      public int ID;
      public string Description;
      public ElementType fromElementsShapeType;
      public ElementType toElementsShapeType;
    }
    private double[ , ] _mappingMatrix; // the mapping matrix
    private int _numberOfRows;
    private int _numberOfColumns;
    private int _methodID;
    private bool _isInitialised;
    sMethod[] _availableMethods;
 
    /// <summary>
    /// Constructor.
    /// </summary>
    public ElementMapper()
    {
      _numberOfRows    = 0;
      _numberOfColumns = 0;
      _isInitialised   = false;

      _availableMethods = new sMethod[NUMBER_OF_AVAILABLE_METHODS];

      _availableMethods[0].fromElementsShapeType = ElementType.XYPoint;
      _availableMethods[0].toElementsShapeType   = ElementType.XYPoint;
      _availableMethods[0].Description           = "Nearest";
      _availableMethods[0].ID				 	    =  (int) eMethods.PointToPoint.Nearest;

      _availableMethods[1].fromElementsShapeType = ElementType.XYPoint;
      _availableMethods[1].toElementsShapeType   = ElementType.XYPoint;
      _availableMethods[1].Description           = "Inverse";
      _availableMethods[1].ID					    =  (int) eMethods.PointToPoint.Inverse;
      
      _availableMethods[2].fromElementsShapeType = ElementType.XYPoint;
      _availableMethods[2].toElementsShapeType   = ElementType.XYPolyLine;
      _availableMethods[2].Description           = "Nearest";
      _availableMethods[2].ID					    =  (int) eMethods.PointToPolyline.Nearest;

      _availableMethods[3].fromElementsShapeType = ElementType.XYPoint;
      _availableMethods[3].toElementsShapeType   = ElementType.XYPolyLine;
      _availableMethods[3].Description           = "Inverse";
      _availableMethods[3].ID					    =  (int) eMethods.PointToPolyline.Inverse;

      _availableMethods[4].fromElementsShapeType = ElementType.XYPoint;
      _availableMethods[4].toElementsShapeType	 = ElementType.XYPolygon;
      _availableMethods[4].Description           = "Mean";
      _availableMethods[4].ID					    =  (int) eMethods.PointToPolygon.Mean;
      
      _availableMethods[5].fromElementsShapeType = ElementType.XYPoint;
      _availableMethods[5].toElementsShapeType	 = ElementType.XYPolygon;
      _availableMethods[5].Description           = "Sum";
      _availableMethods[5].ID					    =  (int) eMethods.PointToPolygon.Sum;

      _availableMethods[6].fromElementsShapeType = ElementType.XYPolyLine;
      _availableMethods[6].toElementsShapeType	 = ElementType.XYPoint;
      _availableMethods[6].Description           = "Nearest";
      _availableMethods[6].ID					    =  (int) eMethods.PolylineToPoint.Nearest;
     
      _availableMethods[7].fromElementsShapeType = ElementType.XYPolyLine;
      _availableMethods[7].toElementsShapeType	 = ElementType.XYPoint;
      _availableMethods[7].Description           = "Inverse";
      _availableMethods[7].ID					    =  (int) eMethods.PolylineToPoint.Inverse;

      _availableMethods[8].fromElementsShapeType = ElementType.XYPolyLine;
      _availableMethods[8].toElementsShapeType	 = ElementType.XYPolygon;
      _availableMethods[8].Description           = "Weighted Mean";
      _availableMethods[8].ID					    =  (int) eMethods.PolylineToPolygon.WeightedMean;

      _availableMethods[9].fromElementsShapeType = ElementType.XYPolyLine;
      _availableMethods[9].toElementsShapeType	 = ElementType.XYPolygon;
      _availableMethods[9].Description           = "Weighted Sum";
      _availableMethods[9].ID					    =  (int) eMethods.PolylineToPolygon.WeightedSum;

      _availableMethods[10].fromElementsShapeType = ElementType.XYPolygon;
      _availableMethods[10].toElementsShapeType	 = ElementType.XYPoint;
      _availableMethods[10].Description					 = "Value";
      _availableMethods[10].ID					    =  (int) eMethods.PolygonToPoint.Value;
			     
      _availableMethods[11].fromElementsShapeType = ElementType.XYPolygon;
      _availableMethods[11].toElementsShapeType   = ElementType.XYPolyLine;
      _availableMethods[11].Description           = "Weighted Mean";
      _availableMethods[11].ID		   		    =  (int) eMethods.PolygonToPolyline.WeightedMean;
      
      _availableMethods[12].fromElementsShapeType = ElementType.XYPolygon;
      _availableMethods[12].toElementsShapeType   = ElementType.XYPolyLine;
      _availableMethods[12].Description           = "Weighted Sum";
      _availableMethods[12].ID		   		    =  (int) eMethods.PolygonToPolyline.WeightedSum;

      _availableMethods[13].fromElementsShapeType = ElementType.XYPolygon;
      _availableMethods[13].toElementsShapeType	 = ElementType.XYPolygon;
      _availableMethods[13].Description           = "Weighted Mean";
      _availableMethods[13].ID					    =  (int) eMethods.PolygonToPolygon.WeightedMean;

      _availableMethods[14].fromElementsShapeType	= ElementType.XYPolygon;
      _availableMethods[14].toElementsShapeType	  = ElementType.XYPolygon;
      _availableMethods[14].Description           = "Weighted Sum";
      _availableMethods[14].ID					   =  (int) eMethods.PolygonToPolygon.WeightedSum;
    }

    /// <summary>
    /// Initialises the ElementMapper. The initialisation includes setting the _isInitialised
    /// flag and calls UpdateMappingMatrix for claculation of the mapping matrix.
    /// </summary>
    ///
    /// <param name="methodDescription">String description of mapping method</param> 
    /// <param name="fromElements">The IElementSet to map from.</param>
    /// <param name="toElements">The IElementSet to map to</param>
    /// 
    /// <returns>
    /// The method has no return value.
    /// </returns>
    public void Initialise(string methodDescription, IElementSet fromElements,  IElementSet toElements)
    {
      UpdateMappingMatrix(methodDescription, fromElements, toElements);
      _isInitialised = true;
    }

    /// <summary>
    /// MapValues calculates a IValueSet through multiplication of an inputValues IValueSet
    /// vector or matrix (ScalarSet or VectorSet) on to the mapping maprix. IScalarSets maps
    /// to IScalarSets and IVectorSets maps to IVectorSets.
    /// </summary>
    /// 
    /// <remarks>
    /// Mapvalues is called every time a georeferenced link is evaluated.
    /// </remarks>
    /// 
    /// <param name="inputValues">IValueSet of values to be mapped.</param>
    /// 
    /// <returns>
    /// A IValueSet found by mapping of the inputValues on to the toElementSet.
    /// </returns>
    public IValueSet MapValues(IValueSet inputValues)
    {
      if (!_isInitialised)
      {
        throw new System.Exception("ElementMapper objects needs to be initialised before the MapValue method can be used");
      }
      if (!inputValues.Count.Equals(_numberOfColumns))
      {
        throw new System.Exception("Dimension mismatch between inputValues and mapping matrix");
      }
      if (inputValues is IScalarSet)
      {
        double[] outValues = new double[_numberOfRows];
        //--- Multiply the Values vector with the MappingMatrix ---
        for (int i = 0; i < _numberOfRows; i++)
        {
          outValues[i] = 0;
          for (int n = 0; n < _numberOfColumns; n++)
          {
            outValues[i] += _mappingMatrix[i,n] * ((IScalarSet) inputValues).GetScalar(n); //(remove)inValues[n];
          }
        }
        ScalarSet outputValues = new ScalarSet(outValues);
        return outputValues;
      }
      else if (inputValues is IVectorSet)
      {
        Vector[] outValues = new Vector[_numberOfRows];
        //--- Multiply the Values vector with the MappingMatrix ---
        for (int i = 0; i < _numberOfRows; i++)
        {
          outValues[i].XComponent = 0;
          outValues[i].YComponent = 0;
          outValues[i].ZComponent = 0;
          for (int n = 0; n < _numberOfColumns; n++)
          {
            outValues[i].XComponent += _mappingMatrix[i,n] * ((IVectorSet) inputValues).GetVector(n).XComponent;
            outValues[i].YComponent += _mappingMatrix[i,n] * ((IVectorSet) inputValues).GetVector(n).YComponent;
            outValues[i].ZComponent += _mappingMatrix[i,n] * ((IVectorSet) inputValues).GetVector(n).ZComponent;
          }
        }
        VectorSet outputValues = new VectorSet(outValues);
        return outputValues;
      }
      else
      {
        throw new System.Exception("Invalid datatype used for inputValues parameter. MapValues failed");
      }
    }

    /// <summary>
    /// Calculates the mapping matrix between fromElements and toElements. The mapping method 
    /// is decided from the combination of methodDescription, fromElements.ElementType and 
    /// toElements.ElementType. 
    /// The valid values for methodDescription is obtained through use of the 
    /// GetAvailableMethods method.
    /// </summary>
    /// 
    /// <remarks>
    /// UpdateMappingMatrix is called during initialisation. UpdateMappingMatrix must be called prior
    /// to Mapvalues.
    /// </remarks>
    /// 
    /// <param name="methodDescription">String description of mapping method</param> 
    /// <param name="fromElements">The IElementset to map from.</param>
    /// <param name="toElements">The IElementset to map to</param>
    ///
    /// <returns>
    /// The method has no return value.
    /// </returns>
    public void UpdateMappingMatrix(string methodDescription, IElementSet fromElements, IElementSet toElements)
    {
      try
      {
        ElementSetChecker.CheckElementSet(fromElements);
        ElementSetChecker.CheckElementSet(toElements);

        _methodID = GetMethodID(methodDescription, fromElements.ElementType, toElements.ElementType);
        _numberOfRows = toElements.ElementCount;
        _numberOfColumns = fromElements.ElementCount;
        _mappingMatrix = new double[_numberOfRows, _numberOfColumns];

        if (fromElements.ElementType == ElementType.XYPoint && toElements.ElementType == ElementType.XYPoint)      
          // Point to Point
        {
          #region
          try
          {
            for (int i = 0; i < _numberOfRows; i++)
            {
              XYPoint ToPoint	= CreateXYPoint(toElements,i);
              for (int j = 0; j < _numberOfColumns; j++)
              {
                XYPoint FromPoint = CreateXYPoint(fromElements,j);
                _mappingMatrix[i,j] = XYGeometryTools.CalculatePointToPointDistance(ToPoint, FromPoint);
              }
            }

            if (_methodID.Equals((int) eMethods.PointToPoint.Nearest))
            {
              for (int i = 0; i < _numberOfRows; i++)
              {
                double MinDist = _mappingMatrix[i,0];
                for (int j = 1; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] < MinDist)
                  {
                    MinDist = _mappingMatrix[i,j];
                  }
                }
                int Denominator = 0;
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] == MinDist)
                  {
                    _mappingMatrix[i,j] = 1;
                    Denominator++;
                  }
                  else
                  {
                    _mappingMatrix[i,j] = 0;
                  }
                }
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  _mappingMatrix[i,j] = _mappingMatrix[i,j]/Denominator;
                }
              }
            } // if (_methodID.Equals((int) eMethods.PointToPoint.Nearest))   
            else if (_methodID.Equals((int) eMethods.PointToPoint.Inverse))
            {
              for (int i = 0; i < _numberOfRows; i++)
              {
                double MinDist = _mappingMatrix[i,0];
                for (int j = 1; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] < MinDist)
                  {
                    MinDist = _mappingMatrix[i,j];
                  }
                }
                if (MinDist == 0)
                {
                  int Denominator = 0;
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    if (_mappingMatrix[i,j] == MinDist)
                    {
                      _mappingMatrix[i,j] = 1;
                      Denominator++;
                    }
                    else
                    {
                      _mappingMatrix[i,j] = 0;
                    }
                  }
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = _mappingMatrix[i,j]/Denominator;
                  }
                }
                else
                {
                  double Denominator = 0;
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = 1/_mappingMatrix[i,j];
                    Denominator = Denominator + _mappingMatrix[i,j];
                  }
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = _mappingMatrix[i,j]/Denominator;
                  }
                }
              }
            } // else if (_methodID.Equals((int) eMethods.PointToPoint.Inverse))
            else
            {
              throw new System.Exception("methodDescription unknown for point point mapping");
            } // else if (_methodID.Equals((int) eMethods.PointToPoint.Nearest)) and else if (_methodID.Equals((int) eMethods.PointToPoint.Inverse))
          }
          catch (System.Exception e) // Catch for all of the Point to Point part
          {
            throw new System.Exception("Point to point mapping failed",e);  
          }
          #endregion
        }
        else if (fromElements.ElementType == ElementType.XYPoint && toElements.ElementType == ElementType.XYPolyLine)      
          // Point to PolyLine
        {
          #region
          try
          {
            for (int i = 0; i < _numberOfRows; i++)
            {
              XYPolyline toPolyLine	= CreateXYPolyline(toElements,i);
              for (int j = 0; j < _numberOfColumns; j++)
              {
                XYPoint fromPoint = CreateXYPoint(fromElements,j);
                _mappingMatrix[i,j] = XYGeometryTools.CalculatePolylineToPointDistance(toPolyLine, fromPoint);
              }
            }

            if (_methodID.Equals((int) eMethods.PointToPolyline.Nearest))
            {
              for (int i = 0; i < _numberOfRows; i++)
              {
                double MinDist = _mappingMatrix[i,0];
                for (int j = 1; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] < MinDist)
                  {
                    MinDist = _mappingMatrix[i,j];
                  }
                }
                int denominator = 0;
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] == MinDist)
                  {
                    _mappingMatrix[i,j] = 1;
                    denominator++;
                  }
                  else
                  {
                    _mappingMatrix[i,j] = 0;
                  }
                }
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  _mappingMatrix[i,j] = _mappingMatrix[i,j]/denominator;
                }
              }
            } // if (_methodID.Equals((int) eMethods.PointToPolyline.Nearest))
            else if (_methodID.Equals((int) eMethods.PointToPolyline.Inverse))
            {
              for (int i = 0; i < _numberOfRows; i++)
              {
                double minDist = _mappingMatrix[i,0];
                for (int j = 1; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] < minDist)
                  {
                    minDist = _mappingMatrix[i,j];
                  }
                }
                if (minDist == 0)
                {
                  int denominator = 0;
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    if (_mappingMatrix[i,j] == minDist)
                    {
                      _mappingMatrix[i,j] = 1;
                      denominator++;
                    }
                    else
                    {
                      _mappingMatrix[i,j] = 0;
                    }
                  }
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = _mappingMatrix[i,j]/denominator;
                  }
                }
                else
                {
                  double denominator = 0;
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = 1/_mappingMatrix[i,j];
                    denominator = denominator + _mappingMatrix[i,j];
                  }
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = _mappingMatrix[i,j]/denominator;
                  }
                }
              }
            } // else if (_methodID.Equals((int) eMethods.PointToPolyline.Inverse))
            else // if _methodID != Nearest and Inverse
            {
              throw new System.Exception("methodDescription unknown for point to polyline mapping");
            }
          }
          catch (System.Exception e)// Catch for all of the Point to Polyline part 
          {
            throw new System.Exception("Point to polyline mapping failed",e);
          }
          #endregion
        }
        else if(fromElements.ElementType == ElementType.XYPoint && toElements.ElementType == ElementType.XYPolygon)
        // Point to Polygon
        {
          #region
          try
          {
            XYPolygon polygon;
            XYPoint   point;
            int count;
            for (int i = 0; i < _numberOfRows; i++)
            {
              polygon 	= CreateXYPolygon(toElements,i);
              count = 0;
              for (int n = 0; n < _numberOfColumns; n++)
              {
                point = CreateXYPoint(fromElements, n);
                if(XYGeometryTools.IsPointInPolygon(point, polygon))
                { 
                  if (_methodID.Equals((int) eMethods.PointToPolygon.Mean))
                  {
                    count = count+1;
                  }
                  else if (_methodID.Equals((int) eMethods.PointToPolygon.Sum))
                  {
                    count = 1;
                  }
                  else
                  {
                    throw new System.Exception("methodDescription unknown for point to polygon mapping");
                  }

                }
              }
              for (int n = 0; n < _numberOfColumns; n++)
              {
                point = CreateXYPoint(fromElements,n);

                if(XYGeometryTools.IsPointInPolygon(point, polygon))
                { 
                  _mappingMatrix[i,n]=1.0/count;
                }
              }
            }
          }
          catch (System.Exception e)// Catch for all of the Point to Polyline part 
          {
            throw new System.Exception("Point to polygon mapping failed",e);
          }
          #endregion
        }
        else if(fromElements.ElementType == ElementType.XYPolyLine && toElements.ElementType ==  ElementType.XYPoint)
        // Polyline to Point
        {
          #region
          try
          {
            for (int i = 0; i < _numberOfRows; i++)
            {
              XYPoint toPoint	= CreateXYPoint(toElements,i);
              for (int j = 0; j < _numberOfColumns; j++)
              {
                XYPolyline fromPolyLine = CreateXYPolyline(fromElements,j);
                _mappingMatrix[i,j] = XYGeometryTools.CalculatePolylineToPointDistance(fromPolyLine, toPoint);
              }
            }

            if (_methodID.Equals((int) eMethods.PolylineToPoint.Nearest))
            {
              for (int i = 0; i < _numberOfRows; i++)
              {
                double minDist = _mappingMatrix[i,0];
                for (int j = 1; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] < minDist)
                  {
                    minDist = _mappingMatrix[i,j];
                  }
                }
                int denominator = 0;
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] == minDist)
                  {
                    _mappingMatrix[i,j] = 1;
                    denominator++;
                  }
                  else
                  {
                    _mappingMatrix[i,j] = 0;
                  }
                }
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  _mappingMatrix[i,j] = _mappingMatrix[i,j]/denominator;
                }
              }
            } // if (_methodID.Equals((int) eMethods.PolylineToPoint.Nearest))
            else if (_methodID.Equals((int) eMethods.PolylineToPoint.Inverse))
            {
              for (int i = 0; i < _numberOfRows; i++)
              {
                double minDist = _mappingMatrix[i,0];
                for (int j = 1; j < _numberOfColumns; j++)
                {
                  if (_mappingMatrix[i,j] < minDist)
                  {
                    minDist = _mappingMatrix[i,j];
                  }
                }
                if (minDist == 0)
                {
                  int denominator = 0;
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    if (_mappingMatrix[i,j] == minDist)
                    {
                      _mappingMatrix[i,j] = 1;
                      denominator++;
                    }
                    else
                    {
                      _mappingMatrix[i,j] = 0;
                    }
                  }
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = _mappingMatrix[i,j]/denominator;
                  }
                }
                else
                {
                  double denominator = 0;
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = 1/_mappingMatrix[i,j];
                    denominator = denominator + _mappingMatrix[i,j];
                  }
                  for (int j = 0; j < _numberOfColumns; j++)
                  {
                    _mappingMatrix[i,j] = _mappingMatrix[i,j]/denominator;
                  }
                }
              }
            } // if (_methodID.Equals((int) eMethods.PolylineToPoint.Inverse))
            else // MethodID != Nearest and Inverse
            {
              throw new System.Exception("methodDescription unknown for polyline to point mapping");
            }
          }
          catch (System.Exception e)// Catch for all of the Point to Polyline part 
          {
            throw new System.Exception("Polyline to point mapping failed",e);
          }
          #endregion
        }
        else if (fromElements.ElementType ==  ElementType.XYPolyLine && toElements.ElementType == ElementType.XYPolygon)      
        // PolyLine to Polygon
        {
          #region
          try 
          {
            for (int i = 0; i < _numberOfRows; i++)
            {
              XYPolygon polygon = CreateXYPolygon(toElements,i);
         
              if (_methodID.Equals((int) eMethods.PolylineToPolygon.WeightedMean))
              {
                double totalLineLengthInPolygon = 0;
                for (int n = 0; n < _numberOfColumns; n++)
                {
                  XYPolyline polyline = CreateXYPolyline(fromElements,n);
                  _mappingMatrix[i,n] = XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(polyline,polygon);
                  totalLineLengthInPolygon += _mappingMatrix[i,n]; 
                }
                if (totalLineLengthInPolygon > 0)
                {
                  for (int n = 0; n < _numberOfColumns; n++)
                  {
                    _mappingMatrix[i,n] = _mappingMatrix[i,n]/totalLineLengthInPolygon;
                  }
                }
              } // if (_methodID.Equals((int) eMethods.PolylineToPolygon.WeightedMean))
              else if (_methodID.Equals((int) eMethods.PolylineToPolygon.WeightedSum))
              {
                for (int n = 0; n < _numberOfColumns; n++)
                {
                  XYPolyline polyline = CreateXYPolyline(fromElements,n);
                  _mappingMatrix[i,n] = XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(polyline,polygon)/polyline.GetLength();
                } // for (int n = 0; n < _numberOfColumns; n++)
              } // else if (_methodID.Equals((int) eMethods.PolylineToPolygon.WeightedSum))
              else // if MethodID != WeightedMean and WeigthedSum
              {
                throw new System.Exception("methodDescription unknown for polyline to polygon mapping");
              }
            } // for (int i = 0; i < _numberOfRows; i++)
          }
          catch (System.Exception e) // Catch for all of polyLine to polygon
          {
            throw new System.Exception("Polyline to polygon mapping failed",e);
          }
          #endregion
        }
        else if (fromElements.ElementType ==  ElementType.XYPolygon && toElements.ElementType == ElementType.XYPoint)
        // Polygon to Point
        {
          #region
          try
          {
            for (int n = 0; n < _numberOfRows; n++)
            {
              XYPoint point = CreateXYPoint(toElements,n);
              for (int i = 0; i < _numberOfColumns; i++)
              {
                XYPolygon polygon = CreateXYPolygon(fromElements,i);    
                if(XYGeometryTools.IsPointInPolygon(point, polygon))
                { 
                  if (_methodID.Equals((int) eMethods.PolygonToPoint.Value))
                  {
                    _mappingMatrix[n,i]=1.0;
                  }
                  else // if _methodID != Value
                  {
                    throw new System.Exception("methodDescription unknown for polygon to point mapping");
                  }
                }
              }
            }
          }
          catch (System.Exception e) // catch for all of Polygon to Point
          {
            throw new System.Exception("Polygon to point mapping failed",e);
          }
          #endregion
        }
        else if(fromElements.ElementType ==  ElementType.XYPolygon && toElements.ElementType == ElementType.XYPolyLine)
        // Polygon to PolyLine
        {
          #region
          try
          {
            for (int i = 0; i < _numberOfRows; i++)
            {
              XYPolyline polyline = CreateXYPolyline(toElements,i);
              if (_methodID.Equals((int) eMethods.PolygonToPolyline.WeightedMean))
              {
                for (int n = 0; n < _numberOfColumns; n++)
                {
                  XYPolygon polygon = CreateXYPolygon(fromElements,n);
                  _mappingMatrix[i,n] = XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(polyline,polygon)/polyline.GetLength();
                }
                double sum = 0;
                for (int n = 0; n < _numberOfColumns; n++)
                {
                  sum += _mappingMatrix[i,n];
                }
                for (int n = 0; n < _numberOfColumns; n++)
                {
                  _mappingMatrix[i,n] = _mappingMatrix[i,n]/sum;
                }
              } // if (_methodID.Equals((int) eMethods.PolygonToPolyline.WeightedMean))
              else if (_methodID.Equals((int) eMethods.PolygonToPolyline.WeightedSum))
              {
                for (int n = 0; n < _numberOfColumns; n++)
                {
                  XYPolygon polygon = CreateXYPolygon(fromElements,n);
                  _mappingMatrix[i,n] = XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(polyline,polygon)/polyline.GetLength();
                }
              } // else if (_methodID.Equals((int) eMethods.PolygonToPolyline.WeightedSum))
              else // _methodID != WeightedMean and WeightedSum
              {
                throw new System.Exception("methodDescription unknown for polygon to polyline mapping");
              }
            }
          }
          catch (System.Exception e) // catch for all of Polygon to PolyLine
          {
            throw new System.Exception("Polygon to polyline mapping failed",e);
          }
          #endregion
        }
        else if(fromElements.ElementType ==  ElementType.XYPolygon && toElements.ElementType == ElementType.XYPolygon)
          // Polygon to Polygon
        {
          #region
          try
          {
            for (int i = 0; i < _numberOfRows; i++)
            {
              XYPolygon toPolygon 	= CreateXYPolygon(toElements,i);
              for (int j = 0; j < _numberOfColumns; j++)
              {
                XYPolygon fromPolygon = CreateXYPolygon(fromElements,j);
                _mappingMatrix[i,j] = XYGeometryTools.CalculateSharedArea(toPolygon, fromPolygon);
              }
              if (_methodID.Equals((int) eMethods.PolygonToPolygon.WeightedMean))
              {
                double denominator = 0;
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  denominator = denominator + _mappingMatrix[i,j];
                }
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  if (denominator != 0)
                  {
                    _mappingMatrix[i,j] = _mappingMatrix[i,j]/denominator;
                  }
                }
              } // if (_methodID.Equals((int) eMethods.PolygonToPolygon.WeightedMean)) 
              else if (_methodID.Equals((int) eMethods.PolygonToPolygon.WeightedSum))
              {
                for (int j = 0; j < _numberOfColumns; j++)
                {
                  _mappingMatrix[i,j] = _mappingMatrix[i,j]/toPolygon.GetArea();
                }
              } // else if (_methodID.Equals((int) eMethods.PolygonToPolygon.WeightedSum))
              else // _methodID != WeightedMean and WeightedSum
              {
                throw new System.Exception("methodDescription unknown for polygon to polygon mapping");
              }
            }
          }
          catch (System.Exception e) // catch for all of Polygon to Polygon
          {
            throw new System.Exception("Polygon to polygon mapping failed",e);
          }
          #endregion
        }
        else // if the fromElementType, toElementType combination is no implemented
        {
          throw new System.Exception("Mapping of specified ElementTypes not included in ElementMapper");
        }
      }
      catch (System.Exception e)
      {
        throw new System.Exception("UpdateMappingMatrix failed to update mapping matrix",e);
      }
		}

		/// <summary>
		/// Extracts the (row, column) element from the MappingMatrix.
		/// </summary>
		/// 
		/// <param name="row">Zero based row index</param>
		/// <param name="column">Zero based column index</param>
		/// <returns>
		/// Element(row, column) from the mapping matrix.
		/// </returns>
		public double GetValueFromMappingMatrix(int row, int column)
		{
      try
      {
        ValidateIndicies(row, column);
      }
      catch (System.Exception e)
      {
        throw new System.Exception("GetValueFromMappingMatrix failed.",e);
      }
      return _mappingMatrix[row, column];
		}

    /// <summary>
    /// Sets individual the (row, column) element in the MappingMatrix.
    /// </summary>
    /// 
    /// <param name="value">Element value to set</param>
    /// <param name="row">Zero based row index</param>
    /// <param name="column">Zero based column index</param>
    /// <returns>
    /// No value is returned.
    /// </returns>
    public void SetValueInMappingMatrix(double value, int row, int column)
    {
      try
      {
        ValidateIndicies(row, column);
      }
      catch (System.Exception e)
      {
        throw new System.Exception("SetValueInMappingMatrix failed.",e);
      }
      _mappingMatrix[row, column] = value;
    }

    private void ValidateIndicies(int row, int column)
    {
      if(row < 0)
      {
        throw new System.Exception("Negative row index not allowed. GetValueFromMappingMatrix failed.");
      }
      else if(row >= _numberOfRows)
      {
        throw new System.Exception("Row index exceeds mapping matrix dimension. GetValueFromMappingMatrix failed.");
      }
      else if(column < 0)
      {
        throw new System.Exception("Negative column index not allowed. GetValueFromMappingMatrix failed.");
      }
      else if(column >= _numberOfColumns)
      {
        throw new System.Exception("Column index exceeds mapping matrix dimension. GetValueFromMappingMatrix failed.");
      }
    }

    /// <summary>
    /// Gives a list of descriptions (strings) for available mapping methods 
    /// given the combination of fromElementType and toElementType
    /// </summary>
    /// 
    /// <param name="fromElementsElementType">Element type of the elements in
    /// the fromElementset</param>
    /// <param name="toElementsElementType">Element type of the elements in
    /// the toElementset</param>
    /// 
    /// <returns>
    ///	<p>ArrayList of method descriptions</p>
    /// </returns>
    public ArrayList GetAvailableMethods(ElementType fromElementsElementType, ElementType toElementsElementType)
		{
			ArrayList methodDescriptions = new ArrayList();

			for ( int i = 0; i < _availableMethods.Length; i++)
			{
				if( fromElementsElementType == _availableMethods[i].fromElementsShapeType)
				{
					if (toElementsElementType == _availableMethods[i].toElementsShapeType)
					{
						methodDescriptions.Add(_availableMethods[i].Description);
					}
				}
			}
			return methodDescriptions;
		}

	  /// <summary>
	  /// Gives a list of ID's (strings) for available mapping methods 
	  /// given the combination of fromElementType and toElementType
	  /// </summary>
	  /// 
	  /// <param name="fromElementsElementType">Element type of the elements in
	  /// the fromElementset</param>
	  /// <param name="toElementsElementType">Element type of the elements in
	  /// the toElementset</param>
	  /// 
	  /// <returns>
	  ///	<p>ArrayList of method ID's</p>
	  /// </returns>
	  public ArrayList GetIDsForAvailableDataOperations(ElementType fromElementsElementType, ElementType toElementsElementType)
	  {
		  ArrayList methodIDs = new ArrayList();

		  for ( int i = 0; i < _availableMethods.Length; i++)
		  {
			  if( fromElementsElementType == _availableMethods[i].fromElementsShapeType)
			  {
				  if (toElementsElementType == _availableMethods[i].toElementsShapeType)
				  {
					  methodIDs.Add("ElementMapper" + _availableMethods[i].ID);
				  }
			  }
		  }
		  return methodIDs;
	  }

	  /// <summary>
	  /// This method will return an ArrayList of IDataOperations that the ElementMapper provides when
	  /// mapping from the ElementType specified in the method argument. 
	  /// </summary>
	  /// <remarks>
	  ///  Each IDataOperation object will contain 3 IArguments:
	  ///  <p> [Key]              [Value]                      [ReadOnly]    [Description]----------------- </p>
	  ///  <p> ["Type"]           ["SpatialMapping"]           [true]        ["Using the ElementMapper"] </p>
	  ///  <p> ["ID"]             [The Operation ID]           [true]        ["Internal ElementMapper dataoperation ID"] </p>
	  ///  <p> ["Description"]    [The Operation Description]  [true]        ["Using the ElementMapper"] </p>
	  ///  <p> ["ToElementType"]  [ElementType]                [true]        ["Valid To-Element Types"]  </p>
	  /// </remarks>
	  /// <param name="fromElementsElementType"></param>
	  /// <returns>
	  ///  ArrayList which contains the available dataOperations (IDataOperation).
	  /// </returns>
	  public ArrayList GetAvailableDataOperations(ElementType fromElementsElementType)
	  {
		  ArrayList availableDataOperations = new ArrayList();

		  for ( int i = 0; i < _availableMethods.Length; i++)
		  {
			  if( fromElementsElementType == _availableMethods[i].fromElementsShapeType)
			  {
				  DataOperation dataOperation = new DataOperation("ElementMapper" + _availableMethods[i].ID);
				  dataOperation.AddArgument(new Argument("ID",_availableMethods[i].ID.ToString(),true,"Internal ElementMapper dataoperation ID"));
				  dataOperation.AddArgument(new Argument("Description",_availableMethods[i].Description,true,"Operation description"));
				  dataOperation.AddArgument(new Argument("Type","SpatialMapping",true,"Using the ElementMapper"));
				  dataOperation.AddArgument(new Argument("FromElementType",_availableMethods[i].fromElementsShapeType.ToString(),true,"Valid From-Element Types"));
				  dataOperation.AddArgument(new Argument("ToElementType",_availableMethods[i].toElementsShapeType.ToString(),true,"Valid To-Element Types"));
				  availableDataOperations.Add(dataOperation);
			  }
		  }
		  return availableDataOperations;
	  }

    private int GetMethodID(string methodDescription, ElementType fromElementsElementType, ElementType toElementsElementType)
    {
      for ( int i = 0; i < _availableMethods.Length; i++)
      {
        if( fromElementsElementType == _availableMethods[i].fromElementsShapeType)
        {
          if (toElementsElementType == _availableMethods[i].toElementsShapeType)
          {
            if (methodDescription == _availableMethods[i].Description)
            return _availableMethods[i].ID;
          }
        }
      }
      throw new System.Exception("methodDescription: "+methodDescription+
                                 " not known for fromElementType: "+fromElementsElementType+
                                 " and to ElementType: "+toElementsElementType);
    }

	  private XYPoint CreateXYPoint(IElementSet elementSet, int index)
	  {
		  if (elementSet.ElementType != ElementType.XYPoint)
		  {
			  throw new System.Exception("Cannot create XYPoint");
		  }

		  XYPoint xyPoint = new XYPoint();
		  xyPoint.X = elementSet.GetXCoordinate(index,0);
		  xyPoint.Y = elementSet.GetYCoordinate(index,0);
		  return xyPoint;
	  }

	  private XYPolyline CreateXYPolyline(IElementSet elementSet, int index)
	  {
		  if (!(elementSet.ElementType == ElementType.XYPolyLine || elementSet.ElementType == ElementType.XYLine))
		  {
			  throw new System.Exception("Cannot create XYPolyline");
		  }

		  XYPolyline xyPolyline = new XYPolyline();
		  for (int i = 0; i < elementSet.GetVertexCount(index); i++)
		  {
			  xyPolyline.Points.Add(new XYPoint(elementSet.GetXCoordinate(index,i), elementSet.GetYCoordinate(index,i)));
		  }

		  return xyPolyline;
	  }

	  private XYPolygon CreateXYPolygon(IElementSet elementSet, int index)
	  {
		  if (elementSet.ElementType != ElementType.XYPolygon)
		  {
			  throw new System.Exception("Cannot create XYPolyline");
		  }

		  XYPolygon xyPolygon = new XYPolygon();

		  for (int i = 0; i < elementSet.GetVertexCount(index); i++)
		  {
			  xyPolygon.Points.Add(new XYPoint(elementSet.GetXCoordinate(index,i), elementSet.GetYCoordinate(index,i)));
		  }

		  return xyPolygon;
	  }
	}
}
