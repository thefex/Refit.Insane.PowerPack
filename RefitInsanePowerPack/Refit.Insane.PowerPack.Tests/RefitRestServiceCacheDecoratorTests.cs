using System.Linq.Expressions;
using Moq;
using Refit.Insane.PowerPack.Caching;
using Refit.Insane.PowerPack.Caching.Internal;
using Refit.Insane.PowerPack.Services;

namespace Refit.Insane.PowerPack.Tests
{
	[TestFixture()]
	public class RefitRestServiceCacheDecoratorTests
    {
        RefitRestServiceCachingDecorator _sut;
        Mock<IRestService> _mockedRestService;
        Mock<IPersistedCache> _persistedCacheMock;

        public RefitRestServiceCacheDecoratorTests()
        {
        }
        
        [SetUp]
        public void Setup()
        {
            _mockedRestService = new Mock<IRestService>();
            _mockedRestService.Setup(x => x.Execute<ICacheRestMockApi, IEnumerable<string>>(
                    It.IsAny<Expression<Func<ICacheRestMockApi, System.Threading.Tasks.Task<IEnumerable<string>>>>>(), It.IsAny<RefitCacheBehaviour>()
            )).ReturnsAsync(new Refit.Insane.PowerPack.Data.Response<IEnumerable<string>>(new List<string>()));

            _persistedCacheMock = new Mock<IPersistedCache>();
            _persistedCacheMock.Setup(x => x.Get<IEnumerable<string>>(It.IsAny<RefitCacheLocation>(), It.IsAny<string>()))!.ReturnsAsync(default(IEnumerable<string>));
            _persistedCacheMock.Setup(x => x.Save<IEnumerable<string>>(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()));

            _sut = new RefitRestServiceCachingDecorator(_mockedRestService.Object, _persistedCacheMock.Object, new Refit.Insane.PowerPack.Caching.Internal.RefitCacheController());
        }

        [Test]
        public async Task Execute_RestInterfaceDoesNotHaveCacheAttribute_PersistedCacheIsNotUsed()
        {
            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.CreateNewItem("test"));

            VerifyThatCacheSaveHasBeenCalled(Times.Never());
        }

        [Test]
        public async Task Execute_RestInterfaceDoHaveCacheAttribute_PersistedCacheIsUsed()
        {
            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems());

            VerifyThatCacheSaveHasBeenCalled(Times.Once());

            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"));

            VerifyThatCacheSaveHasBeenCalled(Times.Exactly(2));
        }

        private void VerifyThatCacheSaveHasBeenCalled(Times howManyTimes){
			_persistedCacheMock.Verify(x => x.Save(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(),
												   It.IsAny<IEnumerable<string>>(),
                                                   It.IsAny<TimeSpan?>()), howManyTimes);
        }

        [Test]
        public async Task Execute_RestInterfaceDoesHaveCacheAttribute_WithDefaultNotForcedFlag_PersistedCacheIsCalledWithSaveOnce()
        {
            var persistedCacheMock = new Mock<IPersistedCache>();

            IEnumerable<string> cachedList = null;

            persistedCacheMock.Setup(x => x.Get<IEnumerable<string>>(It.IsAny<RefitCacheLocation>(), It.IsAny<string>()))!.ReturnsAsync(() => cachedList);

            var systemUnderTest = new RefitRestServiceCachingDecorator(_mockedRestService.Object,
                persistedCacheMock.Object, new RefitCacheController());
            
            await systemUnderTest.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"), RefitCacheBehaviour.Default);
            
            persistedCacheMock.Verify(x => x.Save(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()), Times.Once);

            cachedList = new List<string>();

            await systemUnderTest.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"), RefitCacheBehaviour.Default);
            
            persistedCacheMock.Verify(x => x.Save(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()), Times.Once);
        }
        
        
        [Test]
        public async Task Execute_RestInterfaceDoesHaveCacheAttribute_WithForceExecuteFlag_PersistedCacheIsCalledWithSaveTwice()
        {
            var persistedCacheMock = new Mock<IPersistedCache>();

            IEnumerable<string> cachedList = null;

            persistedCacheMock.Setup(x => x.Get<IEnumerable<string>>(It.IsAny<RefitCacheLocation>(), It.IsAny<string>()))!.ReturnsAsync(() => cachedList);

            var systemUnderTest = new RefitRestServiceCachingDecorator(_mockedRestService.Object,
                persistedCacheMock.Object, new RefitCacheController());
            
            await systemUnderTest.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"), RefitCacheBehaviour.ForceUpdateWithoutFallbackToCache);
            
            persistedCacheMock.Verify(x => x.Save(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()), Times.Once);

            cachedList = new List<string>();

            await systemUnderTest.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"), RefitCacheBehaviour.ForceUpdateWithoutFallbackToCache);
            
            persistedCacheMock.Verify(x => x.Save(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()), Times.Exactly(2));
        }
        
        [Test]
        public async Task Execute_RestInterfaceDoesHaveCacheAttribute_WhenCacheHasItem_WhenPersistedStorageReturnsTimeSpanLessThan24Hours_PersistedCacheSaveIsNotCalled()
        {
            var persistedCacheMock = new Mock<IPersistedCache>();

            persistedCacheMock.Setup(x => x.Get<IEnumerable<string>>(It.IsAny<RefitCacheLocation>(), It.IsAny<string>()))!.ReturnsAsync(new List<string>() { "test cache"});
            persistedCacheMock.Setup(x => x.GetSavedAtTime(It.IsAny<RefitCacheLocation>(), It.IsAny<string>()))
                .ReturnsAsync(() => DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(12)));

            var systemUnderTest = new RefitRestServiceCachingDecorator(_mockedRestService.Object,
                persistedCacheMock.Object, new RefitCacheController());
            
            await systemUnderTest.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"), timeSpan => (timeSpan.HasValue && timeSpan.Value.TotalHours >= 24) ? RefitCacheBehaviour.ForceUpdateWithoutFallbackToCache : RefitCacheBehaviour.Default);
            
            persistedCacheMock.Verify(x => x.Save(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()), Times.Never);
        }
        
        [Test]
        public async Task Execute_RestInterfaceDoesHaveCacheAttribute_WhenCacheHasItem_WhenPersistedStorageReturnsTimeSpanHigherThan24Hours_PersistedCacheSaveIsNotCalled()
        {
            var persistedCacheMock = new Mock<IPersistedCache>();

            persistedCacheMock.Setup(x => x.Get<IEnumerable<string>>(It.IsAny<RefitCacheLocation>(), It.IsAny<string>()))!.ReturnsAsync(new List<string>() { "test cache"});
            persistedCacheMock.Setup(x => x.GetSavedAtTime(It.IsAny<RefitCacheLocation>(), It.IsAny<string>()))
                .ReturnsAsync(() => DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(25)));

            var systemUnderTest = new RefitRestServiceCachingDecorator(_mockedRestService.Object,
                persistedCacheMock.Object, new RefitCacheController());
            
            await systemUnderTest.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"), timeSpan => (timeSpan.HasValue && timeSpan.Value.TotalHours >= 24) ? RefitCacheBehaviour.ForceUpdateWithoutFallbackToCache : RefitCacheBehaviour.Default);
            
            persistedCacheMock.Verify(x => x.Save(It.IsAny<RefitCacheLocation>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()), Times.Once);
        }
    }
}
