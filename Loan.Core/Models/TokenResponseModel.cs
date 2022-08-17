using Newtonsoft.Json;
using System;

namespace Apps.Core.Models
{
    public class TokenResponseModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}