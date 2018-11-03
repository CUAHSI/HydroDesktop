using System;
using System.Runtime.Serialization;

namespace FacetedSearch3.Searching.Exceptions
{
    public class NoSelectedKeywordsException : SearchSettingsException
    {
        public NoSelectedKeywordsException()
        {
            
        }

        public NoSelectedKeywordsException(Exception inner) : base(inner)
        {
        }

        public NoSelectedKeywordsException(string message) : base(message)
        {
        }

        public NoSelectedKeywordsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NoSelectedKeywordsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}