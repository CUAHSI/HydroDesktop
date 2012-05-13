using System.Collections.Generic;
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
}