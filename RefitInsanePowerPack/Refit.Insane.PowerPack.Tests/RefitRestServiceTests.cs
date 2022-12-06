using NuGet.Frameworks;
using NUnit.Framework.Interfaces;
using Refit.Insane.PowerPack.Attributes;
using Refit.Insane.PowerPack.Services;
using Refit.Insane.PowerPack.Tests.Interfaces;

namespace Refit.Insane.PowerPack.Tests;

[TestFixture]
public class RefitRestServiceTests
{
    [Test]
    public void GetApiImplementation_SynchronousCall_ShouldNotThrowException()
    { 
        var restService = new RefitRestService();
          
        Assert.DoesNotThrow(() =>
        {
            var restApiImplementation = restService.GetRestApiImplementation<ITestApiWithApiDefinition>();
            var restApiImplementation2 = restService.GetRestApiImplementation<ITestApiWithApiDefinition>(); 
            var restApiImplementation3 = restService.GetRestApiImplementation<ITestApiWithApiDefinition>(); 
        });
    }
    
    [Test]
    public async Task GetApiImplementation_ConcurentCall_ShouldNotThrowException()
    {
        var restService = new RefitRestService();

        List<Task> tasks = new List<Task>();
        for (int i = 0; i < 10; ++i)
        {
            tasks.Add(Task.Run(() =>
            {
                restService.GetRestApiImplementation<ITestApiWithApiDefinition>();
            }));
        }
        await Task.WhenAll(tasks);
    }

    [Test]
    public async Task GetDelegatingHandler_SynchronousCallForAppClientHandler_ShouldReturnAppClientHandler()
    {
        var restService = new RefitRestService();

        var delegatingHandler = restService.GetHandler(typeof(AppClientDelegatingHandler));
        var secondDelegatingHandler = restService.GetHandler(typeof(AppClientDelegatingHandler));
        
        Assert.IsInstanceOf(typeof(AppClientDelegatingHandler), delegatingHandler);
        Assert.IsInstanceOf(typeof(AppClientDelegatingHandler), secondDelegatingHandler);
    }
    
    [Test]
    public async Task GetDelegatingHandler_ConcurrentCallForAppClientHandler_ShouldReturnAppClientHandler()
    {
        var restService = new RefitRestService();

        List<Task> tasks = new List<Task>();
        List<DelegatingHandler> delegatingHandlers = new List<DelegatingHandler>();
        for (int i = 0; i < 10; ++i)
        {
            tasks.Add(new Task( () =>
            {
                delegatingHandlers.Add(restService.GetHandler(typeof(AppClientDelegatingHandler)));
            }));
        }
        
        foreach(var task in tasks)
            task.Start();
        
        await Task.WhenAll(tasks);

        CollectionAssert.AllItemsAreInstancesOfType(delegatingHandlers, typeof(AppClientDelegatingHandler));
        Assert.That(10, Is.EqualTo(delegatingHandlers.Count));
    }
}