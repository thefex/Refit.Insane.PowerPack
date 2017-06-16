using System;
using System.Threading.Tasks;

namespace Refit.Insane.PowerPack.Caching
{
    public interface IPersistedCache
    {
        /// <summary>
        /// Save the specified value to cache at key.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="atKey">At key.</param>
        /// <param name="valueToCache">Value to cache.</param>
        /// <param name="timeToLive">Time to live of cache value - in case it's null - it will not be after this time.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task Save<T>(string atKey, T valueToCache, TimeSpan? timeToLive);
        Task<TResult> Get<TResult>(string atKey);
    }
}
