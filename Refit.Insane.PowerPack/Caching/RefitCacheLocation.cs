namespace Refit.Insane.PowerPack.Caching
{
    public enum RefitCacheLocation
    {
        /// <summary>
        /// By default IPersistedCache (Akavache) - LocalMachine. This data is preserved between app run but may get deleted without notification
        /// (on iOS/Android to free up space, not deterministic when - but usually when its getting low)
        /// Ideal for things that are good to be cached (like some list data) but ok if gets deleted.
        /// Default value.
        /// </summary>
        Local,
        /// <summary>
        /// By default IPersistedCache (Akavache) - UserAccount. This data is preserved between app run and might be automatically synchronized to cloud (like iCloud on iOS)
        /// Ideal for things like user settings etc.. but don't use for passwords and sensitive data.
        /// </summary>
        UserAccount,
        /// <summary>
        /// By default IPersistedCache (Akavache) - Secure. This data is preserved between app run and might be automatically synchronized to cloud (like iCloud on iOS)
        /// Ideal for sensitive things (as *should be* encrypted)
        /// </summary>
        Secure,
        /// <summary>
        /// By default IPersistedCache (Akavache) - InMemory. This data is not preserved between app run.
        /// Ideal for things that should be cached only during one app session
        /// </summary>
        InMemory
    }
}