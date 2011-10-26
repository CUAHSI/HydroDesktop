using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// The source of the observation data
    /// </summary>   
    public class Source : BaseEntity
    {
        /// <summary>
        /// Creates a new default source (the source is unknown)
        /// </summary>
        public Source()
        {
            Organization = Constants.Unknown;
            Description = Constants.Unknown;
            Link = Constants.Unknown;
            ContactName = Constants.Unknown;
            Phone = Constants.Unknown;
            Email = Constants.Unknown;
            Address = Constants.Unknown;
            City = Constants.Unknown;
            State = Constants.Unknown;
            ZipCode = 0;
            Citation = Constants.Unknown;
            ISOMetadata = ISOMetadata.Unknown;
            //DataService = null;
        }

        //public Source(DataServiceInfo DataService)
        //{
        //    this.DataService = DataService;
        //    Organization = DataService.ServiceTitle;
        //    Description = Constants.Unknown;
        //    Link = DataService.EndpointURL;
        //    ContactName = Constants.Unknown;
        //    Phone = Constants.Unknown;
        //    Email = Constants.Unknown;
        //    Address = Constants.Unknown;
        //    City = Constants.Unknown;
        //    State = Constants.Unknown;
        //    ZipCode = 0;
        //    Citation = Constants.Unknown;
        //    ISOMetadata = ISOMetadata.Unknown;
        //}
        /// <summary>
        /// The original indentifier used in the WaterML
        /// </summary>
        public virtual int OriginId { get; set; }
        /// <summary>
        /// The source organization
        /// </summary>
        public virtual string Organization { get; set; }
        /// <summary>
        /// The source description
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// source link
        /// </summary>
        public virtual string Link { get; set; }
        /// <summary>
        /// contact name
        /// </summary>
        public virtual string ContactName { get; set; }
        /// <summary>
        /// contact phone number
        /// </summary>
        public virtual string Phone { get; set; }
        /// <summary>
        /// contact email
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// contact address (street and number)
        /// </summary>
        public virtual string Address { get; set; }
        /// <summary>
        /// contact address city
        /// </summary>
        public virtual string City { get; set; }
        /// <summary>
        /// contact address state
        /// </summary>
        public virtual string State { get; set; }
        /// <summary>
        /// contact address zip code
        /// </summary>
        public virtual int ZipCode { get; set; }
        /// <summary>
        /// source citation
        /// </summary>
        public virtual string Citation { get; set; }

        // <summary>
        // An optional data service associated with the source
        // removed
        // </summary>
       // public virtual DataServiceInfo DataService { get; set; }
        
        /// <summary>
        /// The ISO Metadata information
        /// </summary>
        public virtual ISOMetadata ISOMetadata { get; set; }
        /// <summary>
        /// shows the organization name
        /// </summary>
        /// <returns>name of organization</returns>
        public override string ToString()
        {
            return Organization;
        }

        /// <summary>
        /// When the source is unknown
        /// </summary>
        public static Source Unknown
        {
            get
            {
                return new Source
                {
                    Organization = Constants.Unknown,
                    Description = Constants.Unknown,
                    Link = Constants.Unknown,
                    ContactName = Constants.Unknown,
                    Phone = Constants.Unknown,
                    Email = Constants.Unknown,
                    Address = Constants.Unknown,
                    City = Constants.Unknown,
                    State = Constants.Unknown,
                    ZipCode = 0,
                    Citation = Constants.Unknown,
                    ISOMetadata = ISOMetadata.Unknown,
                  //  DataService = null
                };
            }
        }
    }
}
