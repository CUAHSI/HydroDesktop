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

    public interface ISearchPlugin
    {
        void AddFeatures(List<Tuple<string, IFeatureSet>> featuresPerCode);
        IMapGroup GetDataSitesLayerGroup(IMap map);
    }
}