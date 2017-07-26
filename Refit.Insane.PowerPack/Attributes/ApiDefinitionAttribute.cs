using System;

namespace Refit.Insane.PowerPack.Attributes
{
    public class ApiDefinitionAttribute : Attribute
    {
        public ApiDefinitionAttribute(string baseUri) : this()
        {
            BaseUri = baseUri;
            HttpClientHandlerType = typeof(HttpClientDiagnosticsHandler);
        }
        
        public ApiDefinitionAttribute(string baseUri, Type httpClientHandlerType) : this()
        {
            BaseUri = baseUri;
            HttpClientHandlerType = httpClientHandlerType;
        }

        private ApiDefinitionAttribute()
        {
            ApiTimeout = TimeSpan.FromSeconds(5);
        }

        public TimeSpan ApiTimeout { get; }

        public string BaseUri { get; }
        
        public Type HttpClientHandlerType { get; }
    }
}