using System.Collections.Generic;
using System.Linq;
using HydroDesktop.WebServices;
using Search3.Searching;
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
                                   service.ServiceCode, (int) service.Id, service.DescriptionURL, service.EndpointURL,
                                   new Box(service.WestLongitude, service.EastLongitude,
                                           service.SouthLatitude, service.NorthLatitude), service.SiteCount, service.VariableCount, (int)service.ValueCount)).ToList();
        }
    }
}