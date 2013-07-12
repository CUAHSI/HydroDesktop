using System;
using System.IO;

namespace HydroDesktop.Configuration
{
   /// <summary>
   /// Helper methods for finding and creating directories
   /// </summary>
    static class ConfigurationHelper
    {
        public static string FindOrCreateAppDataDirectory()
        {
            string baseAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            //check if this directory can be created             
            string hdAppData = Path.Combine(baseAppData, Properties.Settings.Default.AppDataDirectoryName);
            CheckDirectory(hdAppData);
            return hdAppData;
        }

        public static string FindOrCreateTempDirectory()
        {
            string basePath = Path.GetTempPath();
            //check if this directory can be created             
            string hdTempDir = Path.Combine(basePath, Properties.Settings.Default.TempDirectoryName);
            CheckDirectory(hdTempDir);
            return hdTempDir;
        }

        private static void CheckDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                try
                {
                    Directory.CreateDirectory(directoryName);
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException("Error creating directory " +
                        directoryName + ". " + ex.Message);
                }
            }
        }
    }
}
