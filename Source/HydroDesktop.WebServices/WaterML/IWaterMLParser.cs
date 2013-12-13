using System.Collections.Generic;
using System.IO;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterML
{
    /// <summary>
    /// Contains methods for parsing the WaterML files
    /// </summary>
    public interface IWaterMLParser
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
