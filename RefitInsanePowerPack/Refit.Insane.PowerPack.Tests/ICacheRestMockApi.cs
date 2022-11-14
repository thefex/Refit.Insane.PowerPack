using Refit.Insane.PowerPack.Caching;

namespace Refit.Insane.PowerPack.Tests
{
    public interface ICacheRestMockApi
    {
        Task<IEnumerable<string>> CreateNewItem(string itemId);

        [RefitCache]
        Task<IEnumerable<string>> GetItems();

        [RefitCache]
        Task<IEnumerable<string>> GetItems([RefitCachePrimaryKey] string primaryKeyItemId);
    }
}
