using System;
using Moq;
using NUnit.Framework;
using Refit.Insane.PowerPack.Caching;
using Refit.Insane.PowerPack.Services;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tests
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
                    It.IsAny<Expression<Func<ICacheRestMockApi, System.Threading.Tasks.Task<IEnumerable<string>>>>>()
            )).ReturnsAsync(new Refit.Insane.PowerPack.Data.Response<IEnumerable<string>>(new List<string>()));

            _persistedCacheMock = new Mock<IPersistedCache>();
            _persistedCacheMock.Setup(x => x.Get<IEnumerable<string>>(It.IsAny<string>())).ReturnsAsync(default(IEnumerable<string>));
            _persistedCacheMock.Setup(x => x.Save<IEnumerable<string>>(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan?>()));

            _sut = new RefitRestServiceCachingDecorator(_mockedRestService.Object, _persistedCacheMock.Object, new Refit.Insane.PowerPack.Caching.Internal.RefitCacheController());
        }

        [Test]
        public async void Execute_RestInterfaceDoesNotHaveCacheAttribute_PersistedCacheIsNotUsed()
        {
            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.CreateNewItem("test"));

            VerifyThatCacheSaveHasBeenCalled(Times.Never());
        }

        [Test]
        public async void Execute_RestInterfaceDoHaveCacheAttribute_PersistedCacheIsUsed()
        {
            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems());

            VerifyThatCacheSaveHasBeenCalled(Times.Once());

            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"));

            VerifyThatCacheSaveHasBeenCalled(Times.Exactly(2));
        }

        private void VerifyThatCacheGetHasBeenCalled(Times howManyTimes){
            _persistedCacheMock.Verify(x => x.Get<IEnumerable<string>>(It.IsAny<string>()));
        }

        private void VerifyThatCacheSaveHasBeenCalled(Times howManyTimes){
			_persistedCacheMock.Verify(x => x.Save(It.IsAny<string>(),
												   It.IsAny<IEnumerable<string>>(),
                                                   It.IsAny<TimeSpan?>()), howManyTimes);
        }

        [Test]
        public async void Execute_CachedMethodWithoutArgumentsIfValueGetsCached_ThenItShouldBeReturnedFromCacheUsingPersistedStorage(){
            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems());

            VerifyThatCacheGetHasBeenCalled(Times.Never());
            VerifyThatCacheSaveHasBeenCalled(Times.Once());

            await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems());

            VerifyThatCacheGetHasBeenCalled(Times.Once());
			VerifyThatCacheSaveHasBeenCalled(Times.Once());
        }

        [Test]
        public async void Execute_CachedMethodWithArgumentsIfValueGetsCached_ThenItShouldReturnedFromCacheUsingPersistedCache(){
			await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"));

			VerifyThatCacheGetHasBeenCalled(Times.Never());
			VerifyThatCacheSaveHasBeenCalled(Times.Once());

			await _sut.Execute<ICacheRestMockApi, IEnumerable<string>>(api => api.GetItems("test"));

			VerifyThatCacheGetHasBeenCalled(Times.Once());
			VerifyThatCacheSaveHasBeenCalled(Times.Once());
        }
    }
}
