using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HydroDesktop.MetadataFetcher
{
	class WebOperations
	{
		#region Public Members

		/// <summary>
		/// Given a string defining a URI, creates a URI object from the string.  If the URI is redirected, returns a URI object created from the redirected location.
		/// </summary>
		/// <param name="uriString">String defining a URI</param>
		/// <param name="trimParameters">true if URL parameters (the part after the question mark) should be trimmed from the URL; false otherwise</param>
		/// <returns>URI object created from the URI string</returns>
		public static Uri GetUri ( string uriString, bool trimParameters )
		{
			if ( trimParameters == true )
			{
				int index = uriString.IndexOf ( "?" );

				if ( index > -1 )
				{
					// Trim the query off of the URL
					uriString = uriString.Substring ( 0, index );
				}
			}

			// Connect to the URL
			Uri urlCheck = null;
			urlCheck = new Uri ( uriString );

			WebRequest request = WebRequest.Create ( urlCheck );
			request.Timeout = 15000; // Wait 15 seconds

			string responseUrl;
			// Get URL web response.  
			// "using" properly disposes of web response; otherwise, the same URL could be blocked from subsequent requests.
			using ( WebResponse response = request.GetResponse () )
			{
				responseUrl = response.ResponseUri.ToString ();
			}

			return new Uri ( responseUrl );
		}

		/// <summary>
		/// Check that the given URL string points to a valid URL.
		/// </summary>
		/// <param name="url">The URL to check</param>
		/// <param name="trimParameters">true if URL parameters (the part after the question mark) should be trimmed from the URL when validating; false otherwise</param>
		/// <returns></returns>
		public static bool IsUrlValid ( string url, bool trimParameters )
		{
			// Check the formatting of the URL
			if ( url == String.Empty || url == null)
			{
				return false;
			}

			if ( trimParameters == true )
			{
				int index = url.IndexOf ( "?" );

				if ( index > -1 )
				{
					// Trim the query off of the URL
					url = url.Substring ( 0, index );
				}
			}

			// See if we can connect to the URL
			Uri urlCheck = null;
			try
			{
				urlCheck = new Uri ( url );
			}
			catch
			{
				return false;
			}

			WebRequest request = WebRequest.Create ( urlCheck );
			request.Timeout = 15000; // Wait 15 seconds

			string responseUrl;
			try
			{
				// Get URL web response.  
				// "using" properly disposes of web response; otherwise, the same URL could be blocked from subsequent requests.
				using ( WebResponse response = request.GetResponse () )
				{
					responseUrl = response.ResponseUri.ToString ();
				}
			}
			catch ( Exception )
			{
				return false;
			}

			// Check for error on redirect
			if ( string.Compare ( responseUrl, urlCheck.ToString (), true ) != 0 ) //it was redirected, check to see if redirected to error page
			{
				if ( (responseUrl.IndexOf ( "404.php" ) > -1 ||
					  responseUrl.IndexOf ( "500.php" ) > -1 ||
					  responseUrl.IndexOf ( "404.htm" ) > -1 ||
					  responseUrl.IndexOf ( "500.htm" ) > -1) )
				{
					return false;
				}
			}

			// If we made it this far, the URL is valid
			return true;
		}

        /// <summary>
        /// Determines if the format of a URL string is valid
        /// </summary>
        /// <param name="url">The URL string to check</param>
        /// <returns></returns>
        public static bool IsUrlFormatValid(string url)
        {
            try
            {
                Uri urlCheck = new Uri(url);
                return true;
            }
            catch 
            {
                return false;
            }
        }

		/// <summary>
		/// Returns the canonical form of the URL, i.e., "%20" becomes a space and so on.
		/// </summary>
		/// <param name="url">The URL to check</param>
		/// <param name="trimParameters">true if URL parameters (the part after the question mark) should be trimmed from the URL; false otherwise</param>
		/// <returns>The canonical form of the URL, i.e., "%20" becomes a space and so on.</returns>
		public static string GetCanonicalUri ( string url, bool trimParameters )
		{
			if ( trimParameters == true )
			{
				int index = url.IndexOf ( "?" );

				if ( index > -1 )
				{
					// Trim the query off of the URL
					url = url.Substring ( 0, index );
				}
			}

			Uri urlCheck = new Uri ( url );
			return urlCheck.ToString ();
		}

		/// <summary>
		/// Returns the content at a URL as ASCII text
		/// </summary>
		/// <param name="url">The URL to the resource that is to be retrieved</param>
		/// <returns>The content at the URL as ASCII text</returns>
		public static string DownloadASCII(string url) 
		{
			WebClient webClient = new WebClient();
			return System.Text.Encoding.ASCII.GetString(webClient.DownloadData(url));
		}

		#endregion
	}
}
