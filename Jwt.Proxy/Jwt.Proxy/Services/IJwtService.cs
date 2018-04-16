using Jwt.Proxy.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jwt.Proxy.Services
{
    public interface IJwtService
    {
        JsonWebToken GetTokenFromContext(HttpContext context);
    }
}
