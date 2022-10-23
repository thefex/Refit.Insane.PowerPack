using System;

namespace Refit.Insane.PowerPack.Attributes
{
    public class ApiDefinitionAttribute : Attribute
    {
        private const int DefaultTimeoutInSeconds = 5;
        
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
        
        /// <summary>
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="apiTimeoutInSeconds"></param>
        /// <param name="httpClientHandlerType">
        /// If you are running in container enviornment, please let your handler inherit from SocketsHttpHandler and set PooledConnection ..
        /// othwerise you might run issues with DNS entires not updating.
        /// Also if you are using custom handler, make sure you are reusing HttpClientHandler
        /// (just inject it in constructor in your DelegatingHandler implementation)
        /// You can customize creation process in RefitRestService builder Func method params.
        /// </param>
        public ApiDefinitionAttribute(string baseUri, Type httpClientHandlerType) : this(baseUri)
        {
            HttpClientHandlerType = httpClientHandlerType;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="apiTimeoutInSeconds"></param>
        /// <param name="httpClientHandlerType">
        /// If you are running in container enviornment, please let your handler inherit from SocketsHttpHandler and set PooledConnection ..
        /// othwerise you might run issues with DNS entires not updating.
        /// Also if you are using custom handler, make sure you are reusing HttpClientHandler
        /// (just inject it in constructor in your DelegatingHandler implementation)
        /// You can customize creation process in RefitRestService builder Func method params.
        /// </param>
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
