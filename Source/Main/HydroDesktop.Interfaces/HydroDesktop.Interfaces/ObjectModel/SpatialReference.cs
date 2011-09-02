using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents a spatial reference system
    /// SRSID is the EPSG code of the system.
    /// </summary>  
    public class SpatialReference : BaseEntity
    {
        
        public SpatialReference()
        {
            SRSID = 0;
            SRSName = Constants.Unknown;
            Notes = Constants.Unknown;
        }
        
        public SpatialReference(int srsID)
        {
            SRSID = srsID;
            SRSName = "EPSG:" + srsID.ToString();
        }

        public SpatialReference(string srsName)
        {
            SRSName = srsName;
            if (srsName.StartsWith("EPSG:") && srsName.Length > 5)
            {
                string srsIDstring = srsName.Substring(5);
                int srsid = 0;
                if (int.TryParse(srsIDstring, out srsid))
                {
                    this.SRSID = srsid;
                }
            }
        }
        
        public virtual int SRSID { get; set; }

        public virtual string SRSName { get; set; }

        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return SRSName;
        }

        /// <summary>
        /// When the spatial reference information is unknown
        /// </summary>
        public static SpatialReference Unknown
        {
            get
            {
                return new SpatialReference
                {
                    SRSID = 0,
                    SRSName = Constants.Unknown,
                    Notes = Constants.Unknown
                };
            }
        }
    }
}
