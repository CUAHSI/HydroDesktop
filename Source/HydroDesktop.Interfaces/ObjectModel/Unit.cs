namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents an observation unit or a time unit
    /// </summary>
    public class Unit : BaseEntity
    {
        /// <summary>
        /// Creates a new default instance of the unit object
        /// </summary>
        public Unit()
        {
        }

        /// <summary>
        /// Creates an instance of a unit object
        /// </summary>
        /// <param name="name">The unit name (for example, "centimeter")</param>
        /// <param name="type">The unit type (for example, "length")</param>
        /// <param name="abbreviation">The unit abbreviation (for example, "cm")</param>
        public Unit(string name, string type, string abbreviation)
        {
            Name = name;
            UnitsType = type;
            Abbreviation = abbreviation;
        }

        /// <summary>
        /// The unit name (for example "centimeter")
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The unit type ("length", "time")
        /// </summary>
        public virtual string UnitsType { get; set; }

        /// <summary>
        /// The unit abbreviation
        /// </summary>
        public virtual string Abbreviation { get; set; }

        /// <summary>
        /// Dimension of unit
        /// </summary>
        public virtual string Dimension { get; set; }

        /// <summary>
        /// Conversion factor to base SI unit
        /// </summary>
        public virtual double? ConversionFactorToSI { get; set; }

        /// <summary>
        /// Offset to base SI unit
        /// </summary>
        public virtual double? OffsetToSI { get; set; }

        /// <summary>
        /// Converts the unit to string
        /// </summary>
        /// <returns>The unit abbreviation</returns>
        public override string ToString()
        {
            return Abbreviation;
        }

        /// <summary>
        /// When the variable unit is unknown
        /// </summary>
        public static Unit Unknown
        {
            get
            {
                return new Unit
                           {
                               Name = Constants.Unknown,
                               UnitsType = Constants.Unknown,
                               Abbreviation = Constants.Unknown
                           };
            }
        }

        /// <summary>
        /// When the time unit is unknown
        /// </summary>
        public static Unit UnknownTimeUnit
        {
            get
            {
                return new Unit
                           {
                               Name = Constants.Unknown,
                               UnitsType = "Time",
                               Abbreviation = Constants.Unknown
                           };
            }
        }

        #region Equality

        /// <summary>
        /// Two units are considered equal, if they have the same name, abbreviation and unit type
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(BaseEntity other)
        {
            var unit = other as Unit;
            if (unit != null)
            {
                if (unit.Name != Name) return false;
                if (unit.UnitsType != UnitsType) return false;
                if (unit.Abbreviation != Abbreviation) return false;
                return true;
            }
            return base.Equals(other);
        }

        /// <summary>
        /// Returns an unique hash code of the unit object
        /// </summary>
        /// <returns>Code in form Name|UnitType|Abbreviation</returns>
        public override int GetHashCode()
        {
            return (Name + "|" + UnitsType + "|" + Abbreviation).GetHashCode();
        }

        #endregion
    }
}
