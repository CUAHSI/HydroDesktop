using System.Globalization;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents a spatial reference system
    /// SRSID is the EPSG code of the system.
    /// </summary>  
    public class SpatialReference : BaseEntity
    {
        /// <summary>
        /// Creates a new spatial reference (unknown spatial reference)
        /// </summary>
        public SpatialReference()
        {
            SRSID = 0;
            SRSName = Constants.Unknown;
            Notes = Constants.Unknown;
        }
        /// <summary>
        /// Creates a spatial reference with the specified EPSG code (SRS ID)
        /// </summary>
        /// <param name="srsID">The EPSG identifier</param>
        public SpatialReference(int srsID)
        {
            SRSID = srsID;
            SRSName = "EPSG:" + srsID.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Creates a spatial reference with the specified name
        /// </summary>
        /// <param name="srsName">the spatial reference name</param>
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
        /// <summary>
        /// EPSG Code
        /// </summary>
        public virtual int SRSID { get; set; }
        /// <summary>
        /// Spatial reference name
        /// </summary>
        public virtual string SRSName { get; set; }
        /// <summary>
        /// Optional spatial reference notes
        /// </summary>
        public virtual string Notes { get; set; }
        /// <summary>
        /// Shows the spatial reference name
        /// </summary>
        /// <returns>spatial reference name string representation</returns>
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
