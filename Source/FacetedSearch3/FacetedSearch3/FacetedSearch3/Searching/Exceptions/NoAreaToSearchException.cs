using System;
using System.Runtime.Serialization;

namespace FacetedSearch3.Searching.Exceptions
{
    public class NoAreaToSearchException : SearchSettingsException
    {
        public NoAreaToSearchException()
        {

        }

        public NoAreaToSearchException(Exception inner)
            : base(inner)
        {
        }

        public NoAreaToSearchException(string message)
            : base(message)
        {
        }

        public NoAreaToSearchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoAreaToSearchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}