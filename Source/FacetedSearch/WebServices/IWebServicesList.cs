using System.Collections.Generic;
using FacetedSearch.Settings;

namespace FacetedSearch.WebServices
{
    interface IWebServicesList
    {
        IEnumerable<WebServiceNode> GetWebServices();
    }
}