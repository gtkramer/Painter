using System;
using System.Collections.Generic;
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

        protected async Task<string> ResilientDownloadAsync(string url)
        {
            Console.WriteLine($"Downloading {url}");

            HttpResponseMessage response = await _retryPolicy.ExecuteAsync(
                    async () => await _timeoutPolicy.ExecuteAsync(
                        async ct => await _httpClient.GetAsync(url, ct), CancellationToken.None
            ));

            return await response.Content.ReadAsStringAsync();
        }

        public abstract IEnumerable<string> GetUrls();
        public abstract IEnumerable<ColorSwatch> DownloadColors(string url);
    }
}
