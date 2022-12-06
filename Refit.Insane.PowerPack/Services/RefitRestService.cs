using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Data;
using System.Net;
using System.Runtime.CompilerServices;
using Refit.Insane.PowerPack.Attributes;
using Refit.Insane.PowerPack.Caching;
using Refit.Insane.PowerPack.Configuration;

namespace Refit.Insane.PowerPack.Services
{
	public class RefitRestService : IRestService
    {
	    private readonly ConcurrentDictionary<Type, Func<DelegatingHandler>> _handlerFactories;
	    private readonly ConcurrentDictionary<Type, DelegatingHandler> _handlerImplementations;
	    private readonly ConcurrentDictionary<Type, object> _implementations = new ConcurrentDictionary<Type, object>();
	    private readonly RefitSettings _refitSettings;

	    public RefitRestService(RefitSettings refitSettings) : this()
	    {
		    _refitSettings = refitSettings;
	    }
	    
	    public RefitRestService()
	    {
		    _handlerImplementations = new ConcurrentDictionary<Type, DelegatingHandler>();
		    _handlerFactories = new ConcurrentDictionary<Type, Func<DelegatingHandler>>();
	    }

	    public RefitRestService(IReadOnlyDictionary<Type, DelegatingHandler> handlerImplementations, RefitSettings refitSettings) : this(handlerImplementations)
	    {
		    _refitSettings = refitSettings;
	    }
		
	    public RefitRestService(IReadOnlyDictionary<Type, Func<DelegatingHandler>> handlerFactories, RefitSettings refitSettings) : this(handlerFactories)
	    {
		    _handlerImplementations = new ConcurrentDictionary<Type, DelegatingHandler>();
		    _handlerFactories = new ConcurrentDictionary<Type, Func<DelegatingHandler>>(handlerFactories);
		    _refitSettings = refitSettings;
	    }

	    public RefitRestService(IReadOnlyDictionary<Type, DelegatingHandler> handlerImplementations)
	    {
		    _handlerImplementations = new ConcurrentDictionary<Type, DelegatingHandler>(handlerImplementations);
		    _handlerFactories = new ConcurrentDictionary<Type, Func<DelegatingHandler>>();
	    }
		
	    public RefitRestService(IReadOnlyDictionary<Type, Func<DelegatingHandler>> handlerFactories)
	    {
		    _handlerImplementations = new ConcurrentDictionary<Type, DelegatingHandler>();
		    _handlerFactories = new ConcurrentDictionary<Type, Func<DelegatingHandler>>(handlerFactories);
	    }

		public async Task<Response<TResult>> Execute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> executeApiMethod, RefitCacheBehaviour cacheBehaviour = RefitCacheBehaviour.Default)
		{
			var restApi = GetRestApiImplementation<TApi>();

			try
			{
				var responseData = await executeApiMethod.Compile()(restApi).ConfigureAwait(false);
				return new Response<TResult>(responseData);
			}
			catch (ApiException refitApiException)
			{
				if (await CanPrepareResponse(refitApiException))
					return await GetResponse<TResult>(refitApiException);

				throw;
			}
		}

		public Task<Response<TResult>> Execute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> executeApiMethod,
			Func<TimeSpan?, RefitCacheBehaviour> controlCacheBehaviourBasedOnTimeSpanBetweenLastCacheUpdate) => Execute(executeApiMethod, RefitCacheBehaviour.Default);


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
				if (await CanPrepareResponse(refitApiException))
					return await GetResponse(refitApiException);

				throw;
			}
		}
		
	    internal TApi GetRestApiImplementation<TApi>()
	    {
		    if (_implementations.ContainsKey(typeof(TApi))) 
			    return (TApi)_implementations[typeof(TApi)];
			
		    var httpClientHandlerType = ApiDefinitionAttributeExtension.GetHttpClientHandlerType<TApi>();
		    var httpClientMessageHandler = GetHandler(httpClientHandlerType);
		    
		    // note is not an issue HttpClient is not singleton, the only thing that matters is that underlying HttpClientHandler has to be resued
		    var httpClient = new HttpClient(httpClientMessageHandler) 
		    {
			    BaseAddress = ApiDefinitionAttributeExtension.GetUri<TApi>(), 
			    Timeout = ApiDefinitionAttributeExtension.GetTimeout<TApi>()
		    };
		    
		    if (httpClient.BaseAddress == null)
			    throw new InvalidOperationException($"HttpClient Base Address has not been set. " +
			                                        $"In case you are not using {nameof(ApiDefinitionAttribute)} on your interface, set global constant in:" +
			                                        $"{nameof(BaseApiConfiguration)} class.");
			    
			
		    var restApi = default(TApi); 
		    restApi = _refitSettings != null
			    ? RestService.For<TApi>(httpClient, _refitSettings)
			    : RestService.For<TApi>(httpClient);

		    if (restApi == null)
			    throw new InvalidOperationException("Could not create RestService for: " + typeof(TApi) +
			                                        ", please validate your Refit interfaces declaration.");

		    _implementations.TryAdd(typeof(TApi), restApi);
		    return restApi;
	    }

	    internal DelegatingHandler GetHandler(Type httpClientHandlerType)
	    {
		    var httpClientMessageHandler = default(DelegatingHandler);

		    if (_handlerFactories.ContainsKey(httpClientHandlerType) && !_handlerImplementations.ContainsKey(httpClientHandlerType))
		    {
			    if (_handlerFactories.TryGetValue(httpClientHandlerType, out var factory))
					_handlerImplementations.TryAdd(httpClientHandlerType, factory());
		    }
		    
		    if (!_handlerImplementations.ContainsKey(httpClientHandlerType))
		    {
			    httpClientMessageHandler = Activator.CreateInstance(httpClientHandlerType) as DelegatingHandler;
			    _handlerImplementations.TryAdd(httpClientHandlerType, httpClientMessageHandler);
		    }

		    httpClientMessageHandler = _handlerImplementations[httpClientHandlerType];
		    return httpClientMessageHandler;
	    }

        protected virtual Task<bool> CanPrepareResponse(ApiException fromApiException) => Task.FromResult(false);

        protected virtual Task<Response> GetResponse(ApiException fromApiException) {
            throw new InvalidOperationException($"If you are returning true in CanPrepareResponse method " +
                                                "you have to override GetResponse methods.");
        }

        protected virtual Task<Response<TResult>> GetResponse<TResult>(ApiException fromApiException){
			throw new InvalidOperationException($"If you are returning true in CanPrepareResponse method " +
												"you have to override GetResponse methods.");
        } 
    }
}
