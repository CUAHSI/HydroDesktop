#region Copyright
/*
    Copyright (c) 2005,2006,2007, OpenMI Association
    "http://www.openmi.org/"

    This file is part of OpenMI.Standard.dll

    OpenMI.Standard.dll is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    OpenMI.Standard.dll is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

namespace OpenMI.Standard
{
	/// <summary>
	/// Enumeration for base dimensions
	/// </summary>

	public enum DimensionBase : int
	{
		/// <summary>
		/// Base dimension length.
		/// </summary>
		
		Length						= 0,

		/// <summary>
		/// Base dimension mass.
		/// </summary>
		
		Mass						= 1,


		/// <summary>
		/// Base dimension time.
		/// </summary>
		
		Time						= 2,


		/// <summary>
		/// Base dimension electric current.
		/// </summary>
		
		ElectricCurrent				= 3,
		

		/// <summary>
		/// Base dimension temperature.
		/// </summary>
		
		Temperature					= 4,
		

		/// <summary>
		/// Base dimension amount of substance.
		/// </summary>
		
		AmountOfSubstance			= 5,
		

		/// <summary>
		/// Base dimension luminous intensity.
		/// </summary>
		
		LuminousIntensity			= 6,
		

		/// <summary>
		/// Base dimension currency.
		/// </summary>
		
		Currency					= 7,

		/// <summary>
		/// Total number of base dimensions.
		/// </summary>
		
		NUM_BASE_DIMENSIONS

	}

	/// <summary>
	/// Dimension interface
	/// </summary>

	public interface IDimension
	{

		/// <summary>
        /// <para>Returns the power for the requested dimension</para>
        /// 
        /// <para>EXAMPLE:</para>
        /// <para>For a quantity such as flow, which may have the unit m3/s, the GetPower method must
        /// work as follows:</para>
        /// 
        /// <para>myDimension.GetPower(DimensionBase.AmountOfSubstance) -->returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Currency) --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.ElectricCurrent) --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Length) --> returns 3</para>
        /// <para>myDimension.GetPower(DimensionBase.LuminousIntensity) --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Mass) --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Temperature) --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Time) --> returns -1</para>
        /// </summary>
		double GetPower(DimensionBase baseQuantity);


		/// <summary>
		/// Check if a Dimension instance equals to another Dimension instance.
		/// </summary>
		/// <param name="otherDimension">Dimension instance to compare with.</param>
		/// <returns>True if the dimensions are equal.</returns>
    	bool Equals(IDimension otherDimension);
	
	}
}

