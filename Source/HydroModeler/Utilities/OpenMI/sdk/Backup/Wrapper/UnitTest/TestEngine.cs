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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for TestEngine.
	/// </summary>
	public class TestEngine  	 :  Oatc.OpenMI.Sdk.Wrapper.IEngine
	{
		InputExchangeItem    _inputExchangeItem;
		OutputExchangeItem   _outputExchangeItem;
		double[]             _values;
		double               _startTime;
		double               _currentTime;
		double               _initialValue;
		double               _dt;                     //time step length [days]
		double               _dx;                     //values are incremented by _dx in each time step
		string               _modelID;            //used for debugging in ordet see the difference between two instances of TestEngineLC

		public TestEngine()
		{
			_modelID = "TestEngineComponentID";
			
		}
		#region IEngine Members

		public InputExchangeItem GetInputExchangeItem(int exchangeItemIndex)
		{
			return _inputExchangeItem;
		}

		public ITimeSpan GetTimeHorizon()
		{
			return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(_startTime),new TimeStamp(_startTime +  100.0));
		}

		public string GetModelID()
		{
			return _modelID;
		}

		public int GetInputExchangeItemCount()
		{
			return 1;
		}

		public OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex)
		{
			return _outputExchangeItem;
		}

		public string GetModelDescription()
		{
			return "TestModelDescription";
		}

		public int GetOutputExchangeItemCount()
		{
			return 1;
		}

		#endregion

		#region IRunEngine Members

		public void SetValues(string QuantityID, string ElementSetID, IValueSet values)
		{
			for (int i = 0; i < _values.Length; i++)
			{
				_values[i] = ((ScalarSet)values).GetScalar(0);
			}
		}

		public string GetComponentID()
		{
			return "testEngineCompoentID";
		}

		public void Finish()
		{
			
		}

		public ITime GetCurrentTime()
		{
			return new TimeStamp(_currentTime);
		}

		public IValueSet GetValues(string QuantityID, string ElementSetID)
		{
			return new ScalarSet(_values);
		}

		public void Dispose()
		{

		}

		public string GetComponentDescription()
		{
			return "TestEngineComponentDescription";
		}

		public ITimeStamp GetEarliestNeededTime()
		{
			return new TimeStamp(_currentTime);
		}

		public void Initialize(System.Collections.Hashtable properties)
		{

			_dt           = 1.0;
			_dx           = 0.0;
			_initialValue = 100;

			if (properties.ContainsKey("modelID"))
			{
				_modelID = (string) properties["ModelID"];
			}

			if (properties.ContainsKey("dt"))
			{
				_dt = (double) properties["dt"];
			}

			if (properties.ContainsKey("dx"))
			{
				_dx = (double) properties["dx"];
			}
			
			_values = new double[3];

			for (int i = 0; i < _values.Length; i++)
			{
				_values[i] = _initialValue;
			}

			_startTime   = 4000;
			_currentTime = _startTime;

			Element element = new Element("ElementID");
			ElementSet elementSet = new ElementSet("Description","ID",ElementType.IDBased,new SpatialReference(" no "));
			elementSet.AddElement(element);
			Quantity quantity = new Quantity(new Unit("Flow",1,0,"flow"),"Flow","ID",global::OpenMI.Standard.ValueType.Scalar,new Dimension());

			_outputExchangeItem = new OutputExchangeItem();
			_inputExchangeItem  = new InputExchangeItem();

			_outputExchangeItem.Quantity   = quantity;
			_outputExchangeItem.ElementSet = elementSet;

			_inputExchangeItem.Quantity    = quantity;
			_inputExchangeItem.ElementSet  = elementSet;
		}

		public bool PerformTimeStep()
		{
			for (int i = 0; i < _values.Length; i++)
			{
				_values[i] += _dx;
			}

			_currentTime += _dt;


			return true;
		}

		public double GetMissingValueDefinition()
		{
			return -999.0;
		}

		public ITime GetInputTime(string QuantityID, string ElementSetID)
		{
			return new TimeStamp(_currentTime);
		}

		#endregion
	}
}
