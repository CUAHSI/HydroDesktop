using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using Search3.Searching.Exceptions;
using Search3.Settings;
using log4net;

namespace Search3.Searching
{
    public interface IProgressHandler
    {
        void ReportProgress(int persentage, object state);
        void CheckForCancel();
        void ReportMessage(string message);
    }

    /* this is deisgned to be partially testable, 
	 * and confiurable to use  HIScentrals other than ours
	 * Logging was also added
	 * * a list of endpoints is passed in (use HD Settings class)
	 * * The backgroundworker no longer displays a dialog box when a server fails
	 * * exception (HydrodesktopSearchException) is thrown when ALL servers fail.
	 * someone want to add a search is running slow event to searcher
	 * */
	public class BackgroundSearchWithFailover
	{
        //todo: Copied from Search2. Need to be refactored.

		private static readonly log4net.ILog log = LogManager.GetLogger ( System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType );

		public IHISCentralSearcher Searcher { get; set; }

        public void HISCentralSearchWithFailover(DoWorkEventArgs e, IList<string> endpoints, IProgressHandler bgWorker)
		{
			SearchCriteria parameters;
			//  ArrayList parameters = e.Argument as ArrayList;
			if ( !e.Argument.GetType ().Equals ( typeof ( SearchCriteria ) ) )
			{
				throw new ArgumentException ( "Bad Arugment should be of type SearchCriteria" );
			}
			else
			{
				parameters = (SearchCriteria)e.Argument;
			}
			
			var randomEndpoints = endpoints.OrderBy ( v => Guid.NewGuid () );

			StringBuilder errorStack = new StringBuilder ("HIS Central Search Error\n\n");
			foreach ( var url in randomEndpoints )
			{
				try
				{
					errorStack.Append ( "HIS Central URI: " + url.ToString () + "\n" );
					/* this in the only way I can think of injecting since HISCentral Searcher is
					dynamic (can only be created with a URL) and 
					 * there is really no interface definiton of what the search inteface is
					 * */
					IHISCentralSearcher searcher = Searcher ?? new HISCentralSearcher ( url );


					var worked = RunHisCentralSearch ( e, parameters, searcher, bgWorker );
					if ( worked ) return;
					// TODO: It seems like if we get to the code below, then the error is in search parameters (areaParameter), not HIS Central.
					errorStack.Append ( "RunHisCentralSearch returned false.\n\n" );
					log.Info ( "HIS Server " + url + " failed" );
					continue;
				}
				catch ( Exception ex )
				{
                    if (ex is OperationCanceledException)
                    {
                        throw;
                    }

				    string show = String.Format ( "The Server '{0}' is not working. Now trying to switch to the next server. Please wait...",
						url );
					errorStack.Append ( ex.Message + "\n" + ex.StackTrace + "\n\n" );
					log.Warn ( show, ex );
					ex.Source = show;
					// message box cannot be tested
					//  MessageBox.Show(show); // need to raise an event
				}
			}
			log.Error ( "No HIS Central Servers worked" );
			// MessageBox.Show("Servers are not working. Please try later.");
			throw new HydrodesktopSearchException ( errorStack.ToString() );
		}

        public Boolean RunHisCentralSearch(DoWorkEventArgs e, SearchCriteria parameters, IHISCentralSearcher searcher, IProgressHandler bgWorker)
		{
			// TODO: What if areaParameter == null?  Search fails due to local error, not HIS Central error.
			if ( parameters.areaParameter != null )
			{
				if ( !parameters.BoundinBoxSearch )
				{
					log.Info ( "Search in Polygon" );
					searcher.GetSeriesCatalogInPolygon ( (List<IFeature>)parameters.areaParameter,
													   parameters.keywords.ToArray (),
													   parameters.startDate, parameters.endDate,
													   parameters.serviceIDs.ToArray (), bgWorker, e );
					return true;
				}
				else
				{
					log.Info ( "Search in Box" );
					var coords = (AreaRectangle)parameters.areaParameter;
					//*             searcher.GetSeriesCatalogInRectangle(
					//                          (double)coords[0], (double)coords[2], (double)coords[1], (double)coords[3],
					searcher.GetSeriesCatalogInRectangle (
						coords.XMin, coords.XMax, coords.YMin, coords.YMax,
						parameters.keywords.ToArray (),
						parameters.startDate, parameters.endDate,
						parameters.serviceIDs.ToArray (), bgWorker, e );
					return true;
				}
			}
			return false;
		}

        public bool RunMetadataCacheSearch(DoWorkEventArgs e, IProgressHandler bgWorker)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (bgWorker == null) throw new ArgumentNullException("bgWorker");

            MetadataCacheSearcher searcher = new MetadataCacheSearcher();
            SearchCriteria parameters;
            if (!e.Argument.GetType().Equals(typeof(SearchCriteria)))
            {
                throw new ArgumentException("Bad Argument should be of type SearchCriteria");
            }
            else
            {
                parameters = (SearchCriteria)e.Argument;
            }
            
            if (parameters.areaParameter != null)
            {
                if (!parameters.BoundinBoxSearch)
                {
                    log.Info("Search in Polygon");
                    searcher.GetSeriesCatalogInPolygon((List<IFeature>)parameters.areaParameter,
                                                       parameters.keywords.ToArray(),
                                                       parameters.startDate, parameters.endDate,
                                                       parameters.serviceIDs.ToArray(), bgWorker, e);
                    return true;
                }
                else
                {
                    log.Info("Search in Box");
                    var coords = (AreaRectangle)parameters.areaParameter;
                    searcher.GetSeriesCatalogInRectangle(
                        coords.XMin, coords.XMax, coords.YMin, coords.YMax,
                        parameters.keywords.ToArray(),
                        parameters.startDate, parameters.endDate,
                        parameters.serviceIDs.ToArray(), bgWorker, e);
                    return true;
                }
            }
            return false;
        }
	}


}
