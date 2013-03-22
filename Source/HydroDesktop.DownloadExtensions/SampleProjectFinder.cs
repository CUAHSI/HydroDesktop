using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using System.IO;

namespace HydroDesktop.DownloadExtensions
{
    public class SampleProjectFinder
    {
        /// <summary>
        /// Checks if the sample project package is already installed.
        /// </summary>
        /// <returns>Returns true, if the sample project is installed in the "Packages" folder
        /// Returns false otherwise.</returns>
        /// <remarks>This code assumes that the sample project's .dspx file name is the same as the package name.</remarks>
        public static bool IsSampleProjectInstalled(string packageName)
        {
            string dspxFileName = packageName + ".dspx";
            DirectoryInfo dir = new DirectoryInfo(AppManager.AbsolutePathToExtensions);
            if (!dir.Exists)
            {
                dir.Create();
            }
            foreach (string absolutePath in Directory.EnumerateFiles(AppManager.AbsolutePathToExtensions, "*.dspx", SearchOption.AllDirectories))
            {
                if (Path.GetFileNameWithoutExtension(absolutePath) == packageName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
