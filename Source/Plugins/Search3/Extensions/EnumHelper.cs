using System;
using System.ComponentModel;

namespace Search3.Extensions
{
    static class EnumHelper
    {
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