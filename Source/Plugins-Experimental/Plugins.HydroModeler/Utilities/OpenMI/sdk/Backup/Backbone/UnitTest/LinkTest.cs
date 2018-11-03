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
using NUnit.Framework;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class LinkTest : IListener
	{
		Link link1,link2;
		private IEvent lastEvent = null;

		[SetUp]
		public void Init()
		{
			TestComponent sourceComponent = new TestComponent();
			ElementSet sourceElementSet = new ElementSet("SourceElementSet",
				"SourceElementSetID",ElementType.XYPoint,new SpatialReference("SourceRef"));
			
			TestComponent targetComponent = new TestComponent2();
			ElementSet targetElementSet = new ElementSet("TargetElementSet",
				"TargetElementSetID",ElementType.XYPoint,new SpatialReference("TargetRef"));

			ArrayList dataOperations = new ArrayList();
			dataOperations.Add(new DataOperation("dataOperation"));

			link1 = new Link(sourceComponent,sourceElementSet,new Quantity("SourceQuantity"),
				targetComponent,targetElementSet,new Quantity("TargetQuantity"),"Link description","Link1",
				dataOperations);
			link2 = new Link();
			link2.SourceComponent = sourceComponent;
			link2.SourceElementSet = sourceElementSet;
			link2.SourceQuantity = new Quantity("SourceQuantity");
			link2.TargetComponent = targetComponent;
			link2.TargetElementSet = targetElementSet;
			link2.TargetQuantity = new Quantity("TargetQuantity");
			link2.Description = "Link description";
			link2.ID = "Link2";
			link2.AddDataOperation(new DataOperation("dataOperation"));
		}

		[Test]
		public void Constructor()
		{
			Link link3 = new Link(link1);
			Assert.AreEqual(link1,link3);
		}

		[Test]
		public void ID()
		{
			Assert.AreEqual("Link1",link1.ID);
			Assert.AreEqual("Link2",link2.ID);
		}

		[Test]
		public void Description()
		{
			Assert.AreEqual("Link description",link1.Description);
			Assert.AreEqual("Link description",link2.Description);
		}

		[Test]
		public void SourceComponent()
		{
			TestComponent component = new TestComponent();
			Assert.AreEqual(component,link1.SourceComponent);
			Assert.AreEqual(component,link2.SourceComponent);
		}

		[Test]
		public void SourceQuantity()
		{
			Assert.AreEqual(new Quantity("SourceQuantity"),link1.SourceQuantity);
			Assert.AreEqual(new Quantity("SourceQuantity"),link2.SourceQuantity);
		}

		[Test]
		public void SourceElementSet()
		{
			ElementSet sourceElementSet = new ElementSet("SourceElementSet",
				"SourceElementSetID",ElementType.XYPoint,new SpatialReference("SourceRef"));

			Assert.AreEqual(sourceElementSet,link1.SourceElementSet);
			Assert.AreEqual(sourceElementSet,link2.SourceElementSet);
		}

		[Test]
		public void TargetComponent()
		{
			TestComponent component = new TestComponent2();
			Assert.AreEqual(component,link1.TargetComponent);
			Assert.AreEqual(component,link2.TargetComponent);
		}

		[Test]
		public void TargetQuantity()
		{
			Assert.AreEqual(new Quantity("TargetQuantity"),link1.TargetQuantity);
			Assert.AreEqual(new Quantity("TargetQuantity"),link2.TargetQuantity);
		}

		[Test]
		public void TargetElementSet()
		{
			ElementSet targetElementSet = new ElementSet("TargetElementSet",
				"TargetElementSetID",ElementType.XYPoint,new SpatialReference("TargetRef"));

			Assert.AreEqual(targetElementSet,link1.TargetElementSet);
			Assert.AreEqual(targetElementSet,link2.TargetElementSet);
		}

		[Test]
		public void DataOperationCount()
		{
			Assert.AreEqual(1,link1.DataOperationsCount);
			Assert.AreEqual(1,link1.DataOperationsCount);
		}

		[Test]
		public void GetDataOperation()
		{
			Assert.AreEqual(new DataOperation("dataOperation"),
				link1.GetDataOperation(0));
			Assert.AreEqual(new DataOperation("dataOperation"),
				link2.GetDataOperation(0));
		}

		[Test]
		public void TestNoneCloneableDataOperation()
		{
			link2.SourceComponent.Subscribe(this, EventType.Warning);

			IDataOperation nonCloneableDataOperation = new NoneCloneableDataOperation("nonCloneableDataOperation");
			link2.AddDataOperation(nonCloneableDataOperation);
			link2.AddDataOperation(nonCloneableDataOperation);
			Assert.IsTrue(lastEvent != null, "lastEvent!=null");
			Assert.IsTrue(lastEvent.Type == EventType.Warning, "EventType.Warning");
			Assert.IsTrue(lastEvent.Description.ToLower().Contains("clone"), "clone");
		}

		[Test]
		public void Equals()
		{
			Assert.IsFalse(link1.Equals(link2));

			link2.ID = "Link1";

			Assert.IsTrue(link1.Equals(link2));
		}

		#region IListener Members

		public int GetAcceptedEventTypeCount()
		{
			return 1;
		}

		public EventType GetAcceptedEventType(int acceptedEventTypeIndex)
		{
			return EventType.Warning;
		}

		public void OnEvent(IEvent Event)
		{
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
