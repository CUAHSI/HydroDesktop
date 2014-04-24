using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;

namespace HydroDesktop.ErrorReporting
{
    internal static class ZipHelper
    {
        private const long BUFFER_SIZE = 4096;

        public static void AddFileToZip(string zipFilename, string fileToAdd)
        {
            using (var zip = Package.Open(zipFilename, FileMode.OpenOrCreate))
            {
                var destFilename = ".\\" + Path.GetFileName(fileToAdd);
                var uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                if (zip.PartExists(uri))
                {
                    zip.DeletePart(uri);
                }
                var part = zip.CreatePart(uri, "", CompressionOption.Maximum);
                using (var fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Debug.Assert(part != null, "part != null");
                    using (var dest = part.GetStream())
                    {
                        CopyStream(fileStream, dest);
                    }
                }
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