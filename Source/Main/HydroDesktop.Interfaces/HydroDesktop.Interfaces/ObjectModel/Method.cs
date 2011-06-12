using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents an observation method or a data derivation / simulation /
    /// modeling method
    /// </summary>
    public class Method : BaseEntity
    {
        public Method()
        {
            Code = 0;
            Description = "unknown";
            Link = "unknown";
        }
        
        /// <summary>
        /// The code of the method (optional)
        /// </summary>
        public virtual int Code { get; set; }

        public virtual string Description { get; set; }

        public virtual string Link { get; set; }

        public override string ToString()
        {
            return Description;
        }

        /// <summary>
        /// When the method is unknown
        /// </summary>
        public static Method Unknown
        {
            get
            {
                return new Method
                {
                    Description = Constants.Unknown,
                    Link = Constants.Unknown,
                    Code = 0
                };
            }
        }
    }
}
