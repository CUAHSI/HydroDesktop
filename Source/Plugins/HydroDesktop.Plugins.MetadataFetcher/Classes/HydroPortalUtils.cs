using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.MetadataFetcher
{
	public class HydroPortalUtils
	{
		#region Variables

		private string _baseUrl;
		private int _maxRecords;

		#endregion

		#region Constructor

		public HydroPortalUtils ( string portalUrl )
		{
			_baseUrl = GetBasePortalUrl ( portalUrl );
			_maxRecords = 100;
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Returns the base URL of the HydroPortal.  This is required in order to properly construct REST queries on the portal.
		/// </summary>
		/// <param name="portalUrl">Original HydroPortal URL, e.g., https://hydroportal.crwr.utexas.edu/geoportal/catalog/main/home.page or https://hydroportal.crwr.utexas.edu/geoportal</param>
		/// <returns>Base HydroPortal URL, e.g., https://hydroportal.crwr.utexas.edu/geoportal</returns>
		private string GetBasePortalUrl ( string portalUrl )
		{
			// Create a URI object from the given portal URL.  This accounts for possible redirect.
			Uri uri = WebOperations.GetUri ( portalUrl, true );
			string baseUrl = uri.ToString ();

			// Trim the home page fluff
			int index = baseUrl.ToLower ().IndexOf ( "geoportal/catalog" );
			if ( index > -1 )
			{
				baseUrl = portalUrl.Substring ( 0, index + 9 );
			}

			// Trim current searches
			index = baseUrl.IndexOf ( "geoportal/rest" );
			if ( index > -1 )
			{
				baseUrl = portalUrl.Substring ( 0, index + 9 );
			}

			// Trim parameters
			index = baseUrl.IndexOf ( "?" );
			if ( index > -1 )
			{
				baseUrl = portalUrl.Substring ( 0, index );
			}

			// Make sure this is still a valid URL
			if ( !WebOperations.IsUrlValid ( baseUrl, false ) )
			{
				baseUrl = uri.ToString (); // Revert back to the URI that worked
			}

			// Remove trailing backslash
			index = baseUrl.LastIndexOf ( "/" );
			if ( index == (baseUrl.Length - 1) )
			{
				baseUrl = baseUrl.Substring ( 0, baseUrl.Length - 1 );
			}

			return baseUrl;
		}

		/// <summary>
		/// Parses a HydroPortal page for a WaterOneFlow service registered using the FGDC metadata standard.  
		/// </summary>
		/// <param name="pageUrl">The URL to the HydroPortal page describing the service</param>
		/// <returns>If the page describes a WaterOneFlow service, a DataServiceInfo object describing the service is returned</returns>
		private DataServiceInfo ParseFgdcPage ( string pageUrl )
		{
			DataServiceInfo serviceInfo = new DataServiceInfo ();

			try
			{
				using ( XmlTextReader reader = new XmlTextReader ( pageUrl ) )
				{
					while ( reader.Read () )
					{
						if ( reader.NodeType == XmlNodeType.Element )
						{
							// Abstract
							if ( reader.Name.ToLower () == "abstract" )
							{
								reader.Read ();
								serviceInfo.Abstract = reader.Value;
							}

							// Citation
							else if ( reader.Name.ToLower () == "citation" )
							{
								string origin = "(unknown author)";
								string title = "(unknown title)";
								string pubplace = "(unknown organization)";
								string pubdate = "unknown date";
								string link = "(unknown link)";

								while ( reader.Read () )
								{
									if ( reader.NodeType == XmlNodeType.Element )
									{
										if ( reader.Name.ToLower () == "origin" )
										{
											reader.Read ();
											if ( reader.Value.Trim () != String.Empty )
											{
												origin = reader.Value;
											}
										}
										else if ( reader.Name.ToLower () == "title" )
										{
											reader.Read ();
											if ( reader.Value.Trim () != String.Empty )
											{
												title = reader.Value;
											}
										}
										else if ( reader.Name.ToLower () == "pubplace" )
										{
											reader.Read ();
											if ( reader.Value.Trim () != String.Empty )
											{
												pubplace = reader.Value;
											}
										}
										else if ( reader.Name.ToLower () == "pubdate" )
										{
											reader.Read ();
											if ( reader.Value.Trim () != String.Empty )
											{
												pubdate = reader.Value;
											}
										}
										else if ( reader.Name.ToLower () == "onlink" )
										{
											reader.Read ();
											if ( reader.Value.Trim () != String.Empty )
											{
												link = reader.Value;
											}
										}
									}
									else if ( reader.NodeType == XmlNodeType.EndElement && reader.Name == "citation" )
									{
										break;
									}
								}
								serviceInfo.Citation = origin + ". " + title + ". " + pubdate + ". " + pubplace + ". <" + link + ">.";
							}

							// Contact Name and Email
							else if ( reader.Name.ToLower () == "ptcontac" )
							{
								string contactName = "";
								string contactEmail = "";

								while ( reader.Read () )
								{
									if ( reader.NodeType == XmlNodeType.Element )
									{
										if ( reader.Name.ToLower () == "cntper" )
										{
											reader.Read ();
											contactName = reader.Value;
										}
										else if ( reader.Name.ToLower () == "cntemail" )
										{
											reader.Read ();
											contactEmail = reader.Value;
										}
									}
									else if ( reader.NodeType == XmlNodeType.EndElement && reader.Name == "ptcontac" )
									{
										break;
									}
								}

								serviceInfo.ContactEmail = contactEmail;
								serviceInfo.ContactName = contactName;
							}

							// Bounding Box
							else if ( reader.Name == "bounding" )
							{
								double west = 0;
								double east = 0;
								double north = 0;
								double south = 0;

								while ( reader.Read () )
								{
									if ( reader.NodeType == XmlNodeType.Element )
									{
										if ( reader.Name.ToLower () == "westbc" )
										{
											reader.Read ();
											double.TryParse ( reader.Value, out west );
										}
										else if ( reader.Name.ToLower () == "eastbc" )
										{
											reader.Read ();
											double.TryParse ( reader.Value, out east );
										}
										else if ( reader.Name.ToLower () == "northbc" )
										{
											reader.Read ();
											double.TryParse ( reader.Value, out north );
										}
										else if ( reader.Name.ToLower () == "southbc" )
										{
											reader.Read ();
											double.TryParse ( reader.Value, out south );
										}
									}
									else if ( reader.NodeType == XmlNodeType.EndElement && reader.Name == "bounding" )
									{
										break;
									}
								}

								serviceInfo.EastLongitude = east;
								serviceInfo.WestLongitude = west;
								serviceInfo.NorthLatitude = north;
								serviceInfo.SouthLatitude = south;
							}

							// Service Code
							else if ( reader.Name == "ServiceCode" )
							{
								reader.Read ();
								serviceInfo.ServiceCode = reader.Value;
							}

							// Service Endpoint URL
							else if ( reader.Name == "WaterOneFlowURL" )
							{
								reader.Read ();
								serviceInfo.EndpointURL = WebOperations.GetCanonicalUri ( reader.Value, true );
							}

							// Service Description URL
							else if ( reader.Name == "MapServiceURL" )
							{
								reader.Read ();
								serviceInfo.DescriptionURL = reader.Value;
							}

							// Service Title
							else if ( reader.Name == "ServiceName" )
							{
								reader.Read ();
								serviceInfo.ServiceTitle = reader.Value;
							}
						}
					}
				}

			}
			catch ( Exception ex )
			{
				throw new WebException ( "Could not read XML response from " + pageUrl + ".\n" + ex.Message );
			}

			return serviceInfo;
		}

		/// <summary>
		/// Parses a HydroPortal page for a WaterOneFlow service registered using the Dublin Core metadata standard.  
		/// </summary>
		/// <param name="pageUrl">The URL to the HydroPortal page describing the service</param>
		/// <returns>If the page describes a WaterOneFlow service, a DataServiceInfo object describing the service is returned</returns>
		private DataServiceInfo ParseDublinCorePage ( string pageUrl )
		{
			DataServiceInfo serviceInfo = new DataServiceInfo ();

			try
			{
				using ( XmlTextReader reader = new XmlTextReader ( pageUrl ) )
				{
					while ( reader.Read () )
					{
						if ( reader.NodeType == XmlNodeType.Element )
						{
							// Abstract
							if ( reader.Name.ToLower () == "dc:description" )
							{
								reader.Read ();
								serviceInfo.Abstract = reader.Value;
							}

							// Bounding Box
							else if ( reader.Name == "ows:WGS84BoundingBox" )
							{
								double west = 0;
								double east = 0;
								double north = 0;
								double south = 0;

								while ( reader.Read () )
								{
									if ( reader.NodeType == XmlNodeType.Element )
									{
										if ( reader.Name == "ows:LowerCorner" )
										{
											reader.Read ();
											string[] lowerCorner = reader.Value.Split ( ' ' );
											if ( lowerCorner.Length == 2 )
											{
												double.TryParse ( lowerCorner[0], out west );
												double.TryParse ( lowerCorner[1], out south );
											}
										}
										else if ( reader.Name == "ows:UpperCorner" )
										{
											reader.Read ();
											string[] upperCorner = reader.Value.Split ( ' ' );
											if ( upperCorner.Length == 2 )
											{
												double.TryParse ( upperCorner[0], out east );
												double.TryParse ( upperCorner[1], out north );
											}
										}
									}
									else if ( reader.NodeType == XmlNodeType.EndElement && reader.Name == "ows:WGS84BoundingBox" )
									{
										break;
									}
								}

								serviceInfo.EastLongitude = east;
								serviceInfo.WestLongitude = west;
								serviceInfo.NorthLatitude = north;
								serviceInfo.SouthLatitude = south;
							}

							// Service Code
							else if ( reader.Name.ToLower () == "dc:title" )
							{
								reader.Read ();
								serviceInfo.ServiceCode = reader.Value;
							}

							// Service Endpoint URL
							else if ( reader.Name.ToLower () == "dc:creator" )
							{
								reader.Read ();
								serviceInfo.EndpointURL = WebOperations.GetCanonicalUri ( reader.Value, true );
							}

							// Service Description URL
							else if ( reader.Name == "dct:references" )
							{
								reader.Read ();
								serviceInfo.DescriptionURL = reader.Value;
							}

							// Service Title
							else if ( reader.Name.ToLower () == "dct:alternative" )
							{
								reader.Read ();
								serviceInfo.ServiceTitle = reader.Value;
							}
						}
					}
				}

			}
			catch ( Exception ex )
			{
				throw new WebException ( "Could not read XML response from " + pageUrl + ".\n" + ex.Message );
			}

			return serviceInfo;
		}

		#endregion

		#region Public Members

		#region Properties

		public int MaxRecordsToRetrieve
		{
			get
			{
				return _maxRecords;
			}
			set
			{
				if ( value <= 0 )
				{
					throw new Exception ( "Value must be greater than zero" );
				}
				else if ( value > 100 )
				{
					throw new Exception ( "Value must be less than or equal to 100" );
				}
				else
				{
					_maxRecords = value;
				}
			}
		}

		public string HydroPortalUrl
		{
			get
			{
				return _baseUrl;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the unique IDs of items registered with the HydroPortal
		/// </summary>
		/// <returns>List of unique IDs of items registered with the HydroPortal</returns>
		public List<string> GetRegisteredItemIds ()
		{
			List<string> idList = new List<string> ();

			// Add the arguments to search for all item registered with the portal
			string url = _baseUrl + "/rest/find/document?f=atom&max=" + _maxRecords.ToString (); // atom search returns the least bytes of data among the portal search types

			// Retrieve the search results 
			try
			{
				using ( XmlTextReader reader = new XmlTextReader ( url ) )
				{
					while ( reader.Read () )
					{
						// Look for <entry> elements with child <id> elements like <id>urn:uuid:73CBDC5C-7559-4FD9-B510-AC6EFF37C751</id> 
						if ( reader.NodeType == XmlNodeType.Element )
						{
							if ( reader.Name.ToLower () == "entry" )
							{
								while ( reader.Read () )
								{
									if ( reader.NodeType == XmlNodeType.Element )
									{
										if ( reader.Name.ToLower () == "id" )
										{
											reader.Read ();

											// Get the part of the ID past the last colon
											string idText = reader.Value.Trim ();

											int index = idText.LastIndexOf ( ":" );
											if ( index > -1 )
											{
												idText = idText.Substring ( index + 1 );
											}

											// Add to the list of IDs
											if ( idText != String.Empty )
											{
												idList.Add ( idText );
												break;
											}
										}
									}
									else if ( reader.NodeType == XmlNodeType.EndElement && reader.Name == "entry" )
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			catch ( Exception ex )
			{
				throw new WebException ( "Could not read XML response from " + url + ".\n" + ex.Message );
			}

			return idList;
		}

		public DataServiceInfo ReadWaterOneFlowServiceInfo ( string portalPageId )
		{
			DataServiceInfo serviceInfo = null;

			// Create the URL to the descriptive info about the registered item on the HydroPortal
			string url = _baseUrl + "/rest/document?id={" + portalPageId + "}";

			// Determine if this is registered using the FGDC standard or Dublin Core standard, or neither
			string response = WebOperations.DownloadASCII ( url );
			int index = response.IndexOf ( "<ServiceType>CUAHSIService</ServiceType>" );
			if ( index > -1 )
			{
				// FGDC Standard
				index = response.IndexOf ( "<metstdn>FGDC Content Standards for Digital Geospatial</metstdn>" );
				if ( index > -1 )
				{
					index = response.IndexOf ( "<WaterOneFlowURL>" );
					if ( index > -1 )
					{
						serviceInfo = ParseFgdcPage ( url );
					}
				}
			}
			//else
			//{
			//    index = response.IndexOf ( "<dc:contributor>The Dublin Core Metadata Initiative</dc:contributor>" );
			//    if ( index > -1 )
			//    {
			//        // Dublin Core Standard
			//        string detailsUrl = _baseUrl + "/catalog/search/viewMetadataDetails.page?uuid={" + portalPageId + "}";

			//        response = WebOperations.DownloadASCII ( detailsUrl );
			//        index = response.IndexOf ( "<td class=\"parameterLabel\">WaterOneFlow URL</td>" );

			//        if ( index > -1 )
			//        {
			//            serviceInfo = ParseDublinCorePage ( url );
			//        }
			//    }
			//}

			return serviceInfo;
		}

		#endregion

		#endregion
	}
}
