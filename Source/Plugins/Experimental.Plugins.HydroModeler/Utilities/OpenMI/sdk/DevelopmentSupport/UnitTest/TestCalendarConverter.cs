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
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// TemporalTester tests (gregorian) DateTime to
	/// Modified Julian Date(/Time) and vice versa.
	/// </summary>
	[TestFixture]
	public class TestCalendarConverter
	{
		public TestCalendarConverter()
		{
		}

		[Test] public void TestDates()
		{
			Evaluate(new DateTime(1985,1,1,1,0,0,0));
			Evaluate(new DateTime(1980,11,29,23,59,59,999));
			Evaluate(new DateTime(1980,11,30,00,00,00,000));

			DateTime inDateTime_1 = new DateTime(1980,11,30,23,59,59,999);
			DateTime inDateTime_2 = inDateTime_1.AddSeconds(1);

			Evaluate(inDateTime_1);
			Evaluate(inDateTime_2);
			Evaluate(new DateTime(1981,1,28,23,59,59,999));
		}

		[Test] public void TestYears()
		{
			int nItems = 2000;
			DateTime gregDate = new DateTime(1111,12,15,1,0,0,0);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddYears(1);
				Evaluate(gregDate);
			}
		}

		[Test] public void TestMonths()
		{
			int nItems = 200;
			DateTime gregDate = new DateTime(1998,11,30,23,59,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddMonths(1);
				Evaluate(gregDate);
			}
		}

		[Test] public void TestDays()
		{
			int nItems = 1000;
			DateTime gregDate = new DateTime(1999,11,30,23,59,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddDays(1);
				Evaluate(gregDate);
			}
		}

		[Test] public void TestHours()
		{
			int nItems = 500;
			DateTime gregDate = new DateTime(1999,12,25,23,59,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddHours(1);
				Evaluate (gregDate);
			}
		}

		[Test] public void TestMinutes()
		{
			int nItems = 500;
			DateTime gregDate = new DateTime(1999,12,31,21,00,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddMinutes(1);
				Evaluate(gregDate);
			}
		}

		[Test] public void TestSeconds()
		{
			int nItems = 1000;
			DateTime gregDate = new DateTime(1999,12,31,23,55,00,499);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddSeconds(1);
				Evaluate(gregDate);
			}
		}

		[Test] public void TestMilliSeconds()
		{
			int nItems = 5000;
			DateTime gregDate = new DateTime(1999,12,31,23,59,58,350);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddMilliseconds(1);
				Evaluate(gregDate);
			}
		}

		[Test] public void SomeDates() 
		{
			double zero = 0;
			DateTime zeroDate = CalendarConverter.ModifiedJulian2Gregorian (zero);

			Assert.AreEqual (1858, zeroDate.Year, "Year of Modified Julian Date Zero");
			Assert.AreEqual (11, zeroDate.Month, "Month of Modified Julian Date Zero");
			Assert.AreEqual (17, zeroDate.Day, "Day of Modified Julian Date Zero");

			double jan1_1985 = 46066.25;
			DateTime jan1_1985Date = CalendarConverter.ModifiedJulian2Gregorian (jan1_1985);

			Assert.AreEqual (1985, jan1_1985Date.Year, "Year of jan 1 1985");
			Assert.AreEqual (1, jan1_1985Date.Month, "Month of jan 1 1985");
			Assert.AreEqual (1, jan1_1985Date.Day, "Day of jan 1 1985");
			Assert.AreEqual (6, jan1_1985Date.Hour, "Hour of jan 1 1985");
		}

		[Test] public void TrickyDates() 
		{
			double julianDate = 46096.999999998196;
			DateTime gregorian = CalendarConverter.ModifiedJulian2Gregorian (julianDate);

			Assert.AreEqual (1985, gregorian.Year, "Year expected");
			Assert.AreEqual (2, gregorian.Month, "Month expected");
			Assert.AreEqual (1, gregorian.Day, "Day expected");
		}

		private void Evaluate(DateTime inGregDate)
		{
			double modJulDate = CalendarConverter.Gregorian2ModifiedJulian(inGregDate);
			long mjdInt = (long) modJulDate;

			DateTime outGregDate = CalendarConverter.ModifiedJulian2Gregorian(modJulDate);

			Assert.AreEqual (inGregDate.ToString(), outGregDate.ToString(), modJulDate.ToString());
		}
	}
}
