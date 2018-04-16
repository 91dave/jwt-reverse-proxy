using System;
namespace Jwt.Proxy.ConfigSettings
{
    public class JwtSettings
    {
        public string Secret { get; set; } = "secret";
        public string Cookie { get; set; }
        public string QueryString { get; set; }
    }
}
