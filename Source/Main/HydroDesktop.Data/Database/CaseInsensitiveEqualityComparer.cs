using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HydroDesktop.Database
{
	public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
	{
		private CaseInsensitiveComparer myComparer;

		public CaseInsensitiveEqualityComparer ()
		{
			myComparer = CaseInsensitiveComparer.DefaultInvariant;
		}

		public CaseInsensitiveEqualityComparer ( CultureInfo myCulture )
		{
			myComparer = new CaseInsensitiveComparer ( myCulture );
		}

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

		public int GetHashCode ( string obj )
		{
			return obj.ToString ().ToLower ().GetHashCode ();
		}
	}
}
