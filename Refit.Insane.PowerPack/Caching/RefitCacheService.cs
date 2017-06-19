using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Caching.Internal;

namespace Refit.Insane.PowerPack.Caching
{
    public class RefitCacheService
    {
        private static Lazy<RefitCacheService> _lazyInstance = new Lazy<RefitCacheService>(() => new RefitCacheService());
        private RefitCacheController _refitCacheController;
        private IPersistedCache persistedCache;

        private RefitCacheService()
        {
            _refitCacheController = new RefitCacheController();
            persistedCache = new AkavachePersistedCache();
        }

        public Task UpdateCache<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> forApiMethodCall, TResult newCacheValue)
        {
            if (!_refitCacheController.IsMethodCacheable(forApiMethodCall))
                return Task.FromResult(true);

            var cacheKey = _refitCacheController.GetCacheKey(forApiMethodCall);
            var refitCacheAttribute = _refitCacheController.GetRefitCacheAttribute(forApiMethodCall);

            return persistedCache.Save(cacheKey, newCacheValue, refitCacheAttribute.CacheAttribute.CacheTtl);
        }

        public Task ClearCache<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> forApiMethodCall){
			if (!_refitCacheController.IsMethodCacheable(forApiMethodCall))
				return Task.FromResult(true);

			var cacheKey = _refitCacheController.GetCacheKey(forApiMethodCall);

            return persistedCache.Delete(cacheKey);
        }

        /// <summary>
        /// By default it is using Akavache - if you're writting unit tests you can substitute it with your mock.
        /// </summary>
        /// <param name="persistedCache">Persisted cache.</param>
        public void SetPersistedCache(IPersistedCache persistedCache)
        {
            this.persistedCache = persistedCache;  
        }

        public static RefitCacheService Instance => _lazyInstance.Value;
    }
}
