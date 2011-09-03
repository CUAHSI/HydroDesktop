using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// The variable (observed property)
    /// </summary>
    public class Variable : BaseEntity
    { 
        /// <summary>
        /// The WaterML full variable code in form
        /// VocabularyPrefix:VariableCode
        /// </summary>
        public virtual string Code { get; set; }
        /// <summary>
        /// The variable name. This should be one of the
        /// CUAHSI controlled vocabulary names
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// The speciation (for example, Ammonia as N)
        /// This attribute is used for water quality samples
        /// </summary>
        public virtual string Speciation { get; set; }
        /// <summary>
        /// The sample medium (Surface Water, Air, Precipitation)
        /// </summary>
        public virtual string SampleMedium { get; set; }
        /// <summary>
        /// Text value indicating what
        /// type of data value is being
        /// recorded. This should be from
        /// the ValueTypeCV controlled
        /// vocabulary table.
        /// </summary>
        public virtual string ValueType { get; set; }
        /// <summary>
        /// True if the variable is observed at a regular time step
        /// </summary>
        public virtual bool IsRegular { get; set; }
        /// <summary>
        /// True if the variable is categorical
        /// </summary>
        public virtual bool IsCategorical { get; set; }
        /// <summary>
        /// Numerical value that indicates the time support (or temporal
        /// footprint) of the data values. 0 is used to indicate data values
        /// that are instantaneous. Other values indicate the time over
        /// which the data values are implicitly or explicitly
        /// averaged or aggregated.
        /// </summary>
        public virtual double TimeSupport { get; set; }
        /// <summary>
        /// Text value indicating what
        /// type of data value is being
        /// recorded. This should be from
        /// the ValueTypeCV controlled
        /// vocabulary table.
        /// </summary>       
        public virtual string DataType { get; set; }
        /// <summary>
        /// General category of the variable from the GeneralCategoryCV table
        /// </summary>
        public virtual string GeneralCategory { get; set; }
        /// <summary>
        /// Numeric value used to encode no data values for this variable
        /// </summary>
        public virtual double NoDataValue { get; set; }
        /// <summary>
        /// The WaterML vocabulary prefix (part of the VariableCode before the ":" separator)
        /// </summary>
        public virtual string VocabularyPrefix { get; set; }

        /// <summary>
        /// reference to the unit of the data values associated with this variable
        /// </summary>
        public virtual Unit VariableUnit { get; set; }

        /// <summary>
        /// Units of the time support of this variable. If TimeSupport is 0,
        /// indicating an instantaneous observation, a unit needs to still
        /// be given for completeness, although it is somewhat arbitrary.
        /// </summary>
        public virtual Unit TimeUnit { get; set; }
        /// <summary>
        /// returns the name of the unit
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #region Equality
        /// <summary>
        /// Two units are considered equal if they have the same unit code
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(BaseEntity other)
        {
            if (other is Variable)
            {
                return ((Variable)other).Code.Equals(this.Code);
            }
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
        #endregion
    }
}
