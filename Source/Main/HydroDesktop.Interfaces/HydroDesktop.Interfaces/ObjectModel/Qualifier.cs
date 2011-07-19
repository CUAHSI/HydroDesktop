using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// The qualifier provides additional information about a data value
    /// this includes information about special conditions that occurred
    /// during the observation.
    /// </summary>
    public class Qualifier : BaseEntity
    {
        public Qualifier()
        { }

        public Qualifier(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }
        
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }

        /// <summary>
        /// Returns true if this qualifier is a composite qualifier
        /// (combination of two different qualifiers such as "P, ice")..
        /// </summary>
        public virtual bool IsCompositeQualifier
        {
            get
            {
                return (Code.IndexOf(",") >= 0);
            }
        }

        public override string ToString()
        {
            return Code;
        }

        /// <summary>
        /// Two qualifieres are equal if they have the same qualifier code.
        /// </summary>
        public override bool Equals(object obj)
        {
            Qualifier other = obj as Qualifier;
            if (other == null) return base.Equals(obj);

            return Code.Equals(other.Code);
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Code)) return base.GetHashCode();
            return Code.GetHashCode();
        }

        /// <summary>
        /// If the qualifier is a composite qualifier, split it into a list of
        /// simple qualifiers
        /// </summary>
        /// <returns>The collection of simple qualifiers</returns>
        public virtual IList<Qualifier> GetSimpleQualifiers()
        {
            List<Qualifier> resultList = new List<Qualifier>();
            if (IsCompositeQualifier == false)
            {
                resultList.Add(this);
            }
            else
            {
                string[] separators = new string[] {","};
                string[] codes = Code.Split(separators,StringSplitOptions.RemoveEmptyEntries);
                string[] descriptions = Description.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < codes.Length; i++)
                {
                    Qualifier newQual = new Qualifier();
                    newQual.Code = codes[i].Trim();
                    newQual.Description = descriptions[i].Trim();
                    resultList.Add(newQual);
                }
            }
            return resultList;
        }

        /// <summary>
        /// Creates a new composite qualifier. This is a qualifier with multiple codes
        /// for example 'p, ice'. This is used to simplify access to the database in
        /// cases when one data value can have only one qualifier.
        /// </summary>
        /// <param name="qualifiers">The list of multiple qualifiers</param>
        /// <returns>The new composite qualifier</returns>
        public static Qualifier CreateCompositeQualifier(IList<Qualifier> qualifiers)
        {
            string newDescription = "";
            string newCode = "";
            foreach (Qualifier qual in qualifiers)
            {
                newDescription += qual.Description + ", ";
                newCode += qual.Code + ", ";
            }
            newDescription = newDescription.Remove(newDescription.LastIndexOf(","));
            newCode = newCode.Remove(newCode.LastIndexOf(","));

            Qualifier newQualifier = new Qualifier();
            newQualifier.Code = newCode;
            newQualifier.Description = newDescription;

            return newQualifier;
        }
    }

    
}
