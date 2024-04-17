using PrismMauiApp.Model;

namespace PrismMauiApp.Services.Login
{
    public class IdentityService : IIdentityService
    {
        private readonly IApiService apiService;

        public IdentityService(IApiService apiService)
        {
            //var handler = new HttpClientHandler();
            //handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

            //this.httpClient = new HttpClient(handler)
            //{
            //    BaseAddress = new Uri("https://192.168.10.1:5001"),
            //};
            //this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            //this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain));
            this.apiService = apiService;
        }

        public async Task LoginAsync(string username, string password)
        {
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };

            var token = await this.apiService.PostAsync<LoginDto, string>("api/identity/login", loginDto);

            token = token.Replace("\"", "");

            this.apiService.UpdateAuthorization(new TokenModel { AccessToken = token });
        }
    }
}
