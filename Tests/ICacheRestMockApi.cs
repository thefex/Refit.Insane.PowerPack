using System.Collections.Generic;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Caching;

namespace Tests
{
    public interface ICacheRestMockApi
    {
        Task<IEnumerable<string>> CreateNewItem(string itemId);

        [RefitCacheAttribute]
        Task<IEnumerable<string>> GetItems();

        [RefitCache]
        Task<IEnumerable<string>> GetItems([RefitCachePrimaryKey] string primaryKeyItemId);
    }
}
