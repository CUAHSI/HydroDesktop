using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents a water quality sample. One sample
    /// can have multiple data values.
    /// </summary>
    public class Sample : BaseEntity
    {
        public virtual string SampleType { get; set; }
        public virtual string LabSampleCode { get; set; }

        /// <summary>
        /// The lab method used for analysis of this 
        /// sample
        /// </summary>
        public virtual LabMethod LabMethod { get; set; }

        /// <summary>
        /// When the lab method is unknown
        /// </summary>
        /// <returns>a default 'Unknown' lab method</returns>
        public static Sample Unknown
        {
            get
            {
                return new Sample
                {
                    SampleType = Constants.Unknown,
                    LabSampleCode = Constants.Unknown,
                    LabMethod = LabMethod.Unknown
                };
            }
        }
    }
}
