using NUnit.Framework;
using System;
using Refit.Insane.PowerPack.Services;
using Moq;
using Refit;
using System.Net.Http;

namespace Tests
{
    [TestFixture()]
    public class RefitRestServiceRetryProxyTests
    {
        private RefitRestServiceRetryProxy _systemUnderTest;
        Mock<IRestService> _mockedDecoratedRestService;

        int executedRestVoidMethodCount = 0;
        int executedRestNonVoidMethodCount = 0;

        [SetUpAttribute]
        public void Setup()
        {
            executedRestVoidMethodCount = 0;
            executedRestNonVoidMethodCount = 0;

            _mockedDecoratedRestService = new Mock<IRestService>();

            _systemUnderTest = new RefitRestServiceRetryProxy(_mockedDecoratedRestService.Object, typeof(IRestMockedApi).Assembly);
        }

        [Test()]
        public async void NonVoidMethod_ExecutedAndReturnedSuccess_ItShouldBeExecutedOnce()
        {
            _mockedDecoratedRestService.Setup(x => x.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod()))
                               .Callback(() => executedRestNonVoidMethodCount++)
                               .ReturnsAsync(new Refit.Insane.PowerPack.Data.Response<string>("test"));

            await _systemUnderTest.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod());

            Assert.AreEqual(1, executedRestNonVoidMethodCount, "Method has not been executed once.");
        }

        [Test]
        public async void VoidMethod_ExecutedAndReturnedSuccess_ItShouldBeExecutedOnce()
        {
            _mockedDecoratedRestService.Setup(x => x.Execute<IRestMockedApi>(api => api.SampleRestMethod()))
                                       .Callback(() => executedRestVoidMethodCount++)
                                       .ReturnsAsync(new Refit.Insane.PowerPack.Data.Response());

            await _systemUnderTest.Execute<IRestMockedApi>(api => api.SampleRestMethod());
			Assert.AreEqual(1, executedRestVoidMethodCount, "Method has not been executed once.");
		}

        [Test]
        public async void NonVoidMethod_ExecutedWithFailure_ItShouldBeExecutedFourTimes()
        {
            var apiException = await
                ApiException.Create(new Uri("http://www.google.pl"),
                                    HttpMethod.Get,
                                    new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));

            _mockedDecoratedRestService.Setup(x => x.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod()))
                               .Callback(() => executedRestNonVoidMethodCount++)
                                       .ThrowsAsync(apiException);

            try
            {
                await _systemUnderTest.Execute<IRestMockedApi>(api => api.AnotherSampleRestMethod());
            } catch (Exception)
            {
                
            }

            Assert.AreEqual(4, executedRestNonVoidMethodCount, "Method has not been called four times (1 normal execution + 3 retry)");
        }

        [Test]
        public async void VoidMethod_ExecutedWithFailure_ItShouldBeExecutedFourTimes(){
			var apiException = await
				ApiException.Create(new Uri("http://www.google.pl"),
									HttpMethod.Get,
									new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));

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

            Assert.AreEqual(4, executedRestVoidMethodCount, "Method has not been called four times (1 normal execution + 3 retry)");
        }
    }
}
