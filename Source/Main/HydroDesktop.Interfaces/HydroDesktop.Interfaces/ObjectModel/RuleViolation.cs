using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Information about violation of rules
    /// </summary>
    public class RuleViolation
    {
        /// <summary>
        /// Rule violation error message
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// rule violation property name
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Creates a new instance of a rule violation
        /// </summary>
        /// <param name="errorMessage">the error message</param>
        public RuleViolation(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        /// <summary>
        /// Creates a new instance of a rule violation
        /// </summary>
        /// <param name="errorMessage">the rule violation error message</param>
        /// <param name="propertyName">the rule violation property name</param>
        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
    }
}
