#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HydroDesktop.Interfaces.ObjectModel;

#endregion

namespace HydroDesktop.WebServices
{
	/// <summary>
	/// Communicates with an HIS Server that is configured with a HISServerCapabilities service
	/// </summary>
	public class HydroServerClient : WebServiceClientBase
	{
		#region Variables

		private string _asmxUrl;

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new instance of HIS Server web service client
		/// </summary>
		/// <param name="asmxUrl">The URL of the HIS Server with the HISServerCapabilities service</param>
		public HydroServerClient ( string asmxUrl )
			: base ( asmxUrl )
		{
			_asmxUrl = asmxUrl;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Parse the response from a GetWaterOneFlowServices request to HIS Server.  The response should be a string formatted as XML
		/// </summary>
		/// <param name="xmlResponse">Response from the web service</param>
		/// <returns>List of DataServiceInfo objects for each WaterOneFlow web service found</returns>
		private IList<DataServiceInfo> ParseGetWaterOneFlowServicesResponse ( string xmlResponse )
		{
			XmlDocument xmlDoc = new XmlDocument ();
			xmlDoc.LoadXml ( xmlResponse );

			XmlNodeList records = xmlDoc.SelectNodes ( "//Record" );

			List<DataServiceInfo> resultList = new List<DataServiceInfo> ();

			foreach ( XmlNode record in records )
			{
				// WaterOneFlow Service URL
				XmlNode node = record.SelectSingleNode ( "WaterOneFlowWSDL" );
				string asmxUrl = String.Empty;
				if ( node != null )
				{
					asmxUrl = node.InnerText;
				}
				int index = asmxUrl.IndexOf ( "?" );
				if ( index > -1 )
				{
					asmxUrl = asmxUrl.Substring ( 0, index );
				}
				if ( asmxUrl == String.Empty )
				{
					continue;
				}

				// Title
				node = record.SelectSingleNode ( "Title" );
				string title = "(title)";
				if ( node != null && node.InnerText != String.Empty )
				{
					title = node.InnerText;
				}

				// Abstract
				node = record.SelectSingleNode ( "Abstract" );
				string serviceAbstract = String.Empty;
				if ( node != null )
				{
					serviceAbstract = node.InnerText;
				}

				// Citation
				node = record.SelectSingleNode ( "Citation" );
				string citation = String.Empty;
				if ( node != null )
				{
					citation = node.InnerText;
				}

				// Contact Email
				node = record.SelectSingleNode ( "DatasetContact/EmailAddress" );
				string contactEmail = String.Empty;
				if ( node != null )
				{
					contactEmail = node.InnerText;
				}

				// Contact Name
				node = record.SelectSingleNode ( "DatasetContact/FirstName" );
				string contactName = String.Empty;
				if ( node != null )
				{
					contactName = node.InnerText;
				}
				node = record.SelectSingleNode ( "DatasetContact/LastName" );
				if ( node != null )
				{
					if ( contactName != String.Empty )
					{
						contactName += " " + node.InnerText;
					}
					else
					{
						contactName = node.InnerText;
					}
				}

				// North
				node = record.SelectSingleNode ( "NorthExtent" );
				double north = 90;
				if ( node != null )
				{
					if ( Double.TryParse ( node.InnerText, out north ) == false )
					{
						north = 90;
					}
				}

				// South
				node = record.SelectSingleNode ( "SouthExtent" );
				double south = -90;
				if ( node != null )
				{
					if ( Double.TryParse ( node.InnerText, out south ) == false )
					{
						south = -90;
					}
				}

				// East
				node = record.SelectSingleNode ( "EastExtent" );
				double east = 180;
				if ( node != null )
				{
					if ( Double.TryParse ( node.InnerText, out east ) == false )
					{
						east = 180;
					}
				}

				// West
				node = record.SelectSingleNode ( "WestExtent" );
				double west = -180;
				if ( node != null )
				{
					if ( Double.TryParse ( node.InnerText, out west ) == false )
					{
						west = -180;
					}
				}

				// Add the service info to the list
				DataServiceInfo serviceInfo = new DataServiceInfo ( asmxUrl, title );
				serviceInfo.Abstract = serviceAbstract;
				serviceInfo.Citation = citation;
				serviceInfo.ContactEmail = contactEmail;
				serviceInfo.ContactName = contactName;
				serviceInfo.EastLongitude = east;
				serviceInfo.WestLongitude = west;
				serviceInfo.NorthLatitude = north;
				serviceInfo.SouthLatitude = south;

				resultList.Add ( serviceInfo );
			}

			return resultList;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The URL address of the web service being used
		/// </summary>
		public string ServiceUrl
		{
			get { return _asmxUrl; }
		}

		#endregion

		#region Public Methods

        /// <summary>
        /// Gets the list of WaterOneFlow services on the HydroServer
        /// </summary>
        /// <returns>the list of available WOF services on the server</returns>
		public IList<DataServiceInfo> GetWaterOneFlowServices ()
		{
			//string result = (string)CallWebMethod ( "GetWaterOneFlowServices", null );

			//List<DataServiceInfo> resultList = (List<DataServiceInfo>)ParseGetWaterOneFlowServicesResponse ( result );
			//return resultList;
            return null;
		}

		#endregion
	}
}
