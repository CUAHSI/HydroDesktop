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
        /// <summary>
        /// Creates a new unknown method object
        /// </summary>
        public Method()
        {
            Code = 0;
            Description = Constants.Unknown;
            Link = Constants.Unknown;
        }
        
        /// <summary>
        /// The code of the method (optional)
        /// </summary>
        public virtual int Code { get; set; }
        /// <summary>
        /// Method description
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// Method link (web address)
        /// </summary>
        public virtual string Link { get; set; }
        /// <summary>
        /// Shows the method description
        /// </summary>
        /// <returns>Method description</returns>
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
