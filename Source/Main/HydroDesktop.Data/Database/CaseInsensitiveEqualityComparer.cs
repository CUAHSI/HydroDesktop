using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HydroDesktop.Database
{
	/// <summary>
	/// equality comparer (case insensitive)
	/// </summary>
    public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
	{
		private CaseInsensitiveComparer myComparer;
        /// <summary>
        /// creates a new instance of the comparer
        /// </summary>
		public CaseInsensitiveEqualityComparer ()
		{
			myComparer = CaseInsensitiveComparer.DefaultInvariant;
		}
        /// <summary>
        /// creates a culture specific instance of the comparer
        /// </summary>
        /// <param name="myCulture">the CultureInfo parameter</param>
		public CaseInsensitiveEqualityComparer ( CultureInfo myCulture )
		{
			myComparer = new CaseInsensitiveComparer ( myCulture );
		}
        /// <summary>
        /// alphabetic equality comparison of two strings
        /// </summary>
        /// <param name="x">string x</param>
        /// <param name="y">string y</param>
        /// <returns>true if the lowercase x and lowercase y are equal</returns>
		public bool Equals ( string x, string y )
		{
			if ( myComparer.Compare ( x, y ) == 0 )
			{
				return true;
			}
			else
			{
				return false;
			}
		}
        /// <summary>
        /// hash code of the equality comparer
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>true if lowercase strings have identical hash codes</returns>
		public int GetHashCode ( string obj )
		{
			return obj.ToString ().ToLower ().GetHashCode ();
		}
	}
}
