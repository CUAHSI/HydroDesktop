using System;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Information about a laboratory method
    /// used for analyzing a water quality sample
    /// </summary>
    public class LabMethod : BaseEntity
    {
        public virtual string LabName { get; set; }
        public virtual string LabOrganization { get; set; }
        public virtual string LabMethodName { get; set; }
        public virtual string LabMethodDescription { get; set; }
        public virtual string LabMethodLink { get; set; }

        /// <summary>
        /// When the lab method is unknown
        /// </summary>
        public static LabMethod Unknown
        {
            get
            {
                return new LabMethod
                {
                    LabName =Constants.Unknown,
                    LabOrganization = Constants.Unknown,
                    LabMethodDescription = Constants.Unknown,
                    LabMethodName = Constants.Unknown,
                    LabMethodLink = Constants.Unknown
                };
            }
        }
    }
}
