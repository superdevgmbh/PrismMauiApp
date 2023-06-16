using System.Globalization;
using System.Net;
using PrismMauiApp.Model;

namespace PrismMauiApp.Services.Http
{
    public interface IApiService : IDisposable
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);

        Task<string> GetAsync(string requestUri);

        Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken cancellationToken = default, TimeSpan? cacheExpiration = null);

        Task PostAsync(string requestUri, object payload);

        Task<TResult> PostAsync<TResult>(string requestUri, TResult payload);

        Task<TResult> PostAsync<TRequest, TResult>(string requestUri, TRequest payload);

        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content = null);

        Task<TResult> PutAsync<TResult>(string requestUri, HttpContent content);

        Task<TResult> PutAsync<TResult>(string requestUri, TResult payload);

        Task<TResult> PutAsync<TRequest, TResult>(string requestUri, TRequest payload);

        Task DeleteAsync(string requestUri);

        void UpdateAuthorization(TokenModel authenticationToken);

        void UpdateLanguage(CultureInfo cultureInfo);

        event EventHandler<RequestErrorEventArgs> RequestError;

        event EventHandler<ResponseErrorEventArgs> ResponseError;
    }

    public class ResponseErrorEventArgs : EventArgs
    {
        public HttpStatusCode StatusCode { get; }

        public ResponseErrorEventArgs(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }
    }

    public class RequestErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public RequestErrorEventArgs(Exception exception)
        {
            this.Exception = exception;
        }
    }
}