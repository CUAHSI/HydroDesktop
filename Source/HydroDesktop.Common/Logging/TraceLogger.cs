using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HydroDesktop.Common.Logging
{
    /// <summary>
    /// Simple logger, which uses Trace class.
    /// </summary>
    class TraceLogger : ILog, IDisposable
    {
        private const string LOG_FILE_NAME = "hydrodesktop_log.txt";

        public TraceLogger()
        {
            Destination = CreateTraceFile();
        }

        public void Info(string message)
        {
            LogMessage(message, "INFO");
        }

        public void Warn(string message)
        {
            LogMessage(message, "WARN");
        }

        public void Error(string message, Exception exception = null)
        {
            LogMessage(message, "ERROR", exception);
        }

        public string Destination { get; private set; }

        private void LogMessage(string message, string category, Exception exception = null)
        {
            if (!IsEnabled()) return;

            var sb = new StringBuilder();
            sb.AppendFormat("[{0}] [{1}] [{2}]", DateTime.Now, category, GetCallingMethod());
            sb.AppendLine();
            sb.Append(message);
            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine(exception.Message);
                sb.AppendLine("Inner: " + (exception.InnerException != null? exception.InnerException.ToString() : "NULL"));
                sb.Append("Stack trace: " + exception.StackTrace);
            }
            Trace.WriteLine(sb.ToString());
        }

        private static bool IsEnabled()
        {
#if TRACE
            return true;
#else
            return false;
#endif
        }

        private static string GetCallingMethod()
        {
            if (!IsEnabled()) return string.Empty;
            var frame = new StackFrame(3);
            var method = frame.GetMethod();
            return method.DeclaringType != null ? method.DeclaringType.FullName + "." + method.Name : method.Name;
        }

        private static string CreateTraceFile()
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
            var fullPath = Path.Combine(logFileDirectory, LOG_FILE_NAME);
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

        public void Dispose()
        {
            Trace.Flush();
        }
    }
}