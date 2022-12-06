using Refit.Insane.PowerPack.Attributes;

namespace Refit.Insane.PowerPack.Tests.Interfaces;

[ApiDefinition("https://testapi.com/api", typeof(AppClientDelegatingHandler))]
public interface ITestApiWithApiDefinition
{
    [Get("/test")]
    Task<string> GetTestData(CancellationToken cancellationToken);
}