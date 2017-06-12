using System;
using System.Net;

namespace Refit.Insane.PowerPack.Retry
{
	public class RefitRetryAttribute : Attribute
	{
		public RefitRetryAttribute(int retryCount = 3)
			: this(retryCount, HttpStatusCode.BadGateway, HttpStatusCode.GatewayTimeout, HttpStatusCode.InternalServerError,
				   HttpStatusCode.NotFound, HttpStatusCode.RequestTimeout, HttpStatusCode.ServiceUnavailable)
		{

		}

		public int RetryCount { get; }
		public HttpStatusCode[] RetryOnStatusCodes { get; }

		public RefitRetryAttribute(int retryCount, params HttpStatusCode[] retryOnStatusCodes)
		{
			RetryOnStatusCodes = retryOnStatusCodes;
			RetryCount = retryCount;
		}
	}
}
