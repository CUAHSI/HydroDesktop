#region Copyright
/*
    Copyright (c) 2005,2006,2007, OpenMI Association
    "http://www.openmi.org/"

    This file is part of OpenMI.Standard.dll

    OpenMI.Standard.dll is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    OpenMI.Standard.dll is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

namespace OpenMI.Standard
{
	/// <summary>
	/// Shape Type of an Elementset
	/// </summary>

	public enum ElementType : int
	{
							/// <summary>
							/// Identifier based</summary>
		IDBased    = 0,
							/// <summary>
							/// Points</summary>
		XYPoint    = 1,
							/// <summary>
							/// Lines</summary>
		XYLine     = 2,
							/// <summary>
							/// Polylines</summary>
		XYPolyLine = 3,
							/// <summary>
							/// Polygons</summary>
		XYPolygon  = 4,

					
		/// <summary>
		/// 3D Point
		/// </summary>
		XYZPoint   = 5,
		
		/// <summary>
		/// 3D Line
		/// </summary>
		XYZLine    = 6,
		
		/// <summary>
		/// 3D Polyline
		/// </summary>
		XYZPolyLine = 7,
		
		/// <summary>
		/// 3D Polygon
		/// </summary>
		XYZPolygon  = 8,
		
		/// <summary>
		/// 3D Polyhedron
		/// </summary>
		XYZPolyhedron = 9
	}

	/// <summary>
    /// <para>Data exchange between components in OpenMI is always related to one or more elements in a space,
    /// either geo-referenced or not. An element set in OpenMI can be anything from a one-dimensional array
    /// of points, line segments, poly lines or polygons, through to an array of three-dimensional volumes.
    /// As a special case, a cloud of IDBased elements (without co-ordinates) is also supported thus allowing
    /// exchange of arbitrary data that is not related to space in any way.</para>
    /// 
    /// <para>The IElementSet interface has been defined to describe, in a finite element sense, the space where
    /// the values apply, while preserving a course granularity level of the interface.</para>
    /// 
    /// <para>Conceptually, IElementSet is composed of an ordered list of elements having a common type.
    /// The geometry of each element can be described by an ordered list of vertices. The shape of
    /// three-dimensional elements (i.e. volumes or polyhedrons) can be queried by face. If the element set
    /// is geo-referenced (i.e. the SpatialReference is not Null), co-ordinates (X,Y,Z) can be obtained for
    /// each vertex of an element. The ElementType is an enumeration, listed in Table 1. Data not related
    /// to spatial representation can be described by composing an element set containing one (or more)
    /// IDBased elements, without any geo-reference.</para>
    /// 
    /// <para>Note that IElementSet can be used to query the geometric description of a model schematization,
    /// but an implementation does not necessarily provide all topological knowledge on inter-element
    /// connections.</para>
    /// 
    /// <para>The interface of a spatial reference (ISpatialReference) only contains a string ID. No other
    /// properties and methods have been defined, as the OpenGIS SpatialReferenceSystem
    /// specification (OGC 2002) provides an excellent standard for this purpose.</para>
    /// 
    /// <para>The element set and the element are identified by a string ID. The ID is intended to be useful
    /// in terms of an end user. This is particularly useful for configuration as well as for providing
    /// specific logging information. However, the properties of an element (its vertices and/or faces)
    /// are obtained using an integer index (elementIndex, faceIndex and vertexIndex). This functionality
    /// is introduced because an element set is basically an ordered list of elements, an element may have
    /// faces and an element (or a face) is an ordered list of vertices. The integer index indicates the
    /// location of the element/vertex in the array list.</para>
    /// 
    /// <para>While most models encapsulate static element sets, some advanced models might contain dynamic
    /// elements (e.g. waves). A version number has been introduced to enable tracking of changes over
    /// time. If the version changes, the element set might need to be queried again during the computation
    /// process.</para>
	/// </summary>

	public interface IElementSet
	{

		/// <summary>
		/// <para>Identification string</para>
        /// 
        /// <para>EXAMPLE:</para>
        /// <para>"River Branch 34", "Node 34"</para>
		/// </summary>
		string ID {get;}


		/// <summary>
		/// Additional descriptive information
		/// </summary>
		string Description {get;}


		/// <summary>
        /// <para>The SpatialReference defines the spatial reference to be used in association with
        /// the coordinates in the ElementSet. For all ElementSet Types except ElementType.IDBased
        /// a spatial reference must be defined. For ElementSets of type ElementType.IDBased the
        /// SpatialReference property may be null.</para>
        /// 
        /// <para>EXAMPLE:</para>
        /// <para>SpatialReference.ID = "WG84" or "Local coordinate system"</para>
        /// <para></para>
        /// <para></para>
		/// </summary>
        ISpatialReference SpatialReference {get;}


		/// <summary>
		/// ElementType of the elementset. 
		/// </summary>
        ElementType ElementType {get;}


		/// <summary>
		/// Number of elements in the ElementSet
		/// </summary>
		int ElementCount {get;}


		/// <summary>
        /// The current version number for the populated ElementSet.
        /// The version must be incremented if anything inside the ElementSet is changed.
		/// </summary>
		int Version {get;}


		/// <summary>
        /// Index of element 'ElementID' in the elementset. Indexes start from zero.
        /// There are not restrictions to how elements are ordered.
		/// </summary>
        /// <param name="elementID">
        /// Identification string for the element for which the element index is requested.
        /// If no element in the ElementSet has the specified elementID, an exception must be thrown.
        /// .</param>
		int GetElementIndex(string elementID);


		/// <summary>
        /// Returns ID of 'ElementIndex'-th element in the ElementSet. Indexes start from zero.
		/// </summary>
        /// <param name="elementIndex">
        /// The element index for which the element ID is requested. If the element index is outside
        /// the range [0, number of elements -1], and exception must be thrown.
        /// .</param>
		string GetElementID(int elementIndex);


		/// <summary>
        /// <para>Number of vertices for the element specified by the elementIndex.</para>
        /// 
        /// <para>If the GetVertexCount()method is invoked for ElementSets of type ElementType.IDBased, an exception
        /// must be thrown.</para>
		/// </summary>
        /// 
		/// <param name="elementIndex">
        /// <para>The element index for the element for which the number of vertices is requested.</para>
        /// 
        /// <para>If the element index is outside the range [0, number of elements -1], and exception
        /// must be thrown.</para>
        /// .</param>
		/// <returns>Number of vertices in element defined by the elementIndex.</returns>
		int GetVertexCount(int elementIndex);

		/// <summary>
		/// Returns the number of faces in an element.
		/// </summary>
		/// <param name="elementIndex">
        /// <para>Index for the element</para>
        /// 
        /// <para>If the element index is outside the range [0, number of elements -1], and exception
        /// must be thrown.</para>
        /// .</param>
		/// <returns>Number of faces.</returns>
		int GetFaceCount (int elementIndex);


		/// <summary>
		/// Gives an array with the vertex indices for a face.
		/// </summary>
		/// <param name="elementIndex">Element index.</param>
		/// <param name="faceIndex">Face index.</param>
		/// <returns>The vertex indices for this face.</returns>
		int[] GetFaceVertexIndices(int elementIndex, int faceIndex);

		/// <summary>
		/// <para>X-coord for the vertex with VertexIndex of the element with ElementIndex</para>
		/// </summary>
		/// <param name="elementIndex">element index.</param>
		/// <param name="vertexIndex">vertex index in the element with index ElementIndex.</param>
		
		double GetXCoordinate(int elementIndex, int vertexIndex);
		/// <summary>
		/// Y-coord for the vertex with VertexIndex of the element with ElementIndex. 
		/// </summary>
		/// <param name="elementIndex">element index.</param>
		/// <param name="vertexIndex">vertex index in the element with index ElementIndex.</param>
		
		double GetYCoordinate(int elementIndex, int vertexIndex);

		/// <summary>
		/// Z-coord for the vertex with VertexIndex of the element with ElementIndex.
		/// </summary>
		/// <param name="elementIndex">element index.</param>
		/// <param name="vertexIndex">vertex index in the element with index ElementIndex.</param>
		double GetZCoordinate(int elementIndex, int vertexIndex);

	}

}
