using Jwt.Proxy.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jwt.Proxy.Services
{
    public interface IProxyService
    {
        bool TokenHasRequiredProperties(JsonWebToken token);
        bool TokenSatisfiesUrlConstraints(JsonWebToken token, HttpContext context);
    }
}
