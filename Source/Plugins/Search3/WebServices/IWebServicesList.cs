using System.Collections.Generic;
using Search3.Settings;

namespace Search3.WebServices
{
    interface IWebServicesList
    {
        IEnumerable<WebServiceNode> GetWebServices();
    }
}