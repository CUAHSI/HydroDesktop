using System.Collections.Generic;
using FacetedSearch3.Settings;

namespace FacetedSearch3.WebServices
{
    interface IWebServicesList
    {
        IEnumerable<WebServiceNode> GetWebServices();
    }
}