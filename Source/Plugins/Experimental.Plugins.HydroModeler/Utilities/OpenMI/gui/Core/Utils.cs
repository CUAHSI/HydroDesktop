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
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Microsoft.Win32;


namespace Oatc.OpenMI.Gui.Core
{
	/// <summary>
	/// Class contains support methods.
	/// </summary>
	public class Utils
	{

		/// <summary>
		/// Determines whether two dimensions are equal.
		/// </summary>
		/// <param name="dimension1">Dimension one</param>
		/// <param name="dimension2">Dimension two</param>
		/// <returns>Returns <c>true</c> if powers of all dimension bases are same, otherwise returns <c>false</c>.</returns>
		public static bool CompareDimensions(IDimension dimension1, IDimension dimension2)
		{
			for (int i = 0; i < (int)DimensionBase.NUM_BASE_DIMENSIONS; i++)			
				if (dimension1.GetPower((DimensionBase) i) != dimension2.GetPower((DimensionBase) i))				
					return( false );
				
			return( true );
		}


		/// <summary>
		/// Gets <c>FileInfo</c> of file specified by it's (eventually relative) path.
		/// </summary>
		/// <param name="relativeDir">Directory <c>filename</c> is relative to, or <c>null</c> if <c>filename</c> is absolute path or relative path to current directory.</param>
		/// <param name="filename">Relative or absolute path to file.</param>
		/// <returns>Returns <c>FileInfo</c> of file specified.</returns>
		public static FileInfo GetFileInfo( string relativeDir, string filename )
		{
			string oldDirectory=null;
			if( relativeDir!=null )
				oldDirectory = Directory.GetCurrentDirectory();

			try 
			{
				if( relativeDir!=null )
					Directory.SetCurrentDirectory( relativeDir );
				return( new FileInfo(filename) );
			}
			finally
			{
				if( oldDirectory!=null )
					Directory.SetCurrentDirectory( oldDirectory );
			}
		}


		/// <summary>
		/// Converts event to <c>string</c> representation.
		/// </summary>
		/// <param name="Event">Event to be converted to <c>string</c></param>
		/// <returns>Returns resulting <c>string</c>.</returns>
		public static string EventToString( IEvent Event )
		{
			StringBuilder builder = new StringBuilder( 200 );
			builder.Append( "[Type=" );
			builder.Append( Event.Type.ToString() );
			
			if( Event.Description!=null )
			{
				builder.Append( "][Message=" );
				builder.Append( Event.Description );
			}
			
			if( Event.Sender != null )
			{
				builder.Append( "][ModelID=" );
				builder.Append( ((ILinkableComponent) Event.Sender).ModelID );
			}
			
			if( Event.SimulationTime != null )
			{
				builder.Append( "][SimTime=" );
				builder.Append( CalendarConverter.ModifiedJulian2Gregorian(Event.SimulationTime.ModifiedJulianDay).ToString() );
			}

			builder.Append( ']' );	

			return( builder.ToString() );
		}
		

		private const string regOprExtension = ".opr";
		private const string regOprDescription = "OmiEd project";
		private const string regOprIdentifier = "OmiEdProject";

		private const string regOmiExtension = ".omi";
		private const string regOmiDescription = "OpenMI model";
		private const string regOmiIdentifier = "OpenMIModel";


		/// <summary>
		/// Registers OPR and OMI file extension in Win32 registry to be opened with specific OmiEd application.
		/// </summary>
		/// <param name="applicationPath">Full path to specific OmiEd application executable.</param>
		public static void RegisterFileExtensions( string applicationPath )
		{
			RegistryKey keyExtension, keyDefaultIcon, keyIdentifier,
				subKeyShell, subKeyOpen, subKeyCommand;
			
			// OPR extension
			keyExtension = Registry.ClassesRoot.CreateSubKey(regOprExtension);
			keyExtension.SetValue(null, regOprIdentifier);
			keyExtension.Close();

			// OPR description
			keyIdentifier = Registry.ClassesRoot.CreateSubKey(regOprIdentifier);
			keyIdentifier.SetValue(null, regOprDescription);				

			// OPR default icon
			keyDefaultIcon = keyIdentifier.CreateSubKey("Defaulticon");
			keyDefaultIcon.SetValue(null, applicationPath + ",0");
			keyDefaultIcon.Close();

			// OPR open shell command
			subKeyShell = keyIdentifier.CreateSubKey("shell");				
			subKeyOpen = subKeyShell.CreateSubKey("open");				
			subKeyCommand = subKeyOpen.CreateSubKey("command");				
			subKeyCommand.SetValue(null, "\"" + applicationPath + "\" /opr \"%1\"");
			subKeyShell.Close();
			subKeyCommand.Close();
			subKeyOpen.Close();
				
			keyIdentifier.Close();


			// OMI extension
			keyExtension = Registry.ClassesRoot.CreateSubKey(regOmiExtension);
			keyExtension.SetValue(null, regOmiIdentifier);
			keyExtension.Close();

			// OMI description
			keyIdentifier = Registry.ClassesRoot.CreateSubKey(regOmiIdentifier);
			keyIdentifier.SetValue(null, regOmiDescription);				

			// OMI default icon
			keyDefaultIcon = keyIdentifier.CreateSubKey("Defaulticon");
			keyDefaultIcon.SetValue(null, applicationPath + ",0");
			keyDefaultIcon.Close();

			// OMI open shell command
			subKeyShell = keyIdentifier.CreateSubKey("shell");				
			subKeyOpen = subKeyShell.CreateSubKey("open");				
			subKeyCommand = subKeyOpen.CreateSubKey("command");				
			subKeyCommand.SetValue(null, "\"" + applicationPath + "\" /omi \"%1\"");
			subKeyShell.Close();
			subKeyCommand.Close();
			subKeyOpen.Close();
				
			keyIdentifier.Close();

			Registry.ClassesRoot.Flush();
		}

		/// <summary>
		/// Determines whether OPR and OMI file extension are registered in Win32 registry
		/// to be opened with specific OmiEd application.
		/// </summary>
		/// <param name="applicationPath">Path to specific OmiEd application executable.</param>
		/// <returns>If OPR and OMI extensions are correctly registered, returns <c>true</c>,
		/// otherwise returns <c>false</c>.</returns>
		public static bool AreFileExtensionsRegistered(string applicationPath)
		{
			RegistryKey keyExtension, keyDefaultIcon, keyIdentifier,
				subKeyShell, subKeyOpen, subKeyCommand;

			// OPR extension
			keyExtension = Registry.ClassesRoot.OpenSubKey(regOprExtension);
			if( keyExtension==null )
				return( false );
			if( (string)keyExtension.GetValue(null) != regOprIdentifier )
				return( false );

			// OPR description
			keyIdentifier = Registry.ClassesRoot.OpenSubKey(regOprIdentifier);
			if( keyIdentifier==null )
				return( false );
			if( (string)keyIdentifier.GetValue(null) != regOprDescription )
				return( false );			

			// OPR default icon
			keyDefaultIcon = keyIdentifier.OpenSubKey("Defaulticon");
			if( keyDefaultIcon==null )
				return( false );
			if( (string)keyDefaultIcon.GetValue(null) != applicationPath + ",0" )
				return( false );

			// OPR open shell command
			subKeyShell = keyIdentifier.OpenSubKey("shell");
			if( subKeyShell==null )
				return( false );
	
			subKeyOpen = subKeyShell.OpenSubKey("open");
			if( subKeyOpen==null )
				return( false );
	
			subKeyCommand = subKeyOpen.OpenSubKey("command");
			if( subKeyCommand==null )
				return( false );
			if( (string)subKeyCommand.GetValue(null) != "\"" + applicationPath + "\" /opr \"%1\"" )
				return( false );


			// OMI extension
			keyExtension = Registry.ClassesRoot.OpenSubKey(regOmiExtension);
			if( keyExtension==null )
				return( false );
			if( (string)keyExtension.GetValue(null) != regOmiIdentifier )
				return( false );

			// OMI description
			keyIdentifier = Registry.ClassesRoot.OpenSubKey(regOmiIdentifier);
			if( keyIdentifier==null )
				return( false );
			if( (string)keyIdentifier.GetValue(null) != regOmiDescription )
				return( false );			

			// OMI default icon
			keyDefaultIcon = keyIdentifier.OpenSubKey("Defaulticon");
			if( keyDefaultIcon==null )
				return( false );
			if( (string)keyDefaultIcon.GetValue(null) != applicationPath + ",0" )
				return( false );

			// OMI open shell command
			subKeyShell = keyIdentifier.OpenSubKey("shell");
			if( subKeyShell==null )
				return( false );
	
			subKeyOpen = subKeyShell.OpenSubKey("open");
			if( subKeyOpen==null )
				return( false );
	
			subKeyCommand = subKeyOpen.OpenSubKey("command");
			if( subKeyCommand==null )
				return( false );
			if( (string)subKeyCommand.GetValue(null) != "\"" + applicationPath + "\" /omi \"%1\"" )
				return( false );

			return( true );
		}


		/// <summary>
		/// Discards any OPR and OMI file extension registration from Win32 registry.
		/// </summary>
		public static void UnregisterFileExtensions( )
		{			
			Registry.ClassesRoot.DeleteSubKeyTree( regOprExtension );
			Registry.ClassesRoot.DeleteSubKeyTree( regOprIdentifier );
			Registry.ClassesRoot.DeleteSubKeyTree( regOmiExtension );
			Registry.ClassesRoot.DeleteSubKeyTree( regOmiIdentifier );
		}


		#region Workarround to handle a bug from Microsoft
		//  A bug, see http://dturini.blogspot.com/2004_08_01_dturini_archive.html
		//  or  http://support.microsoft.com/default.aspx?scid=KB;EN-US;q326219#appliesto
		[DllImport("msvcr71.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int _controlfp(int n, int mask);
		const int _RC_NEAR       = 0x00000000;
		const int _PC_53         = 0x00010000;
		const int _EM_INVALID    = 0x00000010;
		const int _EM_UNDERFLOW  = 0x00000002;
		const int _EM_ZERODIVIDE = 0x00000008;
		const int _EM_OVERFLOW   = 0x00000004;
		const int _EM_INEXACT    = 0x00000001;
		const int _EM_DENORMAL   = 0x00080000;
		const int _CW_DEFAULT    = (  _RC_NEAR + _PC_53 
			+ _EM_INVALID 
			+ _EM_ZERODIVIDE
			+ _EM_OVERFLOW 
			+ _EM_UNDERFLOW
			+ _EM_INEXACT
			+ _EM_DENORMAL);
    
		/// <summary>
		/// Resets floating point unit (FPU).
		/// </summary>
		public static void ResetFPU()
		{ 
			_controlfp(_CW_DEFAULT ,0xfffff);
		} 

		#endregion
	}
}
