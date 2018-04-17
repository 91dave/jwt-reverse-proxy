using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Jwt.Proxy.ConfigSettings;
using Jwt.Proxy.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Jwt.Proxy.Services
{
    public class ProxyService : IProxyService
    {
        private readonly ProxySettings _settings;

        public ProxyService(IOptions<ProxySettings> settings)
        {
            _settings = settings.Value;
        }

        public bool TokenHasRequiredProperties(JsonWebToken token)
        {
            if (token == null)
            {
                Console.WriteLine("Token was null");
                return false;
            }

            if (!token.IsValid)
            {
                Console.WriteLine("Token was Invalid");
                return false;
            }

            if (_settings.RequiredKeys?.Where(r => r.Type == RequiredKeyType.LiteralValue)?.Any() ?? false)
            {
                foreach (var requiredKey in _settings.RequiredKeys.Where(r => r.Type == RequiredKeyType.LiteralValue))
                {
                    Console.WriteLine($"Checking that token has {requiredKey.Key} == {requiredKey.Value}");

                    if (TokenHasKeyValue(token, requiredKey.Key, requiredKey.Value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                Console.WriteLine("No RequiredKeys to check");
                return true;
            }

            return false;
        }

        public bool TokenSatisfiesUrlConstraints(JsonWebToken token, HttpContext context)
        {
            // Setting empty - not checking
            if (String.IsNullOrWhiteSpace(_settings.UrlFormat))
            {
                Console.WriteLine("No UrlFormat to check");
                return true;
            }

            // Check request matches URL format
            Console.WriteLine($"Checking Path '{context.Request.Path}' against '{_settings.UrlFormat}'");

            var urlFormatExpression = _settings.UrlFormat.Replace("{", "(?<").Replace("}", ">.+)").Replace("/", "\\/");
            Regex regex = new Regex(urlFormatExpression);
            var match = regex.Match(context.Request.Path);

            if (match.Success)
            {
                foreach (Group group in match.Groups.Skip(1))
                {
                    Console.WriteLine($"Checking that token.{group.Name} == {group.Value}");

                    if (!String.IsNullOrEmpty(group.Name))
                    {
                        // If any key/value checks fail - disallow
                        if (!TokenHasKeyValue(token, group.Name, group.Value))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Path does not match, allowing request through");
            }

            // Request has specific querystring parameters that match
            if (_settings.RequiredKeys?.Where(r => r.Type == RequiredKeyType.FromQueryString)?.Any() ?? false)
            {
                foreach (var requiredKey in _settings.RequiredKeys.Where(r => r.Type == RequiredKeyType.FromQueryString))
                {
                    if (context.Request.Query.ContainsKey(requiredKey.Value))
                    {
                        Console.WriteLine($"Checking that token has {requiredKey.Key} == query[{requiredKey.Value}]");

                        if (!TokenHasKeyValue(token, requiredKey.Key, context.Request.Query[requiredKey.Value]))
                        {
                            return false;
                        }
                    }
                }
            }

            // If request does not match OR no key/value checks failed - allow
            return true;
        }

        #region Private Helpers
        private bool TokenHasKeyValue(JsonWebToken token, string key, string value)
        {
            if (token.PayloadData.ContainsKey(key))
            {
                if (token.PayloadData[key].ToString().ToLowerInvariant() == value.ToLowerInvariant())
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
