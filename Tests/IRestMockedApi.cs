using System;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Retry;

namespace Tests
{
    public interface IRestMockedApi
    {
        [RefitRetryAttribute]
        Task SampleRestMethod();

        [RefitRetryAttribute]
        Task<string> AnotherSampleRestMethod();
    }
}
