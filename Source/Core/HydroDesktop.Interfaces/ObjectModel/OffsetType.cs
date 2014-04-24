namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Information about the type of the vertical offset
    /// of a data value
    /// </summary>
    public class OffsetType : BaseEntity
    {
        /// <summary>
        /// Creates a new default vertical offset type
        /// (offset is unknown
        /// </summary>
        public OffsetType()
        {
            Description = Constants.Unknown;
            Unit = Unit.Unknown;
        }
        /// <summary>
        /// Offset type description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The distance units of the vertical offset
        /// </summary>
        public virtual Unit Unit { get; set; }

        /// <summary>
        /// When the offset is unknown
        /// </summary>
        public static OffsetType Unknown
        {
            get
            {
                return new OffsetType
                           {
                               Description = Constants.Unknown,
                               Unit = Unit.Unknown,
                           };
            }
        }

        /// <summary>
        /// Shows the description of the offset type
        /// </summary>
        /// <returns>offset type description string</returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
