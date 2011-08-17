using System;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// ISO Metadata Information
    /// </summary>
    public class ISOMetadata : BaseEntity
    {
        public virtual string TopicCategory { get; set; }
        public virtual string Title { get; set; }
        public virtual string Abstract { get; set; }
        public virtual string ProfileVersion { get; set; }
        public virtual string MetadataLink { get; set; }

        /// <summary>
        /// When the ISO Metadata is unknown
        /// </summary>
        public static ISOMetadata Unknown
        {
            get
            {
                return new ISOMetadata 
                {
                    TopicCategory = Constants.Unknown,
                    Title = Constants.Unknown,
                    Abstract = Constants.Unknown,
                    ProfileVersion = Constants.Unknown,
                    MetadataLink = Constants.Unknown
                };
            }
        }
    }
}
