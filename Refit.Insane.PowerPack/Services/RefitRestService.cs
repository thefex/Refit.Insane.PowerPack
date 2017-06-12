using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Data;
using System.Net;

namespace Refit.Insane.PowerPack.Services
{
    public class RefitRestService : IRestService
    {
        readonly Func<HttpClient> _httpClientFactory;

        public RefitRestService(Func<HttpClient> httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


		public async Task<Response<TResult>> Execute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> executeApiMethod)
		{
			var httpClient = _httpClientFactory();
			var restApi = RestService.For<TApi>(httpClient);

			try
			{
				var responseData = await executeApiMethod.Compile()(restApi).ConfigureAwait(false);
				return new Response<TResult>(responseData);
			}
			catch (ApiException refitApiException)
			{
				if (refitApiException.StatusCode == HttpStatusCode.Forbidden)
					return GetResponse<TResult>(refitApiException);

				throw;
			}
		}

		public async Task<Response> Execute<TApi>(Expression<Func<TApi, Task>> executeApiMethod)
		{
			var httpClient = _httpClientFactory();
			var restApi = RestService.For<TApi>(httpClient);

			try
			{
				await executeApiMethod.Compile()(restApi).ConfigureAwait(false);
				return new Response();
			}
			catch (ApiException refitApiException)
			{
				if (refitApiException.StatusCode == HttpStatusCode.Forbidden)
					return GetResponse(refitApiException);

				throw;
			}
		}

		private Response<TResult> GetResponse<TResult>(ApiException fromApiException)
		{
			try
			{
                var response = fromApiException.GetContentAs<Response<TResult>>() ?? new Response<TResult>();
				return response.SetAsFailureResponse();
			}
			catch (Exception e)
			{
				throw fromApiException;
			}
		}

		private Response GetResponse(ApiException fromApiException)
		{
			try
			{
                var response = fromApiException.GetContentAs<Response>() ?? new Response();
				return response.SetAsFailureResponse();
			}
			catch (Exception e)
			{
				throw fromApiException;
			}
		}
    }
}
