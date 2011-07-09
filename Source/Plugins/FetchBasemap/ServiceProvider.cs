using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DotSpatial.Controls.Header;
using FetchBasemap.Resources;

namespace FetchBasemap
{
    public class ServiceProvider
    {
        public static IEnumerable<ServiceProvider> GetDefaultServiceProviders()
        {
            foreach (var item in Services.Default.List)
            {
                var serviceDescArr = item.Split(',');

                var serviceName = serviceDescArr[0];
                var serviceUrl = serviceDescArr[1];

                yield return new ServiceProvider(serviceName, serviceUrl);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ServiceProvider class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public ServiceProvider(string name, string url)
        {
            Url = url;
            Name = name;
        }

        public ServiceProvider()
        {
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}