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
using Oatc.OpenMI.Sdk.Buffer;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Buffer.UnitTest
{
  /// <summary>
	/// The SmartBufferTest class serves as teting of all public methods in the
	/// Oatc.OpenMI.Sdk.Buffer.SmartBuffer class. The SmartBufferTest class 
	/// is used with the NUnit software.
	/// </summary>
	[TestFixture]
  public class SmartBufferTest
	{

		[Test]
		public void SmartBuffer()
		{
			// Testing the overloaded constructor SmartBuffer.SmartBuffer(SmartBuffer buffer)
			// Note: this test does not include testing for buffers containing VectorSets

			SmartBuffer smartBufferA = new SmartBuffer();
			smartBufferA.AddValues(new TimeStamp(1), new ScalarSet(new double[3] { 1, 2, 3 }));
			smartBufferA.AddValues(new TimeStamp(3), new ScalarSet(new double[3] { 3, 4, 5 }));
			smartBufferA.AddValues(new TimeStamp(6), new ScalarSet(new double[3] { 6, 7, 8 }));

			SmartBuffer buffer1 = new SmartBuffer(smartBufferA);
			Assert.AreEqual(smartBufferA.TimesCount, buffer1.TimesCount);
			for (int i = 0; i < smartBufferA.TimesCount; i++)
			{
				Assert.AreEqual(((ITimeStamp)smartBufferA.GetTimeAt(i)).ModifiedJulianDay, ((ITimeStamp)buffer1.GetTimeAt(i)).ModifiedJulianDay);
				for (int n = 0; n < smartBufferA.ValuesCount; n++)
				{
					Assert.AreEqual(((IScalarSet) smartBufferA.GetValuesAt(i)).GetScalar(n),((IScalarSet) buffer1.GetValuesAt(i)).GetScalar(n));
				}
			}

			SmartBuffer smartBufferB = new SmartBuffer();
			smartBufferB.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(1),new TimeStamp(2)), new ScalarSet(new double[3] { 11, 12, 13 }));
			smartBufferB.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(2),new TimeStamp(3)), new ScalarSet(new double[3] { 13, 14, 15 }));
			smartBufferB.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(3),new TimeStamp(5)), new ScalarSet(new double[3] { 16, 17, 18 }));

			SmartBuffer buffer2 = new SmartBuffer(smartBufferB);
			Assert.AreEqual(smartBufferB.TimesCount, buffer2.TimesCount);
			for (int i = 0; i < smartBufferB.TimesCount; i++)
			{
				Assert.AreEqual(((ITimeSpan)smartBufferB.GetTimeAt(i)).Start.ModifiedJulianDay, ((ITimeSpan)buffer2.GetTimeAt(i)).Start.ModifiedJulianDay);
				Assert.AreEqual(((ITimeSpan)smartBufferB.GetTimeAt(i)).End.ModifiedJulianDay, ((ITimeSpan)buffer2.GetTimeAt(i)).End.ModifiedJulianDay);

				for (int n = 0; n < smartBufferB.ValuesCount; n++)
				{
					Assert.AreEqual(((IScalarSet) smartBufferB.GetValuesAt(i)).GetScalar(n),((IScalarSet) buffer2.GetValuesAt(i)).GetScalar(n));
				}
			}
      	}


    [Test] // testing the Initialise method
    public void GetValues_TimeStampsToTimeStamp_01()
    {
      SmartBuffer smartBuffer = new SmartBuffer();
      smartBuffer.AddValues(new TimeStamp(1), new ScalarSet(new double[3] { 1, 2, 3 }));
      smartBuffer.AddValues(new TimeStamp(3), new ScalarSet(new double[3] { 3, 4, 5 }));
      
      //ScalarSet scalarSet = (ScalarSet) smartBuffer.GetValues(new TimeStamp(2));
      double a = smartBuffer.RelaxationFactor;
      // Extrapolation
      Assert.AreEqual(new ScalarSet(new double[3] { (1-3)/(3-1)*(1-a)+1, (2-4)/(3-1)*(1-a)+2, (3-5)/(3-1)*(1-a)+3 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(0)));
      // "Hit" first TimeStamp
      Assert.AreEqual(new ScalarSet(new double[3] { 1, 2, 3 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(1)));
      // Interpolation
      Assert.AreEqual(new ScalarSet(new double[3] { 2, 3, 4 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(2)));
      // "Hit" last TimeStamp
      Assert.AreEqual(new ScalarSet(new double[3] { 3, 4, 5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(3)));
      // Extrapolation
      Assert.AreEqual(new ScalarSet(new double[3] { (3-1)/(3-1)*(1-a)+3, (4-2)/(3-1)*(1-a)+4, (5-3)/(3-1)*(1-a)+5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(4)));
		
    }

	[Test] // testing the Initialise method
	public void GetValues_TimeStampsToTimeStamp_02()
	{
		// GetValues_TimeStampsToTimeStamp_02()method is teting situation where the Times in
		// the buffer is of type ITimeStamp and the requested values is associated to a ITimeStamp
        //
		// Three different relaxation factors are used
      

		// See drawing at the following link.
		//		http://projects.dhi.dk/harmonIT/WP6/SourceCodeDocumentation/Oatc.OpenMI.Sdk.Buffer.TestCode.htm

	
		SmartBuffer smartBuffer = new SmartBuffer();

		

		// --Populate the SmartBuffer --
		smartBuffer.AddValues(new TimeStamp(10), new ScalarSet(new double[3] { 11, 2, 6}));
		smartBuffer.AddValues(new TimeStamp(13), new ScalarSet(new double[3] { 5 , 5, 6}));
		smartBuffer.AddValues(new TimeStamp(16), new ScalarSet(new double[3] { 2 ,14, 6}));
		smartBuffer.AddValues(new TimeStamp(20), new ScalarSet(new double[3] { 2 , 2, 6}));
		smartBuffer.AddValues(new TimeStamp(27), new ScalarSet(new double[3] { 2 , 9, 6}));
		smartBuffer.AddValues(new TimeStamp(30), new ScalarSet(new double[3] {-4, 9, 6}));
		smartBuffer.AddValues(new TimeStamp(48), new ScalarSet(new double[3] { 8,- 3, 6}));

		smartBuffer.RelaxationFactor = 0.0;

		Assert.AreEqual(new ScalarSet(new double[3] {13,1,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(9)));
		Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
		Assert.AreEqual(new ScalarSet(new double[3] {9,3,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
		Assert.AreEqual(new ScalarSet(new double[3] {7,4,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
		Assert.AreEqual(new ScalarSet(new double[3] {5,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
		Assert.AreEqual(new ScalarSet(new double[3] {4,8,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
		Assert.AreEqual(new ScalarSet(new double[3] {3,11,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(15)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,14,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(16)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,11,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(17)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,2,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(20)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,3,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(21)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(23)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,9,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(27)));
		Assert.AreEqual(new ScalarSet(new double[3] {0,9,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(28)));
		Assert.AreEqual(new ScalarSet(new double[3] {-2,9,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(29)));
		Assert.AreEqual(new ScalarSet(new double[3] {-4,9,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(30)));
		Assert.AreEqual(new ScalarSet(new double[3] {-2,7,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(33)));
		Assert.AreEqual(new ScalarSet(new double[3] {0,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(36)));
		Assert.AreEqual(new ScalarSet(new double[3] {4,1,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(42)));
		Assert.AreEqual(new ScalarSet(new double[3] {8,-3,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(48)));
		Assert.AreEqual(new ScalarSet(new double[3] {12,-7,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(54)));

		smartBuffer.RelaxationFactor = 1.0;

		Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(9)));
		Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
		Assert.AreEqual(new ScalarSet(new double[3] {9,3,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
		Assert.AreEqual(new ScalarSet(new double[3] {7,4,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
		Assert.AreEqual(new ScalarSet(new double[3] {5,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
		Assert.AreEqual(new ScalarSet(new double[3] {4,8,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
		Assert.AreEqual(new ScalarSet(new double[3] {3,11,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(15)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,14,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(16)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,11,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(17)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,2,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(20)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,3,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(21)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(23)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,9,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(27)));
		Assert.AreEqual(new ScalarSet(new double[3] {0,9,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(28)));
		Assert.AreEqual(new ScalarSet(new double[3] {-2,9,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(29)));
		Assert.AreEqual(new ScalarSet(new double[3] {-4,9,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(30)));
		Assert.AreEqual(new ScalarSet(new double[3] {-2,7,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(33)));
		Assert.AreEqual(new ScalarSet(new double[3] {0,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(36)));
		Assert.AreEqual(new ScalarSet(new double[3] {4,1,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(42)));
		Assert.AreEqual(new ScalarSet(new double[3] {8,-3,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(48)));
		Assert.AreEqual(new ScalarSet(new double[3] {8,-3,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(54)));

		double a = 0.7;
		smartBuffer.RelaxationFactor = a;

		Assert.AreEqual(new ScalarSet(new double[3] {11 + (1 - a) * 2,2 - (1 - a)* 1 ,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(9)));
		Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
		Assert.AreEqual(new ScalarSet(new double[3] {9,3,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
		Assert.AreEqual(new ScalarSet(new double[3] {7,4,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
		Assert.AreEqual(new ScalarSet(new double[3] {5,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
		Assert.AreEqual(new ScalarSet(new double[3] {4,8,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
		Assert.AreEqual(new ScalarSet(new double[3] {3,11,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(15)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,14,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(16)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,11,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(17)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,2,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(20)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,3,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(21)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(23)));
		Assert.AreEqual(new ScalarSet(new double[3] {2,9,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(27)));
		Assert.AreEqual(new ScalarSet(new double[3] {0,9,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(28)));
		Assert.AreEqual(new ScalarSet(new double[3] {-2,9,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(29)));
		Assert.AreEqual(new ScalarSet(new double[3] {-4,9,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(30)));
		Assert.AreEqual(new ScalarSet(new double[3] {-2,7,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(33)));
		Assert.AreEqual(new ScalarSet(new double[3] {0,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(36)));
		Assert.AreEqual(new ScalarSet(new double[3] {4,1,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(42)));
		Assert.AreEqual(new ScalarSet(new double[3] {8,-3,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(48)));
		Assert.AreEqual(new ScalarSet(new double[3] {8 + (1 - a) * 4,-3 - (1 - a) *4, 6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(54)));
	}

		
		/// <summary>
		/// 
		/// </summary>
[Test] 
		
		public void GetValues_TimeStampsToTimeStamp_05()
		{
			//-------------------------------------------------------------------------------------------------
			// Only two ValueSets in buffer
			//-------------------------------------------------------------------------------------------------

			SmartBuffer smartBuffer = new SmartBuffer();

			smartBuffer.RelaxationFactor = 0;

			smartBuffer.AddValues(new TimeStamp(10), new ScalarSet(new double[3] { 11, 2, 6}));
			smartBuffer.AddValues(new TimeStamp(13), new ScalarSet(new double[3] { 5 , 5, 6}));
			
			Assert.AreEqual(new ScalarSet(new double[3] {13,1,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(9)));
			Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
			Assert.AreEqual(new ScalarSet(new double[3] {9,3,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
			Assert.AreEqual(new ScalarSet(new double[3] {7,4,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
			Assert.AreEqual(new ScalarSet(new double[3] {5,5,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
			Assert.AreEqual(new ScalarSet(new double[3] {3,6,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
		}

		[Test]
		public void GetValues_TimeStampsToTimeStamp_06()
		{
			//-------------------------------------------------------------------------------------------------
			// Only one ValueSets in buffer
			//-------------------------------------------------------------------------------------------------

			SmartBuffer smartBuffer = new SmartBuffer();

			smartBuffer.RelaxationFactor = 0;

			smartBuffer.AddValues(new TimeStamp(10), new ScalarSet(new double[3] { 11, 2, 6}));
						
			Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(9)));
			Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}),(ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
			Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
			Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
			Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
			Assert.AreEqual(new ScalarSet(new double[3] {11,2,6}), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
		}

		[Test]
		public void GetValues_TimeSpansToTimeStamp_01()
		{
			//-------------------------------------------------------------------------------------------------
			// Teting Getvalues when buffer contains TimeSpans and the requested value corresponds to a
			// TimeStamp. Three different relaxation factors are used.
			//-------------------------------------------------------------------------------------------------

			SmartBuffer smartBuffer = new SmartBuffer();

			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(10),new TimeStamp(13)), new ScalarSet(new double[3] { 5, 12,  5 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(16)), new ScalarSet(new double[3] { 7, 11,  5 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(16),new TimeStamp(20)), new ScalarSet(new double[3] { 9, 10,  5 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(20),new TimeStamp(27)), new ScalarSet(new double[3] { 2,  7,  5 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(27),new TimeStamp(30)), new ScalarSet(new double[3] { -5, 6,  5 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(30),new TimeStamp(48)), new ScalarSet(new double[3] { 7,  3,  5 }));

			smartBuffer.RelaxationFactor = 0.0;
			Assert.AreEqual(new ScalarSet(new double[3] { 1,  14,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(4)));
			Assert.AreEqual(new ScalarSet(new double[3] { 3,  13,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(7)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(15)));
			Assert.AreEqual(new ScalarSet(new double[3] { 9,  10,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(16)));
			Assert.AreEqual(new ScalarSet(new double[3] { 9,  10,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(17)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(20)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(21)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(23)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(27)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(28)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(29)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(30)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(33)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(36)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(42)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(48)));
			Assert.AreEqual(new ScalarSet(new double[3] {11,   2,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(54)));
			Assert.AreEqual(new ScalarSet(new double[3] {15,   1,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(60)));
	
			smartBuffer.RelaxationFactor = 1.0;

			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(4)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(7)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(15)));
			Assert.AreEqual(new ScalarSet(new double[3] { 9,  10,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(16)));
			Assert.AreEqual(new ScalarSet(new double[3] { 9,  10,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(17)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(20)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(21)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(23)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(27)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(28)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(29)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(30)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(33)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(36)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(42)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(48)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(54)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(60)));

			double a = 0.7;
			smartBuffer.RelaxationFactor = a;

			Assert.AreEqual(new ScalarSet(new double[3] { 5 - (1 - a)*(5 -1),  12 - (1-a)*(12-14),  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(4)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5 - (1 - a)*(5 - 3),  12 - (1 -a)* (12-13),  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(7)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(10)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(11)));
			Assert.AreEqual(new ScalarSet(new double[3] { 5,  12,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(12)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(13)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(14)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,  11,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(15)));
			Assert.AreEqual(new ScalarSet(new double[3] { 9,  10,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(16)));
			Assert.AreEqual(new ScalarSet(new double[3] { 9,  10,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(17)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(20)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(21)));
			Assert.AreEqual(new ScalarSet(new double[3] { 2,   7,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(23)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(27)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(28)));
			Assert.AreEqual(new ScalarSet(new double[3] {-5,   6,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(29)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(30)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(33)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(36)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(42)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7,   3,  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(48)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7+(1-a)*(11-7), 3+(1-a)*(2-3),  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(54)));
			Assert.AreEqual(new ScalarSet(new double[3] { 7+(1-a)*(15-7), 3+(1-a)*(1-3),  5 }), (ScalarSet) smartBuffer.GetValues(new TimeStamp(60)));
		}

		[Test]
		public void GetValues_TimeSpansToTimeSpans()
		{
			SmartBuffer smartBuffer = new SmartBuffer();

			smartBuffer.RelaxationFactor = 1.0;

			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(10),new TimeStamp(13)), new ScalarSet(new double[1] { 1 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(16)), new ScalarSet(new double[1] { 2 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(16),new TimeStamp(19)), new ScalarSet(new double[1] { 3 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(19),new TimeStamp(27)), new ScalarSet(new double[1] { 4 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(27),new TimeStamp(30)), new ScalarSet(new double[1] { 5 }));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(30),new TimeStamp(48)), new ScalarSet(new double[1] { 6 }));

			Assert.AreEqual( 1.0 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(9),new TimeStamp(13)))).GetScalar(0));
			Assert.AreEqual( 1.0 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(10),new TimeStamp(13)))).GetScalar(0));
			Assert.AreEqual( 1.0 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(11),new TimeStamp(12)))).GetScalar(0));
			Assert.AreEqual( 2.5 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(19)))).GetScalar(0));
			Assert.AreEqual( 2.5 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(14.5),new TimeStamp(17.5)))).GetScalar(0));

			smartBuffer.RelaxationFactor = 1.0;
			Assert.AreEqual( 1.0 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(10),new TimeStamp(13)))).GetScalar(0));
			Assert.AreEqual( 1.0 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(11),new TimeStamp(12)))).GetScalar(0));
			Assert.AreEqual( 2.5 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(19)))).GetScalar(0));
			Assert.AreEqual( 2.5 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(14.5),new TimeStamp(17.5)))).GetScalar(0));
		}




		[Test] 
		public void GetValues_TimeStampsToTimeSpan_01()
		{
			SmartBuffer smartBuffer = new SmartBuffer();
			smartBuffer.AddValues(new TimeStamp(2), new ScalarSet(new double[1]  {2}));
			smartBuffer.AddValues(new TimeStamp(4), new ScalarSet(new double[1]  {4}));
			smartBuffer.AddValues(new TimeStamp(7), new ScalarSet(new double[1]  {4}));
			smartBuffer.AddValues(new TimeStamp(11), new ScalarSet(new double[1] {6}));
			smartBuffer.AddValues(new TimeStamp(13), new ScalarSet(new double[1] {4}));
			smartBuffer.AddValues(new TimeStamp(15), new ScalarSet(new double[1] {3}));
			
			smartBuffer.RelaxationFactor = 1.0;
			
			Assert.AreEqual( 24.5/6 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(3),new TimeStamp(9)))).GetScalar(0),0.0000000001);
			Assert.AreEqual( 49.25/11 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(3),new TimeStamp(14)))).GetScalar(0),0.0000000001);
			Assert.AreEqual( 13.0/4.0 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(17)))).GetScalar(0));

			smartBuffer.RelaxationFactor = 0.0;
			Assert.AreEqual( 3 , ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(17)))).GetScalar(0));

			smartBuffer.RelaxationFactor = 1.0;
			Assert.AreEqual( 3, ((ScalarSet) smartBuffer.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(1),new TimeStamp(5)))).GetScalar(0));


			
		}

		[Test]
		public void AddValues_01()
		{
			try
			{
				SmartBuffer smartBuffer = new SmartBuffer();
				smartBuffer.DoExtendedDataVerification = true;
				
				ScalarSet scalarSet = new ScalarSet(new double[3] { 0, 1, 2 });
				TimeStamp timeStamp = new TimeStamp(1);

				smartBuffer.AddValues(timeStamp, scalarSet);

				timeStamp.ModifiedJulianDay = 2;
				scalarSet.data[0] = 10;
				scalarSet.data[1] = 11;
				scalarSet.data[2] = 12;

				smartBuffer.AddValues(timeStamp, scalarSet);

				smartBuffer.AddValues(new TimeStamp(3), new ScalarSet(new double[3] { 110, 111, 112 }));
				smartBuffer.AddValues(new TimeStamp(4), new ScalarSet(new double[3] { 1110, 1111, 1112 }));

				Assert.AreEqual(0,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
				Assert.AreEqual(1,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(1));
				Assert.AreEqual(2,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(2));

				Assert.AreEqual(10,((IScalarSet) smartBuffer.GetValuesAt(1)).GetScalar(0));
				Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(1)).GetScalar(1));
				Assert.AreEqual(12,((IScalarSet) smartBuffer.GetValuesAt(1)).GetScalar(2));

				Assert.AreEqual(110,((IScalarSet) smartBuffer.GetValuesAt(2)).GetScalar(0));
				Assert.AreEqual(111,((IScalarSet) smartBuffer.GetValuesAt(2)).GetScalar(1));
				Assert.AreEqual(112,((IScalarSet) smartBuffer.GetValuesAt(2)).GetScalar(2));

				Assert.AreEqual(1110,((IScalarSet) smartBuffer.GetValuesAt(3)).GetScalar(0));
				Assert.AreEqual(1111,((IScalarSet) smartBuffer.GetValuesAt(3)).GetScalar(1));
				Assert.AreEqual(1112,((IScalarSet) smartBuffer.GetValuesAt(3)).GetScalar(2));

				Assert.AreEqual(4,smartBuffer.TimesCount);
				Assert.AreEqual(3,smartBuffer.ValuesCount);
			}
			catch (System.Exception e)
			{
				WriteException(e);
				throw (e);
			}
		}

		[Test]
		public void ClearAfter()
		{
			SmartBuffer smartBuffer = new SmartBuffer();
			smartBuffer.DoExtendedDataVerification = true;

			// --Populate the SmartBuffer --
			smartBuffer.AddValues(new TimeStamp(10), new ScalarSet(new double[2] {11, 21}));
			smartBuffer.AddValues(new TimeStamp(13), new ScalarSet(new double[2] {12 ,22}));
			smartBuffer.AddValues(new TimeStamp(16), new ScalarSet(new double[2] {13 ,23}));
			smartBuffer.AddValues(new TimeStamp(20), new ScalarSet(new double[2] {14 ,24}));
			smartBuffer.AddValues(new TimeStamp(27), new ScalarSet(new double[2] {15 ,25}));
			smartBuffer.AddValues(new TimeStamp(30), new ScalarSet(new double[2] {16, 26}));
			smartBuffer.AddValues(new TimeStamp(48), new ScalarSet(new double[2] {17, 27}));

            Oatc.OpenMI.Sdk.Backbone.TimeStamp time = new Oatc.OpenMI.Sdk.Backbone.TimeStamp();

			time.ModifiedJulianDay = 50; // this time is after the last time in buffer
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(7,smartBuffer.TimesCount);  //nothing removed
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(27,((IScalarSet) smartBuffer.GetValuesAt(6)).GetScalar(1));

			time.ModifiedJulianDay = 30;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(5,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(25,((IScalarSet) smartBuffer.GetValuesAt(4)).GetScalar(1));

			time.ModifiedJulianDay = 16.5;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(3,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(23,((IScalarSet) smartBuffer.GetValuesAt(2)).GetScalar(1));

			time.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(0,smartBuffer.TimesCount);

			time.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(0,smartBuffer.TimesCount);

			smartBuffer.AddValues(new TimeStamp(10), new ScalarSet(new double[2] {11, 21}));
			smartBuffer.AddValues(new TimeStamp(13), new ScalarSet(new double[2] {12 ,22}));
			smartBuffer.AddValues(new TimeStamp(16), new ScalarSet(new double[2] {13 ,23}));
			smartBuffer.AddValues(new TimeStamp(20), new ScalarSet(new double[2] {14 ,24}));
			smartBuffer.AddValues(new TimeStamp(27), new ScalarSet(new double[2] {15 ,25}));
			smartBuffer.AddValues(new TimeStamp(30), new ScalarSet(new double[2] {16, 26}));
			smartBuffer.AddValues(new TimeStamp(48), new ScalarSet(new double[2] {17, 27}));

			Oatc.OpenMI.Sdk.Backbone.TimeStamp start   = new Oatc.OpenMI.Sdk.Backbone.TimeStamp(50);
			Oatc.OpenMI.Sdk.Backbone.TimeStamp end     = new Oatc.OpenMI.Sdk.Backbone.TimeStamp(55);
			Oatc.OpenMI.Sdk.Backbone.TimeSpan timeSpan = new Oatc.OpenMI.Sdk.Backbone.TimeSpan(start,end);

			start.ModifiedJulianDay = 50; // this time is after the last time in buffer
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(7,smartBuffer.TimesCount);  //nothing removed
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(27,((IScalarSet) smartBuffer.GetValuesAt(6)).GetScalar(1));

			start.ModifiedJulianDay = 30;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(5,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(25,((IScalarSet) smartBuffer.GetValuesAt(4)).GetScalar(1));

			start.ModifiedJulianDay = 16.5;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(3,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(23,((IScalarSet) smartBuffer.GetValuesAt(2)).GetScalar(1));

			start.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(0,smartBuffer.TimesCount);

			start.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(0,smartBuffer.TimesCount);

			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(10),new TimeStamp(13)), new ScalarSet(new double[2] {11, 21}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(16)), new ScalarSet(new double[2] {12, 22}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(16),new TimeStamp(20)), new ScalarSet(new double[2] {13, 23}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(20),new TimeStamp(27)), new ScalarSet(new double[2] {14, 24}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(27),new TimeStamp(30)), new ScalarSet(new double[2] {15, 25}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(30),new TimeStamp(48)), new ScalarSet(new double[2] {16, 26}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(48),new TimeStamp(55)), new ScalarSet(new double[2] {17, 27}));

			
			time.ModifiedJulianDay = 50; // this time is after the last time in buffer
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(7,smartBuffer.TimesCount);  //nothing removed
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(27,((IScalarSet) smartBuffer.GetValuesAt(6)).GetScalar(1));

			time.ModifiedJulianDay = 30;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(5,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(25,((IScalarSet) smartBuffer.GetValuesAt(4)).GetScalar(1));

			time.ModifiedJulianDay = 16.5;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(3,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(23,((IScalarSet) smartBuffer.GetValuesAt(2)).GetScalar(1));

			time.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(0,smartBuffer.TimesCount);

			time.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(time);
			Assert.AreEqual(0,smartBuffer.TimesCount);

			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(10),new TimeStamp(13)), new ScalarSet(new double[2] {11, 21}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(13),new TimeStamp(16)), new ScalarSet(new double[2] {12, 22}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(16),new TimeStamp(20)), new ScalarSet(new double[2] {13, 23}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(20),new TimeStamp(27)), new ScalarSet(new double[2] {14, 24}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(27),new TimeStamp(30)), new ScalarSet(new double[2] {15, 25}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(30),new TimeStamp(48)), new ScalarSet(new double[2] {16, 26}));
			smartBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(48),new TimeStamp(55)), new ScalarSet(new double[2] {17, 27}));

			start.ModifiedJulianDay = 50; // this time is after the last time in buffer
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(7,smartBuffer.TimesCount);  //nothing removed
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(27,((IScalarSet) smartBuffer.GetValuesAt(6)).GetScalar(1));

			start.ModifiedJulianDay = 30;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(5,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(25,((IScalarSet) smartBuffer.GetValuesAt(4)).GetScalar(1));

			start.ModifiedJulianDay = 16.5;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(3,smartBuffer.TimesCount);
			Assert.AreEqual(11,((IScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(23,((IScalarSet) smartBuffer.GetValuesAt(2)).GetScalar(1));

			start.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(0,smartBuffer.TimesCount);

			start.ModifiedJulianDay = 9;
			smartBuffer.ClearAfter(timeSpan);
			Assert.AreEqual(0,smartBuffer.TimesCount);
		}

		[Test]
		public void ClearBefore()
		{
			SmartBuffer smartBuffer = new SmartBuffer();
			smartBuffer.AddValues(new TimeStamp(1), new ScalarSet(new double[3] {  1.1,  2.1,  3.1 }));
			smartBuffer.AddValues(new TimeStamp(3), new ScalarSet(new double[3] {  4.1,  5.1,  6.1 }));
			smartBuffer.AddValues(new TimeStamp(4), new ScalarSet(new double[3] {  7.1,  8.1,  9.1 }));
			smartBuffer.AddValues(new TimeStamp(5), new ScalarSet(new double[3] { 10.1, 11.1, 12.1 }));

			smartBuffer.ClearBefore(new TimeStamp(0.5));
			Assert.AreEqual(4,smartBuffer.TimesCount);
			smartBuffer.CheckBuffer();

			smartBuffer.ClearBefore(new TimeStamp(1));
			Assert.AreEqual(4,smartBuffer.TimesCount);
			smartBuffer.CheckBuffer();

			smartBuffer.ClearBefore(new TimeStamp(1.1));
			Assert.AreEqual(4,smartBuffer.TimesCount);
			Assert.AreEqual(1.1,((ScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(1, ((ITimeStamp) smartBuffer.GetTimeAt(0)).ModifiedJulianDay);
			smartBuffer.CheckBuffer();

			smartBuffer.ClearBefore(new TimeStamp(4.1));
			Assert.AreEqual(2, smartBuffer.TimesCount);
			Assert.AreEqual(7.1,((ScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(4, ((ITimeStamp) smartBuffer.GetTimeAt(0)).ModifiedJulianDay);
			smartBuffer.CheckBuffer();

			smartBuffer.ClearBefore(new TimeStamp(5.1));
			Assert.AreEqual(1, smartBuffer.TimesCount);
			Assert.AreEqual(10.1,((ScalarSet) smartBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(5, ((ITimeStamp) smartBuffer.GetTimeAt(0)).ModifiedJulianDay);
			smartBuffer.CheckBuffer();

			SmartBuffer timeSpanBuffer = new SmartBuffer();
			timeSpanBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(1),new TimeStamp(3)), new ScalarSet(new double[3] { 1.1,  2.1,   3.1 }));
			timeSpanBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(3),new TimeStamp(5)), new ScalarSet(new double[3] { 4.1,  5.1,   6.1 })); 
			timeSpanBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(5),new TimeStamp(7)), new ScalarSet(new double[3] { 7.1,  8.1,   9.1 })); 
			timeSpanBuffer.AddValues(new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(7),new TimeStamp(9)), new ScalarSet(new double[3] { 10.1, 11.1, 12.1 })); 
			
			timeSpanBuffer.ClearBefore(new TimeStamp(0.5));
			Assert.AreEqual(4,timeSpanBuffer.TimesCount);
			timeSpanBuffer.CheckBuffer();

			timeSpanBuffer.ClearBefore(new TimeStamp(1.0));
			Assert.AreEqual(4,timeSpanBuffer.TimesCount);
			timeSpanBuffer.CheckBuffer();

			timeSpanBuffer.ClearBefore(new TimeStamp(2.0));
			Assert.AreEqual(4,timeSpanBuffer.TimesCount);
			timeSpanBuffer.CheckBuffer();

			timeSpanBuffer.ClearBefore(new TimeStamp(3.0));
			Assert.AreEqual(4,timeSpanBuffer.TimesCount);
			timeSpanBuffer.CheckBuffer();

			timeSpanBuffer.ClearBefore(new TimeStamp(4.0));
			Assert.AreEqual(4,timeSpanBuffer.TimesCount);
			Assert.AreEqual(1.1,((ScalarSet) timeSpanBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(1, ((ITimeSpan) timeSpanBuffer.GetTimeAt(0)).Start.ModifiedJulianDay);
			Assert.AreEqual(3, ((ITimeSpan) timeSpanBuffer.GetTimeAt(0)).End.ModifiedJulianDay);
			timeSpanBuffer.CheckBuffer();

			timeSpanBuffer.ClearBefore(new TimeStamp(7.0));
			Assert.AreEqual(3,timeSpanBuffer.TimesCount);
			Assert.AreEqual(4.1,((ScalarSet) timeSpanBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(3, ((ITimeSpan) timeSpanBuffer.GetTimeAt(0)).Start.ModifiedJulianDay);
			Assert.AreEqual(5, ((ITimeSpan) timeSpanBuffer.GetTimeAt(0)).End.ModifiedJulianDay);
			timeSpanBuffer.CheckBuffer();

			timeSpanBuffer.ClearBefore(new TimeStamp(10.0));
			Assert.AreEqual(1,timeSpanBuffer.TimesCount);
			Assert.AreEqual(10.1,((ScalarSet) timeSpanBuffer.GetValuesAt(0)).GetScalar(0));
			Assert.AreEqual(7, ((ITimeSpan) timeSpanBuffer.GetTimeAt(0)).Start.ModifiedJulianDay);
			Assert.AreEqual(9, ((ITimeSpan) timeSpanBuffer.GetTimeAt(0)).End.ModifiedJulianDay);
			timeSpanBuffer.CheckBuffer();
		}

		
		private void WriteException(System.Exception e)
		{
			Console.WriteLine(" ");
			Console.WriteLine("------- System.Exception ----------------------------- ");
			Console.WriteLine("Catched in....: org.OpenMITest.Utilities.Wrapper.GetValues_River_Trigger_IDBased()");
			Console.WriteLine("Message.......: " + e.Message);
			Console.WriteLine("Stact trace...: " + e.StackTrace);
			Console.WriteLine("TargetSite....: " + e.TargetSite.Name);
			Console.WriteLine("Source........: " + e.Source);
			Console.WriteLine(" ");
		}
    }
}
