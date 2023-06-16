using System.Diagnostics;
using Newtonsoft.Json;

namespace PrismMauiApp.Model
{
    [DebuggerDisplay("TokenModel: UserId={this.UserId}, Username={this.Username}, Expires={this.Expires}")]
    public class TokenModel
    {
        public static readonly TokenModel Default = new TokenModel();

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(".issued")]
        public DateTime Issued { get; set; }

        [JsonProperty(".expires")]
        public DateTime Expires { get; set; }

        public bool IsValid()
        {
            return this != Default &&
                   !string.IsNullOrEmpty(this.AccessToken) &&
                   DateTime.Compare(this.Expires, DateTime.UtcNow) > 0;
        }
    }
}