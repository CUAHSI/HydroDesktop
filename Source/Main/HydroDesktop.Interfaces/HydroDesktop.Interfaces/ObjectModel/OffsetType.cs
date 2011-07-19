using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Information about the type of the vertical offset
    /// of a data value
    /// </summary>
    public class OffsetType : BaseEntity
    {
        public OffsetType()
        {
            Description = Constants.Unknown;
            Unit = Unit.Unknown;
        }
        
        public virtual string Description { get; set; }

        /// <summary>
        /// The distance units of the vertical offset
        /// </summary>
        public virtual Unit Unit { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
