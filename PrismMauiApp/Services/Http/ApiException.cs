namespace PrismMauiApp.Services.Http
{
    /// <summary>
    ///     A general error raised by the API.
    /// </summary>
    public class ApiException : Exception
    {
        internal ApiException(string message)
            : base(message)
        {
        }

        internal ApiException(string message, string requestMethod, string requestUri, string correlationId, string exception)
            : base(message)
        {
            this.RequestMethod = requestMethod;
            this.RequestUri = requestUri;
            this.CorrelationId = correlationId;
            this.Exception = exception;
        }

        public string RequestMethod { get; }

        public string RequestUri { get; }

        public string CorrelationId { get; }

        public string Exception { get; }
    }
}