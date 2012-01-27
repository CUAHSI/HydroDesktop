using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Auxiliary data reader class
    /// </summary>
    static class DataReader
    {
        /// <summary>
        /// Convert object in data reader result to null or string
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, string if the value is a valid string</returns>
        public static string ReadString(object obj)
        {
            if (obj != DBNull.Value)
            {
                return (string)obj;
            }
            return string.Empty;
        }
        /// <summary>
        /// Convert object in data reader result to Integer
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// Integer if the value is a valid Integer number</returns>
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
        /// <summary>
        /// Convert object in data reader result to Double
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// Double if the value in the DB is a valid Double number</returns>
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
        /// <summary>
        /// Convert object in data reader result to boolean
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// a boolean value if the value in the DB is a valid boolean value</returns>
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
        /// <summary>
        /// Convert object in data reader result to DateTime
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// DateTime if the value in the DB is a valid date time</returns>
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
