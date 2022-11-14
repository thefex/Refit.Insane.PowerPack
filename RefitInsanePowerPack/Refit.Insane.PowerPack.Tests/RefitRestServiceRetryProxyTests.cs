using Moq;
using Refit.Insane.PowerPack.Services;

namespace Refit.Insane.PowerPack.Tests
{
    [TestFixture()]
    public class RefitRestServiceRetryProxyTests
    {
        private RefitRestServiceRetryProxy _systemUnderTest;
        Mock<IRestService> _mockedDecoratedRestService;

        int executedRestVoidMethodCount = 0;
        int executedRestNonVoidMethodCount = 0;

        [SetUp]
        public void Setup()
        {
            executedRestVoidMethodCount = 0;
            executedRestNonVoidMethodCount = 0;

            _mockedDecoratedRestService = new Mock<IRestService>();

            _systemUnderTest = new RefitRestServiceRetryProxy(_mockedDecoratedRestService.Object, typeof(IRestMockedApi).Assembly);
        }

        [Test()]
        public async Task NonVoidMethod_ExecutedAndReturnedSuccess_ItShouldBeExecutedOnce()
        {
            _mockedDecoratedRestService.Setup(x => x.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod()))
                               .Callback(() => executedRestNonVoidMethodCount++)
                               .ReturnsAsync(new Refit.Insane.PowerPack.Data.Response<string>("test"));

            await _systemUnderTest.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod());

            Assert.That(executedRestNonVoidMethodCount, Is.EqualTo(1), "Method has not been executed once.");
        }

        [Test]
        public async Task VoidMethod_ExecutedAndReturnedSuccess_ItShouldBeExecutedOnce()
        {
            _mockedDecoratedRestService.Setup(x => x.Execute<IRestMockedApi>(api => api.SampleRestMethod()))
                                       .Callback(() => executedRestVoidMethodCount++)
                                       .ReturnsAsync(new Refit.Insane.PowerPack.Data.Response());

            await _systemUnderTest.Execute<IRestMockedApi>(api => api.SampleRestMethod());
			Assert.That(executedRestVoidMethodCount, Is.EqualTo(1), "Method has not been executed once.");
		}

        [Test]
        public async Task NonVoidMethod_ExecutedWithFailure_ItShouldBeExecutedFourTimes()
        {
            var apiException = await
                ApiException.Create(new HttpRequestMessage(HttpMethod.Get, new Uri("http://www.google.pl")),
                    HttpMethod.Get, new HttpResponseMessage(System.Net.HttpStatusCode.NotFound), new RefitSettings());


            _mockedDecoratedRestService.Setup(x => x.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod()))
                               .Callback(() => executedRestNonVoidMethodCount++)
                                       .ThrowsAsync(apiException);

            try
            {
                await _systemUnderTest.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod());
            } catch (Exception)
            {
                
            }

            Assert.That(executedRestNonVoidMethodCount, Is.EqualTo(4), "Method has not been called four times (1 normal execution + 3 retry)");
        }

        [Test]
        public async Task VoidMethod_ExecutedWithFailure_ItShouldBeExecutedFourTimes(){
			var apiException = await
				ApiException.Create(new HttpRequestMessage(HttpMethod.Get, new Uri("http://www.google.pl")),
                    HttpMethod.Get, new HttpResponseMessage(System.Net.HttpStatusCode.NotFound), new RefitSettings());

            _mockedDecoratedRestService.Setup(x => x.Execute<IRestMockedApi>(api => api.SampleRestMethod()))
							   .Callback(() => executedRestVoidMethodCount++)
									   .ThrowsAsync(apiException);

			try
			{
                await _systemUnderTest.Execute<IRestMockedApi>(api => api.SampleRestMethod());
			}
			catch (Exception)
			{

			}

            Assert.That(executedRestVoidMethodCount, Is.EqualTo(4), "Method has not been called four times (1 normal execution + 3 retry)");
        }
    }
}
