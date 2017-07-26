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
            ApiTimeout = 5;
        }

        public int ApiTimeout { get; }

        public string BaseUri { get; }
        
        public Type HttpClientHandlerType { get; }
    }
}