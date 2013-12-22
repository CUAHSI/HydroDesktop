using System;
using HydroDesktop.WebServices;

namespace Search3.Settings
{
    public class WebServiceNode
    {
        private WebServiceNode()
        {
        }

        public WebServiceNode(string title, string serviceCode, int serviceID, string descriptionUrl, string serviceUrl,
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

        public WebServiceNode(string title, string serviceCode, int serviceID, string descriptionUrl, string serviceUrl,
        Box boundingBox, int sites, int variables, int values)
        {
            ServiceID = serviceID;
            ServiceCode = serviceCode;
            Title = title;
            DescriptionUrl = descriptionUrl;
            ServiceUrl = serviceUrl;
            ServiceBoundingBox = boundingBox;
            Sites = sites;
            Variables = variables;
            Values = values;
            Checked = true;
        }

        public WebServiceNode(string title, string serviceCode, int serviceID, string descriptionUrl, string serviceUrl,
            Box boundingBox, string organization, long sites, long variables, long values)
        {
            ServiceID = serviceID;
            ServiceCode = serviceCode;
            Title = title;
            DescriptionUrl = descriptionUrl;
            ServiceUrl = serviceUrl;
            ServiceBoundingBox = boundingBox;
            Checked = true;
            Organization = organization;
            Sites = sites;
            Variables = variables;
            Values = values;
        }

        public Box ServiceBoundingBox { get; private set; }
        public int ServiceID { get; private set; }
        public string ServiceCode { get; private set; }
        public string Title { get; private set; }
        public string DescriptionUrl { get;  private set; }
        public string ServiceUrl { get; private set; }
        public bool Checked { get; set; }
        public string Organization { get; private set; }
        public long Sites { get; private set; }
        public long Variables { get; private set; }
        public long Values { get; private set; }

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
            ServiceBoundingBox = source.ServiceBoundingBox == null
                                     ? null
                                     : new Box(source.ServiceBoundingBox.XMin,
                                               source.ServiceBoundingBox.XMax, source.ServiceBoundingBox.YMin,
                                               source.ServiceBoundingBox.YMax);
            Organization = source.Organization;
            Sites = source.Sites;
            Variables = source.Variables;
            Values = source.Values;
        }
    }
}