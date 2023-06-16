namespace PrismMauiApp.Services.Http
{
    /// <summary>
    ///     This exception occurs if the API service receives any evidence of a security impact.
    /// </summary>
    /// <example>
    ///     Http Response Code is Unauthorized or Forbidden.
    /// </example>
    public class ApiSecurityException : ApiException
    {
        internal ApiSecurityException(string message) : base(message)
        {
        }

        internal ApiSecurityException(string message, string requestMethod, string requestUri, string correlationId, string exception)
            : base(message, requestMethod, requestUri, correlationId, exception)
        {
        }
    }
}