using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;

namespace Refit.Insane.PowerPack.Caching.Internal
{
    public class AkavachePersistedCache : IPersistedCache
    {
        public AkavachePersistedCache()
        {
        }

        public async Task Delete(string cachedValueAtKey)
        {
            await BlobCache.LocalMachine.Invalidate(cachedValueAtKey);
        }

        public async Task<TResult> Get<TResult>(string atKey)
        {
            return await BlobCache.LocalMachine.GetObject<TResult>(atKey).Catch(Observable.Return(default(TResult)));
        }

        public async Task Save<T>(string atKey, T valueToCache, TimeSpan? timeToLive)
        {
            if (timeToLive.HasValue)
                await BlobCache.LocalMachine.InsertObject(atKey, valueToCache, timeToLive.Value);
            else
                await BlobCache.LocalMachine.InsertObject(atKey, valueToCache);
        }

    }
}
