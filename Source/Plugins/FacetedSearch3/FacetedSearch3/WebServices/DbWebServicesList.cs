using System.Collections.Generic;
using System.Linq;
using FacetedSearch3.Searching;
using FacetedSearch3.Settings;

namespace FacetedSearch3.WebServices
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