using System;

namespace Refit.Insane.PowerPack.Attributes
{
    public class ApiDefinitionAttribute : Attribute
    {
        private const int DefaultTimeoutInSeconds = 5;

        public ApiDefinitionAttribute(Type httpClientHandlerType) : this(DefaultTimeoutInSeconds)
        {
            HttpClientHandlerType = httpClientHandlerType;
        }

        public ApiDefinitionAttribute(string baseUri) : this(DefaultTimeoutInSeconds)
        {
            BaseUri = baseUri;
            HttpClientHandlerType = typeof(HttpClientDiagnosticsHandler);
        }

        public ApiDefinitionAttribute(string baseUri, int apiTimoutInSeconds) : this(apiTimoutInSeconds)
        {
            BaseUri = baseUri;
            HttpClientHandlerType = typeof(HttpClientDiagnosticsHandler);
        }

        public ApiDefinitionAttribute(string baseUri, Type httpClientHandlerType) : this(baseUri)
        {
            HttpClientHandlerType = httpClientHandlerType;
        }

        public ApiDefinitionAttribute(string baseUri, int apiTimeoutInSeconds, Type httpClientHandlerType) : this(baseUri, apiTimeoutInSeconds)
        {
            HttpClientHandlerType = httpClientHandlerType;
        }

        private ApiDefinitionAttribute(int apiTimeoutInSeconds)
        {
            ApiTimeout = TimeSpan.FromSeconds(apiTimeoutInSeconds);
        }

        public TimeSpan ApiTimeout { get; }

        public string BaseUri { get; }

        public Type HttpClientHandlerType { get; }
    }
}
