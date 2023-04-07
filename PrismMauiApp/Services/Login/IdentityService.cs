using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PrismMauiApp.Services.Login
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient httpClient;

        public IdentityService()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

            this.httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://192.168.10.1:5001"),
            };
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain));
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };

            var jsonContent = JsonConvert.SerializeObject(loginDto);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await this.httpClient.PostAsync("api/identity/login", stringContent);
            response.EnsureSuccessStatusCode();

            var bearerToken = await response.Content.ReadAsStringAsync();
            return bearerToken;
        }
    }
}
