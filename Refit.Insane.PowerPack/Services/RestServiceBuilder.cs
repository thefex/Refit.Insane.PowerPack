using System;
using System.Net.Http;
using System.Reflection;

namespace Refit.Insane.PowerPack.Services
{
    public class RestServiceBuilder
    {
        bool isAutoRetryEnabled = false;
        bool isCacheEnabled = false;

        public RestServiceBuilder WithAutoRetry(bool shouldEnableAutoRetry = true)
        {
            isAutoRetryEnabled = shouldEnableAutoRetry;
            return this;
        }

        public RestServiceBuilder WithCaching(bool shouldEnableCache = true)
        {
            isCacheEnabled = shouldEnableCache;
            return this;
        }

        public IRestService BuildRestService(Func<HttpClient> httpClientFactory, Assembly restApiAssembly) 
        {
            IRestService refitRestService = new RefitRestService(httpClientFactory);

            if (isAutoRetryEnabled)
                refitRestService = new RefitRestServiceRetryProxy(refitRestService, restApiAssembly);

            if (isCacheEnabled)
                refitRestService = new RefitRestServiceCachingDecorator(refitRestService, new Caching.Internal.RefitCacheController());

            return refitRestService;
        }
    }
}
