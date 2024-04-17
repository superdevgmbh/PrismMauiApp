using System.Diagnostics.CodeAnalysis;

namespace PrismMauiApp.Services.Login
{
    [ExcludeFromCodeCoverage]
    public class IdentityServiceMock : IIdentityService
    {
        public async Task LoginAsync(string username, string password)
        {
            await Task.Delay(1000);
        }
    }
}
