using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jose;
using Jwt.Proxy.ConfigSettings;
using Jwt.Proxy.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Jwt.Proxy.Services
{
    public class JwtService : IJwtService
    {
        private readonly ConfigSettings.JwtSettings _settings;

        public JwtService(IOptions<ConfigSettings.JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public JsonWebToken GetTokenFromContext(HttpContext context)
        {
            try
            {
                JsonWebToken result = JWT.Decode<JsonWebToken>(GetRawToken(context), Encoding.ASCII.GetBytes(_settings.Secret), JwsAlgorithm.HS256);
                
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        #region Private Helpers
        private string GetRawToken(HttpContext context, bool debug = false)
        {
            var request = context.Request;
            string raw = "";

            if (request.Headers.ContainsKey("Authorization") &&
                !String.IsNullOrWhiteSpace(request.Headers["Authorization"].FirstOrDefault()) &&
                request.Headers["Authorization"].FirstOrDefault().StartsWith("Bearer "))
            {
                raw = request.Headers["Authorization"].FirstOrDefault().Substring("Bearer ".Length);

                return raw;
            }

            var cookieName = _settings.Cookie;

            if (!String.IsNullOrWhiteSpace(cookieName) && request.Cookies.ContainsKey(cookieName) && !String.IsNullOrWhiteSpace(request.Cookies[cookieName]))
            {
                raw = request.Cookies[cookieName];
                
                return raw;
            }

            string queryStringName = _settings.QueryString;

            if (request.Query.ContainsKey(queryStringName) && !String.IsNullOrWhiteSpace(request.Query[queryStringName]))
            {
                raw = request.Query[queryStringName];

                return raw;
            }

            return String.Empty;
        }
        #endregion
    }
}
