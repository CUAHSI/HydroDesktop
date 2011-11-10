using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HydroDesktop.Help
{
	public class LocalHelp
	{
		#region Private Member Variables

		private static readonly string _helpRelativePath = Properties.Settings.Default.helpRelativePath;

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the directory in which HydroDesktop help files are located.
		/// </summary>
		/// <returns>The path to the directory in which HydroDesktop help files are located.</returns>
		public static string GetHelpPath ()
		{
			string hydroDesktopFolder = AppDomain.CurrentDomain.BaseDirectory;
			string helpPath = Path.Combine ( hydroDesktopFolder, _helpRelativePath );
			return helpPath;
		}

		/// <summary>
		/// Opens a help topic from the HydroDesktop help system using the default program for viewing HTML files.
		/// </summary>
		/// <param name="RelativeFileLocation">The location of the file to open, relative to the HydroDesktop Help directory, and including the .html extension, e.g., Printing.html, Extensions/HydroR/HydroR.html.</param>
		public static void OpenHelpFile ( string RelativeFileLocation )
		{
			string rootHelpPath = GetHelpPath ();

			if ( Directory.Exists ( rootHelpPath ) == false )
			{
				throw new Exception ( "Could not open help file. The system cannot find the directory at '" + rootHelpPath + "'." );
			}

			string helpFilePath = Path.Combine ( rootHelpPath, RelativeFileLocation );

			if ( File.Exists ( helpFilePath ) == false )
			{
				throw new ArgumentException ( "Could not open help file. File at '" + helpFilePath + "' does not exist.", "RelativeFileLocation" );
			}
			else
			{
				WebUtilities.OpenUri ( helpFilePath );
			}
		}

		#endregion
	}
}
