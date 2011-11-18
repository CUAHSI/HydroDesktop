using System.Collections.Generic;
using System.Linq;
using FacetedSearch.Searching;
using FacetedSearch.Settings;

namespace FacetedSearch.WebServices
{
    class DbWebServicesList : IWebServicesList
    {
        public IEnumerable<WebServiceNode> GetWebServices()
        {
            return new MetadataCacheSearcher().GetWebServices().Select(
                service =>
                new WebServiceNode(service.ServiceTitle,
                    service.ServiceCode, service.Id.ToString(), service.DescriptionURL, service.EndpointURL)).ToList();
        }
    }
}