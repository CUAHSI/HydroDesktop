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
	/// The link is used to describe the data transfer between
	/// linkable components.
    /// <para>This is a trivial implementation of OpenMI.Standard.ILink, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class Link : ILink
	{	
		private ArrayList _dataOperations = new ArrayList();
		private string _description = "";
		private string _id = "";
		private ILinkableComponent _sourceComponent;
		private IElementSet _sourceElementSet;
		private IQuantity _sourceQuantity;
		private ILinkableComponent _targetComponent;
		private IElementSet _targetElementSet;
		private IQuantity _targetQuantity;

		/// <summary>
		/// Constructor
		/// </summary>
		public Link()
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The link to copy</param>
		public Link(ILink source)
		{
			SourceComponent = source.SourceComponent;
			SourceElementSet = source.SourceElementSet;
			SourceQuantity = source.SourceQuantity;
			TargetComponent = source.TargetComponent;
			TargetElementSet = source.TargetElementSet;
			TargetQuantity = source.TargetQuantity;
			Description = source.Description;
			ID = source.ID;
			for (int i=0;i<source.DataOperationsCount;i++)
				AddDataOperation(source.GetDataOperation(i));
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="SourceComponent">The source component</param>
		/// <param name="SourceElementSet">The source element set</param>
		/// <param name="SourceQuantity">The source quantity</param>
		/// <param name="TargetComponent">The target component</param>
		/// <param name="TargetElementSet">The target element set</param>
		/// <param name="TargetQuantity">The target quantity</param>
		/// <param name="Description">The description</param>
		/// <param name="ID">The ID</param>
		/// <param name="DataOperations">Data operations to be carried out by the provider</param>
		public Link(ILinkableComponent SourceComponent,
			IElementSet SourceElementSet,
			IQuantity SourceQuantity,
			ILinkableComponent TargetComponent,
			IElementSet TargetElementSet,
			IQuantity TargetQuantity,
			string Description,
			string ID,
			ArrayList DataOperations)
		{
			_sourceComponent = SourceComponent;
			_sourceElementSet = SourceElementSet;
			_sourceQuantity = SourceQuantity;
			_targetComponent = TargetComponent;
			_targetElementSet = TargetElementSet;
			_targetQuantity = TargetQuantity;
			_description = Description;
			_id = ID;
			_dataOperations = DataOperations;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="SourceComponent">The source component</param>
		/// <param name="SourceElementSet">The source element set</param>
		/// <param name="SourceQuantity">The source quantity</param>
		/// <param name="TargetComponent">The target component</param>
		/// <param name="TargetElementSet">The target element set</param>
		/// <param name="TargetQuantity">The target quantity</param>
		/// <param name="ID">The ID</param>
		public Link(ILinkableComponent SourceComponent,
			IElementSet SourceElementSet,
			IQuantity SourceQuantity,
			ILinkableComponent TargetComponent,
			IElementSet TargetElementSet,
			IQuantity TargetQuantity,
			string ID)
		{
			_sourceComponent = SourceComponent;
			_sourceElementSet = SourceElementSet;
			_sourceQuantity = SourceQuantity;
			_targetComponent = TargetComponent;
			_targetElementSet = TargetElementSet;
			_targetQuantity = TargetQuantity;
			_description = Description;
			_id = ID;
		}

		/// <summary>
		/// Getter and setter for the link ID
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
		/// Getter and setter for the link description
		/// </summary>
		public string Description
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
		/// Getter and setter for the source component
		/// </summary>
		public ILinkableComponent SourceComponent
		{
			get
			{
				return _sourceComponent;
			}
			set
			{
				_sourceComponent = value;
			}
		}

		/// <summary>
		/// Getter and setter for the source quantity
		/// </summary>
		public IQuantity SourceQuantity
		{
			get
			{
				return _sourceQuantity;
			}
			set
			{
				_sourceQuantity = value;
			}
		}

		/// <summary>
		/// Getter and setter for the source element set
		/// </summary>
		public IElementSet SourceElementSet
		{
			get
			{
				return _sourceElementSet;
			}
			set
			{
				_sourceElementSet = value;
			}
		}

		/// <summary>
		/// Getter and setter for the target component
		/// </summary>
		public ILinkableComponent TargetComponent
		{
			get
			{
				return _targetComponent;
			}
			set
			{
				_targetComponent = value;
			}
		}

		/// <summary>
		/// Getter and setter for the target quantity
		/// </summary>
		public IQuantity TargetQuantity
		{
			get
			{
				return _targetQuantity;
			}
			set
			{
				_targetQuantity = value;
			}
		}

		/// <summary>
		/// Getter and setter for the target element set
		/// </summary>
		public IElementSet TargetElementSet
		{
			get
			{
				return _targetElementSet;
			}
			set
			{
				_targetElementSet = value;
			}
		}

		/// <summary>
		/// The number of data operations
		/// </summary>
		public int DataOperationsCount
		{
			get
			{
				return _dataOperations.Count;
			}
		}

		/// <summary>
		/// Adds a data operation
		/// </summary>
		/// <param name="dataOperation">The data operation</param>
		public void AddDataOperation (IDataOperation dataOperation)
		{
			if ( ! (dataOperation is ICloneable ) )
			{
				// Data Operation can not be cloned, issue warning
				Event warning = new Event(EventType.Warning);
				warning.Description = "DataOperation " + dataOperation.ID + " can not be cloned yet!";
				warning.Sender = _sourceComponent;
				_sourceComponent.SendEvent(warning);

				_dataOperations.Add(dataOperation);
			}
			else
			{
				_dataOperations.Add(((ICloneable)dataOperation).Clone());
			}
		}


		/// <summary>
		/// Gets a data operation
		/// </summary>
		/// <param name="DataOperationIndex">The index of the data operation</param>
		/// <returns>The data operation</returns>
		public IDataOperation GetDataOperation(int DataOperationIndex)
		{
			return (IDataOperation) _dataOperations[DataOperationIndex];
		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			Link link = (Link) obj;
			return (ID.Equals(link.ID));
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_sourceQuantity != null) hashCode += _sourceQuantity.GetHashCode();
			if (_sourceElementSet != null) hashCode += _sourceElementSet.GetHashCode();
			if (_targetQuantity != null) hashCode += _targetQuantity.GetHashCode();
			if (_targetElementSet != null) hashCode += _targetElementSet.GetHashCode();
			return hashCode;
		}
	}
}
 
