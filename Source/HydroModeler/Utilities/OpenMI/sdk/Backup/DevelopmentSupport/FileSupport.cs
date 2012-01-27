#region Copyright
/*
* Copyright (c) 2005,2006,2007, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion 

using System;
using System.Collections;
using System.IO;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
	/// <summary>
	/// This class provides some static methods to handle relative paths to files
	/// </summary>
	public class FileSupport
	{
		/// <summary>
		/// Gets the relative path from a starting directory to a file
		/// </summary>
		/// <param name="baseDirectory">Starting point</param>
		/// <param name="targetFile">File to refer to</param>
		/// <returns></returns>
		public static string GetRelativePath(DirectoryInfo baseDirectory, FileInfo targetFile) 
		{
			// When roots are different, the result is the full path of the target.
			if (!targetFile.Directory.Root.ToString().Equals (baseDirectory.Root.ToString())) 
			{
				return targetFile.FullName;
			}

			// Quick result when the target resides in the base directory.
			if (targetFile.Directory.FullName.Equals(baseDirectory.FullName)) 
			{
				return targetFile.Name;
			}

			ArrayList baseDir = FileSupport.GetDirectoryArray(baseDirectory);
			ArrayList target  = FileSupport.GetDirectoryArray(targetFile.Directory);

			// Finally add the target file name
			return FileSupport.GetRelativeDir(baseDir, target) + targetFile.Name;
		}

		/// <summary>
		/// Gets the relative path from a starting directory to a directory
		/// </summary>
		/// <param name="baseDirectory">Starting point</param>
		/// <param name="targetDir">Directory to refer to</param>
		/// <returns></returns>
		public static string GetRelativePath(DirectoryInfo baseDirectory, DirectoryInfo targetDir) 
		{
			// When roots are different, the result is the full path of the target.
			if (!targetDir.Root.ToString().Equals (baseDirectory.Root.ToString())) 
			{
				return targetDir.FullName;
			}

			// Quick result when the target resides in the base directory.
			if (targetDir.FullName.Equals(baseDirectory.FullName)) 
			{
				return "";
			}

			ArrayList baseDir = GetDirectoryArray(baseDirectory);
			ArrayList target  = GetDirectoryArray(targetDir);

			// Finally add the target file name
			return GetRelativeDir(baseDir, target).TrimEnd ('\\');
		}

		/// <summary>
		/// Gets the relative path given the base and target
		/// </summary>
		/// <param name="baseDir">Array of directories for the base directory</param>
		/// <param name="target">Array of directories for the target file or directory</param>
		/// <returns>The relative path</returns>
		private static string GetRelativeDir (ArrayList baseDir, ArrayList target) 
		{
			// Determine the common part in the full paths of base directory and target file
			int common = 0;
			while ((common < baseDir.Count) && (common < target.Count) && (baseDir[common].Equals(target[common]))) 
			{
				common++;
			}

			// The relative path starts with going up from the base directory file for the uncommon part
			string result = "";
			for (int i = common; i < baseDir.Count; i++) 
			{
				result += ".." + Path.DirectorySeparatorChar;
			}

			// Then the relative path continues with going down to the target file for the uncommon part
			for (int i = common; i < target.Count; i++) 
			{
				result += target[i].ToString() + Path.DirectorySeparatorChar;
			}

			return result;
		}

		/// <summary>
		/// Returns an array of all directories in a path. The first directory is the highest directory in the path.
		/// </summary>
		/// <param name="Directory">Directory from which to generate the array</param>
		/// <returns>ArrayList containing all directories. Each element is a DirectoryInfo object</returns>
		private static ArrayList GetDirectoryArray (DirectoryInfo Directory) 
		{
			ArrayList array = new ArrayList();
			DirectoryInfo dir = Directory;
			while (dir.Parent != null) 
			{
				array.Insert (0, dir.Name);
				dir = dir.Parent;
			}

			return array;
		}

		/// <summary>
		/// Expands a relative path to a FileInfo object
		/// </summary>
		/// <param name="baseDirectory">The directory from which the relative path is defined</param>
		/// <param name="targetFile">The relative path</param>
		/// <returns>FileInfo object corresponding to the base directory and relative path</returns>
		public static FileInfo ExpandRelativePath(DirectoryInfo baseDirectory, string targetFile) 
		{
			string current = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(baseDirectory.FullName);
			FileInfo newFile = new FileInfo(targetFile);
			Directory.SetCurrentDirectory(current);

			return newFile;
		}

		/// <summary>
		/// Expands a relative path to a DirectoryInfo object
		/// </summary>
		/// <param name="baseDirectory">The directory from which the relative path is defined</param>
		/// <param name="targetDir">The relative path</param>
		/// <returns>DirectoryInfo object corresponding to the base directory and relative path</returns>
		public static DirectoryInfo ExpandRelativeDirectory(DirectoryInfo baseDirectory, string targetDir) 
		{
			if (targetDir.Trim().Equals(""))
			{
				return new DirectoryInfo(baseDirectory.FullName);
			}

			string current = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(baseDirectory.FullName);
			DirectoryInfo newFile = new DirectoryInfo(targetDir);
			Directory.SetCurrentDirectory(current);

			return newFile;
		}

		/// <summary>
		/// Generates relative path by path.
		/// </summary>
		/// <param name="dirPath"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string GetRelativePath(string dirPath, string filePath)
		{
			return GetRelativePath(new DirectoryInfo(dirPath), new FileInfo(filePath));
		}
	}
}
