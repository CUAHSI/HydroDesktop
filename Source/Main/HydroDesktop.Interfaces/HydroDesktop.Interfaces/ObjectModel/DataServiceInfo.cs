using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents information about a web service
    /// </summary>
    public class DataServiceInfo : BaseEntity
    {
        public DataServiceInfo()
        {
            EndpointURL = Constants.Unknown;
            ServiceTitle = Constants.Unknown;
            ServiceCode = Constants.Unknown;
            ServiceName = Constants.Unknown;
            ServiceType = "CUAHSI";
            Version = 1.0;
            Protocol = "soap";
            EndpointURL = Constants.Unknown;
            DescriptionURL = Constants.Unknown;
            NorthLatitude = 90.0;
            SouthLatitude = -90.0;
            EastLongitude = 180.0;
            WestLongitude = -180.0;
            Abstract = Constants.Unknown;
            ContactName = Constants.Unknown;
            ContactEmail = Constants.Unknown;
            Citation = Constants.Unknown;
            IsHarvested = false;
        }

        /// <summary>
        /// Creates a copy of the data service object with the same
        /// properties as the original
        /// </summary>
        /// <param name="originalService">The original dataServiceInfo object</param>
        public DataServiceInfo(DataServiceInfo original)
        {
            ServiceTitle = original.ServiceTitle;
            ServiceCode = original.ServiceCode;
            ServiceName = original.ServiceName;
            ServiceType = original.ServiceType;
            Version = original.Version;
            Protocol = original.Protocol;
            EndpointURL = original.EndpointURL;
            DescriptionURL = original.DescriptionURL;
            NorthLatitude = original.NorthLatitude;
            SouthLatitude = original.SouthLatitude;
            EastLongitude = original.EastLongitude;
            WestLongitude = original.WestLongitude;
            Abstract = original.Abstract;
            ContactName = original.ContactName;
            ContactEmail = original.ContactEmail;
            Citation = original.Citation;
            IsHarvested = original.IsHarvested;
            HarveDateTime = original.HarveDateTime;
        }
        
        /// <summary>
        /// Creates a new data service entry with the specified url and service title
        /// </summary>
        /// <param name="serviceURL">the url of the .asmx web page</param>
        /// <param name="serviceTitle">the short description of the web service</param>
        public DataServiceInfo(string serviceURL, string serviceTitle)
        {
            ServiceTitle = serviceTitle;
            ServiceCode = Constants.Unknown;
            ServiceName = Constants.Unknown;
            ServiceType = "CUAHSI";
            Version = 1.0;
            Protocol = "soap";
            EndpointURL = serviceURL;
            DescriptionURL = serviceURL;
            NorthLatitude = 90.0;
            SouthLatitude = -90.0;
            EastLongitude = 180.0;
            WestLongitude = -180.0;
            Abstract = Constants.Unknown;
            ContactName = Constants.Unknown;
            ContactEmail = Constants.Unknown;
            Citation = Constants.Unknown;
            IsHarvested = false;
        }
        
        /// <summary>
        /// The service code (for example NWISDW, txEvap,...)
        /// </summary>
        public virtual string ServiceCode { get; set; }

        /// <summary>
        /// The service name. In most cases the name is 'WaterOneFlow' or 'HISCentral'.
        /// </summary>
        public virtual string ServiceName { get; set; }

        /// <summary>
        /// The service type. Default value is 'CUAHSI'.
        /// </summary>
        public virtual string ServiceType { get; set; }

        /// <summary>
        /// The version of the service (1.0 or 1.1)
        /// </summary>
        public virtual double Version { get; set; }

        /// <summary>
        /// The service protocol (SOAP or REST)
        /// </summary>
        public virtual string Protocol { get; set; }

        /// <summary>
        /// The URI of the web service. For SOAP web services,
        /// the URI ends with .asmx (without the ?WSDL suffix)
        /// </summary>
        public virtual string EndpointURL { get; set; }

        /// <summary>
        /// The URI of a web page with detailed description of the
        /// web service
        /// </summary>
        public virtual string DescriptionURL { get; set; }

        /// <summary>
        /// Latitude of the northernmost site provided by the 
        /// web service. Default value is +90
        /// </summary>
        public virtual double NorthLatitude { get; set; }

        /// <summary>
        /// Latitude of the southernmost site provided by the 
        /// web service. Default value is -90
        /// </summary>
        public virtual double SouthLatitude { get; set; }

        /// <summary>
        /// Longitude of the easternmost site provided by the 
        /// web service. Default value is +180
        /// </summary>
        public virtual double EastLongitude { get; set; }

        /// <summary>
        /// Longitude of the westernmost site provided by the
        /// web service. Default value is -180
        /// </summary>
        public virtual double WestLongitude { get; set; }

        /// <summary>
        /// (Optional) Abstract with a description of the web service
        /// </summary>
        public virtual string Abstract { get; set; }

        /// <summary>
        /// (Optional) The contact name
        /// </summary>
        public virtual string ContactName { get; set; }

        /// <summary>
        /// (Optional) The contact email
        /// </summary>
        public virtual string ContactEmail { get; set; }

        /// <summary>
        /// (Optional) the citation
        /// </summary>
        public virtual string Citation { get; set; }
        
        /// <summary>
        /// True if the data from the web service have been harvested
        /// </summary>
        public virtual bool IsHarvested { get; set; }

        /// <summary>
        /// The last time this service was harvested by the client
        /// </summary>
        public virtual DateTime HarveDateTime { get; set; }

        /// <summary>
        /// A brief description or name of the web service
        /// </summary>
        public virtual string ServiceTitle { get; set; }

        /// <summary>
        /// The total number of data values provided by the web service
        /// </summary>
        public virtual Int64 ValueCount { get; set; }

        /// <summary>
        /// The total number of sites provided by the web service
        /// </summary>
        public virtual int SiteCount { get; set; }

        /// <summary>
        /// The Id of this web service as specified by HIS Central
        /// Only specify this property when the web service is 
        /// registered at HIS Central.
        /// </summary>
        public virtual int HISCentralID { get; set; }

        /// <summary>
        /// Creates a copy of this instance
        /// </summary>
        public virtual DataServiceInfo Copy()
        {
            return new DataServiceInfo(this);
        }
        
        public override bool Equals(BaseEntity other)
        {
            DataServiceInfo otherService = other as DataServiceInfo;
            if (otherService == null)
            {
                return base.Equals(other);
            }
            else
            {
                return EndpointURL.Equals(otherService.EndpointURL);
            }
        }

        public override int GetHashCode()
        {
            return EndpointURL.GetHashCode();
        }
    }
}
