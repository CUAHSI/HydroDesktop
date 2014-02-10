using System.Collections.Generic;
using System.IO;
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
        /// Reads the stream returned by GetSites call to a WaterOneFlow web service.
        /// </summary>
        /// <param name="stream">Stream that contains xml file.</param>
        /// <returns>List of sites.</returns>
        IList<Site> ParseGetSites(Stream stream);

        /// <summary>
        /// Reads the stream returned by GetSiteInfo call to a WaterOneFlow web service.
        /// </summary>
        /// <param name="stream">Stream that contains xml file.</param>
        /// <returns>List of SeriesMetadata.</returns>
        IList<SeriesMetadata> ParseGetSiteInfo(Stream stream);

        /// <summary>
        /// Parses a WaterML TimeSeriesResponse XML file
        /// </summary>
        /// <param name="stream">Stream that contains xml file.</param>
        /// <returns>List of series.</returns>
        IList<Series> ParseGetValues(Stream stream);
    }
}
