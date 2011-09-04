using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents the data site. One site can have one or many series available at the site.
    /// The site is specific for a data service If two services have data from the same
    /// location, then they are represented as two different sites
    /// </summary>
    public class Site : BaseEntity
    {
        /// <summary>
        /// Create a new instance of a WaterML Site object
        /// </summary>
        public Site()
        {
            //default time zone is UTC
            DefaultTimeZone = TimeZoneInfo.Utc;
        }
        /// <summary>
        /// WaterML Site code in NetworkPrefix:SiteCode format
        /// </summary>
        public virtual string Code { get; set; }
        /// <summary>
        /// Site name provided by the data source organization
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// Latitude in decimal degrees
        /// </summary>
        public virtual double Latitude { get; set; }
        /// <summary>
        /// Longitude in decimal degrees
        /// </summary>
        public virtual double Longitude { get; set; }
        /// <summary>
        /// Elevation in meters
        /// </summary>
        public virtual double Elevation_m { get; set; }
        /// <summary>
        /// Vertical datum
        /// </summary>
        public virtual string VerticalDatum { get; set; }
        /// <summary>
        /// X coordinate in projected coordinate system
        /// </summary>
        public virtual double LocalX { get; set; }
        /// <summary>
        /// Y coordinate in projected coordinate system
        /// </summary>
        public virtual double LocalY { get; set; }
        /// <summary>
        /// Positional accuracy in meters
        /// </summary>
        public virtual double PosAccuracy_m { get; set; }
        /// <summary>
        /// State (optional, use only for U.S states)
        /// </summary>
        public virtual string State { get; set; }
        /// <summary>
        /// County (optional, use only for U.S counties)
        /// </summary>
        public virtual string County { get; set; }
        /// <summary>
        /// Comments (optional comments about the site)
        /// </summary>
        public virtual string Comments { get; set; }
        /// <summary>
        /// Network prefix. This is the first part of the site
        /// code before the ':' separator.
        /// </summary>
        public virtual string NetworkPrefix { get; set; }
        
        /// <summary>
        /// The spatial reference(datum) of the site's geographic coordinates
        /// </summary>
        public virtual SpatialReference SpatialReference { get; set; }

        /// <summary>
        /// The local projection spatial reference system (if available)
        /// </summary>
        public virtual SpatialReference LocalProjection { get; set; }

        /// <summary>
        /// The list of all time series available for this site
        /// </summary>
        public virtual IList<Series> DataSeriesList { get; protected set; }

        /// <summary>
        /// Information about time zone at this site
        /// </summary>
        public virtual TimeZoneInfo DefaultTimeZone { get; set; }

        ///// <summary>
        ///// Information about data service that was used to 
        ///// retrieve this site
        ///// </summary>
        //public virtual DataServiceInfo DataService { get; set; }

        /// <nheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}
