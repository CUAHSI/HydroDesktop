
namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// ISO Metadata Information
    /// </summary>
    public class ISOMetadata : BaseEntity
    {
        /// <summary>
        /// The ISO metadata topic category
        /// </summary>
        public virtual string TopicCategory { get; set; }
        /// <summary>
        /// The ISO metadata title
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// The ISO metadata abstract
        /// </summary>
        public virtual string Abstract { get; set; }
        /// <summary>
        /// The ISO metadata profile version
        /// </summary>
        public virtual string ProfileVersion { get; set; }
        /// <summary>
        /// The ISO metadata link
        /// </summary>
        public virtual string MetadataLink { get; set; }

        /// <summary>
        /// When the ISO metadata is unknown
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
