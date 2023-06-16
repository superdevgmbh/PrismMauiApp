using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Caching.Abstractions;
using Microsoft.Extensions.Caching.InMemory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PrismMauiApp.Model;

namespace PrismMauiApp.Services.Http
{
    public class ApiService : IApiService
    {
        private readonly JsonSerializerSettings serializerSettings;
        private readonly HttpClient httpClient;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger logger;

        public ApiService(ILogger<ApiService> logger, IApiServiceConfiguration apiServiceConfiguration, IMemoryCache memoryCache, HttpMessageHandler httpMessageHandler)
            : this(logger, CreateHttpClient(apiServiceConfiguration, httpMessageHandler), memoryCache)
        {
        }

        private ApiService(ILogger logger, HttpClient httpClient, IMemoryCache memoryCache)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

            this.serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            this.serializerSettings.Converters.Add(new StringEnumConverter());
        }

        private static HttpClient CreateHttpClient(IApiServiceConfiguration apiServiceConfiguration, HttpMessageHandler httpMessageHandler)
        {
            if (apiServiceConfiguration == null)
            {
                throw new ArgumentNullException(nameof(apiServiceConfiguration));
            }

            var httpClient = new HttpClient(httpMessageHandler);
            httpClient.BaseAddress = new Uri(apiServiceConfiguration.BaseUrl);
            //httpClient.DefaultRequestHeaders.Add(ApiVersions.HttpHeader, apiServiceConfiguration.ApiVersion);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            UpdateAuthorization(httpClient, apiServiceConfiguration.AuthenticationToken);
            UpdateAcceptLanguage(httpClient, apiServiceConfiguration.Language);

            if (apiServiceConfiguration.Timeout > TimeSpan.Zero)
            {
                httpClient.Timeout = apiServiceConfiguration.Timeout;
            }

            return httpClient;
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>
        ///     Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous
        ///     operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The request message was already sent by the
        ///     <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
            var httpResponseMessage = await this.httpClient.SendAsync(httpRequestMessage);
            return httpResponseMessage;
        }

        public async Task PostAsync(string requestUri, object payload)
        {
            var serialized = await Task.Run(() => JsonConvert.SerializeObject(payload, this.serializerSettings));
            var httpResponseMessage = await this.httpClient.PostAsync(requestUri, new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json));

            await this.HandleResponse(httpResponseMessage);
        }

        public Task<TResult> PostAsync<TResult>(string requestUri, TResult payload)
        {
            return this.PostAsync<TResult, TResult>(requestUri, payload);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string requestUri, TRequest payload)
        {
            if (payload is not HttpContent httpContent)
            {
                var serialized = await Task.Run(() => JsonConvert.SerializeObject(payload, this.serializerSettings));
                httpContent = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            }

            var httpResponseMessage = await this.httpClient.PostAsync(requestUri, httpContent);

            var jsonResponse = await this.HandleResponse(httpResponseMessage);

            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(jsonResponse, this.serializerSettings));
        }

        public async Task DeleteAsync(string requestUri)
        {
            var httpResponseMessage = await this.httpClient.DeleteAsync(requestUri);

            await this.HandleResponse(httpResponseMessage);
        }

        private static void UpdateAuthorization(HttpClient httpClient, TokenModel token)
        {
            if (token == null || token == TokenModel.Default)
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            }
        }

        public void UpdateAuthorization(TokenModel token)
        {
            UpdateAuthorization(this.httpClient, token);
        }

        public void UpdateLanguage(CultureInfo cultureInfo)
        {
            UpdateAcceptLanguage(this.httpClient, cultureInfo);

            // Clear cache because some queries return language-dependent content
            this.memoryCache.Clear();
        }

        private static void UpdateAcceptLanguage(HttpClient client, CultureInfo language)
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();

            if (language != null)
            {
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language.Name));
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            var httpResponseMessage = await this.httpClient.GetAsync(uri);

            return await this.HandleResponse(httpResponseMessage);
        }

        public async Task<TResult> GetAsync<TResult>(string uri, CancellationToken cancellationToken = default, TimeSpan? cacheExpiration = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var caching = cacheExpiration.HasValue;
            if (caching && this.memoryCache.TryGetValue(uri, out var result))
            {
                stopwatch.Stop();
                this.logger.LogDebug($"{nameof(this.GetAsync)} for Uri '{uri}' finished in {stopwatch.Elapsed /*.ToSecondsString()*/} (caching=true)");
                return (TResult)result;
            }

            var httpResponseMessage = await this.HandleRequest(() => this.httpClient.GetAsync(uri, cancellationToken));
            var jsonResponse = await this.HandleResponse(httpResponseMessage);
            result = await Task.Run(() => JsonConvert.DeserializeObject<TResult>(jsonResponse, this.serializerSettings));

            if (caching)
            {
                this.memoryCache.Set(uri, result, cacheExpiration.Value);
            }
            else
            {
                this.memoryCache.Remove(uri);
            }

            stopwatch.Stop();
            this.logger.LogDebug($"{nameof(this.GetAsync)} for Uri '{uri}' finished in {stopwatch.Elapsed}");
            return (TResult)result;
        }

        private async Task<TResult> HandleRequest<TResult>(Func<Task<TResult>> action, int maxAttempts = 2)
        {
            Exception lastException = null;
            for (var i = 0; i < maxAttempts; i++)
            {
                try
                {
                    return await action().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            this.OnRequestError(lastException);
            throw lastException;
        }

        public Task<TResult> PutAsync<TResult>(string uri, TResult payload)
        {
            return this.PutAsync<TResult, TResult>(uri, payload);
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content = null)
        {
            var httpResponseMessage = await this.httpClient.PutAsync(requestUri, content);

            await this.HandleResponse(httpResponseMessage);

            return httpResponseMessage;
        }

        public async Task<TResult> PutAsync<TResult>(string requestUri, HttpContent content)
        {
            var httpResponseMessage = await this.httpClient.PutAsync(requestUri, content);

            var jsonResponse = await this.HandleResponse(httpResponseMessage);

            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(jsonResponse, this.serializerSettings));
        }

        public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest payload)
        {
            var serialized = await Task.Run(() => JsonConvert.SerializeObject(payload, this.serializerSettings));
            var httpResponseMessage = await this.httpClient.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json));

            var jsonResponse = await this.HandleResponse(httpResponseMessage);

            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(jsonResponse, this.serializerSettings));
        }

        private async Task<string> HandleResponse(HttpResponseMessage response)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = GetException(response, jsonResponse);
                this.OnResponseError(response.StatusCode);
                throw exception;
            }

            return jsonResponse;
        }

        private static ApiException GetException(HttpResponseMessage response, string responseContent)
        {
            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                return new ApiSecurityException(
                    message: $"Request '{response.RequestMessage.Method} {response.RequestMessage.RequestUri.AbsoluteUri}' failed with StatusCode={response.StatusCode}",
                    requestMethod: response.RequestMessage.Method.Method,
                    requestUri: response.RequestMessage.RequestUri.AbsoluteUri,
                    correlationId: null,
                    exception: null);
            }

            var responseContentType = response.Content?.Headers?.ContentType;
            if (responseContentType != null)
            {
                if (responseContentType.MediaType == MediaTypeNames.Text.Plain)
                {
                    return new ApiException(
                        message: responseContent,
                        requestMethod: response.RequestMessage.Method.Method,
                        requestUri: response.RequestMessage.RequestUri.AbsoluteUri,
                        correlationId: null,
                        exception: null);
                }
            }

            return new ApiException(
                message: $"Request '{response.RequestMessage.Method} {response.RequestMessage.RequestUri.AbsoluteUri}' failed with StatusCode={response.StatusCode}, ReasonPhrase={response.ReasonPhrase}",
                requestMethod: response.RequestMessage.Method.Method,
                requestUri: response.RequestMessage.RequestUri.AbsoluteUri,
                correlationId: null,
                exception: null);
        }

        /// <summary>
        ///     Implemenation of <see cref="T:System.IDisposable" />
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Implemenation of <see cref="T:System.IDisposable" /> for
        ///     derived classes to use.
        /// </summary>
        /// <param name="disposing">
        ///     Indicates if being called from the Dispose() method
        ///     or the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.httpClient?.Dispose();
        }

        public event EventHandler<RequestErrorEventArgs> RequestError;

        protected virtual void OnRequestError(Exception exception)
        {
            this.RequestError?.Invoke(this, new RequestErrorEventArgs(exception));
        }

        public event EventHandler<ResponseErrorEventArgs> ResponseError;

        protected virtual void OnResponseError(HttpStatusCode statusCode)
        {
            this.ResponseError?.Invoke(this, new ResponseErrorEventArgs(statusCode));
        }
    }
}