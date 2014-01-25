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
using System.IO;
using System.Collections;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Buffer;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for TimeSeriesComponentTest.
	/// </summary>
	[TestFixture]
	public class TimeSeriesComponentTest
	{
		[Test]
		public void ComponentID()
		{
			TimeSeriesComponent ts = new TimeSeriesComponent();
			Assert.AreEqual("TimeSeriesComponentID",ts.ComponentID);
		}

		[Test]
		public void GetValues()
		{
			Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();
			TimeSeriesComponent ts = new TimeSeriesComponent();

			ts.Initialize(new Argument[0]);
			Link triggerLink = new Link();

			triggerLink.ID				 = "TriggerLink";
			triggerLink.SourceComponent  = ts;
			triggerLink.SourceElementSet = ts.GetOutputExchangeItem(0).ElementSet;
			triggerLink.SourceQuantity   = ts.GetOutputExchangeItem(0).Quantity;
			triggerLink.TargetComponent  = trigger;
			triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
			triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;

			trigger.AddLink(triggerLink);
			ts.AddLink(triggerLink);

			double tt = ts.TimeHorizon.Start.ModifiedJulianDay;
			TimeStamp[] triggerTimes = new TimeStamp[4];
			triggerTimes[0] = new TimeStamp(tt + 0.5);
			triggerTimes[1] = new TimeStamp(tt + 1.5);
			triggerTimes[2] = new TimeStamp(tt + 1.9);
			triggerTimes[3] = new TimeStamp(tt + 2.1);

			trigger.Run(triggerTimes);
			Assert.AreEqual(0,((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(1,((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0));
			Assert.AreEqual(1,((IScalarSet)trigger.ResultsBuffer.GetValuesAt(2)).GetScalar(0));
			Assert.AreEqual(2,((IScalarSet)trigger.ResultsBuffer.GetValuesAt(3)).GetScalar(0));

			//Teting with timespans
			Assert.AreEqual(2.0,((ScalarSet) ts.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(tt+2),new TimeStamp(tt + 2.0 + 1.0/24.0)),"TriggerLink")).GetScalar(0));
			Assert.AreEqual(2.0,((ScalarSet) ts.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(tt + 2.0 + 1.0/24.0),new TimeStamp(tt + 2.0 + 2.0/24.0)),"TriggerLink")).GetScalar(0));
			Assert.AreEqual(2.0,((ScalarSet) ts.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(tt + 2.0 + 2.0/24.0),new TimeStamp(tt + 2.0 + 3.0/24.0)),"TriggerLink")).GetScalar(0));
			Assert.AreEqual(2.0,((ScalarSet) ts.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(tt + 2.0 + 5.0/24.0),new TimeStamp(tt + 2.0 + 6.0/24.0)),"TriggerLink")).GetScalar(0));
			Assert.AreEqual(2.0,((ScalarSet) ts.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(tt + 2.0 + 15.0/24.0),new TimeStamp(tt + 2.0 + 16.0/24.0)),"TriggerLink")).GetScalar(0));
			Assert.AreEqual(2.0,((ScalarSet) ts.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(tt + 2.0 + 23.0/24.0),new TimeStamp(tt + 2.0 + 24.0/24.0)),"TriggerLink")).GetScalar(0));
			Assert.AreEqual(3.0,((ScalarSet) ts.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(tt + 3.0 ),new TimeStamp(tt + 3.0 + 1.0/24.0)),"TriggerLink")).GetScalar(0));
		
		}
	}
}
