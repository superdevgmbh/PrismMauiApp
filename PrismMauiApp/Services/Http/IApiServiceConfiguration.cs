using System.Globalization;
using PrismMauiApp.Model;

namespace PrismMauiApp.Services.Http
{
    public class DefaultApiServiceConfiguration : IApiServiceConfiguration
    {
        public TokenModel AuthenticationToken { get; set; }

        public string BaseUrl { get; } = "https://192.168.10.1:5001";

        public string ApiVersion { get; set; }

        public TimeSpan Timeout { get; set; }

        public bool IsDebug { get; set; }

        public CultureInfo Language { get; set; }

        public event EventHandler<ChangedLanguageEventArgs> LanguageChanged;
    }

    public interface IApiServiceConfiguration
    {
        TokenModel AuthenticationToken { get; set; }

        string BaseUrl { get; }

        string ApiVersion { get; }

        TimeSpan Timeout { get; }

        bool IsDebug { get; }

        CultureInfo Language { get; }

        event EventHandler<ChangedLanguageEventArgs> LanguageChanged;
    }

    public class ChangedLanguageEventArgs : EventArgs
    {
        public ChangedLanguageEventArgs(CultureInfo cultureInfo)
        {
            this.CultureInfo = cultureInfo;
        }

        public CultureInfo CultureInfo { get; }
    }
}