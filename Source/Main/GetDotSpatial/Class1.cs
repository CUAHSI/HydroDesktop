using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace GetDotSpatial
{
    /// <summary>
    /// This class is used for downloading the specified version of DotSpatial libraries
    /// </summary>
    public class Class1
    {
        static void Main(string[] args)
        {
            string zipFilePath = @"C:\dev\Tutorials\ZipStorerTest";
            string outputFolder = @"C:\dev\Tutorials\ZipStorerTest\DotSpatial_1.0.634";

            UnzipDotSpatialFolder(zipFilePath, outputFolder);
        }

        public static void UnzipDotSpatialFolder(string zipFile, string targetFolder)
        {
            
            
            // Previous checkings
            if (string.IsNullOrEmpty(zipFile))
            {
                Console.WriteLine("Unzip DotSpatial - Storage filename not defined.");
                return;
            }
            if (string.IsNullOrEmpty(targetFolder))
            {
                Console.WriteLine("Unzip DotSpatial - Target folder not defined.");
                return;
            }

            try
            {
                // Opens existing zip file
                ZipStorer zip = ZipStorer.Open(zipFile, FileAccess.Read);

                // Read all directory contents
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                // Extract all files in target directory
                string path;
                bool result;
                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    path = Path.Combine(targetFolder, Path.GetFileName(entry.FilenameInZip));
                    result = zip.ExtractFile(entry, path);
                    Console.WriteLine(path + (result ? "" : " (error)"));
                }
                zip.Close();

                Console.WriteLine("Unzip DotSpatial - Source file processed with success.");
            }
            catch (InvalidDataException)
            {
                Console.WriteLine("Unzip DotSpatial Error: Invalid or not supported Zip file.");
            }
            catch
            {
                Console.WriteLine("Unzip DotSpatial Error while processing source file.");
            }
        }
    }
}
