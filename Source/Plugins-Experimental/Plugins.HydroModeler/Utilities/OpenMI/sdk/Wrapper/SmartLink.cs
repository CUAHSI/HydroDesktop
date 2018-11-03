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
using System.Runtime.Remoting; 
using OpenMI.Standard;  
using Oatc.OpenMI.Sdk.Backbone;
   
namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The SmartLink contains the link and a reference to the engine
	/// </summary>
	[Serializable]
	public abstract class SmartLink 
	{

		/// <summary>
		/// Reference to the Link
		/// </summary>
		protected ILink _link;

		/// <summary>
		/// Reference to the engine
		/// </summary>
		protected IRunEngine _engine;


		/// <summary>
		/// The ILink object contained in the SmartLink
		/// </summary>
		public ILink link
		{
			get
			{
				return _link;
			}
		}

		/// <summary>
		/// Reference to the engine
		/// </summary>
		public IRunEngine Engine
		{
			get
			{
				return _engine;
			}
		}

		/// <summary>
		/// Error messages 
		/// </summary>
		/// <returns>Error Messages</returns>
        public virtual string[] GetErrors()
		{
			ILink link = this.link;
			ArrayList messages = new ArrayList();

			// check valuetype
			if (link.SourceQuantity.ValueType != global::OpenMI.Standard.ValueType.Scalar || link.TargetQuantity.ValueType != global::OpenMI.Standard.ValueType.Scalar)
			{
				if (this is SmartInputLink) 
				{
					messages.Add("Component " + link.TargetComponent.ComponentID + "does not support VectorSets");
				}
				else 
				{
					messages.Add("Component " + link.SourceComponent.ComponentID + "does not support VectorSets");
				}
			}

			// check unit
			if (link.SourceQuantity.Unit == null || link.TargetQuantity.Unit == null)
			{
				messages.Add("Unit  equals null in link from " + link.SourceComponent.ModelID + " to " + link.TargetComponent.ModelID);
			}
			else if (link.SourceQuantity.Unit.ConversionFactorToSI == 0.0 || link.TargetQuantity.Unit.ConversionFactorToSI == 0)
			{
				messages.Add("Unit conversion factor equals zero in link from " + link.SourceComponent.ModelID + " to " + link.TargetComponent.ModelID);
			}

			return (string[]) messages.ToArray(typeof(string));
		}

		/// <summary>
		/// Warining
		/// </summary>
		/// <returns>warnings</returns>
        public virtual string[] GetWarnings()
		{
			ILink link = this.link;
			ArrayList messages = new ArrayList();

			// check dimension
			if( ! CompareDimensions(link.SourceQuantity.Dimension, link.TargetQuantity.Dimension))
			{
				messages.Add("Different dimensions used in link from " + link.SourceComponent.ModelID + " to " + link.TargetComponent.ModelID);
			}

			return (string[]) messages.ToArray(typeof(string));
		}

		private bool CompareDimensions(IDimension dimension1, IDimension dimension2)
		{
			bool isSameDimension = true;

			for (int i = 0; i < (int)DimensionBase.NUM_BASE_DIMENSIONS; i++)
			{
				if (dimension1.GetPower((DimensionBase) i) != dimension2.GetPower((DimensionBase) i))
				{
					isSameDimension = false;
				}
			}
			return isSameDimension;
		}
	}
}
