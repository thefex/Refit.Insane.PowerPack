using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Data;

namespace Refit.Insane.PowerPack.Services
{
	public interface IRestService
	{
		/// <summary>
		/// Calls Refit API. 
		/// </summary>
		/// <param name="executeApiMethod">Expression to Refit interface method.</param>
		/// <param name="forceExecuteEvenIfResponseIsInCache">When true supplied - API is called even if the value is in cache. Useful in scenario when you want to optionally force refresh - typical usecase: pull to refresh in Mobile App.</param>
		/// <typeparam name="TApi">Refit API interface</typeparam>
		/// <typeparam name="TResult">Method result type.</typeparam>
		/// <returns></returns>
		Task<Response<TResult>> Execute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> executeApiMethod, bool forceExecuteEvenIfResponseIsInCache = false);
		Task<Response> Execute<TApi>(Expression<Func<TApi, Task>> executeApiMethod);
	}
}
