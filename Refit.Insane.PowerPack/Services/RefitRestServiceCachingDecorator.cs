using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Refit.Insane.PowerPack.Caching.Internal;
using Refit.Insane.PowerPack.Data;

namespace Refit.Insane.PowerPack.Services
{
    public class RefitRestServiceCachingDecorator : IRestService
    {
        private readonly IRestService _decoratedRestService;
        private readonly RefitCacheController _refitCacheController;

        public RefitRestServiceCachingDecorator(IRestService decoratedRestService, RefitCacheController refitCacheController)
        {
            _decoratedRestService = decoratedRestService;
            _refitCacheController = refitCacheController;
        }

        public async Task<Response<TResult>> Execute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> executeApiMethod)
        {
            if (!_refitCacheController.IsMethodCacheable(executeApiMethod))
                return await _decoratedRestService.Execute(executeApiMethod).ConfigureAwait(false);

            var cacheKey = _refitCacheController.GetCacheKey(executeApiMethod);
            var cachedValue = await Akavache.BlobCache.LocalMachine.GetObject<TResult>(cacheKey).Catch(Observable.Return(default(TResult)));

            if (cachedValue != null)
                return new Response<TResult>(cachedValue);

            var restResponse = await _decoratedRestService.Execute(executeApiMethod);

            if (restResponse.IsSuccess)
            {
                var refitCacheAttributes = _refitCacheController.GetRefitCacheAttribute<TApi, TResult>(executeApiMethod);

                if (refitCacheAttributes.CacheAttribute.CacheTtl.HasValue)
                    await BlobCache.LocalMachine.InsertObject(cacheKey, restResponse.Results, refitCacheAttributes.CacheAttribute.CacheTtl.Value);
                else
                    await BlobCache.LocalMachine.InsertObject(cacheKey, restResponse.Results);
            }

            return restResponse;
        }

        public Task<Response> Execute<TApi>(Expression<Func<TApi, Task>> executeApiMethod) => _decoratedRestService.Execute(executeApiMethod);


    }
}
