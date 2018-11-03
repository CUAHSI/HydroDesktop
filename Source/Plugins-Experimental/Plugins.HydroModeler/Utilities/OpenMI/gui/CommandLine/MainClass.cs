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
using System.Threading;

using OpenMI.Standard;
using Oatc.OpenMI.Gui.Core;
using System.Diagnostics;

namespace Oatc.OpenMI.Gui.CommandLine
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class MainClass
	{
		private static int _exitCode = 0;

		public static void WriteCaption()
		{
			Console.WriteLine("OmiEd for command-line. Copyright(c) 2005-2006 OpenMI Association.\n");
		}

		public static void WriteUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("  OmiEd_cmd.exe -r OPR_FILE [-v] [-mta]");
			Console.WriteLine("    -r OPR_FILE   Runs simulation of specified OmiEd project.");
			Console.WriteLine("    -v            Verbose mode off.");
			Console.WriteLine("    -mta          Application creates and enters a multi-threaded apartment COM model at startup.");
			Console.WriteLine("    -help         Shows this help.");
		}

		public static void ProceedCommandLineArgs(string[] args)
		{
			bool mta = false;
			bool verboseOff = false;
			string oprFilename = null;

			// read command-line options
			for( int i=0; i<args.Length; i++ )
				switch( args[i].ToLower() )
				{
					case "-v":
					case "/v":
						verboseOff = true;
						break;

					case "-r":
					case "/r":
						if( args.Length <= i+1 )
						{
							Console.WriteLine("Error: -r switch must be followed by filename.");
							_exitCode = 1;
							return;
						}
						oprFilename = args[i+1];

						i++;						
						break;

					case "-mta":
					case "/mta":
						mta = true;
						break;

					case "-help":
					case "/help":
					case "-?":
					case "/?":
					case "--help":
					case "-h":
					case "/h":
						WriteUsage();
						_exitCode = 0;
						return;

					default:
						Console.WriteLine("Unknown command-line option: "+args[i]);
						_exitCode = 2;
						return;
				}

			// set apartment state 
			// VS2005 fix
			// In VS2005 the main thread uses MTA by default, so we have to create new thread,
			// which will run the application, and set it's appartment state before it's started

			Thread thread = new Thread( new ParameterizedThreadStart( RunApplication ) );			

			if( mta )
				thread.SetApartmentState( ApartmentState.MTA );
			else
				thread.SetApartmentState( ApartmentState.STA );

			thread.Start( new object[] { verboseOff, oprFilename } );
			thread.Join();
		}

		private static void RunApplication( object data )
		{
			try
			{
				bool verboseOff = (bool)( (object[]) data )[0];
				string oprFilename = (string) ( (object[]) data )[1];

				// check whether opr file exists
				if( oprFilename==null )
				{
					Console.WriteLine( "Error: -r switch was not specified." );
					_exitCode = 3;
					return;
				}

				FileInfo fileInfo = new FileInfo( oprFilename );
				if( !fileInfo.Exists )
				{
					Console.WriteLine( "Error: cannot find input file "+oprFilename );
					_exitCode = 4;
					return;
				}


				// open OPR
				CompositionManager composition = new CompositionManager();

				if( !verboseOff )
					Console.WriteLine( "Loading project file "+fileInfo.FullName+"..." );
				composition.LoadFromFile( fileInfo.FullName );


				// prepare listeners
				if( !verboseOff )
					Console.WriteLine( "Preparing listener(s)..." );
				ArrayList listOfListeners = new ArrayList();

				// logfile listener
				if( composition.LogToFile!=null && composition.LogToFile!="" )
				{
					// get composition file's directory to logfile is saved in same directory
					string logFileName = Utils.GetFileInfo( fileInfo.DirectoryName, composition.LogToFile ).FullName;
					LogFileListener logFileListener = new LogFileListener( composition.ListenedEventTypes, logFileName );
					listOfListeners.Add( logFileListener );
				}

				// console listener
				if( !verboseOff )
				{
					ConsoleListener consoleListener = new ConsoleListener( composition.ListenedEventTypes );
					listOfListeners.Add( consoleListener );
				}

				// create proxy listener
				ProxyListener proxyListener = new ProxyListener();
				proxyListener.Initialize( listOfListeners );

				// run simulation
				if( !verboseOff )
					Console.WriteLine( "Starting composition run..." );
				composition.Run( proxyListener, true );

				if( !verboseOff )
					Console.WriteLine( "Closing composition..." );
				composition.Release();

				_exitCode = 0;
			}
			catch( Exception e )
			{
				Console.WriteLine( "Exception occured: " + e.ToString() );
				_exitCode = -2;
				return;
			}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>		
		static int Main(string[] args)
		{
			WriteCaption();
			
			try 
			{
				ProceedCommandLineArgs(args);
			}
			catch( Exception e )
			{
				Console.WriteLine("Exception occured while initiating the application: " + e.ToString() );
				_exitCode = -1;
			}

			return ( _exitCode );
		}
	}
}
