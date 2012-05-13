using System.Collections.Generic;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Contains methods for parsing the xml (WaterML) files returned
    /// by different versions of the WaterOneFlow web services
    /// </summary>
    public interface IWaterOneFlowParser
    {
        /// <summary>
        /// Parses the xml file returned by GetSites call to a WaterOneFlow
        /// web service
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        IList<Site> ParseGetSitesXml(string xmlFile);

        /// <summary>
        /// Parses the xml file returned by GetSiteInfo call to a WaterOneFlow web service
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        IList<SeriesMetadata> ParseGetSiteInfo(string xmlFile);
        
        /// <summary>
        /// Parses a WaterML TimeSeriesResponse XML file
        /// </summary>
        /// <param name="xmlFile"></param>
        IList<Series> ParseGetValues(string xmlFile);
    }
}
