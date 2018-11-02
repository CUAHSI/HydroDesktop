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

namespace Oatc.OpenMI.Sdk.Backbone
{

	/// <summary>
	/// The TimeSpan class defines a time span given a
	/// start and end time.
    /// <para>This is a trivial implementation of OpenMI.Standard.ITimeSpan, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class TimeSpan  : ITimeSpan
	{	
		private ITimeStamp _start;
		private ITimeStamp _end;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Start">Start time</param>
		/// <param name="End">End time</param>
		public TimeSpan(ITimeStamp Start, ITimeStamp End)
		{
			_start = Start;
			_end = End;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The time span to copy</param>
		public TimeSpan(ITimeSpan source)
		{
			Start = new TimeStamp(source.Start);
			End = new TimeStamp(source.End);
		}

		/// <summary>
		/// Getter and setter for start time
		/// </summary>
		public ITimeStamp Start 
		{
			get { return _start;}
			set
			{
				_start = value;
			}
		}

		/// <summary>
		/// Getter and setter for end time
		/// </summary>
		public ITimeStamp End
		{
			get { return _end;}
			set
			{
				_end = value;
			}
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
			TimeSpan s = (TimeSpan) obj;
			if (!Start.Equals(s.Start))
				return false;
			if (!End.Equals(s.End))
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
			if (_start != null) hashCode += _start.GetHashCode();
			if (_end != null) hashCode += _end.GetHashCode();
			return hashCode;
		}
	}
}
 
