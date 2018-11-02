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
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The class Oatc.OpenMI.Sdk.Wrapper.LinkableEngine will access the model engine through
	/// this interface.
	/// </summary>
	public interface  IEngine : IRunEngine
	{

		/// <summary>
		/// Returns the ModelID. The ModelID identifies the populated model component. 
		/// Example: "River Rhine"
		/// </summary>
		/// <returns>ModelID</returns>
		string GetModelID();

		/// <summary>
		/// Return the Model Description. The Model Description is a description of the populated
		/// model component.
		/// </summary>
		/// <returns>Model description</returns>
		string GetModelDescription();

		/// <summary>
		/// Return the time horison for the populated model compoent. The Time Horizon for a model i typically
		/// the same as the simulation period, which normally depend on de available input data. When you model 
		/// is running in the OpenMI environment, the model component must be able to return values within the 
		/// TimeHorizon
		/// </summary>
		/// <returns>TimeHorizon</returns>
        ITimeSpan GetTimeHorizon();

		/// <summary>
		/// Returns the number of input exchange items for the populated model component.
		/// </summary>
		/// <returns>InputExchangeItemCount</returns>
		int GetInputExchangeItemCount();

		/// <summary>
		/// Returns the number of output exchange items for the populated model component.
		/// </summary>
		/// <returns>OutputExchangeItemCount</returns>
		int GetOutputExchangeItemCount();
		
		/// <summary>
		/// Returns a specific output exchange item from the populated model component.
		/// </summary>
		/// <param name="exchangeItemIndex">index number</param>
		/// <returns>OutputExchangeItem according the the index number</returns>
		OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex);

		/// <summary>
		/// Returns a specific input exchange item from the populated model component.
		/// </summary>
		/// <param name="exchangeItemIndex">index number</param>
		/// <returns>InputExchangeItem according the the index number</returns>
		InputExchangeItem GetInputExchangeItem(int exchangeItemIndex);	
	}
}
