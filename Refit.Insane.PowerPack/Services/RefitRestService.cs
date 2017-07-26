﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Data;
using System.Net;
using Refit.Insane.PowerPack.Attributes;

namespace Refit.Insane.PowerPack.Services
{
	public class RefitRestService : IRestService
    {
	    private readonly IDictionary<Type, Func<DelegatingHandler>> _handlerFactories;
	    private readonly IDictionary<Type, DelegatingHandler> _handlerImplementations;
	    private readonly IDictionary<Type, object> _implementations = new Dictionary<Type, object>();

	    public RefitRestService()
	    {
		    _handlerImplementations = new Dictionary<Type, DelegatingHandler>();
		    _handlerFactories = new Dictionary<Type, Func<DelegatingHandler>>();
	    }

	    public RefitRestService(IDictionary<Type, DelegatingHandler> handlerImplementations)
	    {
		    _handlerImplementations = handlerImplementations;
		    _handlerFactories = new Dictionary<Type, Func<DelegatingHandler>>();
	    }
		
	    public RefitRestService(IDictionary<Type, Func<DelegatingHandler>> handlerFactories)
	    {
		    _handlerImplementations = new Dictionary<Type, DelegatingHandler>();
		    _handlerFactories = handlerFactories;
	    }

		public async Task<Response<TResult>> Execute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> executeApiMethod)
		{
			var restApi = GetRestApiImplementation<TApi>();

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
			var restApi = GetRestApiImplementation<TApi>();

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
	    
	    private TApi GetRestApiImplementation<TApi>()
	    {
		    if (_implementations.ContainsKey(typeof(TApi))) return (TApi)_implementations[typeof(TApi)];
			
		    var httpClientHandlerType = ApiDefinitionAttributeExtension.GetHttpClientHandlerType<TApi>();
		    var httpClientMessageHandler = GetHandler(httpClientHandlerType);
		    var httpClient = new HttpClient(httpClientMessageHandler)
		    {
			    BaseAddress = ApiDefinitionAttributeExtension.GetUri<TApi>(), 
			    Timeout = ApiDefinitionAttributeExtension.GetTimeout<TApi>()
		    };
			
		    var restApi = default(TApi);
		    try
		    {
			    restApi = RestService.For<TApi>(httpClient);
			    _implementations.Add(typeof(TApi), restApi);
		    }
		    catch (Exception ex)
		    {
			    System.Diagnostics.Debug.WriteLine(ex);
		    }

		    return restApi;
	    }

	    private DelegatingHandler GetHandler(Type httpClientHandlerType)
	    {
		    var httpClientMessageHandler = default(DelegatingHandler);

		    if (_handlerFactories.ContainsKey(httpClientHandlerType) && !_handlerImplementations.ContainsKey(httpClientHandlerType))
		    {
			    var factory = _handlerFactories[httpClientHandlerType];
			    _handlerImplementations.Add(httpClientHandlerType, factory());
		    }

		    if (_handlerImplementations.ContainsKey(httpClientHandlerType))
			    httpClientMessageHandler = _handlerImplementations[httpClientHandlerType];
		    else
		    {
			    httpClientMessageHandler = Activator.CreateInstance(httpClientHandlerType) as DelegatingHandler;
			    _handlerImplementations.Add(httpClientHandlerType, httpClientMessageHandler);
		    }

		    return httpClientMessageHandler;
	    }

        protected virtual bool CanPrepareResponse(ApiException fromApiException) => false;

        protected virtual Response GetResponse(ApiException fromApiException) {
            throw new InvalidOperationException($"If you are returning true in CanPrepareResponse method " +
                                                "you have to override GetResponse methods.");
        }

        protected virtual Response<TResult> GetResponse<TResult>(ApiException fromApiException){
			throw new InvalidOperationException($"If you are returning true in CanPrepareResponse method " +
												"you have to override GetResponse methods.");
        }

		 
    }
}
