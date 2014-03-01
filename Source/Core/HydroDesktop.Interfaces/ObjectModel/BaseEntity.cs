using System;
using System.Collections.Generic;
using System.Linq;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Base Entity overrides the comparison operators 
    /// so that the persistence tests will work when comparing
    /// referenced entities during testing.
    /// </summary>
    /// <remarks>From http://pastie.org/434198</remarks>
    [Serializable]
    public class BaseEntity : IEquatable<BaseEntity>, ICloneable
    {
        /// <summary>
        /// Id (primary key) of the entity
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// Two entities are considered equal if they have the same Id
        /// </summary>
        /// <param name="other">other entity</param>
        /// <returns>true if the entities are considered equal</returns>
        public virtual bool Equals(BaseEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id == 0 || other.Id == 0) return false;
            return other.Id == Id;
        }

        /// <summary>
        /// A base entity and another object are considered equal
        /// only if they represent a reference to the same object
        /// instance
        /// </summary>
        /// <param name="obj">the compared object</param>
        /// <returns>true if the objects are considered equal</returns>
        public override bool Equals(object obj)
        {
            if (obj.Equals(DBNull.Value)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            return Equals((BaseEntity)obj);
        }

        /// <summary>
        /// Creates a hash code identifier using the Id
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
           return (Id.GetHashCode() * 397) ^ GetType().GetHashCode();
        }

        /// <summary>
        /// equals operator for comparing two base entities
        /// </summary>
        /// <param name="left">left side of the operator</param>
        /// <param name="right">right side of the operator</param>
        /// <returns>true if entities are considered equal</returns>
        public static bool operator ==(BaseEntity left, BaseEntity right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// not equals operator for comparing two base entities
        /// </summary>
        /// <param name="left">left side of != operator</param>
        /// <param name="right">right side of != operator</param>
        /// <returns>true if entities are considered equal</returns>
        public static bool operator !=(BaseEntity left, BaseEntity right)
        {
            return !Equals(left, right);
        }

        # region Validation
       
        /// <summary>
        /// Return true if the object is valid
        /// </summary>
        public virtual bool IsValid
        {
            get { return (!GetRuleViolations().Any()); }
        }

        /// <summary>
        /// Show all rule violations
        /// </summary>
        /// <returns>rule violations</returns>
        public virtual IEnumerable<RuleViolation> GetRuleViolations()
        {
            yield break; // basically, empty
        }

        #endregion

        public object Clone()
        {
            var result = (BaseEntity)MemberwiseClone();
            OnCopy(result);
            return result;
        }

        protected  virtual void OnCopy(BaseEntity copy)
        {
            // do nothing here
        }
    }
}
