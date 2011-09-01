using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace HydroDesktop.Database
{
    public static class DataReader
    {
        public static string ReadString(object obj)
        {
            if (obj != DBNull.Value)
            {
                return (string)obj;
            }
            return string.Empty;
        }

        public static int ReadInteger(object obj)
        {
            if (obj != DBNull.Value)
            {
                return Convert.ToInt32(obj);
            }
            else
            {
                return 0;
            }
        }

        public static double ReadDouble(object obj)
        {
            if (obj != DBNull.Value)
            {
                return Convert.ToDouble(obj);
            }
            else
            {
                return 0.0;
            }
        }

        public static bool ReadBoolean(object obj)
        {
            if (obj != DBNull.Value)
            {
                return Convert.ToBoolean(obj);
            }
            else
            {
                return false;
            }
        }

        public static DateTime ReadDateTime(object obj)
        {
            if (obj != DBNull.Value)
            {
                return Convert.ToDateTime(obj);
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
}
