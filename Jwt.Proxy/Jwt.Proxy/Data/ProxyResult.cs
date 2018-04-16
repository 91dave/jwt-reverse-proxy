using System;
using System.Collections.Generic;
using System.Text;

namespace Jwt.Proxy.Data
{
    public class ProxyResult
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Scheme { get; set; }
    }

    public class ProxyUnauthorisedResult
    {
        public bool StatusCode { get; set; }
        public string RedirectUrl { get; set; }
    }
}
