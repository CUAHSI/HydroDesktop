using System.IO;
using System.IO.Compression;

namespace HydroDesktop.ErrorReporting
{
    internal static class ZipHelper
    {
        private const long BUFFER_SIZE = 4096;

        public static void AddFileToZip(string zipFilename, string fileToAdd)
        {
            using (var sourceFile = new FileStream(fileToAdd, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var destFile = File.Create(zipFilename))
            using (var compStream = new GZipStream(destFile, CompressionMode.Compress))
            {
                CopyStream(sourceFile, compStream);
            }
        }

        private static void CopyStream(Stream inputStream, Stream outputStream)
        {
            var bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            var buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }
        } 
    }
}