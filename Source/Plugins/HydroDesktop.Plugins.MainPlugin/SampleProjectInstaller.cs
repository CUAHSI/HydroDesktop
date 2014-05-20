using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using HydroDesktop.Configuration;
using DotSpatial.Controls;
using DotSpatial.Extensions;

namespace HydroDesktop.Plugins.MainPlugin
{
    /// <summary>
    /// This class is responsible for installing sample projects that were shipped inside the installer.
    /// It attempts to install the sample projects inside %appdata%\Roaming\HydroDesktop_xxx_exe\extensions
    /// where xxx is the current HydroDesktop version.
    /// If this fails then it tries to install the sample projects to the Documents folder.
    /// </summary>
    public class SampleProjectInstaller
    {
        /// <summary>
        /// Gets the list of sample project files (project templates) available.
        /// This will look in the %appdata%\Roaming\HydroDesktop and also in the %hydrodesktop_sample_projects% directory.
        /// </summary>
        public List<SampleProjectInfo> FindSampleProjectFiles() 
        {
            //create the AbsolutePathToExtensions directory in case this directory doesn't exist
            if (!Directory.Exists(AppManager.AbsolutePathToExtensions))
            {
                Directory.CreateDirectory(AppManager.AbsolutePathToExtensions);
            }
            
            List<SampleProjectInfo> sampleProjectList = new List<SampleProjectInfo>();
            foreach (string absolutePath in Directory.EnumerateFiles(AppManager.AbsolutePathToExtensions, "*.dspx", SearchOption.AllDirectories))
            {
                var sample = new SampleProjectInfo();
                sample.AbsolutePathToProjectFile = absolutePath;
                sample.Name = Path.GetFileNameWithoutExtension(absolutePath);
                sample.Description = "description";
                sample.Version = "1.0";
                sampleProjectList.Add(sample);
            }
            return sampleProjectList;
        }

        private string CopyShippedSampleProject(string shippedProjectFile, string targetDirectory) 
        {
            //the result path to return
            string resultDspxPath = shippedProjectFile;
            
            //check if project file is in excluded names
            string parentDir1 = Path.GetDirectoryName(shippedProjectFile);
            string parentDir = parentDir1;
            if (parentDir1.EndsWith("content") || parentDir1.EndsWith(@"content\"))
            {
                parentDir = Directory.GetParent(parentDir1).FullName;
            }

            //copy directories
            foreach (string dirPath in Directory.GetDirectories(parentDir, "*", SearchOption.AllDirectories))
            {
                string newDir = dirPath.Replace(parentDir, targetDirectory);
                if (!Directory.Exists(newDir))
                {
                    Directory.CreateDirectory(newDir);
                }
            }
                

            //Copy all the files
            foreach (string newPath in Directory.GetFiles(parentDir, "*.*",
                SearchOption.AllDirectories))
            {
                string newPath2 = newPath.Replace(parentDir, targetDirectory);
                if (!File.Exists(newPath2))
                {
                    File.Copy(newPath, newPath2);
                }
                if (newPath2.EndsWith("*.dspx"))
                {
                    resultDspxPath = newPath2;
                }
            }

            //return new project path
            return resultDspxPath;
        }

        private string TryToCreateTargetDirectory(string targetName) 
        {
            string userProjDir = targetName;
            
            if (!Directory.Exists(userProjDir))
            {
                try
                {
                    Directory.CreateDirectory(userProjDir);
                }
                catch (Exception ex)
                {
                    Trace.Write("error creating directory " + userProjDir + " " + ex.Message);
                }
            }
            if (!Directory.Exists(userProjDir))
            {
                try
                {
                    userProjDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HydroDesktop");
                    Directory.CreateDirectory(userProjDir);
                }
                catch (Exception ex)
                {
                    Trace.Write("error creating directory " + userProjDir + " " + ex.Message);
                }
            }
            if (!Directory.Exists(userProjDir))
            {
                try
                {
                    userProjDir = Path.Combine(Path.GetTempPath(), "HydroDesktop");
                    Directory.CreateDirectory(userProjDir);
                }
                catch { }
            }
            if (!Directory.Exists(userProjDir))
                throw new IOException("error creating directory for copying project: " + userProjDir);

            return userProjDir;
        }
        
        public IEnumerable<ISampleProject> SetupInstalledSampleProjects(List<SampleProjectInfo> downloadedSampleProjects) 
        {
            List<ISampleProject> allSampleProjects = new List<ISampleProject>();
            List<ISampleProject> resultList = new List<ISampleProject>();
            
            //this is usually C:\users\{username}\Documents\AppData\Roaming\{HydroDesktop version number}\extensions\packages.
            //this is usually a hiddden folder that doesn't show up in windows explorer.
            string absolutePathToExtensionsInAppdata = AppManager.AbsolutePathToExtensions;
            
            List<string> sampleProjectFilesToInstall = new List<string>();
            List<string> installedSampleProjectNames = new List<string>();
            foreach (SampleProjectInfo p in downloadedSampleProjects)
            {
                installedSampleProjectNames.Add(Path.GetFileNameWithoutExtension(p.AbsolutePathToProjectFile));
                allSampleProjects.Add(p);
            }
            
            //step 1: check list of shipped sample projects in [Program Files]\HydroDesktop\hydrodesktop_sample_projects
            //also add the project files from hd_sample_projects folder
            string projDir = Properties.Settings.Default.SampleProjectsDirectory;
            string shippedSampleProjectDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, projDir);

            //step 2: find names of shipped sample projects that should be copied to the %appdata% folder
            //this excludes shpipped sample projects that have the same name as sample projects in %appdata% folder
            if (Directory.Exists(shippedSampleProjectDir))
            {
                string[] filesInShippedDir = Directory.GetFiles(shippedSampleProjectDir, "*.dspx", SearchOption.AllDirectories);
                foreach (string shippedFile in filesInShippedDir)
                {
                    string shippedProjectName = Path.GetFileNameWithoutExtension(shippedFile);
                    if (!installedSampleProjectNames.Contains(shippedProjectName))
                    {
                        sampleProjectFilesToInstall.Add(shippedFile);
                    }
                }

                //step 3: copy each sample project to target directory
                string defaultTargetBaseDir = AppManager.AbsolutePathToExtensions;
                string packagesDir = Path.Combine(AppManager.AbsolutePathToExtensions, "Packages");

                foreach (string spf in sampleProjectFilesToInstall)
                {
                    //attempt to create target directory             
                    string targDirName = Path.Combine(packagesDir,Path.GetFileNameWithoutExtension(spf));

                    try
                    {
                        string targetDir = TryToCreateTargetDirectory(targDirName);
                        string newFullPath = CopyShippedSampleProject(spf, targetDir);
                        SampleProjectInfo spi = new SampleProjectInfo();
                        spi.AbsolutePathToProjectFile = newFullPath;
                        spi.Description = Path.GetFileNameWithoutExtension(spi.AbsolutePathToProjectFile);
                        spi.Name = spi.Description;
                        resultList.Add(spi);
                    }
                    catch (Exception ex)
                    {
                        Trace.Write("Error copying project files: " + ex.Message);
                    }
                }
            }
          
            if(allSampleProjects.Count >= 3)
            {
                if (allSampleProjects.ElementAt(0).Name == "Europe Map" &&
                    allSampleProjects.ElementAt(1).Name == "North America Map" &&
                    allSampleProjects.ElementAt(2).Name == "World Map")
                {
                    SampleProjectInfo europe = allSampleProjects.ElementAt(0) as SampleProjectInfo;
                    allSampleProjects.RemoveAt(0);
                    allSampleProjects.Insert(2, europe);
                }
            }

            resultList.AddRange(allSampleProjects);
            return resultList;
        }
    }
}
