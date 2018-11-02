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

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The IAdvancedEngine interface is introduced in order to facilitate models
	/// where different quantities are calculated base on different time step lengths.
	/// One example of such model could be a multi-domain model such a models for combined
	/// ground water and surface water. Typically the time step length for the ground 
	/// water calculations will be much longer that the time step length for the surface 
	/// water calculations. In the IEngine interface values are pulled from the engine 
	/// through the GetValues method, which returns a IValueSet. In this cases it will 
	/// be assumed that the accociated time is the current time which is obtained through 
	/// the IEngine interface through the GetCurrentTime. By use of the IAdvanceEngine 
	/// interface accociated values can be pulled from the engine through the 
	/// GetValues method that will return an instance of the TimeValues class, 
	/// which contains a IValueSet and the associated ITime. The IAdvanced i
	/// nterface is implemented as a separate interface in order to facilitate 
	/// backward compatibility. 
	/// Summary description for IAdvancedEngine.
	/// </summary>
	public interface IAdvancedEngine : IEngine
	{
        /// <summary>
        /// The GetValues method will return an instance of the TimeValues class, 
        /// which is the currently calculated values as IValueSet and the associated time as ITime.
        /// </summary>
        /// <param name="quantityID">The Quantity ID for the requested values</param>
        /// <param name="ElementSetID">The ElementSet ID for the requested values</param>
        /// <returns>TimeValueSet which is the calculated values and the associated time</returns>
        new TimeValueSet GetValues(string quantityID, string ElementSetID);
	}
}
