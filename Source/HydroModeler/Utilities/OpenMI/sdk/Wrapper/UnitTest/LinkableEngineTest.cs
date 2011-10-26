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
using Oatc.OpenMI.Sdk.Wrapper;
//using Oatc.OpenMI.Sdk.Spatial;
using Oatc.OpenMI.Sdk.Buffer;
//using org.OpenMI.Examples.ExeptionHandlers.SimpleExceptionHandler;
//using org.OpenMI.Examples.TriggerComponents.SimpleTrigger;
//using org.OpenMI.Examples.EventListeners.SimpleEventListener;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for LinkableComponentTest.
	/// </summary>
	[TestFixture]
	public class LinkableEngineTest
	{
		[Test]
		public void ComponentID()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual("Test River Model Component ID", riverModelLC.ComponentID);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void ComponentDescription()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual("Test River model component description", riverModelLC.ComponentDescription);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void ModelID()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual("TestRiverModel Model ID", riverModelLC.ModelID);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void ModelModelDescription()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual("Test River model - Model description", riverModelLC.ModelDescription);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void TimeHorison()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			double simulationStart = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(2005,1,1,0,0,0));
			double simulationEnd   = CalendarConverter.Gregorian2ModifiedJulian(new DateTime(2005,2,10,0,0,0));
			Assert.AreEqual(simulationStart,riverModelLC.TimeHorizon.Start.ModifiedJulianDay);
			Assert.AreEqual(simulationEnd, riverModelLC.TimeHorizon.End.ModifiedJulianDay);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void InputExchangeItemCount()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual(5, riverModelLC.InputExchangeItemCount);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void OutputExchangeItemCount()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual(7, riverModelLC.OutputExchangeItemCount);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void InputExchangeItem()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);

			IInputExchangeItem exchangeItem0 = riverModelLC.GetInputExchangeItem(0);
			Assert.AreEqual("Node:0",exchangeItem0.ElementSet.ID);
			Assert.AreEqual("Flow",exchangeItem0.Quantity.ID);
			Assert.AreEqual("LiterPrSecond",exchangeItem0.Quantity.Unit.ID);
			Assert.AreEqual(0.001,exchangeItem0.Quantity.Unit.ConversionFactorToSI);
			Assert.AreEqual(0.0,exchangeItem0.Quantity.Unit.OffSetToSI);
			Assert.AreEqual(3,exchangeItem0.Quantity.Dimension.GetPower(DimensionBase.Length));
			Assert.AreEqual(-1,exchangeItem0.Quantity.Dimension.GetPower(DimensionBase.Time));
			Assert.AreEqual(1,exchangeItem0.ElementSet.ElementCount);
			Assert.AreEqual(ElementType.IDBased,exchangeItem0.ElementSet.ElementType);

			IInputExchangeItem exchangeItem1 = riverModelLC.GetInputExchangeItem(1);
			Assert.AreEqual("Node:1",exchangeItem1.ElementSet.ID);
			Assert.AreEqual("Flow",exchangeItem1.Quantity.ID);
			Assert.AreEqual(1,exchangeItem1.ElementSet.ElementCount);
			Assert.AreEqual(ElementType.IDBased,exchangeItem1.ElementSet.ElementType);

			IInputExchangeItem exchangeItem2 = riverModelLC.GetInputExchangeItem(2);
			Assert.AreEqual("Node:2",exchangeItem2.ElementSet.ID);
			Assert.AreEqual("Flow",exchangeItem2.Quantity.ID);
			Assert.AreEqual(1,exchangeItem2.ElementSet.ElementCount);
			Assert.AreEqual(ElementType.IDBased,exchangeItem2.ElementSet.ElementType);

			IInputExchangeItem exchangeItem3 = riverModelLC.GetInputExchangeItem(3);
			Assert.AreEqual("Node:3",exchangeItem3.ElementSet.ID);
			Assert.AreEqual("Flow",exchangeItem3.Quantity.ID);
			Assert.AreEqual(1,exchangeItem3.ElementSet.ElementCount);
			Assert.AreEqual(ElementType.IDBased,exchangeItem3.ElementSet.ElementType);

			IInputExchangeItem exchangeItem4 = riverModelLC.GetInputExchangeItem(4);
			Assert.AreEqual("WholeRiver",exchangeItem4.ElementSet.ID);
			Assert.AreEqual("Flow",exchangeItem4.Quantity.ID);
			Assert.AreEqual(3,exchangeItem4.ElementSet.ElementCount);
			Assert.AreEqual(ElementType.XYPolyLine,exchangeItem4.ElementSet.ElementType);
			Assert.AreEqual(2,exchangeItem4.ElementSet.GetVertexCount(0));
			Assert.AreEqual(2,exchangeItem4.ElementSet.GetVertexCount(1));
			Assert.AreEqual(2,exchangeItem4.ElementSet.GetVertexCount(2));
			Assert.AreEqual(3.0,exchangeItem4.ElementSet.GetXCoordinate(0,0));
			Assert.AreEqual(5.0,exchangeItem4.ElementSet.GetXCoordinate(0,1));
			Assert.AreEqual(5.0,exchangeItem4.ElementSet.GetXCoordinate(1,0));
			Assert.AreEqual(8.0,exchangeItem4.ElementSet.GetXCoordinate(1,1));
			Assert.AreEqual(8.0,exchangeItem4.ElementSet.GetXCoordinate(2,0));
			Assert.AreEqual(8.0,exchangeItem4.ElementSet.GetXCoordinate(2,1));
			Assert.AreEqual(9.0,exchangeItem4.ElementSet.GetYCoordinate(0,0));
			Assert.AreEqual(7.0,exchangeItem4.ElementSet.GetYCoordinate(0,1));
			Assert.AreEqual(7.0,exchangeItem4.ElementSet.GetYCoordinate(1,0));
			Assert.AreEqual(7.0,exchangeItem4.ElementSet.GetYCoordinate(1,1));
			Assert.AreEqual(7.0,exchangeItem4.ElementSet.GetYCoordinate(2,0));
			Assert.AreEqual(3.0,exchangeItem4.ElementSet.GetYCoordinate(2,1));
		}

		[Test]
		public void GetOutputExchangeItem()
		{
			try
			{
				ILinkableComponent riverModelLC = new RiverModelLC();
				riverModelLC.Initialize(new Argument[0]);

				IOutputExchangeItem exchangeItem0 = riverModelLC.GetOutputExchangeItem(0);
				Assert.AreEqual("Branch:0",exchangeItem0.ElementSet.ID);
				Assert.AreEqual("Flow",exchangeItem0.Quantity.ID);
				Assert.AreEqual(1,exchangeItem0.ElementSet.ElementCount);
				Assert.AreEqual(ElementType.IDBased,exchangeItem0.ElementSet.ElementType);

				IOutputExchangeItem exchangeItem1 = riverModelLC.GetOutputExchangeItem(1);
				Assert.AreEqual("Branch:1",exchangeItem1.ElementSet.ID);
				Assert.AreEqual("Flow",exchangeItem1.Quantity.ID);
				Assert.AreEqual(1,exchangeItem1.ElementSet.ElementCount);
				Assert.AreEqual(ElementType.IDBased,exchangeItem0.ElementSet.ElementType);

				IOutputExchangeItem exchangeItem2 = riverModelLC.GetOutputExchangeItem(2);
				Assert.AreEqual("Branch:2",exchangeItem2.ElementSet.ID);
				Assert.AreEqual("Flow",exchangeItem2.Quantity.ID);
				Assert.AreEqual(1,exchangeItem2.ElementSet.ElementCount);
				Assert.AreEqual(ElementType.IDBased,exchangeItem0.ElementSet.ElementType);

				IOutputExchangeItem exchangeItem3 = riverModelLC.GetOutputExchangeItem(3);
				Assert.AreEqual("Branch:0",exchangeItem3.ElementSet.ID);
				Assert.AreEqual("Leakage",exchangeItem3.Quantity.ID);
				Assert.AreEqual(1,exchangeItem3.ElementSet.ElementCount);
				Assert.AreEqual(ElementType.IDBased,exchangeItem0.ElementSet.ElementType);

				IOutputExchangeItem exchangeItem4 = riverModelLC.GetOutputExchangeItem(4);
				Assert.AreEqual("Branch:1",exchangeItem4.ElementSet.ID);
				Assert.AreEqual("Leakage",exchangeItem4.Quantity.ID);
				Assert.AreEqual(1,exchangeItem4.ElementSet.ElementCount);
				Assert.AreEqual(ElementType.IDBased,exchangeItem0.ElementSet.ElementType);

				IOutputExchangeItem exchangeItem5 = riverModelLC.GetOutputExchangeItem(5);
				Assert.AreEqual("Branch:2",exchangeItem5.ElementSet.ID);
				Assert.AreEqual("Leakage",exchangeItem5.Quantity.ID);
				Assert.AreEqual(1,exchangeItem5.ElementSet.ElementCount);
				Assert.AreEqual(ElementType.IDBased,exchangeItem0.ElementSet.ElementType);

				IOutputExchangeItem exchangeItem6 = riverModelLC.GetOutputExchangeItem(6);
				Assert.AreEqual("WholeRiver",exchangeItem6.ElementSet.ID);
				Assert.AreEqual("Leakage",exchangeItem6.Quantity.ID);
				Assert.AreEqual(3,exchangeItem6.ElementSet.ElementCount);
				Assert.AreEqual(ElementType.XYPolyLine,exchangeItem6.ElementSet.ElementType);
				Assert.AreEqual(3,exchangeItem6.ElementSet.ElementCount);
				Assert.AreEqual(2,exchangeItem6.ElementSet.GetVertexCount(0));
				Assert.AreEqual(2,exchangeItem6.ElementSet.GetVertexCount(1));
				Assert.AreEqual(2,exchangeItem6.ElementSet.GetVertexCount(2));
				Assert.AreEqual(3.0,exchangeItem6.ElementSet.GetXCoordinate(0,0));
				Assert.AreEqual(5.0,exchangeItem6.ElementSet.GetXCoordinate(0,1));
				Assert.AreEqual(5.0,exchangeItem6.ElementSet.GetXCoordinate(1,0));
				Assert.AreEqual(8.0,exchangeItem6.ElementSet.GetXCoordinate(1,1));
				Assert.AreEqual(8.0,exchangeItem6.ElementSet.GetXCoordinate(2,0));
				Assert.AreEqual(8.0,exchangeItem6.ElementSet.GetXCoordinate(2,1));
				Assert.AreEqual(9.0,exchangeItem6.ElementSet.GetYCoordinate(0,0));
				Assert.AreEqual(7.0,exchangeItem6.ElementSet.GetYCoordinate(0,1));
				Assert.AreEqual(7.0,exchangeItem6.ElementSet.GetYCoordinate(1,0));
				Assert.AreEqual(7.0,exchangeItem6.ElementSet.GetYCoordinate(1,1));
				Assert.AreEqual(7.0,exchangeItem6.ElementSet.GetYCoordinate(2,0));
				Assert.AreEqual(3.0,exchangeItem6.ElementSet.GetYCoordinate(2,1));
			
				riverModelLC.Prepare();
				riverModelLC.Finish();
				riverModelLC.Dispose();
			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}


		[Test]
		public void Initialize()
		{
			//----- Testing -----------------------------------------------------------
			// 1: That the Initialize method is called on the engine
			// 2: That the initializeMethodWasInvoked flas is altered from false to true
			// 3: That the Argument are correctly changed to a Hashtable
			// -------------------------------------------------------------------------

			ILinkableComponent riverModelLC = new RiverModelLC();
			Argument[] arguments = new Argument[1];
			arguments[0] = new Argument("ModelID","TestRiverModelID",true,"ID for the model");
			Assert.AreEqual(false,((RiverModelLC)riverModelLC)._riverModelEngine._initializeMethodWasInvoked);
			riverModelLC.Initialize(arguments);
			Assert.AreEqual(true,((RiverModelLC)riverModelLC)._riverModelEngine._initializeMethodWasInvoked);
			Assert.AreEqual("TestRiverModelID",riverModelLC.ModelID);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}
	
		

	
		[Test]
		public void TimeEpsilon()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			Assert.AreEqual(false,((RiverModelLC)riverModelLC)._riverModelEngine._initializeMethodWasInvoked);
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual(0.10 * 1.0 / (3600.0 * 24.0), ((RiverModelLC)riverModelLC).TimeEpsilon);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void EngineApiAccess()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			Assert.AreEqual(false,((RiverModelLC)riverModelLC)._riverModelEngine._initializeMethodWasInvoked);
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual("Test River Model Component ID",((RiverModelLC) riverModelLC).EngineApiAccess.GetComponentID());
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void AddLink()
		{
			TestEngineLC  sourceModel = new TestEngineLC();
			TestEngineLC  targetModel = new TestEngineLC();

			sourceModel.Initialize(new Argument[0]);
			targetModel.Initialize(new Argument[0]);
	
			Link link = new Link();
			link.ID = "SourceToTargetLink";
			link.SourceComponent  = sourceModel;
			link.SourceElementSet = sourceModel.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			link.SourceQuantity   = sourceModel.GetOutputExchangeItem(0).Quantity;
			link.TargetComponent  = targetModel;
			link.TargetElementSet = targetModel.GetInputExchangeItem(0).ElementSet;  //first node in the river
			link.TargetQuantity   = targetModel.GetInputExchangeItem(0).Quantity;

			Assert.AreEqual(0,sourceModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,sourceModel.SmartInputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartInputLinks.Count);
			
			sourceModel.AddLink(link);
			targetModel.AddLink(link);

			Assert.AreEqual(1,sourceModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,sourceModel.SmartInputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartOutputLinks.Count);
			Assert.AreEqual(1,targetModel.SmartInputLinks.Count);

            sourceModel.Prepare(); // create the buffer

			Assert.AreEqual(1,((SmartOutputLink)sourceModel.SmartOutputLinks[0]).SmartBuffer.TimesCount);
		}

		[Test]
		public void RemoveLink()
		{
			TestEngineLC  sourceModel = new TestEngineLC();
			TestEngineLC  targetModel = new TestEngineLC();

			sourceModel.Initialize(new Argument[0]);
			targetModel.Initialize(new Argument[0]);
	
			Link link = new Link();
			link.ID = "SourceToTargetLink";
			link.SourceComponent  = sourceModel;
			link.SourceElementSet = sourceModel.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			link.SourceQuantity   = sourceModel.GetOutputExchangeItem(0).Quantity;
			link.TargetComponent  = targetModel;
			link.TargetElementSet = targetModel.GetInputExchangeItem(0).ElementSet;  //first node in the river
			link.TargetQuantity   = targetModel.GetInputExchangeItem(0).Quantity;

			Assert.AreEqual(0,sourceModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,sourceModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartOutputLinks.Count);
			
			sourceModel.AddLink(link);
			targetModel.AddLink(link);

			Assert.AreEqual(1,sourceModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,sourceModel.SmartInputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartOutputLinks.Count);
			Assert.AreEqual(1,targetModel.SmartInputLinks.Count);

			sourceModel.RemoveLink(link.ID);
			targetModel.RemoveLink(link.ID);

			Assert.AreEqual(0,sourceModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,sourceModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartOutputLinks.Count);
			Assert.AreEqual(0,targetModel.SmartOutputLinks.Count);
		
		}

		[Test]
		public void Validate()
		{
			ILinkableComponent upperRiver = new RiverModelLC();
			ILinkableComponent lowerRiver = new RiverModelLC();
			Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger     = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();

			// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
			Argument[] upperRiverArguments = new Argument[1];
			upperRiverArguments[0] = new Argument("ModelID","upperRiverModel",true,"argument");
			upperRiver.Initialize(upperRiverArguments);

			// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
			Argument[] lowerRiverArguments = new Argument[1];
			lowerRiverArguments[0] = new Argument("ModelID","lowerRiverModel",true,"argument");
			lowerRiver.Initialize(lowerRiverArguments);
			trigger.Initialize(new Argument[0]);

			Assert.AreEqual("upperRiverModel",upperRiver.ModelID);
			Assert.AreEqual("lowerRiverModel",lowerRiver.ModelID);

			Dimension wrongDimension = new Dimension();
			wrongDimension.SetPower(DimensionBase.Mass,1);
			Quantity wrongQuantity = new Quantity(new Unit("dummy",0.0,0.0,"dummy"),"test","qid",global::OpenMI.Standard.ValueType.Vector,wrongDimension);
			ElementSet wrongElementSet = new ElementSet("Wrong ElementSet","BadID",ElementType.XYPolyLine,new SpatialReference("no ref"));
			Element element = new Element("dum Element");
			element.AddVertex(new Vertex(4,5,0));
			wrongElementSet.AddElement(element);

			Link link = new Link();
			link.ID = "RiverToRiverLink";
			link.SourceComponent  = upperRiver;
			link.SourceElementSet = upperRiver.GetOutputExchangeItem(2).ElementSet; //last branch in the river
			link.SourceQuantity   = upperRiver.GetOutputExchangeItem(2).Quantity;
			link.TargetComponent  = lowerRiver;
			link.TargetElementSet = lowerRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
			link.TargetQuantity   = wrongQuantity;
//			link.AddDataOperation(upperRiver.GetOutputExchangeItem(6).GetDataOperation(0)); // bad data Operation

			Link triggerLink = new Link();

			triggerLink.ID				 = "TriggerLink";
			triggerLink.SourceComponent  = lowerRiver;
			triggerLink.SourceElementSet = wrongElementSet;
			triggerLink.SourceQuantity   = wrongQuantity;
			triggerLink.TargetComponent  = trigger;
			triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
			triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;
		
			upperRiver.AddLink(link);
			lowerRiver.AddLink(link);

			lowerRiver.AddLink(triggerLink);
			trigger.AddLink(triggerLink);

			bool isSilent = false;
			if (!isSilent)
			{
				Console.WriteLine(lowerRiver.Validate());

				foreach (string str in ((RiverModelLC) upperRiver).ValidationErrorMessages)
				{
					Console.WriteLine("Error upperRiver: " + str);
				}
				foreach (string str in ((RiverModelLC) lowerRiver).ValidationErrorMessages)
				{
					Console.WriteLine("Error lowerRiver: " + str);
				}
				foreach (string str in ((RiverModelLC) upperRiver).ValidationWarningMessages)
				{
					Console.WriteLine("Warning upperRiver: " + str);
				}
				foreach (string str in ((RiverModelLC) lowerRiver).ValidationWarningMessages)
				{
					Console.WriteLine("Warning lowerRiver: " + str);
				}
			}

			Assert.AreEqual(0,((RiverModelLC) upperRiver).ValidationErrorMessages.Count);
			Assert.AreEqual(4,((RiverModelLC) lowerRiver).ValidationErrorMessages.Count);

			Assert.AreEqual(0,((RiverModelLC) upperRiver).ValidationWarningMessages.Count);
			Assert.AreEqual(2,((RiverModelLC) lowerRiver).ValidationWarningMessages.Count);
			
			
		}
				
		[Test]
		public void Prepare()
		{
			try
			{
				ILinkableComponent riverModelLC = new RiverModelLC();
				ILinkableComponent trigger = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();
				
				riverModelLC.Initialize(new Argument[0]);
				trigger.Initialize(new Argument[0]);

				Link link = new Link(riverModelLC, riverModelLC.GetOutputExchangeItem(0).ElementSet,riverModelLC.GetOutputExchangeItem(0).Quantity,trigger,trigger.GetInputExchangeItem(0).ElementSet,trigger.GetInputExchangeItem(0).Quantity,"LinkID");
				riverModelLC.AddLink(link);
				trigger.AddLink(link);

				Assert.AreEqual(false, ((RiverModelLC) riverModelLC).PrepareForCompotationWasInvoked);
				riverModelLC.Prepare();
				Assert.AreEqual(true, ((RiverModelLC) riverModelLC).PrepareForCompotationWasInvoked);

				double x = ((IScalarSet)((SmartOutputLink)((RiverModelLC) riverModelLC).SmartOutputLinks[0]).SmartBuffer.GetValuesAt(0)).GetScalar(0);
				Assert.AreEqual(7,x); //test if the initial state variables (e.g. flow) is copied to the buffer.

				double t = ((ITimeSpan)((SmartOutputLink)((RiverModelLC) riverModelLC).SmartOutputLinks[0]).SmartBuffer.GetTimeAt(0)).Start.ModifiedJulianDay;
				Assert.AreEqual(riverModelLC.TimeHorizon.Start.ModifiedJulianDay, t); //test if the initial time is copied to the buffer

				riverModelLC.Finish();
				riverModelLC.Dispose();
			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}

		[Test]
		public void GetValues1A()
		{
			// Running with one instances of riverModelLC linked ID-based to trigger

			try
			{
				ILinkableComponent riverModelLC = new RiverModelLC();
				Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();
	
				Argument[] riverArguments = new Argument[2];
				riverArguments[0] = new Argument("ModelID","RiverModel",true,"argument");
				riverArguments[1] = new Argument("TimeStepLength","3600",true,"A time step length of 1 hour");

				riverModelLC.Initialize(riverArguments);
				trigger.Initialize(new Argument[0]);

				Link link = new Link(riverModelLC, riverModelLC.GetOutputExchangeItem(2).ElementSet,riverModelLC.GetOutputExchangeItem(2).Quantity,trigger,trigger.GetInputExchangeItem(0).ElementSet,trigger.GetInputExchangeItem(0).Quantity,"LinkID");
				riverModelLC.AddLink(link);
				trigger.AddLink(link);

				riverModelLC.Prepare();
				
				double firstTriggerGetValuesTime = riverModelLC.TimeHorizon.Start.ModifiedJulianDay;
				TimeStamp[] triggerTimes = new TimeStamp[2];
				triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 2);
				triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 4.3);
	
				trigger.Run(triggerTimes);
				double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
				Assert.AreEqual(35.0/4.0,x1);

				double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0);
				Assert.AreEqual(35.0/4.0,x2);

				riverModelLC.Finish();
				riverModelLC.Dispose();
				
			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}

		[Test]
		public void GetValues1B()
		{
			// Running with one instances of riverModelLC linked ID-based to trigger and to 
			// an instance of the TimeSeriesComponent.
			try
			{
				ILinkableComponent timeSeries = new TimeSeriesComponent();
				ILinkableComponent riverModelLC = new RiverModelLC();
				Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();
	
				timeSeries.Initialize(new Argument[0]);

				Argument[] riverArguments = new Argument[2];
				riverArguments[0] = new Argument("ModelID","upperRiverModel",true,"argument");
				riverArguments[1] = new Argument("TimeStepLength","3600",true,"A time step length of 1 day");

				riverModelLC.Initialize(riverArguments);
				trigger.Initialize(new Argument[0]);

				Link timeSeriesToRiverLink = new Link();
				timeSeriesToRiverLink.ID = "timeSeriesToUpperRiverLink";
				timeSeriesToRiverLink.SourceComponent  = timeSeries;
				timeSeriesToRiverLink.SourceElementSet = timeSeries.GetOutputExchangeItem(0).ElementSet; //last branch in the river
				timeSeriesToRiverLink.SourceQuantity   = timeSeries.GetOutputExchangeItem(0).Quantity;
				timeSeriesToRiverLink.TargetComponent  = riverModelLC;
				timeSeriesToRiverLink.TargetElementSet = riverModelLC.GetInputExchangeItem(0).ElementSet;  //first node in the river
				timeSeriesToRiverLink.TargetQuantity   = riverModelLC.GetInputExchangeItem(0).Quantity;
				

				Link link = new Link(riverModelLC, riverModelLC.GetOutputExchangeItem(2).ElementSet,riverModelLC.GetOutputExchangeItem(2).Quantity,trigger,trigger.GetInputExchangeItem(0).ElementSet,trigger.GetInputExchangeItem(0).Quantity,"LinkID");
				
                timeSeries.AddLink(timeSeriesToRiverLink);
				riverModelLC.AddLink(timeSeriesToRiverLink);

				riverModelLC.AddLink(link);
				trigger.AddLink(link);

				riverModelLC.Prepare();
				
				double firstTriggerGetValuesTime = riverModelLC.TimeHorizon.Start.ModifiedJulianDay;
				TimeStamp[] triggerTimes = new TimeStamp[2];
				triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 12.1);
				triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 16.7);
	
				trigger.Run(triggerTimes);
				double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
				Assert.AreEqual(35.0/4.0 + 13.0/8.0,x1);

				double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0);
				Assert.AreEqual(35.0/4.0 + 17.0/8.0,x2);

				riverModelLC.Finish();
				riverModelLC.Dispose();
				
			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}


		[Test]
		public void GetValues2A()
		{
			// == Running with two instances of riverModelLC ==
			// 
			// - The Two river are running with the same timestepping.
			//
			// - The link is ID Based link with flow from last branch of the source river to the top
			//   node of the target river.
			//
			// - The time argument in the GetValues from rive to river is of type ITimeSpan
			//
			//TODO: 1: The RiverModelEngine should change the inflow over time. As it is now the inflow is the same
			//         in all time steps. Another idea would be to have a output exchange item that hold the accumulated
			//         inflow, this could be useful when testing the manage state interface.
			//
			//       2: Make this test run with the two river using different timesteps and with the source river
			//          starting ealier that the target river.
			//
			//       3: In this test also events could be tested. Simply test if all the required events are
			//          thrown during the simulations.
			try
			{
				ILinkableComponent upperRiver = new RiverModelLC();
				ILinkableComponent lowerRiver = new RiverModelLC();
                Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger     = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] upperRiverArguments = new Argument[1];
				upperRiverArguments[0] = new Argument("ModelID","upperRiverModel",true,"argument");
				upperRiver.Initialize(upperRiverArguments);

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] lowerRiverArguments = new Argument[1];
				lowerRiverArguments[0] = new Argument("ModelID","lowerRiverModel",true,"argument");
				lowerRiver.Initialize(lowerRiverArguments);
				trigger.Initialize(new Argument[0]);

				Assert.AreEqual("upperRiverModel",upperRiver.ModelID);
				Assert.AreEqual("lowerRiverModel",lowerRiver.ModelID);

				Link link = new Link();
				link.ID = "RiverToRiverLink";
				link.SourceComponent  = upperRiver;
				link.SourceElementSet = upperRiver.GetOutputExchangeItem(2).ElementSet; //last branch in the river
				link.SourceQuantity   = upperRiver.GetOutputExchangeItem(2).Quantity;
				link.TargetComponent  = lowerRiver;
				link.TargetElementSet = lowerRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
				link.TargetQuantity   = lowerRiver.GetInputExchangeItem(0).Quantity;

				Link triggerLink = new Link();

				triggerLink.ID				 = "TriggerLink";
				triggerLink.SourceComponent  = lowerRiver;
				triggerLink.SourceElementSet = lowerRiver.GetOutputExchangeItem(2).ElementSet;
				triggerLink.SourceQuantity   = lowerRiver.GetOutputExchangeItem(2).Quantity;
				triggerLink.TargetComponent  = trigger;
				triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
				triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;
		
				upperRiver.AddLink(link);
				lowerRiver.AddLink(link);

				lowerRiver.AddLink(triggerLink);
				trigger.AddLink(triggerLink);

				upperRiver.Prepare();
				lowerRiver.Prepare();
				trigger.Prepare();

				double firstTriggerGetValuesTime = lowerRiver.TimeHorizon.Start.ModifiedJulianDay;
				TimeStamp[] triggerTimes = new TimeStamp[2];
				triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 3);
				triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 4.3);

				trigger.Run(triggerTimes);

				double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
				Assert.AreEqual(315.0/32.0,x1);

				double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0);
				Assert.AreEqual(315.0/32.0,x2);

				upperRiver.Finish();
				lowerRiver.Finish();

				upperRiver.Dispose();
				lowerRiver.Dispose();

			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}

		[Test]
		public void GetValues2B()
		{
			// This is a variation of GetValues2A. In this test the timeSeries is linked ID based to the 
			// top node of the upperRiver. The last upperRiver branch of the upperRiver is ID based connected
			// to the top node of the lowerRiver. The last branch in the lowerRiver is linked to the trigger.
			// The timeSeries provides data that changes over time. This is what makes this test different from
			// GetValues2A, where everytning is the same for every time step.
			
			try
			{
				ILinkableComponent timeSeries = new TimeSeriesComponent();
				ILinkableComponent upperRiver = new RiverModelLC();
				ILinkableComponent lowerRiver = new RiverModelLC();
				Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger     = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();

				timeSeries.Initialize(new Argument[0]);

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] upperRiverArguments = new Argument[1];
				upperRiverArguments[0] = new Argument("ModelID","upperRiverModel",true,"argument");
				upperRiver.Initialize(upperRiverArguments);

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] lowerRiverArguments = new Argument[1];
				lowerRiverArguments[0] = new Argument("ModelID","lowerRiverModel",true,"argument");
				lowerRiver.Initialize(lowerRiverArguments);
				trigger.Initialize(new Argument[0]);

				Assert.AreEqual("upperRiverModel",upperRiver.ModelID);
				Assert.AreEqual("lowerRiverModel",lowerRiver.ModelID);

				Link timeSeriesToUpperRiverLink = new Link();
				timeSeriesToUpperRiverLink.ID = "timeSeriesToUpperRiverLink";
				timeSeriesToUpperRiverLink.SourceComponent  = timeSeries;
				timeSeriesToUpperRiverLink.SourceElementSet = timeSeries.GetOutputExchangeItem(0).ElementSet; //last branch in the river
				timeSeriesToUpperRiverLink.SourceQuantity   = timeSeries.GetOutputExchangeItem(0).Quantity;
				timeSeriesToUpperRiverLink.TargetComponent  = upperRiver;
				timeSeriesToUpperRiverLink.TargetElementSet = upperRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
				timeSeriesToUpperRiverLink.TargetQuantity   = upperRiver.GetInputExchangeItem(0).Quantity;
				
				Link link = new Link();
				link.ID = "RiverToRiverLink";
				link.SourceComponent  = upperRiver;
				link.SourceElementSet = upperRiver.GetOutputExchangeItem(2).ElementSet; //last branch in the river
				link.SourceQuantity   = upperRiver.GetOutputExchangeItem(2).Quantity;
				link.TargetComponent  = lowerRiver;
				link.TargetElementSet = lowerRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
				link.TargetQuantity   = lowerRiver.GetInputExchangeItem(0).Quantity;

				Link triggerLink = new Link();

				triggerLink.ID				 = "TriggerLink";
				triggerLink.SourceComponent  = lowerRiver;
				triggerLink.SourceElementSet = lowerRiver.GetOutputExchangeItem(2).ElementSet;
				triggerLink.SourceQuantity   = lowerRiver.GetOutputExchangeItem(2).Quantity;
				triggerLink.TargetComponent  = trigger;
				triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
				triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;
		
				timeSeries.AddLink(timeSeriesToUpperRiverLink);
				upperRiver.AddLink(timeSeriesToUpperRiverLink);

				upperRiver.AddLink(link);
				lowerRiver.AddLink(link);

				lowerRiver.AddLink(triggerLink);
				trigger.AddLink(triggerLink);

				timeSeries.Prepare();
				upperRiver.Prepare();
				lowerRiver.Prepare();
				trigger.Prepare();

				double firstTriggerGetValuesTime = lowerRiver.TimeHorizon.Start.ModifiedJulianDay;
				TimeStamp[] triggerTimes = new TimeStamp[2];
				triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 12.5);
				triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 16.2);

				trigger.Run(triggerTimes);

				double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
				Assert.AreEqual(315.0/32.0 + 13.0/64.0,x1);

				double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0);
				Assert.AreEqual(315.0/32.0 + 17.0/64.0,x2);

				upperRiver.Finish();
				lowerRiver.Finish();

				upperRiver.Dispose();
				lowerRiver.Dispose();

			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}

	
		[Test]
//		[Ignore ("This test fails when dt = 1 hour but works for dt = 0.5 day")]
		public void GetValues2C()
		{
			// This test is a variation of GetValues 2B. What makes this test different is that the the two
			// connected river models are not using the same timesteps.
			// This test will test:
			// - is the buffer working correcly with respect to interpolations and buffering
			// - is the buffer working correctly with respect to clearing the buffer.
			
			try
			{
				ILinkableComponent timeSeries = new TimeSeriesComponent();
				ILinkableComponent upperRiver = new RiverModelLC();
				ILinkableComponent lowerRiver = new RiverModelLC();
				Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger     = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();

				timeSeries.Initialize(new Argument[0]);

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] upperRiverArguments = new Argument[2];
				upperRiverArguments[0] = new Argument("ModelID","upperRiverModel",true,"argument");
				upperRiverArguments[1] = new Argument("TimeStepLength","21600",true,"A time step length of 1 day");
				upperRiver.Initialize(upperRiverArguments);

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] lowerRiverArguments = new Argument[2];
				lowerRiverArguments[0] = new Argument("ModelID","lowerRiverModel",true,"argument");
				lowerRiverArguments[1] = new Argument("TimeStepLength","86400",true,"xx");
				lowerRiver.Initialize(lowerRiverArguments);
				trigger.Initialize(new Argument[0]);

				Assert.AreEqual("upperRiverModel",upperRiver.ModelID);
				Assert.AreEqual("lowerRiverModel",lowerRiver.ModelID);

				Link timeSeriesToUpperRiverLink = new Link();
				timeSeriesToUpperRiverLink.ID = "timeSeriesToUpperRiverLink";
				timeSeriesToUpperRiverLink.SourceComponent  = timeSeries;
				timeSeriesToUpperRiverLink.SourceElementSet = timeSeries.GetOutputExchangeItem(0).ElementSet; //last branch in the river
				timeSeriesToUpperRiverLink.SourceQuantity   = timeSeries.GetOutputExchangeItem(0).Quantity;
				timeSeriesToUpperRiverLink.TargetComponent  = upperRiver;
				timeSeriesToUpperRiverLink.TargetElementSet = upperRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
				timeSeriesToUpperRiverLink.TargetQuantity   = upperRiver.GetInputExchangeItem(0).Quantity;
				
				Link link = new Link();
				link.ID = "RiverToRiverLink";
				link.SourceComponent  = upperRiver;
				link.SourceElementSet = upperRiver.GetOutputExchangeItem(2).ElementSet; //last branch in the river
				link.SourceQuantity   = upperRiver.GetOutputExchangeItem(2).Quantity;
				link.TargetComponent  = lowerRiver;
				link.TargetElementSet = lowerRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
				link.TargetQuantity   = lowerRiver.GetInputExchangeItem(0).Quantity;

				Link triggerLink = new Link();

				triggerLink.ID				 = "TriggerLink";
				triggerLink.SourceComponent  = lowerRiver;
				triggerLink.SourceElementSet = lowerRiver.GetOutputExchangeItem(2).ElementSet;
				triggerLink.SourceQuantity   = lowerRiver.GetOutputExchangeItem(2).Quantity;
				triggerLink.TargetComponent  = trigger;
				triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
				triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;
		
				timeSeries.AddLink(timeSeriesToUpperRiverLink);
				upperRiver.AddLink(timeSeriesToUpperRiverLink);

				upperRiver.AddLink(link);
				lowerRiver.AddLink(link);

				lowerRiver.AddLink(triggerLink);
				trigger.AddLink(triggerLink);
				
				timeSeries.Prepare();
				upperRiver.Prepare();
				lowerRiver.Prepare();
				trigger.Prepare();

				double firstTriggerGetValuesTime = lowerRiver.TimeHorizon.Start.ModifiedJulianDay;
				TimeStamp[] triggerTimes = new TimeStamp[2];
				triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 12.5);
				triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 16.2);

				trigger.Run(triggerTimes);

				double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
				Assert.AreEqual(315.0/32.0 + 13.0/64.0,x1);

				double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0);
				Assert.AreEqual(315.0/32.0 + 17.0/64.0,x2);

				Assert.AreEqual(10,((RiverModelLC) upperRiver)._maxBufferSize); //Test that the buffer is cleared

				upperRiver.Finish();
				lowerRiver.Finish();

				upperRiver.Dispose();
				lowerRiver.Dispose();

			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}

		[Test]
		[Ignore ("Test code is not implemented")]
		public void GetValues3A()
		{
			//TODO: implement a test for bi-directional link
		}

		[Test]
		public void GetValues4A()
		{
			//This test is: RiverModel --> GWModel --> Trigger
			//Testing: Georeferenced links
			RiverModelLC riverModel = new RiverModelLC();
			GWModelLC    gWModel    = new GWModelLC();
			Trigger      trigger    = new Trigger();

			riverModel.Initialize(new Argument[0]);
			gWModel.Initialize(new Argument[0]);

			Link riverGWLink = new Link();
			riverGWLink.ID = "RiverGWLink";
			riverGWLink.SourceComponent = riverModel;
			riverGWLink.SourceElementSet = riverModel.GetOutputExchangeItem(6).ElementSet;
			riverGWLink.SourceQuantity   = riverModel.GetOutputExchangeItem(0).Quantity;
			riverGWLink.TargetComponent  = gWModel;
			riverGWLink.TargetElementSet = gWModel.GetInputExchangeItem(0).ElementSet;
			riverGWLink.TargetQuantity   = gWModel.GetInputExchangeItem(0).Quantity;

			int dataOperationIndex = -9;
			for (int i = 0; i < riverModel.GetOutputExchangeItem(6).DataOperationCount; i++)
			{
				if (riverModel.GetOutputExchangeItem(6).GetDataOperation(i).ID == "ElementMapper501")
				{
					dataOperationIndex = i;
				}
			}

			if (dataOperationIndex < 0)
			{
				throw new Exception("failed to find dataOperation");
			}
			
			riverGWLink.AddDataOperation(riverModel.GetOutputExchangeItem(6).GetDataOperation(dataOperationIndex));

			Link triggerLink = new Link();
			triggerLink.ID = "RiverGWLink";
			triggerLink.SourceComponent = gWModel;
			triggerLink.SourceElementSet = gWModel.GetOutputExchangeItem(0).ElementSet;
			triggerLink.SourceQuantity   = gWModel.GetOutputExchangeItem(0).Quantity;
			triggerLink.TargetComponent  = trigger;
			triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
			triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;

			riverModel.AddLink(riverGWLink);
			gWModel.AddLink(riverGWLink);
			gWModel.AddLink(triggerLink);
			trigger.AddLink(triggerLink);

			riverModel.Prepare();
			gWModel.Prepare();

			double firstTriggerGetValuesTime = riverModel.TimeHorizon.Start.ModifiedJulianDay;
			TimeStamp[] triggerTimes = new TimeStamp[2];
			triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 12.1);
			triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 16.7);
	
			trigger.Run(triggerTimes);

			double x0 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
			Assert.AreEqual(0.0,x0);

			double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(1);
			Assert.AreEqual(105.0/16.0,x1);

			double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(2);
			Assert.AreEqual(7.5,x2);

			double x3 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(3);
			Assert.AreEqual(5.0+35.0/16.0,x3);

			riverModel.Finish();
			gWModel.Finish();

			riverModel.Dispose();
			gWModel.Dispose();

		}

		[Test]
		public void LinearConvertionDataOperation()
		{
			// Running with one instances of riverModelLC linked ID-based to trigger
			// using the linearConversionDataOperation.

			try
			{
				ILinkableComponent riverModelLC = new RiverModelLC();
				Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();
	
				Argument[] riverArguments = new Argument[2];
				riverArguments[0] = new Argument("ModelID","RiverModel",true,"argument");
				riverArguments[1] = new Argument("TimeStepLength","3600",true,"A time step length of 1 hour");

				riverModelLC.Initialize(riverArguments);
				trigger.Initialize(new Argument[0]);

				Link link = new Link(riverModelLC, riverModelLC.GetOutputExchangeItem(2).ElementSet,riverModelLC.GetOutputExchangeItem(2).Quantity,trigger,trigger.GetInputExchangeItem(0).ElementSet,trigger.GetInputExchangeItem(0).Quantity,"LinkID");

				//add linear conversion data operation
				bool dataOperationWasFound = false;
				int dataOperationIndex = -9;
				for (int i = 0; i < riverModelLC.GetOutputExchangeItem(2).DataOperationCount; i++)
				{
					if (riverModelLC.GetOutputExchangeItem(2).GetDataOperation(i).ID == "Linear Conversion")
					{
						dataOperationWasFound = true;
						dataOperationIndex = i;
					}
				}
				Assert.AreEqual(true,dataOperationWasFound);
				IDataOperation linearConvertionDataOperation = riverModelLC.GetOutputExchangeItem(2).GetDataOperation(dataOperationIndex);
				bool key_A_WasFound = false;
				bool key_B_WasFound = false;
				bool key_Type_WasFound = false;

				for (int i = 0; i < linearConvertionDataOperation.ArgumentCount; i++)
				{
					if ( linearConvertionDataOperation.GetArgument(i).Key == "A")
					{
						linearConvertionDataOperation.GetArgument(i).Value = "2.5";
						key_A_WasFound = true;
						Assert.AreEqual(false,linearConvertionDataOperation.GetArgument(i).ReadOnly);
					}
					if ( linearConvertionDataOperation.GetArgument(i).Key == "B")
					{
						linearConvertionDataOperation.GetArgument(i).Value = "3.5";
						key_B_WasFound = true;
						Assert.AreEqual(false,linearConvertionDataOperation.GetArgument(i).ReadOnly);
					}

					if ( linearConvertionDataOperation.GetArgument(i).Key == "Type")
					{
						key_Type_WasFound = true;
						Assert.AreEqual(true,linearConvertionDataOperation.GetArgument(i).ReadOnly);
					}
				}

				Assert.AreEqual(true,key_A_WasFound);
				Assert.AreEqual(true,key_B_WasFound);
				Assert.AreEqual(true,key_Type_WasFound);
				Assert.AreEqual("Linear Conversion",linearConvertionDataOperation.ID);

				link.AddDataOperation(linearConvertionDataOperation);

				riverModelLC.AddLink(link);
				trigger.AddLink(link);

				riverModelLC.Prepare();
				
				double firstTriggerGetValuesTime = riverModelLC.TimeHorizon.Start.ModifiedJulianDay;
				TimeStamp[] triggerTimes = new TimeStamp[2];
				triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 2);
				triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 4.3);
	
				trigger.Run(triggerTimes);
				double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
				Assert.AreEqual(2.5*(35.0/4.0) + 3.5, x1);

				double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0);
				Assert.AreEqual(2.5 * (35.0/4.0) + 3.5, x2);

				riverModelLC.Finish();
				riverModelLC.Dispose();
				
			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}

		[Test]
		public void SmartBufferDataOperationTest()
		{
			// Running with one instances of riverModelLC linked ID-based to trigger
			// using the SmartBufferDataOperation.

			try
			{
				ILinkableComponent riverModelLC = new RiverModelLC();
				Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();
	
				Argument[] riverArguments = new Argument[2];
				riverArguments[0] = new Argument("ModelID","RiverModel",true,"argument");
				riverArguments[1] = new Argument("TimeStepLength","3600",true,"A time step length of 1 hour");

				riverModelLC.Initialize(riverArguments);
				trigger.Initialize(new Argument[0]);

				Link link = new Link(riverModelLC, riverModelLC.GetOutputExchangeItem(2).ElementSet,riverModelLC.GetOutputExchangeItem(2).Quantity,trigger,trigger.GetInputExchangeItem(0).ElementSet,trigger.GetInputExchangeItem(0).Quantity,"LinkID");

				//add linear conversion data operation
				bool dataOperationWasFound = false;
				int dataOperationIndex = -9;
				for (int i = 0; i < riverModelLC.GetOutputExchangeItem(2).DataOperationCount; i++)
				{
					if (riverModelLC.GetOutputExchangeItem(2).GetDataOperation(i).ID == new SmartBufferDataOperation().ID)
					{
						dataOperationWasFound = true;
						dataOperationIndex = i;
					}
				}
				Assert.AreEqual(true,dataOperationWasFound);
				IDataOperation smartBufferDataOperation = riverModelLC.GetOutputExchangeItem(2).GetDataOperation(dataOperationIndex);
				bool key_A_WasFound = false;
				bool key_B_WasFound = false;
				bool key_Type_WasFound = false;

				for (int i = 0; i < smartBufferDataOperation.ArgumentCount; i++)
				{
					if ( smartBufferDataOperation.GetArgument(i).Key == "Relaxation Factor")
					{
						smartBufferDataOperation.GetArgument(i).Value = "0.7";
						key_A_WasFound = true;
						Assert.AreEqual(false,smartBufferDataOperation.GetArgument(i).ReadOnly);
					}
					if ( smartBufferDataOperation.GetArgument(i).Key == "Do Extended Data Validation")
					{
						smartBufferDataOperation.GetArgument(i).Value = "False";
						key_B_WasFound = true;
						Assert.AreEqual(false,smartBufferDataOperation.GetArgument(i).ReadOnly);
					}

					if ( smartBufferDataOperation.GetArgument(i).Key == "Type")
					{
						key_Type_WasFound = true;
						Assert.AreEqual(true,smartBufferDataOperation.GetArgument(i).ReadOnly);
					}
				}

				Assert.AreEqual(true,key_A_WasFound);
				Assert.AreEqual(true,key_B_WasFound);
				Assert.AreEqual(true,key_Type_WasFound);
				Assert.AreEqual("Buffering and temporal extrapolation",smartBufferDataOperation.ID);

				link.AddDataOperation(smartBufferDataOperation);

				riverModelLC.AddLink(link);
				trigger.AddLink(link);

				riverModelLC.Prepare();

				Assert.AreEqual(0.7,((SmartOutputLink)((RiverModelLC) riverModelLC).SmartOutputLinks[0]).SmartBuffer.RelaxationFactor);
				Assert.AreEqual(false,((SmartOutputLink)((RiverModelLC) riverModelLC).SmartOutputLinks[0]).SmartBuffer.DoExtendedDataVerification);

				riverModelLC.Finish();
				riverModelLC.Dispose();
				
			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}


	

		[Test]
		public void EarliestInputTime()
		{
			TestEngineLC testEngineLC = new TestEngineLC();
			Trigger trigger = new Trigger();

			testEngineLC.Initialize(new Argument[0]);

			Link triggerLink = new Link();
			triggerLink.ID = "TargetToTriggerLink";
			triggerLink.SourceComponent  = testEngineLC;
			triggerLink.SourceElementSet = testEngineLC.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			triggerLink.SourceQuantity   = testEngineLC.GetOutputExchangeItem(0).Quantity;
			triggerLink.TargetComponent  = trigger;
			triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;  
			triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;

			testEngineLC.AddLink(triggerLink);
			testEngineLC.Prepare();
			Assert.AreEqual(testEngineLC.TimeHorizon.Start,testEngineLC.EarliestInputTime);

		}

		[Test]
		public void GetPublishedEventTypeCount()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual(6, riverModelLC.GetPublishedEventTypeCount());
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}

		[Test]
		public void GetPublishedEventType()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			Assert.AreEqual(EventType.DataChanged, riverModelLC.GetPublishedEventType(0));
			Assert.AreEqual(EventType.Informative, riverModelLC.GetPublishedEventType(1));
			Assert.AreEqual(EventType.SourceAfterGetValuesCall, riverModelLC.GetPublishedEventType(2));
			Assert.AreEqual(EventType.SourceBeforeGetValuesReturn, riverModelLC.GetPublishedEventType(3));
			Assert.AreEqual(EventType.TargetAfterGetValuesReturn, riverModelLC.GetPublishedEventType(4));
			Assert.AreEqual(EventType.TargetBeforeGetValuesCall, riverModelLC.GetPublishedEventType(5));
			riverModelLC.Prepare();
			riverModelLC.Finish();
			riverModelLC.Dispose();
		}
	
		[Test]
		public void KeepCurrentState()
		{
			RiverModelLC riverModelLC = new RiverModelLC();
			Trigger trigger = new Trigger();

			riverModelLC.Initialize(new Argument[0]);

			Link triggerLink = new Link();
			triggerLink.ID = "TargetToTriggerLink";
			triggerLink.SourceComponent  = riverModelLC;
			triggerLink.SourceElementSet = riverModelLC.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			triggerLink.SourceQuantity   = riverModelLC.GetOutputExchangeItem(0).Quantity;
			triggerLink.TargetComponent  = trigger;
			triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;  
			triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;

			riverModelLC.AddLink(triggerLink);
			riverModelLC.Prepare();
			string stateID = riverModelLC.KeepCurrentState();
			Assert.AreEqual("state:1",stateID);
		}

		[Test]
		public void RestoreState()
		{
			//  This test is based on GetValues2C
			try
			{
				ILinkableComponent timeSeries = new TimeSeriesComponent();
				ILinkableComponent upperRiver = new RiverModelLC();
				ILinkableComponent lowerRiver = new RiverModelLC();
				Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger trigger     = new Oatc.OpenMI.Sdk.Wrapper.UnitTest.Trigger();

				timeSeries.Initialize(new Argument[0]);

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] upperRiverArguments = new Argument[2];
				upperRiverArguments[0] = new Argument("ModelID","upperRiverModel",true,"argument");
				upperRiverArguments[1] = new Argument("TimeStepLength","21600",true,"xx");
				upperRiver.Initialize(upperRiverArguments);

				// The ModelID is passes in ordet to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
				Argument[] lowerRiverArguments = new Argument[2];
				lowerRiverArguments[0] = new Argument("ModelID","lowerRiverModel",true,"argument");
				lowerRiverArguments[1] = new Argument("TimeStepLength","86400",true,"xx");
				lowerRiver.Initialize(lowerRiverArguments);
				trigger.Initialize(new Argument[0]);

				Assert.AreEqual("upperRiverModel",upperRiver.ModelID);
				Assert.AreEqual("lowerRiverModel",lowerRiver.ModelID);

				Link timeSeriesToUpperRiverLink = new Link();
				timeSeriesToUpperRiverLink.ID = "timeSeriesToUpperRiverLink";
				timeSeriesToUpperRiverLink.SourceComponent  = timeSeries;
				timeSeriesToUpperRiverLink.SourceElementSet = timeSeries.GetOutputExchangeItem(0).ElementSet; //last branch in the river
				timeSeriesToUpperRiverLink.SourceQuantity   = timeSeries.GetOutputExchangeItem(0).Quantity;
				timeSeriesToUpperRiverLink.TargetComponent  = upperRiver;
				timeSeriesToUpperRiverLink.TargetElementSet = upperRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
				timeSeriesToUpperRiverLink.TargetQuantity   = upperRiver.GetInputExchangeItem(0).Quantity;
				
				Link link = new Link();
				link.ID = "RiverToRiverLink";
				link.SourceComponent  = upperRiver;
				link.SourceElementSet = upperRiver.GetOutputExchangeItem(2).ElementSet; //last branch in the river
				link.SourceQuantity   = upperRiver.GetOutputExchangeItem(2).Quantity;
				link.TargetComponent  = lowerRiver;
				link.TargetElementSet = lowerRiver.GetInputExchangeItem(0).ElementSet;  //first node in the river
				link.TargetQuantity   = lowerRiver.GetInputExchangeItem(0).Quantity;

				Link triggerLink = new Link();

				triggerLink.ID				 = "TriggerLink";
				triggerLink.SourceComponent  = lowerRiver;
				triggerLink.SourceElementSet = lowerRiver.GetOutputExchangeItem(2).ElementSet;
				triggerLink.SourceQuantity   = lowerRiver.GetOutputExchangeItem(2).Quantity;
				triggerLink.TargetComponent  = trigger;
				triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
				triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;
		
				timeSeries.AddLink(timeSeriesToUpperRiverLink);
				upperRiver.AddLink(timeSeriesToUpperRiverLink);

				upperRiver.AddLink(link);
				lowerRiver.AddLink(link);

				lowerRiver.AddLink(triggerLink);
				trigger.AddLink(triggerLink);
				
				timeSeries.Prepare();
				upperRiver.Prepare();
				lowerRiver.Prepare();
				trigger.Prepare();

//				double firstTriggerGetValuesTime = lowerRiver.TimeHorizon.Start.ModifiedJulianDay;
//				TimeStamp[] triggerTimes = new TimeStamp[2];
//				triggerTimes[0] = new TimeStamp(firstTriggerGetValuesTime + 12.5);
//				triggerTimes[1] = new TimeStamp(firstTriggerGetValuesTime + 16.2);

				double t = lowerRiver.TimeHorizon.Start.ModifiedJulianDay;
				Assert.AreEqual(315.0/32.0 + 13.0/64.0, ((ScalarSet)lowerRiver.GetValues(new TimeStamp(t+12.5),"TriggerLink")).GetScalar(0));
				string lowerRiverStateID = ((IManageState) lowerRiver).KeepCurrentState();
				string upperRiverStateID = ((IManageState) upperRiver).KeepCurrentState();
				Assert.AreEqual(315.0/32.0 + 17.0/64.0, ((ScalarSet)lowerRiver.GetValues(new TimeStamp(t+16.2),"TriggerLink")).GetScalar(0));
				((IManageState) lowerRiver).RestoreState(lowerRiverStateID);
				((IManageState) upperRiver).RestoreState(upperRiverStateID);
				lowerRiver.GetValues(new TimeStamp(t + 14.0),"TriggerLink");
				Assert.AreEqual(315.0/32.0 + 17.0/64.0, ((ScalarSet)lowerRiver.GetValues(new TimeStamp(t+16.2),"TriggerLink")).GetScalar(0));



			

//				double x1 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(0)).GetScalar(0);
//				Assert.AreEqual(315.0/32.0 + 13.0/64.0,x1);
//
//				double x2 = ((IScalarSet)trigger.ResultsBuffer.GetValuesAt(1)).GetScalar(0);
//				Assert.AreEqual(315.0/32.0 + 17.0/64.0,x2);
//
//				Assert.AreEqual(2,((RiverModelLC) upperRiver)._maxBufferSize); //Test that the buffer is cleared

				upperRiver.Finish();
				lowerRiver.Finish();

				upperRiver.Dispose();
				lowerRiver.Dispose();

			}
			catch (System.Exception e)
			{
				ExceptionHandler.WriteException(e);
				throw (e);
			}
		}

		[Test]
		public void ClearState()
		{
			RiverModelLC riverModelLC = new RiverModelLC();
			Trigger trigger = new Trigger();

			riverModelLC.Initialize(new Argument[0]);

			Link triggerLink = new Link();
			triggerLink.ID = "TargetToTriggerLink";
			triggerLink.SourceComponent  = riverModelLC;
			triggerLink.SourceElementSet = riverModelLC.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			triggerLink.SourceQuantity   = riverModelLC.GetOutputExchangeItem(0).Quantity;
			triggerLink.TargetComponent  = trigger;
			triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;  
			triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;

			riverModelLC.AddLink(triggerLink);
			riverModelLC.Prepare();
			string stateID = riverModelLC.KeepCurrentState();
			Assert.AreEqual("state:1",stateID);
			Assert.AreEqual(1,riverModelLC._riverModelEngine._states.Count);
			riverModelLC.ClearState("state:1");
			Assert.AreEqual(0,riverModelLC._riverModelEngine._states.Count);
		}

		[Test]
		public void TimeToTimeStamp()
		{
			Oatc.OpenMI.Sdk.Backbone.TimeSpan timeSpan = new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(10.0),new TimeStamp(20.0));
			Oatc.OpenMI.Sdk.Backbone.TimeStamp timeStamp = new TimeStamp(15.0);

			Assert.AreEqual(20.0, LinkableRunEngine.TimeToTimeStamp(timeSpan).ModifiedJulianDay);
			Assert.AreEqual(15.0, LinkableRunEngine.TimeToTimeStamp(timeStamp).ModifiedJulianDay);
		}

		[Test]
		public void ITimeToString()
		{
			System.DateTime t1 = new DateTime(2004,7,12,10,25,34);
			System.DateTime t2 = new DateTime(2004,8,15,13,15,14);
			TimeStamp timeStamp1 = new TimeStamp(Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.Gregorian2ModifiedJulian(t1));
			TimeStamp timeStamp2 = new TimeStamp(Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.Gregorian2ModifiedJulian(t2));
			Oatc.OpenMI.Sdk.Backbone.TimeSpan timeSpan = new Oatc.OpenMI.Sdk.Backbone.TimeSpan(timeStamp1,timeStamp2);

			Assert.AreEqual(t1.ToString(),LinkableRunEngine.ITimeToString(timeStamp1));
			string str = "[" + t1.ToString()+", "+t2.ToString()+"]";
			Assert.AreEqual(str, LinkableRunEngine.ITimeToString(timeSpan));

		}

		[Test]
		public void Dispose()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			riverModelLC.Prepare();
			riverModelLC.Finish();
			Assert.AreEqual(false,((RiverModelLC)riverModelLC)._riverModelEngine._disposeMethodWasInvoked);
			riverModelLC.Dispose();
			Assert.AreEqual(true,((RiverModelLC)riverModelLC)._riverModelEngine._disposeMethodWasInvoked);
		}

		[Test]
		public void Finish()
		{
			ILinkableComponent riverModelLC = new RiverModelLC();
			riverModelLC.Initialize(new Argument[0]);
			riverModelLC.Prepare();
			Assert.AreEqual(false,((RiverModelLC)riverModelLC)._riverModelEngine._finishMethodWasInvoked);
			riverModelLC.Finish();
			Assert.AreEqual(true,((RiverModelLC)riverModelLC)._riverModelEngine._finishMethodWasInvoked);
			riverModelLC.Dispose();
		}

		[Test]
		[Ignore ("Test code is not implemented")]
		public void ExpectedException01()
		{
			// TODO:This test should test methods are callen in correct order
		}

		[Test]
		public void XEvent()
		{
			// Event Test
			// Testing : 1) That all events are actually thrown during calculations
			
			TestEngineLC  sourceModel = new TestEngineLC();
			TestEngineLC  targetModel = new TestEngineLC();
			Trigger trigger           = new Trigger();

			sourceModel.Initialize(new Argument[0]);
			targetModel.Initialize(new Argument[0]);
			trigger.Initialize(new Argument[0]);

			Link link = new Link();
			link.ID = "SourceToTargetLink";
			link.SourceComponent  = sourceModel;
			link.SourceElementSet = sourceModel.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			link.SourceQuantity   = sourceModel.GetOutputExchangeItem(0).Quantity;
			link.TargetComponent  = targetModel;
			link.TargetElementSet = targetModel.GetInputExchangeItem(0).ElementSet;  //first node in the river
			link.TargetQuantity   = targetModel.GetInputExchangeItem(0).Quantity;

			Link triggerLink = new Link();
			triggerLink.ID = "TargetToTriggerLink";
			triggerLink.SourceComponent  = targetModel;
			triggerLink.SourceElementSet = targetModel.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			triggerLink.SourceQuantity   = targetModel.GetOutputExchangeItem(0).Quantity;
			triggerLink.TargetComponent  = trigger;
			triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;  
			triggerLink.TargetQuantity   = trigger.GetInputExchangeItem(0).Quantity;

			sourceModel.AddLink(link);
			targetModel.AddLink(link);

			targetModel.AddLink(triggerLink);
			trigger.AddLink(triggerLink);

			EventListener eventListener = new EventListener();
			eventListener._isSilent = true;

			for (int i = 0; i < eventListener.GetAcceptedEventTypeCount(); i++)
			{
				for (int n = 0; n < sourceModel.GetPublishedEventTypeCount(); n++)
				{
					if (eventListener.GetAcceptedEventType(i) == sourceModel.GetPublishedEventType(n))
					{
						sourceModel.Subscribe(eventListener, eventListener.GetAcceptedEventType(i));
					}
				}

				for (int n = 0; n < targetModel.GetPublishedEventTypeCount(); n++)
				{
					if (eventListener.GetAcceptedEventType(i) == targetModel.GetPublishedEventType(n))
					{
						targetModel.Subscribe(eventListener, eventListener.GetAcceptedEventType(i));
					}
				}
			}

			sourceModel.Prepare();
			targetModel.Prepare();
			trigger.Prepare();

			trigger.Run(new TimeStamp(sourceModel.TimeHorizon.Start.ModifiedJulianDay + 10));

			Assert.AreEqual(true , eventListener._dataChanged);   
			Assert.AreEqual(false, eventListener._globalProgress);
//			Assert.AreEqual(true , eventListener._informative);   //TODO This test was out commented, because it fails, further investigation needed
			Assert.AreEqual(false, eventListener._other);
			Assert.AreEqual(true, eventListener._sourceAfterGetValuesCall);
			Assert.AreEqual(true, eventListener._sourceBeforeGetValuesReturn);
			Assert.AreEqual(true, eventListener._targetAfterGetValuesReturn);
			Assert.AreEqual(true, eventListener._targetBeforeGetValuesCall);
			Assert.AreEqual(false, eventListener._timeStepProgres);
			Assert.AreEqual(false, eventListener._valueOutOfRange);
			Assert.AreEqual(false, eventListener._warning);
		}
	

		[Test]
		public void XUnitConvertion()
		{
			// Unit conversion. Converting Fahrenheit to Celcius
			
			double x;
			double y;
			
			TestEngineLC  sourceModel = new TestEngineLC();
			TestEngineLC  targetModel = new TestEngineLC();

			sourceModel.Initialize(new Argument[0]);
			targetModel.Initialize(new Argument[0]);

			Link link = new Link();
			link.ID = "SourcToTargetLink";
			link.SourceComponent  = sourceModel;
			link.SourceElementSet = sourceModel.GetOutputExchangeItem(0).ElementSet; //last branch in the river
			link.SourceQuantity   = new Quantity(new Unit("Deg. Fahrenheit",5.0/9.0, 273.16 - 32.0 * (5.0/9.0) ,"Fahrenheit"),"temperature","Temperature",global::OpenMI.Standard.ValueType.Scalar,new Dimension());
			link.TargetComponent  = targetModel;
			link.TargetElementSet = targetModel.GetInputExchangeItem(0).ElementSet;  //first node in the river
			link.TargetQuantity   = new Quantity(new Unit("Deg. Celcius",1.0, 273.16,"Celcius"),"temperature","Temperature",global::OpenMI.Standard.ValueType.Scalar,new Dimension());

			sourceModel.AddLink(link);
			targetModel.AddLink(link);

			sourceModel.Prepare();
			targetModel.Prepare();

			for (int i = 0; i < ((ScalarSet)sourceModel.GetValues(new TimeStamp(sourceModel.TimeHorizon.Start.ModifiedJulianDay + 10),"SourcToTargetLink")).Count; i++)
			{
				x = ((ScalarSet)sourceModel.GetValues(new TimeStamp(sourceModel.TimeHorizon.Start.ModifiedJulianDay + 10),"SourcToTargetLink")).GetScalar(i);
				y = (100.0 - 32.0) * (5.0/9.0); // = 37.7778  (100 deg. F = 37.7778 deg.C)
				Assert.AreEqual(y,x,0.0000000001);  //internal value is 80 deg Fahrenheit
			}
		}


	}
}
