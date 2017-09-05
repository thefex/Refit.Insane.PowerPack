using System;

namespace Refit.Insane.PowerPack.Attributes
{
    public class ApiDefinitionAttribute : Attribute
    {
        public ApiDefinitionAttribute(string baseUri) : this(5)
        {
            BaseUri = baseUri;
            HttpClientHandlerType = typeof(HttpClientDiagnosticsHandler);
        }
        
        public ApiDefinitionAttribute(string baseUri, int apiTimoutInSeconds) : this(apiTimoutInSeconds)
        {
            BaseUri = baseUri;
            HttpClientHandlerType = typeof(HttpClientDiagnosticsHandler);
        }
        
        public ApiDefinitionAttribute(string baseUri, int apiTimeoutInSeconds, Type httpClientHandlerType) : this(apiTimeoutInSeconds)
        {
            BaseUri = baseUri;
            HttpClientHandlerType = httpClientHandlerType;
        }
        
        public ApiDefinitionAttribute(string baseUri, Type httpClientHandlerType) : this(5)
        {
            BaseUri = baseUri;
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
