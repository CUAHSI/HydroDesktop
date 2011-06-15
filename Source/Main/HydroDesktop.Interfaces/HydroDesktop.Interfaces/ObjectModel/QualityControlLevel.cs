using System;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Specifies the quality control level (raw data, approved data)
    /// </summary>
    public class QualityControlLevel : BaseEntity
    {
        /// <summary>
        /// The original identifier of the quality control level specified by a
        /// web service. This is an optional property. Set this property to 0 if not
        /// used.
        /// </summary>
        public virtual int OriginId { get; set; }    
        
        public virtual string Code { get; set; }
        public virtual string Definition { get; set; }
        public virtual string Explanation { get; set; }

        public override string ToString()
        {
            return Definition;
        }

        /// <summary>
        /// When the quality control level is unknown or unspecified
        /// </summary>
        public static QualityControlLevel Unknown
        {
            get
            {
                return new QualityControlLevel
                {
                    Code = Constants.Unknown,
                    Definition = Constants.Unknown,
                    Explanation = Constants.Unknown,
                    OriginId = 0
                };
            }
        }
    }   
}
