using System;
using HydroDesktop.WebServices;

namespace Search3.Settings
{
    public class WebServiceNode
    {
        private WebServiceNode()
        {
        }

        public WebServiceNode(string title, string serviceCode, string serviceID, string descriptionUrl, string serviceUrl,
            Box boundingBox)
        {
            ServiceID = serviceID;
            ServiceCode = serviceCode;
            Title = title;
            DescriptionUrl = descriptionUrl;
            ServiceUrl = serviceUrl;
            ServiceBoundingBox = boundingBox;
            Checked = true;
        }

        public Box ServiceBoundingBox { get; private set; }
        public string ServiceID { get; private set; }
        public string ServiceCode { get; private set; }
        public string Title { get; private set; }
        public string DescriptionUrl { get;  private set; }
        public string ServiceUrl { get; private set; }
        public bool Checked { get; set; }

        /// <summary>
        /// Create deep copy of current instance.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public WebServiceNode Copy()
        {
            var result = new WebServiceNode();
            result.Copy(this);
            return result;

        }

        /// <summary>
        /// Create deep from source into current instance.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>must be not null.</exception>
        public void Copy(WebServiceNode source)
        {
            if (source == null) throw new ArgumentNullException("source");

            ServiceID = source.ServiceID;
            ServiceCode = source.ServiceCode;
            Title = source.Title;
            DescriptionUrl = source.DescriptionUrl;
            ServiceUrl = source.ServiceUrl;
            Checked = source.Checked;
        }
    }
}