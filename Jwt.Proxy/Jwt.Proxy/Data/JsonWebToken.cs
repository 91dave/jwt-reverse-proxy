using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jwt.Proxy.Data
{
    public class JsonWebToken
    {
        [JsonIgnore]
        public DateTime NotBefore { get; set; } = DateTime.MinValue;

        [JsonProperty("nbf")]
        public int EpochNotBefore
        {
            get
            {
                return (Int32)(Expiry.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
            set
            {
                NotBefore = new DateTime(1970, 1, 1).AddSeconds(value);
            }
        }

        [JsonIgnore]
        public DateTime Expiry { get; set; } = DateTime.MinValue;

        [JsonProperty("exp")]
        public int EpochExpiry
        {
            get
            {
                return (Int32)(Expiry.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
            set
            {
                Expiry = new DateTime(1970, 1, 1).AddSeconds(value);
            }
        }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return NotBefore < DateTime.UtcNow && Expiry > DateTime.UtcNow;
            }
        }

        [JsonExtensionData]
        public IDictionary<string, JToken> PayloadData { get; set; }
    }
}
