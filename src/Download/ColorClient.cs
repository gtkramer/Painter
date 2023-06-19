using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Painter.Domain;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using Polly.Timeout;

namespace Painter.Download {
    public abstract class ColorClient : IColorClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy<HttpResponseMessage> _timeoutPolicy;

        public ColorClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5),
                (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry count: {retryCount}. Exception: {exception.Message}");
                    }
            );
            _timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(60));
        }

        protected async Task<string?> ResilientDownloadAsync(string url)
        {
            Console.WriteLine($"Downloading {url}");
            try {
                HttpResponseMessage response = await _retryPolicy.ExecuteAsync(
                        async () => await _timeoutPolicy.ExecuteAsync(
                            async ct => await _httpClient.GetAsync(url, ct), CancellationToken.None
                ));

                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new HttpRequestException($"Received non-OK status code {response.StatusCode} for {url}");
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex) when (ex is TimeoutRejectedException || ex is HttpRequestException) {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        public abstract IEnumerable<string> GetUrls();
        public abstract void PopulateColors(string url, ConcurrentBag<ColorSwatch> colorSwatches);
    }
}
