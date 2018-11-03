using System.Collections.Generic;
using HydroDesktop.Plugins.Search.Settings;

namespace HydroDesktop.Plugins.Search.WebServices
{
    interface IWebServicesList
    {
        IEnumerable<WebServiceNode> GetWebServices();
    }
}