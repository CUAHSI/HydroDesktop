using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Search
{
	internal class TimeSeriesDownloadResults
	{
		#region Constructors

		public TimeSeriesDownloadResults ()
		{
			ThemeName = "";
			WarningMessage = "";
		}

		#endregion

		#region Public Properties

		public string ThemeName { get; set; }
		public string WarningMessage { get; set; }

		#endregion
	}
}
