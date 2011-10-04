using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Search3.Settings.UI;

namespace Search3.Settings
{
    public class WebServicesSettings
    {
        private IEnumerable<WebServiceNode> _webServices;
        public IEnumerable<WebServiceNode> WebServices
        {
            get { return _webServices ?? (_webServices = new WebServicesList().GetWebServicesCollection()); }
            set
            {
                Debug.Assert(value != null);
                _webServices = value;
            }
        }

        public int CheckedCount
        {
            get { return WebServices.Count(w => w.Checked); }
        }

        public int TotalCount
        {
            get { return WebServices.Count(); }
        }

        /// <summary>
        /// Create deep copy of current instance.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public WebServicesSettings Copy()
        {
            var result = new WebServicesSettings();
            result.Copy(this);
            return result;

        }

        /// <summary>
        /// Create deep from source into current instance.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>must be not null.</exception>
        public void Copy(WebServicesSettings source)
        {
            if (source == null) throw new ArgumentNullException("source");

            var list = new List<WebServiceNode>(source.WebServices.Count());
            list.AddRange(source.WebServices.Select(webNode => webNode.Copy()));
            WebServices = list;
        }
    }
}