using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace HydroDesktop.Main
{
    /// <summary>
    /// This class writes the "Trace" to a log file
    /// </summary>
    public class TraceLogger
    {
        private const string logFileName = "hydrodesktop_log.txt";

        public void CreateTraceFile()
        {
            //first try to create it in application startup path
            string programFilesPath = Application.StartupPath;
            string documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HydroDesktop");
            string tempPath = Path.Combine(Path.GetTempPath(), "HydroDesktop");

            Stream logStream = null;

            logStream = TryToCreateLogFile(programFilesPath);

            if (logStream == null)
                logStream = TryToCreateLogFile(documentsPath);

            if (logStream == null)
                logStream = TryToCreateLogFile(tempPath);

            //create the trace listener
            if (logStream != null && File.Exists(logFileName))
            {
                var myTextListener = new TextWriterTraceListener(logStream);
                Trace.Listeners.Add(myTextListener);
            }      
        }

        private Stream TryToCreateLogFile(string logFileDirectory)
        {
           
            if (!Directory.Exists(logFileDirectory))
            {
                try
                {
                    Directory.CreateDirectory(logFileDirectory);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            if (!Directory.Exists(logFileDirectory))
            {
                return null;
            }
            
            //at this point the directory exists
            string fullPath = Path.Combine(logFileDirectory, logFileName);
            try
            {
                Stream logFileStream = File.Create(fullPath);
                return logFileStream;

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
