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
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Security.Policy;
using System.Configuration.Assemblies;


namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
	/// <summary>
	/// Number of methods for general operations on objects. They comprise:
	/// 1.	instantiating objects given a class name
	/// 2.	deep copy of objects
	/// </summary>
	public class ObjectSupport
	{
		private static ArrayList _assemblyList = new ArrayList();

		#region Copy objects

		/// <summary>
		/// Gets a deep copy of a specified object.
		/// Deep copying copies all primitive and enumeration properties and the properties for which MetaInfo "ObjectCopy" is set to true
		/// </summary>
		/// <param name="source">The object to be copied</param>
		/// <returns>The copied object</returns>
		public static object GetCopy(object source) 
		{
			object copy = ObjectSupport.GetInstance(source.GetType());
			if (copy != null) 
			{
				ObjectSupport.Copy (source, copy);
			}

			return copy;
		}

		/// <summary>
		/// Gets a deep copy of a specified object and copies referenced files
		/// Deep copying copies all primitive and enumeration properties and the properties for which MetaInfo "ObjectCopy" is set to true
		/// </summary>
		/// <param name="source">The object to be copied</param>
		/// <param name="path">The path in which copied files will be positioned (relative to original position)</param>
		/// <returns>The copied object</returns>
		public static object GetCopy(object source, string path) 
		{
			if (source == null) 
			{
				return null;
			}

			if (source is FileSystemInfo) 
			{
				if ((path != null) && (!path.Trim().Equals("")))
				{
					return CopyFile ((FileSystemInfo) source, path);
				}
				else 
				{
					return ObjectSupport.GetInstance(source.GetType(), source.ToString());
				}
			}

			object copy = ObjectSupport.GetInstance(source.GetType(), source.ToString());
			if (copy != null) 
			{
				ObjectSupport.Copy (source, copy, path);
			}

			return copy;
		}

		/// <summary>
		/// Creates an object which is a deep copy fo a source object, including copying of referenced file
		/// Deep copying copies all primitive and enumeration properties and the properties for which MetaInfo "ObjectCopy" is set to true
		/// </summary>
		/// <param name="source">The object to be copied</param>
		/// <param name="copiedObjects">Lookup table for already copied objects</param>
		/// <param name="path">Path where files are copied to</param>
		/// <returns></returns>
		private static object GetCopy(object source, Hashtable copiedObjects, string path) 
		{
			if (copiedObjects[source] != null) 
			{
				return copiedObjects[source];
			}
			else if (source is FileSystemInfo) 
			{
				object copiedFile = CopyFile ((FileSystemInfo) source, path);
				copiedObjects.Add (source, copiedFile);
				return copiedFile;
			}
			else 
			{
				object copy = ObjectSupport.GetInstance(source.GetType());
				if (copy != null) 
				{
					copiedObjects.Add (source, copy);
					ObjectSupport.Copy (source, copy, copiedObjects, path);
				}
				return copy;
			}
		}

		/// <summary>
		/// Deep copies all properties of source into the properties of target
		/// Deep copying copies all primitive and enumeration properties and the properties for which MetaInfo "ObjectCopy" is set to true
		/// </summary>
		/// <param name="source">The source object</param>
		/// <param name="target">The target object</param>
		public static void Copy(object source, object target) 
		{
			ObjectSupport.Copy (source, target, "");
		}

		/// <summary>
		/// Deep copies all properties of source into the properties of target, including files
		/// Deep copying copies all primitive and enumeration properties and the properties for which MetaInfo "ObjectCopy" is set to true
		/// </summary>
		/// <param name="source">The source object</param>
		/// <param name="target">The target object</param>
		/// <param name="path">Path where files are copied to</param>
		public static void Copy(object source, object target, string path) 
		{
			ObjectSupport.Copy (source, target, new Hashtable(), path);
		}

		/// <summary>
		/// Gets the deep copy
		/// </summary>
		/// <param name="source">The object to be copied</param>
		/// <param name="target">The object which will be the copy</param>
		/// <param name="copiedObjects">Collection of objects and their copied equivalents. Will be populated and queried during copy.</param>
		/// <param name="path">Relative path to location where copied files will reside</param>
		private static void Copy(object source, object target, Hashtable copiedObjects, string path) 
		{
			Copy (source, target, copiedObjects, path, true);
		}

		/// <summary>
		/// Gets the deep copy
		/// </summary>
		/// <param name="source">The object to be copied</param>
		/// <param name="target">The object which will be the copy</param>
		/// <param name="copiedObjects">Collection of objects and their copied equivalents. Will be populated and queried during copy.</param>
		/// <param name="path">Relative path to location where files will be copied to</param>
		/// <param name="copyValue">Indicates whether the new object should refer to a copy or to the same object</param>
		private static void Copy(object source, object target, Hashtable copiedObjects, string path, bool copyValue) 
		{
			if (source is IList) 
			{
				IList copiedArray = (IList) target;
				copiedArray.Clear();
				for (int j = 0; j < ((IList) source).Count; j++) 
				{
					object copiedArrayValue = ((IList) source)[j];
					if (copyValue) 
					{
						copiedArrayValue = ObjectSupport.GetCopy (copiedArrayValue, copiedObjects, path);
					}
					copiedArray.Add (copiedArrayValue);
				}
			}
			else if (source is IDictionary) 
			{
				IDictionary copiedArray = (IDictionary) target;
				copiedArray.Clear();

				IDictionaryEnumerator dictionaryEnumerator = ((IDictionary) source).GetEnumerator();
				while (dictionaryEnumerator.MoveNext()) 
				{
					object copiedArrayKey = dictionaryEnumerator.Key;
					object copiedArrayValue = dictionaryEnumerator.Value;
					if (copyValue) 
					{
						copiedArrayKey   = ObjectSupport.GetCopy (copiedArrayKey, copiedObjects, path);
						copiedArrayValue = ObjectSupport.GetCopy (copiedArrayValue, copiedObjects, path);
					}

					if (!copiedArray.Contains (copiedArrayKey)) 
					{
						copiedArray.Add (copiedArrayKey, copiedArrayValue);
					}
				}
			} 
				// Special handling for files
			else if (source is FileSystemInfo) 
			{
				if (copyValue && (path != null) && (!path.Trim().Equals("")))
				{
					object copiedValue = CopyFile ((FileSystemInfo) source, path);
				}
			}
			else 
			{
				PropertyInfo[] property = source.GetType().GetProperties();
				for (int i = 0; i < property.Length; i++) 
				{
					object sourceValue = property[i].GetValue (source, null);

					if (sourceValue != null) 
					{
						// Default copied value. This is just a reference to the original value
						object copiedValue = sourceValue;

						// If the value has been copied before, use the same copied equivalent
						if (copiedObjects.ContainsKey (sourceValue)) 
						{
							copiedValue = copiedObjects[sourceValue];
						} 
						else 
						{
							// Determine whether the property is to be copied 
							bool copyPropertyValue = (Boolean) MetaInfo.GetAttributeDefault (source.GetType(), property[i].Name, "ObjectCopy", false);

							if (copyPropertyValue)
							{
								if (property[i].CanWrite) 
								{
									copiedValue = ObjectSupport.GetCopy (sourceValue, copiedObjects, path);
								}
								else
								{
									object targetValue = property[i].GetValue (target, null);
									Copy (sourceValue, targetValue, copiedObjects, path);
								}
							}
							else if ((sourceValue is IList) || (sourceValue is IDictionary)) 
							{
								object targetValue = property[i].GetValue (target, null);
								Copy (sourceValue, targetValue, copiedObjects, path, copyPropertyValue);
							}
						}

						// Populate the target object
						if (property[i].CanWrite) 
						{
							property[i].SetValue (target, copiedValue, null);
						}

						if (!copiedObjects.Contains(sourceValue)) 
						{
							copiedObjects.Add (sourceValue, copiedValue);
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets a copy of a file.
		/// If the file doesn't exist, the copied file will not exist neither.
		/// </summary>
		/// <param name="fileSystem">The file or directory to be copied</param>
		/// <param name="path">The relative path (starting from the location of the original file) to the directory where the file will be copied to</param>
		/// <returns>The copied file</returns>
		private static FileSystemInfo CopyFile (FileSystemInfo fileSystem, string path) 
		{
			if ((path != null) && (path.Trim() != "")) 
			{
				if (fileSystem is FileInfo) 
				{
					FileInfo file = (FileInfo) fileSystem;
					FileInfo targetFile = FileSupport.ExpandRelativePath(file.Directory, path + "\\" + file.Name);
					if (File.Exists(file.FullName))
					{
						if (!targetFile.Directory.Exists) 
						{
							targetFile.Directory.Create();
						}
						file.CopyTo(targetFile.FullName, true);
					}
					else
					{
						if (File.Exists(targetFile.FullName))
						{
							targetFile.Delete();
						}
					}

					return new FileInfo(targetFile.FullName);
				}

				if (fileSystem is DirectoryInfo) 
				{
					DirectoryInfo dir = (DirectoryInfo) fileSystem;
					DirectoryInfo targetDir = FileSupport.ExpandRelativeDirectory(dir, path);

					if (Directory.Exists(dir.FullName))
					{
						DirCopy (dir.FullName, targetDir.FullName, false);
					}
					else
					{
						if (Directory.Exists(targetDir.FullName))
						{
							Directory.Delete (targetDir.FullName);
						}
					}

					return targetDir;
				}

				return null;
			}
			else 
			{
				return fileSystem;
			}
		}

		private static void DirCopy(string srcdir, string destdir, bool recursive)
		{
			DirectoryInfo   dir;
			FileInfo[]      files;
			DirectoryInfo[] dirs;
			string          tmppath;

			//determine if the destination directory exists, if not create it
			if (! Directory.Exists(destdir))
			{
				Directory.CreateDirectory(destdir);
			}

			dir = new DirectoryInfo(srcdir);
            
			//if the source dir doesn't exist, throw
			if (! dir.Exists)
			{
				throw new ArgumentException("source dir doesn't exist -> " + srcdir);
			}

			//get all files in the current dir
			files = dir.GetFiles();

			//loop through each file
			foreach(FileInfo file in files)
			{
				//create the path to where this file should be in destdir
				tmppath=Path.Combine(destdir, file.Name);                

				//copy file to dest dir
				file.CopyTo(tmppath, false);
			}

			//cleanup
			files = null;
            
			//if not recursive, all work is done
			if (! recursive)
			{
				return;
			}

			//otherwise, get dirs
			dirs = dir.GetDirectories();

			//loop through each sub directory in the current dir
			foreach(DirectoryInfo subdir in dirs)
			{
				//create the path to the directory in destdir
				tmppath = Path.Combine(destdir, subdir.Name);

				//recursively call this function over and over again
				//with each new dir.
				DirCopy(subdir.FullName, tmppath, recursive);
			}
            
			//cleanup
			dirs = null;
            
			dir = null;
		}

		#endregion

		#region Assembly handling

		/// <summary>
		/// Loads an assembly.
		/// The assembly name can be either a full path to a file or a full or partial name of an assembly registered in the GAC.
		/// An empty assembly name is ignored.
		/// </summary>
		/// <param name="assemblyName">The assembly name</param>
		/// <exception cref="System.Exception">Assembly cannot be found in the GAC</exception>
		public static void LoadAssembly(string assemblyName) 
		{
			if ((assemblyName == null) || (assemblyName.Trim().Equals(""))) 
			{
				return;
			}

			Assembly assembly = null;

			if (File.Exists(assemblyName)) 
			{
				FileInfo assemblyFile = new FileInfo(assemblyName);

				foreach (Assembly loadedAssembly in _assemblyList) 
				{
					if (loadedAssembly.Location.EndsWith(Path.DirectorySeparatorChar + assemblyFile.Name))
					{
						return;
					}
				}

				assembly = Assembly.LoadFrom(assemblyFile.FullName);
			}
			else 
			{
				try
				{
					// VS2005 fix
					// The line below produces "System.Reflection.Assembly.LoadWithPartialName(string)' is obsolete:
					// 'This method has been deprecated. Please use Assembly.Load() instead. " warning
					// however for backwards compatibility we need to use this method,
					// because some older XML configuration files still may use short assembly names
#pragma warning disable 0618
					assembly = Assembly.LoadWithPartialName(assemblyName); 
#pragma warning restore 0618
				}
				catch (FileNotFoundException e) 
				{
					// bug in framework; assembly could be found
					if (assembly == null) 
					{
						throw e;
					}
				}
			}

			LoadAssembly (assembly);
		}

		/// <summary>
		/// Loads an assembly.
		/// </summary>
		/// <param name="assembly">The assembly</param>
		public static void LoadAssembly (Assembly assembly) 
		{
			if ((assembly != null) && !_assemblyList.Contains(assembly)) 
			{
				_assemblyList.Add (assembly);
			}
		}

		/// <summary>
		/// Tells whether an assembly has been loaded already.
		/// </summary>
		/// <param name="assembly">The assembly</param>
		/// <returns>Indication of loaded</returns>
		public static bool IsLoadedAssembly (Assembly assembly) 
		{
			return _assemblyList.Contains (assembly);
		}

		#endregion

		#region Object instantiation

		/// <summary>
		/// Gets the class object given a string describing the class. The following assemblies are queried:
		/// 1.	All assemblies loaded with ObjectSupport.LoadAssembly
		/// 2.	All assemblies which reside in the same directory as this assembly
		/// </summary>
		/// <param name="ClassType">The class name, including path with namespaces</param>
		/// <returns>The class object or null if not found</returns>
		/// <exception cref="System.Exception">Class cannot be found</exception>
		public static Type GetType(string ClassType) 
		{
			if (ClassType == null) 
			{
				return null;
			}

			Type type = Type.GetType(ClassType);
			for (int i = 0; (i < _assemblyList.Count) && (type == null); i++)
			{
				Assembly assembly = (Assembly) _assemblyList[i];
				if (assembly != null) 
				{
					type = assembly.GetType(ClassType);
				}
			}

			if (type == null) 
			{
				throw new Exception("Cannot find class " + ClassType);
			}

			return type;
		}

		/// <summary>
		/// Creates a new object. 
		/// Types with an argumentless constructor can be created this way
		/// </summary>
		/// <param name="classType">Full class name of the object to be instantiated</param>
		/// <returns>New object, null if not possible</returns>
		public static object GetInstance (string classType) 
		{
			return ObjectSupport.GetInstance(ObjectSupport.GetType(classType));
		}

		/// <summary>
		/// Creates a new object using a base value (e.g. a string with its value).
		/// Normally primitives, enumerations and some value types can be instantiated this way.
		/// Also types with constructors having one argument can be instantiated.
		/// </summary>
		/// <param name="classType">Full class name of the object to be instantiated</param>
		/// <param name="baseValue">Value which is passed as argument to the constructor</param>
		/// <returns>New object, null if not possible</returns>
		public static object GetInstance (string classType, object baseValue) 
		{
			return ObjectSupport.GetInstance(ObjectSupport.GetType(classType), baseValue, null);
		}

		/// <summary>
		/// Creates a new object. 
		/// Types with an argumentless constructor can be created this way
		/// </summary>
		/// <param name="type">Class type of the object to be instantiated</param>
		/// <returns>New object, null if not possible</returns>
		public static object GetInstance (Type type) 
		{
			if (type == null) 
			{
				return null;
			}

			Type[] types = new Type[0];

			ConstructorInfo constructor = type.GetConstructor(types);
			if (constructor != null) 
			{
				return constructor.Invoke(null);
			}

			return null;
		}

		/// <summary>
		/// Creates a new object using a base value (e.g. a string with its value).
		/// Normally primitives, enumerations and some value types can be instantiated this way.
		/// Also types with constructors having one argument can be instantiated.
		/// </summary>
		/// <param name="type">Class type of the object to be instantiated</param>
		/// <param name="baseValue">Value which is passed as argument to the constructor</param>
		/// <returns>New object, null if not possible</returns>
		public static object GetInstance (Type type, object baseValue) 
		{
			return ObjectSupport.GetInstance (type, baseValue, null);
		}

		/// <summary>
		/// Creates a new object using a base value (e.g. a string with its value).
		/// Normally primitives, enumerations and some value types can be instantiated this way.
		/// Also types with constructors having one argument can be instantiated.
		/// </summary>
		/// <param name="type">Class type of the object to be instantiated</param>
		/// <param name="baseValue">Value which is passed as argument to the constructor</param>
		/// <param name="culture">Culture info used for parsing the base value</param>
		/// <returns>New object, null if not possible</returns>
		public static object GetInstance (Type type, object baseValue, CultureInfo culture) 
		{
			if (type == null) 
			{
				return null;
			}
			else if (type.IsEnum) 
			{
				return Enum.Parse (type, baseValue.ToString(), true);
			}
			else if (type.Equals (typeof (String)))
			{
				return baseValue.ToString();
			}
			else if (type.Equals (typeof (Double)))
			{
				if (culture == null) 
				{
					return Double.Parse (baseValue.ToString());
				}
				else 
				{
					return Double.Parse (baseValue.ToString(), culture);
				}
			}
			else if (type.Equals (typeof (Boolean)))
			{
				return Boolean.Parse (baseValue.ToString());
			}
			else if (type.Equals (typeof (Int16)))
			{
				return Int16.Parse (baseValue.ToString());
			}
			else if (type.Equals (typeof (Int32)))
			{
				return Int32.Parse (baseValue.ToString());
			}
			else if (type.Equals (typeof (Int64)))
			{
				return Int64.Parse (baseValue.ToString());
			}
			else if (type.Equals (typeof (DateTime)))
			{
				return DateTime.Parse (baseValue.ToString(), culture.DateTimeFormat);
			}
			else
			{
				Type[] types;
				Type classType;
				ConstructorInfo constructor;

				if (baseValue != null)
				{
					types = new Type[1];
					classType = type;

					while (classType != null) 
					{
						types[0] = baseValue.GetType();
						while (types[0] != null) 
						{
							constructor = classType.GetConstructor(types);
							if (constructor != null) 
							{
								Object[] arguments = new Object[1];
								arguments[0] = baseValue;
								Object instance = constructor.Invoke(arguments);
								return instance;
							}  
							else 
							{
								types[0] = types[0].BaseType;
							}
						}
						classType = classType.BaseType;
					}
				}

				types = new Type[0];
				classType = type;
				while (classType != null) 
				{
					constructor = type.GetConstructor(types);
					if (constructor != null) 
					{
						object instance = constructor.Invoke(null);
						return instance;
					}  
					classType = classType.BaseType;
				}

				return null;
			}
		}

		#endregion
	}
}
