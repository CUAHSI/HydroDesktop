using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Net;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// Helper class for getting the web service logo icons from HIS Central
    /// </summary>
    public class ServiceIconHelper
    {
        /// <summary>
        /// Creates a new instance of the icon helper class
        /// </summary>
        public ServiceIconHelper(string hisCentralUrl)
        {
            _hisCentralUrl = hisCentralUrl;
            if (_hisCentralUrl.EndsWith("webservices/hiscentral.asmx"))
            {
                _hisCentralUrl = _hisCentralUrl.Substring(0, _hisCentralUrl.IndexOf("webservices/hiscentral.asmx"));
                _defaultIconUrl = _hisCentralUrl + "images/defaulticon.gif";
            }

            LoadIcons();
        }

        readonly string _hisCentralUrl;
        string _defaultIconUrl;

        private readonly Dictionary<string, Image> _serviceIcons = new Dictionary<string, Image>();

        private void LoadIcons()
        {
            ResourceManager rm = Properties.Resources.ResourceManager;

            ResourceSet rs = rm.GetResourceSet(new CultureInfo("en-US"), true, true);

            foreach (DictionaryEntry entry in rs)
            {
                var entryImage = entry.Value as Image;
                if (entryImage != null)
                {
                    _serviceIcons.Add(entry.Key.ToString(), entryImage);
                }
            }
        }

        /// <summary>
        /// Given a service code (such as 'NWISDV'), returns the web service logo icon
        /// Uses the getIcon.aspx service at http://hiscentral.cuahsi.org
        /// </summary>
        /// <param name="serviceCode">the service code</param>
        /// <returns>the service logo icon</returns>
        public Image GetImageForService(string serviceCode)
        {
            if (!_serviceIcons.ContainsKey(serviceCode))
            {
                Image webImage = GetImageFromHISCentral(serviceCode);
                _serviceIcons.Add(serviceCode, webImage);
            }

            return _serviceIcons[serviceCode];
        }

        private Image GetImageFromHISCentral(string serviceCode)
        {
            int requestTimeout = 2000;

            try
            {
                string url = String.Format("{0}/getIcon.aspx?name={1}", _hisCentralUrl, serviceCode);
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                req.Timeout = requestTimeout;
                // Request response:
                System.Net.WebResponse webResponse = req.GetResponse();

                // Open data stream:
                System.IO.Stream webStream = webResponse.GetResponseStream();

                // convert webstream to image
                System.Drawing.Image tmpImage = System.Drawing.Image.FromStream(webStream);

                // Cleanup
                webResponse.Close();
                return tmpImage;
            }
            catch (WebException)
            {
                //if the icon is not available on the web
                return Properties.Resources.defaulticon;
            }
        }
    }
}
