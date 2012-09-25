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
            var programFilesPath = Application.StartupPath;
            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HydroDesktop");
            var tempPath = Path.Combine(Path.GetTempPath(), "HydroDesktop");

            var stream = TryToCreateLogFile(programFilesPath);

            if (stream == null)
                stream = TryToCreateLogFile(documentsPath);

            if (stream == null)
                stream = TryToCreateLogFile(tempPath);

            //create the trace listener
            if (stream != null)
            {
                var myTextListener = new TextWriterTraceListener(stream);
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
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to create directory {0}: {1}", logFileDirectory, ex.Message);
                    return null;
                }
            }
            if (!Directory.Exists(logFileDirectory))
            {
                return null;
            }
            
            //at this point the directory exists
            var fullPath = Path.Combine(logFileDirectory, logFileName);
            try
            {
                // Add to existing log file or create new
                return new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.Read);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unable to create log file {0}: {1}", fullPath, ex.Message);
                return null;
            }
        }
    }
}
