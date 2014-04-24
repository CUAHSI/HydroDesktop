using System;
using System.Globalization;

namespace HydroDesktop.WebServices.WaterML
{
    /// <summary>
    /// Contains helpers for parsing
    /// </summary>
    internal static class ParserHelper
    {
        /// <summary>
        /// Converts the 'UTC Offset' value to a double digit in hours
        /// </summary>
        public static double ConvertUtcOffset(string offsetString)
        {
            var colonIndex = offsetString.IndexOf(":", StringComparison.Ordinal);
            var minutes = 0.0;
            var hours = 0.0;
            if (colonIndex > 0 && colonIndex < offsetString.Length - 1)
            {
                minutes = Convert.ToDouble(offsetString.Substring(colonIndex + 1), CultureInfo.InvariantCulture);
                hours = Convert.ToDouble((offsetString.Substring(0, colonIndex)), CultureInfo.InvariantCulture);
            }

            return hours + Math.Sign(hours)*(minutes / 60.0);
        }   
    }
}