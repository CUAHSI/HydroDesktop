using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using HydroDesktop.Common;
using HydroDesktop.Common.UserMessage;
using Search3.WebServices;

namespace Search3.Settings
{
    public class WebServicesSettings
    {
        private readonly SearchSettings _parent;

        public WebServicesSettings(SearchSettings parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            _parent = parent;
        }

        private IList<WebServiceNode> _webServices;
        public ReadOnlyCollection<WebServiceNode> WebServices
        {
            get
            {
                if (_webServices == null)
                {
                    RefreshWebServices();
                    Debug.Assert(_webServices != null);
                }
                return new ReadOnlyCollection<WebServiceNode>(_webServices);
            }
            private set
            {
                _webServices = value;
            }
        }

        /// <summary>
        /// Refresh WebServices list.
        /// </summary>
        /// <param name="catalogSettings">Catalog settings to use. If null - used current catalog settings.</param>
        public void RefreshWebServices(CatalogSettings catalogSettings = null)
        {
            try
            {
                _webServices = WebServicesReader.GetWebServices(catalogSettings ?? _parent.CatalogSettings).ToList();
            }
            catch (Exception ex)
            {
                AppContext.Instance.Get<IUserMessage>().Error("Unable to refresh WebServices. Empty list will be used.", ex);
                _webServices = new List<WebServiceNode>();
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
            var result = new WebServicesSettings(_parent);
            result.Copy(this);
            return result;
        }

        /// <summary>
        /// Create deep copy from source into current instance.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>must be not null.</exception>
        public void Copy(WebServicesSettings source)
        {
            if (source == null) throw new ArgumentNullException("source");

            var list = new List<WebServiceNode>(source.WebServices.Count());
            list.AddRange(source.WebServices.Select(webNode => webNode.Copy()));
            WebServices = new ReadOnlyCollection<WebServiceNode>(list);
        }
    }
}