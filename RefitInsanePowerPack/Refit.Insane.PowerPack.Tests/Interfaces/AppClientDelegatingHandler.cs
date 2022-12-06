namespace Refit.Insane.PowerPack.Tests.Interfaces;

public class AppClientDelegatingHandler : DelegatingHandler
{
    public AppClientDelegatingHandler() : base(new HttpClientHandler())
    {

    }
}