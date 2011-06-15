using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// [MapTo("Sites")]
    /// </summary>
    public class Site : BaseEntity
    {
        public Site()
        {
            //default time zone is UTC
            DefaultTimeZone = TimeZoneInfo.Utc;
        }
        
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        
        public virtual double Elevation_m { get; set; }
        public virtual string VerticalDatum { get; set; }
        public virtual double LocalX { get; set; }
        public virtual double LocalY { get; set; }
        
        public virtual double PosAccuracy_m { get; set; }
        public virtual string State { get; set; }
        public virtual string County { get; set; }
        public virtual string Comments { get; set; }
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

        /// <summary>
        /// Information about data service that was used to 
        /// retrieve this site
        /// </summary>
        //public virtual DataServiceInfo DataService { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
