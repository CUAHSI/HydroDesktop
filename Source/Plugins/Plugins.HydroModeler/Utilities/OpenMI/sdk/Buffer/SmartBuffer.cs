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
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Buffer
{
  /// <summary>
  /// The SmartBuffer class provides bufferig functionality that will store values needed for a
  /// particular link in memory and functionality that will interpolate, extrapolate and aggregate 
  /// values from these values.
  /// </summary>
	[Serializable]
	public class SmartBuffer
	{
		ArrayList _times;
		ArrayList _values;
		double _relaxationFactor;  //Used for the extrapolation algorithm see also RelaxationFactor property
		bool _doExtendedDataVerification;
	
		/// <summary>
		/// Short description of the SmartBuffer
		/// </summary>
		/// <remarks>
		///The content of the SmartBuffer is lists of corresponding times and ValueSets,
		///where times can be TimeStamps or TimeSpans and the ValueSets can be ScalarSets or VectorSets.
		///Or in other words the content of the SmartBuffer is corresponding ScalarSets and TimeStamps, or ScalarSets and TimeSpans, or VectorSets and TimeStamps, or VectorSets and TimeSpans.
		///
		///SmartBuffer objects may not contain mixtures of TimeSpans and TimeStamps and may not contain mixtures of ScalarSets and VectorSets.
		///The number of Times (TimeSpans or TimeStamps) must equal the number of ValueSets ( ScalarSets or VectorSets) in the SmartBuffer.
		/// </remarks>
		public SmartBuffer()
		{
			Create();
		}

		/// <summary>
		/// Create a new SmartBuffer with values and times copied from another SmartBuffer
		/// </summary>
		/// <param name="smartBuffer">The SmartBuffer to copy</param>
        public SmartBuffer(SmartBuffer smartBuffer)
		{
			Create();

			if (smartBuffer.TimesCount > 0)
			{
				if (smartBuffer.GetTimeAt(0) is ITimeStamp && smartBuffer.GetValuesAt(0) is IScalarSet)
				{
					for (int i = 0; i < smartBuffer.TimesCount; i++)
					{
						AddValues(new TimeStamp((ITimeStamp) smartBuffer.GetTimeAt(i)),new ScalarSet((IScalarSet) smartBuffer.GetValuesAt(i)));
					}
				}

				if (smartBuffer.GetTimeAt(0) is ITimeStamp && smartBuffer.GetValuesAt(0) is IVectorSet)
				{
					for (int i = 0; i < smartBuffer.TimesCount; i++)
					{
						AddValues(new TimeStamp((ITimeStamp)smartBuffer.GetTimeAt(i)),new VectorSet((IVectorSet)smartBuffer.GetValuesAt(i)));
					}
				}

				if (smartBuffer.GetTimeAt(0) is ITimeSpan && smartBuffer.GetValuesAt(0) is IScalarSet)
				{
					for (int i = 0; i < smartBuffer.TimesCount; i++)
					{
						AddValues(new Backbone.TimeSpan((ITimeSpan)smartBuffer.GetTimeAt(i)),new ScalarSet((IScalarSet)smartBuffer.GetValuesAt(i)));
					}
				}

				if (smartBuffer.GetTimeAt(0) is ITimeSpan && smartBuffer.GetValuesAt(0) is IVectorSet)
				{
					for (int i = 0; i < smartBuffer.TimesCount; i++)
					{
						AddValues(new Backbone.TimeSpan((ITimeSpan)smartBuffer.GetTimeAt(i)),new VectorSet((IVectorSet)smartBuffer.GetValuesAt(i)));
					}
				}

				
			}

		}

		private void Create()
		{
			_times = new ArrayList();
			_values = new ArrayList();
			_doExtendedDataVerification = true;
			_relaxationFactor = 1.0;
		}

		/// <summary>
		///	Add corresponding values for time and values to the SmartBuffer.
		/// </summary>
		/// <param name="time"> Description of the time parameter</param>
		/// <param name="valueSet">Description of the values parameter</param>
		/// <remarks>
		/// The AddValues method will internally make a copy of the added times and values. The reason for
		/// doing this is that the times and values arguments are references, and the correspondign values 
		/// could be changed by the owner of the classes
		/// </remarks>
		public void AddValues(ITime time, IValueSet valueSet)
		{
			if (time is ITimeStamp)
			{
				_times.Add(new TimeStamp( ((ITimeStamp) time).ModifiedJulianDay ));
			}
			else if(time is ITimeSpan)
			{
				TimeStamp newStartTime = new TimeStamp(((ITimeSpan) time).Start.ModifiedJulianDay);
				TimeStamp newEndTime = new TimeStamp(((ITimeSpan) time).End.ModifiedJulianDay);

				Backbone.TimeSpan newTimeSpan = new Backbone.TimeSpan(newStartTime, newEndTime);
				_times.Add(newTimeSpan);
			}
			else
			{
				throw new Exception("Invalid datatype used for time argument in method AddValues");
			}

			if (valueSet is IScalarSet)
			{
				double[] x = new double[(valueSet).Count];
				for (int i = 0; i < x.Length; i++)
				{
					x[i] = ((IScalarSet) valueSet).GetScalar(i);
				}

				ScalarSet newScalarSet = new ScalarSet(x);

                if (valueSet is ScalarSet)
                {
                    newScalarSet.MissingValueDefinition =
                        ((ScalarSet)valueSet).MissingValueDefinition;
                    newScalarSet.CompareDoublesEpsilon =
                        ((ScalarSet)valueSet).CompareDoublesEpsilon;
                }

                _values.Add(newScalarSet);
			}
			else if (valueSet is IVectorSet)
			{			
				Vector[] vectors = new Vector[valueSet.Count];
				for (int i = 0; i < vectors.Length; i++)
				{
					vectors[i] = new Vector(((IVectorSet) valueSet).GetVector(i).XComponent, ((IVectorSet) valueSet).GetVector(i).YComponent, ((IVectorSet) valueSet).GetVector(i).ZComponent);
				}
			  VectorSet newVectorSet = new VectorSet(vectors);
				_values.Add(newVectorSet);
			}
			else
			{
				throw new Exception("Invalid datatype used for values argument in method AddValues");
			}
	
			if (_doExtendedDataVerification)
			{
				CheckBuffer();
			}
		}
			
		/// <summary>
    /// RelaxationFactor. The relaxation factor must be in the interval [0; 1]. The relaxation
    /// parameter is used when doing extrapolation. A value of 1 results in nearest extrapolation
    /// whereas a value 0 results in linear extrapolation.
		/// </summary>
		public double RelaxationFactor
		{
			get
			{
				return _relaxationFactor;
			}
			set
			{
				_relaxationFactor = value;
				if (_relaxationFactor < 0 || _relaxationFactor > 1)
				{
					throw new Exception("ReleaxationFactor is out of range");
				}
			}
		}
	
		/// <summary>
		/// Returns the timeStep´th ITime.
		/// </summary>
		/// <param name="timeStep">time step index</param>
    /// <returns>The timeStep´th ITime</returns>
		public ITime GetTimeAt(int timeStep)
		{
			if (_doExtendedDataVerification)
			{
				CheckBuffer();
			}
			return (ITime) _times[timeStep];
		}

		//===============================================================================================
		// GetValuesAt(int timeStep) : IValueSet
		//===============================================================================================
		/// <summary>
		/// Returns the timeStep´th IValueSet
		/// </summary>
		/// <param name="timeStep">time step index</param>
    /// <returns>The timeStep´th IValueSet</returns>
		public IValueSet GetValuesAt(int timeStep)
		{
			if (_doExtendedDataVerification)
			{
				CheckBuffer();
			}
			return (IValueSet) _values[timeStep];
		}

		/// <summary>
		/// Returns the ValueSet that corresponds to requestTime. The ValueSet may be found by 
		/// interpolation, extrapolation and/or aggregation.
		/// </summary>
		/// <param name="requestedTime">time for which the value is requested</param>
		/// <returns>valueSet that corresponds to requestTime</returns>
		public IValueSet GetValues(ITime requestedTime)
		{
			if (_doExtendedDataVerification)
			{
				CheckTime(requestedTime);
				CheckBuffer();
			}
			IValueSet returnValueSet;
			if (_values.Count == 0)
			{
				returnValueSet = new ScalarSet(); 
			}
			else if (_values.Count == 1)
			{
				returnValueSet = MakeCopyOfValues();
			}
			else if (requestedTime is ITimeStamp && _times[0] is ITimeStamp)
			{
				returnValueSet = MapFromTimeStampsToTimeStamp((ITimeStamp) requestedTime);
			}
			else if (requestedTime is ITimeSpan && _times[0] is ITimeSpan)
			{
				returnValueSet = MapFromTimeSpansToTimeSpan((ITimeSpan) requestedTime);
			}
			else if (requestedTime is ITimeSpan && _times[0] is ITimeStamp)
			{
				returnValueSet = MapFromTimeStampsToTimeSpan((ITimeSpan) requestedTime);
			}
			else if (requestedTime is ITimeStamp && _times[0] is ITimeSpan)
			{
				returnValueSet = MapFromTimeSpansToTimeStamp((ITimeStamp) requestedTime);
			}
			else
			{
				throw new Exception("Requested TimeMapping not available in SmartWrapper Class");
			}
			return returnValueSet;
		}

		/// <summary>
		/// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
		/// extrapolation in corresponding lists of ValueSets and TimeSpans.
		/// </summary>
		/// <param name="requestedTime">Time for which the ValueSet is requested</param>
		/// <returns>ValueSet that corresponds to requestedTime</returns>
		private IValueSet MapFromTimeSpansToTimeSpan(ITimeSpan requestedTime)

		{
      try
      {
        int	       m  = ((IValueSet)_values[0]).Count;
        double[][] xr = new double[m][];                                       // Values to return
        double trb    = requestedTime.Start.ModifiedJulianDay;   // Begin time in requester time interval
        double tre    = requestedTime.End.ModifiedJulianDay;     // End time in requester time interval

        int nk; // number of components (scalars has only 1 and vectors has 3 (3 axis))

        if (_values[0] is IVectorSet)
        {
          nk = 3;
        }
        else
        {
          nk = 1;
        }
				
        for (int i = 0; i < m; i++)
        {
          xr[i] = new double[nk];
        }

        for (int i = 0; i < m; i++)
        {
          for (int k = 0; k < nk; k++)
          {
            xr[i][k] = 0;
          }
        }

        for (int n = 0; n < _times.Count; n++)
        {
          double tbbn = ((ITimeSpan) _times[n]).Start.ModifiedJulianDay;
          double tben = ((ITimeSpan) _times[n]).End.ModifiedJulianDay;

          //---------------------------------------------------------------------------
          //    B:           <-------------------------->
          //    R:        <------------------------------------->
          // --------------------------------------------------------------------------
          if (trb <= tbbn && tre >= tben ) //Buffered TimeSpan fully included in requested TimeSpan
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = Support.GetVal((IValueSet)_values[n], i, k);
                xr[i][k-1] += sbin * (tben - tbbn)/(tre - trb);
              }
            }
          }

            //---------------------------------------------------------------------------
            //           Times[i] Interval:        t1|-----------------------|t2
            //           Requested Interval:          rt1|--------------|rt2
            // --------------------------------------------------------------------------
          else if (tbbn <= trb && tre <= tben) //cover all
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                xr[i][k-1] += Support.GetVal((IValueSet)_values[n], i, k);
              }
            }
          }

            //---------------------------------------------------------------------------
            //           Times[i] Interval:       t1|-----------------|t2
            //           Requested Interval:                 rt1|--------------|rt2
            // --------------------------------------------------------------------------
          else if (tbbn < trb && trb < tben && tre > tben)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = Support.GetVal((IValueSet)_values[n], i, k);
                xr[i][k-1] += sbin * (tben - trb)/(tre - trb);
              }
            }
          }

            //---------------------------------------------------------------------------
            //           Times[i] Interval:             t1|-----------------|t2
            //           Requested Interval:      rt1|--------------|rt2
            // --------------------------------------------------------------------------
          else if (trb < tbbn && tre > tbbn && tre < tben)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = Support.GetVal((IValueSet)_values[n], i, k);
                xr[i][k-1] += sbin * (tre - tbbn)/(tre - trb);
              }
            }
          }
        }

        //--------------------------------------------------------------------------
        //              |--------|---------|--------| B
        //        |----------------|                  R
        //---------------------------------------------------------------------------
        double tbb0 = ((ITimeSpan) _times[0]).Start.ModifiedJulianDay;
        double tbe0 = ((ITimeSpan) _times[0]).End.ModifiedJulianDay;
        //double tbb1 = ((ITimeSpan) _times[1]).Start.ModifiedJulianDay;
        double tbe1 = ((ITimeSpan) _times[1]).End.ModifiedJulianDay;

        if (trb < tbb0 && tre > tbb0)
        {
          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++)
            {
              double sbi0 = Support.GetVal((IValueSet)_values[0], i, k);
              double sbi1 = Support.GetVal((IValueSet)_values[1], i, k); 
              xr[i][k-1] += ((tbb0 - trb)/(tre - trb)) * (sbi0 - (1 - _relaxationFactor) * ((tbb0 - trb)*(sbi1 - sbi0)/(tbe1 - tbe0)));
            }
          }
        }

        //-------------------------------------------------------------------------------------
        //              |--------|---------|--------| B
        //                                    |----------------|                  R
        //-------------------------------------------------------------------------------------

        double tbeN_1 = ((ITimeSpan) _times[_times.Count-1]).End.ModifiedJulianDay;
        double tbbN_2 = ((ITimeSpan) _times[_times.Count-2]).Start.ModifiedJulianDay;

        if (tre > tbeN_1 && trb < tbeN_1)
        {
          //double tbeN_2 = ((ITimeSpan) _times[_times.Count-2]).End.ModifiedJulianDay;
          double tbbN_1 = ((ITimeSpan) _times[_times.Count-1]).Start.ModifiedJulianDay;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++)
            {
              double sbiN_1 = Support.GetVal((IValueSet)_values[_times.Count-1], i, k);
              double sbiN_2 = Support.GetVal((IValueSet)_values[_times.Count-2], i,k);
              xr[i][k-1] += ((tre - tbeN_1)/(tre - trb)) * (sbiN_1 + (1 - _relaxationFactor) * ((tre - tbbN_1)*(sbiN_1 - sbiN_2)/(tbeN_1 - tbbN_2)));
            }
          }
        }
        //-------------------------------------------------------------------------------------
        //              |--------|---------|--------| B
        //                                              |----------------|   R
        //-------------------------------------------------------------------------------------

        if (trb >= tbeN_1)
        {
          double tbeN_2 = ((ITimeSpan) _times[_times.Count-2]).End.ModifiedJulianDay;
          //double tbbN_1 = ((ITimeSpan) _times[_times.Count-1]).Start.ModifiedJulianDay;
			
          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++)
            {
              double sbiN_1 = Support.GetVal((IValueSet)_values[_times.Count-1], i, k);
              double sbiN_2 = Support.GetVal((IValueSet)_values[_times.Count-2], i, k);
              xr[i][k-1] = sbiN_1 + (1 - _relaxationFactor) * ((sbiN_1 - sbiN_2)/(tbeN_1 - tbbN_2)) * (trb + tre - tbeN_1 - tbeN_2);
            }
          }
        }

        //-------------------------------------------------------------------------------------
        //                           |--------|---------|--------| B
        //        |----------------|   R
        //-------------------------------------------------------------------------------------

        if (tre <= tbb0)
        {
          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++)
            {
              double sbi0 = Support.GetVal((IValueSet)_values[0], i, k);
              double sbi1 = Support.GetVal((IValueSet)_values[1], i, k);
              xr[i][k-1] = sbi0 - (1 - _relaxationFactor) * ((sbi1 - sbi0)/(tbe1- tbb0))*(tbe0 + tbb0 - tre - trb);
            }
          }
        }

        //-------------------------------------------------------------------------------------
        if (_values[0] is IVectorSet)
        {
          Vector [] vectors = new Vector[m]; 

          for (int i = 0; i < m; i++)
          {
            vectors[i] = new Vector(xr[i][0],xr[i][1],xr[i][2]);
          }

          VectorSet vectorSet = new VectorSet(vectors);

          return vectorSet;
        }
        else
        {
          double[] xx = new double[m];

          for (int i = 0; i < m; i++)
          {
            xx[i] = xr[i][0];
          }
				
          ScalarSet scalarSet = new ScalarSet(xx);

          return scalarSet;
        }
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeSpansToTimeSpan Failed",e);
      }
		}

		/// <summary>
		/// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
		/// extrapolation in corresponding lists of ValueSets and TimeStamps.
		/// </summary>
		/// <param name="requestedTime">Time for which the ValueSet is requested</param>
		/// <returns>ValueSet that corresponds to requestedTime</returns>
		private IValueSet MapFromTimeStampsToTimeSpan(ITimeSpan requestedTime)
		{
			try
      {
        int	       m  = ((IValueSet)_values[0]).Count;
			//int        N  = _times.Count;								   	      // Number of time steps in buffer
			double[][] xr = new double[m][];                                      // Values to return
			double trb    = requestedTime.Start.ModifiedJulianDay;   // Begin time in requester time interval
			double tre    = requestedTime.End.ModifiedJulianDay;    // End time in requester time interval

			int nk; // number of components (scalars has only 1 and vectors has 3 (3 axis))

			if (_values[0] is IVectorSet)
			{
				nk = 3;
			}
			else
			{
				nk = 1;
			}
				
			for (int i = 0; i < m; i++)
			{
				xr[i] = new double[nk];
			}

			for (int i = 0; i < m; i++)
			{
				for (int k = 0; k < nk; k++)
				{
					xr[i][k] = 0;
				}
			}


			for (int n = 0; n < _times.Count-1; n++)
			{
				double tbn   = ((ITimeStamp) _times[n]).ModifiedJulianDay;
				double tbnp1 = ((ITimeStamp) _times[n+1]).ModifiedJulianDay;
				

				//---------------------------------------------------------------------------
				//    B:           <-------------------------->
				//    R:        <------------------------------------->
				// --------------------------------------------------------------------------
				if (trb <= tbn && tre >= tbnp1 )
				{
					for (int k = 1; k <= nk; k++)
					{
						for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
						{
							double sbin   = Support.GetVal((IValueSet)_values[n], i, k);
							double sbinp1 = Support.GetVal((IValueSet)_values[n+1], i, k);
							xr[i][k-1] += 0.5 * (sbin + sbinp1) * (tbnp1 - tbn)/(tre - trb);
						}
					}
				}

				//---------------------------------------------------------------------------
				//           Times[i] Interval:        t1|-----------------------|t2
				//           Requested Interval:          rt1|--------------|rt2
				// --------------------------------------------------------------------------
				else if (tbn <= trb && tre <= tbnp1) //cover all
				{
					for (int k = 1; k <= nk; k++)
					{
						for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
						{
							double sbin   = Support.GetVal((IValueSet)_values[n], i, k);
							double sbinp1 = Support.GetVal((IValueSet)_values[n+1], i, k);
							xr[i][k-1] += sbin + ((sbinp1 - sbin)/(tbnp1 - tbn))*((tre + trb)/2 - tbn);
						}
					}
				}

				//---------------------------------------------------------------------------
				//           Times[i] Interval:       t1|-----------------|t2
				//           Requested Interval:                 rt1|--------------|rt2
				// --------------------------------------------------------------------------
				else if (tbn < trb && trb < tbnp1 && tre > tbnp1)
				{
					for (int k = 1; k <= nk; k++)
					{
						for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
						{
							double sbin   = Support.GetVal((IValueSet)_values[n], i, k);
							double sbinp1 = Support.GetVal((IValueSet)_values[n+1], i, k);
							xr[i][k-1] +=  (sbinp1 - (sbinp1 - sbin)/(tbnp1 - tbn)*((tbnp1 - trb)/2))* (tbnp1 - trb)/(tre - trb);
						}
					}
				}

				//---------------------------------------------------------------------------
				//           Times[i] Interval:             t1|-----------------|t2
				//           Requested Interval:      rt1|--------------|rt2
				// --------------------------------------------------------------------------
				else if (trb < tbn && tre > tbn && tre < tbnp1)
				{
					for (int k = 1; k <= nk; k++)
					{
						for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
						{
							double sbin   = Support.GetVal((IValueSet)_values[n], i, k);
							double sbinp1 = Support.GetVal((IValueSet)_values[n+1], i, k);
							xr[i][k-1] += (sbin + (sbinp1 - sbin)/(tbnp1 - tbn)*((tre - tbn)/2)) * (tre - tbn)/(tre - trb);
						}
					}
				}
			}
			//--------------------------------------------------------------------------
			//              |--------|---------|--------| B
			//        |----------------|                  R
			//---------------------------------------------------------------------------
			double tb0   = ((ITimeStamp) _times[0]).ModifiedJulianDay;
			//double tb1   = ((ITimeStamp) _times[0]).ModifiedJulianDay;
			double tb1   = ((ITimeStamp) _times[1]).ModifiedJulianDay; // line above was corrected to this Gregersen Sep 15 2004
			double tbN_1 = ((ITimeStamp) _times[_times.Count-1]).ModifiedJulianDay;
			double tbN_2 = ((ITimeStamp) _times[_times.Count-2]).ModifiedJulianDay;
			
			if (trb < tb0 && tre > tb0)
			{
				for (int k = 1; k <= nk; k++)
				{
					for (int i = 0; i < m; i++)
					{
						double sbi0 = Support.GetVal((IValueSet)_values[0], i, k);
						double sbi1 = Support.GetVal((IValueSet)_values[1], i, k);
						xr[i][k-1] += ((tb0 - trb)/(tre - trb)) * (sbi0 - (1 - _relaxationFactor) * 0.5 * ((tb0 - trb)*(sbi1 - sbi0)/(tb1 - tb0)));
					}
				}
			}
			//-------------------------------------------------------------------------------------
			//              |--------|---------|--------| B
			//                                    |----------------|                  R
			//-------------------------------------------------------------------------------------
			if (tre > tbN_1 && trb < tbN_1)
			{
				for (int k = 1; k <= nk; k++)
				{
					for (int i = 0; i < m; i++)
					{
						double sbiN_1 = Support.GetVal((IValueSet)_values[_times.Count-1], i, k);
						double sbiN_2 = Support.GetVal((IValueSet)_values[_times.Count-2], i, k);
						xr[i][k-1] += ((tre - tbN_1)/(tre - trb)) * (sbiN_1 + (1 - _relaxationFactor) * 0.5 * ((tre - tbN_1)*(sbiN_1 - sbiN_2)/(tbN_1 - tbN_2)));
					}
				}
			}
			//-------------------------------------------------------------------------------------
			//              |--------|---------|--------| B
			//                                              |----------------|   R
			//-------------------------------------------------------------------------------------
			if (trb >= tbN_1)
			{
				
				for (int k = 1; k <= nk; k++)
				{
					for (int i = 0; i < m; i++)
					{
						double sbiN_1 = Support.GetVal((IValueSet)_values[_times.Count-1], i, k);
						double sbiN_2 = Support.GetVal((IValueSet)_values[_times.Count-2], i, k);
					
						xr[i][k-1] = sbiN_1 + (1 - _relaxationFactor) * ((sbiN_1 - sbiN_2)/(tbN_1 - tbN_2)) * ( 0.5 * (trb + tre) - tbN_1);
					}
				}
			}
			//-------------------------------------------------------------------------------------
			//                           |--------|---------|--------| B
			//        |----------------|   R
			//-------------------------------------------------------------------------------------
			if (tre <= tb0)
			{
				for (int k = 1; k <= nk; k++)
				{
					for (int i = 0; i < m; i++)
					{
						double sbi0 = Support.GetVal((IValueSet)_values[0], i, k);
						double sbi1 = Support.GetVal((IValueSet)_values[1], i, k);
						xr[i][k-1] = sbi0 - (1 - _relaxationFactor) * ((sbi1 - sbi0)/(tb1- tb0))*(tb0 - 0.5 * (trb + tre));
					}
				}
			}
			//-------------------------------------------------------------------------------------
        if (_values[0] is IVectorSet)
        {
          Vector [] vectors = new Vector[m]; 

          for (int i = 0; i < m; i++)
          {
            vectors[i] = new Vector(xr[i][0],xr[i][1],xr[i][2]);
          }

          VectorSet vectorSet = new VectorSet(vectors);

          return vectorSet;
        }
        else
        {
          double[] xx = new double[m];

          for (int i = 0; i < m; i++)
          {
            xx[i] = xr[i][0];
          }
				
          ScalarSet scalarSet = new ScalarSet(xx);

          return scalarSet;
        }
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeStampsToTimeSpan Failed",e);
      }
		}
		

		/// <summary>
		/// Makes a copy of the first ValueSet in the list of valueSets
		/// </summary>
		/// <returns></returns>
		private IValueSet MakeCopyOfValues()
		{

			if (_values[0] is IScalarSet)
			{
				int NumberOfScalarsInEachScalarSet = ((IScalarSet) _values[0]).Count;
				double[] x = new Double[NumberOfScalarsInEachScalarSet];
				for (int i = 0; i < NumberOfScalarsInEachScalarSet; i++)
				{
					x[i] = ((IScalarSet) _values[0]).GetScalar(i);
				}
				ScalarSet scalarSet = new ScalarSet(x);
				return scalarSet;
			}

			else // _values[0] is VectorSet
			{
				
				int NumberOfVectorsInEachVectorSet = ((IVectorSet) _values[0]).Count;

				Vector[] vectors = new Vector[NumberOfVectorsInEachVectorSet];

				for (int i = 0; i < NumberOfVectorsInEachVectorSet; i++)
				{
          Vector vector;         
          double x = ((IVectorSet)_values[0]).GetVector(i).XComponent;
					double y = ((IVectorSet) _values[0]).GetVector(i).YComponent;
					double z = ((IVectorSet) _values[0]).GetVector(i).ZComponent;
					vector = new Vector(x, y, z);
					vectors[i] = vector;

				}
				VectorSet vectorSet = new VectorSet(vectors);
							
				return vectorSet;
			}
		}

    /// <summary>
    /// A ValueSet corresponding to a TimeStamp is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeStamps.
    /// </summary>
		/// <param name="requestedTimeStamp">TimeStamp for which the values are requested</param>
    /// <returns>ValueSet that corresponds to the requested time stamp</returns>
		private IValueSet MapFromTimeStampsToTimeStamp(ITimeStamp requestedTimeStamp)
		{
      try
      {
        int	     m  = ((IValueSet)_values[0]).Count;
        double[][] xr = new double[m][];                             // Values to return
        double   tr = requestedTimeStamp.ModifiedJulianDay;		     // Requested TimeStamp

        int nk; // number of components (scalars has only 1 and vectors has 3 (3 axis))

        if (_values[0] is IVectorSet)
        {
          nk = 3;
        }
        else
        {
          nk = 1;
        }

        for (int i = 0; i < m; i++)
        {
          xr[i] = new double[nk];
        }

        //---------------------------------------------------------------------------
        //  Buffered TimesStamps: |          >tb0<   >tb1<   >tb2<  >tbN<
        //  Requested TimeStamp:  |    >tr<
        //                         -----------------------------------------> t
        // --------------------------------------------------------------------------
        if (tr <= ((ITimeStamp)_times[0]).ModifiedJulianDay )
        {
          double tb0 = ((ITimeStamp) _times[0]).ModifiedJulianDay;
          double tb1 = ((ITimeStamp) _times[1]).ModifiedJulianDay;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m ; i++) //For each Vector in buffered VectorSet [0]
            {
              double sbi0 = Support.GetVal((IValueSet)_values[0], i, k);
              double sbi1       = Support.GetVal((IValueSet)_values[1], i, k);
              xr[i][k-1] = ((sbi0 - sbi1)/(tb0 - tb1))*(tr - tb0) * (1 - _relaxationFactor) + sbi0;
            }
          }
        }

          //---------------------------------------------------------------------------
          //  Buffered TimesStamps: |    >tb0<   >tb1<   >tb2<  >tbN_2<  >tbN_1<
          //  Requested TimeStamp:  |                                             >tr<
          //                         ---------------------------------------------------> t
          // --------------------------------------------------------------------------
        else if (tr > ((ITimeStamp) _times[_times.Count - 1]).ModifiedJulianDay)
        {
          double tbN_2 = ((ITimeStamp) _times[_times.Count - 2]).ModifiedJulianDay;
          double tbN_1 = ((ITimeStamp) _times[_times.Count - 1]).ModifiedJulianDay;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [N-1]
            {
              double sbiN_2     = Support.GetVal((IValueSet)_values[_times.Count-2], i, k);
              double sbiN_1     = Support.GetVal((IValueSet)_values[_times.Count-1], i, k);

              xr[i][k-1] = ((sbiN_1 - sbiN_2)/(tbN_1 - tbN_2))*(tr - tbN_1)*(1 - _relaxationFactor) + sbiN_1;
            }
          }
        }

          //---------------------------------------------------------------------------
          //  Availeble TimesStamps: |    >tb0<   >tb1<  >tbna<       >tnb<   >tbN_1<  >tbN_2<
          //  Requested TimeStamp:   |                          >tr<
          //                         -------------------------------------------------> t
          // --------------------------------------------------------------------------
        else
        {
          for (int n = _times.Count - 2; n >= 0 ; n--)
          {
            double tbn1 = ((ITimeStamp) _times[n]).ModifiedJulianDay;
            double tbn2 = ((ITimeStamp) _times[n+1]).ModifiedJulianDay;

            if ( tbn1 <= tr && tr <= tbn2)
            {
              for (int k = 1; k <= nk; k++)
              {
                for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [n]
                {
					IValueSet valueSet_n = (IValueSet)_values[n];
					double sbin1 = Support.GetVal(valueSet_n, i, k);

					IValueSet valueSet_nPlus1 = (IValueSet)_values[n+1];
                	double sbin2 = Support.GetVal(valueSet_nPlus1, i, k);;

                    //BROKEN:
					if ( valueSet_n.IsValid(i) && valueSet_nPlus1.IsValid(i))
					{
                        xr[i][k - 1] = ((sbin2 - sbin1) / (tbn2 - tbn1)) * (tr - tbn1) * (1 - _relaxationFactor) + sbin1;
					}
					else if ( valueSet_n.IsValid(i) ) 
					{
						xr[i][k-1] = sbin1;
					}
					else if ( valueSet_nPlus1.IsValid(i) ) 
					{
						xr[i][k-1] = sbin2;
					}
					else
					{
						// both invalid, set to one of the (==invalid) values
						xr[i][k-1] = sbin1;
					}
                }
              }
              break;
            }
          }
        }
        //----------------------------------------------------------------------------------------------
        if (_values[0] is IVectorSet)
        {
          Vector [] vectors = new Vector[m]; 

          for (int i = 0; i < m; i++)
          {
            vectors[i] = new Vector(xr[i][0],xr[i][1],xr[i][2]);
          }

          VectorSet vectorSet = new VectorSet(vectors);

          return vectorSet;
        }
        else
        {
          double[] xx = new double[m];

          for (int i = 0; i < m; i++)
          {
            xx[i] = xr[i][0];
          }
				
          ScalarSet scalarSet = new ScalarSet(xx);

          return scalarSet;
        }
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeStampsToTimeStamp Failed",e);
      }
		}

    /// <summary>
    /// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeSpans.
    /// </summary>
    /// <param name="requestedTimeStamp">Time for which the ValueSet is requested</param>
    /// <returns>ValueSet that corresponds to requestedTime</returns>
		private IValueSet MapFromTimeSpansToTimeStamp(ITimeStamp requestedTimeStamp)
		{
      try
      {
        int	     m  = ((IValueSet)_values[0]).Count;
        double[][] xr = new double[m][];                             // Values to return
        double   tr = requestedTimeStamp.ModifiedJulianDay; 	     // Requested TimeStamp

        int nk; // number of components (scalars has only 1 and vectors has 3 (3 axis))

        if (_values[0] is IVectorSet)
        {
          nk = 3;
        }
        else
        {
          nk = 1;
        }

        for (int i = 0; i < m; i++)
        {
          xr[i] = new double[nk];
        }

        //---------------------------------------------------------------------------
        //  Buffered TimesSpans:  |          >tbb0<  ..........  >tbbN<
        //  Requested TimeStamp:  |    >tr<
        //                         -----------------------------------------> t
        // --------------------------------------------------------------------------
        if (tr <= ((ITimeSpan)_times[0]).Start.ModifiedJulianDay )
        {
          double tbb0 = ((ITimeSpan) _times[0]).Start.ModifiedJulianDay;
          double tbb1 = ((ITimeSpan) _times[1]).Start.ModifiedJulianDay;
				
          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m ; i++) //For each Vector in buffered VectorSet [0]
            {
              double sbi0       = Support.GetVal((IValueSet)_values[0], i, k);
              double sbi1       = Support.GetVal((IValueSet)_values[1], i, k);
              xr[i][k-1] = ((sbi0 - sbi1)/(tbb0 - tbb1))*(tr - tbb0) * (1 - _relaxationFactor) + sbi0;
            }
          }
        }

          //---------------------------------------------------------------------------
          //  Buffered TimesSpans:  |    >tbb0<   .................  >tbbN_1<
          //  Requested TimeStamp:  |                                             >tr<
          //                         ---------------------------------------------------> t
          // --------------------------------------------------------------------------
        else if (tr >= ((ITimeSpan) _times[_times.Count - 1]).End.ModifiedJulianDay)
        {
          double tbeN_2 = ((ITimeSpan) _times[_times.Count - 2]).End.ModifiedJulianDay;
          double tbeN_1 = ((ITimeSpan) _times[_times.Count - 1]).End.ModifiedJulianDay;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [N-1]
            {
              double sbiN_2 = Support.GetVal((IValueSet)_values[_times.Count-2], i, k);
              double sbiN_1 = Support.GetVal((IValueSet)_values[_times.Count-1], i, k);

              xr[i][k-1] = ((sbiN_1 - sbiN_2)/(tbeN_1 - tbeN_2))*(tr - tbeN_1)*(1 - _relaxationFactor) + sbiN_1;
            }
          }
        }

          //---------------------------------------------------------------------------
          //  Availeble TimesSpans:  |    >tbb0<   ......................  >tbbN_1<
          //  Requested TimeStamp:   |                          >tr<
          //                         -------------------------------------------------> t
          // --------------------------------------------------------------------------
        else
        {
          for (int n = _times.Count - 1; n >= 0 ; n--)
          {
            double tbbn = ((ITimeSpan) _times[n]).Start.ModifiedJulianDay;
            double tben = ((ITimeSpan) _times[n]).End.ModifiedJulianDay;

            if ( tbbn <= tr && tr < tben)
            {
              for (int k = 1; k <= nk; k++)
              {
                for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [n]
                {
                  xr[i][k-1]     = Support.GetVal((IValueSet)_values[n], i, k);
                }
              }
              break;
            }
          }
        }

        //----------------------------------------------------------------------------------------------
	
		
        if (_values[0] is IVectorSet)
        {
          Vector [] vectors = new Vector[m]; 

          for (int i = 0; i < m; i++)
          {
            vectors[i] = new Vector(xr[i][0],xr[i][1],xr[i][2]);
          }

          VectorSet vectorSet = new VectorSet(vectors);

          return vectorSet;
        }
        else
        {
          double[] xx = new double[m];

          for (int i = 0; i < m; i++)
          {
            xx[i] = xr[i][0];
          }
				
          ScalarSet scalarSet = new ScalarSet(xx);

          return scalarSet;
        }
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeSpansToTimeStamp Failed",e);
      }
		}

		/// <summary>
		/// Number of time streps in the buffer.
		/// </summary>
		public int TimesCount
		{
			get
			{
				return _times.Count;
			}
		}

    /// <summary>
		/// Read only property for the number of values in each of the valuesets contained in the buffer.
		/// </summary>
		public int ValuesCount
		{
			get
			{
				return ((IValueSet) _values[0]).Count;
			}
		}

    /// <summary>
    /// Checks weather the contents of the buffer is valid.
    /// </summary>
    public void CheckBuffer()
		{
			if(_times.Count != _values.Count)
			{
				throw new Exception("Different numbers of values and times in buffer");
			}

			if(_times.Count == 0)
			{
				throw new Exception("Buffer is empty");
			}
			
			for (int i = 0; i < _times.Count; i++)
			{
				if (!(_times[i] is ITimeSpan || _times[i] is ITimeStamp))
				{
					throw new Exception("Illegal data type for time in buffer");
				}
			}

			for (int i = 0; i < _values.Count; i++)
			{
				if (!(_values[i] is IScalarSet || _values[i] is IVectorSet))
				{
					throw new Exception("Illegal data type for values in buffer");
				}
			}

			if (_times[0] is ITimeSpan)
			{
				foreach ( ITimeSpan t in _times)
				{
					if (t.Start.ModifiedJulianDay >= t.End.ModifiedJulianDay)
					{
						throw new Exception("BeginTime is larger than or equal to EndTime in TimeSpan");
					}
				}

				for (int i = 1; i < _times.Count; i++)
				{
					if (1.0e-8 < Math.Abs(((ITimeSpan)_times[i]).Start.ModifiedJulianDay - ((ITimeSpan)_times[i-1]).End.ModifiedJulianDay))
					{
						throw new Exception("EndTime is not equal to StartTime for the following time step");
					}
				}
			}
			if (_times[0] is ITimeStamp)
			{
				for (int i = 1; i < _times.Count; i++)
				{
					if (((ITimeStamp)_times[i]).ModifiedJulianDay <= ((ITimeStamp) _times[i-1]).ModifiedJulianDay)
						throw new Exception("TimeStamps are not increasing in buffer");
				}
			}
		}

    /// <summary>
    /// Validates a given time. The check made is for TimeSpan the starting time must be smaller 
    /// than the end time. Throws exception if the time is not valid.
    /// </summary>
    private static void CheckTime(ITime time)
		{
			if (time is ITimeSpan)
			{
				if(((ITimeSpan) time).Start.ModifiedJulianDay >= ((ITimeSpan) time).End.ModifiedJulianDay)
				{
					throw new Exception("Start Time is larger than or equal to End Time in TimeSpan");
				}
			}
		}

    /// <summary>
    /// Read/Write property flag that indicates wheather or not to perform extended data
    /// checking.
    /// </summary>
    public bool DoExtendedDataVerification
		{
			get
			{
				return _doExtendedDataVerification;
			}
			set
			{
				_doExtendedDataVerification = value;
			}
		}


		/// <summary>
		/// Clear all times and values in the buffer at or later than the specified time
		/// If the specified time is type ITimeSpan the Start time is used.
		/// </summary>
		/// <param name="time"></param>
		public void ClearAfter(ITime time)
		{
			//TODO: this method can be simplyfied (see implementation of method: ClearBefore
			TimeStamp timeStamp = new TimeStamp();
			if (time is ITimeStamp)
			{
				timeStamp.ModifiedJulianDay = ((ITimeStamp) time).ModifiedJulianDay;
			}
			else if(time is ITimeSpan)
			{
				timeStamp.ModifiedJulianDay = ((ITimeSpan) time).Start.ModifiedJulianDay;
			}
			else
			{
				throw new Exception("Wrong argument type for call to Oatc.OpenMI.Sdk.Buffer.SmartBuffer.ClearAfter()");
			}

			
			if (_times.Count > 0)
			{
        bool recordWasRemoved;
				if(_times[0] is ITimeStamp)
				{
          do
					{
						recordWasRemoved = false;
						if (((ITimeStamp)_times[_times.Count - 1]).ModifiedJulianDay >= timeStamp.ModifiedJulianDay)
						{
							_values.RemoveAt(_times.Count - 1);
							_times.RemoveAt(_times.Count - 1);
							recordWasRemoved = true;
							
						}
					} while (recordWasRemoved && _times.Count > 0);
					
				}
				else if (_times[0] is ITimeSpan)
				{
					do
					{
						recordWasRemoved = false;
						if (((ITimeSpan)_times[_times.Count - 1]).Start.ModifiedJulianDay >= timeStamp.ModifiedJulianDay)
						{
							_values.RemoveAt(_times.Count - 1);
							_times.RemoveAt(_times.Count - 1);
							recordWasRemoved = true;
						}
					} while (recordWasRemoved && _times.Count > 0);
					
				}
			}
			
		}

		
		/// <summary>
		/// Clear all records in the buffer assocaited to time that is earlier that the
		/// time specified in the argument list. However, one record associated to time 
		/// before the time in the argument list is left in the buffer.
		/// The criteria when comparing TimeSpans is that they may not overlap in order
		/// to be regarded as before each other.
		/// (see also Oatc.OpenMI.Sdk.Buffer.Support.IsBefore(ITime ta, ITime tb)
		/// </summary>
		/// <param name="time">time before which the records are removed</param>
		public void ClearBefore(ITimeStamp time)
		{
			int numberOfRecordsToRemove = 0;

			foreach (ITime t in _times)
			{
				if (Support.IsBefore(t, time))
				{
					numberOfRecordsToRemove ++;
				}
			}

			numberOfRecordsToRemove--; // decrease index to ensure that one record before time is left back


			if (numberOfRecordsToRemove > 0)
			{
				_times.RemoveRange(0,numberOfRecordsToRemove);
				_values.RemoveRange(0,numberOfRecordsToRemove);
			}
		}

    /// <summary>
    /// Clears the buffer between start- and end- time of the time (TimeSpan).
    /// </summary>
    public void Clear(ITimeSpan time)

		//TODO: This method may not be used anywhere. Check if this is true and then remove this method
		{
			if (_times.Count > 0)
			{
				if(_times[0] is ITimeStamp)
				{
					for (int i = 0; i < _times.Count; i++)
					{
						if (((ITimeStamp)_times[i]).ModifiedJulianDay > time.Start.ModifiedJulianDay && ((ITimeStamp)_times[i]).ModifiedJulianDay < time.End.ModifiedJulianDay)
						{
							_times.RemoveAt(i);
							_values.RemoveAt(i);
						}
					}
				}
				else if (_times[0] is ITimeSpan)
				{
					for (int i = 0; i < _times.Count; i++)
					{
						if (((ITimeSpan)_times[i]).Start.ModifiedJulianDay > time.Start.ModifiedJulianDay && ((ITimeSpan)_times[i]).End.ModifiedJulianDay < time.End.ModifiedJulianDay)
						{
							_times.RemoveAt(i);
							_values.RemoveAt(i);
						}
					}
				}
			}
		}
	}
}
