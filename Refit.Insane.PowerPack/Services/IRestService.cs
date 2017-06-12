using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Data;

namespace Refit.Insane.PowerPack.Services
{
	public interface IRestService
	{
		Task<Response<TResult>> Execute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> executeApiMethod);
		Task<Response> Execute<TApi>(Expression<Func<TApi, Task>> executeApiMethod);
	}
}
