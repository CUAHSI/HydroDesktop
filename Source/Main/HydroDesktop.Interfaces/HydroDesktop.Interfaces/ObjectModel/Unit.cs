using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents an observation unit or a time unit
    /// </summary>
    public class Unit : BaseEntity
    {
        public Unit()
        { }
        
        public Unit(string name, string type, string abbreviation)
        {
            Name = name;
            UnitsType = type;
            Abbreviation = abbreviation;
        }
        
        public virtual string Name { get; set; }
        public virtual string UnitsType { get; set; }
        public virtual string Abbreviation { get; set; }

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
        public override bool Equals(BaseEntity other)
        {
            if (other is Unit)
            {
                if (((Unit)other).Name != Name) return false;
                if (((Unit)other).UnitsType != UnitsType) return false;
                if (((Unit)other).Abbreviation != Abbreviation) return false;
                return true;
            }
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return (Name + "|" + UnitsType + "|" + Abbreviation).GetHashCode();
        }
        #endregion
    }
}
