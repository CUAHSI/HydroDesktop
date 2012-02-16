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

namespace Oatc.OpenMI.Sdk.Backbone
{
	/// <summary>
	/// The ElementSet class describes a collection of spatial elements.
    /// <para>This is a trivial implementation of OpenMI.Standard.IElementSet, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class ElementSet:IElementSet
	{
		private ArrayList _elements = new ArrayList();
		private string _description="";
		private string _id="";
		private ElementType _elementType = new ElementType();
		private ISpatialReference _spatialReference = new SpatialReference();

		/// <summary>
		/// Constructor
		/// </summary>
		public ElementSet()
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The element set to copy</param>
		public ElementSet(IElementSet source)
		{
			_description = source.Description;
			_id = source.ID;
			_elementType = source.ElementType;
		    _spatialReference = source.SpatialReference;

			for (int i=0;i<source.ElementCount;i++) 
			{
				Element element = new Element(source.GetElementID(i));
				for (int j=0;j<source.GetVertexCount(i);j++) 
				{
					double x = source.GetXCoordinate(i,j);
					double y = source.GetYCoordinate(i,j);
					double z = source.GetZCoordinate(i,j);

					element.AddVertex(new Vertex(x,y,z));
				}
				_elements.Add(element);
			}
		}

		/// <summary>
		/// ElementSet version
		/// </summary>
		public virtual int Version 
		{
			get { return 0;}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Description">Description</param>
		/// <param name="ID">ID</param>
		/// <param name="ElementType">Element type</param>
		/// <param name="SpatialReference">Spatial reference</param>
		public ElementSet(string Description, string ID,
			ElementType ElementType,ISpatialReference SpatialReference)
		{
			_description = Description;
			_id = ID;
			_elementType = ElementType;
			_spatialReference = SpatialReference;
		}

		/// <summary>
		/// Adds an element
		/// </summary>
		/// <param name="element">The element to add</param>
		public virtual void AddElement (Element element)
		{
			_elements.Add(element);
		}

		/// <summary>
		/// Gets an element
		/// </summary>
		/// <param name="ElementIndex">The element index</param>
		/// <returns>The element</returns>
		public virtual Element GetElement(int ElementIndex)
		{
			return (Element) _elements[ElementIndex];

		}

		/// <summary>
		/// Getter and setter functions for the element list
		/// </summary>
		public virtual Element[] Elements
		{
			get
			{
				Element[] elements = new Element[_elements.Count];
				for (int i=0;i<_elements.Count;i++) 
				{
					elements[i] = (Element) _elements[i];
				}
				return elements;
			}
			set
			{
				_elements.Clear();
				for (int i=0;i<value.Length;i++) 
				{
					_elements.Add(value[i]);
				}
			}
		}

		/// <summary>
		/// Returns an element ID for an element
		/// </summary>
		/// <param name="ElementIndex">The element index</param>
		/// <returns>The element ID</returns>
		public virtual string GetElementID(int ElementIndex)
		{
			Element element = (Element) _elements[ElementIndex];
			return element.ID;
		}

		/// <summary>
		/// Setter and getter for the element set description
		/// </summary>
		public virtual string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Setter and getter for the element set ID
		/// </summary>
		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Setter and getter for the element type
		/// </summary>
		public virtual ElementType ElementType
		{
			get
			{
				return _elementType;
			}
			set
			{
				_elementType = value;
			}
		}

		/// <summary>
		/// Returns the x coordinate for a vertex
		/// </summary>
		/// <param name="ElementIndex">The element index</param>
		/// <param name="VertexIndex">The vertex index</param>
		/// <returns>The x coordinate</returns>
		public virtual double GetXCoordinate(int ElementIndex, int VertexIndex)
		{
			Element element = (Element) _elements[ElementIndex];
			Vertex vertex = element.GetVertex(VertexIndex);
			return vertex.x;
		}

		/// <summary>
		/// Returns the y coordinate for a vertex
		/// </summary>
		/// <param name="ElementIndex">The element index</param>
		/// <param name="VertexIndex">The vertex index</param>
		/// <returns>The y coordinate</returns>
		public virtual double GetYCoordinate(int ElementIndex, int VertexIndex)
		{
			Element element = (Element) _elements[ElementIndex];
			Vertex vertex = element.GetVertex(VertexIndex);
			return vertex.y;
		}

		/// <summary>
		/// Returns the z coordinate for a vertex
		/// </summary>
		/// <param name="ElementIndex">The element index</param>
		/// <param name="VertexIndex">The vertex index</param>
		/// <returns>The z coordinate</returns>
		public virtual double GetZCoordinate(int ElementIndex, int VertexIndex)
		{
			Element element = (Element) _elements[ElementIndex];
			Vertex vertex = element.GetVertex(VertexIndex);
			return vertex.z;
		}

		/// <summary>
		/// Returns the number of elements
		/// </summary>
		public virtual int ElementCount
		{
			get
			{
				return _elements.Count;
			}
		}

		/// <summary>
		/// Returns the number of vertices for an element
		/// </summary>
		/// <param name="ElementIndex">The element index</param>
		/// <returns>The number of vertices for this element</returns>
		public virtual int GetVertexCount(int ElementIndex)
		{
			Element element = (Element) _elements[ElementIndex];
			return element.VertexCount;
		}

		/// <summary>
		/// Getter and setter for the spatial reference
		/// </summary>
		public virtual ISpatialReference SpatialReference
		{
			get
			{
				return _spatialReference;
			}
			set
			{
				_spatialReference = value;
			}
		}

		/// <summary>
		/// Returns the element index for a given element ID
		/// </summary>
		/// <param name="ElementID">The element ID</param>
		/// <returns>The element index</returns>
		public virtual int GetElementIndex(string ElementID)
		{	
			for (int i=0;i<_elements.Count;i++)
			{
				Element element = (Element) _elements[i];
				if (element.ID.Equals(ElementID)) 
				{
					return i;
				}
			}
			throw new Exception("Element with ID "+ElementID+ " not found.");
		}

		/// <summary>
		/// Returns the list of face vertex indices for a given element and face
		/// </summary>
		/// <param name="elementIndex">The element index</param>
		/// <param name="faceIndex">The face index</param>
		/// <returns>List of face vertex indices</returns>
		public int[] GetFaceVertexIndices(int elementIndex, int faceIndex)
		{
			return ((Element)_elements[elementIndex]).GetFaceVertexIndices(faceIndex);
		}

		/// <summary>
		/// Returns the face count for a given element
		/// </summary>
		/// <param name="elementIndex">The element index</param>
		/// <returns>The face count for the given element</returns>
		public int GetFaceCount(int elementIndex)
		{
			return ((Element)_elements[elementIndex]).FaceCount;
		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(Object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;
			ElementSet s = (ElementSet)obj;
			if (!Description.Equals(s.Description))
				return false;
			if (!ID.Equals(s.ID))
				return false;
			if (!SpatialReference.Equals(s.SpatialReference))
				return false;
			if (!ElementType.Equals(s.ElementType))
				return false;
			if (ElementCount != s.ElementCount)
				return false;
			for (int i = 0; i < ElementCount; i++)
				if (!GetElement(i).Equals(s.GetElement(i)))
					return false;
			return true;
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_id != null) hashCode += _id.GetHashCode();
			hashCode += _elementType.GetHashCode();
			return hashCode;
		}

		///<summary>
		/// String representation of the 
		///</summary>
		///<returns></returns>
		public override string ToString()
		{
			return ID;
		}

	}
}
