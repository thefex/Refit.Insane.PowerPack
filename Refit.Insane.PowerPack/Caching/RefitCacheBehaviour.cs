namespace Refit.Insane.PowerPack.Caching
{
    public enum RefitCacheBehaviour
    {
        /// <summary>
        /// If caching disabled - API is always called.
        /// If caching enabled - calls API only when there is no cache available. 
        /// </summary>
        Default,
        /// <summary>
        /// If caching disabled - default behaviour.
        /// If caching enabled - always calls API (but never clears current cache), in case request fails and there is already value in cache - successful response is returned with cached value  
        /// </summary>
        ForceUpdateFallbackToCache,
        /// <summary>
        /// If caching disabled - default behaviour.
        /// If caching enabled - always calls API (but never clears current cache), in case request fails - failed response will be returned (even if there is value in cache!)
        /// </summary>
        ForceUpdateWithoutFallbackToCache
    }
}