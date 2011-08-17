using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Data.SQLite;

namespace HydroDesktop.Database
{
    /// <summary>
    /// This class contains methods for working with the
    /// SQLite database
    /// </summary>
    public static class SQLiteHelper
    {
        /// <summary>
        /// To get the SQLite database path given the SQLite connection string
        /// </summary>
        public static string GetSQLiteFileName(string sqliteConnString)
        {
            //string startPart = "Data Source= ";
            //string endPart = ";New=False;";
            //int startIndex = startPart.Length;
            //int endIndex = sqliteConnString.IndexOf(endPart);
            //string dbFileName = sqliteConnString.Substring(startIndex - 1, endIndex - startIndex + 1);
            //return dbFileName;
            SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder(sqliteConnString);
            return conn.DataSource;
        }
        /// <summary>
        /// To get the full SQLite connection string given the SQLite database path
        /// </summary>
        public static string GetSQLiteConnectionString(string dbFileName)
        {
          
         // return "Data Source= " + dbFileName + ";New=False;Compress=True;Version=3";
            SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
            conn.DataSource = dbFileName;
            conn.Version = 3;
            conn.FailIfMissing = true;
            conn.Add("Compress", true);

            return conn.ConnectionString;

        }

        /// <summary>
        /// Create the default .SQLITE database in the user-specified path
        /// </summary>
        /// <returns>true if database was created, false otherwise</returns>
        public static Boolean CreateSQLiteDatabase(string dbPath)
        {
            Assembly asm = Assembly.GetAssembly(typeof(HydroDesktop.Database.DbOperations));

            //to create the default.sqlite database file
            try
            {
                using (Stream input = asm.GetManifestResourceStream("HydroDesktop.Resources.defaultDatabase.sqlite"))
                {
                    using (Stream output = File.Create(dbPath))
                    {
                        CopyStream(input, output);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("Error creating the default database " + dbPath +
                    ". Please check the write permissions for HydroDesktop. " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error creating the default database " + dbPath +
                    ". Error details: " + ex.Message);
                return false;
            }
            if (File.Exists(dbPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Create the default empty MetadataCache.SQLITE database in the user-specified path
        /// The created database has the correct db schema.
        /// </summary>
        /// <returns>true if database was created, false otherwise</returns>
        public static Boolean CreateMetadataCacheDb(string dbPath)
        {
            Assembly asm = Assembly.GetAssembly(typeof(HydroDesktop.Database.DbOperations));

            //to create the default.sqlite database file
            try
            {
                using (Stream input = asm.GetManifestResourceStream("HydroDesktop.Resources.MetadataCache.sqlite"))
                {
                    using (Stream output = File.Create(dbPath))
                    {
                        CopyStream(input, output);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("Error creating the metadata cache database " + dbPath +
                    ". Please check the write permissions for HydroDesktop. " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error creating the metadata cache database " + dbPath +
                    ". Error details: " + ex.Message);
                return false;
            }
            if (File.Exists(dbPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Check if the path is a valid SQLite database
        /// This function returns false, if the SQLite db
        /// file doesn't exist or if the file size is 0 Bytes
        /// </summary>
        public static bool DatabaseExists(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                return false;
            }
            else
            {
                FileInfo dbFileInfo = new FileInfo(dbPath);
                if (dbFileInfo.Length == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
