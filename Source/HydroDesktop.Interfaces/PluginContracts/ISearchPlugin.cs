using System;
using System.Collections.Generic;
using DotSpatial.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces.PluginContracts
{
    /// <summary>
    /// Provides methods for displaying search results
    /// </summary>
    public interface ISearchPlugin
    {
        /// <summary>
        /// Adds features to the search results layer
        /// </summary>
        /// <param name="featuresPerCode">number of features per site code</param>
        void AddFeatures(List<Tuple<string, IFeatureSet>> featuresPerCode);

        /// <summary>
        /// Get web services list.
        /// </summary>
        /// <returns>Web services list.</returns>
        IList<DataServiceInfo> GetWebServices();

        /// <summary>
        /// Gets current HIS Central URL.
        /// </summary>
        string HisCentralUrl { get; }
    }
}