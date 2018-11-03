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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Wrapper;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for RiverModelEngine.
	/// </summary>
	public class RiverModelEngine : IEngine, IManageState
	{
		double[]  _xCoordinate; // x-coordinates for the nodes
		double[]  _yCoordinate; // y-coordinates for the nodes
		int       _numberOfNodes;
		DateTime  _simulationStart; 
		DateTime  _simulationEnd;   
		double    _timeStepLength;      //[seconds]
		ArrayList _inputExchangeItems;
		ArrayList _outputExchangeItems;
		double[]   _storage; //[liters]
		double[]   _flow;    //[liter pr second]
		double[]   _leakage; //[liter pr second]
		double     _runoff;  //[liter pr second]
		int        _currentTimeStepNumber;
		string     _modelID;

		public  bool   _initializeMethodWasInvoked;
		public  bool   _finishMethodWasInvoked;
		public  bool   _disposeMethodWasInvoked;

		public ArrayList _states;
		int    _stateIdCreator;
	
	

		public RiverModelEngine()
		{
			_modelID = "TestRiverModel Model ID";
			_xCoordinate = new double[]{3,5,8,8};
			_yCoordinate = new double[]{9,7,7,3};

			_numberOfNodes = _xCoordinate.Length;

			_simulationStart = new DateTime(2005,1,1,0,0,0);
			_simulationEnd   = new DateTime(2005,2,10,0,0,0);
			_timeStepLength      = 3600*24;  //one day

			_inputExchangeItems = new ArrayList();
			_outputExchangeItems = new ArrayList();

			_storage = new double[_numberOfNodes];
			for ( int i = 0; i < _numberOfNodes; i++)
			{
				_storage[i] = 0;
			}
			_flow    = new double[_numberOfNodes - 1];
			_leakage = new double[_numberOfNodes - 1];
			_runoff  = 10;
			_currentTimeStepNumber = 0;

			_initializeMethodWasInvoked = false;
		    _finishMethodWasInvoked     = false;
		    _disposeMethodWasInvoked    = false;

			_states = new ArrayList();
			_stateIdCreator = 0;
		}
		#region IEngine Members

		public InputExchangeItem GetInputExchangeItem(int exchangeItemIndex)
		{
			return (InputExchangeItem) _inputExchangeItems[exchangeItemIndex];
		}

		public ITimeSpan GetTimeHorizon()
		{
			TimeStamp startTime = new TimeStamp(Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.Gregorian2ModifiedJulian(_simulationStart));
			TimeStamp endTime   = new TimeStamp(Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.Gregorian2ModifiedJulian(_simulationEnd));
			Oatc.OpenMI.Sdk.Backbone.TimeSpan timeHorizon = new Oatc.OpenMI.Sdk.Backbone.TimeSpan(startTime,endTime);
			return timeHorizon;
		}

		public string GetModelID()
		{
			return _modelID;
		}

		public int GetInputExchangeItemCount()
		{
			return _inputExchangeItems.Count;
		}

		public OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex)
		{
			return (OutputExchangeItem) _outputExchangeItems[exchangeItemIndex];
		}

		public string GetModelDescription()
		{
			return "Test River model - Model description";
		}

		public int GetOutputExchangeItemCount()
		{
			return _outputExchangeItems.Count;
		}

		#endregion

		#region IRunEngine Members

		public void SetValues(string QuantityID, string ElementSetID, IValueSet values)
		{
			char[] separator = new char[]{':'};

			if (!(values is IScalarSet))
			{
				throw new Exception("Illigal data type for values argument in method SetValues");
			}

			if (ElementSetID == "WholeRiver")
			{
				if (values.Count != _numberOfNodes - 1)
				{
					throw new Exception("Illigal number of values in ValueSet in argument to SetValues method");
				}
				for ( int i = 1; i < _numberOfNodes; i++)
				{
					_storage[i] += ((IScalarSet) values).GetScalar(i) * _timeStepLength;
				}
			}
			else if(ElementSetID.Split(separator)[0] == "Node")
			{
				if (values.Count != 1)
				{
					throw new Exception("illigal number of values in ValueSet in argument to SetValues method");
				}
				int nodeIndex = Convert.ToInt32(ElementSetID.Split(separator)[1]);
				_storage[nodeIndex] += ((IScalarSet) values).GetScalar(0) * _timeStepLength;
			}
			else 
			{
				throw new Exception("Failed to recognize ElementSetID in method SetValues");
			}
		}

		public string GetComponentID()
		{
			return "Test River Model Component ID";
		}

		public void Finish()
		{
			_finishMethodWasInvoked = true;
		}

		public ITime GetCurrentTime()
		{
			double t = Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.Gregorian2ModifiedJulian(_simulationStart);
			t += _currentTimeStepNumber * _timeStepLength / (24.0*3600.0);
			Oatc.OpenMI.Sdk.Backbone.TimeSpan currentTime = new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(t - _timeStepLength / (24.0*3600.0)),new TimeStamp(t));
			return currentTime;
		}

		public IValueSet GetValues(string QuantityID, string ElementSetID)
		{
			char[] separator = new char[]{':'};
			double[] x;
			
			if(ElementSetID.Split(separator)[0] == "Branch")
			{
				 x = new double[1];
				int branchIndex = Convert.ToInt32(ElementSetID.Split(separator)[1]);
				if (QuantityID == "Flow")
				{
					x[0] = _flow[branchIndex];
				}
				else if (QuantityID == "Leakage")
				{
					x[0] = _leakage[branchIndex];
				}
				else 
				{
					throw new Exception("Quanity ID not recognized in GetValues method");
				}

			}
			else if(ElementSetID.Split(separator)[0] == "WholeRiver")
			{
				x = new double[_leakage.Length];
				for (int i = 0; i < _leakage.Length; i++)
				{
					x[i] = _leakage[i];
				}
			}
			else 
			{
				throw new Exception("Failed to recognize ElementSetID in method GetValues");
			}

			ScalarSet scalarSet = new ScalarSet(x);
			return scalarSet;
		}

		public void Dispose()
		{
			_disposeMethodWasInvoked = true;
		}

		public string GetComponentDescription()
		{
			return "Test River model component description";
		}

		public ITimeStamp GetEarliestNeededTime()
		{
			return (ITimeStamp) ((ITimeSpan)GetCurrentTime()).Start;
		}

		public void Initialize(System.Collections.Hashtable properties)
		{

			if (properties.ContainsKey("ModelID"))
			{
				_modelID = (string) properties["ModelID"];
			}

			if (properties.ContainsKey("TimeStepLength"))
			{
				_timeStepLength = Convert.ToDouble((string) properties["TimeStepLength"]);
			}

			// -- create a flow quanitity --
			Dimension flowDimension = new Dimension();
			flowDimension.SetPower(DimensionBase.Length,3);
			flowDimension.SetPower(DimensionBase.Time,-1);
			Unit literPrSecUnit = new Unit("LiterPrSecond",0.001,0,"Liters pr Second");
			Quantity flowQuantity = new Quantity(literPrSecUnit,"Flow","Flow",global::OpenMI.Standard.ValueType.Scalar,flowDimension);

			// -- create leakage quantity --
			Quantity leakageQuantity = new Quantity(literPrSecUnit,"Leakage","Leakage",global::OpenMI.Standard.ValueType.Scalar,flowDimension);

			// -- create and populate elementset to represente the whole river network --
			ElementSet fullRiverElementSet = new ElementSet("WholeRiver","WholeRiver",ElementType.XYPolyLine,new SpatialReference("no reference"));
			for (int i = 0; i < _numberOfNodes -1; i++)
			{
				Element element = new Element();
				element.ID = "Branch:" + i.ToString();
				element.AddVertex(new Vertex(_xCoordinate[i],_yCoordinate[i],-999));
				element.AddVertex(new Vertex(_xCoordinate[i+1],_yCoordinate[i+1],-999));
				fullRiverElementSet.AddElement(element);
			}

			// --- populate input exchange items for flow to individual nodes ---
			for ( int i = 0; i < _numberOfNodes; i++)
			{
				Element element = new Element();
				element.ID = "Node:" + i.ToString();
				ElementSet elementSet = new ElementSet("Individual nodes","Node:" + i.ToString(), ElementType.IDBased,new SpatialReference("no reference"));
				elementSet.AddElement(element);
				InputExchangeItem inputExchangeItem = new InputExchangeItem();
				inputExchangeItem.ElementSet = elementSet;
				inputExchangeItem.Quantity = flowQuantity;
				
				_inputExchangeItems.Add(inputExchangeItem);
			}

			// --- Populate input exchange item for flow to the whole georeferenced river ---
			InputExchangeItem wholeRiverInputExchangeItem = new InputExchangeItem();
			wholeRiverInputExchangeItem.ElementSet = fullRiverElementSet;
			wholeRiverInputExchangeItem.Quantity   = flowQuantity;
			_inputExchangeItems.Add(wholeRiverInputExchangeItem);

			// --- Populate output exchange items for flow in river branches ---
			for (int i = 0; i < _numberOfNodes - 1; i++)
			{
				Element element = new Element();
				element.ID = "Branch:" + i.ToString();
				ElementSet elementSet = new ElementSet("Individual nodes","Branch:" + i.ToString(),ElementType.IDBased,new SpatialReference("no reference"));
				elementSet.AddElement(element);
			    OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
				outputExchangeItem.ElementSet = elementSet;
				outputExchangeItem.Quantity = flowQuantity;
				
				_outputExchangeItems.Add(outputExchangeItem);
			}

			// --- polulate output exchange items for leakage for individual branches --
			for (int i = 0; i < _numberOfNodes - 1; i++)
			{
				Element element = new Element();
				element.ID = "Branch:" + i.ToString();
				ElementSet elementSet = new ElementSet("Individual nodes","Branch:" + i.ToString(),ElementType.IDBased,new SpatialReference("no reference"));
				elementSet.AddElement(element);
				OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
				outputExchangeItem.ElementSet = elementSet;
				outputExchangeItem.Quantity = leakageQuantity;
				_outputExchangeItems.Add(outputExchangeItem);
			}

			// --- Populate output exchange item for leakage from the whole georeferenced river ---
			OutputExchangeItem wholeRiverOutputExchangeItem = new OutputExchangeItem();
			wholeRiverOutputExchangeItem.ElementSet = fullRiverElementSet;
			wholeRiverOutputExchangeItem.Quantity   = leakageQuantity;
			_outputExchangeItems.Add(wholeRiverOutputExchangeItem);

			// --- populate with initial state variables ---
			for (int i = 0; i < _numberOfNodes -1; i++)
			{
				_flow[i] = 7;
			}

			_currentTimeStepNumber = 1;
			_initializeMethodWasInvoked = true;
		}

		public bool PerformTimeStep()
		{

			for (int i = 0; i < _numberOfNodes; i++)
			{
				_storage[i] += _runoff * _timeStepLength;
			}

			for (int i = 0; i < _numberOfNodes -1; i++)
			{
				_flow[i] = 0.5 * _storage[i] / _timeStepLength;
				_leakage[i] = _flow[i];
				_storage[i+1] += 0.5* _storage[i];
			}

			for (int i = 0; i < _numberOfNodes; i++)
			{
				_storage[i] = 0;
			}

			_currentTimeStepNumber++;

			// -- debug output writing ----
//			string outstring;
//			outstring = "TsNo:" + _currentTimeStepNumber.ToString() + " ";
//			outstring += Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.ModifiedJulian2Gregorian(((ITimeSpan)GetCurrentTime()).Start.ModifiedJulianDay).ToString() + " - ";
//			outstring += Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.ModifiedJulian2Gregorian(((ITimeSpan)GetCurrentTime()).End.ModifiedJulianDay).ToString() + " - ";
//			for (int n = 0; n < _numberOfNodes -1; n++)
//			{
//				outstring += " F" + n.ToString() + ": " + _flow[n].ToString();
//
//			}
//
//			System.Console.WriteLine(outstring);


			// ----------------------------

			return true;
		}

		public double GetMissingValueDefinition()
		{
			return -999;
		}

		public ITime GetInputTime(string QuantityID, string ElementSetID)
		{
			Oatc.OpenMI.Sdk.Backbone.TimeStamp inputStart = new TimeStamp(((ITimeSpan)GetCurrentTime()).End.ModifiedJulianDay);
			Oatc.OpenMI.Sdk.Backbone.TimeStamp inputEnd = new TimeStamp(((ITimeSpan) GetCurrentTime()).End.ModifiedJulianDay + _timeStepLength/(24.0 * 3600.0));
			return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(inputStart, inputEnd);
			
		}

		#endregion

		#region IManageState Members

		public string KeepCurrentState()
		{
			_stateIdCreator++;

			string stateID = "state:" + _stateIdCreator.ToString();
			_states.Add(new RiverModelState(stateID, this._currentTimeStepNumber));

			return stateID;
		}

		public void RestoreState(string stateID)
		{
			int index = -999;

			for (int i = 0; i < _states.Count; i++)
			{
				if (((RiverModelState)_states[i]).StateId == stateID)
				{
					index = i;
				}
			}

			if (index < 0)
			{
				throw new Exception("Failed to find stateID in RestoreState method");
			}
			else
			{
				_currentTimeStepNumber = ((RiverModelState)_states[index]).TimeStepNumber;
			}
		}

		public void ClearState(string stateID)
		{
			int index = -999;

			for (int i = 0; i < _states.Count; i++)
			{
				if (((RiverModelState)_states[i]).StateId == stateID)
				{
					index = i;
				}
			}

			if (index < 0)
			{
				throw new Exception("Failed to find stateID in RemoveState method");
			}
			else
			{
				_states.RemoveAt(index);
			}
		}

		#endregion
	}
}
