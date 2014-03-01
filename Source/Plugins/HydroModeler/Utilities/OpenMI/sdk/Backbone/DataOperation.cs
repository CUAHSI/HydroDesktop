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
	/// The DataOperation class contains operations the providing component should
	/// carry out on the data.
    /// <para>This is a trivial implementation of OpenMI.Standard.IDataOperation, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class DataOperation : IDataOperation
	{
		private string _id;
		private ArrayList _arguments = new ArrayList();

		/// <summary>
		/// Constructor
		/// </summary>
		public DataOperation()
		{
		}

		/// <summary>
		/// Initialize method
		/// </summary>
		/// <param name="properties">The arguments for the data operations</param>
		public void Initialize(IArgument[] properties) 
		{
			_arguments.AddRange (properties);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The Data Operation to copy</param>
		public DataOperation(IDataOperation source)
		{
			ID = source.ID;
			for (int i=0;i<source.ArgumentCount;i++)
				AddArgument(source.GetArgument(i));
		}

		/// <summary>
		/// Constructor with just a string ID
		/// </summary>
		/// <param name="ID">The ID</param>
		public DataOperation(
			string ID)
		{
			_id = ID;
		}

		/// <summary>
		/// Setter and getter methods for ID
		/// </summary>
		public virtual string ID
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
		/// Getter method for the argument count
		/// </summary>
		public virtual int ArgumentCount
		{
			get
			{
				return _arguments.Count;
			}
		}

		/// <summary>
		/// Setter and getter methods for the arguments
		/// </summary>
		public virtual ArrayList Arguments
		{
			get { 
				return _arguments;
			}
			set { 
				_arguments = value;
			}
		}

		/// <summary>
		/// Gets one argument
		/// </summary>
		/// <param name="argumentIndex">The index of the argument</param>
		/// <returns>The argument</returns>
		public virtual IArgument GetArgument(int argumentIndex)
		{
			return (IArgument) _arguments[argumentIndex];
		}


		/// <summary>
		/// Adds an argument
		/// </summary>
		/// <param name="argument">The argument to add</param>
		public virtual void AddArgument(
			IArgument argument
			)
		{
			_arguments.Add(argument);
		}

		/// <summary>
		/// Checks whether the current data operation is valid for the combination of
		/// input and output exchange items
		/// </summary>
		/// <param name="inputExchangeItem">The input exchange item</param>
		/// <param name="outputExchangeItem">The output exchange item</param>
		/// <param name="SelectedDataOperations">The selected data operations</param>
		/// <returns>True if the data operation is valid</returns>
		public virtual bool IsValid(IInputExchangeItem inputExchangeItem,IOutputExchangeItem outputExchangeItem,
			IDataOperation[] SelectedDataOperations)
		{
			return true;
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
			DataOperation d = (DataOperation)obj;
			if (!ID.Equals(d.ID)) return false;
			if (!ArgumentCount.Equals(d.ArgumentCount)) return false;
			for (int i=0;i<ArgumentCount;i++)
				if (!GetArgument(i).Equals(d.GetArgument(i)))
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
			return hashCode;
		}
	}
}
