using System;

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
            return obj != DBNull.Value ? (string) obj : string.Empty;
        }

        /// <summary>
        /// Convert object in data reader result to Integer
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// Integer if the value is a valid Integer number</returns>
        public static int ReadInteger(object obj)
        {
            return obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
        }

        /// <summary>
        /// Convert object in data reader result to Double
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// Double if the value in the DB is a valid Double number</returns>
        public static double ReadDouble(object obj)
        {
            return obj != DBNull.Value ? Convert.ToDouble(obj) : 0.0;
        }

        /// <summary>
        /// Convert object in data reader result to boolean
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// a boolean value if the value in the DB is a valid boolean value</returns>
        public static bool ReadBoolean(object obj)
        {
            return obj != DBNull.Value && Convert.ToBoolean(obj);
        }

        /// <summary>
        /// Convert object in data reader result to DateTime
        /// </summary>
        /// <param name="obj">the value in the DB</param>
        /// <returns>Null if the value in DB is NULL, 
        /// DateTime if the value in the DB is a valid date time</returns>
        public static DateTime ReadDateTime(object obj)
        {
            return obj != DBNull.Value ? Convert.ToDateTime(obj) : DateTime.MinValue;
        }
    }
}
