using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;

namespace GetDotSpatial
{
    /// <summary>
    /// This class is used for downloading the specified version of DotSpatial libraries
    /// </summary>
    public class Downloader
    {
        private const string packageXmlFile = "packages.config";
        private const string versionNumberTextFile = "dotspatial_version.txt";
        private const string deleteFilesTextFile = "delete_files.txt";

        private const string defaultPackageSource = @"http://packages.nuget.org/v1";
        private const string defaultPackageTarget = "Packages";
        
        static void Main(string[] args)
        {
            DeleteOldAssemblies();          
            UpdatePackages();
        }

        /// <summary>
        /// Deletes any old assemblies to prevent composition error conflicts
        /// </summary>
        public static void DeleteOldAssemblies()
        {
            try
            {
                //open file delete_files.txt
                string[] filesToDelete = ReadListOfFilesToDelete();

                foreach (string fileToDelete in filesToDelete)
                {
                    string fullPath = FindTargetFolder(fileToDelete);
                    if (fullPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        fullPath = fullPath.Substring(0, fullPath.Length - 1);

                    if (fullPath.EndsWith("*"))
                    {
                        DeleteFilesInDirectory(fullPath.Substring(0, fullPath.Length - 1));
                    }
                    else if (Directory.Exists(Path.GetDirectoryName(fullPath)))
                    {
                        try
                        {
                            //* means delete all files in the directory
                            if (!(fullPath.EndsWith("*")))
                            {
                                File.Delete(fullPath);
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Console.WriteLine("Cannot delete file " + fullPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in DeleteOldAssemblies - " + ex.Message);
            }
        }
        private static void DeleteFilesInDirectory(string dir)
        {
            if (!Directory.Exists(dir)) return;
            
            foreach (string fileToDelete in Directory.GetFiles(dir))
            {
                File.Delete(fileToDelete);
            }
        }

        public static void UpdatePackages()
        {
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, packageXmlFile));
                XmlElement root = doc.DocumentElement;
                if (root.Name != "packages")
                    throw new XmlException("The name of root element in packages.config must be 'packages'.");

                foreach (XmlNode childNode in root.ChildNodes)
                {
                    XmlElement child = childNode as XmlElement;
                    if (child == null) continue;
                    
                    string id = child.GetAttribute("id");
                    string version = child.GetAttribute("version");
                    string source = defaultPackageSource;
                    string target = defaultPackageTarget;

                    if (child.HasAttribute("source"))
                    {
                        source = child.GetAttribute("source");                 
                    }
                    if (child.HasAttribute("target"))
                    {
                        target = child.GetAttribute("target");
                    }
                    DownloadNugetPackage(id, version, source, target);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdatePackages error - " + ex.Message);
            }
        }

        private static string FindTargetFolder(string packageTarget)
        {
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory; //hydroDesktop/Source
            
            if (baseFolder.EndsWith("/") || baseFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                baseFolder = (Directory.GetParent(baseFolder)).FullName;
            }
            string[] parts = packageTarget.Split(new char[] { '/' });
            StringBuilder newPath = new StringBuilder();
            foreach (string part in parts)
            {
                if (part == "..")
                {
                    DirectoryInfo parent = Directory.GetParent(baseFolder);
                    baseFolder = parent.FullName;
                }
                else
                {
                    newPath.Append(part);
                    newPath.Append("/");
                }
            }
            string fullPath = (Path.Combine(baseFolder, newPath.ToString())).Replace('/', Path.DirectorySeparatorChar);
            return fullPath;     
        }

        /// <summary>
        /// Downloads a NUGET package from the official NUGET website
        /// </summary>
        /// <param name="packageId">the package id</param>
        /// <param name="packageVersion">the package version</param>
        public static void DownloadNugetPackage(string packageId, string packageVersion, string packageSource, string packageTarget)
        {
            try
            {

                string targetFolder; //target folder where files will be copied

                //find the 'Packages' folder
                string packagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultPackageTarget);
                //string packagesFolder = FindPackagesFolder(packageTarget);

                //generate name of the .nupkg zip file
                string packageUrl;

                string zipFileToDownload = Path.Combine(packagesFolder, String.Format("{0}.{1}{2}{0}.{1}.nupkg", packageId, packageVersion, Path.DirectorySeparatorChar));

                if (packageSource.Contains("packages.nuget.org"))
                {
                    packageUrl = String.Format("{0}/Package/Download/{1}/{2}", packageSource, packageId, packageVersion);
                }
                else if (packageId.Contains("DotSpatial_x86"))
                {
                    //SDSC.EDU package source is using a special naming convention for package naming
                    packageUrl = String.Format("{0}/DotSpatial_x86.{1}.nupkg", packageSource, packageVersion);
                }
                else if (packageSource.Contains("hydro10.sdsc.edu"))
                {
                    //SDSC.EDU package source is using a special naming convention for package naming
                    packageUrl = String.Format("{0}/{1}.{2}.nupkg", packageSource, packageId, packageVersion);
                }
                else
                {
                    packageUrl = String.Format("{0}/{1}/{2}", packageSource, packageId, packageVersion);
                }
                targetFolder = FindTargetFolder(packageTarget);


                //check if the package file already exists - this also checks the version
                if (File.Exists(zipFileToDownload))
                {
                    Console.WriteLine(String.Format("package file {0} already downloaded.", zipFileToDownload));
                    UnzipNugetPackage(zipFileToDownload, targetFolder, packageId, packageVersion);
                    return;
                }

                //create directory if necessary
                string downloadDirectory = Path.GetDirectoryName(zipFileToDownload);
                if (!Directory.Exists(downloadDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(downloadDirectory);
                    }
                    catch (Exception ex)
                    {
                        throw new UnauthorizedAccessException("Unable to create directory " + downloadDirectory);
                    }
                }

                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        Console.WriteLine("Downloading package " + packageUrl);
                        wc.DownloadFile(packageUrl, zipFileToDownload);
                    }
                }
                catch (WebException ex1)
                {
                    throw new WebException("Error downloading package " + packageUrl + " - " + ex1.Message);
                }
                catch (UnauthorizedAccessException ex2)
                {
                    throw new WebException("Access denied to folder " + Path.GetDirectoryName(zipFileToDownload) + " - " + ex2.Message);
                }

                //also try to unzip the package already downloaded
                UnzipNugetPackage(zipFileToDownload, targetFolder, packageId, packageVersion);

            }
            catch (Exception ex)
            {
                Console.WriteLine("error downloading package " + ex.InnerException.Message);
            }
        }

        private static void UnzipNugetPackage(string zipFile, string targetFolder, string packageId, string packageVersion)
        {
            string[] listOfFilesToUnzip = ReadListOfFilesToUnzip(packageId); //this array is NULL if there are no restrictions
            bool restrictFiles = (listOfFilesToUnzip != null);
            
            if (!targetFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                targetFolder += Path.DirectorySeparatorChar.ToString();
            }
            string packageFolder = Path.GetDirectoryName(zipFile);
            if (!packageFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                packageFolder += Path.DirectorySeparatorChar.ToString();
            }
            bool copyToBinaries = (targetFolder != packageFolder);
            
            if (!Directory.Exists(targetFolder))
            {
                try
                {
                    Directory.CreateDirectory(targetFolder);
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException(String.Format("Error creating directory {0} - {1}", targetFolder, ex.Message));
                }
            }

            // Previous checkings
            string shortFileName = Path.GetFileName(zipFile);
            if (string.IsNullOrEmpty(zipFile))
            {
                throw new ArgumentException(String.Format("Unzip {0} - filename not defined.", shortFileName));
            }
            if (string.IsNullOrEmpty(targetFolder))
            {
                throw new ArgumentException(String.Format("Unzip {0} - Target folder not defined.", shortFileName));
            }
            if (!Directory.Exists(targetFolder))
                throw new IOException(String.Format("Directory {0} doesn't exist.", targetFolder));

            //checking if directory already contains unzipped files
            if (IsCorrectVersionUnzipped(targetFolder, packageId, packageVersion))
            {
                Console.WriteLine(String.Format("The package {0} is already unzipped.", zipFile));
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
                    if (copyToBinaries) //select the files which should be copied to /Binaries folder
                    {
                        path = Path.Combine(targetFolder, Path.GetFileName(entry.FilenameInZip));
                        string ext = Path.GetExtension(path).ToLower();

                        if (!restrictFiles) //don't restrict files
                        {
                            if (entry.FilenameInZip.StartsWith("lib"))
                            {
                                result = zip.ExtractFile(entry, path);
                                Console.WriteLine(path + (result ? "" : " (error)"));
                            }
                        }
                        else //restrict files
                        {
                            if (listOfFilesToUnzip.Contains(Path.GetFileName(entry.FilenameInZip)))
                            {
                                result = zip.ExtractFile(entry, path);
                                Console.WriteLine(path + (result ? "" : " (error)"));
                            }
                        }
                    }
                    else
                    {
                        path = (Path.Combine(targetFolder, entry.FilenameInZip)).Replace('/', Path.DirectorySeparatorChar);
                        result = zip.ExtractFile(entry, path);
                        Console.WriteLine(path + (result ? "" : " (error)"));
                    }
                }
                zip.Close();

                //add the version.txt file
                string versionFilePath = Path.Combine(targetFolder, String.Format("{0}-version.txt", packageId));
                CreateTextFile(versionFilePath, packageVersion);

                Console.WriteLine("Unzip Nuget Package - Source file processed with success.");
            }
            catch (InvalidDataException)
            {
                Console.WriteLine("Unzip Package Error: Invalid or not supported Zip file.");
            }
            catch
            {
                Console.WriteLine("Unzip Package Error while processing source file.");
            }
        }

        /// <summary>
        /// Creates a text file containing the string
        /// </summary>
        /// <param name="str">the content of the text file</param>
        private static void CreateTextFile(string versionFilePath, string versionNumber)
        {
            try
            {
                using (StreamWriter sr = new StreamWriter(versionFilePath, false))
                {
                    sr.WriteLine(versionNumber);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("unable to create file: " + versionFilePath);
            }
            catch (IOException)
            {
                Console.WriteLine("unable to create file: " + versionFilePath);
            }
        }

        /// <summary>
        /// Check if the correct version of the NUGET package has been unzipped
        /// </summary>
        /// <param name="targetFolder">the target folder</param>
        /// <param name="version">the version number</param>
        /// <returns></returns>
        private static bool IsCorrectVersionUnzipped(string targetFolder, string packageId, string version)
        {
            if (!Directory.Exists(targetFolder)) return false;
            
            string versionTextFile = Path.Combine(targetFolder, String.Format("{0}-version.txt", packageId, version));
            
            if (!File.Exists(versionTextFile)) return false;

            try
            {
                using (StreamReader r = new StreamReader(versionTextFile))
                {
                    string line = r.ReadLine();
                    if (line.ToLower().Trim() == version) return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading version.txt text file. " + ex.Message);
                return false;
            }
        }

        public static string[] ReadListOfFilesToDelete()
        {
            string deleteFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, deleteFilesTextFile);

            return readLinesFromFile(deleteFilesPath);
        }

        public static string[] ReadListOfFilesToUnzip(string packageId)
        {
            string unzipFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, packageId + "_files.txt");

            if (!File.Exists(unzipFilesPath)) return null;

            return readLinesFromFile(unzipFilesPath);
        }

        private static string[] readLinesFromFile(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException(Path.Combine("The file {0} doesn't exist.", fileName));

            List<string> lines = new List<string>();
            using (StreamReader r = new StreamReader(fileName))
            {
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    // "line" is a line in the file. Add it to our List.
                    lines.Add(line);
                }
            }
            return lines.ToArray();
        }
    }
}
