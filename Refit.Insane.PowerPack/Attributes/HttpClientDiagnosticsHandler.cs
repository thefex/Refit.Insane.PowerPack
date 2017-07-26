using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Refit.Insane.PowerPack.Attributes
{
    public class HttpClientDiagnosticsHandler : DelegatingHandler
    {
        public HttpClientDiagnosticsHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        public HttpClientDiagnosticsHandler() : base(new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage httpResponseMessage;
            Stopwatch totalElapsedTime = Stopwatch.StartNew();
            Debug.WriteLine($"Request: {request}");
            if (request?.Content != null)
            {
                string str = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                Debug.WriteLine($"Request Content: {str}");
            }
            Stopwatch responseElapsedTime = Stopwatch.StartNew();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            Debug.WriteLine($"Response: {response}");
            if (response?.Content != null)
            {
                string str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Debug.WriteLine($"Response Content: {str}");
            }
            responseElapsedTime.Stop();
            Debug.WriteLine($"Response elapsed time: {responseElapsedTime.ElapsedMilliseconds} ms");
            totalElapsedTime.Stop();
            Debug.WriteLine($"Total elapsed time: {totalElapsedTime.ElapsedMilliseconds} ms");
            httpResponseMessage = response;
            return httpResponseMessage;
        }
    }
}