using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampleApp.Core.Model;
using System.Threading;
using Refit;
using Refit.Insane.PowerPack.Retry;
using Refit.Insane.PowerPack.Caching;

[assembly:RefitRetryAttribute(3)]

namespace SampleApp.Core.Rest
{
    public interface IClientApi
    {
        [Get("/api/Client")]
        [RefitCacheAttribute(10)]
        Task<IEnumerable<Client>> GetClients(CancellationToken cancellationToken = default(CancellationToken));

        [Post("/api/Client")]
        Task<Client> AddClient(CancellationToken cancellationToken = default(CancellationToken));

        [Get("/api/Client/{id}")]
        [RefitCache(20)]
        Task<ClientDetails> GetClientDetails([RefitCachePrimaryKey]string id, CancellationToken cancellationToken = default(CancellationToken));

        [Put("/api/Client/{id}")]
        Task UpdateClientDetails(string id, [Body] ClientDetails updatedDetails, CancellationToken cancellationToken = default(CancellationToken));
    }
}
