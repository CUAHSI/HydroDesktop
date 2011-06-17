using System;
using System.Diagnostics;
using System.Net;
//using Microsoft.Win32; // Not supported in Mono.

namespace HydroDesktop.Help
{
	public class WebUtilities
	{
		#region Public Methods

		/// <summary>
		/// Tests for an internet connection.
		/// </summary>
		/// <returns>True if internet connection found; false otherwise.</returns>
		public static bool IsInternetAvailable ()
		{
			// Test a few sites for connectivity.
			string[] sites = { "www.bing.com", "www.google.com" };

			for ( int i = 0; i < sites.Length; i++ )
			{
				try
				{
					// This works even if the site is down because it just tries to reach one of several ISP DNS servers 
					// to resolve the address and get the name, which ping also does even before sending the packets. 
					IPHostEntry ipHostEntry = Dns.GetHostEntry ( sites[i] );
					return true;
				}
				catch
				{
				}
			}

			// If we made it this far, then no tests were successful.
			return false;
		}

		/// <summary>
		/// Retrieves the user's default Web browser.
		/// </summary>
		/// <returns>The full path to the default Web browser, or string.Empty if the browser could not be determined.</returns>
		public static string GetSystemDefaultBrowser ()
		{
			string browser = string.Empty;

			// Not supported in Mono.
			//RegistryKey regKey = null;

			//try
			//{
			//    regKey = Registry.ClassesRoot.OpenSubKey ( "HTTP\\shell\\open\\command", false );

			//    //get rid of the enclosing quotes
			//    browser = regKey.GetValue ( null ).ToString ().ToLower ().Replace ( "" + (char)34, "" );

			//    //check to see if the value ends with .exe (this way we can remove any command line arguments)
			//    if ( !browser.EndsWith ( "exe" ) )
			//    {
			//        //get rid of all command line arguments (anything after the .exe must go)
			//        browser = browser.Substring ( 0, browser.LastIndexOf ( ".exe" ) + 4 );
			//    }
			//}
			//finally
			//{
			//    if ( regKey != null )
			//    {
			//        regKey.Close ();
			//    }
			//}

			return browser;
		}

		/// <summary>
		/// Opens the given URI using the default Web browser.
		/// </summary>
		/// <param name="uriString">The URI to open.</param>
		public static void OpenUri ( string uriString )
		{
			// Attempt to create a Uri from the input string.  This also validates the format of the Uri string.
			Uri uri = new Uri ( uriString.Trim () );

			// Try to open the Uri using the default Web browser.
			string browser = GetSystemDefaultBrowser ();
			if ( browser != "" )
			{
				Process process = new Process ();
				process.StartInfo.FileName = browser;
				process.StartInfo.Arguments = uri.AbsoluteUri;
				process.Start ();
			}
			else
			{
				// We could not identify the default browser, so just try opening the link directly.
				Process.Start ( uri.AbsoluteUri );
			}
		}

		/// <summary>
		/// Opens the default program for handling the given mailto link.
		/// </summary>
		/// <param name="mailtoLink">The mailto link to open.</param>
		public static void OpenMailtoLink ( string mailtoLink )
		{
			string link = mailtoLink.Trim ();
			if ( link.Substring ( 0, 7 ).ToLower () != "mailto:" )
			{
				throw new ArgumentException ( "Invalid mailto link format." );
			}
			else
			{
				Process.Start ( mailtoLink );
			}
		}

		#endregion
	}
}
