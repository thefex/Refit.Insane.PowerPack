using System;
using System.Threading.Tasks;

namespace Refit.Insane.PowerPack.Caching
{
    public interface IPersistedCache
    {
        /// <summary>
        /// Save the specified value to cache at key.
        /// </summary>
        /// <returns></returns>
        /// <param name="atKey">At key.</param>
        /// <param name="valueToCache">Value to cache.</param>
        /// <param name="timeToLive">Time to live of cache value - in case it's null - it will not be after this time.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task Save<T>(string atKey, T valueToCache, TimeSpan? timeToLive);

        /// <summary>
        /// Gets cache value at the specified key.
        /// </summary>
        /// <returns>Cached value</returns>
        /// <param name="atKey">At key.</param>
        /// <typeparam name="TResult">Cached value type.</typeparam>
        Task<TResult> Get<TResult>(string atKey);

        /// <summary>
        /// Deletes the cached value at specified key.
        /// </summary>
        /// <returns></returns>
        /// <param name="cachedValueAtKey">Cached value key to delete.</param>
        Task Delete(string cachedValueAtKey);
    }
}
