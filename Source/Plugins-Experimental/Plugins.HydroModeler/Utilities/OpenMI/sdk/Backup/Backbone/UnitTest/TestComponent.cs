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
using NUnit.Framework;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	public class TestComponent : LinkableComponent
	{
		public override string ComponentDescription
		{
			get
			{
				return "ComponentDescription";
			}
		}

		public override string ComponentID
		{
			get
			{
				return "ComponentID";
			}
		}
		public override string ModelID
		{
			get
			{
				return "ModelID";
			}
		}
		public override string ModelDescription
		{
			get
			{
				return "ModelDescription";
			}
		}

		public override void Initialize(IArgument[] properties)
		{
		}

		public override ITimeStamp EarliestInputTime
		{
			get
			{
				return null;
			}
		}

		public override ITimeSpan TimeHorizon
		{
			get
			{
				return null;
			}
		}

		public override EventType GetPublishedEventType(int providedEventTypeIndex)
		{
			return new EventType ();
		}

		public override int GetPublishedEventTypeCount()
		{
			return 0;
		}

		public override IValueSet GetValues(ITime time, string LinkID)
		{
			return null;
		}

		public override string Validate()
		{
			return "";
		}

		public override void Prepare()
		{
		}

		
		public override void Finish()
		{
		}

	}

	public class TestComponent2 : TestComponent
	{
		public override string ComponentID
		{
			get
			{
				return "ComponentID2";
			}
		}

	}
}
