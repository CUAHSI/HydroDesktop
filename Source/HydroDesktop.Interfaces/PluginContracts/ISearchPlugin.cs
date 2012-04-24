using System;
using System.Collections.Generic;
using DotSpatial.Data;

namespace HydroDesktop.Interfaces
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
    }
}