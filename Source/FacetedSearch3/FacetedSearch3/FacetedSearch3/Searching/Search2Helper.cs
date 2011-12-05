using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Projections;
using FacetedSearch3.Keywords;
using FacetedSearch3.Properties;
using FacetedSearch3.Settings;

namespace FacetedSearch3.Searching
{
    /// <summary>
    /// Helper to port logic from Search2
    /// </summary>
    static class Search2Helper
    {
        /// <summary>
        /// Creates from Search3 settings SearchCriteria class (used in Search2)
        /// </summary>
        /// <param name="settings">Search3 settings</param>
        /// <returns>Search2 SearchCriteria class</returns>
        public static SearchCriteria GetSearchParameters(SearchSettings settings)
        {
            // TODO: Remove this method and use SearchSettings

            var searchCritria = new SearchCriteria();

            searchCritria.SearchMethod = settings.CatalogSettings.TypeOfCatalog;
            foreach (var keyword in settings.KeywordsSettings.SelectedKeywords)
                searchCritria.keywords.Add(keyword);

            /* this region to be refactored /*/
            // Validate and refine the list of keywords.
            //string ontologyFilePath = GetOntologyFilePath ();
            //XmlDocument ontologyXml = new XmlDocument ();
            //ontologyXml.Load ( ontologyFilePath );
            if (searchCritria.SearchMethod == TypeOfCatalog.HisCentral)
            {
                var ontologyXml = HdSearchOntologyHelper.ReadOntologyXmlFile();
                var ontologyHelper = new HdSearchOntologyHelper();
                ontologyHelper.RefineKeywordList(searchCritria.keywords, ontologyXml);
            }
            else
            {
                //in the special case of metadata cache - hydrosphere keyword
                if (searchCritria.keywords.Contains("Hydrosphere"))
                {
                    searchCritria.keywords.Clear();
                }
            }
            /* this region to be refactored /*/

            //get the start date & end date
            searchCritria.startDate = settings.DateSettings.StartDate;
            searchCritria.endDate = settings.DateSettings.EndDate;


            //if all webservices are selected, pass an empty array
            if (settings.WebServicesSettings.TotalCount == settings.WebServicesSettings.CheckedCount)
            {
                searchCritria.serviceIDs.Clear();
            }
            else
            {
                foreach (var item in settings.WebServicesSettings.WebServices.Where(item => item.Checked))
                {
                    searchCritria.serviceIDs.Add(Convert.ToInt32(item.ServiceID));
                }
            }

            //get the HIS Central URL
            searchCritria.hisCentralURL = settings.CatalogSettings.HISCentralUrl;

            //get the selected polygons from the active layer or the rectangle
            GetSearchArea(searchCritria, settings);

            //To pass in the search parameters
            return searchCritria;
        }

        private static void GetSearchArea(SearchCriteria searchCriteria, SearchSettings settings)
        {
            object areaParameter = null;
            if (settings.AreaSettings.Polygons != null)
            {
                var polyFs = settings.AreaSettings.Polygons;
                string esri = Resources.wgs_84_esri_string;
                var wgs84 = ProjectionInfo.FromEsriString(esri);

                //reproject the selected polygons to WGS1984         
                polyFs.Reproject(wgs84);

                //the list of selected polygons passed in to the search function
                var selectedPolygons = Enumerable.ToList(polyFs.Features);

                areaParameter = selectedPolygons;
            }
            else if (settings.AreaSettings.AreaRectangle != null)
            {
                areaParameter = settings.AreaSettings.AreaRectangle;
            }

            //to assign the area parameter
            searchCriteria.areaParameter = areaParameter;
            return;
        }
    }
}