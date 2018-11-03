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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Spatial;


namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
    /// The LinkableEngine class inplements the ILinkableComponent interface.
    /// In normal usage of the Wrapper package, the person migrating a model will inherit  
    /// his own class from this class. The LinkableEngine class is an abstract class due to the 
    /// abstract method SetEngineApiAccess. This method must be overridden in the derived class.
    /// 
    /// Implementation of the ILinkableComponent methods are done partly in this class (LinkebleEngine class) 
    /// and partly in the parent class – LinkableRunEngine class. There are historical reasons for 
    /// dividing the implementation into two classes, and basically the LinkebleEngine class and the 
    /// LinkebleRunEngine class could be merged. However, in order to keep the backward compatibility 
    /// the two classes still exists.   
	/// </summary>
	public abstract class LinkableEngine : LinkableRunEngine	
	{
		/// <summary>
		/// constructor
		/// </summary>
        public LinkableEngine()
		{
		}

		#region IExchangeModel Members

		/// <summary>
		/// Number of input exchange items
		/// </summary>
        public override int InputExchangeItemCount
		{
			get
			{
				return ((IEngine)_engineApiAccess).GetInputExchangeItemCount();
			}
		}

		/// <summary>
		/// Get an input exchange item
		/// </summary>
		/// <param name="index">index of the requested input exchange item</param>
		/// <returns>The input exchange item</returns>
        public override IInputExchangeItem GetInputExchangeItem(int index)
		{
			return (IInputExchangeItem) ((IEngine)_engineApiAccess).GetInputExchangeItem(index);
		}

		/// <summary>
		/// Number of output exchange items
		/// </summary>
        public override int OutputExchangeItemCount
		{
			get
			{
				return ((IEngine)_engineApiAccess).GetOutputExchangeItemCount();
			}
		}

		/// <summary>
		/// get a output exchange item
		/// </summary>
		/// <param name="index">index number of the requested output exchange item</param>
		/// <returns>The requested exchange item</returns>
        public override IOutputExchangeItem GetOutputExchangeItem(int index)
		{
			OutputExchangeItem outputExchangeItem = ((IEngine)_engineApiAccess).GetOutputExchangeItem(index);

			//Add dataoperations to outputExchangeItems
			ElementMapper elementMapper = new ElementMapper();
			ArrayList dataOperations = new ArrayList();
			dataOperations = elementMapper.GetAvailableDataOperations(outputExchangeItem.ElementSet.ElementType);
			bool spatialDataOperationExists;
			bool linearConversionDataOperationExists;
			bool smartBufferDataOperationExists;
			foreach (IDataOperation dataOperation in dataOperations)
			{
				spatialDataOperationExists = false;
				foreach (IDataOperation existingDataOperation in outputExchangeItem.DataOperations)
				{
					if (dataOperation.ID == existingDataOperation.ID)
					{
						spatialDataOperationExists = true;
					}
				}

				if (!spatialDataOperationExists)
				{
					outputExchangeItem.AddDataOperation(dataOperation);
				}
			}

			IDataOperation linearConversionDataOperation = new LinearConversionDataOperation();
			linearConversionDataOperationExists = false;
			foreach (IDataOperation existingDataOperation in outputExchangeItem.DataOperations)
			{
				if (linearConversionDataOperation.ID == existingDataOperation.ID)
				{
					linearConversionDataOperationExists = true;
				}
			}

			if (!linearConversionDataOperationExists)
			{
				outputExchangeItem.AddDataOperation(new LinearConversionDataOperation());
			}

			IDataOperation smartBufferDataOperaion = new SmartBufferDataOperation();
			smartBufferDataOperationExists = false;
			foreach (IDataOperation existingDataOperation in outputExchangeItem.DataOperations)
			{
				if (smartBufferDataOperaion.ID == existingDataOperation.ID)
				{
					smartBufferDataOperationExists = true;
				}
			}

			if (!smartBufferDataOperationExists)
			{
				outputExchangeItem.AddDataOperation(new SmartBufferDataOperation());
			}

			return (IOutputExchangeItem) outputExchangeItem;
		}


		/// <summary>
		/// Model description
		/// </summary>
        public override string ModelDescription
		{
			get
			{
				return ((Oatc.OpenMI.Sdk.Wrapper.IEngine) _engineApiAccess).GetModelDescription();
			}
		}

		/// <summary>
		/// Model ID
		/// </summary>
        public override string ModelID
		{
			get
			{
				if (_engineApiAccess != null) 
				{
					return ((Oatc.OpenMI.Sdk.Wrapper.IEngine) _engineApiAccess).GetModelID();
				}
				else 
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Time TimeHorizon for the model, which is the time period in which the model can be requested
		/// for values
		/// </summary>
        public override  ITimeSpan TimeHorizon
		{
			get
			{	
				return ((IEngine) _engineApiAccess).GetTimeHorizon();
			}
		}

		/// <summary>
		/// The SetEngineApiAccess() method is abstract and as such should be overridden in the derived class. 
		/// This method should set the reference to the class that implements the IEngine interface. 
		/// The property EngineApiAccess, which is implemented in the LinkableRunEngine class is used 
		/// to assign this reference.  
		/// </summary>
        protected override abstract void SetEngineApiAccess();

		#endregion

	
	}
}
