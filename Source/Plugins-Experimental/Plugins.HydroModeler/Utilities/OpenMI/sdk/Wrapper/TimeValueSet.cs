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
	/// Combined ITime and IValueSet. Used in connection with the IAdvancedEngine interface
	/// </summary>
    public class TimeValueSet
    {
        ITime       _time;
        IValueSet   _valueSet;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="time">ITime object</param>
        /// <param name="valueSet">IValuesSet object</param>
        public TimeValueSet(ITime time, IValueSet valueSet)
        {
            _time     = time;
            _valueSet = valueSet;
        }

        /// <summary>
        /// The time
        /// </summary>
        public ITime Time
        {
            get
            {
                return _time;
            }
        }

        /// <summary>
        /// The values
        /// </summary>
        public IValueSet ValueSet
        {
            get
            {
                return _valueSet;
            }

         }
	}
}
