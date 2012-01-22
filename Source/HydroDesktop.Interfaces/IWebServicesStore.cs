using System;
using System.Collections.Generic;
using DotSpatial.Controls;
using DotSpatial.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Provides methods to get web services.
    /// </summary>
    public interface IWebServicesStore
    {
        /// <summary>
        /// Get web services list.
        /// </summary>
        /// <returns>Web services list.</returns>
        IList<DataServiceInfo> GetWebServices();
    }
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
    }
}