using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Plugins.Search.Searching.Exceptions
{
    public class SearchSettingsValidationException : Exception
    {
        public SearchSettingsValidationException()
        {
        }

        public SearchSettingsValidationException(Exception inner)
            : this(null, inner)
        {

        }

        public SearchSettingsValidationException(string message, Exception inner = null)
            : base(message, inner)
        {

        }

        public SearchSettingsValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}