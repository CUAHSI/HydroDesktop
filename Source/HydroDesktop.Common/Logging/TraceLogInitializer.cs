using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace HydroDesktop.Common.Logging
{
    class TraceLogInitializer : ILogInitializer
    {
        private const string LOG_FILE_NAME = "trace{0}.log";

        public TraceLogInitializer()
        {
            Destination = CreateTraceFile();
        }

        #region Implementation of ILogInitializer

        public string Destination { get; private set; }

        #endregion

        private static string CreateTraceFile()
        {
            //first try to create it in application startup path
            var programFilesPath = Application.StartupPath;
            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                             "HydroDesktop");
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
                return stream.Name;
            }
            return null;
        }

        private static FileStream TryToCreateLogFile(string logFileDirectory)
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
            var logFileName = string.Format(LOG_FILE_NAME, string.Empty);
            var fullPath = Path.Combine(logFileDirectory, logFileName);
            try
            {
                // Rename previous file to LOG_FILE_NAME_yyyyMMdd.log
                if (File.Exists(fullPath))
                {
                    var lastTime = File.GetLastWriteTime(fullPath);
                    if (lastTime.Date != DateTime.Now.Date)
                    {
                        File.Move(fullPath, Path.Combine(logFileDirectory, string.Format(LOG_FILE_NAME, "_" + lastTime.Date.ToString("yyyyMMdd"))));   
                    }
                }

                // Add to existing log file or create new
                return new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unable to create log file {0}: {1}", fullPath, ex.Message);
                return null;
            }
        }

        public void Dispose()
        {
            Trace.Flush();
        }
    }
}