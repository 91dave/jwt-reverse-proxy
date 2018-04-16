using System;
using System.Collections.Generic;
using System.Text;

namespace Jwt.Proxy.ConfigSettings
{
    public class ProxySettings
    {
        public string BackendHost { get; set; }
        public int BackendPort { get; set; } = 80;
        public string BackendScheme { get; set; } = "http";
        public ProxyMode Mode { get; set; }
        public string RedirectUrl { get; set; }
        public string UrlFormat { get; set; }
        public RequiredKeyValue[] RequiredKeys { get; set; }
    }

    public class RequiredKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public enum ProxyMode
    {
        Redirect,
        Http403
    }
}
