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
using NUnit.Framework;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class LinkableComponentTest : IListener
	{
		bool eventSent;
		TestComponent testComponent1,testComponent2;
		private IEvent lastEvent = null;

		[SetUp]
		public void Init()
		{
			eventSent = false;
			testComponent1 = new TestComponent();

			testComponent2 = new TestComponent2();

			Link link1 = new Link();
			link1.ID ="Link1";
			link1.SourceComponent = testComponent1;
			link1.TargetComponent = testComponent2;
			Link link2 = new Link();
			link2.ID ="Link2";
			link2.SourceComponent = testComponent2;
			link2.TargetComponent = testComponent1;
			Link link3 = new Link();
			link3.ID = "Link3";
			link3.SourceComponent = link3.TargetComponent = testComponent1;

			testComponent1.AddLink(link1);
			testComponent1.AddLink(link2);
			testComponent1.AddLink(link3);
			testComponent2.AddLink(link1);
			testComponent2.AddLink(link2);
			testComponent1.RemoveLink("Link3");

			testComponent1.Subscribe(this,EventType.Informative);

			Quantity q = new Quantity("Q");
			ElementSet elementSet = new ElementSet();
			elementSet.ID = "ES";

			InputExchangeItem inputExchangeItem = new InputExchangeItem();
			inputExchangeItem.Quantity = q;
			inputExchangeItem.ElementSet = elementSet;
			testComponent1.AddInputExchangeItem(inputExchangeItem);

			OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
			outputExchangeItem.Quantity = new Quantity("Q2");
			ElementSet elementSet2 = new ElementSet();
			elementSet2.ID = "ES2";
			outputExchangeItem.ElementSet = elementSet;
			testComponent1.AddOutputExchangeItem(outputExchangeItem);
		}

		[Test]
		public void Links()
		{
			Link link1 = new Link();
			link1.ID ="Link1";
			Link link2 = new Link();
			link2.ID ="Link2";
			Assert.AreEqual(2,testComponent1.LinkCount);
			Assert.AreEqual(2,testComponent2.LinkCount);
			Assert.AreEqual(link2,testComponent1.GetAcceptingLinks()[0]);
			Assert.AreEqual(link1,testComponent1.GetProvidingLinks()[0]);
		}

		[Test]
		public void Events()
		{
			testComponent1.SendEvent(new Event(EventType.Informative));
			Assert.IsTrue(eventSent);
			testComponent1.UnSubscribe(this,EventType.Informative);
			eventSent = false;
			testComponent1.SendEvent(new Event(EventType.Informative));
			Assert.IsFalse(eventSent);
		}

		[Test]
		public void HasListeners()
		{
			Assert.IsTrue(testComponent1.HasListeners());
			Assert.IsFalse(testComponent2.HasListeners());
		}

		[Test]
		public void ExchangeItems()
		{
			Assert.AreEqual(1,testComponent1.InputExchangeItemCount);
			Assert.AreEqual(1,testComponent1.OutputExchangeItemCount);
			Quantity q = new Quantity("Q");

			ElementSet elementSet = new ElementSet();
			elementSet.ID = "ES";

			InputExchangeItem inputExchangeItem = new InputExchangeItem();
			inputExchangeItem.Quantity = q;
			inputExchangeItem.ElementSet = elementSet;
		    Assert.AreEqual(testComponent1.GetInputExchangeItem(0),inputExchangeItem);

			OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
			outputExchangeItem.Quantity = new Quantity("Q2");
			ElementSet elementSet2 = new ElementSet();
			elementSet2.ID = "ES2";
			outputExchangeItem.ElementSet = elementSet;
			Assert.AreEqual(testComponent1.GetOutputExchangeItem(0),outputExchangeItem);
		}

		[Test]
		public void TestNoneCloneableDataOperation()
		{
			lastEvent = null;

			testComponent1.Subscribe(this, EventType.Warning);

			IDataOperation nonCloneableDataOperation = new NoneCloneableDataOperation("nonCloneableDataOperation");

			Link link_a = new Link();
			link_a.ID ="Link a with NoneCloneableDataOperations";
			link_a.SourceComponent = testComponent1;
			link_a.TargetComponent = testComponent2;
			link_a.AddDataOperation(nonCloneableDataOperation);
			testComponent1.AddLink(link_a);
			testComponent2.AddLink(link_a);

			Link link_b = new Link();
			link_b.ID = "Link b with NoneCloneableDataOperations";
			link_b.SourceComponent = testComponent1;
			link_b.TargetComponent = testComponent2;
			link_b.AddDataOperation(nonCloneableDataOperation);

			lastEvent = null;
			testComponent2.AddLink(link_b);
			Assert.IsTrue(lastEvent == null, "lastEvent==null");
			testComponent1.AddLink(link_b);
			Assert.IsTrue(lastEvent != null, "lastEvent!=null");
			Assert.IsTrue(lastEvent.Type == EventType.Warning, "EventType.Warning");
			Assert.IsTrue(lastEvent.Description.ToLower().Contains("clone"), "clone");
		}

		[Test]
		public void Equals()
		{
			TestComponent component2 = new TestComponent();
			Assert.IsTrue(testComponent1.Equals(component2));
			component2 = new TestComponent2();
			Assert.IsFalse(testComponent1.Equals(component2));
		}

		#region IListener Members

		public EventType GetAcceptedEventType(int acceptedEventTypeIndex)
		{
			if ( acceptedEventTypeIndex == 0)
			{
				return EventType.Informative;
			}
			else
			{
				return EventType.Warning;
			}
		}

		public int GetAcceptedEventTypeCount()
		{
			return 2;
		}

		public void OnEvent(IEvent Event)
		{
			eventSent = true;
			lastEvent = Event;
		}

		#endregion

		#region private test class

		private class NoneCloneableDataOperation : IDataOperation
		{
			private string id;

			public NoneCloneableDataOperation(string id)
			{
				this.id = id;
			}

			public void Initialize(IArgument[] properties)
			{
				throw new NotImplementedException();
			}

			public IArgument GetArgument(int argumentIndex)
			{
				throw new NotImplementedException();
			}

			public bool IsValid(IInputExchangeItem inputExchangeItem, IOutputExchangeItem outputExchangeItem,
								IDataOperation[] SelectedDataOperations)
			{
				throw new NotImplementedException();
			}

			public string ID
			{
				get { return id; }
			}

			public int ArgumentCount
			{
				get { throw new NotImplementedException(); }
			}
		}
		#endregion
	}
}
