using System;
using System.ComponentModel;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Helper for enums
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Get description from Enum Value
        /// </summary>
        /// <param name="enumValue">Enum value.</param>
        /// <returns>Description of value.</returns>
        public static string Description(this Enum enumValue)
        {
            var descriptions =
                (DescriptionAttribute[])enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptions.Length == 0)
            {
                return Enum.GetName(enumValue.GetType(), enumValue);
            }
            return descriptions[0].Description;
        }
    }
}