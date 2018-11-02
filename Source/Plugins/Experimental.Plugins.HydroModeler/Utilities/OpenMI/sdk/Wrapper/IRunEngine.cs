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
using System.Runtime.Remoting;  

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The IRunEngine is the interface the ModelEngine component
	/// The Class Oatc.OpenMI.Sdk.Wrapper.LinkableRunEngine will access the
	/// model engine component through this interface.
	/// </summary>
	/// <remarks>
	/// None 
	/// </remarks>
	public interface  IRunEngine
	{
	/// <summary>
	/// Initialize will typically be invoked just after creation of the object
	/// that implements the IRunEngine interface.
	/// </summary>
	/// <param name="properties">
	/// Hashtable with the same contents as the Component arguments
	/// in the ILinkableComponent interface. Typically any information
	/// needed for initialization of the model will be included in this table.
	/// This could be path and file names for input files.
	/// </param>
    void Initialize(Hashtable properties);
    
    /// <summary>
    /// This method will be invoked after all computations are completed. Deallocation of memory
    /// and closing files could be implemented in this method
    /// </summary>
    void Finish();
    
    /// <summary>
    /// This method will be invoked after all computations are completed
    /// and after the Finish method has been invoked
    /// </summary>
    void Dispose();
    
    /// <summary>
    /// This method will make the model engine perform one time step.
    /// </summary>
    /// <returns> Returns true if the time step was completed,
    /// otherwise it will return false
    /// </returns>
    bool PerformTimeStep();
    
    /// <summary>
    /// Get the current time of the model engine
    /// </summary>
    /// <returns>The current time for the model engine</returns>
    ITime GetCurrentTime();

    /// <summary>
    /// Get the time for which the next input is needed for
    /// a specific Quantity and ElementSet combination
    /// </summary>
    /// <param name="QuantityID">ID for the quantity</param>
    /// <param name="ElementSetID">ID for the ElementSet</param> 
    /// <returns>ITimeSpan or ITimeStamp	</returns>
    ITime GetInputTime(string QuantityID, string ElementSetID);

     /// <summary>
    /// Get earlist needed time, which can be used 
    /// to clear the buffer. For most time stepping model engines this
    /// time will be the time for the previous time step.
    /// </summary>
    /// <returns>TimeStamp</returns>
    ITimeStamp GetEarliestNeededTime();

    /// <summary>
    /// Sets values in the model engine
    /// </summary>
    /// <param name="QuantityID">quantityID associated to the values</param>
    /// <param name="ElementSetID">elementSetID associated to the values</param> 
    /// <param name="values">The values</param> 
    void SetValues(string QuantityID, string ElementSetID, IValueSet values);

    /// <summary>
    /// Gets values from the model engine
    /// </summary>
    /// <param name="QuantityID">quantityID associated to the requested values</param>
    /// <param name="ElementSetID">elementSetID associated to the requested values</param>  
    /// <returns>The requested values</returns>
    IValueSet GetValues(string QuantityID, string ElementSetID);
    
	/// <summary>
    /// In some situations a valied values cannot be return when the 
    /// Oatc.OpenMI.Sdk.Wrapper.IRunEngine.GetValues is invoked. In such case a missing values
    /// can be returned. The GetMissingValeusDefinition method can be used to query which definition
    /// of a missing value that applies to this particular model component. Example of missing value
    /// definition could be: -999.99
    /// </summary>
    /// <returns>Missing value definition</returns>
	double GetMissingValueDefinition();

   /// <summary>
   /// Get the ComponentID. The component ID is the name of the non-populated component. This is typically 
   /// the product name of your model engine.
   /// </summary>
   /// <returns>Component ID</returns>
    string GetComponentID();
    
   /// <summary>
   /// Get a description of your component. This description refers to the non-populated component. This is 
   /// typically a description of what your component does and which methods that are used. E.g. "Finite element
   /// based ground water model".
   /// </summary>
   /// <returns>Component description</returns>
    string GetComponentDescription();
   }
}
