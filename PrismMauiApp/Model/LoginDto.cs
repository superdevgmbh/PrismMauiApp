using Newtonsoft.Json;

namespace PrismMauiApp.Model
{
    public class LoginDto
    {
        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}