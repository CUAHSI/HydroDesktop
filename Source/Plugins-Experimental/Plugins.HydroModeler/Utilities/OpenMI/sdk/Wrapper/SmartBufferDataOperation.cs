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

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The SmartBuffer data operation class is used to define the temporal relaxations factors and to define the level of validation 
	/// </summary>
  public class SmartBufferDataOperation : IDataOperation, ICloneable
	{
		Oatc.OpenMI.Sdk.Backbone.Argument[] _arguments;
		double _relaxationFactor;
		bool   _doExtendedValidation;
		bool _isActivated;

		/// <summary>
		/// Constructor
		/// </summary>
        public SmartBufferDataOperation()
		{
			_arguments = new Oatc.OpenMI.Sdk.Backbone.Argument[3];

			_arguments[0] = new Oatc.OpenMI.Sdk.Backbone.Argument();
			_arguments[0].Description = "Arguments associated the buffering and extrapolation";
			_arguments[0].Key = "Type";
			_arguments[0].Value = "SmartBuffer Arguments";
			_arguments[0].ReadOnly = true;

			_arguments[1] = new Oatc.OpenMI.Sdk.Backbone.Argument();
			_arguments[1].Description = "Relaxation factor used for temporal extrapolation must be in the interval [0.0,1.0]";
			_arguments[1].Key = "Relaxation Factor";
			_arguments[1].Value = "0.0";
			_arguments[1].ReadOnly = false;

			_arguments[2] = new Oatc.OpenMI.Sdk.Backbone.Argument();
			_arguments[2].Description = "Do extended validation. Must be \"true\" or \"false\"";
			_arguments[2].Key = "Do Extended Data Validation";
			_arguments[2].Value = "true";
			_arguments[2].ReadOnly = false;

			_isActivated = false;
		}
		#region IDataOperation Members: 

		/// <summary>
		/// Data operation ID
		/// </summary>
        public string ID
		{
			get
			{
				return "Buffering and temporal extrapolation";
			}
		}

		/// <summary>
		/// This data operation can be combined with any other data operation, so this method always return true
		/// </summary>
		/// <param name="inputExchangeItem">input exchange items</param>
		/// <param name="outputExchangeItem">output exchange items</param>
		/// <param name="SelectedDataOperations">the selected data operations</param>
		/// <returns>true if valid, false if invalid</returns>
        public bool IsValid(IInputExchangeItem inputExchangeItem, IOutputExchangeItem outputExchangeItem, IDataOperation[] SelectedDataOperations)
		{
			return true;
		}

		/// <summary>
		/// Number of arguments
		/// </summary>
        public int ArgumentCount
		{
			get
			{
				return _arguments.Length;
			}
		}

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="properties">parameters</param>
        public void Initialize(IArgument[] properties)
		{
		
		}

		/// <summary>
		/// get argument
		/// </summary>
		/// <param name="argumentIndex">index for the requested argument</param>
		/// <returns>the requested argument</returns>
        public IArgument GetArgument(int argumentIndex)
		{
			return _arguments[argumentIndex];
		}

		#endregion

		/// <summary>
		/// prepare
		/// </summary>
        public void Prepare()
		{
			bool argumentRelaxationFactorWasFound = false;
			bool argumentDoExtendedValidationWasFound = false;

			_isActivated = true;

			for (int i = 0; i < this._arguments.Length; i++)
			{
				if (_arguments[i].Key == _arguments[1].Key) //Relaxation Factor
				{
					_relaxationFactor = Convert.ToDouble(_arguments[i].Value);
					argumentRelaxationFactorWasFound = true;
				}

				if (_arguments[i].Key == _arguments[2].Key) //Do extended validation
				{
					_doExtendedValidation = Convert.ToBoolean(_arguments[i].Value);
					argumentDoExtendedValidationWasFound = true;
				}
			}
			if (!argumentRelaxationFactorWasFound || !argumentDoExtendedValidationWasFound)
			{
				throw new Exception("Missing argument in data operation: \"Linear Conversion\"");
			}
		}

		/// <summary>
		/// If true the component will do extended data validation
		/// </summary>
        public bool DoExtendedValidation
		{
			get
			{
				if (!_isActivated)
				{
					throw new Exception("Attemt to use DoExtendedValidation property in SmartBufferDataOperation before the prepare() method was invoked");
				}
				return _doExtendedValidation;
			}
		}

		/// <summary>
		/// Relaxation factor for temporal extrapolation
		/// </summary>
        public double RelaxationFactor
		{
			get
			{
				if (!_isActivated)
				{
					throw new Exception("Attemt to use Relaxation property in SmartBufferDataOperation before the prepare() method was invoked");
				}
				return _relaxationFactor;
			}
		}

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>Copy of the instance</returns>
    public object Clone()
    {
      SmartBufferDataOperation clone = new SmartBufferDataOperation();
      clone._relaxationFactor = _relaxationFactor;
      clone._doExtendedValidation = _doExtendedValidation;
      clone._isActivated = _isActivated;
      clone._arguments = new Argument[ArgumentCount];
      for (int i = 0; i < ArgumentCount; i++)
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
