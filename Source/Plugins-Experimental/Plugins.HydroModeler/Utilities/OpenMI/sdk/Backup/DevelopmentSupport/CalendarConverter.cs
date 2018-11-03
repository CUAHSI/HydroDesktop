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

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
	/// <summary>
	/// Support class for conversions between DateTime objects and Modified Julian Dates
	/// Modified Julian Date is the number of days since November 17, 1858.
	/// </summary>
	public class CalendarConverter
	{
		private static DateTime _ModifiedJulianDateZero = new DateTime (1858, 11, 17);
		private static long _ModifiedJulianDateZeroTicks = new DateTime (1858, 11, 17).Ticks;

		/// <summary>
		/// Converts a DateTime object to modified julian date
		/// </summary>
		/// <param name="gregorianDate">DateTime object</param>
		/// <returns>Modified Julian Date (days since November 17, 1858)</returns>
		public static double Gregorian2ModifiedJulian(DateTime gregorianDate)
		{
			long ticks = gregorianDate.Ticks - _ModifiedJulianDateZeroTicks;
			double result = ((double) ticks) / ((double) TimeSpan.TicksPerDay);
			return result;
		}

		/// <summary>
		/// Converts a modified julian date to a DateTime object
		/// </summary>
		/// <param name="modifiedJulianDate">Modified Julian Date (days since November 17, 1858)</param>
		/// <returns>DateTime object</returns>
		public static DateTime ModifiedJulian2Gregorian(double modifiedJulianDate)
		{
			return _ModifiedJulianDateZero.AddDays (modifiedJulianDate);
		}
	}
}
