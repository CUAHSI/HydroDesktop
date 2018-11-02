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
using System.Runtime.Remoting;
 
using Oatc.OpenMI.Sdk.Spatial; 
using OpenMI.Standard;  
using Oatc.OpenMI.Sdk.Backbone;  
 
using Oatc.OpenMI.Sdk.Buffer; 

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// Smart output link
	/// </summary>
    [Serializable]
	public class SmartOutputLink  : SmartLink
	{
		private SmartBuffer smartBuffer;
		private ElementMapper elementMapper;
		private bool _useSpatialMapping;
		LinearConversionDataOperation _linearDataOperation;
		private Hashtable _bufferStates;

		

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="engine">reference to the engine</param>
		/// <param name="link">The ILink object</param>
        public SmartOutputLink(IRunEngine engine, ILink link)
		{
            this._link = link;
            this._engine = engine;
		}

        /// <summary>
        /// Initialize will create buffers, prepare data operations and create the mapping matrice when georeferenced links are used.
        /// The initialize method should be invoked in the ILinkableComponent prepare method (is done from the LinkableRunEngine).
        /// </summary>
        public void Initialize()
        {
            _bufferStates = new Hashtable();

            smartBuffer = new SmartBuffer();
            _useSpatialMapping = false;
            _linearDataOperation = null;

            

            //Setup Spatial mapper - mapping method is set to default for now!
            int index = -1;
            string description = " ";
            for (int i = 0; i < link.DataOperationsCount; i++)
            {
                for (int n = 0; n < link.GetDataOperation(i).ArgumentCount; n++)
                {
                    if (link.GetDataOperation(i).GetArgument(n).Key == "Type" && link.GetDataOperation(i).GetArgument(n).Value == "SpatialMapping")
                    {
                        for (int m = 0; m < link.GetDataOperation(i).ArgumentCount; m++)
                        {
                            if (link.GetDataOperation(i).GetArgument(m).Key == "Description")
                            {
                                description = link.GetDataOperation(i).GetArgument(m).Value;
                                break;
                            }
                        }
                        index = i;
                        break;
                    }
                }
                if (index == i)
                {
                    break;
                }
            }

            if (index >= 0)
            {
                if (description == " ")
                {
                    throw new Exception("Missing key: \"Description\" in spatial dataoperation arguments");
                }
                _useSpatialMapping = true;
                elementMapper =new ElementMapper();
                elementMapper.Initialise(description,link.SourceElementSet, link.TargetElementSet);
            }

            //Prepare linear data operation
            for(int i = 0; i < link.DataOperationsCount; i++)
            {
                if (link.GetDataOperation(i).ID == (new LinearConversionDataOperation()).ID)
                {
                    _linearDataOperation = (LinearConversionDataOperation) link.GetDataOperation(i);
                    _linearDataOperation.Prepare();
                    break;
                }
            }

            //prepare SmartBufferDataOperation
            for(int i = 0; i < link.DataOperationsCount; i++)
            {
                if (link.GetDataOperation(i).ID == (new SmartBufferDataOperation()).ID)
                {
                    ((SmartBufferDataOperation)link.GetDataOperation(i)).Prepare();
                    smartBuffer.DoExtendedDataVerification = ((SmartBufferDataOperation)link.GetDataOperation(i)).DoExtendedValidation;
                    smartBuffer.RelaxationFactor = ((SmartBufferDataOperation)link.GetDataOperation(i)).RelaxationFactor;
                    break;
                }
            }

        }

        /// <summary>
        /// The last time in the buffer
        /// </summary>
        /// <returns>the latest time in the buffer</returns>
        public ITimeStamp GetLastBufferedTime()
        {
            ITime time = SmartBuffer.GetTimeAt(SmartBuffer.TimesCount - 1);
            if (time is ITimeSpan)
            {
                return new TimeStamp( ((ITimeSpan)time).End);
            }
            else
            {
                return (ITimeStamp) time;
            }
        }

        /// <summary>
        /// The SmartBuffer associated to the SmartOutputLink
        /// </summary>
        public SmartBuffer SmartBuffer
        {
            get
            {
                return smartBuffer;
            }
        }

		/// <summary>
		/// Update the associated buffer with the last values calculated by the engine
		/// </summary>
        public virtual void UpdateBuffer()
		{	
            if ((link.SourceQuantity != null) && (link.SourceElementSet != null)) 
            {

                if (this.Engine is Oatc.OpenMI.Sdk.Wrapper.IAdvancedEngine)
                {
                    TimeValueSet timeValueSet = ((IAdvancedEngine) this.Engine).GetValues(link.SourceQuantity.ID, link.SourceElementSet.ID);

                    if (timeValueSet.Time != null)
                    {
                        if (_useSpatialMapping)
                        {
                            this.smartBuffer.AddValues(timeValueSet.Time, elementMapper.MapValues(timeValueSet.ValueSet));
                        }
                        else
                        {
                            this.smartBuffer.AddValues(timeValueSet.Time, timeValueSet.ValueSet);
                        }
                    }
                }
                else // the engine is IEngine or IRunEngine
                {
            
                    ITime time = this.Engine.GetCurrentTime();

               
                    IValueSet valueSet = this.Engine.GetValues(link.SourceQuantity.ID,link.SourceElementSet.ID); 
              

                    if (_useSpatialMapping)
                    {
                        this.smartBuffer.AddValues(time,elementMapper.MapValues(valueSet)); 
                    }
                    else
                    {
                        this.smartBuffer.AddValues(time,valueSet);   
                    }
                
                }
            }

			SmartBuffer.ClearBefore(link.TargetComponent.EarliestInputTime);
			
		}

		/// <summary>
		/// Retrieves a value from the buffer that applies to the time passes as argument. 
		/// During this process the buffer will do temporal operations, 
		/// such as extrapolations, interpolations, or aggregation
		/// </summary>
		/// <param name="time">The time for which the values should apply</param>
		/// <returns>The values</returns>
        public virtual IValueSet GetValue(ITime time)
		{
			IValueSet values = this.SmartBuffer.GetValues(time);
		
			if (_linearDataOperation != null)
			{
				values = _linearDataOperation.PerformDataOperation(values);
			}

			return ConvertUnit(values);
		}

		/// <summary>
		/// Convert the units according the what is specified in the link
		/// </summary>
		/// <param name="values">The values</param>
		/// <returns>The unit converted values</returns>
        private IValueSet ConvertUnit(IValueSet values)
		{
			double aSource = link.SourceQuantity.Unit.ConversionFactorToSI;
			double bSource = link.SourceQuantity.Unit.OffSetToSI;
			double aTarget = link.TargetQuantity.Unit.ConversionFactorToSI;
			double bTarget = link.TargetQuantity.Unit.OffSetToSI;

			if (aSource != aTarget || bSource != bTarget)
			{
				if (values is IScalarSet) 
				{
					double[] x = new double[values.Count];

					for (int i = 0; i < values.Count; i++)
					{
						x[i] = (((IScalarSet) values).GetScalar(i) * aSource + bSource - bTarget) / aTarget;
					}

					return new ScalarSet(x);
				}
				else if (values is IVectorSet) 
				{
					ArrayList vectors = new ArrayList();

					for (int i = 0; i < values.Count; i++)
					{
						double x = (((IVectorSet) values).GetVector(i).XComponent * aSource + bSource - bTarget) / aTarget;
						double y = (((IVectorSet) values).GetVector(i).YComponent * aSource + bSource - bTarget) / aTarget;
						double z = (((IVectorSet) values).GetVector(i).ZComponent * aSource + bSource - bTarget) / aTarget;

						Vector newVector = new Vector(x, y, z);
						vectors.Add (newVector);
					}

					return new VectorSet((Vector[]) vectors.ToArray(typeof(Vector)));
				}
				else 
				{
					throw new Exception ("Type " + values.GetType().FullName + " not suppported for unit conversion");
				}
			}

			return values;
		}

		/// <summary>
		/// Saves a copy of the buffer
		/// </summary>
		/// <param name="bufferStateID">ID for the saved buffer state</param>
        public void KeepCurrentBufferState(string bufferStateID)
		{
			_bufferStates.Add(bufferStateID, new SmartBuffer(this.SmartBuffer));
		}

		/// <summary>
		/// Clears a buffer state
		/// </summary>
		/// <param name="bufferStateID">ID for the state to clear</param>
        public void ClearBufferState(string bufferStateID)
		{
			_bufferStates.Remove(bufferStateID);
		}

		/// <summary>
		/// Restores the buffer with a previously saved buffer state
		/// </summary>
		/// <param name="bufferStateID">ID for the state to restore</param>
        public void RestoreBufferState(string bufferStateID)
		{
			this.smartBuffer = new SmartBuffer((SmartBuffer) _bufferStates[bufferStateID]);
		}
	}
}
