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

        private const string defaultPackageSource = @"http://packages.nuget.org/v1";
        private const string defaultPackageTarget = "Packages";
        
        static void Main(string[] args)
        {
            UpdateExternalFiles();
        }

        public static void UpdateExternalFiles()
        {
            UpdateDotSpatial();
            UpdatePackages();
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

                foreach (XmlElement child in root.ChildNodes)
                {
                    string id = child.GetAttribute("id");
                    string version = child.GetAttribute("version");
                    string source = defaultPackageSource;
                    string target = defaultPackageTarget;

                    if (child.HasAttribute("source") && child.HasAttribute("target"))
                    {
                        source = child.GetAttribute("source");
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
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory; //hydroDesktop/Source/Main
            
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
            string targetFolder; //target folder where files will be copied
            
            //find the 'Packages' folder
            string packagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultPackageTarget);
            //string packagesFolder = FindPackagesFolder(packageTarget);

            //generate name of the .nupkg zip file
            string packageUrl;

            string zipFileToDownload = Path.Combine(packagesFolder, String.Format("{0}.{1}{2}{0}.{1}.nupkg", packageId, packageVersion, Path.DirectorySeparatorChar));

            if (packageTarget == defaultPackageTarget)
            {
                packageUrl = String.Format("{0}/Package/Download/{1}/{2}", packageSource, packageId, packageVersion);
                targetFolder = Path.GetDirectoryName(zipFileToDownload);
            }
            else
            {
                packageUrl = String.Format("{0}/{1}/{2}", packageSource, packageId, packageVersion);
                targetFolder = FindTargetFolder(packageTarget);
            }

            //check if the package file already exists
            if (File.Exists(zipFileToDownload) && Directory.Exists(targetFolder))
            {
                Console.WriteLine(String.Format("package file {0} already downloaded.", zipFileToDownload));
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

            UnzipNugetPackage(zipFileToDownload, targetFolder);

        }

        private static void UnzipNugetPackage(string zipFile, string targetFolder)
        {
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
            string[] files = Directory.GetFiles(targetFolder, "*.dll");
            if (files.Length > 0)
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
                    if (copyToBinaries) //only copy dll, pdb and xml to binaries
                    {
                        path = Path.Combine(targetFolder, Path.GetFileName(entry.FilenameInZip));
                        string ext = Path.GetExtension(path).ToLower();
                        if (entry.FilenameInZip.StartsWith("lib"))
                        {
                            result = zip.ExtractFile(entry, path);
                            Console.WriteLine(path + (result ? "" : " (error)"));
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

        /// <summary>
        /// Updates DotSpatial Dll's to the version specified in dotspatial_version.txt
        /// </summary>
        public static void UpdateDotSpatial()
        {
            try
            {
                //read version number from the text file
                string DotSpatialVersion = ReadDotSpatialVersion();
                
                //download the DotSpatial .nupkg zip file
                string DotSpatialZipFile = DownloadDotSpatial(DotSpatialVersion);
                
                //unzip the DotSpatial .nupkg file to the specified folder
                UnzipDotSpatialFolder(DotSpatialZipFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public static string ReadDotSpatialVersion()
        {
            string versionNumberFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, versionNumberTextFile);
            
            if (!File.Exists(versionNumberFilePath)) throw new FileNotFoundException(Path.Combine("The file {0} doesn't exist.", versionNumberFilePath));

            using (StreamReader r = new StreamReader(versionNumberFilePath))
            {
                string line = r.ReadLine();
                return line;
            }
        }

        /// <summary>
        /// Downloads the DotSpatial .zip file to the local folder
        /// </summary>
        /// <param name="teamCityBuildNumber">DotSpatial "team city" build number</param>
        /// <param name="productVersion">DotSpatial product version</param>
        public static string DownloadDotSpatial(string productVersion)
        {
            //generate the URL
            string url = String.Format(@"http://hydro10.sdsc.edu/nuget/Packages/DotSpatial_x86.{0}.nupkg", productVersion);
            
            //find the 'Packages' folder
            string packagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");

            //generate name of the .nupkg zip file
            string zipFileToDownload = Path.Combine(packagesFolder, String.Format("DotSpatial_x86.{0}{1}DotSpatial_x86.{0}.nupkg", productVersion, Path.DirectorySeparatorChar));
            
            //only re-download packages file if it exists
            if (File.Exists(zipFileToDownload))
            {
                Console.WriteLine(String.Format("Package file {0} is already downloaded.", zipFileToDownload));
                return zipFileToDownload;
            }

            //check the download directory
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

            //download the .nupkg file
            try
            {
                using (WebClient wc = new WebClient())
                {
                    Console.WriteLine("Downloading " + url + " ...");
                    wc.DownloadFile(url, zipFileToDownload);
                    return zipFileToDownload;
                }
            } 
            catch (WebException ex1)
            {
                throw new WebException("Error downloading package " + url + " - " + ex1.Message);
            }
            catch(UnauthorizedAccessException ex2)
            {
                throw new WebException("Access denied to folder " + Path.GetDirectoryName(zipFileToDownload) + " - " + ex2.Message);
            }
        }

        public static void UnzipDotSpatialFolder(string zipFile)
        {
            // Create DotSpatial Packages folder
            string targetFolder = Path.Combine(Path.GetDirectoryName(zipFile), "lib");
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            
            // Previous checkings
            if (string.IsNullOrEmpty(zipFile))
            {
                throw new ArgumentException("Unzip DotSpatial - .nupkg filename not defined.");
            }
            if (string.IsNullOrEmpty(targetFolder))
            {
                throw new ArgumentException("Unzip DotSpatial - Target folder not defined.");
            }
            if (!Directory.Exists(targetFolder))
                throw new IOException("Directory " + targetFolder + " doesn't exist.");

            // check if the Dll's are already present. If all dll's are found, don't unzip the files.
            string[] requiredAssemblies = new string[] 
            {
                "DotSpatial.Analysis",
                "DotSpatial.Controls",
                "DotSpatial.Data",
                "DotSpatial.Data.Forms",
                "DotSpatial.Modeling",
                "DotSpatial.Modeling.Forms",
                "DotSpatial.Projections",
                "DotSpatial.Projections.Forms",
                "DotSpatial.Serialization",
                "DotSpatial.Symbology",
                "DotSpatial.Symbology.Forms",
                "DotSpatial.Topology"
            };
            bool assembliesExist = true;
            foreach (string assemblyName in requiredAssemblies)
            {
                string fullPath = Path.Combine(targetFolder, assemblyName + ".dll");
                if (!File.Exists(fullPath))
                {
                    assembliesExist = false;
                    break;
                }
            }

            if (assembliesExist)
            {
                Console.WriteLine(String.Format("The Zip File {0} is already unzipped.",zipFile));
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

                    //only extract required assemblies
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);
                    if (requiredAssemblies.Contains(fileNameWithoutExt))
                    {
                        result = zip.ExtractFile(entry, path);
                        Console.WriteLine(path + (result ? "" : " (error)"));
                    }
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
