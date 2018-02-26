using System;
using System.Runtime.Serialization;

namespace MockIdentityProvider
{
    [Serializable]
    internal class RouteHandlerNotAssignedException : Exception
    {
        public RouteHandlerNotAssignedException()
        {
        }

        public RouteHandlerNotAssignedException(string message) : base(message)
        {
        }

        public RouteHandlerNotAssignedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RouteHandlerNotAssignedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}