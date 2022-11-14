using Refit.Insane.PowerPack.Retry;

namespace Refit.Insane.PowerPack.Tests
{
    public interface IRestMockedApi
    {
        [RefitRetry]
        Task SampleRestMethod();

        [RefitRetry]
        Task<string> AnotherSampleRestMethod();
    }
}
