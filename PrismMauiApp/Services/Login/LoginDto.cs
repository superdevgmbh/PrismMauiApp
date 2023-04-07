using Newtonsoft.Json;

namespace PrismMauiApp.Services.Login
{
    public class LoginDto
    {
        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}