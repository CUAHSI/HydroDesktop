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
using System.Collections;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Spatial;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Spatial.UnitTest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class ElementMapperTest
	{
		private ElementSet CreateElementSet(string ElementSetID)
		{
			if (ElementSetID == "4 Points")
			{
				ElementSet fourPointsElementSet = new ElementSet("4 points","4 Points",ElementType.XYPoint,new SpatialReference("DummyID")); 
		
				Element e0 = new Element("e0"); 
				Element e1 = new Element("e1"); 
				Element e2 = new Element("e2");
				Element e3 = new Element("e3"); 
		
				e0.AddVertex(new Vertex( 0,100,0));
				e1.AddVertex(new Vertex( 0,0,0));
				e2.AddVertex(new Vertex( 100,0,0));
				e3.AddVertex(new Vertex(100,100,0));

				fourPointsElementSet.AddElement(e0);
				fourPointsElementSet.AddElement(e1);
				fourPointsElementSet.AddElement(e2);
				fourPointsElementSet.AddElement(e3);

				return fourPointsElementSet;
			}
			else if (ElementSetID == "2 Points")
			{
				ElementSet twoPointsElementSet = new ElementSet("2 points","2 Points",ElementType.XYPoint,new SpatialReference("dumID")); 

				Element k0 = new Element("k0"); 
				Element k1 = new Element("k1"); 

				k0.AddVertex(new Vertex( 0,75,0));
				k1.AddVertex(new Vertex( 200, 50, 0));

				twoPointsElementSet.AddElement(k0);
				twoPointsElementSet.AddElement(k1);

				return twoPointsElementSet;

			}
			else if (ElementSetID == "4 Other Points")
			{
				ElementSet fourPointsElementSet = new ElementSet("4 Other points","4 Other Points",ElementType.XYPoint,new SpatialReference("DummyID")); 
		
				Element e0 = new Element("e0"); 
				Element e1 = new Element("e1"); 
				Element e2 = new Element("e2");
				Element e3 = new Element("e3"); 
		
				e0.AddVertex(new Vertex( 0,15,0));
				e1.AddVertex(new Vertex( 5,15,0));
				e2.AddVertex(new Vertex( 0,10,0));
				e3.AddVertex(new Vertex(10,10,0));

				fourPointsElementSet.AddElement(e0);
				fourPointsElementSet.AddElement(e1);
				fourPointsElementSet.AddElement(e2);
				fourPointsElementSet.AddElement(e3);

				return fourPointsElementSet;
			}
			else if(ElementSetID == "3 points polyline")
			{
				ElementSet lineElementSet = new ElementSet("3 points polyline","3 points polyline",ElementType.XYPolyLine,new SpatialReference("dumID")); 

				Element l0 = new Element("k0"); 
				Element l1 = new Element("k1");
 
				Vertex v0 = new Vertex(0 ,20, 0);
				Vertex v1 = new Vertex(0 ,10, 0);
				Vertex v2 = new Vertex(0 , 0, 0);

				l0.AddVertex(v0);
				l0.AddVertex(v2);

				l1.AddVertex(v1);
				l1.AddVertex(v2);

				lineElementSet.AddElement(l0);
				lineElementSet.AddElement(l1);
				return lineElementSet;
			}
			else
			{
				throw new Exception("Cound not find specified elementset");
			
			}
		}

		[Test] // testing the Initialise method
		public void Initialise()
		{
			ElementSet fourPointsElementSet = CreateElementSet("4 Points");
			ElementSet twoPointsElementSet  = CreateElementSet("2 Points");
      
			ElementMapper elementMapper = new ElementMapper();
	      
			ArrayList methodDescriptions = elementMapper.GetAvailableMethods(fourPointsElementSet.ElementType, twoPointsElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), fourPointsElementSet,  twoPointsElementSet);

			double Calculated = elementMapper.GetValueFromMappingMatrix(0, 0);
			double Expected  = 1;

			Assert.AreEqual(Expected,Calculated);
		}

		[Test] // testing the Initialise method
		public void UpdateMappingMatrix_PointPoint()
		{
			ElementSet fourPointsElementSet = CreateElementSet("4 Points");
			ElementSet twoPointsElementSet = CreateElementSet("2 Points");

			ElementMapper elementMapper = new ElementMapper();
  	      
			ArrayList methodDescriptions = elementMapper.GetAvailableMethods(fourPointsElementSet.ElementType, twoPointsElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), fourPointsElementSet,  twoPointsElementSet);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0)
				+elementMapper.GetValueFromMappingMatrix(0, 1)
				+elementMapper.GetValueFromMappingMatrix(0, 2)
				+elementMapper.GetValueFromMappingMatrix(0, 3));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(1, 2));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0)
				+elementMapper.GetValueFromMappingMatrix(1, 1)
				+elementMapper.GetValueFromMappingMatrix(1, 2)
				+elementMapper.GetValueFromMappingMatrix(1, 3));
  	      
			elementMapper.Initialise(methodDescriptions[1].ToString(), fourPointsElementSet,  twoPointsElementSet);
			Assert.AreEqual(0.56310461156889, elementMapper.GetValueFromMappingMatrix(0, 0),0.000000001);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0)
				+elementMapper.GetValueFromMappingMatrix(0, 1)
				+elementMapper.GetValueFromMappingMatrix(0, 2)
				+elementMapper.GetValueFromMappingMatrix(0, 3));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0)
				+elementMapper.GetValueFromMappingMatrix(1, 1)
				+elementMapper.GetValueFromMappingMatrix(1, 2)
				+elementMapper.GetValueFromMappingMatrix(1, 3),0.000000001);
		}
		[Test] // testing the Initialise method
		public void UpdateMappingMatrix_PointPolyline()
		{
			ElementSet fourPointsElementSet = CreateElementSet("4 Other Points");
			ElementSet lineElementSet = CreateElementSet("3 points polyline");
     
			ElementMapper elementMapper = new ElementMapper();
      
			// point to polyline
			ArrayList methodDescriptions = elementMapper.GetAvailableMethods(fourPointsElementSet.ElementType, lineElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), fourPointsElementSet, lineElementSet);
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 3));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 3));
        
			elementMapper.Initialise(methodDescriptions[0].ToString(), fourPointsElementSet, lineElementSet);
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 3));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 3));
        
			// polyline to point
			methodDescriptions = elementMapper.GetAvailableMethods(lineElementSet.ElementType, fourPointsElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), lineElementSet, fourPointsElementSet);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 0));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 1));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 0));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 1));
        
			elementMapper.Initialise(methodDescriptions[1].ToString(), lineElementSet, fourPointsElementSet);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
			Assert.AreEqual(0.585786437626905, elementMapper.GetValueFromMappingMatrix(1, 0));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0)
				+ elementMapper.GetValueFromMappingMatrix(1, 1));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 0));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 1));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 0));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 1));      
		}
		[Test] // testing the Initialise method
		public void UpdateMappingMatrix_PointPolygon()
		{
			ElementSet gridElementSet = new ElementSet("gridElm","G1",ElementType.XYPolygon, new SpatialReference("ref"));
			ElementSet fourPointsElementSet = new ElementSet("4 points","4P",ElementType.XYPoint,new SpatialReference("DummyID")); 

			Vertex v_0_20  = new Vertex(0,20,0);
			Vertex v_0_10  = new Vertex(0,10,0);
			Vertex v_0_0   = new Vertex(0, 0,0);
			Vertex v_0_15  = new Vertex(0,15,0);
			Vertex v_5_15  = new Vertex(5,15,0);
			Vertex v_10_20 = new Vertex(10,20,0);
			Vertex v_10_15 = new Vertex(10,15,0);
			Vertex v_10_10 = new Vertex(10,10,0);
			Vertex v_10_0  = new Vertex(10, 0,0);
			Vertex v_15_15 = new Vertex(15,15,0);
			Vertex v_15_5  = new Vertex(15,5,0);
			Vertex v_20_20 = new Vertex(20,20,0);
			Vertex v_20_10 = new Vertex(20,10,0);

			Element square1 = new Element("square1");
			Element square2 = new Element("square2");
			Element square3 = new Element("square3");

			square1.AddVertex(v_0_20);
			square1.AddVertex(v_0_10);
			square1.AddVertex(v_10_10);
			square1.AddVertex(v_10_20);

			square2.AddVertex(v_10_20);
			square2.AddVertex(v_10_10);
			square2.AddVertex(v_20_10);
			square2.AddVertex(v_20_20);

			square3.AddVertex(v_0_10);
			square3.AddVertex(v_0_0);
			square3.AddVertex(v_10_0);
			square3.AddVertex(v_10_10);

			gridElementSet.AddElement(square1);
			gridElementSet.AddElement(square2);
			gridElementSet.AddElement(square3);

			Element point_5_15  = new Element("point 5, 15");
			Element point_10_15 = new Element("point 10, 15");
			Element point_15_15 = new Element("point 15, 15");
			Element point_15_5  = new Element("point 15, 5");

			point_5_15.AddVertex(v_5_15);
			point_10_15.AddVertex(v_10_15);
			point_15_15.AddVertex(v_15_15);
			point_15_5.AddVertex(v_15_5);

			fourPointsElementSet.AddElement(point_5_15);
			fourPointsElementSet.AddElement(point_10_15);
			fourPointsElementSet.AddElement(point_15_15);
			fourPointsElementSet.AddElement(point_15_5);
        
			ElementMapper elementMapper = new ElementMapper();
      
			// point to polygon
      
			ArrayList methodDescriptions = elementMapper.GetAvailableMethods(fourPointsElementSet.ElementType, gridElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), fourPointsElementSet, gridElementSet);
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 0));
			Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 1));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 3));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 3));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 1));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 3));

			// polygon to point
			methodDescriptions = elementMapper.GetAvailableMethods(gridElementSet.ElementType, fourPointsElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), gridElementSet, fourPointsElementSet);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 2));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 0));
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(2, 1));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 2));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(3, 0));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(3, 1));
			Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(3, 2));
		}
		[Test] // testing the Initialise method
		public void UpdateMappingMatrix_PolylinePolygon()
		{
			ElementSet twoSquaresGrid = new ElementSet("TwoSquaresGrid","TwoSquaresGrid",ElementType.XYPolygon,new SpatialReference("ref"));
  		
			Element e1 = new Element("e1");
			Element e2 = new Element("e2");
  		
			e1.AddVertex(new Vertex(1,1,0));
			e1.AddVertex(new Vertex(3,1,0));
			e1.AddVertex(new Vertex(3,3,0));
			e1.AddVertex(new Vertex(1,3,0));

			e2.AddVertex(new Vertex(3,1,0));
			e2.AddVertex(new Vertex(5,1,0));
			e2.AddVertex(new Vertex(5,3,0));
			e2.AddVertex(new Vertex(3,3,0));

			twoSquaresGrid.AddElement(e1);
			twoSquaresGrid.AddElement(e2);

			ElementSet twoLines = new ElementSet("TwoLines","TwoLines",ElementType.XYPolyLine,new SpatialReference("ref"));

			Element l1 = new Element("l1");
			Element l2 = new Element("l2");

			l1.AddVertex(new Vertex(0,2.5,0));
			l1.AddVertex(new Vertex(2,2.5,0));
			l2.AddVertex(new Vertex(2,2.5,0));
			l2.AddVertex(new Vertex(4,1.5,0));

			twoLines.AddElement(l1);
			twoLines.AddElement(l2);
      
			// Line to Polygon
			ElementMapper elementMapper = new ElementMapper();
			ArrayList methodDescriptions = elementMapper.GetAvailableMethods(twoLines.ElementType, twoSquaresGrid.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), twoLines,twoSquaresGrid);
			Assert.AreEqual(1/(1+Math.Sqrt(1+Math.Pow(0.5,2))),elementMapper.GetValueFromMappingMatrix(0,0),"Test1");
			Assert.AreEqual(1-1/(1+Math.Sqrt(1+Math.Pow(0.5,2))),elementMapper.GetValueFromMappingMatrix(0,1),"Test2");
			Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(1,0),"Test3");
			Assert.AreEqual(1,elementMapper.GetValueFromMappingMatrix(1,1),"Test4");

			elementMapper.Initialise(methodDescriptions[1].ToString(), twoLines,twoSquaresGrid);
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(0,0),"Test5");
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(0,1),"Test6");
			Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(1,0),"Test7");
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,1),"Test8");
      
			// Polygon To PolyLine
			methodDescriptions = elementMapper.GetAvailableMethods(twoSquaresGrid.ElementType, twoLines.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), twoSquaresGrid, twoLines);
			Assert.AreEqual(1.0,elementMapper.GetValueFromMappingMatrix(0,0),"Test9");
			Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(0,1),"Test10");
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,0),"Test11");
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,1),"Test12");

			elementMapper.Initialise(methodDescriptions[1].ToString(), twoSquaresGrid, twoLines);
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(0,0),"Test13");
			Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(0,1),"Test14");
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,0),"Test15");
			Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,1),"Test16");
		}
		[Test] // testing the Initialise method
		public void UpdateMappingMatrix_PolygonPolygon()
		{
			Vertex v1_0_10  = new Vertex(0,10,0);
			Vertex v1_0_0   = new Vertex(0,0,0);
			Vertex v1_10_0  = new Vertex(10,0,0);
			Vertex v1_10_10 = new Vertex(10,10,0);
			Vertex v1_20_0  = new Vertex(20,0,0);
			Vertex v1_20_10 = new Vertex(20,10,0);
			Vertex v1_5_9   = new Vertex(5,9,0);
			Vertex v1_5_1   = new Vertex(5,1,0);
			Vertex v1_15_5  = new Vertex(15,5,0);

			Element LeftSquare  = new Element("LeftSquare");
			LeftSquare.AddVertex(v1_0_10);
			LeftSquare.AddVertex(v1_0_0);
			LeftSquare.AddVertex(v1_10_0);
			LeftSquare.AddVertex(v1_10_10);
      
			Element RightSquare = new Element("RightSquare");
			RightSquare.AddVertex(v1_10_10);
			RightSquare.AddVertex(v1_10_0);
			RightSquare.AddVertex(v1_20_0);
			RightSquare.AddVertex(v1_20_10);

			Element Triangle    = new Element("Triangle");
			Triangle.AddVertex(v1_5_9);
			Triangle.AddVertex(v1_5_1);
			Triangle.AddVertex(v1_15_5);

			ElementSet TwoSquareElementSet      = new ElementSet("TwoSquareElementSet","TwoSquareElementSet",ElementType.XYPolygon,new SpatialReference("ref"));
			ElementSet TriangleElementSet       = new ElementSet("TriangleElementSet","TriangleElementSet",ElementType.XYPolygon,new SpatialReference("ref"));
			TwoSquareElementSet.AddElement(LeftSquare);
			TwoSquareElementSet.AddElement(RightSquare);
			TriangleElementSet.AddElement(Triangle);
      
			ElementMapper elementMapper = new ElementMapper();
			ArrayList methodDescriptions = elementMapper.GetAvailableMethods(TwoSquareElementSet.ElementType, TriangleElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), TriangleElementSet,  TwoSquareElementSet);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test1");
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0),"Test2");
      
			elementMapper.Initialise(methodDescriptions[0].ToString(), TwoSquareElementSet, TriangleElementSet);
			Assert.AreEqual(0.75, elementMapper.GetValueFromMappingMatrix(0, 0),0.000000001,"Test3");
			Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 1),"Test4");
	      
			elementMapper.Initialise(methodDescriptions[1].ToString(), TriangleElementSet,  TwoSquareElementSet);
			Assert.AreEqual(0.3, elementMapper.GetValueFromMappingMatrix(0, 0),"Test5");
			Assert.AreEqual(0.1, elementMapper.GetValueFromMappingMatrix(1, 0),"Test6");
	    
			elementMapper.Initialise(methodDescriptions[1].ToString(), TwoSquareElementSet, TriangleElementSet);
			Assert.AreEqual(0.75, elementMapper.GetValueFromMappingMatrix(0, 0),0.0000000001,"Test7");
			Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 1),"Test8");
      
			Vertex v2_0_2 = new Vertex(0,2,0);
			Vertex v2_0_0 = new Vertex(0,0,0);
			Vertex v2_2_0 = new Vertex(2,0,0);
			Vertex v2_1_2 = new Vertex(1,2,0);
			Vertex v2_1_0 = new Vertex(1,0,0);
			Vertex v2_3_0 = new Vertex(3,0,0);

			Element LeftTriangle2  = new Element("LeftTriangle");
			LeftTriangle2.AddVertex(v2_0_2);
			LeftTriangle2.AddVertex(v2_0_0);
			LeftTriangle2.AddVertex(v2_2_0);

			Element RightTriangle2  = new Element("RightTriangle");
			RightTriangle2.AddVertex(v2_1_2);
			RightTriangle2.AddVertex(v2_1_0);
			RightTriangle2.AddVertex(v2_3_0);
      
			ElementSet LeftTriangleElementSet2      = new ElementSet("TwoSquareElementSet","TwoSquareElementSet",ElementType.XYPolygon,new SpatialReference("ref"));
			ElementSet RightTriangleElementSet2       = new ElementSet("TriangleElementSet","TriangleElementSet",ElementType.XYPolygon,new SpatialReference("ref"));
			LeftTriangleElementSet2.AddElement(LeftTriangle2);
			RightTriangleElementSet2.AddElement(RightTriangle2);

      
			elementMapper.Initialise(methodDescriptions[0].ToString(), LeftTriangleElementSet2, RightTriangleElementSet2);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test9");

			elementMapper.Initialise(methodDescriptions[0].ToString(), RightTriangleElementSet2, LeftTriangleElementSet2);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test10");
      
			elementMapper.Initialise(methodDescriptions[1].ToString(), LeftTriangleElementSet2, RightTriangleElementSet2);
			Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 0),"Test11");

			elementMapper.Initialise(methodDescriptions[1].ToString(), RightTriangleElementSet2, LeftTriangleElementSet2);
			Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 0),"Test12");
      
			Vertex v3_0_2 = new Vertex(0,2,0);
			Vertex v3_0_0 = new Vertex(0,0,0);
			Vertex v3_2_0 = new Vertex(2,0,0);
			Vertex v3_1_2 = new Vertex(1,2,0);
			Vertex v3_1_0 = new Vertex(1,0,0);
			Vertex v3_3_2 = new Vertex(3,2,0);
    
			Element LeftTriangle3  = new Element("LeftTriangle");
			LeftTriangle3.AddVertex(v3_0_2);
			LeftTriangle3.AddVertex(v3_0_0);
			LeftTriangle3.AddVertex(v3_2_0);

			Element RightTriangle3  = new Element("RightTriangle");
			RightTriangle3.AddVertex(v3_1_2);
			RightTriangle3.AddVertex(v3_1_0);
			RightTriangle3.AddVertex(v3_3_2);
      
			ElementSet LeftTriangleElementSet3      = new ElementSet("TwoSquareElementSet","TwoSquareElementSet",ElementType.XYPolygon,new SpatialReference("ref"));
			ElementSet RightTriangleElementSet3       = new ElementSet("TriangleElementSet","TriangleElementSet",ElementType.XYPolygon,new SpatialReference("ref"));
			LeftTriangleElementSet3.AddElement(LeftTriangle3);
			RightTriangleElementSet3.AddElement(RightTriangle3);

			elementMapper.Initialise(methodDescriptions[0].ToString(), LeftTriangleElementSet3, RightTriangleElementSet3);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test13");

			elementMapper.Initialise(methodDescriptions[0].ToString(), RightTriangleElementSet3, LeftTriangleElementSet3);
			Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test14");
      
			elementMapper.Initialise(methodDescriptions[1].ToString(), LeftTriangleElementSet3, RightTriangleElementSet3);
			Assert.AreEqual(0.125, elementMapper.GetValueFromMappingMatrix(0, 0),"Test15");

			elementMapper.Initialise(methodDescriptions[1].ToString(), RightTriangleElementSet3, LeftTriangleElementSet3);
			Assert.AreEqual(0.125, elementMapper.GetValueFromMappingMatrix(0, 0),"Test16");
		}
		[Test] // testing the Initialise method
		public void MapValues()
		{
			ElementSet gridElementSet = new ElementSet("RegularGrid","RegularGrid",ElementType.XYPolygon, new SpatialReference("ref"));
			ElementSet fourPointsElementSet = new ElementSet("4 points","4P",ElementType.XYPoint,new SpatialReference("DummyID")); 

			Vertex v_0_20  = new Vertex(0,20,0);
			Vertex v_0_10  = new Vertex(0,10,0);
			Vertex v_0_0   = new Vertex(0, 0,0);
			Vertex v_0_15  = new Vertex(0,15,0);
			Vertex v_5_15  = new Vertex(5,15,0);
			Vertex v_10_20 = new Vertex(10,20,0);
			Vertex v_10_15 = new Vertex(10,15,0);
			Vertex v_10_10 = new Vertex(10,10,0);
			Vertex v_10_0  = new Vertex(10, 0,0);
			Vertex v_15_15 = new Vertex(15,15,0);
			Vertex v_15_5  = new Vertex(15,5,0);
			Vertex v_20_20 = new Vertex(20,20,0);
			Vertex v_20_10 = new Vertex(20,10,0);

			Element square1 = new Element("square1");
			Element square2 = new Element("square2");
			Element square3 = new Element("square3");

			square1.AddVertex(v_0_20);
			square1.AddVertex(v_0_10);
			square1.AddVertex(v_10_10);
			square1.AddVertex(v_10_20);

			square2.AddVertex(v_10_20);
			square2.AddVertex(v_10_10);
			square2.AddVertex(v_20_10);
			square2.AddVertex(v_20_20);

			square3.AddVertex(v_0_10);
			square3.AddVertex(v_0_0);
			square3.AddVertex(v_10_0);
			square3.AddVertex(v_10_10);

			gridElementSet.AddElement(square1);
			gridElementSet.AddElement(square2);
			gridElementSet.AddElement(square3);

			Element point_5_15  = new Element("point 5, 15");
			Element point_10_15 = new Element("point 10, 15");
			Element point_15_15 = new Element("point 15, 15");
			Element point_15_5  = new Element("point 15, 5");

			point_5_15.AddVertex(v_5_15);
			point_10_15.AddVertex(v_10_15);
			point_15_15.AddVertex(v_15_15);
			point_15_5.AddVertex(v_15_5);

			fourPointsElementSet.AddElement(point_5_15);
			fourPointsElementSet.AddElement(point_10_15);
			fourPointsElementSet.AddElement(point_15_15);
			fourPointsElementSet.AddElement(point_15_5);
			ScalarSet fourPointsScalarSet = new ScalarSet();
			ScalarSet gridScalarSet = new ScalarSet();      
      
			ElementMapper elementMapper = new ElementMapper();
      
			// point to polygon  
      
			ArrayList methodDescriptions = elementMapper.GetAvailableMethods(fourPointsElementSet.ElementType, gridElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), fourPointsElementSet, gridElementSet);
			fourPointsScalarSet.data = new double[4] { 0, 10, 20, 30 };
			gridScalarSet = (ScalarSet)elementMapper.MapValues(fourPointsScalarSet);
			Assert.AreEqual(5, gridScalarSet.data[0]);
			Assert.AreEqual(20, gridScalarSet.data[1]);
			Assert.AreEqual(0, gridScalarSet.data[2]);
			// polygon to point
			methodDescriptions = elementMapper.GetAvailableMethods(gridElementSet.ElementType, fourPointsElementSet.ElementType);
			elementMapper.Initialise(methodDescriptions[0].ToString(), gridElementSet, fourPointsElementSet);
			fourPointsScalarSet = (ScalarSet)elementMapper.MapValues(gridScalarSet);
			Assert.AreEqual(5, fourPointsScalarSet.data[0]);
			Assert.AreEqual(5, fourPointsScalarSet.data[1]);
			Assert.AreEqual(20, fourPointsScalarSet.data[2]);
			Assert.AreEqual(0, fourPointsScalarSet.data[3]);
		}

		[Test]
		public void GetAvailableMethods()
		{
			ElementMapper elementMapper = new ElementMapper();
			ElementType fromElementType = ElementType.XYPolygon;
			ElementType toElementType  = ElementType.XYPolygon;

			ArrayList availableMethods = elementMapper.GetAvailableMethods(fromElementType, toElementType);
			Assert.AreEqual("Weighted Mean", availableMethods[0]);
			Assert.AreEqual("Weighted Sum", availableMethods[1]);
		}

		[Test]
		[ExpectedException(typeof(System.Exception))]
		public void ExpectexException_UpdateMappingMatrix_ElementChecker()
		{
			//Two Vertices in point element error
			ElementSet elementSet = new ElementSet("test","test",ElementType.XYPoint,new SpatialReference("DummyID"));
			Element e1 = new Element("e1");
			e1.AddVertex(new Vertex(1,1,1));
			e1.AddVertex(new Vertex(2,2,2)); //here the error is introduced on purpose
						
			elementSet.AddElement(e1);
			
			ElementMapper elementMapper = new ElementMapper();
			string method = (string) elementMapper.GetAvailableMethods(ElementType.XYPolyLine,ElementType.XYPolygon)[0];
			elementMapper.UpdateMappingMatrix(method,elementSet,elementSet);
		}

		[Test]
		public void GetAvailableDataOperations()
		{
			ElementMapper elementMapper = new ElementMapper();

			Console.WriteLine("=========================FROM XYPoint====================================");
			ArrayList dataOperationsXYPoint = elementMapper.GetAvailableDataOperations(ElementType.XYPoint);
			foreach (IDataOperation operation in dataOperationsXYPoint)
			{
				Console.WriteLine(" ");
				Console.WriteLine("------------------------------------");
				Console.WriteLine("DataOperationID: " + operation.ID);
				for (int i = 0; i <  operation.ArgumentCount; i++)
				{
					Console.WriteLine(" ");
					Console.WriteLine("Key:         " + operation.GetArgument(i).Key);
					Console.WriteLine("Value:       " + operation.GetArgument(i).Value);
					Console.WriteLine("ReadOnly:    " + operation.GetArgument(i).ReadOnly.ToString());
					Console.WriteLine("Description: " + operation.GetArgument(i).Description);
				}
			}

            Console.WriteLine(" ");
			Console.WriteLine("========================= FROM XYLine ====================================");
			ArrayList dataOperationsXYLine = elementMapper.GetAvailableDataOperations(ElementType.XYLine);
			foreach (IDataOperation operation in dataOperationsXYLine)
			{
				Console.WriteLine(" ");
				Console.WriteLine("------------------------------------");
				Console.WriteLine("DataOperationID: " + operation.ID);
				for (int i = 0; i <  operation.ArgumentCount; i++)
				{
					Console.WriteLine(" ");
					Console.WriteLine("Key:         " + operation.GetArgument(i).Key);
					Console.WriteLine("Value:       " + operation.GetArgument(i).Value);
					Console.WriteLine("ReadOnly:    " + operation.GetArgument(i).ReadOnly.ToString());
					Console.WriteLine("Description: " + operation.GetArgument(i).Description);
				}
			}

			Console.WriteLine(" ");
			Console.WriteLine("========================= FROM XYPolyLine ====================================");
			ArrayList dataOperationsXYPolyLine = elementMapper.GetAvailableDataOperations(ElementType.XYPolyLine);
			foreach (IDataOperation operation in dataOperationsXYPolyLine)
			{
				Console.WriteLine(" ");
				Console.WriteLine("------------------------------------");
				Console.WriteLine("DataOperationID: " + operation.ID);
				for (int i = 0; i <  operation.ArgumentCount; i++)
				{
					Console.WriteLine(" ");
					Console.WriteLine("Key:         " + operation.GetArgument(i).Key);
					Console.WriteLine("Value:       " + operation.GetArgument(i).Value);
					Console.WriteLine("ReadOnly:    " + operation.GetArgument(i).ReadOnly.ToString());
					Console.WriteLine("Description: " + operation.GetArgument(i).Description);
				}
			}

            Console.WriteLine(" ");
			Console.WriteLine("=========================FROM XYPolygon====================================");
			ArrayList dataOperationsXYPolygon = elementMapper.GetAvailableDataOperations(ElementType.XYPolygon);
			foreach (IDataOperation operation in dataOperationsXYPolygon)
			{
				Console.WriteLine(" ");
				Console.WriteLine("------------------------------------");
				Console.WriteLine("DataOperationID: " + operation.ID);
				for (int i = 0; i <  operation.ArgumentCount; i++)
				{
					Console.WriteLine(" ");
					Console.WriteLine("Key:         " + operation.GetArgument(i).Key);
					Console.WriteLine("Value:       " + operation.GetArgument(i).Value);
					Console.WriteLine("ReadOnly:    " + operation.GetArgument(i).ReadOnly.ToString());
					Console.WriteLine("Description: " + operation.GetArgument(i).Description);
	 			}
			}

			bool operationWasFound       = false;
			bool IDWasFound              = false;
			bool descriptionWasFound     = false;
			bool typeWasFound            = false;
			bool fromElementTypeWasFound = false;
			bool toElementTypeWasFound   = false;

			foreach (IDataOperation operation in dataOperationsXYPolygon)
			{
				if (operation.ID == "ElementMapper801")
				{
					operationWasFound = true;

					for (int i = 0; i <  operation.ArgumentCount; i++)
					{
						if (operation.GetArgument(i).Key == "ID")
						{
							Assert.AreEqual("801",operation.GetArgument(i).Value);
							IDWasFound = true;
						}
						if (operation.GetArgument(i).Key == "Description")
						{
							Assert.AreEqual("Weighted Sum",operation.GetArgument(i).Value);
							descriptionWasFound = true;
						}
						if (operation.GetArgument(i).Key == "Type")
						{
							Assert.AreEqual("SpatialMapping",operation.GetArgument(i).Value);
							typeWasFound = true;
						}
						if (operation.GetArgument(i).Key == "FromElementType")
						{
							Assert.AreEqual("XYPolygon",operation.GetArgument(i).Value);
							fromElementTypeWasFound = true;
						}
						if (operation.GetArgument(i).Key == "ToElementType")
						{
							Assert.AreEqual("XYPolygon",operation.GetArgument(i).Value);
							toElementTypeWasFound = true;
						}
					}
				}
			}

			Assert.AreEqual(true,operationWasFound);
			Assert.AreEqual(true,IDWasFound);
			Assert.AreEqual(true,descriptionWasFound);
			Assert.AreEqual(true,typeWasFound);
			Assert.AreEqual(true,fromElementTypeWasFound);
			Assert.AreEqual(true,toElementTypeWasFound);
		
		}
	}
}
