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
        
        public virtual int OriginId { get; set; }
        
        public virtual string Organization { get; set; }
        public virtual string Description { get; set; }
        public virtual string Link { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Email { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual int ZipCode { get; set; }
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
