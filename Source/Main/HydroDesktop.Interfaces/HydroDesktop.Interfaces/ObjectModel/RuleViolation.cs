using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    public class RuleViolation
    {
        public string ErrorMessage { get; private set; }
        public string PropertyName { get; private set; }

        public RuleViolation(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
    }
}
