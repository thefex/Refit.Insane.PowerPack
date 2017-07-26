using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace Refit.Insane.PowerPack.Services
{
    public class RestServiceBuilder
    {
        bool isAutoRetryEnabled = true;
        bool isCacheEnabled = true;

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
        
        public IRestService BuildRestService(Assembly restApiAssembly) 
        {
            var refitRestService = new RefitRestService();

            return BuildRestService(refitRestService, restApiAssembly);
        }

        public IRestService BuildRestService(IDictionary<Type, DelegatingHandler> handlerImplementations, Assembly restApiAssembly) 
        {
            var refitRestService = new RefitRestService(handlerImplementations);

            return BuildRestService(refitRestService, restApiAssembly);
        }
        
        public IRestService BuildRestService(IDictionary<Type, Func<DelegatingHandler>> handlerFactories, Assembly restApiAssembly) 
        {
            var refitRestService = new RefitRestService(handlerFactories);

            return BuildRestService(refitRestService, restApiAssembly);
        }

        public IRestService BuildRestService(RefitRestService restService, Assembly restApiAssembly){
            IRestService refitRestService = restService;

			if (isAutoRetryEnabled)
				refitRestService = new RefitRestServiceRetryProxy(refitRestService, restApiAssembly);

			if (isCacheEnabled)
				refitRestService = new RefitRestServiceCachingDecorator(refitRestService, new Caching.Internal.RefitCacheController());

			return refitRestService;
        }
    }
}
