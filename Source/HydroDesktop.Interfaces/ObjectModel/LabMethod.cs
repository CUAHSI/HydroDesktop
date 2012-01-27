using System;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Information about a laboratory method
    /// used for analyzing a water quality sample
    /// </summary>
    public class LabMethod : BaseEntity
    {
        /// <summary>
        /// Lab name
        /// </summary>
        public virtual string LabName { get; set; }
        /// <summary>
        /// Lab Organization
        /// </summary>
        public virtual string LabOrganization { get; set; }
        /// <summary>
        /// Lab Method name
        /// </summary>
        public virtual string LabMethodName { get; set; }
        /// <summary>
        /// Lab method description
        /// </summary>
        public virtual string LabMethodDescription { get; set; }
        /// <summary>
        /// Lab method link
        /// </summary>
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
