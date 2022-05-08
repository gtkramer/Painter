using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Painter.Domain;

namespace Painter.Download {
    public sealed class ColorDownloader : IDisposable {
        private readonly HttpClient _client;

        public ColorDownloader(HttpClient client) {
            _client = client;
        }

        public IEnumerable<ColorSwatch> ParallelDownloadColors(IEnumerable<string> urls, Func<string, ColorSwatch> action) {
            ConcurrentBag<ColorSwatch> colorSwatches = new ConcurrentBag<ColorSwatch>();
            Parallel.ForEach(urls,
                // Limit parallel downloads to not overwhelm a service and cause throttling
                new ParallelOptions {MaxDegreeOfParallelism = 4},
                url => {
                    colorSwatches.Add(action(_client.GetStringAsync(url).GetAwaiter().GetResult()));
                }
            );
            return colorSwatches;
        }

        public IEnumerable<ColorSwatch> DownloadColors(string url, Func<string, IEnumerable<ColorSwatch>> action) {
            return action(_client.GetStringAsync(url).GetAwaiter().GetResult());
        }

        public void Dispose() {
            _client.Dispose();
        }
    }
}
