using System;
using System.Collections.Generic;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.MetadataFetcher
{
	class DatabaseOperations
	{
		#region Public Members

		/// <summary>
		/// Builds a list of Service Endpoint URLs from the metadata cache database
		/// </summary>
		/// <param name="trimUrlParameters">true if parameters (e.g., "?WSDL") should be trimmed from the URLs before adding to the output</param>
		/// <returns>List of Service Endpoint URLs from the metadata cache database</returns>
		public static List<string> GetCacheServiceUrls ( bool trimUrlParameters )
		{
			List<string> urls = new List<string> ();

            MetadataCacheManagerSQL cacheManager = GetCacheManager();

			List<DataServiceInfo> serviceList = cacheManager.GetAllServices () as List<DataServiceInfo>;

			foreach ( DataServiceInfo serviceInfo in serviceList )
			{
				string url = serviceInfo.EndpointURL.Trim ();

				if ( trimUrlParameters == true )
				{
					int index = url.IndexOf ( "?" );
					if ( index > -1 )
					{
						url = url.Substring ( 0, index );
					}
				}

				urls.Add ( url );
			}

			return urls;
		}

		public static bool CacheHasServiceUrl ( string serviceUrl )
		{
			// Trim the query off of the URL
			serviceUrl = serviceUrl.Trim ().ToLower ();
			int index = serviceUrl.IndexOf ( "?" );
			if ( index > -1 )
			{
				serviceUrl = serviceUrl.Substring ( 0, index );
			}

			// Get the services from the metadata cache database
            MetadataCacheManagerSQL cacheManager = GetCacheManager();
			List<DataServiceInfo> serviceList = cacheManager.GetAllServices () as List<DataServiceInfo>;

			// Compare service URLs
			foreach ( DataServiceInfo serviceInfo in serviceList )
			{
				string existingUrl = serviceInfo.EndpointURL.ToLower ().Trim ();

				index = existingUrl.IndexOf ( "?" );
				if ( index > -1 )
				{
					// Trim the query off of the URL
					existingUrl = existingUrl.Substring ( 0, index );
				}

				if ( serviceUrl == existingUrl )
				{
					return true;
				}
			}

			return false;
		}

		public static DataServiceInfo GetDataServiceFromCache ( string serviceUrl )
		{
			// Trim the query off of the URL
			string trimmedServiceUrl = serviceUrl.Trim ().ToLower ();
			int index = trimmedServiceUrl.IndexOf ( "?" );
			if ( index > -1 )
			{
				trimmedServiceUrl = trimmedServiceUrl.Substring ( 0, index );
			}

			// Get the services from the metadata cache database
			MetadataCacheManagerSQL cacheManager = GetCacheManager();
			List<DataServiceInfo> serviceList = cacheManager.GetAllServices () as List<DataServiceInfo>;

			// Compare service URLs
			foreach ( DataServiceInfo serviceInfo in serviceList )
			{
				string existingUrl = serviceInfo.EndpointURL.ToLower ().Trim ();

				// Trim the query off of the URL
				index = existingUrl.IndexOf ( "?" );
				if ( index > -1 )
				{
					existingUrl = existingUrl.Substring ( 0, index );
				}

				if ( trimmedServiceUrl == existingUrl )
				{
					return serviceInfo;
				}
			}

			// If we made it this far, a matching service wasn't found
			throw new Exception ( "Matching data service record not found for given service URL: " + serviceUrl );
		}

        public static MetadataCacheManagerSQL GetCacheManager()
        {
            return new MetadataCacheManagerSQL(DatabaseTypes.SQLite, HydroDesktop.Configuration.Settings.Instance.MetadataCacheConnectionString);
        }

		#endregion

	}
}
