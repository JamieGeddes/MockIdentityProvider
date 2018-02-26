using System;
using System.Net;

namespace MockIdentityProvider
{
    public class GenerateResponseArgs: EventArgs
    {
        public HttpListenerRequest Request { get; set; }
        public HttpListenerResponse Response { get; set; }
    }
}
