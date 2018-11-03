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
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The LinearDataOperation class is an implementation of the IDataOperation interface. 
	/// The LinearDataOperation can make linear conversion on ScalarSets. The ax+b type of operations.
	/// </summary>
	public class LinearConversionDataOperation : IDataOperation, ICloneable
	{
		Oatc.OpenMI.Sdk.Backbone.Argument[] _arguments;
		bool   _isActivated; 
		double _a;
		double _b;
		
		/// <summary>
		/// Constructor
		/// </summary>
        public LinearConversionDataOperation()
		{
			_arguments = new Oatc.OpenMI.Sdk.Backbone.Argument[3];

			_arguments[0] = new Oatc.OpenMI.Sdk.Backbone.Argument();
			_arguments[0].Description = "Parameter A. Used in conversion: A*x + B";
			_arguments[0].Key = "Type";
			_arguments[0].Value = "Linear Conversion";
			_arguments[0].ReadOnly = true;

			_arguments[1] = new Oatc.OpenMI.Sdk.Backbone.Argument();
			_arguments[1].Description = "Parameter A. Used in conversion: A*x + B";
			_arguments[1].Key = "A";
			_arguments[1].Value = "1.0";
			_arguments[1].ReadOnly = false;

			_arguments[2] = new Oatc.OpenMI.Sdk.Backbone.Argument();
			_arguments[2].Description = "Parameter B. Used in conversion: A*x + B";
			_arguments[2].Key = "B";
			_arguments[2].Value = "0.0";
			_arguments[2].ReadOnly = false;

			_isActivated = false;
		}

		#region IDataOperation Members

		/// <summary>
		/// DataOperation ID. In this class always "Linear Conversions" (is hardcoded)
		/// </summary>
        public string ID
		{
			get
			{
				return "Linear Conversion";
			}
		}

		/// <summary>
		/// The linear dataoperation is valid for any input and output exchange items and can be combined with any other
		/// dataopertion, consequently this method always return true.
		/// See also documentation for : OpenMI.Standard.IDataOperation for details
		/// </summary>
		/// <param name="inputExchangeItem">inputExchangeItem</param>
		/// <param name="outputExchangeItem">outputExchangeItem</param>
		/// <param name="SelectedDataOperations">SelectedDataOperations</param>
		/// <returns></returns>
        public bool IsValid(IInputExchangeItem inputExchangeItem, IOutputExchangeItem outputExchangeItem, IDataOperation[] SelectedDataOperations)
		{
			return true;
		}

		/// <summary>
		/// Number of dataoperation arguments. For the Linear dataoperation this number is always 3 (coefficient a, offset b and description text)
		/// </summary>
        public int ArgumentCount
		{
			get
			{
				return _arguments.Length;
			}
		}

		/// <summary>
		/// Initialises the data operation. Nothing is done for the Linear dataoperation
		/// </summary>
		/// <param name="properties">arguments</param>
        public void Initialize(IArgument[] properties)
		{
			
		}

		/// <summary>
		/// Returns the arguments for the Linear Dataoperation
		/// </summary>
		/// <param name="argumentIndex">Argument index</param>
		/// <returns></returns>
        public IArgument GetArgument(int argumentIndex)
		{
			
			return (IArgument) _arguments[argumentIndex];
		}

		#endregion

		/// <summary>
		/// The prepare method should be called before the PerformDataOperation. This method is
		/// not part of the OpenMI.Standard.IDataOperation interface. This method will convert
		/// the arguments which originally are defined as strings to doubles and subsequently assign 
		/// these values to private field variables. The prepare method is introduced for performance
		/// reasons.
		/// </summary>
		public void Prepare()
		{
			
			bool argumentAWasFound = false;
			bool argumentBWasFound = false;

			_isActivated = true;

			for (int i = 0; i < this._arguments.Length; i++)
			{
				if (_arguments[i].Key == "A")
				{
					_a = Convert.ToDouble(_arguments[i].Value);
					argumentAWasFound = true;
				}

				if (_arguments[i].Key == "B")
				{
					_b = Convert.ToDouble(_arguments[i].Value);
					argumentBWasFound = true;
				}
			}
			if (!argumentAWasFound || !argumentBWasFound)
			{
				throw new Exception("Missing argument in data operation: \"Linear Conversion\"");
			}
		}

		/// <summary>
		/// The ValueSet is converted. This method does not support VectorSet, so if the ValueSet is a Vectorset
		/// an exception will be thrown. The parameters passed in this method is not used, since all needed information
		/// is already assigned in the Prepare method.
		/// </summary>
		/// <param name="values">argumens but not used in this method</param>
		/// <returns>The converted ValueSet</returns>
        public IValueSet PerformDataOperation(IValueSet values)
		{
			if (_isActivated)
			{
				if (!(values is IScalarSet))
				{
					throw new Exception("The Oatc.OpenMI.Sdk.Wrapper packages only supports ScalarSets (Not VectorSets)");
				}

				double[] x = new double[values.Count];

				for (int i = 0; i < values.Count; i++)
				{
					x[i] = ((IScalarSet) values).GetScalar(i) * _a + _b;
				}

				return ((IValueSet) new ScalarSet(x));
			}

			return values; // return the values unchanged.
		}

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>Copy of the instance</returns>
    public object Clone()
    {
      LinearConversionDataOperation clone = new LinearConversionDataOperation();
      clone._a = _a;
      clone._b = _b;
      clone._isActivated = _isActivated;
      clone._arguments = new Argument[ArgumentCount];
      for(int i=0; i<ArgumentCount; i++)
      {
        clone._arguments[i] = new Oatc.OpenMI.Sdk.Backbone.Argument();
        clone._arguments[i].Description = _arguments[i].Description;
        clone._arguments[i].Key = _arguments[i].Key;
        clone._arguments[i].ReadOnly = _arguments[i].ReadOnly;
        clone._arguments[i].Value = _arguments[i].Value;
      }
      return clone;
    }

    #endregion
  }
}
