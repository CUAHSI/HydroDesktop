using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// Helper class for getting the web service logo icons from HIS Central
    /// </summary>
    public class ServiceIconHelper
    {
        #region Fields

        private readonly Dictionary<string, Image> _serviceIcons = new Dictionary<string, Image>();
        private static readonly Lazy<ServiceIconHelper> _instance = new Lazy<ServiceIconHelper>(() => new ServiceIconHelper(), true);

        #endregion

        private ServiceIconHelper()
        {
            LoadIcons();
        }

        public static ServiceIconHelper Instance
        {
            get { return _instance.Value; }
        }

        #region Public methods

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
                var webImage = GetImageFromHISCentral(serviceCode);
                _serviceIcons.Add(serviceCode, webImage);
            }

            return _serviceIcons[serviceCode];
        }

        #endregion

        #region Private methods

        private void LoadIcons()
        {
            var rs = Properties.Resources.ResourceManager.GetResourceSet(new CultureInfo("en-US"), true, true);
            foreach (DictionaryEntry entry in rs)
            {
                var entryImage = entry.Value as Image;
                if (entryImage != null)
                {
                    _serviceIcons.Add(entry.Key.ToString(), entryImage);
                }
            }
        }

        private static Image GetImageFromHISCentral(string serviceCode)
        {
            try
            {
                var url = String.Format("{0}/getIcon.aspx?name={1}", Properties.Resources.HISCentral_Site, serviceCode);
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 5000;

                using (var webResponse = req.GetResponse())
                using (var webStream = webResponse.GetResponseStream())
                {
                    return Image.FromStream(webStream);
                }
            }
            catch (Exception)
            {
                // todo: log error

                //if the icon is not available on the web
                return Properties.Resources.defaulticon;
            }
        }

        #endregion
    }
}
