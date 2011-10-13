using System.Collections.Generic;
using System.Linq;
using Search3.Searchers;
using Search3.Settings;

namespace Search3.WebServices
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