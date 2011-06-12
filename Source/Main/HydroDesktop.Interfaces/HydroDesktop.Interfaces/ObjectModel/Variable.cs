using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    public class Variable : BaseEntity
    { 
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Speciation { get; set; }
        
        public virtual string SampleMedium { get; set; }
        public virtual string ValueType { get; set; }
        public virtual bool IsRegular { get; set; }
        public virtual bool IsCategorical { get; set; }
        public virtual double TimeSupport { get; set; }
        
        public virtual string DataType { get; set; }
        public virtual string GeneralCategory { get; set; }
        public virtual double NoDataValue { get; set; }
        
        public virtual string VocabularyPrefix { get; set; }

        //The unit of the variable
        public virtual Unit VariableUnit { get; set; }

        //The time unit
        public virtual Unit TimeUnit { get; set; }

        public override string ToString()
        {
            return Name;
        }

        #region Equality
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
