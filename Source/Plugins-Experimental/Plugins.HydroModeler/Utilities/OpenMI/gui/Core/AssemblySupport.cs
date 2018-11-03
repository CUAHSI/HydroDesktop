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
using System.IO;
using System.Collections;
using System.Reflection;

namespace Oatc.OpenMI.Gui.Core
{
	/// <summary>
	/// This class is globaly used to manage assemblies.
	/// </summary>
	public class AssemblySupport
	{
		/// <summary>
		/// One element of internal list of assemblies
		/// </summary>
		private class AssemblyItem
		{
			public Assembly assembly;
			public string fullName;
		}

		/// <summary>
		/// Internal list of assemblies.
		/// </summary>
		private static ArrayList _assemblies;

		/// <summary>
		/// Loads specific assembly into internal list of assemblies.
		/// </summary>
		/// <param name="directory">Directory <c>filename</c> is relative to, or <c>null</c> if <c>filename</c> is absolute or relative to current directory.</param>
		/// <param name="filename">Relative or absolute path to assembly.</param>
		/// <remarks>See <see cref="Utils.GetFileInfo">Utils.GetFileInfo</see> for more info about how
		/// specified file is searched. If file isn't found, method tries to
		/// load assembly from global assembly cache (GAC).</remarks>
		public static void LoadAssembly( string directory, string filename )
		{
			Assembly assembly;
			
			FileInfo assemblyFileInfo = Utils.GetFileInfo( directory, filename );

			// if assemby file exists, try to load it
			if( assemblyFileInfo.Exists )
			{
				assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);
			}
			else
			{
				// if file doesn't exist, try to load assembly from GAC
				try
				{					
					assembly = Assembly.Load(filename);
				}
				catch( Exception e )
				{					
					throw( new Exception("Assembly cannot be loaded (CurrentDirectory='"+Directory.GetCurrentDirectory()+"', Name='"+filename+"')", e) );
				}
			}
		
			// add assembly to list of assemblies only if not already present
			foreach( AssemblyItem assemblyItem in _assemblies )
				if( 0==String.Compare(assemblyItem.fullName, assembly.FullName, true) )
					return;
			
			AssemblyItem newItem = new AssemblyItem();
			newItem.assembly = assembly;
			newItem.fullName = assembly.FullName;
			_assemblies.Add( newItem );			
		}


		/// <summary>
		/// Creates new instance of type contained in one previously loaded assembly, or from application context if 
		/// not found.
		/// </summary>
		/// <param name="typeName">Name of the type</param>
		/// <returns>Returns new instance of specified type.</returns>
		/// <remarks>New instance is created with default parameterless constructor,
		/// if such constructor doesn't exists an exception is thrown.</remarks>
		public static object GetNewInstance( string typeName )
		{
			object result;
			Type type = null;
			
			foreach( AssemblyItem assemblyItem in _assemblies )
			{				
				type = assemblyItem.assembly.GetType( typeName, false );
				if( type!=null )
					break;
			}

			if( type==null )
				type = Type.GetType( typeName, false );
			
			if( type==null )
				throw( new Exception("Class type "+typeName+" not found neither in loaded assemblies nor in application context.") );

			// construct new item with default constructor
			ConstructorInfo constructorInfo = type.GetConstructor( Type.EmptyTypes );
			if( constructorInfo==null )
				throw( new Exception("Requested class type has no default parameterless constructor.") );

			result = constructorInfo.Invoke(null);
			return( result );
		}


		/// <summary>
		/// Releases all assemblies from internal list.
		/// </summary>
		public static void ReleaseAll()
		{
			_assemblies = new ArrayList();
			GC.Collect();			
		}

		/// <summary>
		/// Initializes internal list of assemblies.
		/// </summary>
		static AssemblySupport()
		{
			_assemblies = new ArrayList();
		}
	}
}
