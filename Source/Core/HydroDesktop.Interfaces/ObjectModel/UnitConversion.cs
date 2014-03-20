namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents conversion between units
    /// </summary>
    public class UnitConversion : BaseEntity
    {
        /// <summary>
        /// Source unit
        /// </summary>
        public virtual Unit FromUnit { get; set; }

        /// <summary>
        /// Destination Unit
        /// </summary>
        public virtual Unit ToUnit { get; set; }

        /// <summary>
        /// Conversion factor
        /// </summary>
        public virtual double ConversionFactor { get; set; }

        /// <summary>
        /// Offset
        /// </summary>
        public virtual double Offset { get; set; }
    }
}